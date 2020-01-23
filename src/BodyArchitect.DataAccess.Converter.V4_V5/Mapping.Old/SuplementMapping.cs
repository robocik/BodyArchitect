using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class SuplementMapping: ClassMap<Suplement>
    {
        public SuplementMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.Name).Length(Constants.NameColumnLength).Not.Nullable();
            Map(x => x.SuplementId).Not.Nullable().Unique();
            Map(x => x.ProfileId).Nullable();
            Map(x => x.Comment).CustomType("StringClob");
            Map(x => x.Url).Length(Constants.UrlLength);
        }
    }
}
