namespace Gu.Wpf.ToolTips.Demo.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter VisibleWhenTrueElseCollapsed = new BoolToVisibilityConverter(Visibility.Visible, Visibility.Collapsed);
        public static readonly BoolToVisibilityConverter VisibleWhenTrueElseHidden = new BoolToVisibilityConverter(Visibility.Visible, Visibility.Hidden);

        private readonly object whenTrue;
        private readonly object whenFalse;

        public BoolToVisibilityConverter(Visibility whenTrue, Visibility whenFalse)
        {
            this.whenTrue = whenTrue;
            this.whenFalse = whenFalse;
        }

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value switch
            {
                bool b => b ? this.whenTrue : this.whenFalse,
                null => this.whenFalse,
                _=> throw new ArgumentException("expected bool", nameof(value)),
            };
        }

        object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException($"{nameof(BoolToVisibilityConverter)} can only be used in OneWay bindings");
        }
    }
}
