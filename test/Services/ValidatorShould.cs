using FluentAssertions;
using Temperature.Receiver.Dto;
using Temperature.Receiver.Services;

namespace Tests.Temperature.Receiver;
public class ValidatorShould
{
    [Fact]
    public void Return_invalid_when_is_not_a_weather_station_message()
    {
        var message = new Message()
        {
            Time = "2023-11-02 17:45:07",
            Rows = new[]
            {
                new Row()
                {
                    Data = "ab553255555545ba9b1e4c0a7fffef6d8000000000"
                }
            }
        };
        var validator = new Validator();

        var isValid = validator.IsValidWeatherStationMessage(message);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void Return_valid_when_is_a_weather_station_message()
    {
        var message = new Message()
        {
            Time = "2023-11-02 17:45:07",
            Rows = new[]
            {
                new Row()
                {
                    Data = "ab5555555555545ba9b1e4c0a7fffef6d8000000000"
                }
            }
        };
        var validator = new Validator();

        var isValid = validator.IsValidWeatherStationMessage(message);

        isValid.Should().BeTrue();
    }
}