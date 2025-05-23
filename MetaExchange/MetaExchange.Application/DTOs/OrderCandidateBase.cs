using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public abstract class OrderCandidateBase(string exchangeName, Order order, decimal maxAmount)
    {
        public string ExchangeName { get; } = exchangeName;
        public Order Order { get; } = order;
        public decimal MaxAmount { get; } = maxAmount;

        public bool IsViable => MaxAmount > 0;
    }
}