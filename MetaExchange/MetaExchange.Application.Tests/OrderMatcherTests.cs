using FluentAssertions;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Application.Services;
using MetaExchange.Domain;
using Moq;
using Xunit;

namespace MetaExchange.Application.Tests
{
    public class OrderMatcherTests : OrderMatcherDriver
    {
        [Fact]
        public void MatchOrders_BuyOrderType_UsesBuyStrategy()
        {
            var result = Sut.MatchOrders(OrderBooks, Type, TargetAmount);

            result.Should().BeEquivalentTo(ExpectedResult);
        }
    }

    public class OrderMatcherDriver
    {
        private readonly Mock<IOrderMatchingStrategyFactory> _orderMatchFactory;
        private readonly Mock<IOrderMatchingStrategy> _matchingStrategy;

        public List<OrderBook> OrderBooks { get; }

        public OrderType Type { get; } = OrderType.Buy;
        public decimal TargetAmount { get; }
        public List<MatchedOrder> ExpectedResult { get; }

        public IOrderMatcher Sut { get; }

        public OrderMatcherDriver()
        {
            TargetAmount = 1.0m;
            _matchingStrategy = new Mock<IOrderMatchingStrategy>();

            _orderMatchFactory = new Mock<IOrderMatchingStrategyFactory>();
            _orderMatchFactory.Setup(i => i.Create(Type)).Returns(_matchingStrategy.Object);

            OrderBooks =
            [
                new OrderBook
                {
                    ExchangeName = "TestExchange",
                    AvailableBtc = 1.0m,
                    AvailableEur = 1000,
                    BidsRaw = [],
                    AsksRaw = []
                }
            ];

            var order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Buy, "limit", 1.0m, 3000);

            ExpectedResult = [new("TestExchange", order, 1.0m)];
            _matchingStrategy.Setup(i => i.Match(OrderBooks, TargetAmount)).Returns(ExpectedResult);

            Sut = new OrderMatcher(_orderMatchFactory.Object);
        }
    }
}