using MetaExchange.Domain;

namespace MetaExchange.Application.Interfaces.DataSource
{
    public interface IOrderBookLoader
    {
        Task<List<OrderBook>> LoadOrderBooksAsync(string path);
    }
}