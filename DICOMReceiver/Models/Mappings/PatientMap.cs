using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Mappings
{
    using DICOMReceiver.Models.Entities;
    using FluentNHibernate.Mapping;

    public class PatientMap : ClassMap<Patient>
    {
        public PatientMap()
        {
            Table("Patients");

            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.PatientID);
            Map(x => x.PatientName);
            Map(x => x.PatientSex);
            Map(x => x.PatientBirthDate);
            Map(x => x.PatientAge);
            Map(x => x.IssuerOfPatientID);
        }
    }

}
