using Microsoft.Diagnostics.Tracing.Parsers.Clr;

namespace Perfy
{
    internal class OutPut
    {
        public List<Group<string>> GcStartReasonsCounts { get; set; }
        public int GcStartCount {get; set;}

        public OutPut()
        {
            GcStartReasonsCounts = new List<Group<string>>();
            GcStartCount = 0;

        }
    }

    internal static class AnalysisEngine
    {
        public static OutPut Analyze(TraceEventSink sink)
        {
            var gcStartEvents = sink.Filter<GCStartTraceData>();
            var totalGcStartCount = gcStartEvents.Count();

            var gcReasonsCounts = gcStartEvents
                .GroupBy(x => x.Reason)
                .Select(x => new Group<string>(x.Key.ToString(), x.Count()));

            var gcStopEvents = sink.Filter<GCEndTraceData>();

            var gcHeapStats = sink.Filter<GCHeapStatsTraceData>();

            var contentions = sink.Filter<ContentionStartTraceData>();

            return new OutPut
            {
                GcStartCount = totalGcStartCount,
                GcStartReasonsCounts = gcReasonsCounts.ToList()
            };
        }
    }

    internal class Group<T>
    {
        public T Category { get; }
        public int Count { get; }

        public Group(T category, int count)
        {
            Category = category;
            Count = count;
        }
    }
}
