using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;
using Perfy.Processes;

namespace Perfy.CLI;

public static class AppExtensions
{
    private const int FIVE_MINUTES = (300 * 1000);
    public static void InitArgs(this CommandLineApplication app)
    {
        var processName = app.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);
        var processId = app.Option("-pid|--processid", "Process Id", CommandOptionType.SingleValue);
        var timer = app.Option("-t|--time", "Time in seconds to run the process, defaults to 5 minutes", CommandOptionType.SingleValue);
        app.HelpOption();

        app.OnExecute(() => {

            int ttl = FIVE_MINUTES;
            if(int.TryParse(timer.Value(), out var seconds))
            {
               ttl = seconds * 1000;
            }

            var ev = new Engine(new SpectreWriter(ttl), ttl, () => {
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



            Console.CancelKeyPress += (_, _) => {
                ev.Stop();
            };

            ev.Start();
        });
    }
}
