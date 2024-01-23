using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace MarineParamCalculatorDataBindings
{
    [ValueConversion(typeof(double), typeof(String))]
    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? strValue = value as string;
            if(strValue != null)
            {
                return double.Parse(strValue);
            }
            return 0;
        }
    }
}
