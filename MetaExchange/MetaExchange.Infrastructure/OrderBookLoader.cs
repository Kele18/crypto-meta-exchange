using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Domain;
using System.Text.Json;

namespace MetaExchange.Infrastructure
{
    public class OrderBookLoader : IOrderBookLoader
    {
        private static readonly Dictionary<string, List<OrderBook>> _cache = [];
        private static readonly object _lock = new();

        public async Task<List<OrderBook>> LoadOrderBooksAsync(string path)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(path, out var cachedBooks))
                    return cachedBooks;
            }

            var orderBooks = new List<OrderBook>();
            await using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line is null) continue;

                OrderBook? book = JsonSerializer.Deserialize<OrderBook>(line, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (book != null)
                    orderBooks.Add(book);
            }

            lock (_lock)
            {
                _cache[path] = orderBooks;
            }

            return orderBooks;
        }
    }
}