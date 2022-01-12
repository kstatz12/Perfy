// See https://aka.ms/new-console-template for more information
using McMaster.Extensions.CommandLineUtils;
using Perfy;

var app = new CommandLineApplication();

app.InitArgParsing();

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
