using MetaExchange.Domain;

namespace MetaExchange.Application.Interfaces.DataSource
{
    public interface IBalanceProvider
    {
        Balance GetBalance(string exchangeName);
    }
}