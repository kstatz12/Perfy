namespace Perfy
{
    internal sealed class Engine
    {
        private readonly IProcessManager processManager;
        private readonly TraceEventSink buffer;

        public Engine(IProcessManager processManager)
        {
            this.processManager = processManager;
            this.buffer = new TraceEventSink();
        }

        public void Collect()
        {
            using var process = this.processManager.GetProcess();
            var traceManager = new TraceManager(process, buffer);
            do
            {
                Thread.Sleep(1000);
            }
            while(!process.HasExited);
        }

        public void End()
        {
            try
            {

            }
            catch(Exception ex)
            {
                //this is our little secret
            }
        }
    }
}
