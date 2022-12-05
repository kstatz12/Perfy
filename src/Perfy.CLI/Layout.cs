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
