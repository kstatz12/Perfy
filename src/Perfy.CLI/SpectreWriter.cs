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
        if(input.GCEventsBuffer.Any())
        {
            AnsiConsole.Write(new Rule("GC Stats"));
            AnsiConsole.Write(input.GCEventsBuffer.ToTable());
            input.ArchiveGcBuffer();
        }

        if(input.ContentionEventsBuffer.Any())
        {
            AnsiConsole.Write(new Rule("Thread Contention Stats"));
            AnsiConsole.Write(input.ContentionEventsBuffer.ToTable());
            input.ArchiveContentionBuffer();
        }
    }

    public void WriteEnd(Cache input)
    {
        AnsiConsole.Write(new Rule("Post Run Stats"));
        AnsiConsole.Write(input.GetStats().ToTable());
    }

    public void WriteStart(Process process)
    {
        Console.WriteLine($"Attached to Process {process.Id}|{process.ProcessName}");
    }
}
