using DICOMReceiver.Models.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Mappings
{
    public class StudyMap : ClassMap<Study>
    {
        public StudyMap()
        {
            Table("Studies");

            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.StudyInstanceUID);
            Map(x => x.StudyID);
            Map(x => x.StudyDate);
            Map(x => x.StudyTime);
            Map(x => x.StudyDescription);
            Map(x => x.ReferringPhysicianName);
            Map(x => x.AccessionNumber);
            Map(x => x.ModalitiesInStudy);

            References<Patient>(x => x.PatientId)
                .Column("PatientId")
                .Not.Nullable();
        }
    }

}
