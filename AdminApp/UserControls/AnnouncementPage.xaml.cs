using AdminApp.Pages;
using AdminApp.UserControls;
using Microsoft.Win32;
using ModelSkyPath.Models;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace AdminApp.UserControls
{
    /// <summary>
    /// Interaction logic for AnnouncementPage.xaml
    /// </summary>
    public partial class AnnouncementPage : UserControl
    {
        private string? currentFilterUserId = null;
        List<Announcement> announcements;

        public AnnouncementPage()
        {
            InitializeComponent();
            Loaded += AnnouncementPage_Loaded;
        }

        private async void AnnouncementPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAnnouncements();
        }

        private async Task LoadAnnouncements()
        {
            var apiClient = new ApiClient<List<Announcement>>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/GetAllAnnouncements"
            };

            announcements = await apiClient.GetAsync() ?? new List<Announcement>();

            // Apply current filter (if any) after loading
            ApplyFilterToAnnouncements();
        }

        

        private void ApplyFilterToAnnouncements()
        {
            if (announcements == null) return;

            // No filter => show all
            if (string.IsNullOrWhiteSpace(currentFilterUserId))
            {
                DataContext = announcements;
                return;
            }

            // Filter is active:
            // If they typed "0" -> only public
            // Otherwise -> only that user's announcements
            var filtered = announcements
                .Where(a => a.User_Id == currentFilterUserId)
                .ToList();

            DataContext = filtered;
        }


        private async void CreateAnnouncement_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
                Announcement announcement = new Announcement
                {
                    Announcement_Id = "",
                    Admin_Id = "0",
                    Title = NormalizeTitle(txtTitle.Text),
                    Content = txtContent.Text.Trim(),
                    Announcement_Date = dpAnnouncementDate.SelectedDate?.ToString("dd-MM-yyyy") ?? string.Empty,
                    User_Id = string.IsNullOrWhiteSpace(txtUserId.Text) ? "0" : txtUserId.Text.Trim()
                };

                var apiClient = new ApiClient<Announcement>
                {
                    Scheme = "http",
                    Host = "localhost",
                    Port = 5125,
                    Path = "api/Admin/CreateAnnouncement"
                };

                bool created = await apiClient.PostAsync(announcement);
                if (!created)
                {
                    MessageBox.Show("Failed to create announcement.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Announcement created successfully!",
                               "Success",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);

                ClearForm();
                await LoadAnnouncements();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating announcement: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        private string NormalizeTitle(string title)
        {
            string value = (title ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return char.ToUpper(value[0]) + value.Substring(1);
        }

        private bool ValidateInputs()
        {
            string title = (txtTitle.Text ?? string.Empty).Trim();
            string content = (txtContent.Text ?? string.Empty).Trim();
            string userId = (txtUserId.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter an announcement title.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (title.Length < 2 || title.Length > 20)
            {
                MessageBox.Show("Title must be between 2 and 20 characters.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (!char.IsUpper(title[0]))
            {
                MessageBox.Show("Title must start with a capital letter.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please enter announcement content.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtContent.Focus();
                return false;
            }

            if (content.Length > 500)
            {
                MessageBox.Show("Content must be no longer than 500 characters.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtContent.Focus();
                return false;
            }

            if (!dpAnnouncementDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an announcement date.",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                dpAnnouncementDate.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(userId) && !int.TryParse(userId, out _))
            {
                MessageBox.Show("User ID must be a number (or leave empty for public).",
                               "Validation Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Warning);
                txtUserId.Focus();
                return false;
            }

            return true;
        }


        private void ClearForm()
        {
            txtTitle.Clear();
            txtContent.Clear();
            txtUserId.Clear();
            dpAnnouncementDate.SelectedDate = DateTime.Now;
            txtTitle.Focus();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            string filterUserId = txtFilterUserId.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterUserId))
            {
                MessageBox.Show("Please enter a User ID to filter.",
                    "Filter Required", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFilterUserId.Focus();
                return;
            }

            // Optional: enforce numeric user IDs
            if (!int.TryParse(filterUserId, out _))
            {
                MessageBox.Show("User ID must be a number (use 0 for public).",
                    "Invalid User ID", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFilterUserId.Focus();
                return;
            }

            currentFilterUserId = filterUserId;

            if (filterUserId == "0")
                txtFilterStatus.Text = "Showing public announcements";
            else
                txtFilterStatus.Text = $"Showing announcements for User ID: {filterUserId}";

            ApplyFilterToAnnouncements();
        }

        private async void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            currentFilterUserId = null;
            txtFilterUserId.Clear();
            txtFilterStatus.Text = "";

            // Show all again
            ApplyFilterToAnnouncements();

            // If you want to reload from server:
            await LoadAnnouncements();
        }

        private async void DeleteAnnouncement_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not Announcement announcement)
            {
                MessageBox.Show("Could not determine which announcement to delete.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Use the REAL id
            string announcement_id = announcement.Announcement_Id?.ToString();
            if (string.IsNullOrWhiteSpace(announcement_id))
            {
                MessageBox.Show("Announcement ID is missing.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete announcement {announcement_id}?\n\nThis action cannot be undone.",
                "Delete Announcement",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                bool deleted = await DeleteAnnouncementFromDatabaseAsync(announcement_id);

                if (!deleted)
                {
                    MessageBox.Show($"Delete failed for announcement {announcement_id}.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Announcement deleted successfully!",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await LoadAnnouncements(); // actually await it
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting announcement {announcement_id}: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task<bool> DeleteAnnouncementFromDatabaseAsync(string announcement_id)
        {
            var apiClient = new ApiClient<bool>
            {
                Scheme = "http",
                Host = "localhost",
                Port = 5125,
                Path = "api/Admin/DeleteAnnouncement"
            };

            apiClient.SetQueryParameter("announcement_id", announcement_id);
            return await apiClient.GetAsync();
        }

    }
}
