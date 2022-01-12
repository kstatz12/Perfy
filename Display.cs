using System.Diagnostics;

namespace Perfy
{
    internal sealed class Data
    {
        public Data(string label, double value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; }

        public double Value { get; private set; }

        public void Update(double newValue) => this.Value = newValue;

    }

    internal class Display : IDisposable
    {
        private readonly Process process;

        public Display(Process process)
        {
            this.process = process;
        }

        public void Draw()
        {

        }

        public void Dispose()
        {
            process.Dispose();
        }
    }
}
