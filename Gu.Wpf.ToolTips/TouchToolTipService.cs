namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// Attached properties controlling touch tool tips.
    /// </summary>
    public static class TouchToolTipService
    {
        /// <summary>
        /// Gets or sets whether an <see cref="OverlayAdorner"/> appears.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(bool),
                OnIsEnabledChanged));

        private static readonly DependencyPropertyKey IsOverlayVisiblePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "IsOverlayVisible",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(bool),
                OnIsOverlayVisibleChanged));

        /// <summary>
        /// Gets or sets if the overlay adorner is currently visible.
        /// </summary>
        public static readonly DependencyProperty IsOverlayVisibleProperty = IsOverlayVisiblePropertyKey.DependencyProperty;

        private static readonly DependencyProperty IsOverlayVisibleProxyProperty = DependencyProperty.RegisterAttached(
            "IsOverlayVisibleProxy",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                false,
                (o, e) => o.SetValue(IsOverlayVisiblePropertyKey, e.NewValue)));

        private static readonly DependencyProperty AdornerProperty = DependencyProperty.RegisterAttached(
            "Adorner",
            typeof(OverlayAdorner),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(OverlayAdorner)));

        private static readonly WeakReference<UIElement> Tapped = new(null!);
        private static DispatcherTimer? closeTimer;

        static TouchToolTipService()
        {
            if (typeof(ToolTipService).GetField("FindToolTipEvent", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) is RoutedEvent findToolTipEvent)
            {
                EventManager.RegisterClassHandler(typeof(UIElement), findToolTipEvent, new RoutedEventHandler((_, e) => CancelFindToolTipIfTouch(e)));
                //// For some reason explicit subscription on TextBlock is needed
                EventManager.RegisterClassHandler(typeof(TextBlock), findToolTipEvent, new RoutedEventHandler((_, e) => CancelFindToolTipIfTouch(e)));
                EventManager.RegisterClassHandler(typeof(ContentElement), findToolTipEvent, new RoutedEventHandler((_, e) => CancelFindToolTipIfTouch(e)));
                EventManager.RegisterClassHandler(typeof(UIElement3D), findToolTipEvent, new RoutedEventHandler((_, e) => CancelFindToolTipIfTouch(e)));

                static void CancelFindToolTipIfTouch(RoutedEventArgs e)
                {
                    ////Debug.WriteLine($"FindToolTipEvent: {e} {Stylus.CurrentStylusDevice?.InRange}");
                    if (Stylus.CurrentStylusDevice?.InRange == true)
                    {
                        // Working around the following framework issue
                        // When tool tip is visible and another element is tapped the following happens
                        // 1. Current tool tip is closed with _quickshow = true
                        // 2. _quickshow means that the tool tip for the new element is instantly opened.
                        // 3. The newly opened tool tip is closed when the synthetic mouse down from the touch input is sent.
                        e.Handled = true;
                    }
                }
            }

            EventManager.RegisterClassHandler(
                typeof(UIElement),
                UIElement.StylusSystemGestureEvent,
                new RoutedEventHandler((_, e) =>
                {
                    if (e is StylusSystemGestureEventArgs { SystemGesture: SystemGesture.Tap } tap &&
                        HitTest(tap) is { AdornedElement: { Dispatcher: { } } element })
                    {
                        // Deferring show to StylusOutOfRangeEvent as stylus input triggers synthetic mouse input.
                        Tapped.SetTarget(
                            ToolTipService.GetIsOpen(element) ||
                            ReferenceEquals(element, closeTimer?.Tag)
                                ? null!
                                : element);
                        e.Handled = true;
                    }

                    static OverlayAdorner? HitTest(StylusSystemGestureEventArgs e)
                    {
                        OverlayAdorner? result = null;
                        if (e.Source is Visual source &&
                            AdornerLayer.GetAdornerLayer(source) is { } adornerLayer)
                        {
                            VisualTreeHelper.HitTest(
                                adornerLayer,
                                x => Filter(x),
                                _ => /* Not used. */ HitTestResultBehavior.Stop,
                                new PointHitTestParameters(e.GetPosition(adornerLayer)));

                            HitTestFilterBehavior Filter(DependencyObject potentialHitTestTarget)
                            {
                                switch (potentialHitTestTarget)
                                {
                                    case OverlayAdorner overlayAdorner:
                                        // Using the filter for side effect as the OverlayAdorner is HitTestVisible = false
                                        result = overlayAdorner;
                                        return HitTestFilterBehavior.Stop;
                                    case Adorner _:
                                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                                    default:
                                        return HitTestFilterBehavior.Continue;
                                }
                            }
                        }

                        return result;
                    }
                }));

            EventManager.RegisterClassHandler(
                typeof(UIElement),
                UIElement.StylusOutOfRangeEvent,
                new RoutedEventHandler((_, _) =>
                {
                    if (Tapped.TryGetTarget(out var target))
                    {
                        if (!ToolTipService.GetIsOpen(target))
                        {
                            PopupControlService.ShowToolTip(target);
                            ResetCloseTimer();
                        }

                        Tapped.SetTarget(null!);
                    }
                }));

            EventManager.RegisterClassHandler(
                typeof(FrameworkElement),
                FrameworkElement.ToolTipClosingEvent,
                new RoutedEventHandler((o, _) =>
                {
                    // https://source.dot.net/#PresentationFramework/System/Windows/Controls/Primitives/Popup.cs,2892
                    const int AnimationDelay = 150;
                    ResetCloseTimer();
                    closeTimer = new DispatcherTimer(DispatcherPriority.Normal)
                    {
                        Interval = new TimeSpan(0, 0, 0, 0, AnimationDelay),
                        Tag = o,
                    };

                    closeTimer.Tick += (_, _) => ResetCloseTimer();
                    closeTimer.Start();
                }));

            static void ResetCloseTimer()
            {
                closeTimer?.Stop();
                closeTimer = null;
            }
        }

        /// <summary>Helper for getting <see cref="IsEnabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsEnabledProperty"/> from.</param>
        /// <returns>IsEnabled property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsEnabled(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(IsEnabledProperty);
        }

        /// <summary>Helper for setting <see cref="IsEnabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsEnabledProperty"/> on.</param>
        /// <param name="value">IsEnabled property value.</param>
        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(IsEnabledProperty, value);
        }

        /// <summary>Helper for getting <see cref="IsOverlayVisibleProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="FrameworkElement"/> to read <see cref="IsOverlayVisibleProperty"/> from.</param>
        /// <returns>IsOverlayVisible property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetIsOverlayVisible(this FrameworkElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (bool)element.GetValue(IsOverlayVisibleProperty);
        }

        private static void OnIsOverlayVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(e.NewValue, true) &&
                d is FrameworkElement fe)
            {
                if (d.GetValue(AdornerProperty) is null)
                {
                    var adorner = new OverlayAdorner(fe);
                    d.SetCurrentValue(AdornerProperty, adorner);
                    AdornerService.Show(adorner);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(condition: false, message: $"Element {d} already has an overlay adorner.");
                }
            }
            else if (d.GetValue(AdornerProperty) is OverlayAdorner adorner)
            {
                AdornerService.Remove(adorner);
                d.ClearValue(AdornerProperty);
            }
        }

        private static void OnIsEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is FrameworkElement element)
            {
                if (Equals(e.NewValue, true))
                {
                    var multiBinding = new MultiBinding
                    {
                        Bindings =
                        {
                            new Binding { Source = element, Mode = BindingMode.OneTime },
                            new Binding { Source = element, Path = BindingHelper.GetPath(UIElement.IsVisibleProperty), Mode = BindingMode.OneWay },
                            new Binding { Source = element, Path = BindingHelper.GetPath(ToolTipService.ToolTipProperty), Mode = BindingMode.OneWay },
                            new Binding { Source = element, Path = BindingHelper.GetPath(ToolTipService.IsEnabledProperty), Mode = BindingMode.OneWay },
                        },
                        Converter = IsOverlayVisibleConverter.Default,
                    };
                    _ = BindingOperations.SetBinding(element, IsOverlayVisibleProxyProperty, multiBinding);
                }
                else
                {
                    BindingOperations.ClearBinding(o, IsOverlayVisibleProxyProperty);
                    o.SetValue(IsOverlayVisiblePropertyKey, false);
                }
            }
        }

        private sealed class IsOverlayVisibleConverter : IMultiValueConverter
        {
            internal static readonly IsOverlayVisibleConverter Default = new IsOverlayVisibleConverter();

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var element = (FrameworkElement)values[0];
                return element is { IsVisible: true, ToolTip: { } } &&
                       ToolTipService.GetIsEnabled(element);
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException($"{nameof(IsOverlayVisibleConverter)} can only be used in OneWay bindings");
            }
        }
    }
}
