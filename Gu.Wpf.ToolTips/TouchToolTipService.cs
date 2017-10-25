namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Threading;

    /// <summary>
    /// Helper class with attached properties for the TouchToolTipAdorner
    /// Enables using it in styles
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
                    FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.NotDataBindable,
                    OnOverlayTemplateChanged));

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
                OnIsOverlayVisibleChanged));

        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
            "ToolTip",
            typeof(ToolTip),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(ToolTip),
                OnToolTipChanged));

        /// <summary>
        /// Dummy property to get notifications on when Visibility should toggle if Visibility is set to null
        /// </summary>
        private static readonly DependencyProperty DefaultVisibleProxyProperty = DependencyProperty.RegisterAttached(
            "DefaultVisibleProxy",
            typeof(object),
            typeof(TouchToolTipService),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.None,
                OnDefaultVisibleProxyChanged));

        /// <summary>
        /// This is used to get notification on visibility changes of adornedelement
        /// </summary>
        private static readonly DependencyProperty IsAdornedElementVisibleProperty = DependencyProperty.RegisterAttached(
            "IsAdornedElementVisible",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(bool),
                OnIsAdornedElementVisibleChanged));

        /// <summary>
        /// Reference to the ToolTipAdorner
        /// </summary>
        private static readonly DependencyProperty ToolTipAdornerProperty = DependencyProperty.RegisterAttached(
            "ToolTipAdorner",
            typeof(OverlayAdorner),
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

        public static void SetToolTip(UIElement element, ToolTip value)
        {
            element.SetValue(ToolTipProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ToolTip GetToolTip(UIElement element)
        {
            return (ToolTip)element.GetValue(ToolTipProperty);
        }

        public static void SetIsOverlayVisible(DependencyObject element, bool? value)
        {
            element.SetValue(IsOverlayVisibleProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool? GetIsOverlayVisible(UIElement element)
        {
            return (bool?)element.GetValue(IsOverlayVisibleProperty);
        }

        // ReSharper disable once UnusedMember.Local
        private static void SetIsAdornedElementVisible(UIElement element, bool value)
        {
            element.SetValue(IsAdornedElementVisibleProperty, value);
        }

        private static bool GetIsAdornedElementVisible(DependencyObject element)
        {
            return (bool)element.GetValue(IsAdornedElementVisibleProperty);
        }

        private static void ShowOverlayAdorner(DependencyObject targetElement, bool show, bool tryAgain)
        {
            var uiElement = targetElement as UIElement;
            if (uiElement == null)
            {
                return;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);

            if (adornerLayer == null)
            {
                if (tryAgain)
                {
                    // try again later, perhaps giving layout a chance to create the adorner layer
                    targetElement.Dispatcher.BeginInvoke(
                                     DispatcherPriority.Loaded,
                                     new DispatcherOperationCallback(ShowAdornerOperation),
                                     new object[] { targetElement, show });
                }

                return;
            }

            var adorner = uiElement.ReadLocalValue(ToolTipAdornerProperty) as OverlayAdorner;

            if (show && adorner == null)
            {
                var overlayTemplate = GetOverlayTemplate(uiElement);
                var toolTip = ToolTipService.GetToolTip(uiElement) as ToolTip;
                adorner = new OverlayAdorner(uiElement, toolTip, overlayTemplate);
                adornerLayer.Add(adorner);
                uiElement.SetCurrentValue(ToolTipAdornerProperty, adorner);
            }
            else if (!show && adorner != null)
            {
                adorner.ClearChild();
                adornerLayer.Remove(adorner);
                uiElement.ClearValue(ToolTipAdornerProperty);
            }
        }

        private static object ShowAdornerOperation(object arg)
        {
            var args = (object[])arg;
            var targetElement = (DependencyObject)args[0];
            var show = (bool)args[1];
            ShowOverlayAdorner(targetElement, show, false);
            return null;
        }

        private static void OnIsOverlayVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = o as UIElement;
            if (uiElement != null)
            {
                UpdateOverlayVisibility(uiElement);
            }
        }

        private static void UpdateOverlayVisibility(UIElement element)
        {
            var isAdornedElementVisible = GetIsAdornedElementVisible(element);
            if (!isAdornedElementVisible)
            {
                ShowOverlayAdorner(element, false, true);
                return;
            }

            var toolTip = GetToolTip(element);
            if (toolTip != null)
            {
                var visible = (bool?)element.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(element);
                ShowOverlayAdorner(element, visible, true);
            }
        }

        private static void OnDefaultVisibleProxyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpdateOverlayVisibility((UIElement)o);
        }

        private static void OnIsAdornedElementVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UpdateOverlayVisibility((UIElement)o);
        }

        private static void OnOverlayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowOverlayAdorner(d, false, true);
            var visible = (bool?)d.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(d);
            ShowOverlayAdorner(d, visible, true);
        }

        private static void OnToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UIElement;
            if (target == null || e.NewValue == null)
            {
                return;
            }

            ToolTipService.SetToolTip(target, e.NewValue);
            BindingOperations.SetBinding(target, IsAdornedElementVisibleProperty, target.CreateOneWayBinding(UIElement.IsVisibleProperty));
            UpdateOverlayVisibility(target);
        }

        private static bool GetDefaultVisible(DependencyObject o)
        {
            var buttonBase = o as ButtonBase;
            if (buttonBase != null)
            {
                BindingOperations.SetBinding(o, DefaultVisibleProxyProperty, buttonBase.CreateOneWayBinding(UIElement.IsEnabledProperty));
                return buttonBase.IsEnabled != true;
            }

            return true;
        }
    }
}
