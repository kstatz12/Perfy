using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace Perfy.Processes;

public class Engine : IDisposable
{
    private readonly Process process;
    private readonly EventQueue queue;
    private IDisposable session;
    private TraceEventDispatcher dispatcher;

    public Engine(Process process, EventQueue queue)
    {
        this.process = process;
        this.queue = queue;
        (session, dispatcher) = InititializeProviders();
        if(dispatcher is not null)
        {
            ConfigureCallbacks(dispatcher);
        }
        else {
            throw new StartupException("Could Not Initialze TraceEventDispatcher");
        }
    }

    public void Dispose()
    {
        session.Dispose();
    }

    private void ConfigureCallbacks(TraceEventDispatcher dispatch)
    {
        dispatch.NeedLoadedDotNetRuntimes();
        dispatch.AddCallbackOnProcessStart(c => {
            c.AddCallbackOnDotNetRuntimeLoad(r => {
                r.GCEnd += (p, e) => {
                    if(p.ProcessID == this.process.Id)
                    {
                        this.queue.Enqueue(e);
                    }
                };

                r.JITMethodEnd += (p, e) => {
                    if(p.ProcessID == this.process.Id)
                    {
                        this.queue.Enqueue(e);
                    }
                };
            });
        });
    }


    private (IDisposable session, TraceEventDispatcher dispatcher) InititializeProviders()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var traceEventSession = new TraceEventSession($"PerfySession_{Guid.NewGuid()}");
            traceEventSession.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Informational, (ulong)ClrTraceEventParser.Keywords.Default);
            return (traceEventSession, traceEventSession.Source);
        }
        var providers = new List<EventPipeProvider>();
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.Default));
        var client = new DiagnosticsClient(this.process.Id);
        var eventPipeSession = client.StartEventPipeSession(providers, false);
        return (eventPipeSession, new EventPipeEventSource(eventPipeSession.EventStream));
    }
}
