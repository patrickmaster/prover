using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Prover.UI.Converters
{
    public class BoolToSolveCancelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool) value;
            return v ? "Anuluj" : "Rozwi¹¿";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}