using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public abstract class OrderCandidateBase
    {
        protected OrderCandidateBase(string exchangeName, Order order, decimal maxAmount)
        {
            ExchangeName = exchangeName;
            Order = order;
            MaxAmount = maxAmount;
        }

        public string ExchangeName { get; }
        public Order Order { get; }
        public decimal MaxAmount { get; }

        public bool IsViable => MaxAmount > 0;
    }
}