using System.Text.Json.Serialization;

namespace MetaExchange.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderType
    {
        Buy,
        Sell
    }
}