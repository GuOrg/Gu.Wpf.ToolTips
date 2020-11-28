namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;

    internal static class ToolTipExt
    {
        internal static bool IsOpen(this ToolTip toolTip)
        {
            Wait.For(TimeSpan.FromMilliseconds(50));
            try
            {
                return !toolTip.IsOffscreen;
            }
            catch (System.Windows.Automation.ElementNotAvailableException)
            {
                // Got the following exception when running on CI
                // System.Windows.Automation.ElementNotAvailableException : The target element corresponds to UI that is no longer available (for example, the parent window has closed).
                return false;
            }
        }
    }
}
