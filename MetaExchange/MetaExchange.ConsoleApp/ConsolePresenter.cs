using MetaExchange.Application;
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
        Path.GetFullPath(configOptions.Value.OrderBookPath),
        "order_books_data.json"
    );

    public async Task RunAsync()
    {
        Console.WriteLine("MetaExchange - BTC");

        Console.Write("Enter BTC amount: ");
        var btcAmount = decimal.Parse(Console.ReadLine()!);

        Console.Write("Order type (Buy/Sell): ");
        var type = Enum.Parse<OrderType>(Console.ReadLine()!, true);

        Console.WriteLine("\nLoading order book...");
        var orderBooks = await loader.LoadOrderBooksAsync(_filePath);

        var matches = matcher.MatchOrders(orderBooks, type, btcAmount);

        if (matches.Count == 0)
        {
            Console.WriteLine("No viable offers.");
            return;
        }

        Console.WriteLine($"\n {matches.Count} matched orders:");

        foreach (var (exchange, order, usedAmount) in matches)
        {
            Console.WriteLine($"- {exchange}, Price: {order.Price}, Amount: {usedAmount}, Total: {order.Price * usedAmount:F2}");
        }

        var total = matches.Sum(x => x.order.Price * x.usedAmount);
        Console.WriteLine($"\nTotal {(type == OrderType.Buy ? "Cost" : "Revenue")}: {total:F2} EUR");
    }
}