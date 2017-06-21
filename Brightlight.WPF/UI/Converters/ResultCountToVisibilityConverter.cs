using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Brightlight.WPF.UI.Converters
{
    class ResultCountToVisibilityConverter : IValueConverter
    {
        // https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var n = value as int?;
            if (n == null) return null;

            if (n > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
