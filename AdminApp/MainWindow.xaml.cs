using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdminApp.UserControls;
using AdminApp.Pages;

namespace AdminApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button currentActiveButton = null;

        public MainWindow()
        {
            InitializeComponent();

            // Load login page initially
            MainFrame.Navigate(new LoginPage());

            // Hide sidebar initially (until logged in)
            //HideSidebar();
        }

        // Navigate to different pages
        private void NavigateToDashboard(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            // MainFrame.Navigate(new DashboardPage());
            MessageBox.Show("Dashboard page - To be implemented");
        }

        private void NavigateToFlights(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new FlightPage());
        }

        private void NavigateToUsers(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new UsersHomePage());
        }

        private void NavigateToTickets(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            // MainFrame.Navigate(new TicketsPage());
            MessageBox.Show("Tickets page - To be implemented");
        }

        private void NavigateToDiscounts(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            // MainFrame.Navigate(new DiscountsPage());
            MessageBox.Show("Discounts page - To be implemented");
        }

        private void NavigateToAnnouncements(object sender, RoutedEventArgs e)
        {
            SetActiveButton(sender as Button);
            MainFrame.Navigate(new AnnouncementPage());
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
            txtAdminName.Text = name;
        }
    }
}