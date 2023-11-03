using Temperature.Receiver.Dto;
using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public interface IDecoder
{
    Task<WeatherInformation> DecodeMessageAsync(Message message);
}