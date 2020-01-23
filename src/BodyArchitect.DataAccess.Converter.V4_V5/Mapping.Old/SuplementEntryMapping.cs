using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class SuplementEntryMapping : SubclassMap<SuplementsEntry>
    {
        public SuplementEntryMapping()
        {
            this.Not.LazyLoad();
            HasMany(x => x.Items).Cascade.AllDeleteOrphan().OrderBy("Id").Inverse().LazyLoad();
        }
    }
}
