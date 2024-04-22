using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
               if (loginTextBox.Text == "Логин")
               {
                loginTextBox.Text = "";
               }
            
        }

        private void loginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text == "")
            {
                loginTextBox.Text = "Логин";
            }
        }

        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password == "Пароль")
            {
                
                passwordTextBox.Password = "";
            }
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password == "")
            {

                passwordTextBox.Password = "Пароль";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync("http://api-all.ru/api/login",
                        new StringContent(json, Encoding.UTF8, "application/json"));

                    response.EnsureSuccessStatusCode(); // Гарантирует, что ответ успешный

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Парсим ответ в объект
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    string token = responseObject.token;

                    // Сохраняем токен в настройках приложения
                    Token.token = token;
                    Properties.Settings.Default.Token = token;
                    Properties.Settings.Default.Save();

                    // Переход на другую страницу или какая-то другая логика после успешной аутентификации
                    FrameManager.MainFrame.Navigate(new MainPage(mainWindow));
                }

                catch (Exception)
                {
                    // Ошибка при неправильно введенных данных
                    MessageBox.Show("Проверьте правильность введенных данных");
                }
            }
        }
    }
}
