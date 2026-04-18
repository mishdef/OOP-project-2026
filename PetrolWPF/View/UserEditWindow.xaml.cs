using gsst.Model.User;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PetrolWPF.View
{
    public partial class UserEditWindow : Window
    {
        public User CurrentUser { get; private set; }

        public UserEditWindow(User user)
        {
            InitializeComponent();

            RoleComboBox.ItemsSource = new List<string> { UserRoles.Admin, UserRoles.Cashier };

            CurrentUser = new User();
            CurrentUser.Id = user.Id;
            CurrentUser.Role = user.Role; 

            if (user.Id != 0)
            {
                CurrentUser.FullName = user.FullName;
                CurrentUser.Username = user.Username;
                CurrentUser.Password = user.Password;

                FullNameTextBox.Text = CurrentUser.FullName;
                UsernameTextBox.Text = CurrentUser.Username;
                VisiblePasswordTextBox.Text = CurrentUser.Password;
                HiddenPasswordBox.Password = CurrentUser.Password;
            }

            RoleComboBox.SelectedItem = CurrentUser.Role;
            Title = CurrentUser.Id == 0 ? "Add New User" : "Edit User";
        }

        private bool _isSyncing = false;

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (ShowPasswordButton.IsChecked == true)
            {
                VisiblePasswordTextBox.Visibility = Visibility.Visible;
                HiddenPasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                VisiblePasswordTextBox.Visibility = Visibility.Collapsed;
                HiddenPasswordBox.Visibility = Visibility.Visible;
            }
        }

        private void HiddenPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isSyncing)
            {
                _isSyncing = true;
                VisiblePasswordTextBox.Text = HiddenPasswordBox.Password;
                _isSyncing = false;
            }
        }
        private void VisiblePasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isSyncing)
            {
                _isSyncing = true;
                HiddenPasswordBox.Password = VisiblePasswordTextBox.Text;
                _isSyncing = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentUser.FullName = FullNameTextBox.Text;
                CurrentUser.Username = UsernameTextBox.Text;

                if (RoleComboBox.SelectedItem != null)
                {
                    CurrentUser.Role = RoleComboBox.SelectedItem.ToString();
                }

                if (CurrentUser.Id == 0 && string.IsNullOrWhiteSpace(HiddenPasswordBox.Password))
                {
                    MessageBox.Show("Password is required for a new user.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(HiddenPasswordBox.Password))
                {
                    CurrentUser.Password = HiddenPasswordBox.Password;
                }

                DialogResult = true;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}