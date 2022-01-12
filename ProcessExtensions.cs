using System.Diagnostics;

namespace Perfy
{
    internal static class ProcessExtensions
    {
        private static Dictionary<string, Func<Process, long>> Map = new Dictionary<string, Func<Process, long>>{
            { "Thread Count", p => p.Threads.Count },
            { "OS Handles", p => p.HandleCount },
            { "Current Memory Used", p => (p.PrivateMemorySize64 / 1024) / 1024 },
        };

        public static List<Data> GetProcessData(this Process process)
        {
            var ret = new List<Data>();
            return Map.Select(x => new Data(x.Key, x.Value(process))).ToList();
        }

        public static void UpdateProcessData(this Process process, List<Data> list)
        {
            process.Refresh();
            list.ForEach(x => x.Update(Map[x.Label](process)));
        }

        public static string GetDisplayTitle(this Process process) => $"{process.ProcessName}:{process.Id}";


    }
}
