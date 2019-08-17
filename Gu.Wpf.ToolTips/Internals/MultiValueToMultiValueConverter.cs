namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    internal class MultiValueToMultiValueConverter : IMultiValueConverter
    {
        internal static readonly MultiValueToMultiValueConverter Default = new MultiValueToMultiValueConverter();

        private MultiValueToMultiValueConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
            {
                return values;
            }

            return values.ToArray();
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(MultiValueToMultiValueConverter)} can only be used in OneWay bindings");
        }
    }
}
