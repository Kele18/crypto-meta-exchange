using MetaExchange.Application.Interfaces;

namespace MetaExchange.Infrastructure
{
    public class ConsoleIO : IConsoleIO
    {
        public void WriteLine(string message) => Console.WriteLine(message);

        public void Write(string message) => Console.Write(message);

        public string? ReadLine() => Console.ReadLine();
    }
}