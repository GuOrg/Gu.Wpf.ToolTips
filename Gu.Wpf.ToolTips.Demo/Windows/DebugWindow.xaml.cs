namespace Gu.Wpf.ToolTips.Demo.Windows
{
    using System.Windows;
    using System.Windows.Controls;

    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            this.InitializeComponent();
        }

        private void Button_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
        }

        private void Button_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
        }
    }
}
