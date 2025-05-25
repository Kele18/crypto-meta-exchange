using MetaExchange.Application.DTOs;
using MetaExchange.Domain;

namespace MetaExchangeE2ETests
{
    public static class OrderResponseFactory
    {
        public static OrderResponse CreateExpectedBuyResponse()
        {
            var order1 = new Order(null, DateTime.MinValue, OrderType.Sell, "Limit", 0.405m, 2964.29m);
            var order2 = new Order(null, DateTime.MinValue, OrderType.Sell, "Limit", 0.405m, 2964.29m);
            var order3 = new Order(null, DateTime.MinValue, OrderType.Sell, "Limit", 0.405m, 2964.29m);

            var matches = new List<MatchedOrder>
        {
            new("Exchange_1", order1, 0.405m),
            new("Exchange_2", order2, 0.405m),
            new("Exchange_3", order3, 0.190m),
        };

            return new OrderResponse(
                TotalEur: matches.Sum(x => x.Order.Price * x.UsedAmount),
                TotalBtc: matches.Sum(x => x.UsedAmount),
                Orders: matches
            );
        }

        public static OrderResponse CreateExpectedSellResponse()
        {
            var order1 = new Order(Guid.Empty, DateTime.MinValue, OrderType.Buy, "Limit", 0.01m, 2960.67m);
            var order2 = new Order(Guid.Empty, DateTime.MinValue, OrderType.Buy, "Limit", 1.11117578m, 2960.65m);

            var matches = new List<MatchedOrder>
        {
            new("Exchange_2", order1, 0.01m),
            new("Exchange_2", order2, 0.49m),
        };

            return new OrderResponse(
                TotalEur: matches.Sum(x => x.Order.Price * x.UsedAmount),
                TotalBtc: matches.Sum(x => x.UsedAmount),
                Orders: matches
            );
        }
    }
}