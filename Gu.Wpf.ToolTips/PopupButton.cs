namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public partial class PopupButton : Button
    {
        private static readonly DependencyPropertyKey AdornedElementPropertyKey = DependencyProperty.RegisterReadOnly(
            "AdornedElement",
            typeof(UIElement),
            typeof(PopupButton),
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

        private DateTimeOffset _lastChangeTime = DateTimeOffset.Now;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));

            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipOpening), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipClosingEvent, new ToolTipEventHandler(OnToolTipClosing), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), MouseLeaveEvent, new RoutedEventHandler(OnMouseLeave), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDown), true);
            EventManager.RegisterClassHandler(typeof(PopupButton), Button.UnloadedEvent, new RoutedEventHandler(OnUnloaded));
        }

        public UIElement AdornedElement
        {
            get { return (UIElement)GetValue(AdornedElementProperty); }
            internal set { SetValue(AdornedElementPropertyKey, value); }
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
            popupButton?.OnToolTipOpening(e);
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
            var timeSpan = DateTimeOffset.Now - _lastChangeTime;

            if (timeSpan.TotalMilliseconds < betweenShowDelay)
            {
                Debug.WriteLine("DateTimeOffset.Now - LastChangeTime < TimeSpan.FromMilliseconds(10)");
                return;
            }
            if (toolTip.IsOpen)
            {
                toolTip.IsOpen = false;
            }
            else
            {
                OpenToolTip();
            }
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

        private void OnToolTipOpening(ToolTipEventArgs e)
        {
            OpenToolTip();
            OnToolTipChanged();
            // the framework sets PlacementTarget to this
            // We want it to be AdornedElement if any.
            // e.Handled = true and toolTip.IsOpen = true; worked. Not very elegant.
            e.Handled = true; 
        }

        private void OpenToolTip()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip != null && AdornedElement != null)
            {
                toolTip.PlacementTarget = AdornedElement;
                Debug.Print("Set placement target: {0}", AdornedElement?.GetType().Name ?? "null");
                toolTip.IsOpen = true;
            }
        }

        private void OnToolTipChanged()
        {
            _lastChangeTime = DateTimeOffset.Now;
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
