using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models
{
    using System;
    using System.IO;
    public interface IFileHandler
    {
        string SaveDicomFile(byte[] dicomData, string folderPath, string fileName);
        void DeleteDicomFile(string filePath);
    }
    public class FileHandler : IFileHandler
    {
        //public  void SaveDicomFile(byte[] dicomData, string folderPath, string fileName)
        //{
        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    string fullPath = Path.Combine(folderPath, fileName);

        //    File.WriteAllBytes(fullPath, dicomData);
        //}

        private bool DicomFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
        public void DeleteDicomFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public string SaveDicomFile(byte[] data, string folderPath, string fileName)
        {
            Directory.CreateDirectory(folderPath);
            string fullPath = Path.Combine(folderPath, fileName);
            File.WriteAllBytes(fullPath, data);
            return fullPath;
        }
    }

}
