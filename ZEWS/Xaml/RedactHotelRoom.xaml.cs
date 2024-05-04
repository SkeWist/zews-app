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

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для RedactHotelRoom.xaml
    /// </summary>
    public partial class RedactHotelRoom : Page
    {
        private string accessToken = Properties.Settings.Default.Token;
        private HotelRoom currentHotelRoom;
        private MainWindow mainWindow;
        private long roomId;
        public RedactHotelRoom(long id, MainWindow mainWindow )
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.UpdateWindowTitle("Редактирование пользователя");
            mainWindow.Height = 450;
            mainWindow.Width = 1100;
            LoadData();
        }
        private async void LoadData()
        {
            string token = Properties.Settings.Default.Token;
            try
            {
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
                        currentHotelRoom = room.ToObject<HotelRoom>();
                        MessageBox.Show(currentHotelRoom.Id.ToString());
                        FillForm();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при обновлении информации о пользователе: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
        private void FillForm()
        {
            string apiUrl = APIconfig.APIurl;

            name.Text = currentHotelRoom.Name.ToString();
            type.SelectedItem = currentHotelRoom?.Type.ToString();
            price.Text = currentHotelRoom?.Price.ToString();
            description.Text = currentHotelRoom.Description;
            foreach (var photo in currentHotelRoom.Photos)
            {
                AddThumbnailImage(APIconfig.APIstorage + "/" + photo.Name);

            }

        }
    }
}
