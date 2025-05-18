using MetaExchange.Domain;

namespace MetaExchange.Application
{
    public interface IOrderBookLoader
    {
        Task<List<OrderBook>> LoadOrderBooksAsync(string path);
    }
}