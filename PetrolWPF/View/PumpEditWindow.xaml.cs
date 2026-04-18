using gsst.Model.FuelStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PetrolWPF.View
{
    public class SelectableTank
    {
        public Tank Tank { get; set; }
        public bool IsSelected { get; set; }
        public string DisplayText => $"#{Tank.Id} - {Tank.FuelType?.Name} ({Tank.Capacity}L)";
    }

    public partial class PumpEditWindow : Window
    {
        public Pump CurrentPump { get; private set; }
        public List<SelectableTank> SelectableTanks { get; set; }

        public PumpEditWindow(Pump pump, IEnumerable<Tank> allTanks)
        {
            InitializeComponent();

            StatusComboBox.ItemsSource = Enum.GetValues(typeof(PumpStatus))
                .Cast<PumpStatus>()
                .Where(s => s != PumpStatus.Busy)
                .ToList();

            CurrentPump = new Pump();
            CurrentPump.Id = pump.Id;
            CurrentPump.ConnectedTanks = new List<Tank>();

            if (pump.Id != 0)
            {
                CurrentPump.Name = pump.Name;
                CurrentPump.Status = pump.Status == PumpStatus.Busy ? PumpStatus.Free : pump.Status;

                NameTextBox.Text = CurrentPump.Name;
                StatusComboBox.SelectedItem = CurrentPump.Status;
            }
            else
            {
                StatusComboBox.SelectedItem = PumpStatus.Free;
            }

            SelectableTanks = allTanks.Select(t => new SelectableTank
            {
                Tank = t,
                IsSelected = pump.ConnectedTanks?.Any(ct => ct.Id == t.Id) ?? false
            }).ToList();

            TanksListBox.ItemsSource = SelectableTanks;

            Title = CurrentPump.Id == 0 ? "Add New Pump" : "Edit Pump";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Pump Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentPump.Name = NameTextBox.Text;

                if (StatusComboBox.SelectedItem != null)
                {
                    CurrentPump.Status = (PumpStatus)StatusComboBox.SelectedItem;
                }

                CurrentPump.ConnectedTanks = SelectableTanks
                    .Where(st => st.IsSelected)
                    .Select(st => st.Tank)
                    .ToList();

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