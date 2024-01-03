using Temperature.Receiver.Dto;
using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public interface IValidator
{
    bool IsValidWeatherStationMessage(Message message);

    bool HasValidValues(WeatherInformation weatherInformation);
}