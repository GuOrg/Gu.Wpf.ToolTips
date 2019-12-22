namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// A button that is used in the tooltip overlay.
    /// </summary>
    public partial class PopupButton : Button
    {
#pragma warning disable SA1202 // Elements must be ordered by access

        private static readonly DependencyPropertyKey AdornedElementPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(AdornedElement),
            typeof(UIElement),
            typeof(PopupButton),
            new PropertyMetadata(default(UIElement), OnAdornedElementChanged));

        /// <summary>Identifies the <see cref="AdornedElement"/> dependency property.</summary>
        public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey AdornedElementTypePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(AdornedElementType),
            typeof(AdornedElementType?),
            typeof(PopupButton),
            new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="AdornedElementType"/> dependency property.</summary>
        public static readonly DependencyProperty AdornedElementTypeProperty = AdornedElementTypePropertyKey.DependencyProperty;

#pragma warning restore SA1202 // Elements must be ordered by access

        private DateTimeOffset lastChangeTime = DateTimeOffset.Now;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));

            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipOpeningEvent, new ToolTipEventHandler(OnToolTipOpening), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(PopupButton), ToolTipService.ToolTipClosingEvent, new ToolTipEventHandler(OnToolTipClosing), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(PopupButton), MouseLeaveEvent, new RoutedEventHandler(OnMouseLeave), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(PopupButton), PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDown), handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(PopupButton), UnloadedEvent, new RoutedEventHandler(OnUnloaded));
        }

        /// <summary>
        /// Element for which the AdornedElementPlaceholder is reserving space.
        /// </summary>
        public UIElement AdornedElement
        {
            get => (UIElement)this.GetValue(AdornedElementProperty);
            internal set => this.SetValue(AdornedElementPropertyKey, value);
        }

        /// <summary>
        /// The type of <see cref="AdornedElement"/>.
        /// </summary>
        public AdornedElementType? AdornedElementType
        {
            get => (AdornedElementType?)this.GetValue(AdornedElementTypeProperty);
            protected set => this.SetValue(AdornedElementTypePropertyKey, value);
        }

        /// <inheritdoc/>
        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            this.OpenToolTip();
            this.OnToolTipChanged();

            // the framework sets PlacementTarget to this when opened with mouse over.
            // We want it to be AdornedElement if any.
            // e.Handled = true and toolTip.IsOpen = true; worked. Not very elegant.
            e.Handled = true;
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
            popupButton.AdornedElementType = e.NewValue switch
            {
                null => null,
                ButtonBase _ => ToolTips.AdornedElementType.Button,
                TextBoxBase _ => ToolTips.AdornedElementType.Text,
                Label _ => ToolTips.AdornedElementType.Text,
                TextBlock _ => ToolTips.AdornedElementType.Text,
                _ => ToolTips.AdornedElementType.Other
            };
        }

        private void OnPreviewMouseLeftButtonDown()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null)
            {
                return;
            }

            var betweenShowDelay = ToolTipService.GetBetweenShowDelay(toolTip);
            var timeSpan = DateTimeOffset.Now - this.lastChangeTime;

            if (timeSpan.TotalMilliseconds < betweenShowDelay)
            {
                Debug.WriteLine("DateTimeOffset.Now - LastChangeTime < TimeSpan.FromMilliseconds(10)");
                return;
            }

            if (toolTip.IsOpen)
            {
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.IsOpenProperty, false);
            }
            else
            {
                this.OpenToolTip();
            }

            Debug.WriteLine("Clicked: {0}, IsOpen: {1}", this.Name, toolTip.IsOpen);
        }

        private void OnMouseLeave()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null || !toolTip.IsOpen)
            {
                return;
            }

            toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.IsOpenProperty, false);
            Debug.WriteLine("OnMouseLeave: {0}", toolTip.IsOpen);
        }

        private void OpenToolTip()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip == null)
            {
                return;
            }

            if (this.AdornedElement != null)
            {
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.PlacementTargetProperty, this.AdornedElement);
                Debug.Print("Set placement target: {0}", this.AdornedElement?.GetType().Name ?? "null");
            }

            toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.IsOpenProperty, true);
        }

        private void OnToolTipChanged()
        {
            this.lastChangeTime = DateTimeOffset.Now;
        }

        private void OnUnloaded()
        {
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            toolTip?.SetCurrentValue(System.Windows.Controls.ToolTip.IsOpenProperty, false);
        }
    }
}
