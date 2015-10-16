namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public partial class PopupButton : Button
    {
        private static readonly DependencyPropertyKey AdornedElementPropertyKey = DependencyProperty.RegisterReadOnly(
            "AdornedElement",
            typeof(UIElement),
            typeof(PopupButton),
            new PropertyMetadata(default(UIElement), OnAdornedElementChanged));

        public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey AdornedElementTypePropertyKey = DependencyProperty.RegisterReadOnly(
            "AdornedElementType",
            typeof(AdornedElementType?),
            typeof(PopupButton),
            new PropertyMetadata(null));

        public static readonly DependencyProperty AdornedElementTypeProperty = AdornedElementTypePropertyKey.DependencyProperty;

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

        public AdornedElementType? AdornedElementType
        {
            get { return (AdornedElementType?)GetValue(AdornedElementTypeProperty); }
            protected set { SetValue(AdornedElementTypePropertyKey, value); }
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

        private static void OnAdornedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popupButton = (PopupButton)d;
            if (e.NewValue == null)
            {
                popupButton.AdornedElementType = null;
            }
            else if(e.NewValue is ButtonBase)
            {
                popupButton.AdornedElementType = ToolTips.AdornedElementType.Button;
            }
            else if(e.NewValue is TextBoxBase || e.NewValue is Label || e.NewValue is TextBlock)
            {
                popupButton.AdornedElementType = ToolTips.AdornedElementType.Text;
            }
            else
            {
                popupButton.AdornedElementType = ToolTips.AdornedElementType.Other;
            }
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
            // the framework sets PlacementTarget to this when opened with mousover.
            // We want it to be AdornedElement if any.
            // e.Handled = true and toolTip.IsOpen = true; worked. Not very elegant.
            e.Handled = true; 
        }

        private void OpenToolTip()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null)
            {
                return;
            }
            if (AdornedElement != null)
            {
                toolTip.PlacementTarget = AdornedElement;
                Debug.Print("Set placement target: {0}", AdornedElement?.GetType().Name ?? "null");
            }
            toolTip.IsOpen = true;
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
