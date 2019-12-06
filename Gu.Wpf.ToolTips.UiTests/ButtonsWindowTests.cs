namespace Gu.Wpf.ToolTips.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public class ButtonsWindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "ButtonsWindow";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ImageAssert.OnFail = OnFail.SaveImageToTemp;
        }

        [SetUp]
        public static void SetUp()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindCheckBox("Adorned element visible").IsChecked = true;
            window.FindCheckBox("IsOverlayVisible (null uses default behavior)").IsChecked = null;
            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
        }

        [Test]
        public void AdornedElementVisibility()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindCheckBox("Adorned element visible").IsChecked = false;
            window.FindCheckBox("Adorned element visible").IsChecked = true;
            ImageAssert.AreEqual("Button_with_default_touch_tool_tip.png", window.FindButton("Button with touch tooltip"));
        }
    }
}
