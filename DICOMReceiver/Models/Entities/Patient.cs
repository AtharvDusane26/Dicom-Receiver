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
        public virtual int Id { get; set; }
        public virtual string PatientID { get; set; }                    // (0010,0020)
        public virtual string PatientName { get; set; }                  // (0010,0010)
        public virtual string PatientSex { get; set; }                   // (0010,0040)
        public virtual string PatientBirthDate { get; set; }             // (0010,0030)
        public virtual string PatientAge { get; set; }                   // (0010,1010)
        public virtual string IssuerOfPatientID { get; set; }            // (0010,0021)
    }

}
