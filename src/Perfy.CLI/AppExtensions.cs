using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Perfy.Processes;

namespace Perfy.CLI;

public static class AppExtensions
{
    public static void InitArgs(this CommandLineApplication app)
    {
        var processName = app.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);
        var processId = app.Option("-pid|--processid", "Process Id", CommandOptionType.SingleValue);
        var network = app.Option("-nt|--network", "Network address of machine running the process", CommandOptionType.SingleValue);
        app.HelpOption();

        app.OnExecute(() => {
            var ev = new Engine(new SpectreWriter(), () => {
                Process? p = null;
                if(processName.HasValue())
                {
                    p = Process.GetProcessesByName(processName.Value()).FirstOrDefault();
                }
                else if(processId.HasValue())
                {
                    if(int.TryParse(processId.Value(), out var pid))
                    {
                        p = Process.GetProcessById(pid);
                    }
                }
                else
                {
                   throw new Exception("Invalid Parameters");
                }
                if(p == null)
                {
                    throw new Exception("No Process Found");
                }
                return p;
            });

            ev.Start();

            ev.Dispose();
        });
    }
}
