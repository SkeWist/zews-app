using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Windows.Controls;
using ZEWS.Class;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для RedactHotelRoom.xaml
    /// </summary>
    public partial class RedactHotelRoom : Page
    {
        private HttpClient client;
        private string accessToken = Properties.Settings.Default.Token;
        private HotelRoom currentHotelRoom;
        private MainWindow mainWindow;
        private long roomId;
        public RedactHotelRoom(long id, MainWindow mainWindow )
        {
            roomId = id;
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.client = new HttpClient();
            FillRoomTypesComboBox();
            mainWindow.UpdateWindowTitle("Редактирование номера");
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            LoadData();
        }
        private async void FillRoomTypesComboBox()
        {
            try
            {
                // Отправить запрос к API для получения списка типов номеров
                HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/rooms/types");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JObject.Parse(responseBody);
                var roomTypesObject = (JObject)responseObject["roomTypes"];
                var roomTypesList = new List<RoomType>();
                foreach (var roomType in roomTypesObject.Properties())
                {
                    roomTypesList.Add(new RoomType(Convert.ToInt64(roomType.Name), roomType.Value.ToString()));
                }

                // Присваиваем значения roomTypes свойству ItemsSource ComboBox
                type.ItemsSource = roomTypesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }
        private async void LoadData()
        {
            string token = Properties.Settings.Default.Token;
            //try
            //{
                // Создание HttpClient
                using (HttpClient client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Формирование данных для отправки на сервер
                    string apiUrl = APIconfig.APIurl;
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(currentHotelRoom);


                    // Отправка PUT запроса на сервер для обновления информации о номере
                    HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + $"/rooms/{roomId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var room = JObject.Parse(responseBody);
                        currentHotelRoom = new HotelRoom (){
                            Id = (long)room["id"], 
                            Name = (string)room["name"], 
                            Type = (string)room["type"], 
                            Description = (string)room["description"],
                            Price = (double)room["price"],
                            Photos = HotelRoom.AddPhotos((JObject)room["photos"])    
                        };
                        MessageBox.Show(currentHotelRoom.Id.ToString());
                        FillForm();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при обновлении информации о пользователе: {response.ReasonPhrase}");
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка: {ex.Message}");
            //}
        }
        private void AddThumbnailImage(string imagePath)
        {
            // Создаем новую картинку для каждой фотографии и добавляем ее в StackPanel
            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            image.Source = bitmapImage;
            image.Width = 60;
            image.Height = 60;
            image.Margin = new Thickness(5);
            image.Style = FindResource("ThumbnailImageStyle") as Style;

            // Добавляем обработчик события для каждой миниатюрной фотографии
            image.MouseDown += ThumbnailImage_MouseDown;

            stackPanelImages.Children.Add(image);
        }
        private void ThumbnailImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Получаем источник изображения, на которое нажали
            Image clickedImage = sender as Image;

            // Устанавливаем источник изображения img_1 равным изображению, на которое нажали
            img_1.Source = clickedImage.Source;

            // Показываем кнопку удаления на img_1
            deleteButton.Visibility = Visibility.Visible;
        }
        private void FillForm()
        {
            string apiUrl = APIconfig.APIurl;

            name.Text = currentHotelRoom.Name.ToString();
            type.SelectedValue = currentHotelRoom?.Type.ToString();
            price.Text = currentHotelRoom?.Price.ToString();
            description.Text = currentHotelRoom.Description;
            foreach (var photo in currentHotelRoom.Photos)
            {
                AddThumbnailImage(APIconfig.APIstorage + "/" + photo.Name);
            }
            // Создаем новый объект BitmapImage с URL-адресом изображения


            if (currentHotelRoom?.Photos != null && currentHotelRoom?.Photos?.Count > 0)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(currentHotelRoom.Photos[0].URL));
                // Устанавливаем BitmapImage как источник изображения для элемента Image
                img_1.Source = bitmapImage;
            }

        }

        private async void UpdateRoomAsync()
        {

            string token = Properties.Settings.Default.Token;
            //try

            // Создание HttpClient
            using (HttpClient client = new HttpClient())
            {
              
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                 multiContent.Add(new StringContent(name.Text), "name");
                 multiContent.Add(new StringContent(((RoomType)type.SelectedValue).Id.ToString()), "type_id");
                 multiContent.Add(new StringContent(price.Text), "price");
                 multiContent.Add(new StringContent(description.Text), "description");
                 multiContent.Add(new StringContent(price.Text), "price");




                foreach (var content in multiContent)
                {
                    // Проверяем, является ли текущий элемент StringContent
                    if (content is StringContent stringContent)
                    {
                        // Выводим имя и значение StringContent
                        MessageBox.Show($"Name: {content.Headers.ContentDisposition.Name}, Value: {stringContent.ReadAsStringAsync().Result}");
                    }
                    else
                    {
                        // Если это не StringContent, выводим только имя
                        MessageBox.Show($"Name: {content.Headers.ContentDisposition.Name}, Value: [Non-StringContent]");
                    }
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Формирование данных для отправки на сервер
                string apiUrl = APIconfig.APIurl;


                // Отправка POST запроса на сервер для обновления информации о пользователе
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/users/{currentHotelRoom.Id}", multiContent);

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Информация о пользователе успешно обновлена.");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при изменении пользователя: {response.StatusCode} - {errorMessage}");
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка: {ex.Message}");
            //}
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
