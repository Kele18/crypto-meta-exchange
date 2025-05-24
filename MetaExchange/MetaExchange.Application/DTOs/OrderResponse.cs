namespace MetaExchange.Application.DTOs;

public sealed record OrderResponse(
    decimal TotalEur,
    decimal TotalBtc,
    IReadOnlyList<MatchedOrder> Orders
)
{
    public static readonly OrderResponse Empty = new(0, 0, []);

    public bool IsEmpty => Orders.Count == 0;
}