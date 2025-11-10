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
                    .Configure<QueueOptions>(context.Configuration.GetSection("Queue"))
                    .AddHttpClient<IClient, ProtobufClient>();
            })
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
            )
        .Build();
}