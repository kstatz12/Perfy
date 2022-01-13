using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Analysis.JIT;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public class Cache
{
    public List<TraceGC> GCEvents { get; }
    public List<TraceJittedMethod> JITEvents { get; }
    public TraceJittedMethod? CurrentJIT { get; private set; }
    public TraceGC? CurrentGC { get; private set; }
    public List<ContentionStopTraceData> ContentionEvents { get; }

    public Cache()
    {
        GCEvents = new List<TraceGC>();
        JITEvents = new List<TraceJittedMethod>();
        ContentionEvents = new List<ContentionStopTraceData>();
        CurrentGC = null;
        CurrentJIT = null;
    }

    public void Handle(TraceGC newGc)
    {
        if(CurrentGC != null)
        {
            GCEvents.Add(CurrentGC);
        }
        CurrentGC = newGc;
    }

    public void Handle(ContentionStopTraceData @event)
    {
        ContentionEvents.Add(@event);
    }

    public void Handle(TraceJittedMethod newJIT)
    {
        if(CurrentJIT != null)
        {
            JITEvents.Add(CurrentJIT);
        }
        CurrentJIT = newJIT;
    }
}
