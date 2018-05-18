using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Threading;

namespace ChristmasTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isSettingsOpened = false;

        public MainWindow()
        {
            InitializeComponent();
            Top = 0;
            Left = Properties.Settings.Default.WindowLocation;
            Height = System.Windows.SystemParameters.WorkArea.Height;
            Width = Height / 1.22;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(treeHead, Width / 2.65);

            try
            {
                DataTable treeElements = Properties.Settings.Default.TreeElements;
                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                if (treeElements != null)
                {
                    foreach (DataRow row in treeElements.Rows)
                    {
                        Image image = new Image();
                        image.Source = (ImageSource)imageSourceConverter.ConvertFromString(row["elmSource"].ToString());
                        switch (row["elmStretch"].ToString())
                        {
                            case "None":
                                image.Stretch = Stretch.None;
                                break;
                            case "Fill":
                                image.Stretch = Stretch.Fill;
                                break;
                        }
                        image.Width = image.Height = 64;
                        Canvas.SetLeft(image, double.Parse(row["elmLeft"].ToString()));
                        Canvas.SetTop(image, double.Parse(row["elmTop"].ToString()));
                        treeArea.Children.Add(image);
                    }
                }
            }
            catch { }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isSettingsOpened) {
                DragMove();
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && isSettingsOpened) {
                Top = 0;
                Properties.Settings.Default.WindowLocation = Left;
                Properties.Settings.Default.Save();
            }
        }

        private void settingsMI_Click(object sender, RoutedEventArgs e)
        {
            if (!isSettingsOpened) {
                Settings settings = new Settings();
                settings.Owner = this;
                isSettingsOpened = true;
                treeArea.Cursor = Cursors.Hand;
                settings.Show();
            }
        }

        private void exitMI_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            if (isSettingsOpened) {
                try {
                    ImageData imageData = (ImageData)e.Data.GetData(typeof(ImageData));
                    if (imageData.tag.ToString() == "new") {
                        newItemShadow.Source = imageData.source;
                        newItemShadow.Visibility = System.Windows.Visibility.Visible;
                        newItemShadow.Stretch = imageData.stretch;
                        newItemShadow.Tag = "old";
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (isSettingsOpened) {
                try {
                    ImageData imageData = (ImageData)e.Data.GetData(typeof(ImageData));
                    if (imageData.tag.ToString() == "new") {
                        Canvas.SetLeft(newItemShadow, e.GetPosition(treeArea).X - 32);
                        Canvas.SetTop(newItemShadow, e.GetPosition(treeArea).Y - 32);
                        newItemShadow.Width = newItemShadow.Height = 50;
                    } else {
                        Canvas.SetLeft(imageData.sourceImage, e.GetPosition(treeArea).X - ((Point)imageData.tag).X);
                        Canvas.SetTop(imageData.sourceImage, e.GetPosition(treeArea).Y - ((Point)imageData.tag).Y);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            if (isSettingsOpened) {
                ImageData imageData = (ImageData)e.Data.GetData(typeof(ImageData));
                if (imageData.tag.ToString() == "new") {
                    newItemShadow.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (isSettingsOpened) {
                if (newItemShadow.Visibility == System.Windows.Visibility.Visible) {
                    Image image = new Image();
                    image.Source = newItemShadow.Source;
                    image.Stretch = newItemShadow.Stretch;
                    image.Width = image.Height = 50;
                    Canvas.SetLeft(image, Canvas.GetLeft(newItemShadow));
                    Canvas.SetTop(image, Canvas.GetTop(newItemShadow));
                    treeArea.Children.Add(image);
                    newItemShadow.Visibility = System.Windows.Visibility.Hidden;
                } else {
                    
                }
            }
        }

        private void treeArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSettingsOpened) {
                if (e.LeftButton == MouseButtonState.Pressed && e.OriginalSource.GetType() == typeof(Image)) {
                    Image image = (Image)e.OriginalSource;
                    ImageData imageData = new ImageData();
                    imageData.source = image.Source;
                    imageData.tag = e.GetPosition((Image)e.OriginalSource);
                    imageData.stretch = image.Stretch;
                    imageData.sourceImage = image;
                    DragDrop.DoDragDrop(image, imageData, DragDropEffects.Move);
                    e.Handled = true;
                }
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (isSettingsOpened) {
                if (e.OriginalSource.GetType() == typeof(Image) && ((Image)e.OriginalSource).Name != "treeHead" && ((Image)e.OriginalSource).Name != "newItemShadow") {
                    treeArea.Children.Remove((Image)e.OriginalSource);
                }
            }
        }

    }
}
