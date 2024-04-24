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

    }
}
