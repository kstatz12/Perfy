using System.Diagnostics;

namespace Perfy;

public static class ProcessManager
{
    public static Process? GetProcess(string name) => Process.GetProcessesByName(name).FirstOrDefault();

    public static Process? GetProcess(int processId) => Process.GetProcessById(processId);

}
