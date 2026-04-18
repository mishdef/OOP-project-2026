using gsst.Model;
using Gsstwpfmock.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PetrolWPF.View
{
    public partial class BonusWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public BonusWindow(MainWindowViewModel DataContext)
        {
            InitializeComponent();
            _mainWindowViewModel = DataContext;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddToReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            var amountText = double.TryParse(AddToReceiptTextBox.Text, out double amount) ? amount : 0;

            try
            {
                _mainWindowViewModel.OrderBuilder.SetDiscountFromBonus(amountText);

                _mainWindowViewModel.CurrentOrder = _mainWindowViewModel.OrderBuilder.Build();
                _mainWindowViewModel.Subtotal = _mainWindowViewModel.CurrentOrder.Total;
                _mainWindowViewModel.Discount = _mainWindowViewModel.CurrentOrder.BonusSpent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveBonusesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainWindowViewModel.OrderBuilder.SetDiscountFromBonus(0);
                _mainWindowViewModel.CurrentOrder = _mainWindowViewModel.OrderBuilder.Build();
                _mainWindowViewModel.Subtotal = _mainWindowViewModel.CurrentOrder.Total;
                _mainWindowViewModel.Discount = _mainWindowViewModel.CurrentOrder.BonusSpent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
