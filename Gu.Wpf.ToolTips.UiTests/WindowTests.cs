﻿namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    [TestFixture("DefaultAdornerLayerWindow")]
    [TestFixture("ExplicitAdornerDecoratorWindow")]
    public class WindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private readonly string windowName;

        public WindowTests(string windowName)
        {
            this.windowName = windowName;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Touch must be initialized before the app to test touch on is started.
            // Not sure why but my guess is the call to InitializeTouchInjection adds a touch device making WPF start listening for touch input.
            Touch.Initialize();
        }

        [SetUp]
        public void SetUp()
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            Mouse.Position = window.FindButton("Lose focus").Bounds.Center();
            window.FindCheckBox("IsElementEnabled").IsChecked = false;
            window.FindCheckBox("IsElementVisible").IsChecked = true;
            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = true;
            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = true;
            window.WaitUntilResponsive();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName, this.windowName);
        }

        [TestCase("Button 1", "Button_1_disabled_with_overlay.png", "Button_1_disabled.png")]
        [TestCase("Button 2", "Button_2_disabled_with_overlay.png", "Button_2_disabled.png")]
        [TestCase("TextBlock 1", "TextBlock_1_disabled_with_overlay.png", "TextBlock_1_disabled.png")]
        [TestCase("Label 1", "Label_1_disabled_with_overlay.png", "Label_1_disabled.png")]
        public void TouchToolTipServiceIsEnabled(string name, string withOverlay, string noOverlay)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);

            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{noOverlay}", element, TestImage.OnFail);

            window.FindCheckBox("TouchToolTipServiceIsEnabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);
        }

        [TestCase("Button 1", "Button_1_disabled_with_overlay.png", "Button_1_disabled.png")]
        [TestCase("Button 2", "Button_2_disabled_with_overlay.png", "Button_2_disabled.png")]
        [TestCase("TextBlock 1", "TextBlock_1_disabled_with_overlay.png", "TextBlock_1_disabled.png")]
        [TestCase("Label 1", "Label_1_disabled_with_overlay.png", "Label_1_disabled.png")]
        public void ToolTipServiceIsEnabled(string name, string withOverlay, string noOverlay)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);

            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = false;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{noOverlay}", element, TestImage.OnFail);

            window.FindCheckBox("ToolTipServiceIsEnabled").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);
        }

        [TestCase("Button 1", "Button_1_disabled_with_overlay.png")]
        [TestCase("Button 2", "Button_2_disabled_with_overlay.png")]
        [TestCase("TextBlock 1", "TextBlock_1_disabled_with_overlay.png")]
        [TestCase("Label 1", "Label_1_disabled_with_overlay.png")]
        public void IsElementVisible(string name, string withOverlay)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);
            window.FindCheckBox("IsElementVisible").IsChecked = false;
            window.FindCheckBox("IsElementVisible").IsChecked = true;
            ImageAssert.AreEqual($"Images\\{TestImage.Current}\\{withOverlay}", element, TestImage.OnFail);
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void MouseOver(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            Mouse.Position = element.Bounds.Center();
            var toolTip = element.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            window.FindButton("Lose focus").Click(moveMouse: true);
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void TapThenMouseClick(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            Touch.Tap(element.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            var toolTip = element.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Mouse.Click(MouseButton.Left, element.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void TapAdornerTwice(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            Touch.Tap(element.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            var toolTip = element.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Touch.Tap(element.Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void TapAdornerTwiceManyTimes(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            for (var i = 0; i < 4; i++)
            {
                Touch.Tap(element.Bounds.Center());
                Wait.For(TimeSpan.FromMilliseconds(100));
                var toolTip = element.FindToolTip();
                Assert.AreEqual(false, toolTip.IsOffscreen);

                Touch.Tap(element.Bounds.Center());
                Wait.For(TimeSpan.FromMilliseconds(100));
                Assert.AreEqual(true, toolTip.IsOffscreen);
            }
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void TapAdornerThenTapOther(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));
            Touch.Tap(element.Bounds.Center());
            var toolTip = element.FindToolTip();
            Assert.AreEqual(false, toolTip.IsOffscreen);

            Touch.Tap(window.FindButton("Lose focus").Bounds.Center());
            Wait.For(TimeSpan.FromMilliseconds(100));
            Assert.AreEqual(true, toolTip.IsOffscreen);
        }

        [TestCase("Button 1")]
        [TestCase("Button 2")]
        [TestCase("TextBlock 1")]
        [TestCase("Label 1")]
        public void TapAdornerThenTapOtherManyTimes(string name)
        {
            using var app = Application.AttachOrLaunch(ExeFileName, this.windowName);
            var window = app.MainWindow;
            var element = window.FindFirstChild(Conditions.ByName(name), x => new UiElement(x));

            for (var i = 0; i < 4; i++)
            {
                Touch.Tap(element.Bounds.Center());
                var toolTip = element.FindToolTip();
                Assert.AreEqual(false, toolTip.IsOffscreen);

                Touch.Tap(window.FindButton("Lose focus").Bounds.Center());
                Wait.For(TimeSpan.FromMilliseconds(100));
                Assert.AreEqual(true, toolTip.IsOffscreen);
            }
        }
    }
}
