using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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

namespace ZEWS.Xaml
{
    /// <summary>
    /// Логика взаимодействия для AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        public MainWindow mainWindow;
        public AutorizationPage(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            mainWindow.UpdateWindowTitle("Страница Авторизации");
            mainWindow.Height = 450;
            mainWindow.Width = 300;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            // Создаем объект для хранения данных аутентификации
            var credentials = new { phone = login, password = password };

            // Сериализуем объект в JSON
            string json = JsonConvert.SerializeObject(credentials);
      

            using (HttpClient client = new HttpClient())
            {
                //try
                //{
                    HttpResponseMessage response = await client.PostAsync("http://matokhnyuk-aa.tepk-it.ru/zews/api/auth/login",
                        new StringContent(json, Encoding.UTF8, "application/json"));

                    
                    //response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    string responseBody = await response.Content.ReadAsStringAsync();
                    
                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);



                // Сохраняем токен в настройках приложения
                    Properties.Settings.Default.Token = responseObject.token;
                    Properties.Settings.Default.Name = responseObject.userdata.name;
                    Properties.Settings.Default.Surname = responseObject.userdata.surname;
                    Properties.Settings.Default.Patronymic = responseObject.userdata.patronymic;
                    Properties.Settings.Default.Phone = responseObject.userdata.phone;
                    Properties.Settings.Default.Save();
                    
                    // Переход на другую страницу 
                    FrameManager.MainFrame.Navigate(new HomePage(mainWindow));

                //}

                //catch (Exception ex)
                //{
                // Ошибка при неправильно введенных данных
                //MessageBox.Show("Проверьте правильность введенных данных");
                //MessageBox.Show(ex.Message);
                //}
            }
        }
    }
}
