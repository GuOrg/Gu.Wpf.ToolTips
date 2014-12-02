﻿namespace Gu.Wpf.ToolTips
{
    using System;
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
        ///     Style used to overlay the control on the AdornerLayer.
        ///     Must be TargetType popupbutton
        /// </summary>
        public static readonly DependencyProperty OverlayTemplateProperty =
            DependencyProperty.RegisterAttached(
                "OverlayTemplate",
                typeof(ControlTemplate),
                typeof(TouchToolTipService),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.NotDataBindable)
                    {
                        PropertyChangedCallback = OnAdornerTemplateChanged
                    });

        /// <summary>
        ///     Control when the adorner should be visible
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible",
            typeof(bool?),
            typeof(TouchToolTipService),
            new PropertyMetadata(
                null,
                OnIsVisibleChanged));

        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
            "ToolTip",
            typeof(ToolTip),
            typeof(TouchToolTipService),
            new PropertyMetadata(default(ToolTip), OnToolTipChanged));

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

        /// <summary> Static accessor for AdornerTemplate property </summary>
        /// <exception cref="ArgumentNullException"> DependencyObject element cannot be null </exception>
        public static ControlTemplate GetOverlayTemplate(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return element.GetValue(OverlayTemplateProperty) as ControlTemplate;
        }

        /// <summary> Static modifier for AdornerTemplate property </summary>
        /// <exception cref="ArgumentNullException"> DependencyObject element cannot be null </exception>
        public static void SetOverlayTemplate(DependencyObject element, ControlTemplate value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            var oldValue = element.ReadLocalValue(OverlayTemplateProperty);
            if (!Equals(oldValue, value))
            {
                element.SetValue(OverlayTemplateProperty, value);
            }
        }

        public static void SetToolTip(DependencyObject element, ToolTip value)
        {
            element.SetValue(ToolTipProperty, value);
        }

        public static ToolTip GetToolTip(DependencyObject element)
        {
            return (ToolTip)element.GetValue(ToolTipProperty);
        }

        public static void SetIsVisible(DependencyObject element, bool? value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool? GetIsVisible(DependencyObject element)
        {
            return (bool?)element.GetValue(IsVisibleProperty);
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
            var toolTip = GetToolTip(o);
            if (toolTip != null)
            {
                var visible = (bool?)o.GetValue(IsVisibleProperty) ?? GetDefaultVisible(o);
                ShowAdorner(o, visible, true);
            }
        }

        private static void OnDefaultVisibleProxChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var toolTip = GetToolTip(o);
            if (toolTip != null)
            {
                var visible = (bool?)o.GetValue(IsVisibleProperty) ?? GetDefaultVisible(o);
                ShowAdorner(o, visible, true);
            }
        }

        private static void OnAdornerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShowAdorner(d, false, true);
            var visible = (bool?)d.GetValue(IsVisibleProperty) ?? GetDefaultVisible(d);
            ShowAdorner(d, visible, true);
        }

        private static void OnToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var target = o as UIElement;
            var touchToolTip = e.NewValue as ITouchToolTip;

            if (touchToolTip != null && target != null)
            {
                touchToolTip.OnToolTipChanged(target);
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
            var visible = (bool?)o.GetValue(IsVisibleProperty) ?? GetDefaultVisible(o);
            ShowAdorner(o, visible, true);
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
