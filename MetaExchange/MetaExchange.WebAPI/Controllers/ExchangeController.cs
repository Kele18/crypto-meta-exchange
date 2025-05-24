using FluentValidation;
using FluentValidation.Results;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.UseCase;
using MetaExchange.WebAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeController(
    IOrderMatchingService orderMatchingService,
    IValidator<OrderRequest> validator) : ControllerBase
{
    [HttpPost("match")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MatchBestPrice([FromBody] OrderRequest request, CancellationToken cancellationToken)
    {
        ValidationResult validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.ToProblemDetails());
        }

        try
        {
            OrderResponse result = await orderMatchingService.ExecuteAsync(request);

            if (result.IsEmpty)
                return NotFound("No viable orders found.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Unexpected error",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}