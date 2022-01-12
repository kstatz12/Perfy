using System.Diagnostics;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace Perfy
{
    public class TraceManager
    {
        private readonly Process process;
        private readonly TraceEventSink sink;

        public TraceManager(Process process, TraceEventSink sink)
        {
            this.process = process;
            this.sink = sink;
        }

        public void Start()
        {
            using var session = new TraceEventSession("Perfy");

            session.Source.Clr.GCStart += e => {
                Console.WriteLine($"GC Start: {e.Reason}\t{e.Dump()}");
                this.sink.Handle(e);
            };

            session.Source.Clr.GCStop += e => {
                Console.WriteLine("GC Stop: {e.Reason}\t{e.Dump()}");
                this.sink.Handle(e);
            };

            session.Source.Clr.GCHeapStats += e => {
                this.sink.Handle(e);
            };

            session.Source.Clr.ContentionStart += e => {
                this.sink.Handle(e);
            };

            session.Source.Clr.ContentionStop += e => {
                this.sink.Handle(e);
            };

            session.Source.Process();
        }
    }


    public class TraceEventSink
    {
        private List<TraceEvent> eventBuffer;
        public TraceEventSink()
        {
            this.eventBuffer = new List<TraceEvent>();
        }

        public void Handle(TraceEvent e)
        {
            this.eventBuffer.Add(e);
        }

        public IEnumerable<T> Filter<T>() where T : TraceEvent
        {
            return this.eventBuffer.Where(x => x is T)
                .OrderByDescending(x => x.TimeStamp)
                .Select(x => (T)x);
        }
    }
}
