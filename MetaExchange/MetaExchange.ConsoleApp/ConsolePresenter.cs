using MetaExchange.Application.DTOs;
using MetaExchange.Application.Interfaces;
using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.ConsoleApp.Core.Config;
using MetaExchange.Domain;
using Microsoft.Extensions.Options;

namespace MetaExchange.ConsoleApp
{
    public class ConsolePresenter(
        IOrderBookLoader loader,
        IOrderMatcher matcher,
        IOptions<AppConfig> configOptions,
        IConsoleIO console)
    {
        private readonly string _filePath = Path.Combine(
            Path.GetFullPath(configOptions.Value.OrderBookPath)
        );

        public async Task RunAsync()
        {
            console.WriteLine("MetaExchange - BTC");

            while (true)
            {
                try
                {
                    var request = PromptOrderRequest();
                    if (request is null) break;

                    console.WriteLine("\nLoading order book...");
                    List<OrderBook> orderBooks = await loader.LoadOrderBooksAsync(_filePath);
                    List<MatchedOrder> matches = matcher.MatchOrders(orderBooks, request.Type, request.Amount);

                    if (matches.Count == 0)
                    {
                        console.WriteLine("No viable offers.");
                        continue;
                    }

                    PresentResult(matches, request.Type);
                }
                catch (Exception ex)
                {
                    console.WriteLine($"An unexpected error occurred: {ex.Message}\n");
                }
            }

            console.WriteLine("Session ended!");
        }

        private OrderRequest? PromptOrderRequest()
        {
            console.Write("Order type (Buy/Sell) or Q to quit: ");
            var input = console.ReadLine();

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                return null;

            if (!Enum.TryParse<OrderType>(input, true, out var type))
            {
                console.WriteLine("Invalid input. Please type Buy or Sell.");
                return PromptOrderRequest();
            }

            console.Write("Enter BTC amount: ");
            var amountInput = console.ReadLine();

            if (!decimal.TryParse(amountInput, out var amount) || amount <= 0)
            {
                console.WriteLine("Invalid BTC amount.");
                return PromptOrderRequest();
            }

            return new OrderRequest { Type = type, Amount = amount };
        }

        private void PresentResult(List<MatchedOrder> matches, OrderType type)
        {
            console.WriteLine($"\n{matches.Count} matched orders:");
            foreach (var match in matches)
            {
                var total = match.Order.Price * match.UsedAmount;
                console.WriteLine($"- {match.Exchange}, Price: {match.Order.Price}, Amount: {match.UsedAmount}, Total: {total:F2}");
            }

            var totalCost = matches.Sum(x => x.Order.Price * x.UsedAmount);
            var totalBtc = matches.Sum(x => x.UsedAmount);
            var label = type == OrderType.Buy ? "Cost" : "Revenue";

            console.WriteLine($"\nTotal {label}: {totalCost:F2} EUR for {totalBtc} BTC\n");
        }
    }
}