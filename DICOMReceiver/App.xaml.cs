using ApplicationBase;
using DBConfig;
using DICOMReceiver.Models;
using FluentNHibernate.Mapping;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DICOMReceiver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var startup = new StartUp();
            startup.Start();
        }
    }
    internal class StartUp : ApplicationComponent
    {
        private Mutex _myMutex = null;
        public override void Start()
        {
            base.Start();
            if (String.IsNullOrWhiteSpace(GeneralSettings.Default.BaseDirectory))
            {
                var directoryCreator = new View.DirectoryCreatorControl();
                if (directoryCreator.ShowDialog() != true)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }
            if (IsAlreadyStarted())
            {
                MessageBox.Show("Application Already Starded", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Environment.Exit(0);
            }
            var component = new DatabaseConnectionComponent();
            component.LoadConfig();
            var manager = new DataBaseManager();
            manager.Start(component.ConfigFilePath, null);
            var window = new MainWindow();
            SetHost(window, ApplicationType.Window);
            Platform.Log(LogLevel.Info, "Application Started");
            window.ShowDialog();
        }
        private bool IsAlreadyStarted()
        {
            if (_myMutex == null)
                _myMutex = new Mutex(true, "F7A3C4B2-8D3F-4E29-AD8A-93D7F30CF2B6");
            return !_myMutex.WaitOne(0, false);
        }
    }

}
