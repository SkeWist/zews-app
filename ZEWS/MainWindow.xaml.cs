using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ZEWS.Xaml;

namespace ZEWS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameManager.MainFrame = this.Mainframe;

            Mainframe.Navigate(new AutorizationPage(this));
        }

        public void UpdateWindowTitle(string pageTitle)
        {
            this.Title = pageTitle;
        }
    }
}
