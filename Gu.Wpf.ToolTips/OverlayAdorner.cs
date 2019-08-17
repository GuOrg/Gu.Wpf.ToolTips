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
        private PopupButton popupButton;

        static OverlayAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayAdorner), new FrameworkPropertyMetadata(typeof(OverlayAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAdorner"/> class.
        /// Be sure to call the base class constructor.
        /// </summary>
        /// <param name="adornedElement">The ui element to adorn.</param>
        /// <param name="toolTip">The tooltip to show on click.</param>
        /// <param name="overlayTemplate">A style for a PopupButton.</param>
        public OverlayAdorner(UIElement adornedElement, ToolTip toolTip, ControlTemplate overlayTemplate)
            : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement should not be null");
            this.popupButton = new PopupButton
            {
                IsTabStop = false,
                AdornedElement = adornedElement,
            };
            if (overlayTemplate != null)
            {
                this.popupButton.SetCurrentValue(Control.TemplateProperty, overlayTemplate);
            }

            if (toolTip != null)
            {
                this.popupButton.SetCurrentValue(ToolTipProperty, toolTip);
            }

            this.AddVisualChild(this.popupButton);
        }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => this.popupButton != null ? 1 : 0;

        /// <summary>
        /// The clear the single child of a TemplatedAdorner.
        /// </summary>
        public void ClearChild()
        {
            this.RemoveVisualChild(this.popupButton);
            this.popupButton = null;
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
            if (this.popupButton == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "nope: _child == null || index != 0");
            }

            return this.popupButton;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            Debug.Assert(this.popupButton != null, "_child should not be null");
            this.AdornedElement?.InvalidateMeasure();
            this.popupButton.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return this.popupButton.DesiredSize;
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size size)
        {
            var finalSize = base.ArrangeOverride(size);
            this.popupButton?.Arrange(new Rect(default, finalSize));
            return finalSize;
        }
    }
}
