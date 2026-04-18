using gsst.Interfaces;
using gsst.Model.FuelStuff;
using gsst.Model.User;
using gsst.Services;
using Gsstwpfmock.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using PetrolWPF.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gsstwpfmock
{
    public partial class MainWindow : Window
    {
        public User User { get; set; }

        private readonly IServiceProvider _serviceProvider;

        SolidColorBrush defaultBtnColor = new SolidColorBrush(Color.FromArgb(255, 223, 223, 223));
        SolidColorBrush selectedBtnColor = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));

        public MainWindow(MainWindowViewModel mainViewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
            _serviceProvider = serviceProvider;

            HideGoodsGroup();
            ShowFuelGroup();
        }

        private void HideFuelGroup()
        {
            PumpsGroupBox.Visibility = Visibility.Hidden;
            FuelGroupBox.Visibility = Visibility.Hidden;
            BonusCardBorder.Visibility = Visibility.Hidden;
        }


        private void ShowFuelGroup()
        {
            MainGrid.RowDefinitions.Clear();

            var row = new RowDefinition();
            row.Height = new GridLength(40);

            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(1.5, GridUnitType.Star);

            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(1, GridUnitType.Star);

            MainGrid.RowDefinitions.Add(row);
            MainGrid.RowDefinitions.Add(row1);
            MainGrid.RowDefinitions.Add(row2);

            PumpsGroupBox.Visibility = Visibility.Visible;
            FuelGroupBox.Visibility = Visibility.Visible;
            BonusCardBorder.Visibility = Visibility.Visible;
        }

        private void HideGoodsGroup()
        {
            GoodsGrid.Visibility = Visibility.Hidden;
        }

        private void ShowGoodsGroup()
        {
            MainGrid.RowDefinitions.Clear();

            GoodsGrid.Visibility = Visibility.Visible;
        }

        private void GoodsButtonClick(object sender, RoutedEventArgs e)
        {
            HideFuelGroup();
            ShowGoodsGroup();

            GoodsButton.FontWeight = FontWeights.Bold;
            FuelButton.FontWeight = FontWeights.Normal;

            GoodsButton.Background = selectedBtnColor;
            FuelButton.Background = defaultBtnColor;
        }

        private void FuelButtonClick(object sender, RoutedEventArgs e)
        {
            HideGoodsGroup();
            ShowFuelGroup();

            FuelButton.FontWeight = FontWeights.Bold;
            GoodsButton.FontWeight = FontWeights.Normal;

            FuelButton.Background = selectedBtnColor;
            GoodsButton.Background = defaultBtnColor;
        }

        private void AdminDashboardMenuItemClick(object sender, RoutedEventArgs e)
        {
            var authWindow = _serviceProvider.GetRequiredService<AdminDashboardWindow>();
            authWindow.Owner = this;
            authWindow.ShowDialog();
        }

        private void LogoutButtonClick(object sender, RoutedEventArgs e)
        {
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();

            authWindow.Show();
            this.Close();
        }

        private void ScanerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F7)
            {
                GoodsButtonClick(null, null);
                SearchBox.Focus();
            }
        }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var bindingExpression = SearchBox.GetBindingExpression(TextBox.TextProperty);
                bindingExpression?.UpdateSource();

                if (DataContext is MainWindowViewModel viewModel)
                {
                    if (viewModel.Products != null && viewModel.Products.Count == 1)
                    {
                        viewModel.AddProductCommand.Execute(viewModel.Products[0]);

                        SearchBox.Text = "";

                        e.Handled = true;
                    }
                    else
                    {
                        Keyboard.ClearFocus();
                    }
                }
            }
        }


        private void ConfigureBonusCardButtonClick(object sender, RoutedEventArgs e)
        {
            var bonusWindow = new BonusWindow((MainWindowViewModel)DataContext);
            bonusWindow.Owner = this;
            bonusWindow.DataContext = (MainWindowViewModel)DataContext;
            bonusWindow.ShowDialog();
        }

        private void BonusCardsMenuItemClick(object sender, RoutedEventArgs e)
        {
            var bonusCardsWindow = _serviceProvider.GetRequiredService<PetrolWPF.View.BonusCardsManagementWindow>();
            bonusCardsWindow.Owner = this;
            bonusCardsWindow.ShowDialog();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
