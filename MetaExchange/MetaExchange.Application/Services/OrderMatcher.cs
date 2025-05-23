using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class OrderMatcher : IOrderMatcher
    {
        private readonly Dictionary<OrderType, IOrderMatchingStrategy> _strategies;

        public OrderMatcher()
        {
            _strategies = new()
            {
                [OrderType.Buy] = new BuyOrderMatchingStrategy(),
                [OrderType.Sell] = new SellOrderMatchingStrategy()
            };
        }

        public List<MatchedOrder> MatchOrders(
            List<OrderBook> books, OrderType type, decimal targetAmount)
        {
            return _strategies[type].Match(books, targetAmount);
        }
    }
}