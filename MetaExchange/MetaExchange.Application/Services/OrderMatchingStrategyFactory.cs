using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Domain;

namespace MetaExchange.Application.Services
{
    public class OrderMatchingStrategyFactory : IOrderMatchingStrategyFactory
    {
        public IOrderMatchingStrategy Create(OrderType type)
        {
            return type switch
            {
                OrderType.Buy => new BuyOrderMatchingStrategy(),
                OrderType.Sell => new SellOrderMatchingStrategy(),
                _ => throw new ArgumentException($"No strategy for order type {type}")
            };
        }
    }
}