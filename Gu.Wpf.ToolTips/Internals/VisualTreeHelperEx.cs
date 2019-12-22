namespace Gu.Wpf.ToolTips
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    internal static class VisualTreeHelperEx
    {
        internal static IEnumerable<DependencyObject> VisualAncestors(this DependencyObject dependencyObject)
        {
            while ((dependencyObject = VisualTreeHelper.GetParent(dependencyObject)) != null)
            {
                yield return dependencyObject;
            }
        }
    }
}
