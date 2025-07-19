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
        }
    }
}
