using System.Text.Json.Serialization;

namespace MetaExchange.Domain
{
    [method: JsonConstructor]
    public sealed class Order(
        Guid? id,
        DateTime time,
        OrderType type,
        string kind,
        decimal amount,
        decimal price)
    {
        public Guid? Id { get; } = id;
        public DateTime Time { get; } = time;
        public OrderType Type { get; } = type;
        public string Kind { get; } = kind;
        public decimal Amount { get; } = amount;
        public decimal Price { get; } = price;
    }
}