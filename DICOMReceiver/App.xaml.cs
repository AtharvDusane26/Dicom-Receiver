using DBConfig;
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
            var filePath = @"D:\Projects\Working Project\Dicom Receiver\DICOMReceiver\Configuration.xml";
            var manager = new DataBaseManager();
            manager.Start(filePath, null);
        }
    }
}
