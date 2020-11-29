namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using System.IO;
    using System.Windows.Automation;

    using Gu.Wpf.UiAutomation;

    using NUnit.Framework;

    internal static class AssertToolTip
    {
        internal static void IsOpen(bool expected, UiElement element)
        {
            var startTime = DateTime.Now;
            if (expected)
            {
                while (!Retry.IsTimeouted(startTime, TimeSpan.FromSeconds(1)))
                {
                    if (FindToolTip() is { IsOffscreen: false })
                    {
                        return;
                    }
                }

                Assert.Fail("Expected visible ToolTip.");
            }
            else
            {
                while (!Retry.IsTimeouted(startTime, TimeSpan.FromSeconds(1)))
                {
                    switch (FindToolTip())
                    {
                        case { IsOffscreen: true }:
                            return;
                        case null:
                            return;
                    }
                }

                var fullFileName = Path.Combine(Path.GetTempPath(), TestContext.CurrentContext.Test.MethodName + ".png");
                Capture.ScreenToFile(fullFileName);
                TestContext.AddTestAttachment(fullFileName);
                Assert.Fail("Expected no ToolTip.");
            }

            ToolTip? FindToolTip()
            {
                var retryTime = TimeSpan.FromMilliseconds(10);
                if (Desktop.Instance.TryFindFirst(TreeScope.Children, Conditions.Popup, x => new Popup(x), retryTime, out var popup) ||
                    element.Window.TryFindFirst(TreeScope.Children, Conditions.Popup, x => new Popup(x), retryTime, out popup))
                {
                    if (popup.TryFindFirst(TreeScope.Children, element.CreateCondition(Conditions.ToolTip, element.HelpText), x => new ToolTip(x), retryTime, out var toolTip))
                    {
                        return toolTip;
                    }
                }

                return null;
            }
        }
    }
}
