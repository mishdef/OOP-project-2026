using gsst.Interfaces;
using gsst.Model;
using gsst.Services;
using Gsstwpfmock.ViewModel;
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
using System.Windows.Shapes;

namespace Gsstwpfmock
{
    public partial class AdminDashboardWindow : Window
    {
        private bool Initialized = false;

        SolidColorBrush defaultBtnColor = new SolidColorBrush(Color.FromArgb(255, 254, 254, 254));
        SolidColorBrush activeBtnColor = new SolidColorBrush(Color.FromArgb(255, 234, 234, 234));

        public AdminDashboardWindow(AdminDashboardViewModel adminDashboardWindowViewModel)
        {
            this.DataContext = adminDashboardWindowViewModel;

            InitializeComponent();
            Initialized = true;
        }

        private void HideEveryGrid()
        {
            StatisticsGrid.Visibility = Visibility.Hidden;
            UsersGrid.Visibility = Visibility.Hidden;
            StocksGrid.Visibility = Visibility.Hidden;
            EquipmentGrid.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Hidden;
            ReportsGrid.Visibility = Visibility.Hidden;
            HistoryGrid.Visibility = Visibility.Hidden;

            StatisticsBtn.Background = defaultBtnColor;
            UsersBtn.Background = defaultBtnColor;
            StockBtn.Background = defaultBtnColor;
            EquipmentBtn.Background = defaultBtnColor;
            SettingsBtn.Background = defaultBtnColor;
            ReportsBtn.Background = defaultBtnColor;
            HistoryBtn.Background = defaultBtnColor;
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();
            ReportsGrid.Visibility = Visibility.Visible;
            ReportsBtn.Background = activeBtnColor;
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();
            StatisticsGrid.Visibility = Visibility.Visible;

            StatisticsBtn.Background = activeBtnColor;
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();
            UsersGrid.Visibility = Visibility.Visible;

            UsersBtn.Background = activeBtnColor;
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();
            StocksGrid.Visibility = Visibility.Visible;

            StockBtn.Background = activeBtnColor;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();

            SettingsGrid.Visibility = Visibility.Visible;

            SettingsBtn.Background = activeBtnColor;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();
            HistoryGrid.Visibility = Visibility.Visible;
            HistoryBtn.Background = activeBtnColor;
        }

        private void EquipmentBtn_Click(object sender, RoutedEventArgs e)
        {
            HideEveryGrid();

            EquipmentBtn.Background = activeBtnColor;
            EquipmentGrid.Visibility = Visibility.Visible;
        }

        private void DatabaseFilenameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) return;
            if (string.IsNullOrEmpty(DatabaseFilenameTextBox.Text))
                DBErrorLabel.Content = "Database filename cannot be empty";
            else
                DBErrorLabel.Content = "";

            var viewModel = (AdminDashboardViewModel)DataContext;
            viewModel.DatabaseName = DatabaseFilenameTextBox.Text;

            viewModel.SaveButtonEnabled();
        }

        private void BonusRateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Initialized) return;
            if (int.TryParse(BonusRateTextBox.Text, out int result))
            {
                ErrorLabel.Content = "";
                if (result < 0 || result > 100)
                {
                    ErrorLabel.Content = "Value must be between 0 and 100";
                }
                else
                {
                    var viewModel = (AdminDashboardViewModel)DataContext;
                    viewModel.SettingsCopy.BonusRate = result;

                    viewModel.SaveButtonEnabled();
                }
            }
            else
            {
                ErrorLabel.Content = "Invalid input";
            }
        }

        private void BonusRateTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                return;
            }

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                return;
            }

            e.Handled = true;
        }
    }
}
