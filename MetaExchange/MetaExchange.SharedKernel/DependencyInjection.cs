using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Application.Services;
using MetaExchange.ConsoleApp.Core.Config;
using MetaExchange.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MetaExchange.SharedKernel
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMetaExchangeServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<AppConfig>(config.GetSection(nameof(AppConfig)));

            services.AddScoped<IOrderMatcher, OrderMatcher>();
            services.AddScoped<IOrderBookLoader, OrderBookLoader>();
            services.AddScoped<IOrderMatchingStrategyFactory, OrderMatchingStrategyFactory>();
            services.AddScoped<IBalanceProvider>(sp =>
            {
                var cfg = sp.GetRequiredService<IOptions<AppConfig>>().Value;
                return new JsonBalanceProvider(cfg.BalancePath);
            });

            return services;
        }
    }
}