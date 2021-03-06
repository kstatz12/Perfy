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

public class Engine
{
    private readonly Process process;
    private IDisposable session;
    private TraceEventDispatcher dispatcher;
    private readonly IWriter writer;
    private readonly int sampleTime;

    public Engine(IWriter writer, int sampleTime, Func<Process> processResolverFn)
    {
        if (processResolverFn is null)
        {
            throw new ArgumentNullException(nameof(processResolverFn));
        }

        this.process = processResolverFn();
        Cache.SetProcess(this.process);
        var (session, dispatcher) = InititializeProviders(this.process.Id);
        this.session = session;
        this.dispatcher = dispatcher;
        this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        this.sampleTime = sampleTime;
        ConfigureCallbacks(dispatcher, this.process.Id);

        TimerCallback timerCallback = _ =>
        {
            this.writer.Write();
        };

        this.process.Exited += (_, _) =>
        {
            this.Stop();
        };

        Timer timer = new Timer(callback: timerCallback, dueTime: 0, state: null, period: sampleTime);
    }

    public void Start()
    {
        this.writer.WriteStart();
        this.dispatcher.Process();
    }

    public void Stop()
    {
        this.writer.WriteEnd();
        this.session?.Dispose();
        this.process?.Dispose();
    }

    private static void ConfigureCallbacks(TraceEventDispatcher dispatcher, int processId)
    {
        dispatcher.NeedLoadedDotNetRuntimes();
        dispatcher.AddCallbackOnProcessStart(proc =>
        {
            proc.AddCallbackOnDotNetRuntimeLoad(runtime =>
            {
                runtime.GCEnd += (p, e) =>
                {
                    if (p.ProcessID == processId)
                    {
                        Cache.Handle(e);
                    }
                };
            });
        });

        dispatcher.Clr.ContentionStop += e =>
        {
            if (e.ProcessID == processId)
            {
                Cache.Handle(e);
            }
        };

        dispatcher.Clr.ThreadPoolWorkerThreadStart += e =>
        {
            if (e.ProcessID == processId)
            {
                Cache.Handle(e);
            }
        };

        dispatcher.Clr.ThreadPoolWorkerThreadWait += e =>
        {
            if (e.ProcessID == processId)
            {
                Cache.Handle(e);
            }
        };

        dispatcher.Clr.ThreadPoolWorkerThreadStop += e =>
        {
            if (e.ProcessID == processId)
            {
                Cache.Handle(e);
            }
        };
    }

    private static (IDisposable session, TraceEventDispatcher dispatcher) InititializeProviders(int processId)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var traceEventSession = new TraceEventSession($"PerfySession_{Guid.NewGuid()}");
            traceEventSession.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Informational, (ulong)ClrTraceEventParser.Keywords.Default);
            return (traceEventSession, traceEventSession.Source);
        }
        var providers = new List<EventPipeProvider>();
        providers.Add(new EventPipeProvider("Microsoft-Windows-DotNETRuntime", EventLevel.Informational, (long)ClrTraceEventParser.Keywords.Default));
        var client = new DiagnosticsClient(processId);
        var eventPipeSession = client.StartEventPipeSession(providers, false);
        return (eventPipeSession, new EventPipeEventSource(eventPipeSession.EventStream));
    }
}
