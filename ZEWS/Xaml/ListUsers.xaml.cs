using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
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
using ZEWS.Class;
using ZEWS.Xaml;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для ListUsers.xaml
    /// </summary>
    public partial class ListUsers : Page
    {
        private MainWindow mainWindow;

        private string TranslateRole(string role)
        {
            switch (role)
            {
                case "admin":
                    return "Администратор";
                case "manager":
                    return "Менеджер";
                case "user":
                    return "Пользователь";
                default:
                    return role;

            }
        }
        public ListUsers(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.Height = 550;
            mainWindow.Width = 800;
            Loaded += ListUsers_Loaded;
            usersListBox.MouseDoubleClick += usersListBox_MouseDoubleClick;
        }

        private void ListUsers_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData(); // Вызываем метод загрузки данных при загрузке страницы
        }

        private void usersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Получаем выбранный элемент ListBox
            NewUser selectedUser = (NewUser)usersListBox.SelectedItem;


            // Переходим к странице редактирования пользователя и передаем выбранного пользователя в конструктор
            FrameManager.MainFrame.Navigate(new RedactUser(selectedUser.id, mainWindow));
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

                    // Отправляем GET запрос по указанному URL для загрузки пользователей
                    HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/users");

                    // Проверяем успешность запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Получаем содержимое ответа в виде строки
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Десериализуем JSON в массив объектов JArray
                        JObject jsonResponse = JObject.Parse(responseBody);

                        JArray usersArray = (JArray)jsonResponse["users"];

                        // Десериализуем JSON-массив в список объектов User
                        List<NewUser> users = usersArray.Select(u => u.ToObject<NewUser>()).ToList();

                        //Перевод ролей
                        foreach (var user in users)
                        {
                            user.role = TranslateRole(user.role);
                        }

                        // Привязываем список users к источнику данных ListBox
                        usersListBox.ItemsSource = users;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new HomePage(mainWindow));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new AddNewUser(mainWindow));
        }
    }
}
