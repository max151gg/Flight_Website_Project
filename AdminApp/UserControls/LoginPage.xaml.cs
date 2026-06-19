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
    public partial class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();

            // Focus on username field when page loads
            this.Loaded += (s, e) => txtUsername.Focus();
        }

        // Login button click -> run the login.
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
            // Clear all inline errors at the start
            txtEmailError.Visibility = Visibility.Collapsed;
            txtPasswordError.Visibility = Visibility.Collapsed;
            txtGeneralError.Visibility = Visibility.Collapsed;

            string email = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            bool isValid = true;

            // Validate Email
            if (string.IsNullOrEmpty(email))
            {
                txtEmailError.Text = "Email is required.";
                txtEmailError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!email.Contains("@", StringComparison.Ordinal) || !email.Contains("."))
            {
                txtEmailError.Text = "Enter a valid email address.";
                txtEmailError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Password
            if (string.IsNullOrEmpty(password))
            {
                txtPasswordError.Text = "Password is required.";
                txtPasswordError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
            {
                txtUsername.Focus();
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

                // Send email + password to the API and get the user back (null if wrong).
                var loginVm = new LoginViewModel { Email = email, Password = password };
                User user = await loginClient.PostAsyncReturn<LoginViewModel, User>(loginVm);

                if (user == null)
                {
                    txtGeneralError.Text = "Invalid email or password.";
                    txtGeneralError.Visibility = Visibility.Visible;
                    txtPassword.Clear();
                    return;
                }

                if (user.User_Ban)
                {
                    txtGeneralError.Text = "This account has been banned.";
                    txtGeneralError.Visibility = Visibility.Visible;
                    txtPassword.Clear();
                    return;
                }

                // Role_Id "0" means admin. Block everyone else from the admin panel.
                if (user.Role_Id != "0")
                {
                    txtGeneralError.Text = "Only admins can access the admin panel.";
                    txtGeneralError.Visibility = Visibility.Visible;
                    txtPassword.Clear();
                    return;
                }

                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow == null)
                {
                    txtGeneralError.Text = "Could not open the main window. Please restart the app.";
                    txtGeneralError.Visibility = Visibility.Visible;
                    return;
                }

                string adminDisplayName = await ResolveAdminFullNameAsync(user);
                mainWindow.ShowSidebar();
                mainWindow.SetAdminName(adminDisplayName);
                mainWindow.SetAdminProfileImage(user.User_Image ?? string.Empty);
                mainWindow.MainFrame.Navigate(new DashboardPage());
            }
            catch (Exception ex)
            {
                txtGeneralError.Text = $"Login failed: {ex.Message}";
                txtGeneralError.Visibility = Visibility.Visible;
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
