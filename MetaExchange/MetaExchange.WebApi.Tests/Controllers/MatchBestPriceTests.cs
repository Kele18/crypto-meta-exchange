using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces.UseCase;
using MetaExchange.WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MetaExchange.WebApi.Tests.Controllers
{
    public class MatchBestPriceTests : MatchBestPriceDriver
    {
        [Fact]
        public async Task MatchBestPrice_Success_ReturnsOk()
        {
            IActionResult result = await Sut.MatchBestPrice(Request, CancellationToken.None);

            result.Should().BeOfType<OkObjectResult>();
            OkObjectResult? okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(Response);
        }

        [Fact]
        public async Task MatchBestPrice_NotValidRequest_ReturnBadRequest()
        {
            SetupValidationFails();

            IActionResult result = await Sut.MatchBestPrice(Request, CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
            VerifyServiceNotCalled();
        }

        [Fact]
        public async Task MatchBestPrice_EmptyOrders_ReturnNotFound()
        {
            SetupEmptyMatchOrder();

            IActionResult result = await Sut.MatchBestPrice(Request, CancellationToken.None);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task MatchBestPrice_ServiceThrows_ReturnsInternalServerError()
        {
            SetupServiceException();

            IActionResult result = await Sut.MatchBestPrice(Request, CancellationToken.None);

            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var problem = objectResult.Value as ProblemDetails;
            problem?.Title.Should().Be("Unexpected error");
        }
    }

    public class MatchBestPriceDriver
    {
        private readonly Mock<IOrderMatchingService> _orderMatchingService;
        internal readonly Mock<IValidator<OrderRequest>> _validator;
        private readonly List<MatchedOrder> _orders;

        public OrderRequest Request { get; }
        public OrderResponse Response { get; }

        public ExchangeController Sut { get; }

        public MatchBestPriceDriver()
        {
            var oder1 = new Domain.Order(Guid.NewGuid(), DateTime.Now, Domain.OrderType.Buy, "limit", 10.1m, 10000);
            var oder2 = new Domain.Order(Guid.NewGuid(), DateTime.Now.AddMinutes(-29), Domain.OrderType.Buy, "limit", 10.1m, 10000);
            _orders =
            [
                new MatchedOrder("exc1",oder1, 22.2m),
                new MatchedOrder("exc2",oder2, 22.2m),

            ];
            Request = new OrderRequest();
            Response = new OrderResponse(2443, 1, _orders);
            _orderMatchingService = new Mock<IOrderMatchingService>();
            _orderMatchingService.Setup(i => i.ExecuteAsync(Request)).ReturnsAsync(Response);

            _validator = new Mock<IValidator<OrderRequest>>();
            _validator.Setup(v => v.ValidateAsync(Request, default)).ReturnsAsync(new ValidationResult());

            Sut = new ExchangeController(_orderMatchingService.Object, _validator.Object);
        }

        public void SetupValidationFails()
        {
            _validator.Setup(v => v.ValidateAsync(Request, default)).ReturnsAsync(new ValidationResult
            {
                Errors = [new("properti1", "field 1 required")]
            });
        }

        public void SetupEmptyMatchOrder()
        {
            _orders.Clear();
        }

        public void SetupServiceException()
        {
            _orderMatchingService.Setup(i => i.ExecuteAsync(Request)).ThrowsAsync(new Exception());
        }

        public void VerifyServiceNotCalled()
        {
            _orderMatchingService.Verify(i => i.ExecuteAsync(Request), Times.Never());
        }
    }
}