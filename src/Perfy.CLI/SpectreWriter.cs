using System.Diagnostics;
using Perfy.Misc;
using Perfy.Processes;
using Spectre.Console;

namespace Perfy.CLI;

public class SpectreWriter : IWriter
{

    public void Write()
    {
        if(Cache.GCEventsBuffer.Any())
        {
            AnsiConsole.Write(new Rule("GC Stats"));
            AnsiConsole.Write(Cache.GCEventsBuffer.ToTable());
            Cache.ArchiveGcBuffer();
        }

        if(Cache.ContentionEventsBuffer.Any())
        {
            AnsiConsole.Write(new Rule("Thread Contention Stats"));
            AnsiConsole.Write(Cache.ContentionEventsBuffer.ToTable());
            Cache.ArchiveContentionBuffer();
        }

        if(Cache.ThreadPoolBuffer.Any())
        {
            AnsiConsole.Write(new Rule("Thread Stats"));
            AnsiConsole.Write(Cache.GetIncrementalThreadStats().ToTable());
            Cache.ArchiveThreadPoolBuffer();
        }

        if(Cache.Process != null)
        {
            AnsiConsole.Write(new Rule("Process Stats"));
            AnsiConsole.Write(Cache.Process.ToTable(false));
        }
    }

    public void WriteEnd()
    {
        AnsiConsole.Write(new Rule("Post Run Stats"));
        AnsiConsole.Write(Cache.GetStats().ToTable());
        if(Cache.Process != null)
        {
            AnsiConsole.Write(new Rule("Final Process Stats"));
            AnsiConsole.Write(Cache.Process.ToTable(true));
        }

    }

    public void WriteStart()
    {
        if(Cache.Process != null)
        {
            Console.WriteLine($"Attached to Process {Cache.Process.Id}|{Cache.Process.ProcessName}");
        }
    }
}
