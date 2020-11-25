namespace Gu.Wpf.ToolTips
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Threading;

    internal static class AdornerService
    {
        private static readonly DependencyProperty AdornerLayerProperty = DependencyProperty.RegisterAttached(
            "AdornerLayer",
            typeof(AdornerLayer),
            typeof(AdornerService),
            new PropertyMetadata(default(AdornerLayer)));

        /// <summary>
        /// Adds <paramref name="adorner"/> to the <see cref="AdornerLayer"/>
        /// If no adorner layer is present a retry is performed with  DispatcherPriority.Loaded.
        /// </summary>
        /// <param name="adorner">The <see cref="Adorner"/>.</param>
        internal static void Show(Adorner adorner)
        {
            if (adorner is null)
            {
                throw new System.ArgumentNullException(nameof(adorner));
            }

            Show(adorner, retry: true);
        }

        /// <summary>
        /// Removes <paramref name="adorner"/> from the <see cref="AdornerLayer"/>.
        /// </summary>
        /// <param name="adorner">The <see cref="Adorner"/>.</param>
        internal static void Remove(Adorner adorner)
        {
            if (adorner is null)
            {
                throw new System.ArgumentNullException(nameof(adorner));
            }

            var adornerLayer = (AdornerLayer?)adorner.GetValue(AdornerLayerProperty) ??
                               AdornerLayer.GetAdornerLayer(adorner.AdornedElement);
            adornerLayer?.Remove(adorner);
            adorner.ClearValue(AdornerLayerProperty);
        }

        private static void Show(Adorner adorner, bool retry)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(adorner.AdornedElement);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(adorner);
                adornerLayer.Add(adorner);
                adorner.SetCurrentValue(AdornerLayerProperty, adornerLayer);
            }
            else if (retry)
            {
                // try again later, perhaps giving layout a chance to create the adorner layer
                _ = adorner.Dispatcher?.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new DispatcherOperationCallback(ShowAdornerOperation),
                    new object[] { adorner });
            }
        }

        private static object? ShowAdornerOperation(object arg)
        {
            var args = (object[])arg;
            var adorner = (Adorner)args[0];
            Show(adorner, retry: false);
            return null;
        }
    }
}
