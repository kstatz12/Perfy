using Spectre.Console;

namespace Perfy.CLI;

public static class TableFactory
{
    public static Table Create(Action<Table> tableFn)
    {
        var table = new Table();
        tableFn(table);
        return table;
    }
}

public class Layout
{
    public Table GcTable { get; }
    public Table JitTable { get; }
    public Layout()
    {
        GcTable = TableFactory.Create(table =>
        {
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
        });

        JitTable = TableFactory.Create(table =>
        {
            table.AddColumn("MethodName");
            table.AddColumn("ILSize");
            table.AddColumn("NativeSize");
            table.AddColumn("CompileCpuTimeMSec");
            table.AddColumn("JitHotCodeRequestSize");
            table.AddColumn("JitRODataRequestSize");
            table.AddColumn("ModuleILPath");
            table.AddColumn("ThreadID");
            table.AddColumn("OptimizationTier");
        });
    }

    public Table Create() => TableFactory.Create(table =>
    {
        table.AddColumn("GCEvents");
        table.AddColumn("JITEvents");

        table.AddRow(this.GcTable, this.JitTable);
    });
}
