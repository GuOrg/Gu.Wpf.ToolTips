namespace Gu.Wpf.ToolTips.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class ButtonsWindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "ButtonsWindow";

        [SetUp]
        public static void SetUp()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindCheckBox("Buttons enabled").IsChecked = false;
            window.FindCheckBox("Buttons visible").IsChecked = true;
            window.FindCheckBox("IsOverlayVisible (null uses default behavior)").IsChecked = null;
            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
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
            Assert.NotNull(button.FindToolTip());

            Mouse.Position = window.FindButton("Lose focus").Bounds.Center();
            Assert.Null(button.FindToolTip());
        }
    }
}
