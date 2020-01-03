namespace Gu.Wpf.ToolTips.UiTests
{
    using Gu.Wpf.UiAutomation;

    internal static class ToolTipExt
    {
        internal static bool IsOpen(this ToolTip toolTip)
        {
            try
            {
                return !toolTip.IsOffscreen;
            }
            catch
            {
                // Got the following exception when running on CI
                // System.Windows.Automation.ElementNotAvailableException : The target element corresponds to UI that is no longer available (for example, the parent window has closed).
                return false;
            }
        }
    }
}
