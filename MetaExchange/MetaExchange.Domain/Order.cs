namespace MetaExchange.Domain
{
    public sealed class Order
    {
        public Guid? Id { get; init; }
        public DateTime Time { get; init; }
        public OrderType Type { get; init; }
        public string Kind { get; init; } = default!;
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}