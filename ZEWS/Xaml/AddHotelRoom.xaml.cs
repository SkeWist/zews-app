using System.Windows;
using System.Windows.Controls;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для AddRedactHotelRoom.xaml
    /// </summary>
    public partial class AddHotelRoom : Page
    {
        public AddHotelRoom()
        {
            InitializeComponent();
        }
        private void Description_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Description.Text == "Описание")
            {
                Login.Text = "";
            }
        }

        private void Description_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Description.Text == "")
            {
                Description.Text = "Описание";
            }
        }

        private void Login_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Login.Text == "Логин")
            {
                Login.Text = "";
            }
        }

        private void Login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Login.Text == "")
            {
                Login.Text = "Логин";
            }
        }
    }
}
