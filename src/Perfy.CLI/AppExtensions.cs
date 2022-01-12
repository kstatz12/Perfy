using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;

namespace Perfy.CLI;

public static class AppExtensions
{
    public static void InitArgs(this CommandLineApplication app, Action<Process> startFunc)
    {
        var processName = app.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);
        var processId = app.Option("-pid|--processid", "Process Id", CommandOptionType.SingleValue);
        app.HelpOption();

        app.OnExecute(() => {
            Process? p = null;
            if(processName.HasValue())
            {
                p = ProcessManager.GetProcess(processName.Value() ?? string.Empty);
            }
            else if(processId.HasValue())
            {
                if(int.TryParse(processId.Value(), out var pid))
                {
                    p = ProcessManager.GetProcess(pid) ;
                }
            }
            if(p == null)
            {
                throw new Exception("No Process Found");
            }
            startFunc(p);
        });
    }
}
