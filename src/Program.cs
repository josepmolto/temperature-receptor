using MQTTnet;
using MQTTnet.Client;
using Temperature.Receiver.Model;

var mqttFactory = new MqttFactory();
var decoder = new Decoder();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
    var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost").Build();

    mqttClient.ApplicationMessageReceivedAsync += async e =>
    {
        Console.WriteLine("Received application message.");

        var message = e.ApplicationMessage.ConvertPayloadToString();
        Console.WriteLine(message);

        var weatherInformation = await decoder.DecodeMessageAsync(e.ApplicationMessage.PayloadSegment);

        Console.WriteLine($"Temperature: {weatherInformation.Temperature}");

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

    Console.WriteLine("MQTT client subscribed to topic.");

    Console.WriteLine("Press enter to exit.");
    Console.ReadLine();
}