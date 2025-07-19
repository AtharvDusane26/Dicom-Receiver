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
        public int Id { get; set; }
        public string StudyInstanceUID { get; set; }             // (0020,000D)
        public string StudyID { get; set; }                      // (0020,0010)
        public string StudyDate { get; set; }                    // (0008,0020)
        public string StudyTime { get; set; }                    // (0008,0030)
        public string StudyDescription { get; set; }             // (0008,1030)
        public string ReferringPhysicianName { get; set; }       // (0008,0090)
        public string AccessionNumber { get; set; }              // (0008,0050)
        public string ModalitiesInStudy { get; set; }            // (0008,0061)
        public int PatientId { get; set; }                       // Foreign key
    }

}
