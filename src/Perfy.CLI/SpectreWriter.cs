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
        AnsiConsole.Write(new Rule("GC Stats"));
        AnsiConsole.Write(input.GCEvents.ToTable());
        AnsiConsole.Write(new Rule("Thread Contention Stats"));
        AnsiConsole.Write(input.ContentionEvents.ToTable());
    }

    public void WriteEnd(Cache data)
    {
        Console.WriteLine("Thanks!");
    }

    public void WriteStart(Process process)
    {
        Console.WriteLine($"Attached to Process {process.Id}|{process.ProcessName}");
    }
}
