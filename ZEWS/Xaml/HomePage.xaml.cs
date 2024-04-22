using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using ZEWS.Xaml;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private MainWindow mainWindow;
        public HomePage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            FIO.Text = $"Здравствуйте, {Properties.Settings.Default.Surname} {Properties.Settings.Default.Name} {Properties.Settings.Default.Patronymic}";
            mainWindow.UpdateWindowTitle("Главная cтраница");
            mainWindow.Height = 550;
            mainWindow.Width = 800;
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AutorizationPage autorizationPage = new AutorizationPage(mainWindow);
            mainWindow.Mainframe.Navigate(autorizationPage);
        }
    }
}
