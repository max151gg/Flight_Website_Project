using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminApp
{
    public class UrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var raw = value?.ToString();

            if (!string.IsNullOrWhiteSpace(raw))
                return new Uri("http://localhost:5125" + raw, UriKind.Absolute);
            return new Uri("http://localhost:5125//images/profiles/default.png", UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Uri uri ? uri.ToString() : null;
    }
}
