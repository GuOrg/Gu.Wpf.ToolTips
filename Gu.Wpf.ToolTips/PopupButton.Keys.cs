namespace Gu.Wpf.ToolTips
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public partial class PopupButton
    {
        private static readonly Dictionary<string, ComponentResourceKey> Cache = new Dictionary<string, ComponentResourceKey>();

        public static ResourceKey TextOverlayTemplateKey => Get();

        public static ResourceKey DefaultOverlayTemplateKey => Get();

        public static ResourceKey MissingToolTipKey => Get();

        private static ComponentResourceKey Get([CallerMemberName] string caller = null)
        {
            ComponentResourceKey key;
            if (!Cache.TryGetValue(caller, out key))
            {
                key = new ComponentResourceKey(typeof(PopupButton), caller);
                Cache.Add(caller, key);
            }
            return key;
        }
    }
}
