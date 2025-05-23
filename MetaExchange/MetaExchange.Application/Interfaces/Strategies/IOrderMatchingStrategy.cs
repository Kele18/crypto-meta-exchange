using MetaExchange.Application.DTOs;
using MetaExchange.Domain;

namespace MetaExchange.Application.Interfaces.Strategies
{
    public interface IOrderMatchingStrategy
    {
        List<MatchedOrder> Match(List<OrderBook> books, decimal targetAmount);
    }
}