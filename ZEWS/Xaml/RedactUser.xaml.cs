using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZEWS.Xaml
{
    /// <summary>
    /// Логика взаимодействия для RedactUser.xaml
    /// </summary>
    public partial class RedactUser : Page
    {
        private string accessToken = Properties.Settings.Default.Token;

        private MainWindow mainWindow;
        public RedactUser()
        {
            InitializeComponent();
            InitializeComponent();
            this.mainWindow = new MainWindow();
            mainWindow.UpdateWindowTitle("Редактирование пользователя");
            mainWindow.Height = 450;
            mainWindow.Width = 1100;
        }
    }
}
