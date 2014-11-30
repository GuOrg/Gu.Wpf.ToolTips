namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Sets DataContext to the DataContext of the adorned element
    /// </summary>
    public class TouchToolTip : ToolTip, ITouchToolTip
    {
        static TouchToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTip), new FrameworkPropertyMetadata(typeof(TouchToolTip)));
        }

        public void OnToolTipChanged(UIElement adornedElement)
        {
            var dataContextBinding = new Binding(DataContextProperty.Name)
            {
                Source = adornedElement,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, DataContextProperty, dataContextBinding);
        }
    }
}
