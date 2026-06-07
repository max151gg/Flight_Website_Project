using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using AdminApp.Pages;
using AdminApp.UserControls;

namespace AdminApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Button currentActiveButton = null;

        private string adminImagePath = string.Empty;
        public string AdminImagePath
        {
            get => adminImagePath;
            set
            {
                if (adminImagePath != value)
                {
                    adminImagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Load login page initially
            MainFrame.Navigate(new LoginPage());

            //HideSidebar();
            SetAdminName("Admin User");
            SetAdminProfileImage(string.Empty);
        }

        // Navigate to different pages
        private void NavigateToDashboard(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new DashboardPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Dashboard page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToFlights(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new FlightPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Flights page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToUsers(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new UsersHomePage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Users page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToTickets(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new TicketPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Tickets page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToDiscounts(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new DiscountPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Discounts page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToAnnouncements(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            try
            {
                MainFrame.Navigate(new AnnouncementPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Announcements page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        

        private void NavigateToSettings(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            // MainFrame.Navigate(new SettingsPage());
            MessageBox.Show("Settings page - To be implemented");
        }

        private void NavigateToReports(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            // MainFrame.Navigate(new ReportsPage());
            MessageBox.Show("Reports page - To be implemented");
        }

        // Set active button style
        private void SetActiveButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            if (currentActiveButton != null)
            {
                currentActiveButton.Style = (Style)FindResource("SidebarButtonStyle");
            }

            button.Style = (Style)FindResource("ActiveSidebarButtonStyle");
            currentActiveButton = button;
        }

        // Logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                                        "Confirm Logout",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                HideSidebar();
                SetAdminName("Admin User");
                SetAdminProfileImage(string.Empty);
                MainFrame.Navigate(new LoginPage());

                // Reset active button
                if (currentActiveButton != null)
                {
                    currentActiveButton.Style = (Style)FindResource("SidebarButtonStyle");
                    currentActiveButton = null;
                }
            }
        }

        // Public method to show sidebar after successful login
        public void ShowSidebar()
        {
            // Enable all sidebar buttons
            btnDashboard.IsEnabled = true;
            btnFlights.IsEnabled = true;
            btnUsers.IsEnabled = true;
            btnTickets.IsEnabled = true;
            btnDiscounts.IsEnabled = true;
            btnAnnouncements.IsEnabled = true;
            btnSettings.IsEnabled = true;
            btnReports.IsEnabled = true;


            // Default active button
            SetActiveButton(btnDashboard);

        }

        // Hide sidebar before login
        public void HideSidebar()
        {
            // Disable all sidebar buttons
            btnDashboard.IsEnabled = false;
            btnFlights.IsEnabled = false;
            btnUsers.IsEnabled = false;
            btnTickets.IsEnabled = false;
            btnDiscounts.IsEnabled = false;
            btnAnnouncements.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnReports.IsEnabled = false;
        }

        // Update admin name in sidebar
        public void SetAdminName(string name)
        {
            txtAdminName.Text = string.IsNullOrWhiteSpace(name) ? "Admin User" : name;
        }

        public void SetAdminProfileImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                AdminImagePath = string.Empty;
                return;
            }

            string separator = imagePath.Contains("?") ? "&" : "?";
            AdminImagePath = $"{imagePath}{separator}v={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}