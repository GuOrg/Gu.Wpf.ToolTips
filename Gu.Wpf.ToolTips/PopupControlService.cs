namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Exposes the internal PopupControlService via reflection.
    /// </summary>
    public static class PopupControlService
    {
        private static readonly object Service = typeof(FrameworkElement).GetProperty("PopupControlService", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? throw new InvalidOperationException("Did not find property PopupControlService");
#pragma warning disable REFL009, GU0006, INPC013 // The referenced member is not known to exist.
        private static readonly MethodInfo InspectElementForToolTipMethod = Service.GetType().GetMethod("InspectElementForToolTip", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find method InspectElementForToolTip");
        private static readonly MethodInfo RaiseToolTipClosingEventMethod = Service.GetType().GetMethod("RaiseToolTipClosingEvent", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find method InspectElementForToolTip");
        private static readonly MethodInfo RaiseToolTipOpeningEventMethod = Service.GetType().GetMethod("RaiseToolTipOpeningEvent", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find method InspectElementForToolTip");
        private static readonly PropertyInfo LastObjectWithToolTipProperty = Service.GetType().GetProperty("LastObjectWithToolTip", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find field _quickShow");
        private static readonly PropertyInfo LastMouseOverWithToolTipProperty = Service.GetType().GetProperty("LastMouseOverWithToolTip", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find field _quickShow");
        private static readonly PropertyInfo LastCheckedProperty = Service.GetType().GetProperty("LastChecked", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find field _quickShow");
        private static readonly PropertyInfo ToolTipTimerProperty = Service.GetType().GetProperty("ToolTipTimer", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find field _quickShow");
#pragma warning restore REFL009, GU0006, INPC013  // The referenced member is not known to exist.

        internal static DispatcherTimer? ToolTipTimer => (DispatcherTimer?)ToolTipTimerProperty.GetValue(Service);

        private static DependencyObject? LastObjectWithToolTip
        {
            get => (DependencyObject?)LastObjectWithToolTipProperty.GetValue(Service);
            set => LastObjectWithToolTipProperty.SetValue(Service, value);
        }

        private static DependencyObject? LastMouseOverWithToolTip
        {
            get => (DependencyObject?)LastMouseOverWithToolTipProperty.GetValue(Service);
            set => LastMouseOverWithToolTipProperty.SetValue(Service, value);
        }

        private static DependencyObject? LastChecked
        {
            get => (DependencyObject?)LastCheckedProperty.GetValue(Service);
            set => LastCheckedProperty.SetValue(Service, value);
        }

        /// <summary>
        /// Shows the <see cref="ToolTip"/> for <paramref name="o"/> like if it was hovered with mouse.
        /// </summary>
        /// <param name="o">The <see cref="DependencyObject"/>.</param>
        public static void ShowToolTip(DependencyObject o)
        {
            if (o is null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            if (LastObjectWithToolTip is { })
            {
                RaiseToolTipClosingEvent(reset: true);
                LastMouseOverWithToolTip = null;
            }

            LastChecked = o;
            LastObjectWithToolTip = o;
            RaiseToolTipOpeningEvent(fromKeyboard: false);
        }

        /// <summary>
        /// Hides the <see cref="ToolTip"/> for <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The <see cref="DependencyObject"/>.</param>
        public static void HideToolTip(DependencyObject element) => InspectElementForToolTip(element, 0);

        private static void InspectElementForToolTip(DependencyObject element, int triggerAction) => InspectElementForToolTipMethod.Invoke(Service, new object[] { element, triggerAction });

        private static void RaiseToolTipClosingEvent(bool reset) => RaiseToolTipClosingEventMethod.Invoke(Service, new object[] { reset });

        private static void RaiseToolTipOpeningEvent(bool fromKeyboard) => RaiseToolTipOpeningEventMethod.Invoke(Service, new object[] { fromKeyboard });
    }
}
