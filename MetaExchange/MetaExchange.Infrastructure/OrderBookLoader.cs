using MetaExchange.Application;
using MetaExchange.Domain;
using System.Text.Json;

namespace MetaExchange.Infrastructure
{
    public class OrderBookLoader : IOrderBookLoader
    {
        public async Task<List<OrderBook>> LoadOrderBooksAsync(string path)
        {
            var orderBooks = new List<OrderBook>();

            using var stream = File.OpenRead(path);
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

            return orderBooks;
        }
    }
}