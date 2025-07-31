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
using DICOMReceiver.Models.Dicom;
using System.Collections.ObjectModel;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Network;

namespace DICOMReceiver.View
{
    /// <summary>
    /// Interaction logic for WorklistControl.xaml
    /// </summary>
    public partial class WorklistControl : Window
    {
        public ObservableCollection<Node> Nodes { get; set; }
        public WorklistControl()
        {
            InitializeComponent();
            GetNodesFromDatabase();
            LoadStudyData();
        }
        private void GetNodesFromDatabase()
        {
            Nodes = new ObservableCollection<Node>();
            new DBHandler().GetAllNodes().ForEach(node =>
            {
                Nodes.Add(new Node
                {
                    AETitle = node.AETitle,
                    Host = node.Host,
                    Port = node.Port
                });
            });
        }
        private void CreateDataGridView()
        {
            DBHandler db = new DBHandler();
            var list = new List<StudyView>();
            List<Study> studies = db.GetAllStudies();
            var patients = db.GetAllPatients();
            if (studies.Any())
            {
                foreach (var study in studies)
                {
                    var patient = patients.Where(o => o.PatientID == study.PatientId).FirstOrDefault();
                    if (patient == null)
                        continue; // Skip if no patient found for the study
                    var view = new StudyView();
                    view.StudyID = study.StudyID;
                    view.StudyInstanceUid = study.StudyInstanceUID;
                    view.PatientName = patient.PatientName;
                    view.PatientID = patient.PatientID;
                    view.StudyDate = study.StudyDate;
                    view.Modality = study.ModalitiesInStudy;
                    view.Description = study.StudyDescription;
                    view.AccessionNumber = study.AccessionNumber;
                    list.Add(view);
                }
                StudyDataGrid.ItemsSource = list;
            }
        }
        private void LoadStudyData()
        {
            CreateDataGridView();
            var contextMenu = new ContextMenu();
            //MenuItem viewerMenuItem = new MenuItem
            //{
            //    Header = "Launch Viewer",
            //};
            //viewerMenuItem.Click += ViewerMenuItem_Click;
            //contextMenu.Items.Add(viewerMenuItem);

            MenuItem dicomSendItem = new MenuItem
            {
                Header = "Dicom Send",
            };
            dicomSendItem.Click += DicomSendItem_Click;
            contextMenu.Items.Add(dicomSendItem);
            StudyDataGrid.ContextMenu = contextMenu;
        }

        private void DicomSendItem_Click(object sender, RoutedEventArgs e)
        {
            if (StudyDataGrid.SelectedItem is StudyView selectedStudy)
            {
                var sendWindow = new DicomSendWindow(selectedStudy.StudyInstanceUid);
                sendWindow.Owner = this;
                sendWindow.ShowDialog();
            }
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
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out int port))
            {
                MessageBox.Show("Please enter valid AE Title, Host, and Port.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var node = new Node()
            {
                AETitle = txtAETitle.Text.Trim(),
                Host = txtHost.Text.Trim(),
                Port = port
            };
            new DBHandler().AddNode(new Models.Entities.Nodes
            {
                AETitle = node.AETitle,
                Host = node.Host,
                Port = node.Port
            });
            if (!Nodes.Any(o => o.AETitle == node.AETitle))
            {
                Nodes.Add(node);
            }
            MessageBox.Show($"Successfully configure for {txtAETitle.Text}");
            txtAETitle.Clear();
            txtHost.Clear();
            txtPort.Clear();
        }

        private async void EchoTest_Click(object sender, RoutedEventArgs e)
        {
            string aeTitle = txtAETitle.Text.Trim();
            string host = txtHost.Text.Trim();
            string portText = txtPort.Text.Trim();

            if (string.IsNullOrWhiteSpace(aeTitle) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portText) || !int.TryParse(portText, out int port))
            {
                MessageBox.Show("Please enter valid AE Title, Host, and Port.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnEchoTest.IsEnabled = false;
            try
            {
                string resultMessage = await Task.Run(async () =>
                {
                    try
                    {
                        var client = DicomClientFactory.Create(host, port, false, GeneralSettings.Default.LocalAETitle, aeTitle);
                        var request = new DicomCEchoRequest();

                        bool success = false;
                        request.OnResponseReceived += (req, resp) =>
                        {
                            success = resp.Status == DicomStatus.Success;
                        };

                        await client.AddRequestAsync(request);
                        await client.SendAsync();

                        return success
                            ? $"Echo succeeded for {aeTitle} at {host}:{port}."
                            : $"Echo failed for {aeTitle} at {host}:{port}.";
                    }
                    catch (Exception ex)
                    {
                        return $"Echo failed: {ex.Message}";
                    }
                });

                MessageBox.Show(resultMessage, "C-ECHO Result", MessageBoxButton.OK,
                    resultMessage.StartsWith("Echo succeeded") ? MessageBoxImage.Information :
                    resultMessage.StartsWith("Echo failed:") ? MessageBoxImage.Error :
                    MessageBoxImage.Warning);
            }
            finally
            {
                btnEchoTest.IsEnabled = true;
            }
        }
    }
    internal class StudyView
    {
        public string StudyID { get; set; }
        public string PatientName { get; set; }
        public string PatientID { get; set; }
        public string StudyDate { get; set; }
        public string Modality { get; set; }
        public string Description { get; set; }
        public string AccessionNumber { get; set; }
        public string StudyInstanceUid { get; set; }
    }
}
