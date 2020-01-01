namespace Gu.Wpf.ToolTips
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    /// A decorator that is not hit test visible for mouse.
    /// </summary>
    public class TouchOnlyDecorator : Decorator
    {
        private static InputType currentInput = InputType.None;

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static TouchOnlyDecorator()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            Handle(Mouse.PreviewMouseMoveEvent);
            Handle(Mouse.PreviewMouseDownEvent);
            Handle(Mouse.PreviewMouseDownOutsideCapturedElementEvent);
            Handle(Mouse.PreviewMouseUpEvent);
            Handle(Mouse.PreviewMouseWheelEvent);

            InputManager.Current.PreProcessInput += (sender, args) =>
            {
                if (args.StagingItem.Input is { RoutedEvent: { Name: "PreviewStylusInRange" } })
                {
                    currentInput = InputType.Touch;
                }
                else if (currentInput == InputType.None)
                {
                    currentInput = InputType.Other;
                }

                Dump("PreProcessInput", args);
            };
            InputManager.Current.PostProcessInput += (sender, args) =>
            {
                if (args.StagingItem.Input is { RoutedEvent: { Name: "StylusOutOfRange" } } ||
                    currentInput != InputType.Touch)
                {
                    currentInput = InputType.None;
                }

                Dump("PostProcessInput", args);
            };

            static void Handle(RoutedEvent previewMouseEvent)
            {
                EventManager.RegisterClassHandler(typeof(TouchOnlyDecorator), previewMouseEvent, new RoutedEventHandler((o, e) => e.Handled = true));
            }

            static void Dump(string name, NotifyInputEventArgs args)
            {
                if (args.StagingItem.Input is { } inputEventArgs)
                {
                    //Debug.WriteLine($"{name,-16} {inputEventArgs.GetType().Name,-21} {inputEventArgs.Device?.GetType().Name ?? "null",-21} {inputEventArgs.Device?.Target?.ToString() ?? "null",-21} {inputEventArgs.RoutedEvent.Name}");
                    // Debug.WriteLine($"{name} RoutedEvent: {inputEventArgs.RoutedEvent.Name} Device: {inputEventArgs.Device?.GetType().Name ?? "null"} Source: {inputEventArgs.Source} currentInput: {currentInput}");
                }
            }
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
