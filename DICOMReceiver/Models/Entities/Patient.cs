using DBConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Entities
{
    public class Patient : BaseIdentifier
    {
        public int Id { get; set; }
        public string PatientID { get; set; }                    // (0010,0020)
        public string PatientName { get; set; }                  // (0010,0010)
        public string PatientSex { get; set; }                   // (0010,0040)
        public string PatientBirthDate { get; set; }             // (0010,0030)
        public string PatientAge { get; set; }                   // (0010,1010)
        public string IssuerOfPatientID { get; set; }            // (0010,0021)
    }

}
