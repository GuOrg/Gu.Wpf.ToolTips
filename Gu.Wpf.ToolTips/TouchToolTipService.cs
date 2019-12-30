namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Attached properties controlling touch tool tips.
    /// </summary>
    public static class TouchToolTipService
    {
#pragma warning disable SA1202 // Elements must be ordered by access

        /// <summary>
        /// Gets or sets whether an <see cref="OverlayAdorner"/> appears.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(default(bool), OnIsEnabledChanged));

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
                default(OverlayAdorner),
                (d, e) => ((OverlayAdorner)e.OldValue)?.ClearChild()));

        static TouchToolTipService()
        {
            EventManager.RegisterClassHandler(
                typeof(OverlayAdorner),
                UIElement.TouchDownEvent,
                new RoutedEventHandler((o, e) =>
                {
                    if (o is OverlayAdorner { AdornedElement: { } element } &&
                        !ToolTipService.GetIsOpen(element))
                    {
                        PopupControlService.Show(element);
                        e.Handled = true;
                    }
                }));

            EventManager.RegisterClassHandler(
                typeof(OverlayAdorner),
                UIElement.PreviewTouchUpEvent,
                new RoutedEventHandler((o, e) =>
                {
                    if (o is OverlayAdorner { AdornedElement: { } element } &&
                        ToolTipService.GetIsOpen(element))
                    {
                        e.Handled = true;
                    }
                }));
        }

        /// <summary>Helper for getting <see cref="IsEnabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsEnabledProperty"/> from.</param>
        /// <returns>IsEnabled property value.</returns>
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

#pragma warning restore SA1202 // Elements must be ordered by access

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
                    Debug.Assert(condition: false, message: $"Element {d} already has an overlay adorner.");
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
                            new Binding { Source = element, Path = BindingHelper.GetPath(FrameworkElement.ToolTipProperty), Mode = BindingMode.OneWay },
                            new Binding { Source = element, Path = BindingHelper.GetPath(ToolTipService.IsEnabledProperty), Mode = BindingMode.OneWay },
                        },
                        Converter = IdentityConverter.Default,
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

        private sealed class IdentityConverter : IMultiValueConverter
        {
            internal static readonly IdentityConverter Default = new IdentityConverter();

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var element = (FrameworkElement)values[0];
                return element is { IsVisible: true, ToolTip: { } } &&
                       ToolTipService.GetIsEnabled(element);
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException($"{nameof(IdentityConverter)} can only be used in OneWay bindings");
            }
        }
    }
}
