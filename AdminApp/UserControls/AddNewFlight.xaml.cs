using Microsoft.Win32;
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
using SkyPathWSClient;
using SkyPath_Models.Models;

namespace AdminApp.Pages
{


    /// <summary>
    /// Interaction logic for NewFlight.xaml
    /// </summary>
    public partial class AddNewFlight : UserControl
    {
        private bool isEditMode = false;
        private string editingFlightId = string.Empty;
        private string selectedImagePath = null;
        private List<City> _cities = new();

        // Constructor for Add mode
        public AddNewFlight()
        {
            InitializeComponent();
            InitializePage();
            Loaded += AddNewFlight_Loaded;
        }

        public AddNewFlight(Flight flightToEdit)
        {
            InitializeComponent();
            isEditMode = true;
            editingFlightId = flightToEdit?.Flight_Id?.Trim() ?? string.Empty;
            InitializePage();
            Loaded += async (sender, e) =>
            {
                await LoadCities();
                LoadFlightData(flightToEdit);
            };
        }
        private async void AddNewFlight_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCities();
        }
        private async Task LoadCities()
        {
            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            _cities = await cityClient.GetAsync() ?? new List<City>();

            //sort A-Z by name
            _cities = _cities
                .Where(c => !string.IsNullOrWhiteSpace(c.CityId) && !string.IsNullOrWhiteSpace(c.CityName))
                .OrderBy(c => c.CityName)
                .ToList();

            cmbDeparture.ItemsSource = _cities;
            cmbArrival.ItemsSource = _cities;


        }

        private void InitializePage()
        {
            if (isEditMode)
            {
                txtPageTitle.Text = "Edit Flight";
                btnSubmit.Content = "Accept Changes";
            }
            else
            {
                txtPageTitle.Text = "Add New Flight";
                btnSubmit.Content = "Create Flight";
            }
        }

        private void LoadFlightData(Flight flight)
        {
            if (flight == null)
            {
                return;
            }

            cmbDeparture.SelectedValue = flight.Departure_Id?.Trim();
            cmbArrival.SelectedValue = flight.Arrival_Id?.Trim();
            txtAirline.Text = flight.Airline ?? string.Empty;
            txtFlightNumber.Text = flight.Flight_Number ?? string.Empty;

            if (DateTime.TryParseExact(
                    flight.Departure_Date,
                    new[] { "dd-MM-yyyy", "d-M-yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime departureDate))
            {
                dpDepartureDate.SelectedDate = departureDate;
            }

            if (DateTime.TryParseExact(
                    flight.Arrival_Date,
                    new[] { "dd-MM-yyyy", "d-M-yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime arrivalDate))
            {
                dpArrivalDate.SelectedDate = arrivalDate;
            }

            txtDepartureTime.Text = flight.Departure_Time ?? string.Empty;
            txtArrivalTime.Text = flight.Arrival_Time ?? string.Empty;
            txtPrice.Text = flight.Price.ToString("0.##");
            txtSeats.Text = flight.Seats_Available.ToString();
        }

        private async void SubmitFlight_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                var flight = new Flight
                {
                    Flight_Id = isEditMode ? editingFlightId : "",
                    Departure_Id = cmbDeparture.SelectedValue?.ToString(),
                    Arrival_Id = cmbArrival.SelectedValue?.ToString(),

                    Airline = txtAirline.Text.Trim(),
                    Flight_Number = txtFlightNumber.Text.Trim(),

                    Departure_Date = dpDepartureDate.SelectedDate!.Value.ToString("dd-MM-yyyy"),
                    Arrival_Date = dpArrivalDate.SelectedDate!.Value.ToString("dd-MM-yyyy"),

                    Departure_Time = txtDepartureTime.Text.Trim(),
                    Arrival_Time = txtArrivalTime.Text.Trim(),

                    Price = double.Parse(txtPrice.Text.Trim()),
                    Seats_Available = int.Parse(txtSeats.Text.Trim()),
                };

                bool ok;

                if (isEditMode)
                {
                    ok = await UpdateFlightInDatabaseAsync(flight);
                }
                else
                {
                    ok = await AddFlightToDatabaseAsync(flight);
                }

                if (!ok)
                {
                    MessageBox.Show(isEditMode ? "Failed to update flight." : "Failed to create flight.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show(isEditMode ? "Flight updated successfully!" : "Flight created successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving flight:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task<bool> AddFlightToDatabaseAsync(Flight flight)
        {
            var client = new ApiClient<Flight>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/CreateFlight"
            };

            return await client.PostAsyncReturn<Flight, bool>(flight);
        }

        private async Task<bool> UpdateFlightInDatabaseAsync(Flight flight)
        {
            var client = new ApiClient<Flight>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/UpdateFlight"
            };

            return await client.PostAsyncReturn<Flight, bool>(flight);

        }
        private bool ValidateInputs()
        {
            // Departure City
            if (cmbDeparture.SelectedIndex == -1)
            {
                ShowValidationError("Please select a departure city.");
                cmbDeparture.Focus();
                return false;
            }

            // Destination City
            if (cmbArrival.SelectedIndex == -1)
            {
                ShowValidationError("Please select a destination city.");
                cmbArrival.Focus();
                return false;
            }

            // Check if departure and destination are different
            var depId = cmbDeparture.SelectedValue?.ToString();
            var arrId = cmbArrival.SelectedValue?.ToString();

            if (!string.IsNullOrWhiteSpace(depId) && depId == arrId)
            {
                ShowValidationError("Departure and destination cities must be different.");
                cmbArrival.Focus();
                return false;
            }

            // Airline
            if (string.IsNullOrWhiteSpace(txtAirline.Text))
            {
                ShowValidationError("Please enter an airline name.");
                txtFlightNumber.Focus();
                return false;
            }

            // Flight Number
            if (string.IsNullOrWhiteSpace(txtFlightNumber.Text))
            {
                ShowValidationError("Please enter a flight number.");
                txtFlightNumber.Focus();
                return false;
            }

            // Departure Date
            if (!dpDepartureDate.SelectedDate.HasValue)
            {
                ShowValidationError("Please select a departure date.");
                dpDepartureDate.Focus();
                return false;
            }

            // Arrival Date
            if (!dpArrivalDate.SelectedDate.HasValue)
            {
                ShowValidationError("Please select an arrival date.");
                dpArrivalDate.Focus();
                return false;
            }

            // Check if arrival is after departure
            if (dpArrivalDate.SelectedDate < dpDepartureDate.SelectedDate)
            {
                ShowValidationError("Arrival date must be on or after departure date.");
                dpArrivalDate.Focus();
                return false;
            }

            // Departure Time
            if (!TimeSpan.TryParse(txtDepartureTime.Text, out _))
            {
                ShowValidationError("Please enter a valid departure time (HH:mm format).");
                txtDepartureTime.Focus();
                return false;
            }

            // Arrival Time
            if (!TimeSpan.TryParse(txtArrivalTime.Text, out _))
            {
                ShowValidationError("Please enter a valid arrival time (HH:mm format).");
                txtArrivalTime.Focus();
                return false;
            }

            // Price
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                ShowValidationError("Please enter a valid price greater than 0.");
                txtPrice.Focus();
                return false;
            }

            // Seats
            if (!int.TryParse(txtSeats.Text, out int seats) || seats <= 0)
            {
                ShowValidationError("Please enter a valid number of seats greater than 0.");
                txtSeats.Focus();
                return false;
            }

            return true;
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message,
                          "Validation Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Warning);
        }

        //private void ChooseImage_Click(object sender, RoutedEventArgs e)
        //{
        //    var openFileDialog = new OpenFileDialog

        //    {
        //        Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
        //        Title = "Select Flight Image"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        selectedImagePath = openFileDialog.FileName;
        //        txtImageName.Text = System.IO.Path.GetFileName(selectedImagePath);
        //    }
        //}
        //private void ChooseImage_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog()
        //    openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All Files (*.*)|*.*";
        //    bool? result = openFileDialog.ShowDialog();
        //    if (result == true)
        //    {
        //        string fileName = openFileDialog.FileName;
        //        selectedImagePath = fileName;
        //        Uri uri = new Uri(fileName);
        //        BitmapImage bitmap = new BitmapImage(uri);
        //    }
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel?\n\nAny unsaved changes will be lost.",
                                        "Confirm Cancel",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.GoBack();
            }
        }

        // Add this property to your AddNewFlight class to provide navigation support
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
