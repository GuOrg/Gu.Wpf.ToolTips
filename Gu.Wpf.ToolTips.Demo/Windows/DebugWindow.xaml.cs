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

        private void Button1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button1_ToolTipOpening));
        }

        private void Button1_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button1_ToolTipClosing));
        }

        private void Button2_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipOpening));
        }

        private void Button2_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipClosing));
        }

        private void Button3_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipOpening));
        }

        private void Button3_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipClosing));
        }

        private void Button4_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipOpening));
        }

        private void Button4_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.Button2_ToolTipClosing));
        }
    }
}
