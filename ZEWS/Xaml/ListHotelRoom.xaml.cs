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
using System.Windows.Input;
using ZEWS.Xaml;

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
            HotelRoomListBox.MouseDoubleClick += HotelRoomListBox_MouseDoubleClick;
        }

        private void HotelRoomListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Получаем выбранный элемент ListBox
            HotelRoom selectedHotelRoom = (HotelRoom)HotelRoomListBox.SelectedItem;

            // Переходим к странице редактирования пользователя и передаем выбранного пользователя в конструктор
            FrameManager.MainFrame.Navigate(new RedactHotelRoom(selectedHotelRoom.Id, mainWindow));
        }

        private void ListHotelRooms_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData(); // Вызываем метод загрузки данных при загрузке страницы
        }

        private async void LoadData()
        {
            //try
            //{
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
                        //var photoObject = (JObject);
                        //var photoList = new List<Photo>();
                        //foreach (var roomType in roomTypesObject.Properties())
                        //{
                            //photoList.Add(new Photo(Convert.ToInt64(roomType.Name), roomType.Value.ToString()));
                        //}                        // Преобразуем каждый элемент массива в объект Room
                        var rooms = roomsArray.Select(room => new HotelRoom
                        {

                            Id = (int)room["id"],
                            Name = (string)room["name"],
                            Description = (string)room["description"],
                            Price = (double)room["price"],
                            Type = (string)room["type"],
                            Photos = HotelRoom.AddPhotos((JObject)(room["photos"]))
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
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Ошибка: " + ex.Message);
            //}
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new HomePage(mainWindow));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddHotelRoom(mainWindow));
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем объект комнаты, связанный с нажатой кнопкой "Удалить"
            var button = sender as Button;
            var room = button.DataContext as HotelRoom;

            try
            {
                string token = Properties.Settings.Default.Token;

                // Создаем HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Добавляем заголовок авторизации с токеном Bearer
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Отправляем DELETE запрос по URL для удаления комнаты с определенным id
                    HttpResponseMessage response = await client.DeleteAsync(APIconfig.APIurl + $"/rooms/{room.Id}");

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Успешно удалено, можно обновить список комнат
                        MessageBox.Show("Комната успешно удалена!");
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
