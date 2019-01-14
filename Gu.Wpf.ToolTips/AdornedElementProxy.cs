namespace Gu.Wpf.ToolTips
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    [ContentProperty("Child")]
    public class AdornedElementProxy : FrameworkElement
    {
        private UIElement child;
        private Adorner adorner;
        private bool checkedAdorner;

        /// <summary>
        /// Element for which the AdornedElementProxy is reserving space.
        /// </summary>
        public UIElement AdornedElement => this.Adorner?.AdornedElement;

        /// <inheritdoc/>
        [DefaultValue(null)]
        public virtual UIElement Child
        {
            get => this.child;

            set
            {
                var old = this.child;
                if (!ReferenceEquals(old, value))
                {
                    this.RemoveVisualChild(old);

                    // need to remove old element from logical tree
                    this.RemoveLogicalChild(old);
                    this.child = value;

                    this.AddVisualChild(this.child);
                    this.AddLogicalChild(this.child);

                    this.InvalidateMeasure();
                }
            }
        }

        /// <inheritdoc/>
        protected override int VisualChildrenCount => (this.child == null) ? 0 : 1;

        private Adorner Adorner
        {
            get
            {
                if (this.adorner == null && !this.checkedAdorner)
                {
                    var templateParent = this.TemplatedParent as FrameworkElement;

                    if (templateParent != null)
                    {
                        this.adorner = templateParent.VisualAncestors()
                                                     .OfType<Adorner>()
                                                     .FirstOrDefault();
                    }

                    this.checkedAdorner = true;
                }

                return this.adorner;
            }
        }

        /// <inheritdoc/>
        protected override Visual GetVisualChild(int index)
        {
            if (this.child == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.child;
        }

        /// <inheritdoc/>
        protected override void OnInitialized(EventArgs e)
        {
            if (this.TemplatedParent == null)
            {
                throw new InvalidOperationException("Must be in a template");
            }

            base.OnInitialized(e);
            if (this.AdornedElement != null)
            {
                _ = BindingOperations.SetBinding(this, WidthProperty, this.AdornedElement.CreateOneWayBinding(ActualWidthProperty));
                _ = BindingOperations.SetBinding(this, HeightProperty, this.AdornedElement.CreateOneWayBinding(ActualHeightProperty));
            }
        }

        /// <summary>
        ///     AdornedElementProxy measure behavior is to measure
        ///     only the first visual child.  Note that the return value
        ///     of Measure on this child is ignored as the purpose of this
        ///     class is to match the size of the element for which this
        ///     is a placeholder.
        /// </summary>
        /// <param name="constraint">The measurement constraints.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.TemplatedParent == null)
            {
                throw new InvalidOperationException("Must be in a template");
            }

            if (this.AdornedElement == null)
            {
                return new Size(0, 0);
            }

            var desiredSize = this.AdornedElement.RenderSize;
            this.Child?.Measure(desiredSize);

            return desiredSize;
        }

        /// <summary>
        ///     Default AdornedElementProxy arrangement is to only arrange
        ///     the first visual child. No transforms will be applied.
        /// </summary>
        /// <param name="arrangeBounds">The computed size.</param>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.Child?.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }
    }
}
