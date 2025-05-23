using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class OrderMatcher(IOrderMatchingStrategyFactory factory) : IOrderMatcher
    {
        public List<MatchedOrder> MatchOrders(List<OrderBook> books, OrderType type, decimal targetAmount)
        {
            var strategy = factory.Create(type);
            return strategy.Match(books, targetAmount);
        }
    }
}