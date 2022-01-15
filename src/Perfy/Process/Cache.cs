using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Analysis.JIT;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public class Cache
{
    public List<TraceGC> GCEvents { get; }
    public List<TraceJittedMethod> JITEvents { get; }
    public List<ContentionStopTraceData> ContentionEvents { get; }

    public Cache()
    {
        GCEvents = new List<TraceGC>();
        JITEvents = new List<TraceJittedMethod>();
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

    public void Handle(TraceJittedMethod newJIT)
    {
        JITEvents.Add(newJIT);
    }
}
