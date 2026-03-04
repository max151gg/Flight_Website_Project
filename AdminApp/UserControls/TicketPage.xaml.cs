using AdminApp.Converters;
using SkyPath_Models.Models;
using SkyPathWSClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private List<TicketDisplayItem> allTickets = new();

        public TicketPage()
        {
            InitializeComponent();
            Loaded += TicketPage_Loaded;
        }


        private async void TicketPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateLayout();
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

            var userClient = new ApiClient<SkyPath_Models.ViewModel.UserViewModel>
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

            List<Ticket> tickets = await ticketClient.GetAsync() ?? new List<Ticket>();
            var usersVm = await userClient.GetAsync();
            List<User> users = usersVm?.users ?? new List<User>();
            List<Flight> flights = await flightClient.GetAsync() ?? new List<Flight>();
            List<City> cities = await cityClient.GetAsync() ?? new List<City>();

            var userNameById = users
                .Where(u => u != null && !string.IsNullOrWhiteSpace(u.User_Id))
                .ToDictionary(u => u.User_Id, u => string.IsNullOrWhiteSpace(u.User_FullName) ? $"User {u.User_Id}" : u.User_FullName);

            var flightById = flights
                .Where(f => f != null && !string.IsNullOrWhiteSpace(f.Flight_Id))
                .ToDictionary(f => f.Flight_Id, f => f);

            CityIdToNameConverter.CityNamesById = cities
                .Where(c => c != null && !string.IsNullOrWhiteSpace(c.CityId))
                .ToDictionary(c => c.CityId.Trim(), c => c.CityName ?? c.CityId);

            allTickets = tickets.Where(t => t != null).Select(t =>
            {
                flightById.TryGetValue(t.Flight_Id ?? string.Empty, out Flight? flight);
                string departure = ResolveCityName(flight?.Departure_Id);
                string arrival = ResolveCityName(flight?.Arrival_Id);

                return new TicketDisplayItem
                {
                    Ticket_Id = t.Ticket_Id,
                    User_Id = t.User_Id,
                    Flight_Id = t.Flight_Id,
                    Purchase_Date = t.Purchase_Date,
                    Status = t.Status,
                    Type = t.Type,
                    UserFullName = userNameById.TryGetValue(t.User_Id ?? string.Empty, out var fullName) ? fullName : $"User {t.User_Id}",
                    FlightNumber = string.IsNullOrWhiteSpace(flight?.Flight_Number) ? "N/A" : flight.Flight_Number,
                    Route = string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival)
                        ? "N/A"
                        : $"{departure} → {arrival}"
                };
            }).ToList();

            ApplyFilterAndBind();
            UpdateStatistics();
        }

        private static string ResolveCityName(string? cityId)
        {
            if (string.IsNullOrWhiteSpace(cityId))
            {
                return string.Empty;
            }

            if (CityIdToNameConverter.CityNamesById.TryGetValue(cityId.Trim(), out string? name))
            {
                return name;
            }

            return cityId;
        }
        private bool EnsureTicketsListReady()
        {
            if (TicketsList != null)
            {
                return true;
            }

            var located = FindName("TicketsList") as ItemsControl;
            if (located != null)
            {
                TicketsList = located;
                return true;
            }

            return false;
        }
        private void ApplyFilterAndBind()
        {
            if (!EnsureTicketsListReady())
            {
                return;
            }

            IEnumerable<TicketDisplayItem> filtered = allTickets;

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
                    (t.UserFullName ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.Flight_Id ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (t.FlightNumber ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            TicketsList.ItemsSource = filtered.ToList();
        }

        private void UpdateStatistics()
        {
            int total = allTickets.Count;
            int active = allTickets.Count(t => t.Status);
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
            if (sender is not Button btn || btn.Tag is not TicketDisplayItem ticket)
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
            apiClient.SetQueryParameter("ticket_id", ticket.Ticket_Id ?? string.Empty);
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
            if (sender is not Button btn || btn.Tag is not TicketDisplayItem ticket)
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
                $"Are you sure you want to delete ticket {ticket.Ticket_Id}?\n\nThis action cannot be undone.",
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

        private class TicketDisplayItem
        {
            public string? Ticket_Id { get; set; }
            public string? User_Id { get; set; }
            public string? Flight_Id { get; set; }
            public string? Purchase_Date { get; set; }
            public bool Status { get; set; }
            public string? Type { get; set; }

            public string? UserFullName { get; set; }
            public string? FlightNumber { get; set; }
            public string? Route { get; set; }

            
            
            
            
        }
    }
}