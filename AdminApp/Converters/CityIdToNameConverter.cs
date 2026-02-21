using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AdminApp.Converters
{
    public class CityIdToNameConverter : IValueConverter
    {
        // We will fill this dictionary from code-behind after loading cities
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
//using System;
//using System.Globalization;
//using System.Windows.Data;

//namespace AdminApp
//{
//    public class CityIdToNameConverter : IValueConverter
//    {
//        // Replace this with your actual city lookup logic
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value == null)
//                return string.Empty;

//            // Example: value is city ID (int or string)
//            // You should replace this with your actual lookup (e.g., from a dictionary or database)
//            int cityId;
//            if (int.TryParse(value.ToString(), out cityId))
//            {
//                // Dummy example mapping
//                switch (cityId)
//                {
//                    case 1: return "New York";
//                    case 2: return "London";
//                    case 3: return "Tokyo";
//                    default: return $"City #{cityId}";
//                }
//            }
//            return value.ToString();
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}