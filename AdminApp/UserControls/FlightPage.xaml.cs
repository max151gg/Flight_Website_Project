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

        // One toggle handler for the Cancel/Reactivate button.
        // If the flight is active  -> cancel it (its tickets also become cancelled).
        // If the flight is cancelled -> reactivate it (so users can book it again).
        private async void ToggleFlightStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Flight flight)
            {
                MessageBox.Show("Could not determine which flight to update.",
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

            // Active flight -> we want to cancel (false). Cancelled flight -> reactivate (true).
            bool makeActive = !flight.IsActive;

            string confirmText = makeActive
                ? "Reactivate this flight? Users will be able to book it again."
                : "Cancel this flight? All tickets for this flight will become cancelled.";

            var result = MessageBox.Show(
                confirmText,
                makeActive ? "Reactivate Flight" : "Cancel Flight",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                bool ok = await SetFlightActiveStatusAsync(flightId, makeActive);
                if (!ok)
                {
                    MessageBox.Show($"Update failed for flight {flightId}.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show(
                    makeActive ? $"Flight {flightId} was reactivated." : $"Flight {flightId} was cancelled.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                await LoadFlights();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update flight {flightId}.\n\n{ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        // Calls the API to set the flight's active status (false = cancel, true = reactivate).
        private async Task<bool> SetFlightActiveStatusAsync(string flightId, bool isActive)
        {
            var apiClient = new ApiClient<bool>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/SetFlightActiveStatus"
            };

            apiClient.SetQueryParameter("flight_id", flightId);
            apiClient.SetQueryParameter("isActive", isActive.ToString());
            return await apiClient.GetAsync();
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
