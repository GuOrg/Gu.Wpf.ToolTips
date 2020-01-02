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
            // Just a test so we don't crash.
            using var app = Application.Launch("Gu.Wpf.ToolTips.Demo.exe");
            app.WaitForMainWindow(TimeSpan.FromSeconds(5));
            var window = app.MainWindow;
            foreach (var tabItem in window.FindTabControl().Items)
            {
                _ = tabItem.Select();
            }
        }
    }
}
