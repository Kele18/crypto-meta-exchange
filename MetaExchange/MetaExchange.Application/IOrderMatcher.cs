using MetaExchange.Domain;

namespace MetaExchange.Application
{
    public interface IOrderMatcher
    {
        List<(string exchange, Order order, decimal usedAmount)> MatchOrders(List<OrderBook> books, OrderType type, decimal targetAmount);
    }
}