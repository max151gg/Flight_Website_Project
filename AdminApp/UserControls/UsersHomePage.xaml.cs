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
    public partial class UsersHomePage : UserControl
    {
        private int currentFilter; // 0 = All, 1 = Admins, 2 = Regular
        private string searchTerm = string.Empty;
        private List<User> allUsers = new();

        public UsersHomePage()
        {
            InitializeComponent();
            Loaded += UsersHomePage_Loaded;
        }

        private async void UsersHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
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

            foreach (User user in allUsers)
            {
                user.User_Image = NormalizeImagePath(user.User_Image);
            }

            ApplyFilterAndBind();
            UpdateStatistics();
        }

        private string NormalizeImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return "http://localhost:5125/images/profiles/default.png";
            }

            if (imagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imagePath;
            }

            string normalized = imagePath.StartsWith("/") ? imagePath : $"/{imagePath}";
            return $"http://localhost:5125{normalized}";
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
                    (u.User_FullName ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.UserName ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.User_Id ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
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
            if (rbAll.IsChecked == true)
            {
                currentFilter = 0;
            }
            else if (rbAdmins.IsChecked == true)
            {
                currentFilter = 1;
            }
            else if (rbRegular.IsChecked == true)
            {
                currentFilter = 2;
            }

            ApplyFilterAndBind();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTerm = (txtSearch.Text ?? string.Empty).Trim();
            ApplyFilterAndBind();
        }

        private async void ToggleRole_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not User user)
            {
                MessageBox.Show("Could not determine which user to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

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

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not User user)
            {
                MessageBox.Show("Could not determine which user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Delete {user.User_FullName}?\n\nThis action cannot be undone.",
                "Delete User",
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
                Path = "api/Admin/DeleteUser"
            };
            apiClient.SetQueryParameter("user_id", user.User_Id);

            bool ok = await apiClient.GetAsync();
            if (!ok)
            {
                MessageBox.Show("Failed to delete user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            allUsers.RemoveAll(u => u.User_Id == user.User_Id);
            ApplyFilterAndBind();
            UpdateStatistics();
        }
    }
}
