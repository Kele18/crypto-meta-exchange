using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    namespace MetaExchange.Application.Models
    {
        public class BuyOrderCandidate(
            string exchangeName,
            Order order,
            decimal maxBuyableAmount) : OrderCandidateBase(exchangeName, order, maxBuyableAmount)
        {
            public decimal MaxBuyableAmount => MaxAmount;
        }
    }
}