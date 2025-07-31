using FellowOakDicom.Network.Client;
using FellowOakDicom.Network;
using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.IO;
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
using ApplicationBase;
using DICOMReceiver.Models.Entities;
using DICOMReceiver.Models.Dicom;
using DICOMReceiver.Models;
using System.Collections.ObjectModel;
using FluentNHibernate.Utils;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace DICOMReceiver.View
{
    /// <summary>
    /// Interaction logic for DicomSendWindow.xaml
    /// </summary>
    public partial class DicomSendWindow : Window
    {
        private string _studyInstanceUid;
        public ObservableCollection<Node> Nodes { get; set; }
        public DicomSendWindow(string studyInstanceUid)
        {
            InitializeComponent();
            GetNodesFromDatabase();
            cmbNodes.ItemsSource = Nodes;
            _studyInstanceUid = studyInstanceUid;
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
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSend.IsEnabled = false;

                var dicomFilePaths = Directory
                    .GetFiles(DirectoryPath.Instance.ImageDirectory, "*", SearchOption.AllDirectories)
                    .Where(path => path.EndsWith(".dcm", StringComparison.OrdinalIgnoreCase) && path.Contains(_studyInstanceUid))
                    .ToList();

                if (!dicomFilePaths.Any())
                {
                    MessageBox.Show("No DICOM files found for the selected study.", "No Files", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var nodes = GetRouteNodes();
                if (!nodes.Any())
                {
                    MessageBox.Show("Please select node", "No Nodes", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int totalFiles = dicomFilePaths.Count;
                int successfulSends = 0;

                List<DicomFile> dicomFiles = new();

                // Load all DICOM files first (you could do this in parallel as well if needed)
                foreach (var path in dicomFilePaths)
                {
                    try
                    {
                        var file = await DicomFile.OpenAsync(path);
                        dicomFiles.Add(file);
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(ApplicationBase.LogLevel.Error, $"Failed to load DICOM file {path}: {ex.Message}");
                    }
                }

                // Send to each node
                foreach (var node in nodes)
                {
                    if (string.IsNullOrWhiteSpace(node.AETitle) || string.IsNullOrWhiteSpace(node.Host) || node.Port <= 0)
                    {
                        Platform.Log(ApplicationBase.LogLevel.Info,
                            $"Invalid routing node: {node.AETitle} at {node.Host}:{node.Port}");
                        continue;
                    }

                    var client = DicomClientFactory.Create(node.Host, node.Port, false, GeneralSettings.Default.LocalAETitle, node.AETitle);
                    foreach (var file in dicomFiles)
                    {
                        UpdateStatus($"📤 Sending file {totalFiles} to {node.AETitle}");

                        var request = new DicomCStoreRequest(file);
                        request.OnResponseReceived += (req, resp) =>
                        {
                            if (resp.Status == DicomStatus.Success)
                            {
                                Interlocked.Increment(ref successfulSends);
                            }
                        };

                        await client.AddRequestAsync(request);
                    }
                    try
                    {
                        await client.SendAsync();
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(ApplicationBase.LogLevel.Error,
                            $"Routing to {node.AETitle} failed:\n{ex.Message}");
                    }
                }

                int totalSendAttempts = dicomFiles.Count * nodes.Count;

                if (successfulSends == 0)
                {
                    UpdateStatus("❌ Failed to send any files.");
                    MessageBox.Show("❌ Failed to send any files.", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (successfulSends == totalSendAttempts)
                {
                    UpdateStatus("✅ Successfully sent all files.");
                    MessageBox.Show("✅ Successfully sent all files.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    UpdateStatus("⚠️ Partially sent some files.");
                    MessageBox.Show("⚠️ Partially sent some files.", "Partial Success", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            finally
            {
                btnSend.IsEnabled = true;
            }


        }
        private void UpdateStatus(string message)
        {
            Dispatcher.Invoke(() =>
            {
                lblResult.Content = message;
            });
        }
        private List<RouteNode> GetRouteNodes()
        {
            var list = new List<RouteNode>();
            if (cmbNodes.SelectedItem is Node node)
            {
                list.Add(new RouteNode()
                {
                    AETitle = node.AETitle,
                    Port = node.Port,
                    Host = node.Host
                });
            }
            return list;
        }
    }
}
