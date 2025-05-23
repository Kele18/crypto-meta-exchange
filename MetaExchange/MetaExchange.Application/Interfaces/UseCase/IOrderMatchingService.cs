using MetaExchange.Application.DTOs;

namespace MetaExchange.Application.Interfaces.UseCase
{
    public interface IOrderMatchingService
    {
        Task<OrderResponse> ExecuteAsync(OrderRequest request);
    }
}