using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public interface IClient
{
    Task SendAsync(WeatherInformation weatherInformation);
}