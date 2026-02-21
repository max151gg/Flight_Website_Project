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
using AdminApp.Pages;
using SkyPath_Models.Models;
using SkyPathWSClient;
using AdminApp.Converters;


namespace AdminApp.UserControls
{
    /// <summary>
    /// Interaction logic for FlightPage.xaml
    /// </summary>
    public partial class FlightPage : Page
    {
        List<Flight> flights;
        

        public FlightPage()
        {
            InitializeComponent();
            Loaded += FlightPage_Loaded;
        }
        
        private async Task LoadCityDictionary()
        {
            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            var cities = await cityClient.GetAsync() ?? new List<City>();

            CityIdToNameConverter.CityNamesById =
                cities
                  .Where(c => !string.IsNullOrWhiteSpace(c.CityId))
                  .ToDictionary(c => c.CityId.Trim(), c => c.CityName ?? c.CityId);
        }


        

        private async void FlightPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFlights();
        }

        private async Task LoadFlights()
        {
            // 1) Load city dictionary first
            await LoadCityDictionary();

            // 2) Load flights
            var apiClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetAllFlights"
            };

            flights = await apiClient.GetAsync() ?? new List<Flight>();

            DataContext = flights;
        }

        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Add Flight page
            NavigationService?.Navigate(new AddNewFlight());
        }

        private void UpdateFlight_Click(object sender, RoutedEventArgs e)
        {
            // Get the flight data associated with this button
            // For now, just show a message
            var result = MessageBox.Show("Navigate to edit page for this flight?",
                                        "Update Flight",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Pass flight data to edit page
                // NavigationService?.Navigate(new AddFlightPage(flightData));
                NavigationService?.Navigate(new AddNewFlight());
            }
        }

        private async void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            // Button.Tag is the bound Flight object (because Tag="{Binding}" in XAML)
            if (sender is not Button btn || btn.Tag is not Flight flight)
            {
                MessageBox.Show("Could not determine which flight to delete.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            string flightId = flight.Flight_Id?.ToString() ?? "(unknown)";

            var result = MessageBox.Show(
                $"Are you sure you want to delete flight {flightId}?\n\nThis action cannot be undone.",
                "Delete Flight",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {

                try
                {
                    // TODO: call your WS delete endpoint here (recommended)
                    // var deleteClient = new ApiClient<bool> { ... Path = "api/Admin/DeleteFlight" };
                    // deleteClient.SetQueryParameter("flight_id", flightId);
                    // bool ok = await deleteClient.GetAsync();

                    MessageBox.Show($"Flight {flightId} deleted successfully!",
                                    "Success",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    await LoadFlights();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete flight {flightId}.\n\n{ex.Message}",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();

            // TODO: Implement search functionality
            // Filter flights based on search term
            // You would filter the flights and refresh the FlightsContainer

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Show all flights
                LoadFlights();
            }
            else
            {
                // Filter and display matching flights
                // var filteredFlights = flights.Where(f => 
                //     f.Departure.ToLower().Contains(searchTerm) ||
                //     f.Destination.ToLower().Contains(searchTerm) ||
                //     f.Airline.ToLower().Contains(searchTerm));
            }
        }

        // Helper method to create flight card programmatically
        /*
        private Border CreateFlightCard(Flight flight)
        {
            var border = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(20)
            };
            
            border.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = 2,
                BlurRadius = 10,
                Opacity = 0.08
            };
            
            // Create grid and add flight details
            // ... (implementation details)
            
            return border;
        }
        */
    }
}
