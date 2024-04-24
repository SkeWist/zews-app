using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;
using System;
using System.Windows.Controls;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для AddNewUser.xaml
    /// </summary>
    public partial class AddNewUser : Page
    {
        private MainWindow mainWindow;
        public AddNewUser(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = new MainWindow();
            mainWindow.UpdateWindowTitle("Добавление пользователя");
            mainWindow.Height = 450;
            mainWindow.Width = 1100;
            roleComboBox.Text = "Администратор";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //// Проверка номера телефона
            //if (!IsValidPhoneNumber(phone.Text))
            //{
            //    MessageBox.Show("Неверный формат номера телефона. Должен быть в формате: 92223332211");
            //    return;
            //}

            //// Проверка пароля
            //if (password.Text.Length < 8 || password.Text.Length > 64)
            //{
            //    MessageBox.Show("Пароль должен содержать от 8 до 64 символов.");
            //    return;
            //}

            //// Проверка, что пароль и его повторение совпадают
            //if (password.Text != passwordRepeat.Text)
            //{
            //    MessageBox.Show("Пароль и его повторение не совпадают.");
            //    return;
            //}

            //// Проверка выбора роли пользователя
            //if (roleComboBox.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Выберите роль пользователя.");
            //    return;
            //}

            //// Проверка даты рождения
            //if (!IsValidDate(birthday.Text))
            //{
            //    MessageBox.Show("Неверный формат даты рождения. Должен быть в формате: 2000-01-01");
            //    return;
            //}
            

            // Создаем объект для хранения данных нового пользователя
            var newUser = new
            {
                phone = phone.Text,
                password = password.Text,
                passwordRepeat = password.Text,
                role = roleComboBox.SelectedItem.ToString(),
                birthday = birthday.Text,
                surname = surname.Text,
                name = name.Text,
                patronymic = patronymic.Text,
                passNumber = passNumber.Text,
                passAuthorityCode = passAuthorityCode.Text,
                passAuthorityName = passAuthorityName.Text,
                passBirthAddress = passBirthAddress.Text,
                passIssueDate = passIssueDate.Text,
                
                
                // Предполагается, что ComboBox содержит строки
                // Добавьте другие поля пользователя сюда в зависимости от вашего API
            };

            // Преобразуем объект в JSON
            string json = JsonConvert.SerializeObject(newUser);

            // Определяем адрес вашего API
            string apiUrl = APIconfig.APIurl + "/users";

            using (var client = new HttpClient())
            {
                try
                {
                    // Отправляем POST-запрос на сервер
                    var response = await client.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                    // Проверяем успешность операции
                    if (response.IsSuccessStatusCode)
                    {
                        // Операция завершена успешно
                        MessageBox.Show("Пользователь успешно создан!");
                    }
                    else
                    {
                        // Произошла ошибка
                        MessageBox.Show("Ошибка при создании пользователя. Пожалуйста, попробуйте еще раз.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
                }
            }        
        }

        //// Метод для проверки формата номера телефона
        //private bool IsValidPhoneNumber(string phoneNumber)
        //{
        //    // Создаем регулярное выражение для проверки формата номера телефона
        //    Regex regex = new Regex(@"^9-\(\d{3}\)-\d{3}-\d{2}-\d{2}$");

        //    // Проверяем, соответствует ли номер формату
        //    return regex.IsMatch(phoneNumber);
        //}

        //// Метод для проверки формата даты
        //private bool IsValidDate(string date)
        //{
        //    // Используем регулярное выражение для проверки формата даты
        //    string pattern = @"^\d{4}-\d{2}-\d{2}$";
        //    return Regex.IsMatch(date, pattern);
        //}

        //private bool IsValidPassportSeries(string series)
        //{
        //    // Создаем регулярное выражение для проверки формата серии паспорта
        //    Regex regex = new Regex(@"^\d{4}\s\d{6}$");

        //    // Проверяем, соответствует ли серия паспорта формату
        //    return regex.IsMatch(series);
        //}

        //private bool IsValidDepartmentCode(string code)
        //{
        //    // Создаем регулярное выражение для проверки формата кода подразделения
        //    Regex regex = new Regex(@"^\d{3}-\d{3}$");

        //    // Проверяем, соответствует ли код подразделения формату
        //    return regex.IsMatch(code);
        //}

        //private bool IsValidPassportIssueDate(string date)
        //{
        //    // Создаем регулярное выражение для проверки формата даты
        //    Regex regex = new Regex(@"^\d{4}-\d{2}-\d{2}$");

        //    // Проверяем, соответствует ли дата формату
        //    if (!regex.IsMatch(date))
        //    {
        //        return false;
        //    }

        //    // Проверяем, что дата также является корректной датой
        //    if (!DateTime.TryParse(date, out _))
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}
