using gsst.Model.FuelStuff;
using System;
using System.Windows;

namespace PetrolWPF.View
{
    public partial class FuelTypeEditWindow : Window
    {
        public FuelType CurrentFuelType { get; private set; }

        public FuelTypeEditWindow(FuelType fuelType)
        {
            InitializeComponent();

            CurrentFuelType = new FuelType();
            CurrentFuelType.Id = fuelType.Id;

            if (fuelType.Id != 0)
            {
                CurrentFuelType.Name = fuelType.Name;
                CurrentFuelType.Price = fuelType.Price;

                NameTextBox.Text = CurrentFuelType.Name;
                PriceTextBox.Text = CurrentFuelType.Price.ToString();
            }

            Title = CurrentFuelType.Id == 0 ? "Add New Fuel Type" : "Edit Fuel Type";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Fuel Type Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(PriceTextBox.Text.Replace(".", ","), out double price) || price < 0)
                {
                    MessageBox.Show("Please enter a valid positive price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentFuelType.Name = NameTextBox.Text;
                CurrentFuelType.Price = price;

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