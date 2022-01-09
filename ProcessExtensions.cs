using System.Diagnostics;
using Spectre.Console;

namespace Perfy
{
    internal static class ProcessExtensions
    {
        private static Dictionary<string, Func<Process, long>> Map = new Dictionary<string, Func<Process, long>>{
            { "Thread Count", p => p.Threads.Count },
            { "OS Handles", p => p.HandleCount },
            { "Peak Paged Memory", p => p.PeakPagedMemorySize64 },
            { "Current Paged Memory", p => p.PagedMemorySize64 },
            { "Peak Virtual Memory", p => p.PeakVirtualMemorySize64 },
            { "Current Virtual Memory", p => p.VirtualMemorySize64 },
            { "Current Total Memory", p => p.WorkingSet64 }
        };


        //Im too lazy to think of a color for each of them
        private static Color RandomColor()
        {
            return (Color)Enum.GetValues(typeof(Color))
                .OfType<Enum>()
                .OrderBy(x => Guid.NewGuid())
                .First();
        }

        public static List<ClrValue> InitProcessBars(this Process process)
        {
            var ret = new List<ClrValue>();
            return Map.Select(x => new ClrValue(x.Key, RandomColor(), x.Value(process))).ToList();
        }

        public static void UpdateProcessBars(this Process process, List<ClrValue> list)
        {
            list.ForEach(x => x.Update(Map[x.Label](process)));
        }
    }
}
