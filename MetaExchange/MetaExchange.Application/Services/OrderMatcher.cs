using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class OrderMatcher : IOrderMatcher
    {
        public List<(string exchange, Order order, decimal usedAmount)> MatchOrders(List<OrderBook> books, OrderType type, decimal targetAmount)
        {
            var results = new List<(string, Order, decimal)>();
            var remaining = targetAmount;

            var allOrders = new List<(string exchange, Order order, decimal maxAvailable)>();

            foreach (var book in books)
            {
                var orders = type == OrderType.Buy ? book.Asks : book.Bids;

                foreach (var order in orders)
                {
                    var maxAmount = type switch
                    {
                        OrderType.Buy => Math.Min(order.Amount, book.AvailableEur / order.Price),
                        OrderType.Sell => Math.Min(order.Amount, book.AvailableBtc),
                        _ => 0
                    };

                    if (maxAmount > 0)
                        allOrders.Add((book.ExchangeName, order, maxAmount));
                }
            }

            allOrders = type == OrderType.Buy
                ? allOrders.OrderBy(o => o.order.Price).ToList()
                : allOrders.OrderByDescending(o => o.order.Price).ToList();

            foreach (var (exchange, order, maxAvailable) in allOrders)
            {
                if (remaining <= 0) break;

                var toUse = Math.Min(remaining, maxAvailable);
                results.Add((exchange, order, toUse));
                remaining -= toUse;
            }

            return remaining > 0 ? [] : results;
        }
    }
}