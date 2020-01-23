using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class SizeEntryMapping : SubclassMap<SizeEntry>
    {
        public SizeEntryMapping()
        {
            this.Not.LazyLoad();
            References(x => x.Wymiary).Not.LazyLoad().Cascade.All().Fetch.Join();
        }
    }
}
