using System;
using System.Collections.Generic;
using System.Linq;
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
