using DICOMReceiver.Models.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Mappings
{
    public class SeriesMap : ClassMap<Series>
    {
        public SeriesMap()
        {
            Table("Series");

            Id(x => x.SeriesInstanceUID)
            .Column("SeriesID")
            .GeneratedBy.Assigned();

            Map(x => x.SeriesNumber);
            Map(x => x.SeriesDate);
            Map(x => x.SeriesTime);
            Map(x => x.Modality);
            Map(x => x.SeriesDescription);
            Map(x => x.BodyPartExamined);

            Map(x => x.StudyId)
                .Column("StudyId")
                .Not.Nullable();
        }
    }

}
