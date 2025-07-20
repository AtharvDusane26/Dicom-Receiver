using DICOMReceiver.Models.Dicom;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FellowOakDicom.Network.Client;
using FellowOakDicom.Network;
using DICOMReceiver.Models;
using FellowOakDicom;
using DICOMReceiver.Models.Entities;

namespace DICOMReceiver.View
{
    public partial class DicomReceiverControl : UserControl
    {
        public ObservableCollection<Node> Nodes { get; set; }
        private DicomReceiverServerManager _serverManager;
        private ILogger _logger;

        public DicomReceiverControl()
        {
            InitializeComponent();
            GetNodesFromDatabase();
            dataGridNodes.ItemsSource = Nodes;
            _logger = LoggerFactory.Create(builder => { }).CreateLogger("DICOM");
            _serverManager = new DicomReceiverServerManager();
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
        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAETitle.Text) ||
                string.IsNullOrWhiteSpace(txtHost.Text) ||
                !int.TryParse(txtPort.Text, out int port))
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
            Nodes.Add(node);
            new DBHandler().AddNode(new Models.Entities.Nodes
            {
                AETitle = node.AETitle,
                Host = node.Host,
                Port = node.Port
            });
            txtAETitle.Clear();
            txtHost.Clear();
            txtPort.Clear();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _serverManager.Start(104); // Port is hardcoded; can be configurable
                ApplicationBase.Platform.Log(ApplicationBase.LogLevel.Info, "DICOM Server Started.");
                MessageBox.Show("DICOM Server Started.");
            }
            catch (Exception ex)
            {
                ApplicationBase.Platform.Log(ApplicationBase.LogLevel.Error, $"Failed to start DICOM server: {ex.Message}");
                MessageBox.Show($"Failed to start DICOM server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _serverManager.Stop();
                ApplicationBase.Platform.Log(ApplicationBase.LogLevel.Info, "DICOM Server Stopped.");
                MessageBox.Show("DICOM Server Stopped.");
            }
            catch (Exception ex)
            {
                ApplicationBase.Platform.Log(ApplicationBase.LogLevel.Error, $"Failed to stop DICOM server: {ex.Message}");
                MessageBox.Show($"Failed to stop DICOM server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void EchoTest_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridNodes.SelectedItem is Node node)
            {
                try
                {
                    var client = DicomClientFactory.Create(node.Host, node.Port, false, "Atharv", node.AETitle);
                    var request = new DicomCEchoRequest();

                    bool success = false;
                    request.OnResponseReceived += (req, resp) =>
                    {
                        success = resp.Status == DicomStatus.Success;
                    };

                    await client.AddRequestAsync(request);
                    await client.SendAsync();

                    MessageBox.Show(success
                        ? $"Echo succeeded for {node.AETitle} at {node.Host}:{node.Port}."
                        : $"Echo failed for {node.AETitle} at {node.Host}:{node.Port}.",
                        "C-ECHO Result", MessageBoxButton.OK, success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Echo failed: {ex.Message}", "C-ECHO Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a node from the list.");
            }
        }


        private async void AutoRoute_Click(object sender, RoutedEventArgs e)
        {
            // Let user choose a DICOM file
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "DICOM files (*.dcm)|*.dcm",
                Title = "Select DICOM file to auto-route"
            };

            if (dialog.ShowDialog() != true)
            {
                MessageBox.Show("No DICOM file selected.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string dicomFilePath = dialog.FileName;

            // Load selected DICOM file
            DicomFile dicomFile;
            try
            {
                dicomFile = await DicomFile.OpenAsync(dicomFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load DICOM file:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // List of route nodes
            var nodes = GetRouteNodes();

            foreach (var node in nodes)
            {
                try
                {
                    var client = DicomClientFactory.Create(node.Host, node.Port, false, "MYAE", node.AETitle);
                    var request = new DicomCStoreRequest(dicomFile);

                    await client.AddRequestAsync(request);
                    bool success = false;
                    request.OnResponseReceived += (req, resp) =>
                    {
                        success = resp.Status == DicomStatus.Success;
                    };
                    await client.SendAsync();
                    MessageBox.Show(
                        $"Successfully auto-routed to {node.AETitle} at {node.Host}:{node.Port}.",
                        "Routing Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Routing to {node.AETitle} failed:\n{ex.Message}",
                        "Routing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private List<RouteNode> GetRouteNodes()
        {
            return new List<RouteNode>
            {
            new RouteNode { AETitle = "Atharv", Host = "localhost", Port = 104 },
            };
        }

        private void Worklist_Click(object sender, RoutedEventArgs e)
        {
            var window = new WorklistControl();
            window.Owner = Window.GetWindow(this);
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
