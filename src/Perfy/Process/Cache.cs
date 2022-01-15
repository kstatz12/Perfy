using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public class Cache
{
    public List<TraceGC> GCEvents { get; }
    public List<ContentionStopTraceData> ContentionEvents { get; }

    public Cache()
    {
        GCEvents = new List<TraceGC>();
        ContentionEvents = new List<ContentionStopTraceData>();
    }

    public void Handle(TraceGC newGc)
    {
        GCEvents.Add(newGc);
    }

    public void Handle(ContentionStopTraceData @event)
    {
        ContentionEvents.Add(@event);
    }
}

public class Stats
{
    public int GcCount { get; private set; }
    public int AverageGcTime { get; private set; }
    public int TotalGCTime { get; private set; }
    public int ThreadContentionCount { get; private set; }
    public int AverageContentionTime { get; private set; }
    public int TotalContentionTime { get; private set; }
}
