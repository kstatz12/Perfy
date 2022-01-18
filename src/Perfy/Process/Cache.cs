using System.Diagnostics;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public static class Cache
{
    private static List<TraceGC> gcColdStorage;
    private static List<ContentionStopTraceData> contentionEventsColdStorage;
    private static List<ThreadPoolWorkerThreadTraceData> threadPoolColdStorage;

    public static List<TraceGC> GCEventsBuffer { get; }
    public static List<ContentionStopTraceData> ContentionEventsBuffer { get; }
    public static List<ThreadPoolWorkerThreadTraceData> ThreadPoolBuffer { get; }

    public static Process? Process { get; private set; }


    static Cache()
    {
        GCEventsBuffer = new List<TraceGC>();
        ContentionEventsBuffer = new List<ContentionStopTraceData>();

        gcColdStorage = new List<TraceGC>();
        contentionEventsColdStorage = new List<ContentionStopTraceData>();

        threadPoolColdStorage = new List<ThreadPoolWorkerThreadTraceData>();
        ThreadPoolBuffer = new List<ThreadPoolWorkerThreadTraceData>();
    }

    public static void SetProcess(Process process)
    {
        Process = process;
    }

    public static void Handle(TraceGC newGc)
    {
        GCEventsBuffer.Add(newGc);
    }

    public static void Handle(ContentionStopTraceData @event)
    {
        ContentionEventsBuffer.Add(@event);
    }

    public static void Handle(ThreadPoolWorkerThreadTraceData @event)
    {
        ThreadPoolBuffer.Add(@event);
    }

    public static void ArchiveContentionBuffer()
    {
        if(ContentionEventsBuffer.Any())
        {
            contentionEventsColdStorage.AddRange(ContentionEventsBuffer);
            ContentionEventsBuffer.Clear();
        }
    }

    public static void ArchiveGcBuffer()
    {
        if(GCEventsBuffer.Any())
        {
            gcColdStorage.AddRange(GCEventsBuffer);
            GCEventsBuffer.Clear();
        }
    }

    public static void ArchiveThreadPoolBuffer()
    {
        if(ThreadPoolBuffer.Any())
        {
            threadPoolColdStorage.AddRange(ThreadPoolBuffer);
            ThreadPoolBuffer.Clear();
        }
    }

    public static ThreadStats GetIncrementalThreadStats()
    {
        var stats = new ThreadStats();
        stats.ThreadStartCount = ThreadPoolBuffer.Where(x => x.EventName == "ThreadPoolWorkerThread/Start").Count();
        stats.ThreadStopCount = ThreadPoolBuffer.Where(x => x.EventName == "ThreadPoolWorkerThread/Stop").Count();
        stats.ThreadWaits = ThreadPoolBuffer.Where(x => x.EventName == "ThreadPoolWorkerThread/Wait").Count();
        return stats;
    }

    public static Stats GetStats()
    {
        var stats = new Stats();

        if(gcColdStorage.Any())
        {
            stats.GcCount = gcColdStorage.Count;
            stats.AverageGcTime = gcColdStorage.Average(x => x.DurationMSec);
            stats.TotalGCTime = gcColdStorage.Sum(x => x.DurationMSec);
        }

        if(contentionEventsColdStorage.Any())
        {
            stats.ThreadContentionCount = contentionEventsColdStorage.Count;
        }

        if(threadPoolColdStorage.Any())
        {
            stats.ThreadsCreated = threadPoolColdStorage.Where(x => x.EventName == "ThreadPoolWorkerThread/Start").Count();
            stats.ThreadsStopped = threadPoolColdStorage.Where(x => x.EventName == "ThreadPoolWorkerThread/Stop").Count();
            stats.ThreadWaits = threadPoolColdStorage.Where(x => x.EventName == "ThreadPoolWorkerThread/Wait").Count();
        }
        return stats;
    }
}

public class ThreadStats
{
    public int ThreadStartCount { get; set; }
    public int ThreadStopCount { get; set; }
    public int ThreadWaits { get; set; }
}

public class Stats
{
    public int GcCount { get; set; }
    public double AverageGcTime { get; set; }
    public double TotalGCTime { get; set; }
    public int ThreadContentionCount { get; set; }
    public int ThreadsCreated { get; set; }
    public int ThreadsStopped { get; set; }
    public int ThreadWaits { get; set; }
    public int MaxMemoryUsed { get; set; }
}
