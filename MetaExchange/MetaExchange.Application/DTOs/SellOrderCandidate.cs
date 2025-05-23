using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public class SellOrderCandidate(
        string exchangeName,
        Order order,
        decimal maxSellableAmount) : OrderCandidateBase(exchangeName, order, maxSellableAmount)
    {
        public decimal MaxSellableAmount => MaxAmount;
    }
}