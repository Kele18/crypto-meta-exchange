using MetaExchange.Domain;

namespace MetaExchange.Application.Interfaces.Strategies
{
    public interface IOrderMatchingStrategyFactory
    {
        IOrderMatchingStrategy Create(OrderType type);
    }
}