using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace Perfy;

public class EventManager
{
    private readonly Process process;

    public EventManager(Process process)
    {
        this.process = process;
    }

    private (IDisposable session, TraceEventDispatcher dispatcher) Init()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var traceEventSession = new TraceEventSession($"PerfySession_{Guid.NewGuid()}");
            traceEventSession.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Informational, (ulong)ClrTraceEventParser.Keywords.GC);
            traceEventSession.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Informational, (ulong)ClrTraceEventParser.Keywords.Contention);
            traceEventSession.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Informational, (ulong)ClrTraceEventParser.Keywords.Threading);
            return (traceEventSession, traceEventSession.Source);
        }
        var providers = new List<EventPipeProvider>();
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GC));
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GC));
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.GC));

        var client = new DiagnosticsClient(this.process.Id);
        var eventPipeSession = client.StartEventPipeSession(providers, false);

        return (eventPipeSession, new EventPipeEventSource(eventPipeSession.EventStream));
    }
}
