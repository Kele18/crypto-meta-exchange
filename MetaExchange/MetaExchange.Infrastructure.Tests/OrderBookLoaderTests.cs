using FluentAssertions;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Domain;
using Moq;
using System.Text.Json;
using Xunit;

namespace MetaExchange.Infrastructure.Tests
{
    public class OrderBookLoaderTests : OrderBookLoaderDriver
    {
        [Fact]
        public async Task LoadOrderBooksAsync_MultipleValidLines_ShouldReturnExpectedObjects()
        {
            SetupMultipleValidLinesInJson();

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().BeEquivalentTo(Expected, options =>
                options.Excluding(x => x.AcqTime));
        }

        [Fact]
        public async Task LoadOrderBooksAsync_LineWithOnlyWhitespace_ShouldIgnore()
        {
            WriteRawLine("     ");

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadOrderBooksAsync_LineWithoutJsonObject_ShouldIgnore()
        {
            WriteRawLine("timestamp_no_json_here");

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadOrderBooksAsync_SetupOneLineFailedOtherValid_ShouldIgnoreThatLineAndReturnBook()
        {
            SetupOneValidAndOneInvalidLineInJson();

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(Expected, options =>
                   options.Excluding(x => x.AcqTime));
        }

        [Fact]
        public async Task LoadOrderBooksAsync_MissingExchangeName_AssignsDefault()
        {
            var book = CreateOrderBook(null!, 0, 0);
            WriteJsonLine(book);
            SetupBalance("Exchange_1", 0, 0);

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result[0].ExchangeName.Should().Be("Exchange_1");
        }

        [Fact]
        public async Task LoadOrderBooksAsync_EmptyFile_ShouldReturnEmptyList()
        {
            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task LoadOrderBooksAsync_CacheUsedOnSecondCall()
        {
            WriteJsonLine(CreateOrderBook("CachedX", 0, 0));
            SetupBalance("CachedX", 0, 0);

            var first = await Sut.LoadOrderBooksAsync(OrderBookPath);
            var second = await Sut.LoadOrderBooksAsync(OrderBookPath);

            second.Should().BeSameAs(first);
        }

        [Fact]
        public async Task LoadOrderBooksAsync_LineWithoutJsonStart_ShouldSkipLineAndReturnOtherValidBooks()
        {
            SetupLineWothoutJsonStart();

            var result = await Sut.LoadOrderBooksAsync(OrderBookPath);

            result.Should().HaveCount(1);
            result.Should().BeEquivalentTo(Expected, options =>
                options.Excluding(x => x.AcqTime));
        }
    }

    public class OrderBookLoaderDriver : IDisposable
    {
        private readonly Mock<IBalanceProvider> _balanceProviderMock = new();
        private readonly List<string> _fileLines = new();

        public string OrderBookPath { get; }
        public List<OrderBook> Expected { get; private set; } = [];
        public IOrderBookLoader Sut { get; }

        public OrderBookLoaderDriver()
        {
            OrderBookPath = Path.GetTempFileName();

            Sut = new OrderBookLoader(_balanceProviderMock.Object);
        }

        public void SetupMultipleValidLinesInJson()
        {
            var book1 = CreateOrderBook("Exchange1", 1000, 1.0m);
            var book2 = CreateOrderBook("Exchange2", 2000, 2.0m);
            SetupBalance("Exchange1", 1000, 1.0m);
            SetupBalance("Exchange2", 2000, 2.0m);
            WriteJsonLine(book1);
            WriteJsonLine(book2);
            Expected =
             [
                 new OrderBook
                 {
                     ExchangeName = "Exchange1",
                     AvailableEur = 1000,
                     AvailableBtc = 1.0m,
                     AsksRaw = book1.AsksRaw,
                     BidsRaw = book1.BidsRaw
                 },
                 new OrderBook
                 {
                     ExchangeName = "Exchange2",
                     AvailableEur = 2000,
                     AvailableBtc = 2.0m,
                     AsksRaw = book2.AsksRaw,
                     BidsRaw = book2.BidsRaw
                 }
             ];
        }

        public void SetupOneValidAndOneInvalidLineInJson()
        {
            var book1 = CreateOrderBook("Exchange1", 1000, 1.0m);
            SetupBalance("Exchange1", 1000, 1.0m);
            WriteJsonLine(book1);
            WriteRawLine("INVALID_JSON");
            Expected =
             [
                 new OrderBook
                 {
                     ExchangeName = "Exchange1",
                     AvailableEur = 1000,
                     AvailableBtc = 1.0m,
                     AsksRaw = book1.AsksRaw,
                     BidsRaw = book1.BidsRaw
                 }
             ];
        }

        public void SetupLineWothoutJsonStart()
        {
            var book1 = CreateOrderBook("Exchange1", 1000, 1.0m);
            SetupBalance("Exchange1", 1000, 1.0m);
            WriteJsonLine(book1);

            WriteRawLine("no_json_here_just_text");

            Expected =
              [
                new OrderBook
                {
                  ExchangeName = "Exchange1",
                  AvailableEur = 1000,
                  AvailableBtc = 1.0m,
                  AsksRaw = book1.AsksRaw,
                  BidsRaw = book1.BidsRaw
                }
            ];
        }

        public void WriteJsonLine(OrderBook book)
        {
            var json = JsonSerializer.Serialize(book, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            _fileLines.Add($"1234567890	{json}");
            File.WriteAllLines(OrderBookPath, _fileLines);
        }

        public void WriteRawLine(string line)
        {
            _fileLines.Add(line);

            File.WriteAllLines(OrderBookPath, _fileLines);
        }

        public void SetupBalance(string exchange, decimal eur, decimal btc)
        {
            _balanceProviderMock.Setup(p => p.GetBalance(exchange)).Returns(new Balance { AvailableEur = eur, AvailableBtc = btc });
        }

        public static OrderBook CreateOrderBook(string exchangeName, decimal eur, decimal btc)
        {
            return new OrderBook
            {
                ExchangeName = exchangeName,
                AcqTime = DateTime.UtcNow,
                AsksRaw = [new OrderWrapper { Order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Sell, "limit", 0.5m, 3000) }],
                BidsRaw = [new OrderWrapper { Order = new Order(Guid.NewGuid(), DateTime.UtcNow, OrderType.Buy, "limit", 0.5m, 2950) }],
                AvailableEur = eur,
                AvailableBtc = btc
            };
        }

        public void Dispose()
        {
            if (File.Exists(OrderBookPath))
                File.Delete(OrderBookPath);
        }
    }
}