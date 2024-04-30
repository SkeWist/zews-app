using System.Windows;
using System.Windows.Controls;

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
    }
}
