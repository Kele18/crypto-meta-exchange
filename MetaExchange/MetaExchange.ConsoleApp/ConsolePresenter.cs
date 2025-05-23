using MetaExchange.Application.DTOs;
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
        Path.GetFullPath(configOptions.Value.OrderBookPath),
        "order_books_data.json"
    );

    public async Task RunAsync()
    {
        Console.WriteLine("MetaExchange - BTC");

        Console.Write("Order type (Buy/Sell): ");
        var type = Enum.Parse<OrderType>(Console.ReadLine()!, true);

        Console.Write("Enter BTC amount: ");
        var btcAmount = decimal.Parse(Console.ReadLine()!);

        Console.WriteLine("\nLoading order book...");
        var orderBooks = await loader.LoadOrderBooksAsync(_filePath);

        List<MatchedOrder> matches = matcher.MatchOrders(orderBooks, type, btcAmount);

        if (matches.Count == 0)
        {
            Console.WriteLine("No viable offers.");
            return;
        }

        Console.WriteLine($"\n {matches.Count} matched orders:");

        foreach (var match in matches)
        {
            Console.WriteLine($"- {match.Exchange}, Price: {match.Order.Price}, Amount: {match.UsedAmount}," +
                $" Total: {match.Order.Price * match.UsedAmount:F2}");
        }

        var total = matches.Sum(x => x.Order.Price * x.UsedAmount);
        Console.WriteLine($"\nTotal {(type == OrderType.Buy ? "Cost" : "Revenue")}: {total:F2} EUR");
    }
}