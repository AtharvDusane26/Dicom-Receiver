using DBConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Entities
{
    public class Series : BaseIdentifier
    {
        public int Id { get; set; }
        public string SeriesInstanceUID { get; set; }            // (0020,000E)
        public string SeriesNumber { get; set; }                 // (0020,0011)
        public string SeriesDate { get; set; }                   // (0008,0021)
        public string SeriesTime { get; set; }                   // (0008,0031)
        public string Modality { get; set; }                     // (0008,0060)
        public string SeriesDescription { get; set; }            // (0008,103E)
        public string BodyPartExamined { get; set; }             // (0018,0015)
        public int StudyId { get; set; }                         // Foreign key
    }

}
