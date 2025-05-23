using MetaExchange.Application.Interfaces;
using MetaExchange.Infrastructure;
using MetaExchange.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MetaExchange.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddMetaExchangeServices(context.Configuration);

                services.AddScoped<ConsolePresenter>();
                services.AddSingleton<IConsoleIO, ConsoleIO>();
            })
            .Build();

        var presenter = host.Services.GetRequiredService<ConsolePresenter>();

        await presenter.RunAsync();
    }
}