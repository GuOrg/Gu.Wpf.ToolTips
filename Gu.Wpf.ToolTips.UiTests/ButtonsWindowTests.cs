namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using System.Windows;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;
    using Application = Gu.Wpf.UiAutomation.Application;

    public static class ButtonsWindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "ButtonsWindow";

        [SetUp]
        public static void SetUp()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            Mouse.Position = button.Bounds.Center() + new Vector(0, button.ActualHeight);
            window.FindCheckBox("Buttons enabled").IsChecked = false;
            window.FindCheckBox("Buttons visible").IsChecked = true;
            window.FindCheckBox("IsOverlayVisible (null uses default behavior)").IsChecked = null;
            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName, WindowName);
        }

        [Test]
        public static void ButtonEnabled()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button, TestImage.OnFail);

            window.FindCheckBox("Buttons enabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip_hidden.png", button, TestImage.OnFail);

            window.FindCheckBox("Buttons enabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button, TestImage.OnFail);
        }

        [Test]
        public static void ButtonVisible()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button, TestImage.OnFail);
            window.FindCheckBox("Buttons visible").IsChecked = false;
            window.FindCheckBox("Buttons visible").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button, TestImage.OnFail);
        }

        [Test]
        public static void MouseOver()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            Mouse.Position = button.Bounds.Center();
            var toolTip = button.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Mouse.Position = window.FindButton("Lose focus").Bounds.Center();
            Wait.For(TimeSpan.FromMilliseconds(200));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [Test]
        public static void MouseClick()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            Mouse.Position = button.Bounds.Center();
            var toolTip = button.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Mouse.Click(MouseButton.Left, button.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [Ignore("Not sure if this is a bug in code or test. Need a touch device.")]
        [Test]
        public static void TouchTap()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            Touch.Tap(button.Bounds.Center());
            var toolTip = button.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Touch.Tap(button.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(200));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }
    }
}
