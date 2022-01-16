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

        foreach(var e in gcEvents)
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
            .AddColumn("Flags")
            .AddColumn("Duration");
        foreach(var c in contentions)
        {
                table.AddRow(
                    c.TaskName.FormatForDisplay(x => x.ToString()),
                    c.ContentionFlags.FormatForDisplay(x => x.ToString()),
                    c.DurationNs.FormatForDisplay(x => $"{x} Ns"));
        }
        return table;
    }

    public static IRenderable ToTable(this Stats stats)
    {
       var table = new Table()
           .AddColumn("GC Count")
           .AddColumn("Average GC Time")
           .AddColumn("Total GC Time")
           .AddColumn("Thread Contention Count")
           .AddColumn("Average Contention Time")
           .AddColumn("Total Contention Time");

       table.AddRow(stats.GcCount.FormatForDisplay(x => x.ToString()),
                    stats.AverageGcTime.FormatForDisplay(x => $"{x} MS"),
                    stats.TotalGCTime.FormatForDisplay(x => $"{x} MS"),
                    stats.ThreadContentionCount.FormatForDisplay(x => x.ToString()),
                    stats.AverageContentionTime.FormatForDisplay(x => $"{x} NS"),
                    stats.TotalContentionTime.FormatForDisplay(x => $"{x} NS"));

       return table;
    }

    public static IRenderable ToTable(ThreadStats stats)
    {
       var table = new Table();

       return table;
    }

}
