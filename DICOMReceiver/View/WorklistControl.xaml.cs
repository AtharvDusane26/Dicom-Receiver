using DICOMReceiver.Models.Entities;
using DICOMReceiver.Models;
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
using System.Windows.Shapes;
using System.IO;

namespace DICOMReceiver.View
{
    /// <summary>
    /// Interaction logic for WorklistControl.xaml
    /// </summary>
    public partial class WorklistControl : Window
    {
        public WorklistControl()
        {
            InitializeComponent();
            LoadStudyData();
        }

        private void LoadStudyData()
        {
            DBHandler db = new DBHandler();
            List<Study> studies = db.GetAllStudies(); // You must implement this method
            StudyDataGrid.ItemsSource = studies;
            var contextMenu = new ContextMenu();
            MenuItem viewerMenuItem = new MenuItem
            {
                Header = "Launch Viewer",
            };
            viewerMenuItem.Click += ViewerMenuItem_Click;
            contextMenu.Items.Add(viewerMenuItem);
            StudyDataGrid.ContextMenu = contextMenu;
        }
        public void ViewerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (StudyDataGrid.SelectedItem is Study selectedStudy)
            {
                LaunchViewer(selectedStudy);
            }
        }
        private void LaunchViewer(Study study)
        {
            var image = Directory.GetFiles(DirectoryPath.Instance.ImageDirectory, $"{study.StudyInstanceUID}/*.dcm", SearchOption.AllDirectories);
            if (image != null && image.Count() > 0)
            {
                var viewer = new ImageViewerControl(image);
                viewer.Show();
            }
            else
            {
                MessageBox.Show("No images found for this study.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
