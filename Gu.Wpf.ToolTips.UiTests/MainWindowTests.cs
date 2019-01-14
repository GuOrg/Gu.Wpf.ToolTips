namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class MainWindowTests
    {
        [Test]
        public void ClickAllTabs()
        {
            // Just a smoke test so we don't crash.
            using (var app = Application.Launch("Gu.Wpf.ToolTips.Demo.exe"))
            {
                var window = app.GetMainWindow(TimeSpan.FromSeconds(30));
                var tab = window.FindTabControl();
                foreach (var tabItem in tab.Items)
                {
                    _ = tabItem.Select();
                }
            }
        }
    }
}
