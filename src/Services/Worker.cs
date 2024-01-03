using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using Temperature.Receiver.Dto;

namespace Temperature.Receiver.Services;
public class Worker
{
    private readonly IValidator _validator;
    private readonly IDecoder _decoder;
    private readonly IClient _client;
    private readonly MqttFactory _mqttFactory;

    public Worker(IValidator validator, IDecoder decoder, IClient client)
    {
        _validator = validator;
        _decoder = decoder;
        _client = client;
        _mqttFactory = new MqttFactory();
    }

    public async Task WorkAsync()
    {
        using var mqttClient = _mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("mosquitto").Build();

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var message = Deserialize(e.ApplicationMessage.PayloadSegment);

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

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
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

    private static Message Deserialize(ArraySegment<byte> messageBytes)
    {
        var options = CreateJsonSerializerOptions();

        var message = JsonSerializer.Deserialize<Message>(messageBytes, options);

        return message;

        static JsonSerializerOptions CreateJsonSerializerOptions() =>
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
    }
}