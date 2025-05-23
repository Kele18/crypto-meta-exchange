using System.Text.Json.Serialization;

namespace MetaExchange.Domain
{
    public sealed class OrderBook
    {
        public string ExchangeName { get; set; } = default!;
        public DateTime AcqTime { get; set; }

        [JsonPropertyName("Bids")]
        public List<OrderWrapper> BidsRaw { get; set; } = [];

        [JsonPropertyName("Asks")]
        public List<OrderWrapper> AsksRaw { get; set; } = [];

        [JsonIgnore]
        public List<Order> Bids => BidsRaw.ConvertAll(w => w.Order);

        [JsonIgnore]
        public List<Order> Asks => AsksRaw.ConvertAll(w => w.Order);

        public decimal AvailableEur { get; set; }
        public decimal AvailableBtc { get; set; }
    }
}