using Temperature.Receiver.Dto;
using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public class Decoder : IDecoder
{
    private static (int raw, float real) TemperatureReferenceValue = (118, 23f);

    public Task<WeatherInformation> DecodeMessageAsync(Message message)
    {
        var weatherInformation = new WeatherInformation()
        {
            Temperature = DecodeTemperature(message),
            Humidity = DecodeHumidity(message)
        };

        return Task.FromResult(weatherInformation);
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

    private static byte DecodeHumidity(Message message)
    {
        var humidityHex = message.Rows.First().Data?[24..26];

        var rawIntValue = Convert.ToInt32(humidityHex, 16);

        var binaryValue = Convert.ToString(rawIntValue, 2)[..^1]; //remove last character to get the humidity value

        return Convert.ToByte(binaryValue, 2);
    }
}