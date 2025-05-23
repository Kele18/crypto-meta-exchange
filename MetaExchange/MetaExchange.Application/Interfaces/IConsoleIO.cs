namespace MetaExchange.Application.Interfaces
{
    public interface IConsoleIO
    {
        void Write(string message);

        void WriteLine(string message);

        string? ReadLine();
    }
}