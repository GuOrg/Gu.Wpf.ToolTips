namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    internal static class PopupControlService
    {
        private static readonly object Service = typeof(FrameworkElement).GetProperty("PopupControlService", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? throw new InvalidOperationException("Did not find property PopupControlService");
#pragma warning disable REFL009, GU0006 // The referenced member is not known to exist.
        private static readonly MethodInfo InspectElementForToolTip = Service.GetType().GetMethod("InspectElementForToolTip", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find method InspectElementForToolTip");
        private static readonly FieldInfo QuickShow = Service.GetType().GetField("_quickShow", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Did not find field _quickShow");
#pragma warning restore REFL009, GU0006  // The referenced member is not known to exist.

        internal static void Show(DependencyObject o)
        {
            _ = o.Dispatcher?.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() =>
                {
                    QuickShow.SetValue(Service, true);
                    _ = InspectElementForToolTip.Invoke(Service, new object[] { o, 0 });
                }));
        }
    }
}
