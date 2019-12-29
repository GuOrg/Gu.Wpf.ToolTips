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
            window.FindCheckBox("IsElementEnabled").IsChecked = false;
            window.FindCheckBox("IsElementVisible").IsChecked = true;
            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = true;
            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = true;
            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName, WindowName);
        }

        [Test]
        public static void TouchToolTipServiceIsEnabled()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);

            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled.png", button, TestImage.OnFail);

            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);
        }

        [Test]
        public static void ToolTipServiceIsEnabled()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);

            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled.png", button, TestImage.OnFail);

            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);
        }

        [Test]
        public static void ButtonVisible()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);
            window.FindCheckBox("IsElementVisible").IsChecked = false;
            window.FindCheckBox("IsElementVisible").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_disabled_with_overlay.png", button, TestImage.OnFail);
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

        [Ignore("Not sure if we want toggle on click.")]
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
