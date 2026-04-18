using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using gsst.Interfaces;
using gsst.Model;
using gsst.Model.FuelStuff;
using gsst.Model.User;
using gsst.Services;
using PetrolWPF.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gsstwpfmock.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPumpService _pumpService;
        private readonly IGoodsService _goodsService;
        private readonly IFuelTypeService _fuelTypeService;
        private readonly IBonusService _bonusService;
        private readonly UserSession _userSession;
        private readonly ITanksService _tanksService;
        private readonly IOrderService _orderService;


        public ObservableCollection<PumpViewModel> Pumps { get; set; } = new();

        [ObservableProperty]
        private ObservableCollection<Product> _products;
        [ObservableProperty]



        [NotifyPropertyChangedFor(nameof(IsPumpSelected))]
        private Pump? _selectedPump;
        [ObservableProperty]
        private Product? _selectedProduct;



        [ObservableProperty]
        private ObservableCollection<FuelType> _fuelTypes;
        [ObservableProperty]
        private FuelType? _selectedFuelType;



        [ObservableProperty]
        private string? _name;
        [ObservableProperty]
        private bool _isAdmin;






        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBonusCardValid))]
        private BonusCard? _bonusCard;
        public bool IsBonusCardValid => BonusCard != null;


        [ObservableProperty]
        private Order _currentOrder = new Order();
        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems;
        [ObservableProperty]
        private double _subtotal;
        [ObservableProperty]
        private double _discount;


        [ObservableProperty]
        private OrderBuilder _orderBuilder;





        public bool IsPumpSelected => SelectedPump != null;




        public MainWindowViewModel(IServiceProvider serviceProvider, IPumpService pumpService, IGoodsService goodsService, IFuelTypeService fuelTypeService, IBonusService bonusService, UserSession userSession, ITanksService tanksService, IOrderService orderService)
        {
            _serviceProvider = serviceProvider;
            _pumpService = pumpService;
            _goodsService = goodsService;
            _fuelTypeService = fuelTypeService;
            _bonusService = bonusService;
            _tanksService = tanksService;
            _orderService = orderService;

            _pumpService.PumpStateChanged += OnPumpStateChanged;

            _userSession = userSession;
            _orderBuilder = new OrderBuilder(_bonusService);

            Initalize();

            WeakReferenceMessenger.Default.Register<ProductsChangedMessage>(this, (recipient, message) =>
            {
                Application.Current.Dispatcher.Invoke(() => LoadProducts());
            });

            WeakReferenceMessenger.Default.Register<FuelTypesChangedMessage>(this, (recipient, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FuelTypes = new ObservableCollection<FuelType>(_fuelTypeService.GetAllFuelTypes());

                    if (SelectedFuelType != null && !FuelTypes.Any(f => f.Id == SelectedFuelType.Id))
                    {
                        SelectedFuelType = FuelTypes.FirstOrDefault();
                    }
                    LoadActivePumps();
                });
            });

            WeakReferenceMessenger.Default.Register<PumpsChangedMessage>(this, (recipient, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadActivePumps();
                });
            });

            WeakReferenceMessenger.Default.Register<TanksChangedMessage>(this, (recipient, message) =>
            {
            });
        }

        public void LoadProducts()
        {
            Products = new ObservableCollection<Product>(_goodsService.GetAllProducts());
        }

        [RelayCommand]
        public void SetFuelType(FuelType fuelType)
        {
            SelectedFuelType = fuelType;

            var availableFuelTypes = _pumpService.GetFuelTypesForPump(SelectedPump.Id);

            FuelTypes = new ObservableCollection<FuelType>(availableFuelTypes);

            SelectedFuelType = availableFuelTypes.FirstOrDefault();
        }

        [RelayCommand]
        public void AddProduct(Product product)
        {
            OrderBuilder.AddItem(new CartItem() { Product = product, Quantity = 1 });

            CurrentOrder = OrderBuilder.Build();
            Subtotal = CurrentOrder.Total;
            Discount = CurrentOrder.BonusSpent;
            CartItems = new ObservableCollection<CartItem>(CurrentOrder.Items);
        }

        private void LoadActivePumps()
        {
            var allPumps = _pumpService.GetAllPumps();

            var filteredPumps = allPumps
                .Where(p => p.Status == PumpStatus.Free)
                .Where(p => _pumpService.GetFuelTypesForPump(p.Id).Any())
                .ToList();

            Pumps.Clear();

            foreach (var p in filteredPumps)
            {
                Pumps.Add(new PumpViewModel(p));
            }

            if (SelectedPump != null && !filteredPumps.Any(p => p.Id == SelectedPump.Id))
            {
                SelectedPump = null;
            }
        }


        private void OnPumpStateChanged(int pumpId, bool isAvailable)
        {
            var targetPump = Pumps.FirstOrDefault(p => p.Model.Id == pumpId);
            if (targetPump != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    targetPump.IsAvailable = isAvailable;
                });
            }
        }




        //FUEL SECTION
        private bool _isRecalculating = false;

        [ObservableProperty]
        private double _fuelAmount;

        [ObservableProperty]
        private double _fuelSum;

        partial void OnFuelAmountChanged(double value)
        {
            if (_isRecalculating) return;
            _isRecalculating = true;

            if (SelectedFuelType != null && SelectedFuelType.Price > 0)
            {
                FuelSum = Math.Round(value * SelectedFuelType.Price, 2);
            }

            _isRecalculating = false;
        }

        partial void OnFuelSumChanged(double value)
        {
            if (_isRecalculating) return;
            _isRecalculating = true;

            if (SelectedFuelType != null && SelectedFuelType.Price > 0)
            {
                FuelAmount = Math.Round(value / SelectedFuelType.Price, 2);
            }

            _isRecalculating = false;
        }

        partial void OnSelectedFuelTypeChanged(FuelType? value)
        {
            if (value != null && FuelAmount > 0)
            {
                _isRecalculating = true;
                FuelSum = Math.Round(FuelAmount * value.Price, 2);
                _isRecalculating = false;
            }

            if (value == null)
            {
                AvailableVolumeText = string.Empty;
                return;
            }

            var totalVolume = SelectedPump.ConnectedTanks
                .Where(t => t.FuelType?.Id == value.Id)
                .Sum(t => t.Volume);

            AvailableVolumeText = $"Available: {totalVolume:F2} L";
        }


        [RelayCommand]
        public void SetPump(Pump pump)
        {
            var pumpVm = Pumps.FirstOrDefault(p => p.Model.Id == pump.Id);
            if (pumpVm == null || !pumpVm.IsAvailable)
            {
                return;
            }

            SelectedPump = pump;

            var availableFuelTypes = _pumpService.GetFuelTypesForPump(pump.Id);
            FuelTypes = new ObservableCollection<FuelType>(availableFuelTypes);

            SelectedFuelType = FuelTypes.FirstOrDefault();

            FuelAmount = 0;
        }

        [RelayCommand]
        public void AddFuelProduct()
        {
            if (SelectedPump == null || SelectedFuelType == null || FuelAmount <= 0)
            {
                MessageBox.Show("Please select a pump, fuel type, and enter a valid amount.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double alreadyInCart = CurrentOrder.Items
                .Where(i => i.Product is Fuel f && f.Type?.Id == SelectedFuelType.Id && f.Pump?.Id == SelectedPump.Id)
                .Sum(i => i.Quantity);

            double totalRequested = FuelAmount + alreadyInCart;

            if (!_pumpService.IsFuelAvailableOnPump(SelectedPump.Id, SelectedFuelType.Id, totalRequested))
            {
                MessageBox.Show(
                    $"Not enough {SelectedFuelType.Name} in tanks connected to {SelectedPump.Name}!\n\n" +
                    $"You are trying to order {totalRequested}L, but the physical remaining volume is less.",
                    "Out of Fuel",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var fuel = new Fuel()
            {
                Name = $"{SelectedFuelType.Name} ({SelectedPump.Name})",
                Type = SelectedFuelType,
                Pump = SelectedPump
            };

            OrderBuilder.AddItem(new CartItem() { Product = fuel, Quantity = FuelAmount });

            CurrentOrder = OrderBuilder.Build();
            Subtotal = CurrentOrder.Total;
            Discount = CurrentOrder.BonusSpent;
            CartItems = new ObservableCollection<CartItem>(CurrentOrder.Items);

            FuelAmount = 0;
            SelectedFuelType = null;
            SelectedPump = null;
        }

        [ObservableProperty]
        private string _searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Products = new ObservableCollection<Product>(_goodsService.GetAllProducts());
            }
            else
            {
                var filteredProducts = _goodsService.GetProductsByPrompt(value);
                Products = new ObservableCollection<Product>(filteredProducts);
            }
        }

        public void Initalize()
        {
            LoadActivePumps();

            Products = new ObservableCollection<Product>(_goodsService.GetAllProducts());
            FuelTypes = new ObservableCollection<FuelType>(_fuelTypeService.GetAllFuelTypes());

            Name = "Hey, " + _userSession.SessionUser?.FullName;
            IsAdmin = _userSession.SessionUser.Role == UserRoles.Admin;

            var tanks = _tanksService.GetAllTanks();

            foreach (var tank in tanks)
            {
                tank.LowVolume += OnTankLowVolume;
            }
        }

        private void OnTankLowVolume(object? sender, EventArgs e)
        {
            if (sender is Tank tank)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"Warning! Low fuel level for {tank.FuelType?.Name} in tank #{tank.Id}.\nCurrent volume: {Math.Round(tank.Volume, 2)} L.",
                        "Critical level alert",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                });
            }
        }

        [ObservableProperty]
        private string _bonusCardNumber;

        partial void OnBonusCardNumberChanged(string value)
        {
            TryBonusCard(value);
        }

        public void TryBonusCard(string cardNumber) 
        {
            BonusCard = _bonusService.BonusCardGetByBarcode(cardNumber);
            if (BonusCard == null) return;
            OrderBuilder.AddBonusCard(BonusCard.Id);
        }

        [RelayCommand]
        public void PumpsGroupBoxMouseLeft()
        {
            SelectedPump = null;
        }

        [RelayCommand]
        public void Checkout()
        {
            if (CurrentOrder == null || CurrentOrder.Items.Count == 0)
            {
                MessageBox.Show("The cart is empty. Please add items before checking out.", "Checkout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int currentUserId = _userSession.SessionUser.Id;

                _orderService.ProcessCheckout(CurrentOrder, currentUserId);

                MessageBox.Show($"Order processed successfully!\nTotal paid: {CurrentOrder.Total:F2}$", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                OrderBuilder = new OrderBuilder(_bonusService);
                CurrentOrder = OrderBuilder.Build();

                CartItems.Clear();
                Subtotal = 0;
                Discount = 0;
                BonusCard = null;

                BonusCardNumber = string.Empty;

                SelectedPump = null;
                SelectedFuelType = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during checkout: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void DecreaseQuantity(CartItem item)
        {
            if (item == null) return;

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                OrderBuilder.RemoveItem(item);
            }

            CurrentOrder = OrderBuilder.Build();
            Subtotal = CurrentOrder.Total;
            Discount = CurrentOrder.BonusSpent;
            CartItems = new ObservableCollection<CartItem>(CurrentOrder.Items);
        }

        [RelayCommand]
        public void RemoveCartItem(CartItem item)
        {
            if (item == null) return;

            OrderBuilder.RemoveItem(item);

            CurrentOrder = OrderBuilder.Build();
            Subtotal = CurrentOrder.Total;
            Discount = CurrentOrder.BonusSpent;
            CartItems = new ObservableCollection<CartItem>(CurrentOrder.Items);
        }


        [ObservableProperty]
        private string _availableVolumeText;

    }
}
