using System;
using System.Windows.Controls;
using gsst.Model;
using gsst.Model.FuelStuff;

namespace PetrolControlLibrary
{
    public partial class PumpControl : UserControl
    {
        public event EventHandler<EventArgs> InfoClicked;
        public event Action<Pump> PumpSelected;

        public PumpControl()
        {
            InitializeComponent();
        }

        private void InfoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InfoClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var pump = DataContext as Pump;

            if (pump == null && DataContext != null)
            {
                try
                {
                    dynamic vm = DataContext;
                    pump = vm.Model as Pump;
                }
                catch { }
            }

            if (pump != null)
            {
                PumpSelected?.Invoke(pump);
            }
        }
    }
}