namespace Gu.Wpf.ToolTips.UiTests
{
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class SizeWindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "SizeWindow";

        [SetUp]
        public static void SetUp()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            window.FindTextBox("Width").Text = "60";
            window.FindTextBox("Height").Text = "25";
            window.FindTextBox("Margin").Text = "2";
            window.FindTextBox("Padding").Text = "2";

            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName, WindowName);
        }

        [TestCase("Button1")]
        [TestCase("Button2")]
        [TestCase("Button3")]
        [TestCase("Button4")]
        public static void Resize(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var element = window.FindButton(name);
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_width_60_height_25_margin_2_padding_2.png", element, TestImage.OnFail);

            window.FindTextBox("Width").Text = "62";
            window.FindTextBox("Height").Text = "27";
            window.FindTextBox("Margin").Text = "4";
            window.FindTextBox("Padding").Text = "6";
            window.WaitUntilResponsive();
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_width_62_height_27_margin_4_padding_6.png", element, TestImage.OnFail);

            window.FindTextBox("Width").Text = "60";
            window.FindTextBox("Height").Text = "25";
            window.FindTextBox("Margin").Text = "2";
            window.FindTextBox("Padding").Text = "2";
            window.WaitUntilResponsive();
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\Button_width_60_height_25_margin_2_padding_2.png", element, TestImage.OnFail);
        }
    }
}
