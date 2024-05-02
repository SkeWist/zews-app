using System.Windows;
using System.Windows.Controls;
using ZEWS.Class;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для AddRedactHotelRoom.xaml
    /// </summary>
    public partial class AddHotelRoom : Page
    {
        private MainWindow mainWindow;
        public AddHotelRoom(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;  

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            Rooms rooms = new Rooms
            {
                Name = name.Text,
                Description = description.Text,
                Price = double.Parse(.Text)
            };
        }
    }
}
