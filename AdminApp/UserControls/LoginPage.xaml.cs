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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        // Handle Enter key press in password field
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void PerformLogin()
        {
            // Hide previous error message
            txtErrorMessage.Visibility = Visibility.Collapsed;

            // Get input values
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            // Basic validation
            if (string.IsNullOrEmpty(username))
            {
                ShowError("Please enter your username or email.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your password.");
                txtPassword.Focus();
                return;
            }

            // TODO: Replace with actual authentication logic
            // For now, use hardcoded credentials for testing
            if (username.ToLower() == "admin" && password == "admin123")
            {
                // Successful login
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    // Show sidebar
                    mainWindow.ShowSidebar();

                    // Set admin name
                    mainWindow.SetAdminName(username);

                    // Navigate to dashboard
                    // mainWindow.MainFrame.Navigate(new DashboardPage());

                    // For now, show a success message
                    MessageBox.Show($"Welcome, {username}!\n\nDashboard will load here.",
                                   "Login Successful",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                }
            }
            else
            {
                // Failed login
                ShowError("Invalid username or password. Please try again.");
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            txtErrorMessage.Visibility = Visibility.Visible;
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
