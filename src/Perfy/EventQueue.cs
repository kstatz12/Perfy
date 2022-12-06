using System.Collections.Concurrent;

namespace Perfy;

public interface IQueue
{

}

public class Queue<T> : IQueue
{
    public Queue()
    {
        this.EventQueue = new ConcurrentQueue<T>();
    }
    public ConcurrentQueue<T> EventQueue { get; }
}

public class EventQueue
{
    private readonly ConcurrentDictionary<Type, IQueue> queues;

    public EventQueue()
    {
        this.queues = new ConcurrentDictionary<Type, IQueue>();
    }

    public void Register<T>()
    {
        if (this.queues.ContainsKey(typeof(T)))
        {
            return;
        }
        this.queues.TryAdd(typeof(T), new Queue<T>());
    }

    public void Enqueue<T>(T @event)
    {
        if (this.queues[typeof(T)] is Queue<T> queue)
        {
            queue.EventQueue.Enqueue(@event);
        }
        else
        {
            throw new QueueNotRegisteredException(typeof(T));
        }
    }


    public T? Dequeue<T>()
    {
        if (this.queues[typeof(T)] is Queue<T> queue)
        {
            if (queue.EventQueue.TryDequeue(out var e))
            {
                return e;
            }
            return default(T);
        }
        throw new QueueNotRegisteredException(typeof(T));
    }
}
