using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AdminApp.Converters
{
    public class CityIdToNameConverter : IValueConverter
    {
        public static Dictionary<string, string> CityNamesById { get; set; }
            = new Dictionary<string, string>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value should be the city id (Departure_Id or Arrival_Id)
            if (value == null) return "";

            string id = value.ToString() ?? "";

            if (CityNamesById.TryGetValue(id, out var name))
                return name;

            // fallback: show the id if not found
            return id;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}