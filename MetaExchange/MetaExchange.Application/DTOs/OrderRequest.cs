using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public class OrderRequest
    {
        public OrderType Type { get; set; }
        public decimal Amount { get; set; }
    }
}