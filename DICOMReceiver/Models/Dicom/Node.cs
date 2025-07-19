using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Dicom
{
    public class Node 
    {
        public string AETitle { get; set; }      
        public string Host { get; set; }         
        public int Port { get; set; } 
    }

}
