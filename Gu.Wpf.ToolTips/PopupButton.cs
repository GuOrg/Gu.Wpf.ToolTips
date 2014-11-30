namespace Gu.Wpf.ToolTips
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class PopupButton : Button
    {
        public const string InfoBrushKey = "InfoBrushKey"; // { get { return "InfoBrushKey"; } }
        public const string TextOverlayTemplateKey = "TextOverlayTemplateKey"; // { get { return "TextOverlayTemplateKey"; } }
        public const string DefaultOverlayTemplateKey = "DefaultOverlayTemplateKey"; // { get { return "DefaultOverlayTemplateKey"; } }
        public const string MissingToolTipKey = "MissingToolTipKey"; // { get { return "MissingToolTipKey"; } }

        public static readonly DependencyProperty TouchToolTipProperty = DependencyProperty.Register(
            "TouchToolTip",
            typeof(ToolTip),
            typeof(PopupButton),
            new PropertyMetadata(
                default(ToolTip),
                OnTouchToolTipChanged));

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));
        }

        public PopupButton()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnClick), true);
            AddHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocus), true);
        }

        public ToolTip TouchToolTip
        {
            get
            {
                return (ToolTip)GetValue(TouchToolTipProperty);
            }
            set
            {
                SetValue(TouchToolTipProperty, value);
            }
        }

        private static void OnTouchToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var oldTip = e.OldValue as ToolTip;
            if (oldTip != null)
            {
                oldTip.PlacementTarget = null;
            }
            var uiElement = o as UIElement;
            if (uiElement == null)
            {
                return;
            }

            var toolTip = e.NewValue as ToolTip;
            if (toolTip != null)
            {
                toolTip.PlacementTarget = uiElement;
            }
            else
            {
                var touchToolTip = e.NewValue as ITouchToolTip;
                if (touchToolTip != null)
                {
                    touchToolTip.PlacementTarget = uiElement;
                }
            }
        }

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            toolTip.IsOpen = !toolTip.IsOpen;
            Debug.WriteLine("Clicked: {0}, IsOpen: {1}", ((PopupButton)sender).Name, toolTip.IsOpen);
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            //Debug.WriteLine("OnLostFocus");
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            var close = toolTip.IsOpen && !(this.IsKeyboardFocusWithin || toolTip.IsKeyboardFocusWithin);
            Debug.Print(
                "{0}.LostFocus toolTip.IsOpen: {1} && (this.IsKeyboardFocusWithin: {2} || toolTip.IsKeyboardFocusWithin: {3}): {4}",
                ((PopupButton)sender).Name,
                toolTip.IsOpen,
                this.IsKeyboardFocusWithin,
                toolTip.IsKeyboardFocusWithin,
                close);

            if (close)
            {
                toolTip.IsOpen = false;
                Debug.WriteLine(toolTip.IsOpen);
            }
        }
    }
}
