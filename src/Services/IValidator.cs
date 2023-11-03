using Temperature.Receiver.Dto;

namespace Temperature.Receiver.Services;
public interface IValidator
{
    bool IsValidWeatherStationMessage(Message message);
}