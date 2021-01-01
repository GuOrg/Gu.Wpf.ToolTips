namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Exposes the internal PopupControlService via reflection.
    /// </summary>
    public static class PopupControlService
    {
        private static readonly object Service = typeof(FrameworkElement).GetProperty("PopupControlService", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? throw new InvalidOperationException("Did not find property PopupControlService");
#pragma warning disable REFL009, GU0006, INPC013 // The referenced member is not known to exist.
        private static readonly Delegates.RaiseToolTipClosingEvent RaiseToolTipClosingEvent = GetMethod<Delegates.RaiseToolTipClosingEvent>();
        private static readonly Delegates.RaiseToolTipOpeningEvent RaiseToolTipOpeningEvent = GetMethod<Delegates.RaiseToolTipOpeningEvent>();
        private static readonly Delegates.ResetToolTipTimer ResetToolTipTimer = GetMethod<Delegates.ResetToolTipTimer>();

        private static readonly PropertyInfo LastObjectWithToolTipProperty = GetProperty("LastObjectWithToolTip");
        private static readonly PropertyInfo LastMouseOverWithToolTipProperty = GetProperty("LastMouseOverWithToolTip");
        private static readonly PropertyInfo LastCheckedProperty = GetProperty("LastChecked");
#pragma warning restore REFL009, GU0006, INPC013  // The referenced member is not known to exist.

        private static DependencyObject? LastObjectWithToolTip
        {
            get => (DependencyObject?)LastObjectWithToolTipProperty.GetValue(Service);
            set => LastObjectWithToolTipProperty.SetValue(Service, value);
        }

#pragma warning disable IDE0052 // Remove unread private members
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
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// Shows the <see cref="ToolTip"/> for <paramref name="element"/> like if it was hovered with mouse.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/>.</param>
        public static void ShowToolTip(UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (LastObjectWithToolTip is { })
            {
                RaiseToolTipClosingEvent(reset: true);
                LastMouseOverWithToolTip = null;
            }

            LastChecked = element;
            LastObjectWithToolTip = element;
            ResetToolTipTimer();
            RaiseToolTipOpeningEvent(fromKeyboard: false);
        }

        private static T GetMethod<T>()
            where T : Delegate
        {
            return (T?)Service.GetType().GetMethod(typeof(T).Name, BindingFlags.NonPublic | BindingFlags.Instance)
                                       ?.CreateDelegate(typeof(T), Service)
                   ?? throw new InvalidOperationException($"Did not find method {typeof(T).Name}");
        }

        private static PropertyInfo GetProperty(string name) => Service.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance)
                                                                ?? throw new InvalidOperationException($"Did not find method {name}");

        /// <summary>
        /// This is just silly stuff.
        /// </summary>
        private static class Delegates
        {
            internal delegate void RaiseToolTipClosingEvent(bool reset);

            internal delegate void RaiseToolTipOpeningEvent(bool fromKeyboard);

            internal delegate void ResetToolTipTimer();
        }
    }
}
