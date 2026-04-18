using CommunityToolkit.Mvvm.ComponentModel;
using gsst.Model.FuelStuff;

namespace Gsstwpfmock.ViewModel
{
    public partial class PumpViewModel : ObservableObject
    {
        public Pump Model { get; }

        [ObservableProperty]
        private bool _isAvailable = true;

        public PumpViewModel(Pump pump)
        {
            Model = pump;
        }
    }
}