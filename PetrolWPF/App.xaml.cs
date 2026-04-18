using gsst.Interfaces;
using gsst.Model.OrderProcessors;
using gsst.Model.User;
using gsst.Services;
using Gsstwpfmock.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Gsstwpfmock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

      public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<AppDbContext>(provider =>
                        new AppDbContext(SettingsService.Settings.ConnectionString));
                    services.AddSingleton<SettingsService>();
                    services.AddSingleton<IUserService, UserService>();
                    services.AddScoped<IAuthService,AuthService>(); 
                    services.AddSingleton<IStatisticsService,StatisticsService>();
                    services.AddScoped<IOrderService, OrderService>();
                    services.AddScoped<IBonusService, BonusService>();

                    services.AddScoped<IOrderProcessor, FuelOrderProcessor>();

                    services.AddScoped<IGoodsService, GoodsService>();
                    services.AddScoped<IFuelTypeService, FuelTypeService>();
                    services.AddScoped<ITanksService, TanksService>();
                    services.AddScoped<IPumpService, PumpService>();

                    //----------------------------------------

                    services.AddSingleton<UserSession>();

                    //----------------------------------------

                    services.AddTransient<BonusCardsManagementViewModel>();
                    services.AddTransient<PetrolWPF.View.BonusCardsManagementWindow>();
                    services.AddScoped<IReportService, ReportService>();




                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                    
                    services.AddTransient<AuthWindow>();

                    services.AddTransient<AdminDashboardViewModel>();
                    services.AddTransient<AdminDashboardWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    var stuckPumps = dbContext.Pumps
                        .Where(p => p.Status == gsst.Model.FuelStuff.PumpStatus.Busy)
                        .ToList();

                    if (stuckPumps.Any())
                    {
                        foreach (var pump in stuckPumps)
                        {
                            pump.Status = gsst.Model.FuelStuff.PumpStatus.Free;
                        }

                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error while resetting stuck pumps: {ex.Message}");
                }
            }

            var authWindow = _host.Services.GetRequiredService<AuthWindow>();
            authWindow.Show();

            base.OnStartup(e);
        }
    }
}
