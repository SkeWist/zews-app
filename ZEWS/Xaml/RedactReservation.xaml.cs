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
    /// Логика взаимодействия для RedactReservation.xaml
    /// </summary>
    public partial class RedactReservation : Page
    {
        private string accessToken = Properties.Settings.Default.Token;
        private MainWindow mainWindow;
        private readonly HttpClient client = new HttpClient();
        private Reservation reservations;

        // Добавляем поле для хранения бронирования
        private Reservation reservation;

        public RedactReservation(MainWindow mainWindow, Reservation reservation)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.UpdateWindowTitle("Бронирование номера");
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            
            // Сохраняем переданное бронирование в поле
            this.reservation = reservation;

            LoadData();
        }
        private async void LoadData()
        {
            try
            {
                // Получение токена из настроек
                string token = Properties.Settings.Default.Token;

                // Создание HttpClient с использованием токена авторизации
                using (HttpClient client = new HttpClient())
                {
                    // Установка заголовка авторизации с токеном
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Получение данных о пользователях
                    HttpResponseMessage usersResponse = await client.GetAsync(APIconfig.APIurl + "/users");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        string usersResponseBody = await usersResponse.Content.ReadAsStringAsync();
                        var usersJson = JObject.Parse(usersResponseBody)["users"];
                        var users = usersJson.Select(user => new NewUser
                        {
                            id = (long)user["id"],
                            name = (string)user["name"],
                            surname = (string)user["surname"],
                            patronymic = (string)user["patronymic"]
                        }).ToList();
                        if (this.reservation != null && this.reservation.User != null)
                        {
                            usersComboBox.SelectedItem = users.FirstOrDefault(u => u.id == this.reservation.User.id);
                        }
                        // Заполнение ComboBox с пользователями
                        usersComboBox.ItemsSource = users;
                        usersComboBox.DisplayMemberPath = "GetFIO"; // Установка отображаемого свойства
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при получении данных о пользователях: {usersResponse.ReasonPhrase}");
                    }
                    // Получение данных о комнатах
                    HttpResponseMessage roomsResponse = await client.GetAsync(APIconfig.APIurl + "/rooms");
                    if (roomsResponse.IsSuccessStatusCode)
                    {
                        string roomsResponseBody = await roomsResponse.Content.ReadAsStringAsync();
                        var roomsJson = JObject.Parse(roomsResponseBody)["rooms"];
                        var roomNames = roomsJson.Select(room => new HotelRoom
                        {
                            Id = (long)room["id"],
                            Name = (string)room["name"],
                        }).ToList();
                   
                        // Заполнение ComboBox с комнатами
                        roomsComboBox.ItemsSource = roomNames;
                        roomsComboBox.DisplayMemberPath = "Name"; // Установка отображаемого свойства


                        if (this.reservation != null && this.reservation.Room != null)
                        {
                            roomsComboBox.SelectedItem = roomNames.FirstOrDefault(u => u.Id == this.reservation.Room.Id);
                        }

                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при получении данных о комнатах: {roomsResponse.ReasonPhrase}");
                    }
                    // Установка значений в DatePicker (если они уже есть в бронировании)
                    if (this.reservation != null)
                    {
                        entry.SelectedDate = this.reservation.Entry;
                        exit.SelectedDate = this.reservation.Exit;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private async Task UpdateReservationAsync()
        {
            string token = Properties.Settings.Default.Token;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                // Создание объекта, содержащего данные для обновления
                var updatedReservationData = new
                {
                    user_id = ((NewUser)usersComboBox.SelectedItem).id,
                    room_id = ((HotelRoom)roomsComboBox.SelectedItem).Id,
                    date_entry = entry.SelectedDate,
                    date_exit = exit.SelectedDate
                };

                // Сериализация данных для обновления в формат JSON
                var json = JsonConvert.SerializeObject(updatedReservationData);
                MessageBox.Show(json);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                // Отправка PUT-запроса на сервер для обновления данных
                HttpResponseMessage response = await client.PostAsync(APIconfig.APIurl + $"/reservations/{reservation.Id}", data);
                // Проверка успешности выполнения запроса
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные успешно обновлены.");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при изменении пользователя: {response.StatusCode} - {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateReservationAsync();
        }
    }
}

