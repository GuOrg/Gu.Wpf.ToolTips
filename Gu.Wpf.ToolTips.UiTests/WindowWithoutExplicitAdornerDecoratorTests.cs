namespace Gu.Wpf.ToolTips.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    [Ignore("tbd")]
    public static class WindowWithoutExplicitAdornerDecoratorTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "WindowWithoutExplicitAdornerDecorator";

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
        public static void ButtonVisible()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button);
            window.FindCheckBox("Buttons visible").IsChecked = false;
            window.FindCheckBox("Buttons visible").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button);
        }

        [Test]
        public static void ButtonEnabled()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var button = window.FindButton("Button with touch tool tip");
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button);

            window.FindCheckBox("Buttons enabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip_hidden.png", button);

            window.FindCheckBox("Buttons enabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_with_default_touch_tool_tip.png", button);
        }
    }
}
