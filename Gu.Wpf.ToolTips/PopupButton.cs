namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class PopupButton : Button
    {
        public const string TextOverlayTemplateKey = "TextOverlayTemplateKey"; // { get { return "TextOverlayTemplateKey"; } }
        public const string DefaultOverlayTemplateKey = "DefaultOverlayTemplateKey"; //{ get { return "DefaultOverlayTemplateKey"; } }
        public const string MissingToolTipKey = "MissingToolTipKey"; // { get { return "MissingToolTipKey"; } }

        internal DateTimeOffset LastChangeTime { get; private set; } = DateTimeOffset.Now;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipChanged), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipClosingEvent, new ToolTipEventHandler(OnToolTipChanged), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), MouseLeaveEvent, new RoutedEventHandler(OnMouseLeave), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDown), true);
        }

        public PopupButton()
        {
            IsVisibleChanged += OnIsVisibleChanged;
        }

        private static void OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnPreviewMouseLeftButtonDown();
        }

        private static void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnMouseLeave();
        }

        private static void OnToolTipChanged(object sender, ToolTipEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnToolTipChanged();
        }

        private void OnPreviewMouseLeftButtonDown()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null)
            {
                return;
            }

            var betweenShowDelay = ToolTipService.GetBetweenShowDelay(toolTip);
            var timeSpan = DateTimeOffset.Now - LastChangeTime;

            if (timeSpan.TotalMilliseconds < betweenShowDelay)
            {
                Debug.WriteLine("DateTimeOffset.Now - LastChangeTime < TimeSpan.FromMilliseconds(10)");
                return;
            }

            toolTip.IsOpen = !toolTip.IsOpen;
            Debug.WriteLine("Clicked: {0}, IsOpen: {1}", Name, toolTip.IsOpen);
        }

        private void OnMouseLeave()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null || !toolTip.IsOpen)
            {
                return;
            }
            toolTip.IsOpen = false;
            Debug.WriteLine("OnMouseLeave: {0}", toolTip.IsOpen);
        }

        private void OnToolTipChanged()
        {
            LastChangeTime = DateTimeOffset.Now;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }
    }
}
