
using Temperature.Receiver.Dto;
using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public class Validator : IValidator
{
    private const string MessageStart = "ab55555555555";
    public bool IsValidWeatherStationMessage(Message message)
    {
        if (!message.Rows.First().Data[0..13].ToLower().Equals(MessageStart))
        {
            return false;
        }

        return true;
    }
    public bool HasValidValues(WeatherInformation weatherInformation)
    {
        if (weatherInformation is { Temperature: > 45 or < -15 })
        {
            return false;
        }
        if (weatherInformation is { Humidity: > 100 or < 0 })
        {
            return false;
        }

        return true;
    }
}