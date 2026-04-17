using AdminApp.Converters;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdminApp.UserControls
{
    public partial class TicketPage : UserControl
    {
        private int currentFilter; // 0 = All, 1 = Active, 2 = Cancelled
        private string searchTerm = string.Empty;

        private List<Ticket> allTickets = new();

        public TicketPage()
        {
            InitializeComponent();
            Loaded += TicketPage_Loaded;
        }

        private async void TicketPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadTickets();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tickets page: {ex.Message}", "Tickets Page Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadTickets()
        {
            var ticketClient = new ApiClient<List<Ticket>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetAllTickets"
            };

            var userClient = new ApiClient<UserViewModel>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetUser"
            };

            var flightClient = new ApiClient<List<Flight>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetAllFlights"
            };

            var cityClient = new ApiClient<List<City>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/City/GetAll"
            };

            allTickets = await ticketClient.GetAsync() ?? new List<Ticket>();
            List<User> users = (await userClient.GetAsync())?.users ?? new List<User>();
            List<Flight> flights = await flightClient.GetAsync() ?? new List<Flight>();
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();

            UserIdToFullNameConverter.UserNamesById = users
                .Where(u => u != null && !string.IsNullOrWhiteSpace(u.User_Id))
                .ToDictionary(u => u.User_Id, u => string.IsNullOrWhiteSpace(u.User_FullName) ? $"User {u.User_Id}" : u.User_FullName);

            CityIdToNameConverter.CityNamesById = cities
                .Where(c => c != null && !string.IsNullOrWhiteSpace(c.CityId))
                .ToDictionary(c => c.CityId.Trim(), c => c.CityName ?? c.CityId);

            FlightIdToNumberConverter.FlightNumberById = flights
                .Where(f => f != null && !string.IsNullOrWhiteSpace(f.Flight_Id))
                .ToDictionary(f => f.Flight_Id, f => string.IsNullOrWhiteSpace(f.Flight_Number) ? "N/A" : f.Flight_Number);

            TicketFlightRouteConverter.FlightsById = flights
                .Where(f => f != null && !string.IsNullOrWhiteSpace(f.Flight_Id))
                .ToDictionary(f => f.Flight_Id, f => f);

            ApplyFilterAndBind();
            UpdateStatistics();
        }

        private void ApplyFilterAndBind()
        {
            IEnumerable<Ticket> filtered = allTickets.Where(t => t != null);

            if (currentFilter == 1)
            {
                filtered = filtered.Where(t => t.Status);
            }
            else if (currentFilter == 2)
            {
                filtered = filtered.Where(t => !t.Status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(t =>
                    (t.Ticket_Id ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.User_Id ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.Flight_Id ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    ResolveUserName(t.User_Id).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    ResolveFlightNumber(t.Flight_Id).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            DataContext = filtered.ToList();
        }

        private static string ResolveUserName(string? userId)
        {
            string key = userId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return UserIdToFullNameConverter.UserNamesById.TryGetValue(key, out string? name)
                ? name
                : $"User {key}";
        }

        private static string ResolveFlightNumber(string? flightId)
        {
            string key = flightId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return FlightIdToNumberConverter.FlightNumberById.TryGetValue(key, out string? number)
                ? number
                : string.Empty;
        }

        private void UpdateStatistics()
        {
            int total = allTickets.Count;
            int active = allTickets.Count(t => t != null && t.Status);
            int cancelled = total - active;

            txtTotalTickets.Text = total.ToString("N0");
            txtActiveTickets.Text = active.ToString("N0");
            txtCancelledTickets.Text = cancelled.ToString("N0");
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            if (rbAll.IsChecked == true) currentFilter = 0;
            else if (rbActive.IsChecked == true) currentFilter = 1;
            else if (rbCancelled.IsChecked == true) currentFilter = 2;

            ApplyFilterAndBind();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            searchTerm = (txtSearch.Text ?? string.Empty).Trim();
            ApplyFilterAndBind();
        }

        private async void ToggleTicketStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Ticket ticket)
            {
                MessageBox.Show("Could not determine which ticket to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool newStatus = !ticket.Status;
            string action = ticket.Status ? "cancel" : "reactivate";


            var confirm = MessageBox.Show(
                $"Are you sure you want to {action} ticket {ticket.Ticket_Id}?",
                "Confirm Status Change",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);


            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            var apiClient = new ApiClient<bool>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/UpdateTicketStatus"
            };
            apiClient.SetQueryParameter("ticket_id", ticket.Ticket_Id);
            apiClient.SetQueryParameter("status", newStatus.ToString());

            bool ok = await apiClient.GetAsync();
            if (!ok)
            {
                MessageBox.Show("Failed to update ticket status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ticket.Status = newStatus;
            ApplyFilterAndBind();
            UpdateStatistics();
        }

        private async void DeleteTicket_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Ticket ticket)
            {
                MessageBox.Show("Could not determine which ticket to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ticket.Ticket_Id))
            {
                MessageBox.Show("Ticket ID is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var result = MessageBox.Show(
                $"Are you sure you want to delete Ticket {ticket.Ticket_Id}?\n\nThis action cannot be undone.",
                "Delete Ticket",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var apiClient = new ApiClient<bool>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/DeleteTicket"
            };
            apiClient.SetQueryParameter("ticket_id", ticket.Ticket_Id);

            bool ok = await apiClient.GetAsync();
            if (!ok)
            {
                MessageBox.Show("Failed to delete ticket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            allTickets.RemoveAll(t => t.Ticket_Id == ticket.Ticket_Id);
            ApplyFilterAndBind();
            UpdateStatistics();
        }

    }
}