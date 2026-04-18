using gsst.Model.FuelStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PetrolWPF.View
{
    public partial class TankEditWindow : Window
    {
        public Tank CurrentTank { get; private set; }

        public TankEditWindow(Tank tank, IEnumerable<FuelType> availableFuelTypes)
        {
            InitializeComponent();

            FuelTypeComboBox.ItemsSource = availableFuelTypes;

            CurrentTank = new Tank();
            CurrentTank.Id = tank.Id;

            if (tank.Id != 0) 
            {
                CurrentTank.FuelType = tank.FuelType;
                CurrentTank.Capacity = tank.Capacity;
                CurrentTank.Volume = tank.Volume;

                CapacityTextBox.Text = CurrentTank.Capacity.ToString();
                VolumeTextBox.Text = CurrentTank.Volume.ToString();

                FuelTypeComboBox.SelectedItem = availableFuelTypes.FirstOrDefault(f => f.Id == tank.FuelType?.Id);
            }
            else 
            {
                VolumeTextBox.Text = "0";
                VolumeTextBox.IsEnabled = false;
                FuelTypeComboBox.SelectedIndex = 0;
            }

            Title = CurrentTank.Id == 0 ? "Add New Tank" : "Edit Tank";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FuelTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Fuel Type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(CapacityTextBox.Text.Replace(".", ","), out double capacity) || capacity < 10)
                {
                    MessageBox.Show("Capacity must be at least 10 liters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double volume = 0;
                if (CurrentTank.Id != 0)
                {
                    if (!double.TryParse(VolumeTextBox.Text.Replace(".", ","), out volume) || volume < 0 || volume > capacity)
                    {
                        MessageBox.Show("Volume must be between 0 and Capacity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                CurrentTank.FuelType = (FuelType)FuelTypeComboBox.SelectedItem;
                CurrentTank.Capacity = capacity;
                CurrentTank.Volume = volume;

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