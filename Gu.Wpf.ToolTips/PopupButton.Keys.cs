namespace Gu.Wpf.ToolTips
{
    using System.Runtime.CompilerServices;
    using System.Windows;

    public partial class PopupButton
    {
        public static ResourceKey TextOverlayTemplateKey { get; } = CreateKey();

        public static ResourceKey DefaultOverlayTemplateKey { get; } = CreateKey();

        public static ResourceKey MissingToolTipOverlayKey { get; } = CreateKey();

        private static ComponentResourceKey CreateKey([CallerMemberName] string caller = null)
        {
            return new ComponentResourceKey(typeof(PopupButton), caller);
        }
    }
}
