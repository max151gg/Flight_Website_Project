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
    /// Interaction logic for UsersHomePage.xaml
    /// </summary>
    public partial class UsersHomePage : UserControl
    {
        private int currentFilter = 0; // 0 = All, 1 = Admins, 2 = Regular
        private string searchTerm = "";

        public UsersHomePage()
        {
            InitializeComponent();
            LoadUsers();
            UpdateStatistics();
        }

        private void LoadUsers()
        {
            // TODO: Load users from database
            // For now, the sample users are hardcoded in XAML

            /*
            UsersContainer.Children.Clear();
            
            var users = GetUsersFromDatabase();
            
            // Apply filters
            if (currentFilter == 1) // Admins only
            {
                users = users.Where(u => u.RoleId == 0).ToList();
            }
            else if (currentFilter == 2) // Regular users only
            {
                users = users.Where(u => u.RoleId == 1).ToList();
            }
            
            // Apply search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users.Where(u => 
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.UserId.ToLower().Contains(searchTerm)).ToList();
            }
            
            foreach (var user in users)
            {
                var userCard = CreateUserCard(user);
                UsersContainer.Children.Add(userCard);
            }
            */
        }

        private void UpdateStatistics()
        {
            // TODO: Calculate from database
            // For now, using sample data

            /*
            var users = GetUsersFromDatabase();
            int totalUsers = users.Count;
            int totalAdmins = users.Count(u => u.RoleId == 0);
            int regularUsers = users.Count(u => u.RoleId == 1);
            int newThisMonth = users.Count(u => u.CreatedDate >= DateTime.Now.AddMonths(-1));
            
            txtTotalUsers.Text = totalUsers.ToString();
            txtTotalAdmins.Text = totalAdmins.ToString();
            txtRegularUsers.Text = regularUsers.ToString();
            txtNewUsers.Text = newThisMonth.ToString();
            */
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

            LoadUsers();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchTerm = txtSearch.Text.ToLower().Trim();
            LoadUsers();
        }

        private void ToggleRole_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string currentRole = button.Content.ToString().Contains("Promote") ? "user" : "admin";
            string newRole = currentRole == "user" ? "admin" : "user";
            string action = currentRole == "user" ? "promote" : "demote";

            var result = MessageBox.Show(
                $"Are you sure you want to {action} this user to {newRole}?\n\nThis will change their access permissions.",
                $"Confirm Role Change",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // TODO: Update user role in database
                    /*
                    var userId = button.Tag as string;
                    int newRoleId = newRole == "admin" ? 0 : 1;
                    UpdateUserRole(userId, newRoleId);
                    */

                    MessageBox.Show(
                        $"User successfully {action}d to {newRole}!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Reload users and update statistics
                    LoadUsers();
                    UpdateStatistics();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error updating user role: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this user?\n\n" +
                "⚠️ WARNING: This action cannot be undone!\n" +
                "• All user data will be permanently deleted\n" +
                "• User bookings and history will be lost\n" +
                "• This user will no longer be able to access the system",
                "Delete User - Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Second confirmation for safety
                var finalConfirm = MessageBox.Show(
                    "FINAL CONFIRMATION\n\nAre you absolutely sure you want to delete this user account?",
                    "Delete User - Final Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop);

                if (finalConfirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        // TODO: Delete user from database
                        /*
                        var button = sender as Button;
                        var userId = button.Tag as string;
                        DeleteUserFromDatabase(userId);
                        */

                        MessageBox.Show(
                            "User account deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Reload users and update statistics
                        LoadUsers();
                        UpdateStatistics();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error deleting user: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        // Helper method to create user card programmatically
        /*
        private Border CreateUserCard(User user)
        {
            var border = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(20)
            };

            border.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 270,
                ShadowDepth = 2,
                BlurRadius = 10,
                Opacity = 0.08
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Profile Image
            var profileBorder = new Border
            {
                Width = 80,
                Height = 80,
                CornerRadius = new CornerRadius(40),
                Background = GetColorForUser(user.UserId),
                VerticalAlignment = VerticalAlignment.Top
            };

            var initials = new TextBlock
            {
                Text = GetInitials(user.FullName),
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            profileBorder.Child = initials;
            Grid.SetColumn(profileBorder, 0);
            grid.Children.Add(profileBorder);

            // User Info Grid
            var infoGrid = new Grid { Margin = new Thickness(20, 0, 0, 0) };
            Grid.SetColumn(infoGrid, 1);
            // ... Add user info sections ...

            // Action Buttons
            var buttonStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(buttonStack, 2);

            var roleButton = new Button
            {
                Content = user.RoleId == 0 ? "Demote to User" : "Promote to Admin",
                Style = (Style)FindResource("ActionButtonStyle"),
                Background = user.RoleId == 0 
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#667EEA")),
                Foreground = Brushes.White,
                Tag = user.UserId
            };
            roleButton.Click += ToggleRole_Click;
            buttonStack.Children.Add(roleButton);

            var deleteButton = new Button
            {
                Content = "Delete User",
                Style = (Style)FindResource("ActionButtonStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC3545")),
                Foreground = Brushes.White,
                Margin = new Thickness(5, 10, 5, 0),
                Tag = user.UserId
            };
            deleteButton.Click += DeleteUser_Click;
            buttonStack.Children.Add(deleteButton);

            grid.Children.Add(buttonStack);

            border.Child = grid;
            return border;
        }

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
            }
            return fullName.Substring(0, Math.Min(2, fullName.Length)).ToUpper();
        }

        private Brush GetColorForUser(string userId)
        {
            // Generate consistent color based on user ID
            var colors = new[]
            {
                "#667EEA", "#28A745", "#F5576C", "#FFC107", 
                "#17A2B8", "#6C757D", "#E83E8C", "#20C997"
            };
            
            int hash = userId.GetHashCode();
            int index = Math.Abs(hash % colors.Length);
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[index]));
        }
        */
    }
}
