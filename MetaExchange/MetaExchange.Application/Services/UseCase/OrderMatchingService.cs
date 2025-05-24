using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.UseCase;
using Microsoft.Extensions.Configuration;

namespace MetaExchange.Application.Services.UseCase
{
    public class OrderMatchingService(
    IOrderBookLoader loader,
    IOrderMatcher matcher,
    IConfiguration config) : IOrderMatchingService
    {
        public async Task<OrderResponse> ExecuteAsync(OrderRequest request)
        {
            string filePath = Path.Combine(
            Path.GetFullPath(config["AppConfig:OrderBookPath"]!));

            var books = await loader.LoadOrderBooksAsync(filePath);

            var matches = matcher.MatchOrders(books, request.Type, request.Amount);

            if (matches.Count == 0)
            {
                return OrderResponse.Empty;
            }

            return new OrderResponse(
                      TotalEur: matches.Sum(x => x.Order.Price * x.UsedAmount),
                      TotalBtc: matches.Sum(x => x.UsedAmount),
                      Orders: matches);
        }
    }
}