using System.Diagnostics;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Perfy.Misc;
using Perfy.Processes;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Perfy.CLI;

public static class SpectreExtensions
{
    public static IRenderable ToTable(this List<TraceGC> gcEvents)
    {
        var table = new Table();
        table.AddColumn("Run");
        table.AddColumn("Reason");
        table.AddColumn("Duration");
        table.AddColumn("Generation");
        table.AddColumn("Handles");
        table.AddColumn("Pinned Object Count");
        table.AddColumn("Total Heap Size");
        table.AddColumn("Gen 1");
        table.AddColumn("Gen 2");
        table.AddColumn("Gen 3");
        table.AddColumn("Gen 4");

        foreach (var e in gcEvents)
        {
            var t = e.HeapStats;
            table.AddRow(e.Number.FormatForDisplay(x => x.ToString()),
                         e.Reason.FormatForDisplay(x => x.ToString()),
                         e.DurationMSec.FormatForDisplay(x => $"{x} MS"),
                         e.Generation.FormatForDisplay(x => x.ToString()),
                         t.PinnedObjectCount.FormatForDisplay(x => x.ToString()),
                         t.TotalHeapSize.FormatForDisplay(x => $"{x} Bytes"),
                         t.GenerationSize0.FormatForDisplay(x => $"{x} Bytes"),
                         t.GenerationSize1.FormatForDisplay(x => $"{x} Bytes"),
                         t.GenerationSize2.FormatForDisplay(x => $"{x} Bytes"),
                         t.GenerationSize3.FormatForDisplay(x => $"{x} Bytes"),
                         t.GenerationSize4.FormatForDisplay(x => $"{x} Bytes"));
        }
        return table;
    }

    public static IRenderable ToTable(this List<ContentionStopTraceData> contentions)
    {
        var table = new Table()
            .AddColumn("Name")
            .AddColumn("Flags");
        foreach (var c in contentions)
        {
            table.AddRow(c.TaskName.FormatForDisplay(x => x.ToString()),
                         c.ContentionFlags.FormatForDisplay(x => x.ToString()));
        }
        return table;
    }

    public static IRenderable ToTable(this Process process, bool isEnd)
    {
        process.Refresh();
        return isEnd ? FinalProcessTable(process) : IncrementalProcessTable(process);
    }

    private static IRenderable IncrementalProcessTable(Process process)
    {
        var table = new Table()
            .AddColumn("Process Id")
            .AddColumn("Virtual Memory Size")
            .AddColumn("Paged Memory Size")
            .AddColumn("Working Set Size")
            .AddColumn("Processor Time")
            .AddColumn("User Processor Time")
            .AddColumn("Threads");

        table.AddRow(process.Id.FormatForDisplay(x => x.ToString()),
                     process.VirtualMemorySize64.FormatForDisplay(x => $"{(x / 1024) / 1024 } MBytes"),
                     process.PagedMemorySize64.FormatForDisplay(x => $"{ (x / 1024) / 1024 } MBytes"),
                     process.WorkingSet64.FormatForDisplay(x => $"{(x / 1024) / 1024} MBytes"),
                     process.TotalProcessorTime.FormatForDisplay(x => x.ToString()),
                     process.UserProcessorTime.FormatForDisplay(x => x.ToString()),
                     process.Threads.Count.FormatForDisplay(x => x.ToString()));
        return table;
    }

    private static IRenderable FinalProcessTable(Process process)
    {
        var table = new Table()
                    .AddColumn("Process Id")
                    .AddColumn("Peak Virtual Memory Size")
                    .AddColumn("Peak Paged Memory Size")
                    .AddColumn("Peak Working Set")
                    .AddColumn("Processor Time")
                    .AddColumn("User Processor Time")
                    .AddColumn("Threads");

        table.AddRow(process.Id.FormatForDisplay(x => x.ToString()),
                     process.PeakVirtualMemorySize64.FormatForDisplay(x => $"{(x / 1024) / 1024 } MBytes"),
                     process.PeakPagedMemorySize64.FormatForDisplay(x => $"{(x / 1024) / 1024 } MBytes"),
                     process.PeakWorkingSet64.FormatForDisplay(x => $"{(x / 1024) / 1024} MBytes"),
                     process.TotalProcessorTime.FormatForDisplay(x => x.ToString()),
                     process.UserProcessorTime.FormatForDisplay(x => x.ToString()),
                     process.Threads.Count.FormatForDisplay(x => x.ToString()));
        return table;
    }

    public static IRenderable ToTable(this Stats stats)
    {
        var table = new Table()
            .AddColumn("GC Count")
            .AddColumn("Average GC Time")
            .AddColumn("Total GC Time")
            .AddColumn("Thread Contention Count")
            .AddColumn("Total Threads Started")
            .AddColumn("Total Threads Stopped")
            .AddColumn("Total Thread Waits");

        table.AddRow(stats.GcCount.FormatForDisplay(x => x.ToString()),
                     stats.AverageGcTime.FormatForDisplay(x => $"{x} MS"),
                     stats.TotalGCTime.FormatForDisplay(x => $"{x} MS"),
                     stats.ThreadContentionCount.FormatForDisplay(x => x.ToString()),
                     stats.ThreadsCreated.FormatForDisplay(x => x.ToString()),
                     stats.ThreadsStopped.FormatForDisplay(x => x.ToString()),
                     stats.ThreadWaits.FormatForDisplay(x => x.ToString()));
        return table;
    }

    public static IRenderable ToTable(this ThreadStats stats)
    {
        var table = new Table()
            .AddColumn("Thread Start Count")
            .AddColumn("Thread Stop Count")
            .AddColumn("Thread Wait");

        table.AddRow(stats.ThreadStartCount.FormatForDisplay(x => x.ToString()),
                     stats.ThreadStopCount.FormatForDisplay(x => x.ToString()),
                     stats.ThreadWaits.FormatForDisplay(x => x.ToString()));
        return table;
    }

}
