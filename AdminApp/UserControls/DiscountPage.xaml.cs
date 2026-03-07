using SkyPath_Models.Models;
using SkyPathWSClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for DiscountPage.xaml
    /// </summary>
    public partial class DiscountPage : UserControl
    {
        private string? currentFilterUserId;
        private List<Discount> allDiscounts = new();

        public DiscountPage()
        {
            InitializeComponent();
            Loaded += DiscountsPage_Loaded;
        }

        private async void DiscountsPage_Loaded(object sender, RoutedEventArgs e)
        {
            dpValidFrom.SelectedDate = DateTime.Now;
            dpValidTo.SelectedDate = DateTime.Now.AddMonths(1);
            await LoadDiscounts();
        }

        private async Task LoadDiscounts()
        {
            var apiClient = new ApiClient<List<Discount>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetAllDiscounts"
            };

            allDiscounts = await apiClient.GetAsync() ?? new List<Discount>();
            ApplyFilterToDiscounts();
        }

        private void ApplyFilterToDiscounts()
        {
            IEnumerable<Discount> filtered = allDiscounts.Where(d => d != null);

            if (!string.IsNullOrWhiteSpace(currentFilterUserId))
            {
                string filter = currentFilterUserId.Trim();
                filtered = filtered.Where(d =>
                    string.Equals(d.User_Id?.Trim(), filter, StringComparison.OrdinalIgnoreCase));
            }

            var cards = filtered
                .Select(d => new DiscountCardItem
                {
                    SourceDiscount = d,
                    Description = string.IsNullOrWhiteSpace(d.Description) ? "Unnamed Discount" : d.Description,
                    PercentageDisplay = $"{d.Percentage}% OFF",
                    DiscountIdDisplay = d.Discount_Id ?? string.Empty,
                    ValidFromDisplay = d.Valid_From ?? string.Empty,
                    ValidToDisplay = d.Valid_To ?? string.Empty,
                    UserDisplay = d.User_Id ?? string.Empty
                })
                .ToList();

            DiscountsList.ItemsSource = cards;
            EmptyState.Visibility = cards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void CreateDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out short percentage))
            {
                return;
            }

            try
            {
                var discount = new Discount
                {
                    Discount_Id = string.Empty,
                    Description = txtDescription.Text.Trim(),
                    Percentage = percentage,
                    Valid_From = dpValidFrom.SelectedDate!.Value.ToString("dd-MM-yyyy"),
                    Valid_To = dpValidTo.SelectedDate!.Value.ToString("dd-MM-yyyy"),
                    User_Id = txtUserId.Text.Trim()
                };

                var apiClient = new ApiClient<Discount>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/CreateDiscount"
                };

                bool created = await apiClient.PostAsync(discount);
                if (!created)
                {
                    MessageBox.Show("Failed to create discount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Discount created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                await LoadDiscounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating discount: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs(out short percentage)
        {
            percentage = 0;

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter a discount description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return false;
            }

            if (!short.TryParse(txtPercentage.Text, out percentage) || percentage <= 0 || percentage > 100)
            {
                MessageBox.Show("Please enter a valid percentage between 1 and 100.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPercentage.Focus();
                return false;
            }

            if (!dpValidFrom.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a valid from date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpValidFrom.Focus();
                return false;
            }

            if (!dpValidTo.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a valid to date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpValidTo.Focus();
                return false;
            }

            if (dpValidTo.SelectedDate < dpValidFrom.SelectedDate)
            {
                MessageBox.Show("Valid To date must be after Valid From date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpValidTo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("User ID is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUserId.Focus();
                return false;
            }

            if (!int.TryParse(txtUserId.Text.Trim(), out _))
            {
                MessageBox.Show("User ID must be numeric.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUserId.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtDescription.Clear();
            txtPercentage.Text = "0";
            txtUserId.Clear();
            dpValidFrom.SelectedDate = DateTime.Now;
            dpValidTo.SelectedDate = DateTime.Now.AddMonths(1);
            txtDescription.Focus();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string filterUserId = txtFilterUserId.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterUserId))
            {
                MessageBox.Show("Please enter a User ID to filter.", "Filter Required", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFilterUserId.Focus();
                return;
            }

            if (!int.TryParse(filterUserId, out _))
            {
                MessageBox.Show("User ID must be a number.", "Invalid User ID", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFilterUserId.Focus();
                return;
            }

            currentFilterUserId = filterUserId;
            txtFilterStatus.Text = $"Showing discounts for User ID: {filterUserId}";
            ApplyFilterToDiscounts();
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            currentFilterUserId = null;
            txtFilterUserId.Clear();
            txtFilterStatus.Text = string.Empty;
            ApplyFilterToDiscounts();
        }

        private async void DeleteDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Discount discount)
            {
                MessageBox.Show("Could not determine which discount to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(discount.Discount_Id))
            {
                MessageBox.Show("Discount ID is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete discount {discount.Discount_Id}?\n\nThis action cannot be undone.",
                "Delete Discount",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                var apiClient = new ApiClient<bool>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/DeleteDiscount"
                };

                apiClient.SetQueryParameter("discount_id", discount.Discount_Id);

                bool deleted = await apiClient.GetAsync();
                if (!deleted)
                {
                    MessageBox.Show("Failed to delete discount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await LoadDiscounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting discount: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private sealed class DiscountCardItem
        {
            public Discount SourceDiscount { get; init; } = new();
            public string Description { get; init; } = string.Empty;
            public string PercentageDisplay { get; init; } = string.Empty;
            public string DiscountIdDisplay { get; init; } = string.Empty;
            public string ValidFromDisplay { get; init; } = string.Empty;
            public string ValidToDisplay { get; init; } = string.Empty;
            public string UserDisplay { get; init; } = string.Empty;
        }
    }
}
