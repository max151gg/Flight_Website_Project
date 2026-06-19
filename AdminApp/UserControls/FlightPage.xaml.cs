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
    public partial class FlightPage : UserControl
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

        // Loads all flights from the API and shows them in the list (DataContext binds to the UI).
        private async Task LoadFlights()
        {
            // 1) Load city dictionary first (so ids can be shown as city names)
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

        // Opens the Add New Flight page (empty form).
        private void AddFlight_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddNewFlight());
        }

        // Opens the same form in edit mode for the flight on the clicked row (button Tag).
        private void UpdateFlight_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Flight flight)
            {
                MessageBox.Show("Could not determine which flight to edit.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            NavigationService?.Navigate(new AddNewFlight(flight));
        }

        // Asks for confirmation, then deletes the flight and reloads the list.
        private async void DeleteFlight_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Flight flight)
            {
                MessageBox.Show("Could not determine which flight to delete.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            string flightId = flight.Flight_Id?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(flightId))
            {
                MessageBox.Show("Flight ID is missing.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete flight {flightId}?\n\nThis action cannot be undone.",
                "Delete Flight",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = await DeleteFlightFromDatabaseAsync(flightId);
                if (!deleted)
                {
                    MessageBox.Show($"Delete failed for flight {flightId}.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

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
        private async Task<bool> DeleteFlightFromDatabaseAsync(string flightId)
        {
            var apiClient = new ApiClient<bool>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/DeleteFlight"
            };

            apiClient.SetQueryParameter("flight_id", flightId);
            return await apiClient.GetAsync();
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

        public System.Windows.Navigation.NavigationService NavigationService
        {
            get
            {
                // Try to get NavigationService from the parent window or frame
                return System.Windows.Navigation.NavigationService.GetNavigationService(this);
            }
        }
    }
}
