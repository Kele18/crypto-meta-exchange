using FluentAssertions;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Application.Services;
using MetaExchange.Domain;
using Xunit;

namespace MetaExchange.Application.Tests
{
    public class BuyOrderMatchingStrategyTests : BuyOrderMatchingStrategyDriver
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
            const decimal targetAmount = 0.5m;

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
        public void Match_NotEnoghEur_ReturnsEmpty()
        {
            const decimal targetAmount = 1.0m;
            SetupPartialMatchDueToEurLimit();

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEquivalentTo(new List<MatchedOrder>());
        }

        [Fact]
        public void Match_MultipleBooks_PrioritizesLowestPrice()
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
            const decimal targetAmount = 1.0m;

            var result = Sut.Match(Books, targetAmount);

            result.Should().BeEmpty();
        }
    }

    public class BuyOrderMatchingStrategyDriver
    {
        public IOrderMatchingStrategy Sut { get; }

        public List<OrderBook> Books { get; private set; }

        public List<MatchedOrder> ExpectedResult { get; private set; }

        public BuyOrderMatchingStrategyDriver()
        {
            Books = [];
            ExpectedResult = [];
            Sut = new BuyOrderMatchingStrategy();
        }

        public void SetupNoViableCandidate()
        {
            var order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 0.5m, 3000);
            Books =
              [
                  new OrderBook
                  {
                      ExchangeName = "ExchangeA",
                      AvailableEur = 0,
                      AsksRaw = [new OrderWrapper { Order = order }],
                      BidsRaw = []
                  }
            ];
        }

        public void SetupExactSingleMatch()
        {
            var order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 1.0m, 3000);
            Books =
              [
                  new OrderBook
                     {
                      ExchangeName = "ExchangeA",
                      AvailableEur = 3000,
                      AsksRaw = [new OrderWrapper { Order = order }],
                      BidsRaw = []
                      }
            ];

            ExpectedResult =
            [
                new MatchedOrder("ExchangeA", order,1.0m)
            ];
        }

        public void SetupPartialMatchDueToEurLimit()
        {
            var order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 1.0m, 3000);

            Books =
            [
                new OrderBook
                 {
                  ExchangeName = "ExchangeA",
                  AvailableEur = 1500,
                  AsksRaw = [new OrderWrapper { Order = order }],
                  BidsRaw = []
                  }
            ];
        }

        public void SetupMultipleCandidates()
        {
            var lowPrice = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 1.0m, 2900);
            var highPrice = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 1.0m, 3100);

            Books =
            [
                new OrderBook
                 {
                  ExchangeName = "Low",
                  AvailableEur = 3000,
                  AsksRaw = [new OrderWrapper { Order = lowPrice }],
                  BidsRaw = []
                  },
                 new OrderBook
                 {
                  ExchangeName = "High",
                  AvailableEur = 3100,
                  AsksRaw = [new OrderWrapper { Order = highPrice }],
                  BidsRaw = []
                  }
            ];

            ExpectedResult =
              [
                new MatchedOrder("Low", lowPrice ,1.0m)
              ];
        }

        public void SetupMultipleOrders()
        {
            var order1 = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 0.5m, 2900);
            var order2 = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 0.5m, 2950);

            Books =
            [
                new OrderBook
                {
                     ExchangeName = "Ex1",
                     AvailableEur = 1450,
                     AsksRaw = [new OrderWrapper { Order = order1 }],
                     BidsRaw = []
                },
                new OrderBook
                {
                     ExchangeName = "Ex2",
                     AvailableEur = 1475,
                     AsksRaw = [new OrderWrapper { Order = order2 }],
                     BidsRaw = []
                }
            ];

            ExpectedResult =
              [

                new("Ex1", order1, 0.5m),
                new("Ex2", order2, 0.5m)

              ];
        }

        public void InsufficientAvailableAmount()
        {
            var order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 0.5m, 3000);

            Books =
           [
               new OrderBook
                {
                     ExchangeName = "Ex1",
                     AvailableEur = 1500,
                     AsksRaw = [new OrderWrapper { Order = order }],
                     BidsRaw = []
                }

           ];
        }
    }
}