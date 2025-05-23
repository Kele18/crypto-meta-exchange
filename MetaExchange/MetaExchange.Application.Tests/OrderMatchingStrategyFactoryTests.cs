using FluentAssertions;
using MetaExchange.Application.Interfaces.Strategies;
using MetaExchange.Application.Services;
using MetaExchange.Domain;
using Xunit;

namespace MetaExchange.Application.Tests
{
    public class OrderMatchingStrategyFactoryTests
    {
        private readonly IOrderMatchingStrategyFactory sut;

        public OrderMatchingStrategyFactoryTests()
        {
            sut = new OrderMatchingStrategyFactory();
        }

        [Fact]
        public void Create_BuyOrderType_ReturnsBuyOrderMatchingStrategy()
        {
            var strategy = sut.Create(OrderType.Buy);

            strategy.Should().BeOfType<BuyOrderMatchingStrategy>();
        }

        [Fact]
        public void Create_SellOrderType_ReturnsSellOrderMatchingStrategy()
        {
            var strategy = sut.Create(OrderType.Sell);

            strategy.Should().BeOfType<SellOrderMatchingStrategy>();
        }

        [Fact]
        public void Create_InvalidOrderType_ThrowsArgumentException()
        {
            var invalidType = (OrderType)999;

            Action act = () => sut.Create(invalidType);

            act.Should().Throw<ArgumentException>()
               .WithMessage("No strategy for order type*");
        }
    }
}