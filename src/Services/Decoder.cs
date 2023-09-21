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
            Humidity = DecodeHumidity(message)
        };

        return Task.FromResult(weatherInformation);
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

    private static float DecodeTemperature(Message message)
    {
        var temperatureHex = message.Rows.First().Data?[30..32];

        var rawDecimalTemperature = Convert.ToInt32(temperatureHex, 16);

        var difference = (TemperatureReferenceValue.raw - rawDecimalTemperature) / (float)2 / 10;

        return TemperatureReferenceValue.real - difference;
    }

    private static float DecodeHumidity(Message message)
    {
        var humidityHex = message.Rows.First().Data?[23..25];

        var rawIntValue = Convert.ToInt32(humidityHex, 16);

        var binaryValue = Convert.ToString(rawIntValue, 2)[..^1]; //remove last character to get the humidity value

        return Convert.ToInt32(binaryValue, 2);
    }
}