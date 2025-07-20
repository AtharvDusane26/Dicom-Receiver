using ApplicationBase;
using ApplicationBase.View;
using DBConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DICOMReceiver.Models
{
    public class DatabaseConnectionComponent : ApplicationComponent
    {
        private string _configName = "DbConfig.xml";
        private string _userId;
        private bool _password = false;
        public DatabaseConnectionComponent()
        {
        }
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                NotifyPropertyChanged(nameof(UserId));
            }
        }
        public bool Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChanged(nameof(Password));
            }
        }
        public string ConfigFilePath
        {
            get => System.IO.Path.Combine(GeneralSettings.Default.BaseDirectory, _configName);
        }
        public void LoadConfig()
        {
            if(File.Exists(System.IO.Path.Combine(GeneralSettings.Default.BaseDirectory, _configName)))
                return; 
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Configuration");
            doc.AppendChild(root);

            XmlElement userElement = doc.CreateElement("DatabaseFilePath");
            userElement.InnerText = System.IO.Path.Combine(GeneralSettings.Default.BaseDirectory, GeneralSettings.Default.DatabaseName);
            root.AppendChild(userElement);

            XmlElement passwordElement = doc.CreateElement("UsePassword");
            passwordElement.InnerText = Password ? "true" : "false";
            root.AppendChild(passwordElement);

            doc.Save(System.IO.Path.Combine(GeneralSettings.Default.BaseDirectory, _configName));
        }

    }
}
