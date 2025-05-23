using MetaExchange.Application.Interfaces;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Services;
using MetaExchange.ConsoleApp.Core.Config;
using MetaExchange.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

                services.AddScoped<IOrderBookLoader, OrderBookLoader>();
                services.AddScoped<IOrderMatcher, OrderMatcher>();
                services.AddScoped<ConsolePresenter>();
                services.AddSingleton<IBalanceProvider>(provider =>
                {
                    var config = provider.GetRequiredService<IOptions<AppConfig>>().Value;
                    return new JsonBalanceProvider(config.BalancePath);
                });
                services.AddSingleton<IConsoleIO, ConsoleIO>();
            })
            .Build();

        var presenter = host.Services.GetRequiredService<ConsolePresenter>();

        await presenter.RunAsync();
    }
}