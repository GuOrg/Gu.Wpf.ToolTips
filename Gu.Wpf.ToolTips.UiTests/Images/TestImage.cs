﻿namespace Gu.Wpf.ToolTips.UiTests
{
    using System;
    using System.Drawing;
    using System.IO;
    using Gu.Wpf.UiAutomation;
    using NUnit.Framework;

    public static class TestImage
    {
        internal static readonly string Current = GetCurrent();

        [Explicit]
        [Script]
        public static void Rename()
        {
            var folder = @"C:\Git\_GuOrg\Gu.Wpf.ToolTips\Gu.Wpf.ToolTips.UiTests";
            var oldName = "Red_border_default_visibility_width_100.png";
            var newName = "Red_border_default_visibility_width_100.png";

            foreach (var file in Directory.EnumerateFiles(folder, oldName, SearchOption.AllDirectories))
            {
                File.Move(file, file.Replace(oldName, newName, StringComparison.Ordinal));
            }

            foreach (var file in Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories))
            {
                File.WriteAllText(file, File.ReadAllText(file).Replace(oldName, newName, StringComparison.Ordinal));
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter
        internal static void OnFail(Bitmap? expected, Bitmap actual, string resource)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var fullFileName = Path.Combine(Path.GetTempPath(), resource);
            //// ReSharper disable once AssignNullToNotNullAttribute
            _ = Directory.CreateDirectory(Path.GetDirectoryName(fullFileName)!);
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }

            actual.Save(fullFileName);
            TestContext.AddTestAttachment(fullFileName);
        }

        private static string GetCurrent()
        {
            if (WindowsVersion.IsWindows7())
            {
                return "Win7";
            }

            if (WindowsVersion.IsWindows10())
            {
                return "Win10";
            }

            if (WindowsVersion.CurrentContains("Windows Server 2019") ||
                WindowsVersion.CurrentContains("Windows Server 2022"))
            {
                return "WinServer2019";
            }

            return WindowsVersion.CurrentVersionProductName;
        }
    }
}
