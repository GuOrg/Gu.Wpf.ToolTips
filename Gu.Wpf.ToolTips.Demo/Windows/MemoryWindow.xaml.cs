#pragma warning disable WPF0041 // Set mutable dependency properties using SetCurrentValue.
namespace Gu.Wpf.ToolTips.Demo.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    public partial class MemoryWindow : Window
    {
        public MemoryWindow()
        {
            this.InitializeComponent();
        }

        private void OnGcClick(object sender, RoutedEventArgs e)
        {
            this.TotalMemory.Text = $"TotalMemory {GC.GetTotalMemory(forceFullCollection: true)} B";
        }

        private void OnToggleClick(object sender, RoutedEventArgs e)
        {
            var n = sender switch
            {
                Button { Content: "Toggle 100" } => 100,
                Button { Content: "Toggle 1000" } => 1000,
                Button { Content: "Toggle 10 000" } => 10_000,
                _ => throw new InvalidOperationException(),
            };

            for (var i = 0; i < n; i++)
            {
                _ = this.Dispatcher.Invoke(() => this.TouchToolTipServiceIsEnabled.IsChecked = !this.TouchToolTipServiceIsEnabled.IsChecked, DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
