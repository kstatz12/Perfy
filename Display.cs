using System.Diagnostics;
using Spectre.Console;

namespace Perfy
{
    internal sealed class ClrValue : IBarChartItem
    {
        public ClrValue(string label, Color color, double value)
        {
            Label = label;
            Color = color;
            Value = value;
        }

        public string Label { get; }

        public Color? Color { get; }

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

        public void Init()
        {
            var barValues = process.InitProcessBars();
            var barChart = new BarChart()
                .Width(120)
                .Label("[green bold underline] Process Stats[/]")
                .CenterLabel()
                .AddItems(barValues);

            AnsiConsole.Live(barChart)
                .Start(ctx => {
                    Thread.Sleep(1000);
                    process.UpdateProcessBars(barValues);
                });
        }

        public void Dispose()
        {
            process.Dispose();
        }
    }
}
