namespace Gu.Wpf.ToolTips
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;

    internal static class VisualTreeHelperEx
    {
        private static readonly PropertyInfo InheritanceContextProp = typeof(DependencyObject).GetProperty(
            "InheritanceContext",
            BindingFlags.NonPublic | BindingFlags.Instance);

        internal static IEnumerable<DependencyObject> LogicalAncestors(this DependencyObject dependencyObject)
        {
            while ((dependencyObject = LogicalTreeHelper.GetParent(dependencyObject)) != null)
            {
                yield return dependencyObject;
            }
        }

        internal static IEnumerable<DependencyObject> VisualAncestors(this DependencyObject dependencyObject)
        {
            while ((dependencyObject = VisualTreeHelper.GetParent(dependencyObject)) != null)
            {
                yield return dependencyObject;
            }
        }

        /// <summary>
        /// Uses reflection and internal InheritanceContext, potentially fragile
        /// http://stackoverflow.com/a/20988314/1069200
        /// </summary>
        internal static IEnumerable<DependencyObject> AllAncestors(this DependencyObject child)
        {
            while (child != null)
            {
                var parent = LogicalTreeHelper.GetParent(child);
                if (parent == null)
                {
                    if (child is FrameworkElement)
                    {
                        parent = VisualTreeHelper.GetParent(child);
                    }

                    if (parent == null && child is ContentElement)
                    {
                        parent = ContentOperations.GetParent((ContentElement)child);
                    }

                    if (parent == null)
                    {
                        parent = InheritanceContextProp.GetValue(child, null) as DependencyObject;
                    }
                }

                if (parent == null)
                {
                    yield break;
                }

                child = parent;
                yield return parent;
            }
        }

        internal static INameScope NameScope(this DependencyObject element)
        {
            while (element != null)
            {
                var nameScope = System.Windows.NameScope.GetNameScope(element);

                if (nameScope != null)
                {
                    return nameScope;
                }

                element = LogicalTreeHelper.GetParent(element) ?? VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
