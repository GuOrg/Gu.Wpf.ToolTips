namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class PopupButton : Button
    {
        public const string TextOverlayTemplateKey = "TextOverlayTemplateKey"; // { get { return "TextOverlayTemplateKey"; } }
        public const string DefaultOverlayTemplateKey = "DefaultOverlayTemplateKey"; //{ get { return "DefaultOverlayTemplateKey"; } }
        public const string MissingToolTipKey = "MissingToolTipKey"; // { get { return "MissingToolTipKey"; } }

        internal UIElement AdornedElement;
        internal DateTimeOffset LastChangeTime { get; private set; } = DateTimeOffset.Now;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipOpening), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipClosingEvent, new ToolTipEventHandler(OnToolTipClosing), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), MouseLeaveEvent, new RoutedEventHandler(OnMouseLeave), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDown), true);
            EventManager.RegisterClassHandler(typeof(PopupButton),Button.UnloadedEvent, new RoutedEventHandler(OnUnloaded));
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

        private static void OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnToolTipOpening();
        }

        private static void OnToolTipClosing(object sender, ToolTipEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnToolTipChanged();
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var popupButton = sender as PopupButton;
            popupButton?.OnUnloaded();
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

        private void OnToolTipOpening()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip != null && AdornedElement != null)
            {
                toolTip.PlacementTarget = AdornedElement;
            }
            OnToolTipChanged();
        }

        private void OnToolTipChanged()
        {
            LastChangeTime = DateTimeOffset.Now;
        }

        private void OnUnloaded()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
        }
    }
}
