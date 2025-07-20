using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver
{
    public sealed class DirectoryPath
    {
        private static DirectoryPath _directoryPath;
        private static string _imageDirectory = Path.Combine(GeneralSettings.Default.BaseDirectory, "Images");

        private DirectoryPath()
        {
            CreateDirectoryPath();
        }
        public static DirectoryPath Instance
        {
            get
            {
                if (_directoryPath == null)
                    _directoryPath = new DirectoryPath();
                return _directoryPath;
            }
        }
        public static string ImageDirectory
        {
            get => _imageDirectory;
            private set => _imageDirectory = value;
        }
        private static void CreateDirectoryPath()
        {
            if (!String.IsNullOrWhiteSpace(ImageDirectory) && !Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
        }
    }
}
