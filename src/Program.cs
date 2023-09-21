using MQTTnet;
using MQTTnet.Client;
using Temperature.Receiver.Model;

var mqttFactory = new MqttFactory();
var decoder = new Decoder();

using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("mosquitto").Build();

mqttClient.ApplicationMessageReceivedAsync += async e =>
{
    Console.WriteLine("Received application message.");

    var message = e.ApplicationMessage.ConvertPayloadToString();
    Console.WriteLine(message);

    var weatherInformation = await decoder.DecodeMessageAsync(e.ApplicationMessage.PayloadSegment);

    Console.WriteLine($"Temperature: {weatherInformation.Temperature} Humidity: {weatherInformation.Humidity}");

};

await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
    .WithTopicFilter(
        f =>
        {
            f.WithTopic("rtl_433/Ecowitt");
        })
    .Build();

await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

Console.WriteLine("MQTT client subscribed to topic rtl_433/Ecowitt");

await Task.Delay(-1).ConfigureAwait(false);