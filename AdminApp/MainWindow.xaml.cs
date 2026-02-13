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

namespace AdminApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StartPage startPage;
        LoginPage loginPage;
        FlightPage flightPage;

        // later you can add:
        // FlightsPage flightsPage;
        // TicketsPage ticketsPage;

        public MainWindow()
        {
            InitializeComponent();
            ViewStartPage();
        }

        private void ViewStartPage()
        {
            if (this.startPage == null)
                this.startPage = new StartPage();

            this.MainFrame.Content = this.startPage;
        }

        private void ViewLoginPage()
        {
            if (this.loginPage == null)
                this.loginPage = new LoginPage();

            this.MainFrame.Content = this.loginPage;
        }
        private void ViewFlightPage()
        {
            if (this.flightPage == null)
                this.flightPage = new FlightPage();

            this.MainFrame.Content = this.flightPage;
        }

        // Hyperlink handlers
        private void Nav_Start_Click(object sender, RoutedEventArgs e) => ViewStartPage();
        private void Nav_Login_Click(object sender, RoutedEventArgs e) => ViewLoginPage();

        // placeholders until you create these UserControls
        private void Nav_Flights_Click(object sender, RoutedEventArgs e) => ViewFlightPage();

        private void Nav_Tickets_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("dsdsdsd");
        }

        private void Nav_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}