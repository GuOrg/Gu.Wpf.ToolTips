namespace Gu.Wpf.ToolTips
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Threading;

    /// <summary>
    /// Helper class with attached properties for the TouchToolTipAdorner
    /// Enables using it in syles
    /// </summary>
    public static class TouchToolTipService
    {
        /// <summary>
        /// Style used to overlay the control on the AdornerLayer.
        /// Must be TargetType= popupbutton
        /// </summary>
        public static readonly DependencyProperty OverlayTemplateProperty =
            DependencyProperty.RegisterAttached(
                "OverlayTemplate",
                typeof(ControlTemplate),
                typeof(TouchToolTipService),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.NotDataBindable)
                {
                    PropertyChangedCallback = OnAdornerTemplateChanged
                });

        /// <summary>
        /// Control when the adorner should be visible
        /// </summary>
        public static readonly DependencyProperty IsOverlayVisibleProperty = DependencyProperty.RegisterAttached(
            "IsOverlayVisible",
            typeof(bool?),
            typeof(TouchToolTipService),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnIsVisibleChanged));

        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
            "ToolTip",
            typeof(ToolTip),
            typeof(TouchToolTipService),
            new PropertyMetadata(default(ToolTip), OnToolTipChanged));

        /// <summary>
        /// Gets or sets if the touch tooltip should also be used as mouseover tooltip
        /// </summary>
        public static readonly DependencyProperty UseTouchToolTipAsMouseOverToolTipProperty =
            DependencyProperty.RegisterAttached(
                "UseTouchToolTipAsMouseOverToolTip",
                typeof(bool),
                typeof(TouchToolTipService),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.Inherits,
                    OnUseTouchToolTipAsToolTipChanged));

        /// <summary>
        /// Dummy property to get notificatins on when Visibility should toggle if Visibility is set to null
        /// </summary>
        private static readonly DependencyProperty DefaultVisibleProxyProperty =
            DependencyProperty.RegisterAttached(
                "DefaultVisibleProxy",
                typeof(object),
                typeof(TouchToolTipService),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    OnDefaultVisibleProxChanged));

        private static readonly DependencyProperty IsAdornedElementVisibleProperty = DependencyProperty.RegisterAttached(
            "IsAdornedElementVisible",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(default(bool), OnAdornedElementVisibleChanged));

        /// <summary>
        ///     Reference to the ValidationAdorner
        /// </summary>
        private static readonly DependencyProperty ToolTipAdornerProperty =
            DependencyProperty.RegisterAttached(
                "ToolTipAdorner",
                typeof(TouchToolTipAdorner),
                typeof(TouchToolTipService),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ControlTemplate GetOverlayTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(OverlayTemplateProperty);
        }

        public static void SetOverlayTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(OverlayTemplateProperty, value);
        }

        public static void SetToolTip(DependencyObject element, ToolTip value)
        {
            element.SetValue(ToolTipProperty, value);
        }

        public static ToolTip GetToolTip(DependencyObject element)
        {
            return (ToolTip)element.GetValue(ToolTipProperty);
        }

        public static void SetUseTouchToolTipAsMouseOverToolTip(DependencyObject element, bool value)
        {
            element.SetValue(UseTouchToolTipAsMouseOverToolTipProperty, value);
        }

        public static bool GetUseTouchToolTipAsMouseOverToolTip(DependencyObject element)
        {
            return (bool)element.GetValue(UseTouchToolTipAsMouseOverToolTipProperty);
        }

        public static void SetIsOverlayVisible(DependencyObject element, bool? value)
        {
            element.SetValue(IsOverlayVisibleProperty, value);
        }

        public static bool? GetIsOverlayVisible(DependencyObject element)
        {
            return (bool?)element.GetValue(IsOverlayVisibleProperty);
        }

        private static void SetIsAdornedElementVisible(DependencyObject element, bool value)
        {
            element.SetValue(IsAdornedElementVisibleProperty, value);
        }

        private static bool GetIsAdornedElementVisible(DependencyObject element)
        {
            return (bool)element.GetValue(IsAdornedElementVisibleProperty);
        }

        private static void ShowAdorner(DependencyObject targetElement, bool show, bool tryAgain)
        {
            var uiElement = targetElement as UIElement;

            if (uiElement != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);

                if (adornerLayer == null)
                {
                    if (tryAgain)
                    {
                        // try again later, perhaps giving layout a chance to create the adorner layer
                        targetElement.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                                    new DispatcherOperationCallback(ShowAdornerOperation),
                                    new object[] { targetElement, show });
                    }
                    return;
                }

                var adorner = uiElement.ReadLocalValue(ToolTipAdornerProperty) as TouchToolTipAdorner;

                if (show && adorner == null)
                {
                    var overlayTemplate = GetOverlayTemplate(uiElement);
                    var toolTip = GetToolTip(uiElement);
                    if (overlayTemplate == null && toolTip != null)
                    {
                        if (uiElement is TextBlock || uiElement is Label)
                        {
                            overlayTemplate = (ControlTemplate)Application.Current.FindResource(PopupButton.TextOverlayTemplateKey);
                        }
                    }
                    adorner = new TouchToolTipAdorner(uiElement, toolTip, overlayTemplate);
                    if (GetUseTouchToolTipAsMouseOverToolTip(uiElement))
                    {
                        ToolTipService.SetToolTip(adorner, toolTip);
                        ToolTipService.SetShowOnDisabled(adorner, true);
                    }
                    adornerLayer.Add(adorner);
                    uiElement.SetValue(ToolTipAdornerProperty, adorner);
                }
                else if (!show && adorner != null)
                {
                    adorner.ClearChild();
                    adornerLayer.Remove(adorner);
                    uiElement.ClearValue(ToolTipAdornerProperty);
                }
            }
        }

        private static object ShowAdornerOperation(object arg)
        {
            object[] args = (object[])arg;
            var targetElement = (DependencyObject)args[0];
            var show = (bool)args[1];
            ShowAdorner(targetElement, show, false);
            return null;
        }

        private static void OnIsVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisibility(o);
        }
        private static void UpdateVisibility(DependencyObject o)
        {
            var isAdornedElementVisible = GetIsAdornedElementVisible(o);
            if (!isAdornedElementVisible)
            {
                ShowAdorner(o, false, true);
                return;
            }
            var toolTip = GetToolTip(o);
            if (toolTip != null)
            {
                var visible = (bool?)o.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(o);
                ShowAdorner(o, visible, true);
            }
        }

        private static void OnDefaultVisibleProxChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisibility(o);
        }

        private static void OnAdornedElementVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisibility(o);
        }

        private static void OnAdornerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowAdorner(d, false, true);
            var visible = (bool?)d.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(d);
            ShowAdorner(d, visible, true);
        }

        private static void OnToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UIElement;
            if (target == null || e.NewValue == null)
            {
                return;
            }
            var isVisibleBinding = new Binding(UIElement.IsVisibleProperty.Name)
            {
                Source = target,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(target, IsAdornedElementVisibleProperty, isVisibleBinding);
            var touchToolTip = e.NewValue as TouchToolTip;

            if (touchToolTip != null)
            {
                touchToolTip.ToolTipFor = target;
            }
            else
            {
                var newTip = e.NewValue as ToolTip;
                if (newTip != null)
                {
                    if (newTip.DataContext == null)
                    {
                        var binding = new Binding(FrameworkElement.DataContextProperty.Name)
                        {
                            Source = o,
                            Mode = BindingMode.OneWay
                        };
                        BindingOperations.SetBinding(newTip, FrameworkElement.DataContextProperty, binding);
                    }
                }
            }
            UpdateVisibility(o);
        }

        private static void OnUseTouchToolTipAsToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = o as UIElement;
            if (uiElement == null)
            {
                return;
            }
            var touchToolTip = GetToolTip(o);
            var toolTip = ToolTipService.GetToolTip(o);
            if (Equals(e.NewValue, true) && touchToolTip != null && toolTip == null)
            {
                ToolTipService.SetToolTip(o, touchToolTip);
                var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(uiElement);
                    if (adorners != null)
                    {
                        var touchToolTipAdorner = adorners.OfType<TouchToolTipAdorner>()
                                      .FirstOrDefault();
                        if (touchToolTipAdorner != null)
                        {
                            ToolTipService.SetToolTip(touchToolTipAdorner, touchToolTip);
                            ToolTipService.SetShowOnDisabled(touchToolTipAdorner, true);
                        }
                    }
                }
            }

            if (Equals(e.NewValue, false))
            {
                if (ReferenceEquals(touchToolTip, toolTip))
                {
                    ToolTipService.SetToolTip(o, null);
                }
                var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(uiElement);
                    if (adorners != null)
                    {
                        var touchToolTipAdorner = adorners
                                      .OfType<TouchToolTipAdorner>()
                                      .FirstOrDefault();
                        if (touchToolTipAdorner != null)
                        {
                            ToolTipService.SetToolTip(touchToolTipAdorner, null);
                        }
                    }
                }
            }
        }

        private static bool GetDefaultVisible(DependencyObject o)
        {
            if (o is TextBlock || o is Label)
            {
                return true;
            }
            var buttonBase = o as ButtonBase;
            if (buttonBase != null)
            {
                var binding = new Binding(UIElement.IsEnabledProperty.Name)
                {
                    Source = buttonBase,
                    Mode = BindingMode.OneWay
                };
                BindingOperations.SetBinding(o, DefaultVisibleProxyProperty, binding);
                return buttonBase.IsEnabled != true;
            }
            return false;
        }
    }
}
