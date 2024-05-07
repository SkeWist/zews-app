using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using ZEWS.Class;

namespace ZEWS.Xaml
{
    /// <summary>
    /// Логика взаимодействия для ListReservation.xaml
    /// </summary>
    public partial class ListReservation : Page
    {
        private string accessToken = Properties.Settings.Default.Token;
        private readonly HttpClient client;
        private MainWindow mainWindow;
        private NewUser currentUser;
        private long userId;
        public ListReservation(MainWindow mainWindow)
        {
            InitializeComponent();
            client = new HttpClient();
            this.mainWindow = mainWindow;
            LoadDataAsync();
        }
        public async Task<List<Reservation>> GetReservationsAsync(string apiUrl)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var reservationResponse = JsonConvert.DeserializeObject<ReservationResponse>(json);
                return reservationResponse.Reservations;
            }
            else
            {
                throw new Exception($"Failed to get reservations. Status code: {response.StatusCode}");
            }
        }
        public async Task LoadDataAsync()
        {
            try
            {
                var reservations = await GetReservationsAsync(APIconfig.APIurl + "/reservations");
                if (reservations != null)
                {
                    // Установка данных в качестве контекста данных для страницы
                    DataContext = reservations;
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        public class ReservationResponse
        {
            public List<Reservation> Reservations { get; set; }
        }
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                long userId = Convert.ToInt32(button.Tag);
                RedactUser redactUser = new RedactUser(userId, mainWindow);
                NavigationService.Navigate(redactUser);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
    }
}
