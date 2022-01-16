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
    private readonly Cache data;
    private readonly IWriter writer;
    private readonly int sampleTime;

    public Engine(IWriter writer, int sampleTime, Func<Process> processResolverFn)
    {
        this.process = processResolverFn();
        var (session, dispatcher) = InititializeProviders(this.process.Id);
        this.session = session;
        this.dispatcher = dispatcher;
        this.data = new Cache();
        this.writer = writer;
        this.sampleTime = sampleTime;
        ConfigureCallbacks(dispatcher, this.process.Id, this.data);

        TimerCallback timerCallback = _ => {
            this.writer.Write(this.data);
        };

        Console.CancelKeyPress += (_, _) => {
            this.writer.WriteEnd(this.data);
            this.session?.Dispose();
        };

        Timer timer = new Timer(callback: timerCallback, dueTime: 0, state: null, period: sampleTime);
    }

    public void Start()
    {
        this.writer.WriteStart(this.process);
        this.dispatcher.Process();
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
            });
        });

        dispatcher.Clr.ContentionStop += e => {
            if(e.ProcessID == processId)
            {
                data.Handle(e);
            }
        };

        dispatcher.Clr.ThreadPoolWorkerThreadWait += e => {
            if(e.ProcessID == processId)
            {
                Console.WriteLine($"{e.EventName}|{e.TaskName}");
            }
        };

        dispatcher.Clr.ThreadPoolWorkerThreadStop += e => {
            if(e.ProcessID == processId)
            {
                Console.WriteLine($"{e.EventName}|{e.TaskName}");
            }
        };


        dispatcher.Clr.All += e => {
            Console.WriteLine(e.EventName);
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
