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
using System.Windows.Shapes;
using System.Data;

namespace ChristmasTree
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void wrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)e.Source;
            ImageData imageData = new ImageData();
            imageData.source = image.Source;
            imageData.tag = "new";
            imageData.stretch = image.Stretch;
            imageData.sourceImage = image;
            DragDrop.DoDragDrop(image, imageData, DragDropEffects.Move);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            DataTable treeElements = new DataTable("TreeElements");
            treeElements.Columns.Add("elmSource");
            treeElements.Columns.Add("elmStretch");
            treeElements.Columns.Add("elmLeft");
            treeElements.Columns.Add("elmTop");
            foreach (Image el in ((MainWindow)Owner).treeArea.Children) {
                if (el.Name != "newItemShadow" && el.Name != "treeHead") {
                    treeElements.Rows.Add(el.Source, el.Stretch, Canvas.GetLeft(el), Canvas.GetTop(el));
                }
            }
            Properties.Settings.Default.TreeElements = treeElements;
            Properties.Settings.Default.Save();
            ((MainWindow)Owner).isSettingsOpened = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).treeArea.Cursor = Cursors.Arrow;
        }
    }

    public class ImageData {
        public ImageSource source;
        public object tag;
        public Stretch stretch;
        public Image sourceImage = null;
    }
}
