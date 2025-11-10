using System.Buffers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MQTTnet;
using Temperature.Receiver.Config;
using Temperature.Receiver.Dto;

namespace Temperature.Receiver.Services;

public class Worker
{
    private readonly IValidator _validator;
    private readonly IDecoder _decoder;
    private readonly IClient _client;
    private readonly MqttClientFactory _mqttClientFactory;
    private readonly QueueOptions _queueOptions;

    public Worker(
        IValidator validator,
        IDecoder decoder,
        IClient client,
        IOptions<QueueOptions> queueOptions)
    {
        _validator = validator;
        _decoder = decoder;
        _client = client;
        _mqttClientFactory = new MqttClientFactory();
        _queueOptions = queueOptions.Value;
    }

    public async Task WorkAsync()
    {
        using var mqttClient = _mqttClientFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_queueOptions.MosquittoHost).Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var message = Deserialize(e.ApplicationMessage.Payload);

            if (!_validator.IsValidWeatherStationMessage(message))
            {
                Console.WriteLine($"Message {message.Rows.First().Data} not valid");
                return;
            }

            var weatherInformation = await _decoder.DecodeMessageAsync(message);

            if (!_validator.HasValidValues(weatherInformation))
            {
                Console.WriteLine($"Message {message.Rows.First().Data} has data with wrong measurament: T:{weatherInformation.Temperature} | H:{weatherInformation.Humidity}");
                return;
            }

            await _client.SendAsync(weatherInformation).ConfigureAwait(false);
        };

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = _mqttClientFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic("rtl_433/Ecowitt");
                })
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Console.WriteLine("MQTT client subscribed to topic rtl_433/Ecowitt");

        await Task.Delay(-1).ConfigureAwait(false);
    }

    private static Message Deserialize(ReadOnlySequence<byte> messageBytes)
    {
        var options = CreateJsonSerializerOptions();

        var reader = new Utf8JsonReader(messageBytes);

        var message = JsonSerializer.Deserialize<Message>(ref reader, options);

        return message;

        static JsonSerializerOptions CreateJsonSerializerOptions() =>
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
    }
}