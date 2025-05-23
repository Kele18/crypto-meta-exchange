using MetaExchange.Application.DTOs;
using MetaExchange.Domain;

namespace MetaExchange.Application.Interfaces.Matcher
{
    public interface IOrderMatcher
    {
        List<MatchedOrder> MatchOrders(List<OrderBook> books, OrderType type, decimal targetAmount);
    }
}