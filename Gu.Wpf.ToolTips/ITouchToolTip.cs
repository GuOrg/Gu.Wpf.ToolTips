namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Markup;

    public interface ITouchToolTip
    {
        void OnToolTipChanged(UIElement adornedElement);
       
        UIElement PlacementTarget { get; set; }
    }
}