namespace Perfy;

public interface IProcessor
{
    Task Process();
}

public class EventProcessor<T> : IProcessor
{
    private EventQueue queue;
    private readonly ProcessorState state;
    private readonly Func<T, Task> fn;

    public EventProcessor(EventQueue queue,
                          ProcessorState state,
                          Func<T, Task> fn)
    {
        this.queue = queue;
        this.state = state;
        this.fn = fn;
    }

    public async Task Process()
    {
        while (state.Running)
        {
            var @event = this.queue.Dequeue<T>();
            if (@event is not null)
            {
                await fn(@event);
            }
            else
            {
                await Task.Delay(50);
            }
        }
    }
}


public class ProcessorManager
{
    private ProcessorState state;
    private readonly List<IProcessor> processors;
    private readonly EventQueue dispatcher;

    public ProcessorManager(EventQueue queue)
    {
        this.state = new ProcessorState();
        this.processors = new List<IProcessor>();
        this.dispatcher = queue;
    }

    public ProcessorManager AddProcessor<T>(Func<T, Task> fn)
    {
        var processor = new EventProcessor<T>(this.dispatcher,
                                              this.state,
                                              fn);
        this.processors.Add(processor);
        return this;
    }

    public async Task Start()
    {
        this.state.Start();
        foreach (var p in this.processors)
        {
            await p.Process();
        }
    }

    public void Stop()
    {
        this.state.Stop();
    }

}

public class ProcessorState
{
    public ProcessorState()
    {
        this.Running = false;
    }

    public void Stop() => this.Running = false;
    public void Start() => this.Running = true;

    public bool Running { get; private set; }
}
