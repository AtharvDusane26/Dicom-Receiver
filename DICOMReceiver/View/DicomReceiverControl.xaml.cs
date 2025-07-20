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
using System.IO;
using System.Xml.Linq;
using ApplicationBase;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxButton = System.Windows.MessageBoxButton;
using WindowStartupLocation = System.Windows.WindowStartupLocation;
using FluentNHibernate.Conventions.AcceptanceCriteria;

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
            //txtAETitle.Clear();
            //txtHost.Clear();
            //txtPort.Clear();
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
                        var client = DicomClientFactory.Create(host, port, false, "Atharv", aeTitle);
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



        private async void AutoRoute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnAutoRoute.IsEnabled = false;
                // Let user choose a DICOM file
                foreach (var dicomFilePath in Directory.GetFiles(DirectoryPath.ImageDirectory, "*", SearchOption.AllDirectories))
                {
                    Parallel.ForEach(Directory.GetFiles(DirectoryPath.ImageDirectory, "*", SearchOption.AllDirectories), dicomFilePath =>
                    {
                        if (!dicomFilePath.EndsWith(".dcm", StringComparison.OrdinalIgnoreCase))
                            return; // Skip non-DICOM files
                    });
                    DicomFile dicomFile;
                    try
                    {
                        dicomFile = await DicomFile.OpenAsync(dicomFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load DICOM file:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    // List of route nodes
                    var nodes = GetRouteNodes();

                        Parallel.ForEachAsync(nodes, async (node, cancellationToken) =>
                        {
                            if (string.IsNullOrWhiteSpace(node.AETitle) || string.IsNullOrWhiteSpace(node.Host) || node.Port <= 0)
                            {
                                Platform.Log(ApplicationBase.LogLevel.Info,
                                    $"Invalid routing node: {node.AETitle} at {node.Host}:{node.Port}");
                                return;
                            }
                            // Create DICOM client and send C-STORE request
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
                            }
                            catch (Exception ex)
                            {
                                Platform.Log(ApplicationBase.LogLevel.Error,
                                    $"Routing to {node.AETitle} failed:\n{ex.Message}");
                            }
                        });                                          
                }
                MessageBox.Show(
                $"Successfully auto-routed",
                               "Routing Success", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                btnAutoRoute.IsEnabled = true;
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

        private void dataGridNodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridNodes.SelectedItem is Node selectedNode)
            {
                txtAETitle.Text = selectedNode.AETitle;
                txtHost.Text = selectedNode.Host;
                txtPort.Text = selectedNode.Port.ToString();
            }
        }
    }
}
