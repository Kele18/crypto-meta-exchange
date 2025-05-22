using MetaExchange.Domain;

namespace MetaExchange.Application.DTOs
{
    public record MetaOrderRequest(OrderType Type, decimal BtcAmount);
}