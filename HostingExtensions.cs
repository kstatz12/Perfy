
using McMaster.Extensions.CommandLineUtils;

namespace Perfy
{
    internal static class HostingExtensions
    {
        public static void InitArgParsing(this CommandLineApplication app)
        {
            var processName = app.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);
            var processId = app.Option("-pid|--processid", "Process Id", CommandOptionType.SingleValue);
            app.HelpOption();

            app.OnExecute(() => {
                IProcessManager processManager;
                if(processName.HasValue())
                {
                    processManager = ProcessManagerFactory.GetProcessManagerByName(processName: processName.Value() ?? string.Empty);
                }
                else if(processId.HasValue())
                {
                    processManager = ProcessManagerFactory.GetProcessManagerById(processId.Value() ?? string.Empty);
                }
                else
                {
                    throw new Exception("No Valid Arguments");
                }

                if(processManager != null)
                {
                    var engine = new Engine(processManager);
                    engine.Collect();
                }
            });
        }
    }
}
