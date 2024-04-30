using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newtonsoft.Json;
using ZEWS.Class;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для ListHotelRoom.xaml
    /// </summary>
    public partial class ListHotelRoom : Page
    {
        private MainWindow mainWindow;

        public ListHotelRoom(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            Loaded += ListHotelRooms_Loaded;
        }

        private void ListHotelRooms_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData(); // Вызываем метод загрузки данных при загрузке страницы
        }

        private async void LoadData()
        {
            try
            {
                string token = Properties.Settings.Default.Token;

                // Создаем HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Добавляем заголовок авторизации с токеном Bearer
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Отправляем GET запрос по указанному URL для загрузки номеров
                    HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/rooms");

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Получаем содержимое ответа в виде строки
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Десериализуем JSON в объект
                        var roomsObject = JObject.Parse(responseBody);

                        // Извлекаем массив комнат из объекта
                        JArray roomsArray = (JArray)roomsObject["rooms"];

                        // Преобразуем каждый элемент массива в объект Room
                        var rooms = roomsArray.Select(room => new Room
                        {
                            id = (int)room["id"],
                            name = (string)room["name"],
                            description = (string)room["description"],
                            price = (decimal)room["price"],
                            type = (string)room["type"]
                            // Добавьте другие свойства, если они есть
                        }).ToList();

                        // Привязываем список комнат к источнику данных ListBox
                        HotelRoomListBox.ItemsSource = rooms;
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

        public class Room
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public decimal price { get; set; }
            public string type { get; set; }
            // Добавьте другие свойства, если они есть
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new HomePage(mainWindow));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddHotelRoom(mainWindow));
        }
    }
}
