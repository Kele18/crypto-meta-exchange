using MetaExchange.Application;
using MetaExchange.Application.Services;
using MetaExchange.ConsoleApp.Core.Config;
using MetaExchange.Infrastructure;
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
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

                services.AddScoped<IOrderBookLoader, OrderBookLoader>();
                services.AddScoped<IOrderMatcher, OrderMatcher>();
                services.AddScoped<ConsolePresenter>();
            })
            .Build();

        var presenter = host.Services.GetRequiredService<ConsolePresenter>();

        await presenter.RunAsync();
    }
}