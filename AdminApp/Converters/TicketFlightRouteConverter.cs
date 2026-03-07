using SkyPath_Models.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminApp.Converters
{
    public class TicketFlightRouteConverter : IValueConverter
    {
        public static Dictionary<string, Flight> FlightsById { get; set; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string flightId = value?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(flightId))
            {
                return "N/A";
            }

            if (!FlightsById.TryGetValue(flightId, out Flight? flight) || flight == null)
            {
                return "N/A";
            }

            string departure = ResolveCityName(flight.Departure_Id);
            string arrival = ResolveCityName(flight.Arrival_Id);

            if (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival))
            {
                return "N/A";
            }

            return $"{departure} → {arrival}";
        }

        private static string ResolveCityName(string? cityId)
        {
            string key = cityId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            if (CityIdToNameConverter.CityNamesById.TryGetValue(key, out string? cityName)
                && !string.IsNullOrWhiteSpace(cityName))
            {
                return cityName;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
