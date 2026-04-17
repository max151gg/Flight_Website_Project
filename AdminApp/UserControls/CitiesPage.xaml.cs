using SkyPath_Models.Models;
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
    /// Interaction logic for CitiesPage.xaml
    /// </summary>
    public partial class CitiesPage : UserControl
    {
        private List<City> cities = new();

        public CitiesPage()
        {
            InitializeComponent();
            Loaded += CitiesPage_Loaded;
        }

        private async void CitiesPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCities();
        }

        private async Task LoadCities()
        {
            var apiClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            cities = await apiClient.GetAsync() ?? new List<City>();
            DataContext = cities;
        }

        private async void AddCity_Click(object sender, RoutedEventArgs e)
        {
            string cityName = txtNewCity.Text.Trim();

            if (string.IsNullOrWhiteSpace(cityName))
            {
                MessageBox.Show("Please enter a city name.",
                                "Validation Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                txtNewCity.Focus();
                return;
            }

            if (cities.Any(c => string.Equals(c.CityName?.Trim(), cityName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("This city already exists.",
                                "Duplicate City",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                txtNewCity.Focus();
                txtNewCity.SelectAll();
                return;
            }

            try
            {
                var createClient = new ApiClient<City>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/CreateCity"
                };

                bool created = await createClient.PostAsyncReturn<City, bool>(new City { CityName = cityName });

                if (!created)
                {
                    MessageBox.Show("Failed to add new city.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                txtNewCity.Clear();
                await LoadCities();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add city.\n\n{ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                CitiesList.ItemsSource = cities;
                return;
            }

            CitiesList.ItemsSource = cities
                .Where(c => !string.IsNullOrWhiteSpace(c.CityName) &&
                            c.CityName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
