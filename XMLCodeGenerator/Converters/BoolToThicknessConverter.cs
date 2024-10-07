using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace XMLCodeGenerator.Converters
{
    class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return new Thickness
                {
                    Left = 3,
                    Right = 3,
                    Top = 3,
                    Bottom = 3,
                };
            return new Thickness
            {
                Left = 1,
                Right = 1,
                Top = 1,
                Bottom = 1,
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness && ((Thickness)value).Left == 1)
                return false;
            return true;
        }
    }
}
