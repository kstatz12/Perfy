using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.Diagnostics.Tracing.Analysis.JIT;
using Spectre.Console;

namespace Perfy.CLI;

public class OutputManager
{
    private readonly Layout layout;
    private readonly ProcessorManager processorManager;

    public OutputManager(Layout layout, ProcessorManager processorManager)
    {
        this.layout = layout;
        this.processorManager = processorManager;

        processorManager.AddProcessor<TraceGC>(async e =>
        {
            //making the async gods happy
            await Task.Run(() =>
            {
                var t = e.HeapStats;
                this.layout.GcTable.AddRow(e.Number.FormatForDisplay(x => x.ToString()),
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
            });
        });

        processorManager.AddProcessor<TraceJittedMethod>(async e =>
        {
            await Task.Run(() =>
            {
                this.layout.JitTable.AddRow(e.MethodName,
                                     e.ILSize.FormatForDisplay(x => $"{x} Bytes"),
                                     e.NativeSize.FormatForDisplay(x => $"{x} Bytes"),
                                     e.CompileCpuTimeMSec.FormatForDisplay(x => $"{x} MSecs"),
                                     e.JitHotCodeRequestSize.FormatForDisplay(x => $"{x} Bytes"),
                                     e.JitRODataRequestSize.FormatForDisplay(x => $"{x} Bytes"),
                                     e.ModuleILPath.FormatForDisplay(x => x.ToString()),
                                     e.ThreadID.FormatForDisplay(x => x.ToString()),
                                     e.OptimizationTier.FormatForDisplay(x => x.ToString()));
            });

        });
    }
}

public static class FormatExtensions
{
    public static string FormatForDisplay<T>(this T input, Func<T, string> fn)
    {
        return fn(input);
    }
}
