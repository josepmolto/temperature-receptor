using System.Text.Json;
using Temperature.Receiver.Dto;

namespace Temperature.Receiver.Model;
public class Decoder : IDecoder
{
    private static (int raw, float real) TemperatureReferenceValue = (118, 23f);

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
        var temperatureHex = message.Rows.First().Data?[21..24];

        var temperatureBinary = Convert.ToString(Convert.ToInt32(temperatureHex, fromBase: 16), toBase: 2);

        //Discard the two first bits and the last bit
        var temperatureRawInteger = Convert.ToInt64(temperatureBinary[2..^1], fromBase: 2);

        var difference = ((float)temperatureRawInteger - TemperatureReferenceValue.raw) / 10;

        return TemperatureReferenceValue.real + difference;
    }

    private static float DecodeHumidity(Message message)
    {
        var humidityHex = message.Rows.First().Data?[24..26];

        var rawIntValue = Convert.ToInt32(humidityHex, 16);

        var binaryValue = Convert.ToString(rawIntValue, 2)[..^1]; //remove last character to get the humidity value

        return Convert.ToInt32(binaryValue, 2);
    }
}