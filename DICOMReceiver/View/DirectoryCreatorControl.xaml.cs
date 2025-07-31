using Microsoft.Win32;
using NHibernate.Cfg;
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

namespace DICOMReceiver.View
{
    /// <summary>
    /// Interaction logic for DirectoryCreatorControl.xaml
    /// </summary>
    public partial class DirectoryCreatorControl : Window
    {
        public DirectoryCreatorControl()
        {
            InitializeComponent();
        }
        public bool CheckDataPath()
        {
            if (!String.IsNullOrWhiteSpace(GeneralSettings.Default.BaseDirectory))
                return true;
            return false;
        }
        public string GetDataPath()
        {
            return GeneralSettings.Default.BaseDirectory;
        }
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog();
            openFolderDialog.Title = "Select Folder Path";
            if (openFolderDialog.ShowDialog() == true)
            {
                txtFolderPath.Text = openFolderDialog.FolderName;
                GeneralSettings.Default.BaseDirectory = txtFolderPath.Text.ToString();
            }

        }

        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtFolderPath.Text))
            {
                MessageBox.Show("Please Select Appropriate DataPath", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            GeneralSettings.Default.Save();
            this.DialogResult = true;
            Close();
        }
    }
}
