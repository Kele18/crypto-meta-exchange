namespace MetaExchange.Domain
{
    public sealed class OrderBook
    {
        public string ExchangeName { get; set; } = default!;
        public DateTime AcqTime { get; set; }
        public List<Order> Bids { get; set; } = [];
        public List<Order> Asks { get; set; } = [];
        public decimal AvailableEur { get; set; }
        public decimal AvailableBtc { get; set; }
    }
}