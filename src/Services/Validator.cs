
using Temperature.Receiver.Dto;

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
}