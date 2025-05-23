using FluentAssertions;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Domain;
using System.Text.Json;
using Xunit;

namespace MetaExchange.Infrastructure.Tests
{
    public class JsonBalanceProviderTests : JsonBalanceProviderDriver
    {
        [Fact]
        public void GetBalance_ExistingExchange_ReturnsCorrectBalance()
        {
            var result = Sut.GetBalance("Exchange1");

            result.Should().BeEquivalentTo(ExpectedResult);
        }

        [Fact]
        public void GetBalance_NonExistingExchange_ReturnsZeroBalance()
        {
            var result = Sut.GetBalance("UnknownExchange");

            result.AvailableEur.Should().Be(0);
            result.AvailableBtc.Should().Be(0);
        }

        [Fact]
        public void Constructor_EmptyFile_ThrowsJsonException()
        {
            File.WriteAllText(BalanceFilePath, "");

            Action act = () => new JsonBalanceProvider(BalanceFilePath);

            act.Should().Throw<JsonException>();
        }

        [Fact]
        public void Constructor_InvalidJson_ThrowsJsonException()
        {
            File.WriteAllText(BalanceFilePath, "{ this is not valid JSON }");

            Action act = () => new JsonBalanceProvider(BalanceFilePath);

            act.Should().Throw<JsonException>();
        }

        [Fact]
        public void Constructor_EmptyJsonDictionary_ShouldWork()
        {
            WriteBalanceFile(new Dictionary<string, Balance>());

            var result = Sut.GetBalance("AnyExchange");

            result.AvailableEur.Should().Be(0);
            result.AvailableBtc.Should().Be(0);
        }
    }

    public class JsonBalanceProviderDriver : IDisposable
    {
        public string BalanceFilePath { get; }

        private readonly Dictionary<string, Balance> _balance;

        public Balance ExpectedResult { get; }
        public IBalanceProvider Sut { get; }

        public JsonBalanceProviderDriver()
        {
            BalanceFilePath = Path.GetTempFileName();

            _balance = new Dictionary<string, Balance>
            {
                { "Exchange1", new Balance { AvailableEur = 1000, AvailableBtc = 1.23m } }
            };

            var json = JsonSerializer.Serialize(_balance, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(BalanceFilePath, json);

            ExpectedResult = new Balance
            {
                AvailableBtc = 1.23m,
                AvailableEur = 1000
            };

            Sut = new JsonBalanceProvider(BalanceFilePath);
        }

        public void WriteBalanceFile(Dictionary<string, Balance> balances)
        {
            var json = JsonSerializer.Serialize(balances, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(BalanceFilePath, json);
        }

        public void Dispose()
        {
            if (File.Exists(BalanceFilePath))
                File.Delete(BalanceFilePath);
        }
    }
}