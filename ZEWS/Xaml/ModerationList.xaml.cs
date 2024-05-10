using Newtonsoft.Json;
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
    /// Логика взаимодействия для ModerationList.xaml
    /// </summary>
    public partial class ModerationList : Page
    {
        private MainWindow mainWindow;
        private string accessToken = Properties.Settings.Default.Token;
        private readonly HttpClient client = new HttpClient();

        public ModerationList(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            LoadReviewsAsync();
        }

        private async void LoadReviewsAsync()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/reviews");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<Review> reviews = JsonConvert.DeserializeObject<List<Review>>(responseBody);

                    // Привязать список отзывов к ListBox
                    reviewsListBox.ItemsSource = reviews;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении данных: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
