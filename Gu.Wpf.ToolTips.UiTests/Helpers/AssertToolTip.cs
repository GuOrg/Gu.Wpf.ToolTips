namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using System.Windows.Automation;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    internal static class AssertToolTip
    {
        internal static void IsOpen(bool expected, UiElement element)
        {
            if (expected)
            {
                for (var i = 0; i < 3; i++)
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
                for (var i = 0; i < 3; i++)
                {
                    switch (FindToolTip())
                    {
                        case { IsOffscreen: true }:
                            return;
                        case null:
                            return;
                    }
                }

                Assert.Fail("Expected no ToolTip.");
            }

            ToolTip? FindToolTip()
            {
                var retryTime = TimeSpan.FromMilliseconds(50);
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
