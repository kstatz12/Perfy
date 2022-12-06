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
            table.AddColumn("Method Name");
            table.AddColumn("IL Size");
            table.AddColumn("Native Size");
            table.AddColumn("Compile Cpu Time");
            table.AddColumn("Jit Hot CodeRequest Size");
            table.AddColumn("Jit RO DataRequest Size");
            table.AddColumn("Module IL Path");
            table.AddColumn("Thread ID");
            table.AddColumn("Optimization Tier");
        });
    }

    public Table Create() => TableFactory.Create(table =>
    {
        table.AddColumn("GC Events");
        table.AddColumn("JIT Events");

        table.AddRow(this.GcTable, this.JitTable);
    });
}
