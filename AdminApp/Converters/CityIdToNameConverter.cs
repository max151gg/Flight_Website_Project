using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AdminApp.Converters
{
    // WPF converter: turns a city id into the city name when the admin app shows flights.
    // Used in XAML bindings, e.g. {Binding Departure_Id, Converter={StaticResource CityIdToName}}.
    public class CityIdToNameConverter : IValueConverter
    {
        // Lookup table. Key = city id, Value = city name.
        // Filled once after the cities are loaded, so the screen can show a name instantly
        // instead of searching the whole city list every single time.
        public static Dictionary<string, string> CityNamesById { get; set; }
            = new Dictionary<string, string>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value is the city id (Departure_Id or Arrival_Id).
            if (value == null) return "";

            string id = value.ToString() ?? "";

            // Look up the name in the dictionary; show it if found.
            if (CityNamesById.TryGetValue(id, out var name))
                return name;

            // fallback: show the id if no name was found
            return id;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}