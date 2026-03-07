using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminApp.Converters
{
    public class FlightIdToNumberConverter : IValueConverter
    {
        public static Dictionary<string, string> FlightNumberById { get; set; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string flightId = value?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(flightId))
            {
                return "N/A";
            }

            return FlightNumberById.TryGetValue(flightId, out string? flightNumber)
                ? flightNumber
                : "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
