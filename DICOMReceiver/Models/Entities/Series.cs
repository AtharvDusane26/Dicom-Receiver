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
        public virtual string SeriesInstanceUID { get; set; }            // (0020,000E)
        public virtual string SeriesNumber { get; set; }                 // (0020,0011)
        public virtual string SeriesDate { get; set; }                   // (0008,0021)
        public virtual string SeriesTime { get; set; }                   // (0008,0031)
        public virtual string Modality { get; set; }                     // (0008,0060)
        public virtual string SeriesDescription { get; set; }            // (0008,103E)
        public virtual string BodyPartExamined { get; set; }             // (0018,0015)
        public virtual string StudyId { get; set; }                         // Foreign key
    }

}
