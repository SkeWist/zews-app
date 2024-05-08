using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZEWS.Class;
using System.IO;

namespace ZEWS
{
    public partial class AddHotelRoom : Page
    {
        private MainWindow mainWindow;
        private HttpClient client;
        public AddHotelRoom(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.client = new HttpClient();
            FillRoomTypesComboBox();
            mainWindow.Height = 550;
            mainWindow.Width = 800;
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
                foreach ( var roomType in roomTypesObject.Properties())
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

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем URI источника изображения img_1
            Uri deletedImageUri = ((BitmapImage)img_1.Source)?.UriSource;

            // Устанавливаем источник изображения img_1 равным изображению NoPhoto.png
            img_1.Source = new BitmapImage(new Uri("/Img/NoPhoto.png", UriKind.RelativeOrAbsolute));

            // Скрываем кнопку удаления на img_1
            deleteButton.Visibility = Visibility.Hidden;

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
            // Проверка на пустое имя комнаты
            if (string.IsNullOrWhiteSpace(name.Text))
            {
                MessageBox.Show("Введите название комнаты.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка на выбор типа номера
            if (type.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип номера.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка на пустое поле цены
            if (string.IsNullOrWhiteSpace(price.Text))
            {
                MessageBox.Show("Введите цену комнаты.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка на пустое описание комнаты
            if (string.IsNullOrWhiteSpace(description.Text))
            {
                MessageBox.Show("Введите описание комнаты.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если все поля заполнены, отправляем данные на сервер
            SendFormData();
            FrameManager.MainFrame.Navigate(new ListHotelRoom(mainWindow));
        }

        private async void SendFormData()
        {
            string token = Properties.Settings.Default.Token;
            //try
            //{
                using (HttpClient client = new HttpClient())
                {
                    // Добавляем заголовок авторизации с токеном Bearer
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Создаем экземпляр MultipartFormDataContent
                    var multipartContent = new MultipartFormDataContent();

                    // Добавляем текстовые данные
                    multipartContent.Add(new StringContent(name.Text), "name");
                    multipartContent.Add(new StringContent(((RoomType)type.SelectedValue).Id.ToString()), "type_id");
                    multipartContent.Add(new StringContent(price.Text), "price");
                    multipartContent.Add(new StringContent(description.Text), "description");

                // Добавляем фотографии
                foreach (var child in stackPanelImages.Children)
                {
                    if (child is Image image)
                    {
                        BitmapImage bitmapImage = image.Source as BitmapImage;
                        if (bitmapImage != null)
                        {
                            // Получаем путь к файлу изображения
                            string imagePath = bitmapImage.UriSource.LocalPath;

                            // Читаем фотографию как массив байтов
                            byte[] photoBytes = File.ReadAllBytes(imagePath);

                            // Создаем содержимое для фотографии
                            ByteArrayContent photoContent = new ByteArrayContent(photoBytes);
                            photoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                            multipartContent.Add(photoContent, "photos[]", Path.GetFileName(imagePath));
                            // Добавляем содержимое фотографии к другим данным формы

                            // Ваш код для добавления содержимого фотографии к данным формы
                        }
                    }
                }
                // Отправляем запрос на сервер
                HttpResponseMessage response = await client.PostAsync(APIconfig.APIurl + "/rooms", multipartContent);

                    // Проверяем ответ
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Данные успешно отправлены!");
                    }
                    else
                    {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка при отправке данных: " + responseContent);
                    }
                }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new ListHotelRoom(mainWindow));
        }
    }
}
