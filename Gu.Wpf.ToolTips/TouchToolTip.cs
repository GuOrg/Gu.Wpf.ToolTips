namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Sets DataContext to the DataContext of the adorned element
    /// </summary>
    public class TouchToolTip : ToolTip
    {
        static TouchToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TouchToolTip),
                new FrameworkPropertyMetadata(typeof(TouchToolTip)));
        }
    }
}
