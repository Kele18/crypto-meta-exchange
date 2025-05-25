using FluentAssertions;
using MetaExchange.Application.DTOs;
using MetaExchange.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace MetaExchangeE2ETests
{
    public class ExchangeE2ETests(WebApplicationFactory<Program> factory) : MetaExchangeBaseDriver(factory)
    {
        [Fact]
        public async Task MatchBestPrice_BuyOrder_ReturnsExpectedOrderResponse()
        {
            var expectedResult = OrderResponseFactory.CreateExpectedBuyResponse();
            var request = new OrderRequest { Type = OrderType.Buy, Amount = 1m };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await TestClient.PostAsync("/api/exchange/match", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStringAsync();
            var actualResult = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            actualResult.Should().BeEquivalentTo(expectedResult, options => options
                .Excluding(o => o.Orders[0].Order.Id)
                .Excluding(o => o.Orders[0].Order.Time)
            );
        }

        [Fact]
        public async Task SellOrder_ValidRequest_ReturnsMatchedOrders()
        {
            var expectedResult = OrderResponseFactory.CreateExpectedSellResponse();
            var request = new { type = "Sell", amount = 0.5m };

            var response = await TestClient.PostAsJsonAsync("/api/exchange/match", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync();
            var actualResult = JsonSerializer.Deserialize<OrderResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            actualResult.Should().BeEquivalentTo(expectedResult, options => options
                .Excluding(x => x.Orders[0].Order.Id)
                .Excluding(x => x.Orders[0].Order.Time)
                .WithStrictOrdering());
        }

        [Fact]
        public async Task InvalidOrderType_ReturnsBadRequest()
        {
            var request = new { type = "InvalidType", amount = 1.0m };

            var response = await TestClient.PostAsJsonAsync("/api/exchange/match", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task MissingAmount_ReturnsBadRequest()
        {
            var request = new { type = "Buy" };

            var response = await TestClient.PostAsJsonAsync("/api/exchange/match", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AmountTooLow_ReturnsValidationError()
        {
            var request = new { type = "Sell", amount = 0 };

            var response = await TestClient.PostAsJsonAsync("/api/exchange/match", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task NoMatchingOrders_ReturnsNotFound()
        {
            var request = new { type = "Buy", amount = 9999.99m };

            var response = await TestClient.PostAsJsonAsync("/api/exchange/match", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}