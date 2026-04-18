using gsst.Model; 
using System.Windows;

namespace PetrolWPF.View
{
    public partial class BonusCardEditWindow : Window
    {
        public BonusCard CurrentCard { get; private set; }

        public BonusCardEditWindow(BonusCard card)
        {
            InitializeComponent();
            CurrentCard = card;

            ClientNameTextBox.Text = CurrentCard.ClientName;
            BarcodeTextBox.Text = CurrentCard.Barcode;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentCard.ClientName = ClientNameTextBox.Text;
            CurrentCard.Barcode = BarcodeTextBox.Text;

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}