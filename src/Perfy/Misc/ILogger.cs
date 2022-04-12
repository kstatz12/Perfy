namespace Perfy.Misc;

public interface IEventLogger
{
    void Log<T>(T @event, Func<T, string> formatter);
}
