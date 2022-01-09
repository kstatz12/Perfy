// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.CommandLineUtils;
using Perfy;

var app = new CommandLineApplication();

app.Command("Perfy", cmd => {
    cmd.HelpOption("-?|-h|--help");
    var pidOption = cmd.Option("-p|--pid", "Process Id", CommandOptionType.SingleValue);
    var pNameOption = cmd.Option("-n|--name", "Process Name", CommandOptionType.SingleValue);

    cmd.OnExecute(() => {
        if(pidOption.HasValue())
        {
            var processManager = ProcessManagerFactory.GetProcessManagerById(pidOption.Value());
            var e = new Engine(processManager: processManager);
            e.Start();
        }
        else if(pNameOption.HasValue())
        {
            var processManager = ProcessManagerFactory.GetProcessManagerByName(pNameOption.Value());
            var e = new Engine(processManager: processManager);
            e.Start();
        }
        return 0;
    });
});

try
{
    app.Execute(args);
}
catch(CommandParsingException ex)
{
    Console.WriteLine(ex.Message);
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
