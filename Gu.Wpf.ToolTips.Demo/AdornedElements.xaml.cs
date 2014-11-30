namespace Gu.Wpf.ToolTips.Demo
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AdornedElements.xaml
    /// </summary>
    public partial class AdornedElements : UserControl
    {
        public AdornedElements()
        {
            InitializeComponent();
        }

        private void OnColorClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources[PopupButton.InfoBrushKey] = ((System.Windows.Controls.Button)sender).Background;
        }
    }
}
