namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    internal sealed class TouchToolTipAdorner : Adorner
    {
        private PopupButton _popupButton;

        private static readonly DependencyPropertyKey AdornedElementTypePropertyKey = DependencyProperty.RegisterReadOnly(
                "AdornedElementType",
                typeof(Type),
                typeof(TouchToolTip),
                new PropertyMetadata(default(Type)));

        public static readonly DependencyProperty AdornedElementTypeProperty = AdornedElementTypePropertyKey.DependencyProperty;

        static TouchToolTipAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTipAdorner), new FrameworkPropertyMetadata(typeof(TouchToolTipAdorner)));
        }

        /// <summary>
        /// Be sure to call the base class constructor. 
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="overlayTemplate">A style for a PopupButton</param>
        public TouchToolTipAdorner(UIElement adornedElement, ToolTip toolTip, ControlTemplate overlayTemplate)
            : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement should not be null");
            //Debug.Assert(overlayTemplate != null, "adornerTemplate should not be null");
            _popupButton = new PopupButton
                          {
                              IsTabStop = false,
                          };
            if (overlayTemplate != null)
            {
                _popupButton.Template = overlayTemplate;
            }
            if (toolTip != null)
            {
                _popupButton.TouchToolTip = toolTip;

                //toolTip.DataContext = adornedElement;
                // Not sure we want ^, check bindings for DataContext and DataContext == null first 
            }
            else
            {
                _popupButton.BorderBrush = Brushes.HotPink;
                _popupButton.BorderThickness = new Thickness(2);
            }
            AddVisualChild(_popupButton);
            AdornedElementType = adornedElement.GetType();
        }

        public Type AdornedElementType
        {
            get
            {
                return (Type)GetValue(AdornedElementTypeProperty);
            }
            protected set
            {
                SetValue(AdornedElementTypePropertyKey, value);
            }
        }

        /// <summary>
        /// The clear the single child of a TemplatedAdorner
        /// </summary>
        public void ClearChild()
        {
            RemoveVisualChild(_popupButton);
            _popupButton = null;
        }

        protected override int VisualChildrenCount
        {
            get { return _popupButton != null ? 1 : 0; }
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark:
        ///       During this virtual call it is not valid to modify the Visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (_popupButton == null || index != 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "nope: _child == null || index != 0");
            }

            return _popupButton;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Debug.Assert(_popupButton != null, "_child should not be null");
            _popupButton.Measure(constraint);
            if (AdornedElement != null)
            {
                AdornedElement.InvalidateMeasure();
                AdornedElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                return AdornedElement.RenderSize;
            }
            _popupButton.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            return (_popupButton).DesiredSize;
        }

        protected override Size ArrangeOverride(Size size)
        {
            Size finalSize;

            finalSize = base.ArrangeOverride(size);

            if (_popupButton != null)
            {
                _popupButton.Arrange(new Rect(new Point(), finalSize));
            }
            return finalSize;
        }
    }
}
