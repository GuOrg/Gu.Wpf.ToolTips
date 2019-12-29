namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Collections;
    using System.ComponentModel;
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
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.NotDataBindable,
                (o, e) =>
                {
                    if (o is OverlayAdorner { child: { } child })
                    {
                        child.Template = (ControlTemplate)e.NewValue;
                    }
                }));

        private Control? child;

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
            var control = new Control
            {
                IsTabStop = false,
                Focusable = false,
                Template = this.Template,
            };
            this.Child = control;
        }

        /// <summary>
        /// Gets or sets the visual that renders the content.
        /// marked virtual because AddVisualChild calls the virtual OnVisualChildrenChanged.
        /// </summary>
        [DefaultValue(null)]
        public Control? Child
        {
            get => this.child;

            set
            {
                var old = this.child;
                if (!ReferenceEquals(old, value))
                {
                    this.RemoveVisualChild(old);
                    this.RemoveLogicalChild(old);
                    this.child = value;

                    this.AddVisualChild(this.child);
                    this.AddLogicalChild(value);
                    this.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets the content is the data used to generate the child elements of this control.
        /// </summary>
        public ControlTemplate Template
        {
            get => (ControlTemplate)this.GetValue(TemplateProperty);
            set => this.SetValue(TemplateProperty, value);
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => this.child == null ? 0 : 1;

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren => this.child == null
            ? EmptyEnumerator.Instance
            : new SingleChildEnumerator(this.child);

        /// <summary>
        /// Set child to null and remove it as visual and logical child.
        /// </summary>
        public void ClearChild()
        {
            this.RemoveVisualChild(this.child);
            this.RemoveLogicalChild(this.child);
            this.child = null;
        }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            if (this.child == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.child;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var desiredSize = this.AdornedElement.RenderSize;
            this.Child?.Measure(desiredSize);
            return desiredSize;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Child?.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return this.Child?.RenderSize ?? base.ArrangeOverride(finalSize);
        }
    }
}
