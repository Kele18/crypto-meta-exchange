using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.UseCase;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController(IOrderMatchingService orderMatchingService) : ControllerBase
    {
        [HttpPost("match")]
        public async Task<IActionResult> Match(OrderRequest request)
        {
            try
            {
                var result = await orderMatchingService.ExecuteAsync(request);

                if (result.Orders.Count == 0)
                    return NotFound("No viable orders found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}