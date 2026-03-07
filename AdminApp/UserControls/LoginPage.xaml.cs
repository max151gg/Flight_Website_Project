using AdminApp.Converters;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminApp.UserControls
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            // Focus on username field when page loads
            this.Loaded += (s, e) => txtUsername.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformLoginAsync();
        }

        // Handle Enter key press in password field
        private async void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformLoginAsync();
            }
        }

        private async Task PerformLoginAsync()
        {

            string email = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (!email.Contains("@", StringComparison.Ordinal))
            {
                MessageBox.Show("Please enter a valid email",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                var loginClient = new ApiClient<LoginViewModel>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Guest/Login"
                };

                var loginVm = new LoginViewModel
                {
                    Email = email,
                    Password = password
                };

                User user = await loginClient.PostAsyncReturn<LoginViewModel, User>(loginVm);

                if (user == null)
                {
                    MessageBox.Show("Invalid Email or Password.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                    txtPassword.Clear();
                    txtUsername.Focus();
                    return;
                }

                if (user.Role_Id != "0")
                {
                    MessageBox.Show("Only admins can access the admin panel.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                    txtPassword.Clear();
                    txtUsername.Focus();
                    return;
                }

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow == null)
                {
                    MessageBox.Show("Was unable to access the window.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string adminDisplayName = await ResolveAdminFullNameAsync(user);

                mainWindow.ShowSidebar();
                mainWindow.SetAdminName(adminDisplayName);
                mainWindow.SetAdminProfileImage(user.User_Image ?? string.Empty);
                mainWindow.MainFrame.Navigate(new FlightPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                
            }
        }

        private async Task<string> ResolveAdminFullNameAsync(User user)
        {
            if (user == null)
            {
                return "Admin User";
            }

            var userClient = new ApiClient<UserViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetUser"
            };

            UserViewModel vm = await userClient.GetAsync();
            var users = vm?.users ?? new System.Collections.Generic.List<User>();

            UserIdToFullNameConverter.UserNamesById = users
                .Where(u => u != null && !string.IsNullOrWhiteSpace(u.User_Id))
                .ToDictionary(
                    u => u.User_Id,
                    u => string.IsNullOrWhiteSpace(u.User_FullName) ? $"User {u.User_Id}" : u.User_FullName);

            var converter = new UserIdToFullNameConverter();
            string converted = converter.Convert(user.User_Id, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture)?.ToString() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(converted) && !converted.StartsWith("User ", StringComparison.OrdinalIgnoreCase))
            {
                return converted;
            }

            return user.User_FullName ?? user.UserName ?? "Admin User";
        }

        

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Password recovery feature coming soon!\n\nPlease contact system administrator.",
                           "Forgot Password",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);
        }
    }
}
