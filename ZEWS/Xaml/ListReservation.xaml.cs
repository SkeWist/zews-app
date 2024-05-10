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
            mainWindow.UpdateWindowTitle("Список бронирований");
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
        private async void LoadReservations()
        {
            try
            {
                string token = Properties.Settings.Default.Token;

                // Создаем HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Добавляем заголовок авторизации с токеном Bearer
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Отправляем GET запрос по указанному URL для загрузки бронирований
                    HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/reservations");

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Получаем содержимое ответа в виде строки
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Десериализуем JSON в массив объектов JArray
                        JObject jsonResponse = JObject.Parse(responseBody);
                        JArray reservationsArray = (JArray)jsonResponse["reservations"];

                        // Десериализуем JSON-массив в список объектов Reservation
                        List<Reservation> reservations = reservationsArray.Select(r => r.ToObject<Reservation>()).ToList();

                        // Здесь можно выполнить дополнительные операции с загруженными бронированиями, если необходимо

                        // Привязываем список reservations к источнику данных ListBox
                        reservationsListBox.ItemsSource = reservations;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при получении данных: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
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
        private void EditRoom_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                long roomId = Convert.ToInt32(button.Tag);
                RedactHotelRoom redactHotelRoom = new RedactHotelRoom(roomId, mainWindow);
                NavigationService.Navigate(redactHotelRoom);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddReservation(mainWindow));
        }

        private void EditReservation_click(object sender, MouseButtonEventArgs e)
        {
                // Получаем выбранное бронирование из списка
                Reservation selectedReservation = reservationsListBox.SelectedItem as Reservation;
                // Передаем выбранное бронирование в метод Navigate для редактирования
                FrameManager.MainFrame.Navigate(new RedactReservation(mainWindow, selectedReservation));
            Image image = sender as Image;
            if (image != null)
            {
                Reservation reservation = (Reservation)(image.Tag);
                RedactReservation redactReservation = new RedactReservation(mainWindow, reservation);
                NavigationService.Navigate(redactReservation);
                MessageBox.Show(reservation.Id.ToString());
            }
            

        }

        private async void DeleteReservation_click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Получаем выбранное бронирование из списка
                Reservation selectedReservation = reservationsListBox.SelectedItem as Reservation;

                if (selectedReservation == null)
                {
                    MessageBox.Show("Пожалуйста, выберите бронирование для удаления.");
                    return;
                }

                // Создаем HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Добавляем заголовок авторизации с токеном Bearer
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Отправляем DELETE запрос по указанному URL для удаления бронирования
                    HttpResponseMessage response = await client.DeleteAsync($"{APIconfig.APIurl}/reservations/{selectedReservation.Id}");

                    // Проверяем успешность выполнения запроса
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Бронирование успешно удалено.");
                        // Перезагружаем список бронирований после удаления
                        await LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при удалении бронирования: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
