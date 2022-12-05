// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Perfy;
using Perfy.Processes;

Console.WriteLine("Hello World");

var app = new CommandLineApplication();

app.HelpOption();

var processName = app.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);
var processId = app.Option("-pid|--processid", "Process Id", CommandOptionType.SingleValue);

app.OnExecuteAsync(async e =>
{
    Process? p = null;

    if (processName.HasValue())
    {
        p = Process.GetProcessesByName(processName.Value()).FirstOrDefault();
    }
    else if (processId.HasValue())
    {
        if (int.TryParse(processId.Value(), out var pid))
        {
            p = Process.GetProcessById(pid);
        }
    }
    else
    {
        throw new Exception("Invalid Parameters");
    }
    if (p == null)
    {
        throw new Exception("No Process Found");
    }

    var eventQueue = new EventQueue();

    var engine = new Engine(p, eventQueue);

    var processManager = new ProcessorManager(eventQueue);

    processManager.AddProcessor<TraceGC>(async e => {
       await Task.Yield();

       Console.WriteLine($"{e.DurationMSec}");
    });
});
