using FluentAssertions;
using Temperature.Receiver.Model;

namespace Tests.Temperature.Receiver;
public class DecoderShould
{
    [Fact]
    public async Task Decode_valid_message()
    {
        var messageBytes = await File.ReadAllBytesAsync("./samples/message.json").ConfigureAwait(false);
        var expectedResponse = new WeatherInformation()
        {
            Humidity = 52,
            Temperature = 26.6f
        };
        var sut = new Decoder();

        var actualResponse = await sut.DecodeMessageAsync(messageBytes).ConfigureAwait(false);

        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
}