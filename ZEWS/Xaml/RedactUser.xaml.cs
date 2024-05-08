using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что вводимый символ является цифрой или символом '_'
            if (!char.IsDigit(e.Text, 0) && e.Text != "_")
            {
                // Если вводимый символ не является цифрой или символом '_', отменяем его
                e.Handled = true;
            }
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
                        MessageBox.Show(currentUser.id.ToString());
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

            phone.Text =                        currentUser.phone.ToString();
            roleComboBox.SelectedItem =         currentUser?.role.ToString();
            birthday.Text =                     currentUser?.birthday.ToString();
            surname.Text =                      currentUser.surname;
            name.Text =                         currentUser.name;
            patronymic.Text =                   currentUser.patronymic;
            pass_number.Text =                  currentUser.pass_number.ToString();
            pass_authority_code.Text =          currentUser.pass_authority_code.ToString();
            pass_authority_name.Text =          currentUser.pass_authority_name;
            pass_birth_address.Text =           currentUser.pass_birth_address;
            pass_issue_date.Text =              currentUser?.pass_issue_date.ToString();
            sexComboBox.SelectedItem =          currentUser?.sex.ToString();
        }
        
        private async void UpdateUserAsync()
        {
            string token = Properties.Settings.Default.Token;            
                // Создание HttpClient
                using (HttpClient client = new HttpClient())
                {
                    string gender = sexComboBox.SelectedItem.ToString();
                    int sexValue = gender == "male" ? 1 : 0;
                DateTime Birthday = birthday.SelectedDate ?? new DateTime();
                DateTime PassIssueDate = pass_issue_date.SelectedDate ?? new DateTime();
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                    if (phone.Text != "" && Regex.Replace(phone.Text, @"[^\d]", "") != currentUser.phone.ToString())                   multiContent.Add(new StringContent(Regex.Replace(phone.Text, @"[^\d]", "")), "phone");
                    if (password.Text != "")                                                   multiContent.Add(new StringContent(password.Text), "password");
                    if (birthday.Text != "")                                                   multiContent.Add(new StringContent(Birthday.ToString("yyyy-MM-dd")), "birthday");
                    if (roleComboBox.SelectedItem != null)                                     multiContent.Add(new StringContent(roleComboBox.SelectedItem.ToString()), "role");
                    if (name.Text != "")                                                       multiContent.Add(new StringContent(name.Text), "name");
                    if (surname.Text != "")                                                    multiContent.Add(new StringContent(surname.Text), "surname");
                    if (patronymic.Text != "")                                                 multiContent.Add(new StringContent(patronymic.Text), "patronymic");
                    if ((sexValue == 1?true:false) != currentUser.sex)                                           multiContent.Add(new StringContent(sexValue.ToString()), "sex");
                    if (pass_number.Text != "" &&
                    Regex.Replace(pass_number.Text, @"[^\d]", "") != currentUser.pass_number.ToString()) multiContent.Add(new StringContent(Regex.Replace(pass_number.Text, @"[^\d]", "")), "pass_number");
                    if (pass_authority_code.Text != "")                                        multiContent.Add(new StringContent(Regex.Replace(pass_authority_code.Text, @"[^\d]", "")), "pass_authority_code");
                    if (pass_authority_name.Text != "")                                        multiContent.Add(new StringContent(pass_authority_name.Text), "pass_authority_name");
                    if (pass_birth_address.Text != "")                                         multiContent.Add(new StringContent(pass_birth_address.Text), "pass_birth_address");
                    if (pass_issue_date.Text != "")                                            multiContent.Add(new StringContent(PassIssueDate.ToString("yyyy-MM-dd")), "pass_issue_date");
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
                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/users/{currentUser.id}", multiContent);

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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserAsync();
            FrameManager.MainFrame.Navigate(new ListUsers(mainWindow));
        }

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
            mainWindow.Height = 550;
            mainWindow.Width = 800;
        }
    }
}
