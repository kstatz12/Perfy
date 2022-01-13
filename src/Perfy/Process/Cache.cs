using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy.Processes;

public class Cache
{
    public List<TraceGC> GCEvents { get; }
    public TraceGC? CurrentGC { get; private set; }
    public List<ContentionStopTraceData> ContentionEvents { get; }

    public Cache()
    {
        GCEvents = new List<TraceGC>();
        ContentionEvents = new List<ContentionStopTraceData>();
        CurrentGC = null;
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
}
