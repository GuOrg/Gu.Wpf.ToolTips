namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Sets DataContext to the DataContext of the adorned element
    /// </summary>
    public class TouchToolTip : ToolTip
    {
        public static readonly DependencyProperty ToolTipForProperty = DependencyProperty.Register(
            "ToolTipFor", 
            typeof(UIElement), 
            typeof(TouchToolTip),
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty UseTouchToolTipAsMouseOverToolTipProperty = TouchToolTipService.UseTouchToolTipAsMouseOverToolTipProperty.AddOwner(
            typeof(TouchToolTip),
            new FrameworkPropertyMetadata(
                true,
                FrameworkPropertyMetadataOptions.Inherits));

        static TouchToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TouchToolTip),
                new FrameworkPropertyMetadata(typeof(TouchToolTip)));
        }

        public UIElement ToolTipFor
        {
            get { return (UIElement)GetValue(ToolTipForProperty); }
            set { SetValue(ToolTipForProperty, value); }
        }

        public bool UseTouchToolTipAsMouseOverToolTip
        {
            get { return (bool)this.GetValue(UseTouchToolTipAsMouseOverToolTipProperty); }
            set { this.SetValue(UseTouchToolTipAsMouseOverToolTipProperty, value); }
        }
    }
}
