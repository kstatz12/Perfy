using System.Runtime.Serialization;

namespace Perfy;

public class StartupException : Exception
{
    public StartupException()
    {
    }

    public StartupException(string? message) : base(message)
    {
    }

    public StartupException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected StartupException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class QueueNotRegisteredException : Exception
{
    public QueueNotRegisteredException(Type t) : base($"Could Not Find Event Queue For {t.Name}")
    {
    }

    public QueueNotRegisteredException(string? message) : base(message)
    {
    }

    public QueueNotRegisteredException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected QueueNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
