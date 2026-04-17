using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWSClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdminApp.UserControls
{
    public partial class UsersHomePage : UserControl
    {
        private int currentFilter; // 0 = All, 1 = Admins, 2 = Regular
        private string searchTerm = string.Empty;
        private List<User> allUsers = new();

        private bool _uiReady = false;

        public UsersHomePage()
        {
            InitializeComponent();
            Loaded += UsersHomePage_Loaded;
        }

        private async void UsersHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            _uiReady = true;              // UI is now constructed
            await LoadUsers();            // this will call ApplyFilterAndBind()
        }



        private async Task LoadUsers()
        {
            try
            {
                var apiClient = new ApiClient<UserViewModel>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/GetUser"
                };

                UserViewModel vm = await apiClient.GetAsync();
                allUsers = vm?.users ?? new List<User>();

                

                ApplyFilterAndBind();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load users from API. Make sure SkyPathWS is running.", ex);
            }
        }

        private void ApplyFilterAndBind()
        {
            IEnumerable<User> filteredUsers = allUsers;

            if (currentFilter == 1)
            {
                filteredUsers = filteredUsers.Where(u => u.Role_Id == "0");
            }
            else if (currentFilter == 2)
            {
                filteredUsers = filteredUsers.Where(u => u.Role_Id != "0");
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredUsers = filteredUsers.Where(u =>
                    ((u.User_FullName ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((u.UserName ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((u.Email ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    ((u.User_Id ?? "").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            List<User> result = filteredUsers.ToList();
            UsersList.ItemsSource = result;
            txtNewUsers.Text = result.Count.ToString();
        }

        private void UpdateStatistics()
        {
            int totalUsers = allUsers.Count;
            int totalAdmins = allUsers.Count(u => u.Role_Id == "0");
            int regularUsers = totalUsers - totalAdmins;

            txtTotalUsers.Text = totalUsers.ToString();
            txtTotalAdmins.Text = totalAdmins.ToString();
            txtRegularUsers.Text = regularUsers.ToString();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (!_uiReady) return;        // ignore early events

            if (rbAll.IsChecked == true) currentFilter = 0;
            else if (rbAdmins.IsChecked == true) currentFilter = 1;
            else if (rbRegular.IsChecked == true) currentFilter = 2;

            ApplyFilterAndBind();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTerm = (txtSearch.Text ?? string.Empty).Trim();
            ApplyFilterAndBind();
        }

        private async void ToggleRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not Button btn || btn.Tag is not User user)
                {
                    MessageBox.Show("Could not determine which user to update.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool isAdmin = user.Role_Id == "0";
                string newRoleId = isAdmin ? "1" : "0";
                string action = isAdmin ? "demote" : "promote";

                var result = MessageBox.Show(
                    $"Are you sure you want to {action} this user?\n\nThis will change their access permissions.",
                    "Confirm Role Change",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                var apiClient = new ApiClient<bool>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/UpdateUserRole"
                };
                apiClient.SetQueryParameter("user_id", user.User_Id);
                apiClient.SetQueryParameter("role_id", newRoleId);

                bool ok = await apiClient.GetAsync();
                if (!ok)
                {
                    MessageBox.Show("Failed to update role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                user.Role_Id = newRoleId;
                ApplyFilterAndBind();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Role change failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ToggleBan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not Button btn || btn.Tag is not User user)
                {
                    MessageBox.Show("Could not determine which user to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool newBanStatus = !user.User_Ban;
                string action = newBanStatus ? "ban" : "unban";
                string pastAction = newBanStatus ? "banned" : "unbanned";

                var result = MessageBox.Show(
                    $"Are you sure you want to {action} {user.User_FullName}?",
                    newBanStatus ? "Ban User" : "Unban User",
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
                    Path = "api/Admin/BanUser"
                };
                apiClient.SetQueryParameter("user_id", user.User_Id);
                apiClient.SetQueryParameter("user_Ban", newBanStatus.ToString().ToLowerInvariant());

                bool ok = await apiClient.GetAsync();
                if (!ok)
                {
                    MessageBox.Show($"Failed to {action} user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                user.User_Ban = newBanStatus;
                MessageBox.Show($"User {pastAction} successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                ApplyFilterAndBind();
                UpdateStatistics();
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("Ban request timed out. Please verify SkyPathWS is running and try again.",
                    "Request Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ban operation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
