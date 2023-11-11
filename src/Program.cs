using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Temperature.Receiver.Config;
using Temperature.Receiver.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHost();

        await host.StartAsync().ConfigureAwait(false);

        var worker = host.Services.GetRequiredService<Worker>();

        await worker.WorkAsync().ConfigureAwait(false);

        await host.StopAsync().ConfigureAwait(false);
    }

    private static IHost CreateHost() =>
        Host
            .CreateDefaultBuilder()
            .ConfigureHostConfiguration(c => c.AddJsonFile("./appsettings.secrets.json", optional: true))
            .ConfigureServices((context, services) =>
            {
                services
                    .AddSingleton<IDecoder, Decoder>()
                    .AddSingleton<IValidator, Validator>()
                    .AddSingleton<Worker>()
                    .Configure<ClientOptions>(context.Configuration.GetSection("Client"))
                    .AddHttpClient<IClient, ProtobufClient>(client =>
                    {
                        var token = GetBasicAuthenticationToken(context.Configuration);

                        client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
                    });
            })
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
            )
        .Build();


    private static string GetBasicAuthenticationToken(IConfiguration configuration)
    {
        var username = configuration.GetValue<string>("Client:Username");
        var password = configuration.GetValue<string>("Client:Password");

        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            var logger = Log.ForContext<Program>();
            logger.Warning("Basic Authentication is not well configured");
        }

        return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
    }
}