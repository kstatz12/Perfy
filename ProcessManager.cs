using System.Diagnostics;

namespace Perfy
{
    internal interface IProcessManager
    {
        Process GetProcess();
    }

    internal sealed class NamedProcessManager : IProcessManager
    {
        private readonly string processName;

        public NamedProcessManager(string processName)
        {
           this.processName = processName;
        }

        public Process GetProcess()
        {
            var process = Process.GetProcessesByName(this.processName).FirstOrDefault();
            if(process == null)
            {
                throw new Exception($"No Process Found For {this.processName}");
            }
            return process;
        }
    }

    internal sealed class IdProcessManager : IProcessManager
    {
        private readonly int processId;

        public IdProcessManager(int processId)
        {
            this.processId = processId;
        }

        public Process GetProcess()
        {
            var process = Process.GetProcessById(this.processId);
            if(process == null)
            {
                throw new Exception($"No Process Found For Id {this.processId}");
            }
            return process;
        }
    }

    internal static class ProcessManagerFactory
    {
        public static IProcessManager GetProcessManagerByName(string processName)
            => new NamedProcessManager(processName);

        public static IProcessManager GetProcessManagerById(string processIdStr)
        {
            if(int.TryParse(processIdStr, out var pid))
            {
                return new IdProcessManager(pid);
            }
            throw new ArgumentNullException($"Invalid Process Format {processIdStr}");
        }


    }
}
