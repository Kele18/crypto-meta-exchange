using FluentAssertions;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.Application.Interfaces.UseCase;
using MetaExchange.Application.Services.UseCase;
using MetaExchange.Domain;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace MetaExchange.Application.Tests
{
    public class OrderMatchingServiceTests : OrderMatchingServiceDriver
    {
        [Fact]
        public async Task ExecuteAsync_OneBestMatch_ReturnOrderResponse()
        {
            var result = await Sut.ExecuteAsync(Request);

            result.Should().BeEquivalentTo(Response);
        }

        [Fact]
        public async Task ExecuteAsync_NoMatches_ReturnEmptyResult()
        {
            SetupNoMatcheOders();

            var result = await Sut.ExecuteAsync(Request);

            result.Should().BeEquivalentTo(Response);
        }

        [Fact]
        public async Task ExecuteAsync_LoaderThrowsException_Throws()
        {
            LoaderThrowsError();

            Func<Task> act = () => Sut.ExecuteAsync(Request);

            await act.Should().ThrowAsync<IOException>()
                .WithMessage("Loader failed");
            VerifyMatcherNotCalled();
        }
    }

    public class OrderMatchingServiceDriver
    {
        private readonly Mock<IOrderBookLoader> _orderBookLoader;
        private readonly Mock<IOrderMatcher> _orderMatcher;
        private readonly Mock<IConfiguration> _config;

        private readonly List<OrderBook> _orderBooks;
        private readonly Order _order;
        private readonly List<MatchedOrder> _matchedOrders;
        private readonly decimal _targetAmount;
        private readonly OrderType _type;
        private readonly string _filePath;

        public OrderRequest Request { get; }

        public OrderResponse Response { get; private set; }

        public IOrderMatchingService Sut { get; }

        public OrderMatchingServiceDriver()
        {
            _targetAmount = 1m;
            _type = OrderType.Sell;

            _filePath = "filePath";
            _config = new Mock<IConfiguration>();
            _config.Setup(i => i["AppConfig:OrderBookPath"]).Returns(_filePath);

            _orderBooks = [];
            _orderBookLoader = new Mock<IOrderBookLoader>();
            _orderBookLoader.Setup(i => i.LoadOrderBooksAsync(It.IsAny<string>())).ReturnsAsync(_orderBooks);

            _order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 1.0m, 3000);
            _matchedOrders =
            [
                new MatchedOrder("Exchange1", _order, 2000m)
            ];
            _orderMatcher = new Mock<IOrderMatcher>();
            _orderMatcher.Setup(i => i.MatchOrders(_orderBooks, _type, _targetAmount)).Returns(_matchedOrders);

            Request = new OrderRequest { Amount = _targetAmount, Type = _type };
            Response = new OrderResponse(6000000m, 2000m, _matchedOrders);

            Sut = new OrderMatchingService(_orderBookLoader.Object, _orderMatcher.Object, _config.Object);
        }

        public void SetupNoMatcheOders()
        {
            _orderMatcher.Setup(i => i.MatchOrders(_orderBooks, _type, _targetAmount)).Returns([]);

            Response = OrderResponse.Empty;
        }

        public void LoaderThrowsError()
        {
            _orderBookLoader
                .Setup(i => i.LoadOrderBooksAsync(It.IsAny<string>()))
                .ThrowsAsync(new IOException("Loader failed"));
        }

        public void VerifyMatcherNotCalled()
        {
            _orderMatcher.Verify(i => i.MatchOrders(It.IsAny<List<OrderBook>>(), It.IsAny<OrderType>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}