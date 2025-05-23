namespace MetaExchange.Application.DTOs
{
    public class OrderResponse
    {
        public decimal TotalEur { get; set; }
        public decimal TotalBtc { get; set; }
        public List<MatchedOrder> Orders { get; set; } = [];
    }
}