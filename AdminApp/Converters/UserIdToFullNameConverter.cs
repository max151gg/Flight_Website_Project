using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdminApp.Converters
{
    // WPF converter: shows a user's full name instead of their id in the admin app
    // (for example on the tickets/users screens).
    public class UserIdToFullNameConverter : IValueConverter
    {
        // Lookup table. Key = user id, Value = user's full name.
        // Lets the screen display the name quickly without re-reading the users list each time.
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
