namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
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
#pragma warning disable WPF0041 // Set mutable dependency properties using SetCurrentValue.
                        ((Control)child.Child).Template = (ControlTemplate)e.NewValue;
#pragma warning restore WPF0041 // Set mutable dependency properties using SetCurrentValue.
                    }
                }));

        private readonly TouchOnlyDecorator child;

        static OverlayAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayAdorner), new FrameworkPropertyMetadata(typeof(OverlayAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAdorner"/> class.
        /// Be sure to call the base class constructor.
        /// </summary>
        /// <param name="adornedElement">The ui element to adorn.</param>
        public OverlayAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            var control = new TouchOnlyDecorator()
            {
                Child = new Control
                {
                    Template = this.Template,
                    IsTabStop = false,
                },
            };
            this.child = control;
            this.AddVisualChild(control);
            this.AddLogicalChild(control);
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
        protected override int VisualChildrenCount => 1;

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren => new SingleChildEnumerator(this.child);

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
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return base.ArrangeOverride(finalSize);
        }

        private class TouchOnlyDecorator : Decorator
        {
            private static InputDevice? currentInputDevice;

            static TouchOnlyDecorator()
            {
                InputManager.Current.PreNotifyInput += (sender, args) =>
                {
                    currentInputDevice = args.StagingItem.Input.Device;
                };
            }

            /// <inheritdoc />
            protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
            {
                return currentInputDevice is TouchDevice ? base.HitTestCore(hitTestParameters) : null;
            }

            /// <inheritdoc />
            protected override void OnRender(DrawingContext dc)
            {
#pragma warning disable CA1062 // Validate arguments of public methods
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
#pragma warning restore CA1062 // Validate arguments of public methods
            }
        }
    }
}
