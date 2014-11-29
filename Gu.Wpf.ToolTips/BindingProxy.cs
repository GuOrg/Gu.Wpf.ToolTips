namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Binds AdornedElement to adornedElement;
    /// Binds DataContext to adornedElement.DataContext
    /// </summary>
    public class BindingProxy : Visual
    {
        public static readonly DependencyProperty DataContextProperty = FrameworkElement.DataContextProperty.AddOwner(
            typeof(BindingProxy),
            new PropertyMetadata(null));
        private WeakReference _adornedElementRef = new WeakReference(null);
        public UIElement AdornedElement
        {
            get
            {
                return (UIElement) _adornedElementRef.Target;
            }
        }

        public object DataContext
        {
            get { return (object)this.GetValue(DataContextProperty); }
            set { this.SetValue(DataContextProperty, value); }
        }

        /// <summary>
        /// Binds AdornedElement to adornedElement;
        /// Binds DataContext to adornedElement.DataContext
        /// </summary>
        public BindingProxy(UIElement adornedElement)
        {
            _adornedElementRef.Target = adornedElement;

            var frameworkElement = adornedElement as FrameworkElement;
            if (frameworkElement != null)
            {
                var binding = new Binding(DataContextProperty.Name)
                {
                    Mode = BindingMode.OneWay,
                    Source = frameworkElement
                };
                BindingOperations.SetBinding(adornedElement, DataContextProperty, binding);
            }
        }
    }
}
