using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
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
using static ZEWS.ListUsers;

namespace ZEWS.Xaml
{
    /// <summary>
    /// Логика взаимодействия для RedactUser.xaml
    /// </summary>
    public partial class RedactUser : Page
    {
        private string accessToken = Properties.Settings.Default.Token;
        private NewUser currentUser;
        private long userId;

        private MainWindow mainWindow;
        public RedactUser(long userId, MainWindow mainWindow)
        {
            InitializeComponent();
            this.userId = userId;
            this.mainWindow = mainWindow;
            mainWindow.UpdateWindowTitle("Редактирование пользователя");
            mainWindow.Height = 450;
            mainWindow.Width = 1100;
            LoadUserData();
            roleComboBox.ItemsSource = new List<Role>() {
                new Role("admin", "Администратор"),
                new Role("manager", "Менеджер"),
                new Role("user", "Пользователь")
            };
            roleComboBox.DisplayMemberPath = "Name";
            roleComboBox.SelectedIndex = 0;
            sexComboBox.ItemsSource = new List<Sex>()
            {
                new Sex("male", "Мужчина"),
                new Sex("female", "Женщина")
            };
            sexComboBox.DisplayMemberPath = "Name";
            sexComboBox.SelectedIndex = 0;
        }
        private async void LoadUserData() 
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
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(currentUser);
                    

                    // Отправка PUT запроса на сервер для обновления информации о пользователе
                    HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/{userId}");

                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JObject.Parse(responseBody);
                        currentUser = user.ToObject<NewUser>();
                        FillUserData();
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
        private void FillUserData()
        {
            string apiUrl = APIconfig.APIurl;

            phone.Text = currentUser.phone;
            roleComboBox.Text = currentUser?.role.ToString();
            birthday.Text = currentUser?.birthday.ToString();
            surname.Text = currentUser.surname;
            name.Text = currentUser.name;
            patronymic.Text = currentUser.patronymic;
            pass_number.Text = currentUser.pass_number;
            pass_authority_code.Text = currentUser.pass_authority_code;
            pass_authority_name.Text = currentUser.pass_authority_name;
            pass_birth_address.Text = currentUser.pass_birth_address;
            pass_issue_date.Text = currentUser?.pass_issue_date.ToString();
            sexComboBox.Text = currentUser?.sex.ToString();
        }

        private async void UpdateUserAsync()
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
                    string gender = sexComboBox.SelectedItem.ToString();
                    int sexValue = gender == "male" ? 1 : 0;
                    string json;
                    if (password.Text != null && password.Text != "")
                    {
                        var newUser = new
                        {
                            phone = Convert.ToInt64(Regex.Replace(phone.Text, @"[^\d]", "")),
                            password = password.Text,
                            role = roleComboBox.SelectedItem.ToString(),
                            birthday = birthday.Text,
                            surname = surname.Text,
                            name = name.Text,
                            patronymic = patronymic.Text ?? "",
                            pass_number = Convert.ToInt64(Regex.Replace(pass_number.Text, @"[^\d]", "")),
                            pass_authority_code = Convert.ToInt64(Regex.Replace(pass_authority_code.Text, @"[^\d]", "")),
                            pass_authority_name = pass_authority_name.Text,
                            pass_birth_address = pass_birth_address.Text,
                            pass_issue_date = pass_issue_date.Text,
                            sex = sexValue,

                        };
                        json = Newtonsoft.Json.JsonConvert.SerializeObject(newUser);
                    }
                    else
                    {
                        var newUser = new
                        {
                            phone = Convert.ToInt64(Regex.Replace(phone.Text, @"[^\d]", "")),
                            role = roleComboBox.SelectedItem.ToString(),
                            birthday = birthday.Text,
                            surname = surname.Text,
                            name = name.Text,
                            patronymic = patronymic.Text ?? "",
                            pass_number = Convert.ToInt64(Regex.Replace(pass_number.Text, @"[^\d]", "")),
                            pass_authority_code = Convert.ToInt64(Regex.Replace(pass_authority_code.Text, @"[^\d]", "")),
                            pass_authority_name = pass_authority_name.Text,
                            pass_birth_address = pass_birth_address.Text,
                            pass_issue_date = pass_issue_date.Text,
                            sex = sexValue,
                        };
                        json = Newtonsoft.Json.JsonConvert.SerializeObject(newUser);
                    }

                    StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

                    // Отправка POST запроса на сервер для обновления информации о пользователе
                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/users/{currentUser.id}", data);
                    MessageBox.Show(json);

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserAsync();
            FrameManager.MainFrame.Navigate(new ListUsers(mainWindow));
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new ListUsers(mainWindow));
        }
    }
}
