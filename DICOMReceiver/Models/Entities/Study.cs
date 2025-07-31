using DBConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Entities
{
    public class Study : BaseIdentifier
    {
        public virtual string StudyInstanceUID { get; set; }             // (0020,000D)
        public  virtual string StudyID { get; set; }                      // (0020,0010)
        public virtual string StudyDate { get; set; }                    // (0008,0020)
        public virtual string StudyTime { get; set; }                    // (0008,0030)
        public virtual string StudyDescription { get; set; }             // (0008,1030)
        public virtual string ReferringPhysicianName { get; set; }       // (0008,0090)
        public virtual string AccessionNumber { get; set; }              // (0008,0050)
        public virtual  string ModalitiesInStudy { get; set; }            // (0008,0061)
        public virtual string PatientId { get; set; }                       // Foreign key
    }

}
