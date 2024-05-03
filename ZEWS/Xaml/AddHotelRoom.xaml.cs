using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZEWS.Class;

namespace ZEWS
{
    public partial class AddHotelRoom : Page
    {
        private string imagePath = "";
        private MainWindow mainWindow;
        private HttpClient client;
        private string[] roomTypes;

        public AddHotelRoom(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.client = new HttpClient();
            FillRoomTypesComboBox();
        }

        private async void FillRoomTypesComboBox()
        {
            try
            {
                // Отправить запрос к API для получения списка типов номеров
                HttpResponseMessage response = await client.GetAsync(APIconfig.APIurl + "/rooms/types");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JObject.Parse(responseBody);
                var roomTypesObject = responseObject["roomTypes"];

                roomTypes = roomTypesObject.Select(r => r.ToString()).ToArray();
                // Присваиваем значения roomTypes свойству ItemsSource ComboBox
                type.ItemsSource = roomTypes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message);
            }
        }
    }
}
