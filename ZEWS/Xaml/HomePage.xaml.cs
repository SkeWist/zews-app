using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private async Task SendTokens()
        {
            // Создаем объект для хранения данных аутентификации
            var credentials = new
            {
                phone = Properties.Settings.Default.Phone,
                password = Properties.Settings.Default.Password // Предполагая, что у вас есть сохраненный пароль
            };

            // Сериализуем объект в JSON
            string json = JsonConvert.SerializeObject(credentials);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(APIconfig.APIurl + "/auth/logout",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SendTokens();
            AutorizationPage autorizationPage = new AutorizationPage(mainWindow);
            mainWindow.Mainframe.Navigate(autorizationPage);
        }

        private void Button_Click_ListUsers(object sender, RoutedEventArgs e)
        {
            mainWindow.Mainframe.Navigate(new ListUsers(mainWindow));
        }

    }
}
