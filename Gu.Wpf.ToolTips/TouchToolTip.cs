namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Sets DataContext to the DataContext of the adorned element
    /// </summary>
    public class TouchToolTip : ToolTip, ITouchToolTip, INameScope
    {
        private INameScope _nameScope;
        static TouchToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TouchToolTip),
                new FrameworkPropertyMetadata(typeof(TouchToolTip)));
        }

        public void OnToolTipChanged(UIElement adornedElement)
        {
            _nameScope = adornedElement.NameScope();
            var dataContextBinding = new Binding(DataContextProperty.Name)
            {
                Source = adornedElement,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, DataContextProperty, dataContextBinding);
        }

        /// <summary>
        /// Enables ElementName bindings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object System.Windows.Markup.INameScope.FindName(string name)
        {
            if (_nameScope != null)
            {
                return _nameScope.FindName(name);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Enables ElementName bindings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        void System.Windows.Markup.INameScope.RegisterName(string name, object scopedElement)
        {
            if (_nameScope != null)
            {
                _nameScope.RegisterName(name, scopedElement);
            }
        }

        /// <summary>
        /// Enables ElementName bindings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        void System.Windows.Markup.INameScope.UnregisterName(string name)
        {
            if (_nameScope != null)
            {
                _nameScope.UnregisterName(name);
            }
        }
    }
}
