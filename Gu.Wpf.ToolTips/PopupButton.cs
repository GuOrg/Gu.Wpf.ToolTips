namespace Gu.Wpf.ToolTips
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class PopupButton : Button
    {
        public const string TextOverlayTemplateKey = "TextOverlayTemplateKey"; // { get { return "TextOverlayTemplateKey"; } }
        public const string DefaultOverlayTemplateKey = "DefaultOverlayTemplateKey"; //{ get { return "DefaultOverlayTemplateKey"; } }
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
            AddHandler(ClickEvent, new RoutedEventHandler(OnClick), true);
            AddHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocus), true);
           IsVisibleChanged += OnIsVisibleChanged;
        }

        public ToolTip TouchToolTip
        {
            get { return (ToolTip)GetValue(TouchToolTipProperty); }
            set { SetValue(TouchToolTipProperty, value); }
        }

        protected virtual void OnTouchToolTipChanged(ToolTip oldToolTip, ToolTip newToolTip)
        {
            if (oldToolTip != null)
            {
                oldToolTip.PlacementTarget = null;
            }
            if (newToolTip != null)
            {
                newToolTip.PlacementTarget = this;
            }
        }

        private static void OnTouchToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((PopupButton)o).OnTouchToolTipChanged((ToolTip)e.OldValue, (ToolTip)e.NewValue);
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
            var close = toolTip.IsOpen && !(IsKeyboardFocusWithin || toolTip.IsKeyboardFocusWithin);
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

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TouchToolTip.IsOpen = false;
        }
    }
}
