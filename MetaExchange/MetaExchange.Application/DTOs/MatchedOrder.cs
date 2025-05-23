using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public sealed record MatchedOrder(string Exchange, Order Order, decimal UsedAmount);
}