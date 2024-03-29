﻿namespace Gu.Wpf.ToolTips.Demo.Windows
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            this.InitializeComponent();
            //// InputManager.Current.PreProcessInput += (o, e) => Dump("PreProcessInput", e);
            //// InputManager.Current.PostProcessInput += (o, e) => Dump("PostProcessInput", e);

#pragma warning disable CS8321 // Local function is declared but never used
            static void Dump(string name, NotifyInputEventArgs args)
#pragma warning restore CS8321 // Local function is declared but never used
            {
                if (args.StagingItem.Input is { } inputEventArgs &&
                    inputEventArgs.RoutedEvent.Name != "PreviewInputReport" &&
                    inputEventArgs.Device is not KeyboardDevice)
                {
                    Debug.WriteLine($"{name,-16} {inputEventArgs.GetType().Name,-28} {inputEventArgs.Device?.GetType().Name ?? "null",-21} {inputEventArgs.Device?.Target?.GetType().Name ?? "null",-15} {inputEventArgs.RoutedEvent.Name}");
                }
            }
        }

        private void Button1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button1_ToolTipOpening));
        }

        private void Button1_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button1_ToolTipClosing));
        }

        private void Button2_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button2_ToolTipOpening));
        }

        private void Button2_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button2_ToolTipClosing));
        }

        private void TextBlock1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.TextBlock1_ToolTipOpening));
        }

        private void TextBlock1_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.TextBlock1_ToolTipClosing));
        }

        private void Label1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Label1_ToolTipOpening));
        }

        private void Label1_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Label1_ToolTipClosing));
        }

        private void Button3_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button3_ToolTipOpening));
        }

        private void Button3_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button3_ToolTipClosing));
        }

        private void Button4_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button4_ToolTipOpening));
        }

        private void Button4_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            Debug.WriteLine(nameof(this.Button4_ToolTipClosing));
        }
    }
}
