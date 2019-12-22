namespace Gu.Wpf.ToolTips.Demo.Wpf
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool?), typeof(Visibility))]
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                true => this.whenTrue,
                false => this.whenFalse,
                null => this.whenFalse,
                _=> throw new ArgumentException("expected bool", nameof(value)),
            };
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(BoolToVisibilityConverter)} can only be used in OneWay bindings");
        }
    }
}
