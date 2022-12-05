// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Analysis.JIT;
using Perfy;
using Perfy.CLI;
using Perfy.Processes;
using Spectre.Console;

internal class Program
{
    private static void Main(string[] args)
    {
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

            eventQueue.Register<TraceGC>();
            eventQueue.Register<TraceJittedMethod>();

            var engine = new Engine(p, eventQueue);

            var layout = new Layout();

            var processManager = new ProcessorManager(eventQueue);

            var outputManager = new OutputManager(layout, processManager);

            var displayTable = layout.Create();

            AnsiConsole.Write(displayTable);

            Console.CancelKeyPress += (_, _) =>
            {
                processManager.Stop();
            };

            await processManager.Start();
        });

        app.Execute(args);
    }

}
