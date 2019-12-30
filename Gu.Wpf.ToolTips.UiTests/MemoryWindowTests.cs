namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class MemoryWindowTests
    {
        private const string ExeFileName = "Gu.Wpf.ToolTips.Demo.exe";
        private const string WindowName = "MemoryWindow";

        [Ignore("Not sure we can test this.")]
        [Test]
        public static void ShowHide()
        {
            using var app = Application.Launch(ExeFileName, WindowName);
            var window = app.MainWindow;
            var gc = window.FindButton("GC");
            var totalMemory = window.FindTextBox("TotalMemory");
            var checkBox = window.FindCheckBox("TouchToolTipServiceIsEnabled");

            gc.Invoke();
            var before = totalMemory.Text;
            Console.WriteLine($"Before: {totalMemory.Text}");

            checkBox.IsChecked = true;
            gc.Invoke();
            Console.WriteLine($"With overlays: {totalMemory.Text}");
            Assert.AreNotEqual(before, totalMemory.Text);

            checkBox.IsChecked = false;
            gc.Invoke();
            Console.WriteLine($"After: {totalMemory.Text}");
            Assert.AreEqual(before, totalMemory.Text);
        }
    }
}
