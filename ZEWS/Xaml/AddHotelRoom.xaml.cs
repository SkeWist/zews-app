using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        public AddHotelRoom(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            FillRoomTypesComboBox();
        }

        private void FillRoomTypesComboBox()
        {
            List<string> roomTypes = GetRoomTypesFromAPI();

            foreach (string roomType in roomTypes)
            {
                type.Items.Add(roomType);
            }
        }

        private List<string> GetRoomTypesFromAPI()
        {
            List<string> roomTypes = new List<string>
            {
                "1-комнатная",
                "2-комнатная",
                "Люкс",
                "Президентская"
            };

            return roomTypes;
        }

        // Добавляем кнопку удаления фотографии к img_1
        private void AddDeleteButtonToImg1()
        {
            Button deleteButton = new Button();
            deleteButton.Content = "Удалить фотографию";
            deleteButton.Click += (s, e) =>
            {
                RemoveImage();
            };

            // Добавляем кнопку удаления в StackPanel, который содержит img_1
            stackPanel_img_1.Children.Add(deleteButton);
        }

        // Удаляем кнопку удаления фотографии из img_1
        private void RemoveDeleteButtonFromImg1()
        {
            stackPanel_img_1.Children.Clear();
        }

        // Обработчик события MouseEnter для img_1
        private void Img1_MouseEnter(object sender, MouseEventArgs e)
        {
            AddDeleteButtonToImg1();
        }

        // Обработчик события MouseLeave для img_1
        private void Img1_MouseLeave(object sender, MouseEventArgs e)
        {
            RemoveDeleteButtonFromImg1();
        }

        // Добавляем обработчики событий MouseEnter и MouseLeave для img_1
        private void InitializeImgEvents()
        {
            img_1.MouseLeftButtonDown += Img1_MouseLeftButtonDown;
            img_2.MouseLeftButtonDown += Img2_MouseLeftButtonDown;
            img_3.MouseLeftButtonDown += Img3_MouseLeftButtonDown;
            img_4.MouseLeftButtonDown += Img4_MouseLeftButtonDown;
        }
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage && clickedImage != img_1)
            {
                SwapImages(clickedImage);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Обработчик события для кнопки, если нужно
        }

        private List<string> imagePaths = new List<string>();

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Разрешить выбор нескольких файлов
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg; *.jpeg; *.png; *.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                string[] selectedImagePaths = openFileDialog.FileNames;

                foreach (string path in selectedImagePaths)
                {
                    if (!imagePaths.Contains(path))
                    {
                        imagePaths.Add(path);
                    }
                }

                if (imagePaths.Count > 0)
                {
                    img_1.Source = new BitmapImage(new Uri(imagePaths[0]));
                    deleteImageButton.Visibility = Visibility.Visible; // Показываем кнопку удаления фотографии
                }

                SetImagesToElements();
            }
        }

        private void SetImagesToElements()
        {
            // Очищаем сначала все изображения
            img_1.Source = null;
            img_2.Source = null;
            img_3.Source = null;

            // Устанавливаем источники только для доступных изображений
            for (int i = 0; i < imagePaths.Count; i++)
            {
                if (i == 0)
                {
                    // Устанавливаем изображение только для img_1
                    img_1.Source = new BitmapImage(new Uri(imagePaths[i]));
                    continue;
                }
                else if (i == 1)
                {
                    // Устанавливаем изображение только для img_2
                    img_2.Source = new BitmapImage(new Uri(imagePaths[i]));
                }
                else if (i == 2)
                {
                    // Устанавливаем изображение только для img_3
                    img_3.Source = new BitmapImage(new Uri(imagePaths[i]));
                }
                else
                {
                    // Если больше нет места для изображений, выходим из цикла
                    break;
                }
            }

            // Инициализируем события для img_1
            img_1.MouseLeftButtonDown += Img1_MouseLeftButtonDown;
        }
        private void Img1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Если кнопка удаления видима, скрыть её; иначе показать
            deleteImageButton.Visibility = deleteImageButton.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Img2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwapImages(img_2);
        }

        private void Img3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwapImages(img_3);
        }

        private void Img4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwapImages(img_4);
        }

        private void SwapImages(Image clickedImage)
        {
            // Получаем источник изображения выбранного изображения
            ImageSource clickedImageSource = clickedImage.Source;

            // Сохраняем источник изображения img_1
            ImageSource img1Source = img_1.Source;

            // Обмениваем источники изображений
            img_1.Source = clickedImageSource;
            clickedImage.Source = img1Source;

            // Обновляем список путей к изображениям
            int indexClickedImage = int.Parse(clickedImage.Name.Split('_')[1]);
            string temp = imagePaths[indexClickedImage - 1];
            imagePaths[indexClickedImage - 1] = imagePaths[0];
            imagePaths[0] = temp;
        }



        private void RemoveImage()
        {
            img_1.Source = null;
            imagePaths[0] = "";
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем источник изображения img_1
            img_1.Source = null;

            // Очищаем путь к изображению в списке
            imagePaths[0] = "";

            // Скрываем кнопку удаления фотографии
            deleteImageButton.Visibility = Visibility.Collapsed;
        }
    }
}
