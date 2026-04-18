using gsst.Interfaces;
using gsst.Model.User;
using gsst.Services;
using Gsstwpfmock.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gsstwpfmock
{
    public partial class AuthWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuthService _authService;
        private readonly UserSession _userSession;

        public AuthWindow(IServiceProvider serviceProvider, IAuthService authService, UserSession userSession)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _authService = authService;
            _userSession = userSession;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = _authService.Login(LoginBox.Text, PasswordBox.Password);

                if (user == null) return;
                else
                {
                    _userSession.SessionUser = user;

                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("An unexpected error occurred. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}
