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
using System.IO;
using Microsoft.Win32;
using System.Linq;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для RedactHotelRoom.xaml
    /// </summary>
    public partial class RedactHotelRoom : Page
    {
        private List<string> apiPhotoURLs = new List<string>(); // Список URL-адресов фотографий из API
        private HttpClient client;
        private string accessToken = Properties.Settings.Default.Token;
        private HotelRoom currentHotelRoom;
        private MainWindow mainWindow;
        private long roomId;
        private List<long> rPhoto = new List<long>();
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

                    if (currentHotelRoom?.Photos != null && currentHotelRoom?.Photos?.Count > 0)
                    {
                    foreach (var photo in currentHotelRoom.Photos)
                        {
                        // Добавляем URL-адреса фотографий из API в список
                        apiPhotoURLs.Add(APIconfig.APIstorage + "/" + photo.Name);
                        }
                    }

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

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    AddThumbnailImage(filename);
                }
            }
        }
        private void AddThumbnailImage(string imagePath, long? id = null)
        {
            // Создаем новую картинку для каждой фотографии и добавляем ее в StackPanel
            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            image.Source = bitmapImage;
            image.Width = 60;
            image.Height = 60;
            image.Margin = new Thickness(5);
            image.Style = FindResource("ThumbnailImageStyle") as Style;
            if(id != null)
            {
                image.Tag = id;
            }

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
            img_1.Tag = clickedImage.Tag;
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
                AddThumbnailImage(APIconfig.APIstorage + "/" + photo.Name, photo.Id);

            }
            // Создаем новый объект BitmapImage с URL-адресом изображения


            if (currentHotelRoom?.Photos != null && currentHotelRoom?.Photos?.Count > 0)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(currentHotelRoom.Photos[0].URL));
                // Устанавливаем BitmapImage как источник изображения для элемента Image
                img_1.Source = bitmapImage;
                img_1.Tag = currentHotelRoom.Photos[0]?.Id;
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
                 if(currentHotelRoom.Name != name.Text)multiContent.Add(new StringContent(name.Text), "name");
                 multiContent.Add(new StringContent(((RoomType)type.SelectedItem).Id.ToString()), "type_id");
                 multiContent.Add(new StringContent(price.Text), "price");
                 multiContent.Add(new StringContent(description.Text), "description");
                 foreach (var child in rPhoto)
                 {
                    multiContent.Add(new StringContent(child.ToString()), "removePhotos[]");
                    
                 }
                
                foreach (var child in stackPanelImages.Children)
                {
                    if (child is Image image)
                    {
                        BitmapImage bitmapImage = image.Source as BitmapImage;
                        if (bitmapImage != null)
                        {
                            // Получаем путь к файлу изображения
                            string imagePath = bitmapImage.UriSource.LocalPath;
                            if (!File.Exists(imagePath))
                            {
                                continue;
                            }
                            // Читаем фотографию как массив байтов
                            byte[] photoBytes = File.ReadAllBytes(imagePath);

                            // Создаем содержимое для фотографии
                            ByteArrayContent photoContent = new ByteArrayContent(photoBytes);
                            photoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            multiContent.Add(photoContent, "photos[]", Path.GetFileName(imagePath));
                            // Добавляем содержимое фотографии к другим данным формы

                            // Ваш код для добавления содержимого фотографии к данным формы
                        }
                    }
                }
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
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/rooms/{currentHotelRoom.Id}", multiContent);

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Информация о номере успешно обновлена.");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при изменении комнаты: {response.StatusCode} - {errorMessage}");
                }
            }


            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка: {ex.Message}");
            //}
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем URI источника изображения img_1
            Uri deletedImageUri = ((BitmapImage)img_1.Source)?.UriSource;

            // Получаем имя файла из URI источника изображения
            string imageName = Path.GetFileName(deletedImageUri.LocalPath);

            // Устанавливаем источник изображения img_1 равным изображению NoPhoto.png
            img_1.Source = new BitmapImage(new Uri("/Img/NoPhoto.png", UriKind.RelativeOrAbsolute));

            // Скрываем кнопку удаления на img_1
            deleteButton.Visibility = Visibility.Hidden;
            if(img_1.Tag != null) rPhoto.Add((long)img_1.Tag);
            MessageBox.Show("Bread");

            // Проходимся по дочерним элементам stackPanelImages
            foreach (UIElement child in stackPanelImages.Children.OfType<Image>().ToList())
            {
                // Получаем URI источника текущего дочернего элемента
                Uri childImageUri = ((BitmapImage)(child as Image)?.Source)?.UriSource;

                // Проверяем, совпадает ли URI источника с URI источника удаленного изображения
                if (childImageUri != null && childImageUri.Equals(deletedImageUri))
                {
                    // Удаляем текущее изображение из stackPanelImages
                    stackPanelImages.Children.Remove(child);
                }
            }
        }
        private void img_1_MouseEnter(object sender, MouseEventArgs e)
        {
            // Показываем кнопку удаления на img_1 при наведении мыши
            deleteButton.Visibility = Visibility.Visible;
        }

        private void img_1_MouseLeave(object sender, MouseEventArgs e)
        {
            // Скрываем кнопку удаления на img_1 при уходе мыши
            deleteButton.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateRoomAsync();
            FrameManager.MainFrame.Navigate(new ListHotelRoom(mainWindow));
        }
    }
}
