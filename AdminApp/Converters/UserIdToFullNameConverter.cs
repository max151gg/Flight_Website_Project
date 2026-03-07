using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminApp.Converters
{
    public class UserIdToFullNameConverter : IValueConverter
    {
        public static Dictionary<string, string> UserNamesById { get; set; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string userId = value?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "Unknown User";
            }

            return UserNamesById.TryGetValue(userId, out string? fullName)
                ? fullName
                : $"User {userId}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
