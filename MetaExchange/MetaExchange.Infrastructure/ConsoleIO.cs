using MetaExchange.Application.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace MetaExchange.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class ConsoleIO : IConsoleIO
    {
        public void WriteLine(string message) => Console.WriteLine(message);

        public void Write(string message) => Console.Write(message);

        public string? ReadLine() => Console.ReadLine();
    }
}