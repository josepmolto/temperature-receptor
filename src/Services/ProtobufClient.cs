using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Serilog;
using Temperature.Receiver.Config;
using Temperature.Receiver.Model;

namespace Temperature.Receiver.Services;
public class ProtobufClient : IClient
{
    private const string ProtobufContenType = "application/x-protobuf";
    private readonly HttpClient _httpClient;
    private readonly ClientOptions _options;
    private readonly ILogger _logger;

    public ProtobufClient(HttpClient httpClient, IOptions<ClientOptions> options, ILogger logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(WeatherInformation weatherInformation)
    {
        using var stream = new MemoryStream();

        _logger.Information($"Send {weatherInformation.Temperature} and {weatherInformation.Humidity} to {_options.BaseUrl}");

        var httpRequestMessage = CreateHttpRequestMessage(weatherInformation, stream);

        var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

        _logger.Information($"Received {httpResponseMessage.StatusCode} HTTP response");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            Console.WriteLine($"Call to {httpRequestMessage.RequestUri} responded with {httpResponseMessage.StatusCode} status code");
        }
    }

    private HttpRequestMessage CreateHttpRequestMessage(
        WeatherInformation weatherInformation,
        Stream stream)
    {
        SerializeProtobuf(weatherInformation, stream);

        var httpRequestMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = GetUri(),
            Content = new StreamContent(stream)
        };

        httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ProtobufContenType);

        return httpRequestMessage;
    }

    private static void SerializeProtobuf(
        WeatherInformation weatherInformation,
        Stream stream)
    {
        ProtoBuf.Serializer.Serialize(stream, weatherInformation);
        stream.Seek(0, SeekOrigin.Begin);
    }

    private Uri GetUri()
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var path = "receiver";

        return new Uri($"{baseUrl}/{path}");
    }
}