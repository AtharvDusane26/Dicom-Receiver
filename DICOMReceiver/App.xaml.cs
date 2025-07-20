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
            var component = new DatabaseConnectionComponent();
            component.LoadConfig();
            var manager = new DataBaseManager();
            manager.Start(component.ConfigFilePath, null);
            var window = new MainWindow();
            SetHost(window, ApplicationType.Window);
            Platform.Log(LogLevel.Info, "Application Started");
            window.ShowDialog();
        }
    }
}
