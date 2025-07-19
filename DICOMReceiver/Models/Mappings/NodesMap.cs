using DICOMReceiver.Models.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Mappings
{
    public class NodesMap : ClassMap<Nodes>
    {
        public NodesMap()
        {
            Table("Nodes");

            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.AETitle).Not.Nullable();
            Map(x => x.Host).Not.Nullable();
            Map(x => x.Port).Not.Nullable();
        }
    }

}
