namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    internal class IsAnyTrueConverter : IMultiValueConverter
    {
        internal static readonly IsAnyTrueConverter Default = new IsAnyTrueConverter();

        private IsAnyTrueConverter()
        {
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return false;
            }

            var result = values.Any(x => Equals(x, BoolBoxes.True))
                                ? BoolBoxes.True
                                : BoolBoxes.False;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
