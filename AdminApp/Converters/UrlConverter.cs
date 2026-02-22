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
            // value should be something like: "default.png" or "someUser.png" or "/images/profiles/x.png"
            var raw = value?.ToString();

            // fallback
            if (string.IsNullOrWhiteSpace(raw))
                raw = "default.png";

            // If your DB already stores full URL -> just use it
            if (raw.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return new Uri(raw, UriKind.Absolute);

            // If your DB stores "/images/profiles/x.png" -> attach host
            if (raw.StartsWith("/"))
                return new Uri("http://localhost:5125" + raw, UriKind.Absolute);

            // If your DB stores "images/profiles/x.png" -> add leading slash
            if (raw.StartsWith("images/", StringComparison.OrdinalIgnoreCase))
                return new Uri("http://localhost:5125/" + raw, UriKind.Absolute);

            // If your DB stores only filename -> assume it's under /images/profiles/
            return new Uri($"http://localhost:5125/images/profiles/{Uri.EscapeDataString(raw)}", UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Uri uri ? uri.ToString() : null;
    }
}
