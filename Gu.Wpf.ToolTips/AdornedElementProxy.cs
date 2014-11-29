namespace Gu.Wpf.ToolTips
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    [ContentProperty("Child")]
    public class AdornedElementProxy : FrameworkElement
    {
        private UIElement _child;
        private Adorner _adorner;
        private bool _checkedAdorner;

        ///<summary>
        /// Element for which the AdornedElementProxy is reserving space.
        ///</summary>
        public UIElement AdornedElement
        {
            get
            {
                var adorner = Adorner;
                return adorner == null ? null : adorner.AdornedElement;
            }
        }

        [DefaultValue(null)]
        public virtual UIElement Child
        {
            get { return _child; }
            set
            {
                UIElement old = _child;

                if (!ReferenceEquals(old, value))
                {
                    RemoveVisualChild(old);
                    //need to remove old element from logical tree
                    RemoveLogicalChild(old);
                    _child = value;

                    AddVisualChild(_child);
                    AddLogicalChild(_child);

                    InvalidateMeasure();
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get { return (_child == null) ? 0 : 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_child == null || index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _child;
        }

        protected override void OnInitialized(EventArgs e)
        {
            if (TemplatedParent == null)
            {
                throw new InvalidOperationException("Must be in a template");
            }

            base.OnInitialized(e);
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
            if (TemplatedParent == null)
            {
                throw new InvalidOperationException("Must be in a template");
            }

            if (AdornedElement == null)
            {
                return new Size(0, 0);
            }

            Size desiredSize = AdornedElement.RenderSize;
            UIElement child = Child;

            if (child != null)
            {
                child.Measure(desiredSize);
            }

            return desiredSize;
        }

        /// <summary>
        ///     Default AdornedElementProxy arrangement is to only arrange
        ///     the first visual child. No transforms will be applied.
        /// </summary>
        /// <param name="arrangeBounds">The computed size.</param>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            UIElement child = Child;

            if (child != null)
            {
                child.Arrange(new Rect(arrangeBounds));
            }

            return arrangeBounds;
        }

        private Adorner Adorner
        {
            get
            {
                if (_adorner == null && !_checkedAdorner)
                {
                    var templateParent = TemplatedParent as FrameworkElement;

                    if (templateParent != null)
                    {
                        _adorner = (Adorner)templateParent.VisualAncestors().FirstOrDefault(x => (x is Adorner));
                    }

                    _checkedAdorner = true;
                }
                return _adorner;
            }
        }
    }
}
