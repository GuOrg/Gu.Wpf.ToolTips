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
    /// Enables using it in styles.
    /// </summary>
    public static class TouchToolTipService
    {
        /// <summary>
        /// Style used to overlay the control on the AdornerLayer.
        /// Must be TargetType <see cref="PopupButton"/>.
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
        /// Control when the adorner should be visible.
        /// </summary>
        public static readonly DependencyProperty IsOverlayVisibleProperty = DependencyProperty.RegisterAttached(
            "IsOverlayVisible",
            typeof(bool?),
            typeof(TouchToolTipService),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnIsOverlayVisibleChanged));

        /// <summary>
        /// The tooltip contents.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
            "ToolTip",
            typeof(ToolTip),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(ToolTip),
                OnToolTipChanged));

        /// <summary>
        /// Dummy property to get notifications on when Visibility should toggle if Visibility is set to null.
        /// </summary>
        private static readonly DependencyProperty DefaultVisibleProxyProperty = DependencyProperty.RegisterAttached(
            "DefaultVisibleProxy",
            typeof(object),
            typeof(TouchToolTipService),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.None,
                (o, e) => UpdateOverlayVisibility((UIElement)o)));

        /// <summary>
        /// This is used to get notification on visibility changes of adorned element.
        /// </summary>
        private static readonly DependencyProperty IsAdornedElementVisibleProperty = DependencyProperty.RegisterAttached(
            "IsAdornedElementVisible",
            typeof(bool),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                default(bool),
                (o, e) => UpdateOverlayVisibility((UIElement)o)));

        /// <summary>
        /// Reference to the ToolTipAdorner.
        /// </summary>
        private static readonly DependencyProperty ToolTipAdornerProperty = DependencyProperty.RegisterAttached(
            "ToolTipAdorner",
            typeof(OverlayAdorner),
            typeof(TouchToolTipService),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.NotDataBindable));

        /// <summary>Helper for getting <see cref="OverlayTemplateProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OverlayTemplateProperty"/> from.</param>
        /// <returns>OverlayTemplate property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ControlTemplate GetOverlayTemplate(UIElement element)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return (ControlTemplate)element.GetValue(OverlayTemplateProperty);
        }

        /// <summary>Helper for setting <see cref="OverlayTemplateProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OverlayTemplateProperty"/> on.</param>
        /// <param name="value">OverlayTemplate property value.</param>
        public static void SetOverlayTemplate(UIElement element, ControlTemplate value)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            element.SetValue(OverlayTemplateProperty, value);
        }

        /// <summary>Helper for setting <see cref="ToolTipProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="ToolTipProperty"/> on.</param>
        /// <param name="value">ToolTip property value.</param>
        public static void SetToolTip(UIElement element, ToolTip value)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            element.SetValue(ToolTipProperty, value);
        }

        /// <summary>Helper for getting <see cref="ToolTipProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="ToolTipProperty"/> from.</param>
        /// <returns>ToolTip property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static ToolTip GetToolTip(UIElement element)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return (ToolTip)element.GetValue(ToolTipProperty);
        }

        /// <summary>Helper for setting <see cref="IsOverlayVisibleProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsOverlayVisibleProperty"/> on.</param>
        /// <param name="value">IsOverlayVisible property value.</param>
        public static void SetIsOverlayVisible(DependencyObject element, bool? value)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            element.SetValue(IsOverlayVisibleProperty, value);
        }

        /// <summary>Helper for getting <see cref="IsOverlayVisibleProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="IsOverlayVisibleProperty"/> from.</param>
        /// <returns>IsOverlayVisible property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool? GetIsOverlayVisible(UIElement element)
        {
            if (element is null)
            {
                throw new System.ArgumentNullException(nameof(element));
            }

            return (bool?)element.GetValue(IsOverlayVisibleProperty);
        }

        // ReSharper disable once UnusedMember.Local
        private static void SetIsAdornedElementVisible(UIElement element, bool value)
        {
            element.SetValue(IsAdornedElementVisibleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
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
                    _ = targetElement.Dispatcher.BeginInvoke(
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
            ShowOverlayAdorner(targetElement, show, tryAgain: false);
            return null;
        }

        private static void OnIsOverlayVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UIElement uiElement)
            {
                UpdateOverlayVisibility(uiElement);
            }
        }

        private static void UpdateOverlayVisibility(UIElement element)
        {
            var isAdornedElementVisible = GetIsAdornedElementVisible(element);
            if (!isAdornedElementVisible)
            {
                ShowOverlayAdorner(element, show: false, tryAgain: true);
                return;
            }

            var toolTip = GetToolTip(element);
            if (toolTip != null)
            {
                var visible = (bool?)element.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(element);
                ShowOverlayAdorner(element, visible, tryAgain: true);
            }
            else
            {
                ShowOverlayAdorner(element, show: false, tryAgain: false);
            }
        }

        private static void OnOverlayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowOverlayAdorner(d, show: false, tryAgain: true);
            var visible = (bool?)d.GetValue(IsOverlayVisibleProperty) ?? GetDefaultVisible(d);
            ShowOverlayAdorner(d, visible, tryAgain: true);
        }

        private static void OnToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UIElement target)
            {
                ToolTipService.SetToolTip(target, e.NewValue);
                _ = BindingOperations.SetBinding(target, IsAdornedElementVisibleProperty, target.CreateOneWayBinding(UIElement.IsVisibleProperty));
                UpdateOverlayVisibility(target);
            }
        }

        private static bool GetDefaultVisible(DependencyObject o)
        {
            if (o is ButtonBase buttonBase)
            {
                _ = BindingOperations.SetBinding(o, DefaultVisibleProxyProperty, buttonBase.CreateOneWayBinding(UIElement.IsEnabledProperty));
                return buttonBase.IsEnabled != true;
            }

            return true;
        }
    }
}
