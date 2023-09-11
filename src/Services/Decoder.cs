using System.Text.Json;
using Temperature.Receiver.Dto;

namespace Temperature.Receiver.Model;
public class Decoder : IDecoder
{
    private static (int raw, int real) TemperatureReferenceValue = (112, 28);

    public Task<WeatherInformation> DecodeMessageAsync(ArraySegment<byte> messageBytes)
    {
        var message = Deserialize(messageBytes);

        var weatherInformation = new WeatherInformation()
        {
            Temperature = DecodeTemperature(message),
            Humidity = DecodeHumidity()
        };

        return Task.FromResult(weatherInformation);
    }

    private Message Deserialize(ArraySegment<byte> messageBytes)
    {
        var options = CreateJsonSerializerOptions();

        var message = JsonSerializer.Deserialize<Message>(messageBytes, CreateJsonSerializerOptions());

        return message;

        static JsonSerializerOptions CreateJsonSerializerOptions() =>
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
    }

    private float DecodeTemperature(Message message)
    {
        var temperatureHex = message.Rows.First().Data?[30..32];

        var rawDecimalTemperature = Convert.ToInt32(temperatureHex, 16);

        var difference = (TemperatureReferenceValue.raw - rawDecimalTemperature) / (float)2 / 10;

        return TemperatureReferenceValue.real - difference;
    }

    private float DecodeHumidity() =>
        60f;
}