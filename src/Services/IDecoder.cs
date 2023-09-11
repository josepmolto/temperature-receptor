namespace Temperature.Receiver.Model;
public interface IDecoder
{
    Task<WeatherInformation> DecodeMessageAsync(ArraySegment<byte> message);
}