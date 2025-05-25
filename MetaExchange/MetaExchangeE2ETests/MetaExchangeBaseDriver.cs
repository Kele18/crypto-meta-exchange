using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MetaExchangeE2ETests
{
    public class MetaExchangeBaseDriver(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient TestClient = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseSetting("AppConfig:OrderBookPath", "TestData/order_book_test");
                    builder.UseSetting("AppConfig:BalancePath", "TestData/balances.json");
                })
                .CreateClient();
    }
}