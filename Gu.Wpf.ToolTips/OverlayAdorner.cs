namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// An <see cref="Adorner"/> similar to the one used for validation errors.
    /// </summary>
    public sealed class OverlayAdorner : Adorner
    {
        /// <summary>Identifies the <see cref="Template"/> dependency property.</summary>
        public static readonly DependencyProperty TemplateProperty = Control.TemplateProperty.AddOwner(
            typeof(OverlayAdorner),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                (o, e) =>
                {
                    if (o is OverlayAdorner { child: Control control })
                    {
                        control.Template = (ControlTemplate)e.NewValue;
                    }
                }));

        private readonly Control child;

        static OverlayAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayAdorner), new FrameworkPropertyMetadata(typeof(OverlayAdorner)));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(OverlayAdorner), new UIPropertyMetadata(false));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAdorner"/> class.
        /// Be sure to call the base class constructor.
        /// </summary>
        /// <param name="adornedElement">The ui element to adorn.</param>
        public OverlayAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            var child = new Control
            {
                Template = this.Template,
                IsTabStop = false,
                Focusable = false,
            };
            this.child = child;
            this.AddVisualChild(child);
            this.AddLogicalChild(child);
        }

        /// <summary>
        /// Gets or sets the content is the data used to generate the child elements of this control.
        /// </summary>
        public ControlTemplate? Template
        {
            get => (ControlTemplate?)this.GetValue(TemplateProperty);
            set => this.SetValue(TemplateProperty, value);
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => 1;

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren => new SingleChildEnumerator(this.child);

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            if (this.child is null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.child;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }
    }
}
