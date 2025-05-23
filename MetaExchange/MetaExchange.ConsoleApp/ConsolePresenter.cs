using MetaExchange.Application.Interfaces.DataSource;
using MetaExchange.Application.Interfaces.Matcher;
using MetaExchange.ConsoleApp.Core.Config;
using MetaExchange.Domain;
using Microsoft.Extensions.Options;

namespace MetaExchange.ConsoleApp;

public class ConsolePresenter(
    IOrderBookLoader loader,
    IOrderMatcher matcher,
    IOptions<AppConfig> configOptions)
{
    private readonly string _filePath = Path.Combine(
        Path.GetFullPath(configOptions.Value.OrderBookPath)
    );

    public async Task RunAsync()
    {
        Console.WriteLine("MetaExchange - BTC Console");

        while (true)
        {
            try
            {
                Console.Write("\nOrder type (Buy/Sell or Q to quit): ");
                var inputType = Console.ReadLine()?.Trim();

                if (string.Equals(inputType, "Q", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (!Enum.TryParse<OrderType>(inputType, true, out var orderType))
                {
                    Console.WriteLine("Invalid order type. Please enter 'Buy' or 'Sell'.");
                    continue;
                }

                Console.Write("Enter BTC amount: ");
                var amountInput = Console.ReadLine();
                if (!decimal.TryParse(amountInput, out var btcAmount) || btcAmount <= 0)
                {
                    Console.WriteLine("Invalid BTC amount. Please enter a positive decimal number.");
                    continue;
                }

                Console.WriteLine("\nLoading order book...");
                var orderBooks = await loader.LoadOrderBooksAsync(_filePath);

                var matches = matcher.MatchOrders(orderBooks, orderType, btcAmount);

                if (matches.Count == 0)
                {
                    Console.WriteLine("No viable offers found for the requested amount.");
                    continue;
                }

                Console.WriteLine($"\n{matches.Count} matched orders:");
                foreach (var match in matches)
                {
                    var total = match.Order.Price * match.UsedAmount;
                    Console.WriteLine($"- {match.Exchange}, Price: {match.Order.Price}, Amount: {match.UsedAmount}, Total: {total:F2}");
                }

                var grandTotal = matches.Sum(x => x.Order.Price * x.UsedAmount);
                var totalAmount = matches.Sum(x => x.UsedAmount);
                var label = orderType == OrderType.Buy ? "Cost" : "Revenue";
                Console.WriteLine($"\nTotal {label}: {grandTotal:F2} EUR for {totalAmount} BTC");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}