using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Domain;
using System.Text.Json;

namespace MetaExchange.Infrastructure
{
    public class JsonBalanceProvider : IBalanceProvider
    {
        private readonly Dictionary<string, Balance> _balances;

        public JsonBalanceProvider(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);
            _balances = JsonSerializer.Deserialize<Dictionary<string, Balance>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }

        public Balance GetBalance(string exchangeName)
        {
            return _balances.TryGetValue(exchangeName, out var balance)
                ? balance
                : new Balance { AvailableEur = 0, AvailableBtc = 0 };
        }
    }
}