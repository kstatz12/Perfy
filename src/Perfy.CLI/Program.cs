// See https://aka.ms/new-console-template for more information
using McMaster.Extensions.CommandLineUtils;
using Perfy.CLI;

var app = new CommandLineApplication();
app.InitArgs();

try
{
    app.Execute(args);
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
