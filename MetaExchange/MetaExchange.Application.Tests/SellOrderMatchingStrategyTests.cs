using FluentAssertions;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Application.Services;
using MetaExchange.Domain;
using Xunit;

namespace MetaExchange.Application.Tests
{
    public class SellOrderMatchingStrategyTests : SellOrderMatchingStrategyDriver
    {
        [Fact]
        public void Match_NoBooks_ReturnsEmptyList()
        {
            const decimal targetAmount = 1.0m;

            var result = Sut.Match([], targetAmount);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Match_NoViableCandidates_ReturnsEmptyList()
        {
            SetupNoViableCandidate();
            const decimal targetAmount = 1.0m;

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Match_ExactMatchSingleExchange_ReturnsExpectedMatch()
        {
            const decimal targetAmount = 1.0m;
            SetupExactSingleMatch();

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEquivalentTo(ExpectedResult);
        }

        [Fact]
        public void Match_NotEnoughBTC_ReturnsEmpty()
        {
            const decimal targetAmount = 1.0m;
            SetupPartialMatchDueToBtcLimit();

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Match_MultipleBooks_PrioritizesHighestPrice()
        {
            SetupMultipleCandidates();
            const decimal targetAmount = 1.0m;

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEquivalentTo(ExpectedResult);
        }

        [Fact]
        public void Match_MultipleBooks_CombinesMultipleToReachTarget()
        {
            SetupMultipleOrders();
            const decimal targetAmount = 1.0m;

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEquivalentTo(ExpectedResult);
        }

        [Fact]
        public void Match_InsufficientAvailableAmount_ReturnsEmpty()
        {
            SetupInsufficientAvailableAmount();
            const decimal targetAmount = 1.0m;

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEmpty();
        }
    }

    public class SellOrderMatchingStrategyDriver
    {
        public IOrderMatchingStrategy Sut { get; }

        public List<OrderBook> Books { get; private set; }
        public List<MatchedOrder> ExpectedResult { get; private set; }

        public SellOrderMatchingStrategyDriver()
        {
            Books = [];
            ExpectedResult = [];
            Sut = new SellOrderMatchingStrategy();
        }

        public void SetupNoViableCandidate()
        {
            var order = CreateOrder(0.5m, 3000, OrderType.Buy);
            Books = [
                new OrderBook
                {
                    ExchangeName = "ExchangeA",
                    AvailableBtc = 0,
                    BidsRaw = [new OrderWrapper { Order = order }],
                    AsksRaw = []
                }
            ];
        }

        public void SetupExactSingleMatch()
        {
            var order = CreateOrder(1.0m, 3000, OrderType.Buy);
            Books = [
                new OrderBook
                {
                    ExchangeName = "ExchangeA",
                    AvailableBtc = 1.0m,
                    BidsRaw = [new OrderWrapper { Order = order }],
                    AsksRaw = []
                }
            ];

            ExpectedResult = [new MatchedOrder("ExchangeA", order, 1.0m)];
        }

        public void SetupPartialMatchDueToBtcLimit()
        {
            var order = CreateOrder(1.0m, 3000, OrderType.Buy);
            Books = [
                new OrderBook
                {
                    ExchangeName = "ExchangeA",
                    AvailableBtc = 0.5m,
                    BidsRaw = [new OrderWrapper { Order = order }],
                    AsksRaw = []
                }
            ];
        }

        public void SetupMultipleCandidates()
        {
            var highPrice = CreateOrder(1.0m, 3100, OrderType.Buy);
            var lowPrice = CreateOrder(1.0m, 2900, OrderType.Buy);

            Books = [
                new OrderBook
                {
                    ExchangeName = "High",
                    AvailableBtc = 1.0m,
                    BidsRaw = [new OrderWrapper { Order = highPrice }],
                    AsksRaw = []
                },
                new OrderBook
                {
                    ExchangeName = "Low",
                    AvailableBtc = 1.0m,
                    BidsRaw = [new OrderWrapper { Order = lowPrice }],
                    AsksRaw = []
                }
            ];

            ExpectedResult = [new MatchedOrder("High", highPrice, 1.0m)];
        }

        public void SetupMultipleOrders()
        {
            var order1 = CreateOrder(0.5m, 3100, OrderType.Buy);
            var order2 = CreateOrder(0.5m, 3050, OrderType.Buy);

            Books = [
                new OrderBook
                {
                    ExchangeName = "Ex1",
                    AvailableBtc = 0.5m,
                    BidsRaw = [new OrderWrapper { Order = order1 }],
                    AsksRaw = []
                },
                new OrderBook
                {
                    ExchangeName = "Ex2",
                    AvailableBtc = 0.5m,
                    BidsRaw = [new OrderWrapper { Order = order2 }],
                    AsksRaw = []
                }
            ];

            ExpectedResult = [
                new MatchedOrder("Ex1", order1, 0.5m),
                new MatchedOrder("Ex2", order2, 0.5m)
            ];
        }

        public void SetupInsufficientAvailableAmount()
        {
            var order = CreateOrder(0.5m, 3000, OrderType.Buy);
            Books = [
                new OrderBook
                {
                    ExchangeName = "Ex1",
                    AvailableBtc = 0.5m,
                    BidsRaw = [new OrderWrapper { Order = order }],
                    AsksRaw = []
                }
            ];
        }

        private Order CreateOrder(decimal amount, decimal price, OrderType type)
        {
            return new Order(Guid.NewGuid(), DateTime.UtcNow, type, "limit", amount, price);
        }
    }
}