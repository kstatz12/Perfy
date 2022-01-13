using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using Perfy.Misc;

namespace Perfy.Processes;

public class EventHandler : IDisposable
{
    private readonly Process process;
    private IDisposable session;
    private TraceEventDispatcher dispatcher;
    private readonly Cache data;
    private readonly IWriter writer;

    public EventHandler(IWriter writer, Func<Process> processResolverFn)
    {
        this.process = processResolverFn();
        var (session, dispatcher) = InititializeProviders(this.process.Id);
        this.session = session;
        this.dispatcher = dispatcher;
        this.data = new Cache();
        this.writer = writer;
        ConfigureCallbacks(dispatcher, this.process.Id, this.data);
    }

    public void Dispose()
    {
        this.process.Dispose();
        this.session.Dispose();
        this.dispatcher.Dispose();
    }

    public void Start()
    {
        this.dispatcher.Process();
    }


    public void Stop()
    {
        this.session.Dispose();
        this.writer.Write(this.data);
    }


    protected virtual void ConfigureShutdownHooks(IDisposable session)
    {
        Console.CancelKeyPress += (e, a) => {
           session.Dispose();
           Environment.Exit(0);
        };

        if(!Console.IsInputRedirected)
        {
            var key = Console.ReadKey(true);
            while(key.Key == ConsoleKey.Enter)
            {
                Stop();
                Console.ReadKey(true);
            }
        }
    }

    private static void ConfigureCallbacks(TraceEventDispatcher dispatcher, int processId, Cache data)
    {
        dispatcher.NeedLoadedDotNetRuntimes();
        dispatcher.AddCallbackOnProcessStart(proc => {
            proc.AddCallbackOnDotNetRuntimeLoad(runtime => {
                runtime.GCEnd += (p, e) => {
                    if(p.ProcessID == processId)
                    {
                        data.Handle(e);
                    }
                };

                runtime.JITMethodEnd += (p, e) => {
                    if(p.ProcessID == processId)
                    {
                        data.Handle(e);
                    }
                }
            });
        });

        dispatcher.Clr.ContentionStop += e => {
            if(e.ProcessID == processId)
            {
                data.Handle(e);
            }
        };
    }

    private static (IDisposable session, TraceEventDispatcher dispatcher) InititializeProviders(int processId)
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
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.Contention));
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.Threading));

        var client = new DiagnosticsClient(processId);
        var eventPipeSession = client.StartEventPipeSession(providers, false);

        return (eventPipeSession, new EventPipeEventSource(eventPipeSession.EventStream));
    }
}
