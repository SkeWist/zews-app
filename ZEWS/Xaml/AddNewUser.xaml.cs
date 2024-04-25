using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;
using System;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Data;
using System.Collections.Generic;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для AddNewUser.xaml
    /// </summary>
        public partial class AddNewUser : Page
        {
            private string accessToken = Properties.Settings.Default.Token;

            private MainWindow mainWindow;
            public AddNewUser(MainWindow mainWindow)
            {
                InitializeComponent();
                this.mainWindow = new MainWindow();
                mainWindow.UpdateWindowTitle("Добавление пользователя");
                mainWindow.Height = 450;
                mainWindow.Width = 1100;
                roleComboBox.ItemsSource = new string[] { "admin", "manager", "user" };
                sexComboBox.ItemsSource = new string[] { "male", "femaly" };
                roleComboBox.SelectedIndex = 0;
            }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
            {
                if (string.IsNullOrEmpty(phone.Text) || string.IsNullOrEmpty(password.Text) ||
                    string.IsNullOrEmpty(name.Text) || string.IsNullOrEmpty(surname.Text) ||
                    string.IsNullOrEmpty(birthday.Text) || string.IsNullOrEmpty(pass_number.Text) ||
                    string.IsNullOrEmpty(pass_issue_date.Text) || string.IsNullOrEmpty(pass_birth_address.Text) ||
                    string.IsNullOrEmpty(pass_authority_name.Text) || string.IsNullOrEmpty(pass_authority_code.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                    return;
                }

                var newUser = new
                {
                    phone = phone.Text,
                    password = password.Text,
                    passwordRepeat = password.Text,
                    role = roleComboBox.SelectedItem.ToString(),
                    birthday = birthday.Text,
                    surname = surname.Text,
                    name = name.Text,
                    patronymic = patronymic.Text ?? "",
                    pass_number = pass_number.Text,
                    pass_authority_code = pass_authority_code.Text,
                    pass_authority_name = pass_authority_name.Text,
                    pass_birth_address = pass_birth_address.Text,
                    pass_issue_date = pass_issue_date.Text,
                    sex = sexComboBox.Text,
                };

                string json = JsonConvert.SerializeObject(newUser);

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(APIconfig.APIurl + "/users", content);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Пользователь успешно создан!");
                        }
                        else
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Ошибка при создании пользователя: {response.StatusCode} - {errorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                }
            }
        }
    }    
