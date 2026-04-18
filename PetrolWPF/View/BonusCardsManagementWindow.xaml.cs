using Gsstwpfmock.ViewModel;
using System.Windows;

namespace PetrolWPF.View
{
    public partial class BonusCardsManagementWindow : Window
    {
        public BonusCardsManagementWindow(BonusCardsManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}