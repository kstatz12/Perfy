using Perfy.Misc;

namespace Perfy.CLI;

public class ConsoleEventLogger : IEventLogger
{
    public void Log<T>(T @event, Func<T, string> formatter)
    {
        var message = formatter(@event);
        Console.WriteLine(message);
    }
}
