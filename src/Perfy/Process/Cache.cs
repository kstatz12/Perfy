using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public class Cache
{
    private readonly List<TraceGC> gcColdStorage;
    private readonly List<ContentionStopTraceData> contentionEventsColdStorage;
    private readonly List<ThreadPoolWorkerThreadTraceData> threadPoolColdStorage;

    public List<TraceGC> GCEventsBuffer { get; }
    public List<ContentionStopTraceData> ContentionEventsBuffer { get; }
    public List<ThreadPoolWorkerThreadTraceData> ThreadPoolBuffer { get; }


    public Cache()
    {
        GCEventsBuffer = new List<TraceGC>();
        ContentionEventsBuffer = new List<ContentionStopTraceData>();

        gcColdStorage = new List<TraceGC>();
        contentionEventsColdStorage = new List<ContentionStopTraceData>();

        threadPoolColdStorage = new List<ThreadPoolWorkerThreadTraceData>();
        ThreadPoolBuffer = new List<ThreadPoolWorkerThreadTraceData>();
    }

    public void Handle(TraceGC newGc)
    {
        GCEventsBuffer.Add(newGc);
    }

    public void Handle(ContentionStopTraceData @event)
    {
        ContentionEventsBuffer.Add(@event);
    }

    public void Handle(ThreadPoolWorkerThreadTraceData @event)
    {
        ThreadPoolBuffer.Add(@event);
    }

    public void ArchiveContentionBuffer()
    {
        if(this.ContentionEventsBuffer.Any())
        {
            this.contentionEventsColdStorage.AddRange(this.ContentionEventsBuffer);
            this.ContentionEventsBuffer.Clear();
        }
    }

    public void ArchiveGcBuffer()
    {
        if(this.GCEventsBuffer.Any())
        {
            this.gcColdStorage.AddRange(this.GCEventsBuffer);
            this.GCEventsBuffer.Clear();
        }
    }

    public void ArchiveThreadPoolBuffer()
    {
        if(this.ThreadPoolBuffer.Any())
        {
            this.threadPoolColdStorage.AddRange(this.ThreadPoolBuffer);
            this.ThreadPoolBuffer.Clear();
        }
    }

    public ThreadStats GetIncrementalThreadStats()
    {
        var stats = new ThreadStats();
        stats.ThreadCount = this.ThreadPoolBuffer.Where(x => x.EventName == "ThreadPoolWorkerThread/Stop").Count();
        stats.ThreadWaits = this.ThreadPoolBuffer.Where(x => x.EventName == "ThreadPoolWorkerThread/Wait").Count();
        return stats;
    }

    public Stats GetStats()
    {
        var stats = new Stats();

        if(this.gcColdStorage.Any())
        {
            stats.GcCount = this.gcColdStorage.Count;
            stats.AverageGcTime = this.gcColdStorage.Average(x => x.DurationMSec);
            stats.TotalGCTime = this.gcColdStorage.Sum(x => x.DurationMSec);
        }

        if(this.contentionEventsColdStorage.Any())
        {
            stats.ThreadContentionCount = this.contentionEventsColdStorage.Count;
        }
        return stats;
    }
}

public class ThreadStats
{
    public int ThreadCount { get; set; }
    public int ThreadWaits { get; set; }
}

public class Stats
{
    public int GcCount { get; set; }
    public double AverageGcTime { get; set; }
    public double TotalGCTime { get; set; }
    public int ThreadContentionCount { get; set; }
    public double AverageContentionTime { get; set; }
    public double TotalContentionTime { get; set; }
}
