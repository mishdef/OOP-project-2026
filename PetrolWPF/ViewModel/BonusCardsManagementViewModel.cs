using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using gsst.Interfaces;
using gsst.Model;
using PetrolWPF.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Gsstwpfmock.ViewModel
{
    public partial class BonusCardsManagementViewModel : ObservableObject
    {
        private readonly IBonusService _bonusService;

        [ObservableProperty]
        private ObservableCollection<BonusCard> _bonusCards;

        [ObservableProperty]
        private BonusCard _selectedCard;

        public BonusCardsManagementViewModel(IBonusService bonusService)
        {
            _bonusService = bonusService;
            LoadCards();
        }

        private void LoadCards()
        {
            BonusCards = new ObservableCollection<BonusCard>(_bonusService.GetAllBonusCards());
        }

        [RelayCommand]
        public void AddCard()
        {
            var newCard = new BonusCard();
            var window = new BonusCardEditWindow(newCard);

            if (window.ShowDialog() == true)
            {
                try
                {
                    _bonusService.CreateBonusCard(window.CurrentCard.ClientName, window.CurrentCard.Barcode);
                    LoadCards();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void EditCard()
        {
            if (SelectedCard == null)
            {
                MessageBox.Show("Please select a card to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new BonusCardEditWindow(SelectedCard);
            if (window.ShowDialog() == true)
            {
                try
                {
                    _bonusService.UpdateBonusCard(SelectedCard.Id, window.CurrentCard.ClientName, window.CurrentCard.Barcode);
                    LoadCards(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void DeleteCard()
        {
            if (SelectedCard == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete card for '{SelectedCard.ClientName}'?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bonusService.DeleteBonusCard(SelectedCard.Id);
                    LoadCards();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}