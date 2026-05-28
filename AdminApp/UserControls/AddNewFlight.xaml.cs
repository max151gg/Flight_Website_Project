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
            bool isValid = true;

            // Clear all error messages first
            ClearAllErrors();

            // Departure City
            if (cmbDeparture.SelectedIndex == -1)
            {
                ShowError(errDeparture, "Please select a departure city.");
                isValid = false;
            }

            // Arrival City
            if (cmbArrival.SelectedIndex == -1)
            {
                ShowError(errArrival, "Please select an arrival city.");
                isValid = false;
            }
            else if (cmbDeparture.SelectedIndex != -1)
            {
                var depId = cmbDeparture.SelectedValue?.ToString();
                var arrId = cmbArrival.SelectedValue?.ToString();
                if (!string.IsNullOrWhiteSpace(depId) && depId == arrId)
                {
                    ShowError(errArrival, "Departure and arrival cities cannot be the same.");
                    isValid = false;
                }
            }

            // Airline
            if (string.IsNullOrWhiteSpace(txtAirline.Text))
            {
                ShowError(errAirline, "Airline name is required.");
                isValid = false;
            }

            // Flight Number
            if (string.IsNullOrWhiteSpace(txtFlightNumber.Text))
            {
                ShowError(errFlightNumber, "Flight number is required.");
                isValid = false;
            }

            // Departure Date
            if (!dpDepartureDate.SelectedDate.HasValue)
            {
                ShowError(errDepartureDate, "Please select a departure date.");
                isValid = false;
            }

            // Arrival Date
            if (!dpArrivalDate.SelectedDate.HasValue)
            {
                ShowError(errArrivalDate, "Please select an arrival date.");
                isValid = false;
            }
            else if (dpDepartureDate.SelectedDate.HasValue &&
                     dpArrivalDate.SelectedDate < dpDepartureDate.SelectedDate)
            {
                ShowError(errArrivalDate, "Arrival date must be on or after departure date.");
                isValid = false;
            }

            // Departure Time
            if (!TimeSpan.TryParse(txtDepartureTime.Text, out _))
            {
                ShowError(errDepartureTime, "Enter a valid departure time (HH:mm).");
                isValid = false;
            }

            // Arrival Time
            if (!TimeSpan.TryParse(txtArrivalTime.Text, out _))
            {
                ShowError(errArrivalTime, "Enter a valid arrival time (HH:mm).");
                isValid = false;
            }

            // Price
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                ShowError(errPrice, "Price must be greater than 0.");
                isValid = false;
            }

            // Seats
            if (!int.TryParse(txtSeats.Text, out int seats) || seats < 0)
            {
                ShowError(errSeats, "Seats must be 0 or greater.");
                isValid = false;
            }

            return isValid;
        }

        private void ClearAllErrors()
        {
            errDeparture.Visibility = Visibility.Collapsed;
            errArrival.Visibility = Visibility.Collapsed;
            errAirline.Visibility = Visibility.Collapsed;
            errFlightNumber.Visibility = Visibility.Collapsed;
            errDepartureDate.Visibility = Visibility.Collapsed;
            errDepartureTime.Visibility = Visibility.Collapsed;
            errArrivalDate.Visibility = Visibility.Collapsed;
            errArrivalTime.Visibility = Visibility.Collapsed;
            errPrice.Visibility = Visibility.Collapsed;
            errSeats.Visibility = Visibility.Collapsed;
        }

        private void ShowError(System.Windows.Controls.TextBlock block, string message)
        {
            block.Text = message;
            block.Visibility = Visibility.Visible;
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
