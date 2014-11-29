namespace Gu.Wpf.ToolTips
{
    using System.Windows;

    public interface ITouchToolTip
    {
        void OnToolTipChanged(UIElement adornedElement);
    }
}