using FluentAssertions;
using Temperature.Receiver.Dto;
using Temperature.Receiver.Model;
using Temperature.Receiver.Services;

namespace Tests.Temperature.Receiver;

public class DecoderShould
{
    [Fact]
    public async Task Decode_valid_message()
    {
        var message = new Message()
        {
            Time = "2023-09-11 19:58:15",
            Rows = new[]
            {
                new Row()
                {
                    Data = "ab5555555555545ba9b1e5368fffff54d8000000000"
                }
            }
        };
        var expectedResponse = new WeatherInformation()
        {
            Humidity = 71,
            Temperature = 26.7f
        };
        var sut = new Decoder();

        var actualResponse = await sut.DecodeMessageAsync(message);

        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }


    [Fact]
    public async Task Decode_valid_message_with_temperature_below_10()
    {
        var message = new Message()
        {
            Time = "2023-09-11 19:58:15",
            Rows = new[]
            {
                new Row()
                {
                    Data = "ab5555555555545ba9b1e3f895fffe1ad8000000000"
                }
            }
        };
        var expectedResponse = new WeatherInformation()
        {
            Humidity = 74,
            Temperature = 10.8f
        };
        var sut = new Decoder();

        var actualResponse = await sut.DecodeMessageAsync(message);

        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}