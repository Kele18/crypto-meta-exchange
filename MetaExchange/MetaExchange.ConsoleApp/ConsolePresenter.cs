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
                    console.Write("Order type (Buy/Sell) or Q to quit: ");
                    var input = console.ReadLine();
                    if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) break;

                    if (!Enum.TryParse<OrderType>(input, true, out var type))
                    {
                        console.WriteLine("Invalid input. Please type Buy or Sell.");
                        continue;
                    }

                    console.Write("Enter BTC amount: ");
                    var btcInput = console.ReadLine();
                    if (!decimal.TryParse(btcInput, out var btcAmount))
                    {
                        console.WriteLine("Invalid BTC amount.");
                        continue;
                    }

                    console.WriteLine("\nLoading order book...");
                    List<OrderBook> orderBooks = await loader.LoadOrderBooksAsync(_filePath);

                    List<MatchedOrder> matches = matcher.MatchOrders(orderBooks, type, btcAmount);

                    if (matches.Count == 0)
                    {
                        console.WriteLine("No viable offers.");
                        continue;
                    }

                    console.WriteLine($"\n{matches.Count} matched orders:");
                    foreach (var match in matches)
                    {
                        var total = match.Order.Price * match.UsedAmount;
                        console.WriteLine($"- {match.Exchange}, Price: {match.Order.Price}, Amount: {match.UsedAmount}, Total: {total:F2}");
                    }

                    var totalCost = matches.Sum(x => x.Order.Price * x.UsedAmount);
                    var totalBtc = matches.Sum(x => x.UsedAmount);
                    console.WriteLine($"\nTotal {(type == OrderType.Buy ? "Cost" : "Revenue")}: {totalCost:F2} EUR for {totalBtc} BTC\n");
                }
                catch (Exception ex)
                {
                    console.WriteLine($"An unexpected error occurred: {ex.Message}\n");
                }
            }

            console.WriteLine("Session ended. Goodbye!");
        }
    }
}