namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Collections;
    using System.Diagnostics;
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
                        child.Template = (ControlTemplate)e.NewValue;
                    }
                }));

        private readonly TouchOnlyControl child;

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
            var control = new TouchOnlyControl
            {
                Template = this.Template,
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

        private class TouchOnlyControl : Control
        {
            private static InputType currentInput = InputType.None;

#pragma warning disable CA1810 // Initialize reference type static fields inline
            static TouchOnlyControl()
#pragma warning restore CA1810 // Initialize reference type static fields inline
            {
                IsTabStopProperty.OverrideMetadata(typeof(TouchOnlyControl), new FrameworkPropertyMetadata(false));
                FocusableProperty.OverrideMetadata(typeof(TouchOnlyControl), new FrameworkPropertyMetadata(false));

                InputManager.Current.PreNotifyInput += (sender, args) =>
                {
                    if (args.StagingItem.Input is { RoutedEvent: { Name: "TouchEnter" }, Source: TouchOnlyControl _ })
                    {
                        currentInput = InputType.Touch;
                    }
                    else if (currentInput == InputType.None)
                    {
                        currentInput = InputType.Other;
                    }

                    //Dump("PreNotifyInput", args);
                };

                InputManager.Current.PostNotifyInput += (sender, args) =>
                {
                    if (args.StagingItem.Input is { RoutedEvent: { Name: "TouchLeave" }, Source: TouchOnlyControl _ })
                    {
                        currentInput = InputType.None;
                    }
                    else if (currentInput != InputType.Touch)
                    {
                        currentInput = InputType.None;
                    }

                    //Dump("PostNotifyInput", args);
                };

                //static void Dump(string name, NotifyInputEventArgs args)
                //{
                //    if (args.StagingItem.Input is { } inputEventArgs)
                //    {
                //        Debug.WriteLine($"{name,-15} currentInput: {currentInput,-5} RoutedEvent: {inputEventArgs.RoutedEvent.Name}");
                //        // Debug.WriteLine($"{name} RoutedEvent: {inputEventArgs.RoutedEvent.Name} Device: {inputEventArgs.Device?.GetType().Name ?? "null"} Source: {inputEventArgs.Source} currentInput: {currentInput}");
                //    }
                //}
            }

            private enum InputType
            {
                None,
                Touch,
                Other,
            }

            protected override int VisualChildrenCount => currentInput == InputType.Other ? 0 : base.VisualChildrenCount;
        }
    }
}
