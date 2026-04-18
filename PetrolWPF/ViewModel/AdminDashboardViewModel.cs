using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using gsst.Interfaces;
using gsst.Model;
using gsst.Model.FuelStuff;
using gsst.Model.User;
using gsst.Services;
using Microsoft.Win32;
using PetrolWPF.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Gsstwpfmock.ViewModel
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserService _usersService;
        public SettingsService _settingsService;
        private readonly ITanksService _tanksService;
        private readonly IPumpService _pumpService;
        private readonly IGoodsService _goodsService;
        private readonly IFuelTypeService _fuelTypeService;
        private readonly IReportService _reportService;
        private readonly IOrderService _orderService;

        [ObservableProperty]
        private DateTime _reportStartDate = DateTime.Now.Date;

        [ObservableProperty]
        private DateTime _reportEndDate = DateTime.Now.Date;


        [ObservableProperty]
        private ObservableCollection<string> _mostPopularProducts;

        [ObservableProperty]
        private double _totalIncome;

        [ObservableProperty]
        private double _averageIncome;

        [ObservableProperty]
        private ObservableCollection<Pump> _pumps;

        [ObservableProperty]
        private ObservableCollection<Tank> _tanks;

        [ObservableProperty]
        private ObservableCollection<FuelType> _fuelTypes;

        [ObservableProperty]
        private ObservableCollection<User> _users;

        [ObservableProperty]
        private ObservableCollection<string> _mostPopularPumps;

        [ObservableProperty]
        private double _totalFuelIncome;

        [ObservableProperty]
        private double _averageFuelIncome;

        [ObservableProperty]
        private bool _isSaveButtonEnabled;



        [ObservableProperty]
        private ObservableCollection<Good> _products;



        [ObservableProperty]
        private SettingsModel _settingsCopy;

        public string DatabaseName
        {
            get
            {
                var connectionString = SettingsCopy.ConnectionString;
                var dbName = connectionString.Replace("Data Source=", "").Replace(".db", "");
                return dbName;
            }
            set
            {
                if (SettingsCopy != null)
                {
                    SettingsCopy.ConnectionString = "Data Source=" + value + ".db";
                    SaveButtonEnabled();
                }
            }
        }

        [RelayCommand]
        public void SaveSettings()
        { 
            bool isDatabseNameChanged = SettingsCopy.ConnectionString != SettingsService.Settings.ConnectionString;

            _settingsService.SaveSettings(SettingsCopy);


            if (isDatabseNameChanged)
            {
                MessageBox.Show("Database name changed. Please restart the application.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool SaveButtonEnabled()
        {
            if (SettingsCopy != null)
            {
                if (SettingsCopy.ConnectionString != SettingsService.Settings.ConnectionString || SettingsCopy.BonusRate != SettingsService.Settings.BonusRate)
                {
                    IsSaveButtonEnabled = true;
                    return true;
                }
                else
                {
                    IsSaveButtonEnabled = false;
                    return false;
                }
            }
            return false;
        }

        public AdminDashboardViewModel(IServiceProvider serviceProvider, IStatisticsService statisticsService, IUserService usersService, SettingsService settingsService, ITanksService tanksService, IPumpService pumpService, IGoodsService goodsService, IFuelTypeService fuelTypeService, IReportService reportService, IOrderService orderService)
        {
            _serviceProvider = serviceProvider;
            _statisticsService = statisticsService;
            _usersService = usersService;
            _settingsService = settingsService;
            _tanksService = tanksService;
            _pumpService = pumpService;
            _goodsService = goodsService;
            _fuelTypeService = fuelTypeService;
            _reportService = reportService;
            _orderService = orderService;

            LoadUsers();
            UpdateData();
        }

        private void UpdateData()
        {
            MostPopularPumps = new ObservableCollection<string>(_statisticsService.GetMostPopularPumps(3));
            AverageFuelIncome = _statisticsService.AverageFuelSales();
            TotalFuelIncome = _statisticsService.TotalFuelSales();

            MostPopularProducts = new ObservableCollection<string>(_statisticsService.GetMostPopularProducts(3));
            TotalIncome = _statisticsService.CalcucateTotalSales();
            AverageIncome = _statisticsService.AveraageMoneySpent();

            Products = new ObservableCollection<Good>(_goodsService.GetAllProducts());
            Pumps = new ObservableCollection<Pump>(_pumpService.GetAllPumps());
            Tanks = new ObservableCollection<Tank>(_tanksService.GetAllTanks());
            FuelTypes = new ObservableCollection<FuelType>(_fuelTypeService.GetAllFuelTypes());

            SettingsCopy = (SettingsModel)SettingsService.Settings.Clone();

            AllOrders = new ObservableCollection<Order>(_orderService.GetOrderHistory());
        }

        public void LoadUsers()
        {
            Users = new ObservableCollection<User>(_usersService.GetAllUsers());
        }


        [RelayCommand]
        public void GenerateFinancialReport()
        {
            try
            {
                var reportData = _reportService.GenerateZReport(ReportStartDate, ReportEndDate);

                if (reportData.OrdersCount == 0)
                {
                    MessageBox.Show("No transactions found for the selected period.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text Report (*.txt)|*.txt|JSON Data (*.json)|*.json",
                    FileName = $"Z-Report_{DateTime.Now:yyyyMMdd}",
                    Title = "Export Financial Report"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string ext = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                    if (ext == ".json")
                        _reportService.ExportToJson(reportData, saveFileDialog.FileName);
                    else
                        _reportService.ExportToTxt(reportData, saveFileDialog.FileName);

                    MessageBox.Show("Report exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [ObservableProperty]
        private ObservableCollection<Order> _allOrders; 

        [ObservableProperty]
        private Order _selectedOrder; 


        [ObservableProperty]
        private Good _selectedGood;

        [RelayCommand]
        public void AddGood()
        {
            var newGood = new Good();
            var window = new PetrolWPF.View.ProductEditWindow(newGood); 

            if (window.ShowDialog() == true)
            {
                try
                {
                    _goodsService.AddProduct(
                        window.CurrentProduct.Name,
                        window.CurrentProduct.Price,
                        window.CurrentProduct.BarCode,
                        window.CurrentProduct.Image
                    );

                    Products = new ObservableCollection<Good>(_goodsService.GetAllProducts());

                    WeakReferenceMessenger.Default.Send(new ProductsChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while adding new good: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void EditGood()
        {
            if (SelectedGood == null)
            {
                MessageBox.Show("Please select a good to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PetrolWPF.View.ProductEditWindow(SelectedGood);

            if (window.ShowDialog() == true)
            {
                try
                {
                    _goodsService.UpdateProduct(
                        window.CurrentProduct.Id,
                        window.CurrentProduct.Name,
                        window.CurrentProduct.Price,
                        window.CurrentProduct.BarCode,
                        window.CurrentProduct.Image
                    );

                    Products = new ObservableCollection<Good>(_goodsService.GetAllProducts());

                    WeakReferenceMessenger.Default.Send(new ProductsChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while editing good: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void DeleteGood()
        {
            if (SelectedGood == null)
            {
                MessageBox.Show("Please select a good to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedGood.Name}'?",
                                         "Delete Good",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _goodsService.DeleteProduct(SelectedGood.Id);

                    Products.Remove(SelectedGood);

                    WeakReferenceMessenger.Default.Send(new ProductsChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //USER SERCTION

        [ObservableProperty]
        private User _selectedUser;

        [RelayCommand]
        public void AddUser()
        {
            var newUser = new User() { Role = UserRoles.Cashier };

            var window = new PetrolWPF.View.UserEditWindow(newUser);

            if (window.ShowDialog() == true)
            {
                try
                {
                    _usersService.CreateUser(
                        window.CurrentUser.FullName,
                        window.CurrentUser.Username,
                        window.CurrentUser.Password ?? "",
                        window.CurrentUser.Role
                    );

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void EditUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user to edit first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PetrolWPF.View.UserEditWindow(SelectedUser);

            if (window.ShowDialog() == true)
            {
                try
                {
                    _usersService.UpdateUser(
                        window.CurrentUser.Id,
                        window.CurrentUser.FullName,
                        window.CurrentUser.Username,
                        window.CurrentUser.Password, 
                        window.CurrentUser.Role
                    );

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void DeleteUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedUser.FullName}'?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _usersService.DeleteUser(SelectedUser.Id);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        //stoks section
        [ObservableProperty]
        private FuelType _selectedFuelType;

        // --- CRUD Fuel Types ---

        [RelayCommand]
        public void AddFuelType()
        {
            var newFuelType = new FuelType();
            var window = new PetrolWPF.View.FuelTypeEditWindow(newFuelType); 

            if (window.ShowDialog() == true)
            {
                try
                {
                    _fuelTypeService.AddFuelType(window.CurrentFuelType.Name, window.CurrentFuelType.Price);
                    FuelTypes = new ObservableCollection<FuelType>(_fuelTypeService.GetAllFuelTypes());

                    WeakReferenceMessenger.Default.Send(new FuelTypesChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while adding fuel type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void EditFuelType()
        {
            if (SelectedFuelType == null)
            {
                MessageBox.Show("Please select a fuel type to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PetrolWPF.View.FuelTypeEditWindow(SelectedFuelType);
            if (window.ShowDialog() == true)
            {
                try
                {
                    _fuelTypeService.UpdateFuelType(window.CurrentFuelType.Id, window.CurrentFuelType.Name, window.CurrentFuelType.Price);
                    FuelTypes = new ObservableCollection<FuelType>(_fuelTypeService.GetAllFuelTypes());

                    WeakReferenceMessenger.Default.Send(new FuelTypesChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while editing fuel type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void DeleteFuelType()
        {
            if (SelectedFuelType == null)
            {
                MessageBox.Show("Please select a fuel type to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedFuelType.Name}'?",
                                         "Delete Fuel Type",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _fuelTypeService.DeleteFuelType(SelectedFuelType.Id);
                    FuelTypes.Remove(SelectedFuelType);

                    WeakReferenceMessenger.Default.Send(new FuelTypesChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [ObservableProperty]
        private Pump _selectedPump;


        [RelayCommand]
        public void AddPump()
        {
            var window = new PetrolWPF.View.PumpEditWindow(new Pump(), Tanks);
            if (window.ShowDialog() == true)
            {
                try
                {
                    _pumpService.AddPump(window.CurrentPump.Name, window.CurrentPump.ConnectedTanks);
                    Pumps = new ObservableCollection<Pump>(_pumpService.GetAllPumps());
                    WeakReferenceMessenger.Default.Send(new PumpsChangedMessage());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        [RelayCommand]
        public void EditPump()
        {
            if (SelectedPump == null) return;

            var window = new PetrolWPF.View.PumpEditWindow(SelectedPump, Tanks);
            if (window.ShowDialog() == true)
            {
                try
                {
                    _pumpService.UpdatePump(window.CurrentPump.Id, window.CurrentPump.Name, window.CurrentPump.ConnectedTanks);

                    if (SelectedPump.Status != window.CurrentPump.Status)
                    {
                        _pumpService.ChangePumpStatus(window.CurrentPump.Id, window.CurrentPump.Status);
                    }

                    Pumps = new ObservableCollection<Pump>(_pumpService.GetAllPumps());
                    WeakReferenceMessenger.Default.Send(new PumpsChangedMessage());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        [RelayCommand]
        public void DeletePump()
        {
            if (SelectedPump == null)
            {
                MessageBox.Show("Please select a pump to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedPump.Name}'?",
                                         "Delete Pump",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _pumpService.DeletePump(SelectedPump.Id);
                    Pumps.Remove(SelectedPump);

                    WeakReferenceMessenger.Default.Send(new PumpsChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [ObservableProperty]
        private Tank _selectedTank;

        [RelayCommand]
        public void AddTank()
        {
            var window = new PetrolWPF.View.TankEditWindow(new Tank(), FuelTypes);
            if (window.ShowDialog() == true)
            {
                try
                {
                    _tanksService.AddTank(window.CurrentTank.FuelType, window.CurrentTank.Capacity);
                    Tanks = new ObservableCollection<Tank>(_tanksService.GetAllTanks());

                    WeakReferenceMessenger.Default.Send(new TanksChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding tank: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void EditTank()
        {
            if (SelectedTank == null)
            {
                MessageBox.Show("Please select a tank to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PetrolWPF.View.TankEditWindow(SelectedTank, FuelTypes);
            if (window.ShowDialog() == true)
            {
                try
                {
                    if (SelectedTank.FuelType?.Id != window.CurrentTank.FuelType?.Id)
                    {
                        _tanksService.ChangeTankFuelType(window.CurrentTank.Id, window.CurrentTank.FuelType);
                        window.CurrentTank.Volume = 0;
                    }

                    _tanksService.UpdateTank(window.CurrentTank.Id, window.CurrentTank.Capacity, window.CurrentTank.Volume);

                    Tanks = new ObservableCollection<Tank>(_tanksService.GetAllTanks());
                    WeakReferenceMessenger.Default.Send(new TanksChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error editing tank: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        public void DeleteTank()
        {
            if (SelectedTank == null) return;

            if (MessageBox.Show($"Are you sure you want to delete Tank #{SelectedTank.Id}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _tanksService.DeleteTank(SelectedTank.Id);
                    Tanks.Remove(SelectedTank);

                    WeakReferenceMessenger.Default.Send(new TanksChangedMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
