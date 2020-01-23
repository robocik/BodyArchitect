using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class SuplementItemMapping:ClassMap<SuplementItem>
    {
        public SuplementItemMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.DosageType).CustomType<DosageType>().Not.Nullable();
            Map(x => x.Dosage).Not.Nullable();
            Map(x => x.SuplementId).Not.Nullable();
            Map(x => x.Time).Not.Nullable();
            References(x => x.SuplementsEntry);
            Map(x => x.Name).Nullable().Length(Constants.NameColumnLength);
            Map(x => x.Comment).CustomType("StringClob").Nullable();

        }

    }
}
