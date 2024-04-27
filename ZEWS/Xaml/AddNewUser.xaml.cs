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
using System.Globalization;
using System.Windows.Data;
using ZEWS.Class;
using System.Windows.Input;

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
            name.PreviewTextInput += TextBox_PreviewTextInput;
            surname.PreviewTextInput += TextBox_PreviewTextInput;
            patronymic.PreviewTextInput += TextBox_PreviewTextInput;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что вводимый символ является буквой
            if (!Regex.IsMatch(e.Text, @"^[а-яА-Яa-zA-Z]+$"))
            {
                // Если вводимый символ не является буквой, отменяем его
                e.Handled = true;
            }
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

            string gender = sexComboBox.SelectedItem.ToString();
            int sexValue = gender == "male" ? 1 : 0;
            
            var newUser = new
                {
                    phone = Convert.ToInt64(Regex.Replace(phone.Text, @"[^\d]", "")),
                    password = password.Text,
                    passwordRepeat = password.Text,
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

                string json = JsonConvert.SerializeObject(newUser);
            MessageBox.Show(json);

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
