namespace Perfy
{
    internal sealed class Engine
    {
        private readonly IProcessManager processManager;

        public Engine(IProcessManager processManager)
        {
            this.processManager = processManager;
        }

        public void Start()
        {
            using var process = this.processManager.GetProcess();
            using var display = new Display(process);
            display.Init();
        }
    }
}
