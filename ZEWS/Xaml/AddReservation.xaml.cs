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
    /// Логика взаимодействия для AddReservation.xaml
    /// </summary>
    public partial class AddReservation : Page
    {
        private string apiUrl = APIconfig.APIurl + "/reservations";
        private string accessToken = Properties.Settings.Default.Token;
        private MainWindow mainWindow;
        private readonly HttpClient client = new HttpClient();
        public AddReservation(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.UpdateWindowTitle("Бронирование номера");
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            FillUsersComboBox();
            FillRoomsComboBox();
        }
        private async void FillUsersComboBox()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/users");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                // Преобразование JSON в список объектов NewUser с нужными полями
                var usersJson = JObject.Parse(responseBody)["users"];
                var users = usersJson.Select(user => new NewUser
                {
                    id = Convert.ToInt64(user["id"]),                    
                    surname = user["surname"].ToString(),
                    name = user["name"].ToString(),
                    patronymic = user["patronymic"].ToString()
                }).ToList();

                // Установка списка пользователей в ItemsSource для комбобокса
                usersComboBox.ItemsSource = users;
                usersComboBox.DisplayMemberPath = "GetFIO"; // Установка свойства для отображения в комбобоксе
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }

        private async void FillRoomsComboBox()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/rooms");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                // Получение списка имен комнат из JSON
                var roomsJson = JObject.Parse(responseBody)["rooms"];
                var rooms = roomsJson.Select(room => new HotelRoom
                {
                    Id = Convert.ToInt64(room["id"]),
                    Name = room["name"].ToString(),
                }).ToList();

                // Установка списка имен комнат в ItemsSource для комбобокса
                roomsComboBox.ItemsSource = rooms;
                roomsComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }

        }
        private async void SendAllDataToAPI()
        {
            try
            {
                // Получаем выбранную дату въезда
                DateTime entryDate = entry.SelectedDate ?? DateTime.Today;
                string entryIsoDate = entryDate.ToString("yyyy-MM-dd");

                // Получаем выбранную дату выезда
                DateTime exitDate = exit.SelectedDate ?? DateTime.Today.AddDays(1); // По умолчанию устанавливаем дату следующего дня
                string exitIsoDate = exitDate.ToString("yyyy-MM-dd");

                // Получаем выбранного пользователя
                NewUser selectedUser = (NewUser)usersComboBox.SelectedItem;

                // Получаем выбранную комнату
                HotelRoom selectedRoom = (HotelRoom)roomsComboBox.SelectedItem;

                // Создаем JSON объект для отправки в API
                var requestData = new
                {
                    date_entry = entryIsoDate,
                    date_exit = exitIsoDate,
                    user_id = selectedUser.id, // Предположим, что в вашем классе NewUser есть свойство Id
                    room_id = selectedRoom.Id
                };

                // Преобразуем объект requestData в JSON строку
                string jsonRequest = JsonConvert.SerializeObject(requestData);
                MessageBox.Show($"Sending {jsonRequest}");
                // Определяем URL вашего API, куда нужно отправить данные
                string apiUrl =APIconfig.APIurl + "/reservations";

                // Создаем HTTP контент из JSON строки
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                MessageBox.Show(content.ToString());
                // Отправляем POST запрос на ваш API
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Проверяем, что ответ от API успешный
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные успешно отправлены!");
                }
                else
                {
                    // Если ответ не успешный, показываем сообщение об ошибке
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка при отправке данных: " + responseContent);
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка при отправке запроса, показываем сообщение об ошибке
                MessageBox.Show("Ошибка при отправке данных: " + ex.Message);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SendAllDataToAPI();
            FrameManager.MainFrame.GoBack();
        }
    }
}
