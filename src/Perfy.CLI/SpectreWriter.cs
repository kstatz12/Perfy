using System.Diagnostics;
using Perfy.Misc;
using Perfy.Processes;
using Spectre.Console;

namespace Perfy.CLI;

public class SpectreWriter : IWriter
{
    private readonly int duration;

    public SpectreWriter(int duration)
    {
        this.duration = duration;
    }

    public void Write(Cache input)
    {
        if(input.GCEvents.Any())
        {
            AnsiConsole.Write(new Rule("GC Stats"));
            AnsiConsole.Write(input.GCEvents.ToTable());
            input.GCEvents.Clear();
        }
        if(input.ContentionEvents.Any())
        {
            AnsiConsole.Write(new Rule("Thread Contention Stats"));
            AnsiConsole.Write(input.ContentionEvents.ToTable());
            input.ContentionEvents.Clear();
        }
    }

    public void WriteEnd(Cache data)
    {
        Console.WriteLine("Thanks!");
    }

    public void WriteStart(Process process)
    {
        AnsiConsole.Status().Start("Collecting Data...", ctx => {
            ctx.Status($"Attached to Process {process.Id}|{process.ProcessName}, Collecting Data");
            Thread.Sleep(this.duration);
        });
    }
}
