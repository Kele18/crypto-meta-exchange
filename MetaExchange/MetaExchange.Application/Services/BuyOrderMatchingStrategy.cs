using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class BuyOrderMatchingStrategy : IOrderMatchingStrategy
    {
        public List<MatchedOrder> Match(List<OrderBook> books, decimal targetAmount)
        {
            var results = new List<MatchedOrder>();
            var allOrders = books
                            .SelectMany(b =>
                                  b.Asks.Select(order => (b.ExchangeName, order, MaxBuyableAmount(order, b))))
                                        .Where(x => x.Item3 > 0)
                                        .OrderBy(x => x.order.Price)
                                        .ToList();

            decimal remaining = targetAmount;
            foreach (var (exchange, order, maxAvailable) in allOrders)
            {
                if (remaining <= 0) break;
                var toUse = Math.Min(remaining, maxAvailable);
                results.Add(new MatchedOrder(exchange, order, toUse));
                remaining -= toUse;
            }

            return remaining > 0 ? [] : results;
        }

        private decimal MaxBuyableAmount(Order order, OrderBook book)
        {
            return Math.Min(order.Amount, book.AvailableEur / order.Price);
        }
    }
}