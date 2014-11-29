namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the DataContext of the adorned element
    /// </summary>
    public class TouchToolTip : ToolTip, ITouchToolTip
    {
        static TouchToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTip), new FrameworkPropertyMetadata(typeof(TouchToolTip)));
        }

        public void OnToolTipChanged(UIElement adornedElement)
        {
            DataContext = new BindingProxy(adornedElement);
        }
    }
}
