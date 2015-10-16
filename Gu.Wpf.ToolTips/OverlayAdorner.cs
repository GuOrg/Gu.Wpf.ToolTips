namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public sealed class OverlayAdorner : Adorner
    {
        private PopupButton _popupButton;

        static OverlayAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayAdorner), new FrameworkPropertyMetadata(typeof(OverlayAdorner)));
        }

        /// <summary>
        /// Be sure to call the base class constructor. 
        /// </summary>
        /// <param name="adornedElement">The ui element to adorn</param>
        /// <param name="toolTip">The tooltip to show on click</param>
        /// <param name="overlayTemplate">A style for a PopupButton</param>
        public OverlayAdorner(UIElement adornedElement, ToolTip toolTip, ControlTemplate overlayTemplate)
            : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement should not be null");
            _popupButton = new PopupButton
            {
                IsTabStop = false,
                AdornedElement = adornedElement
            };
            if (overlayTemplate != null)
            {
                _popupButton.Template = overlayTemplate;
            }
            if (toolTip != null)
            {
                _popupButton.ToolTip = toolTip;
            }
            AddVisualChild(_popupButton);
        }

        /// <summary>
        /// The clear the single child of a TemplatedAdorner
        /// </summary>
        public void ClearChild()
        {
            RemoveVisualChild(_popupButton);
            _popupButton = null;
        }

        protected override int VisualChildrenCount => _popupButton != null ? 1 : 0;

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
                throw new ArgumentOutOfRangeException(nameof(index), index, "nope: _child == null || index != 0");
            }

            return _popupButton;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Debug.Assert(_popupButton != null, "_child should not be null");
            AdornedElement?.InvalidateMeasure();
            _popupButton.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            return _popupButton.DesiredSize;
        }

        protected override Size ArrangeOverride(Size size)
        {
            var finalSize = base.ArrangeOverride(size);
            _popupButton?.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }
    }
}
