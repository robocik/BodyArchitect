using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using FluentNHibernate.Mapping;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class WP7TrialMapping:ClassMap<WP7Trial>
    {
        public WP7TrialMapping()
        {
            this.Not.LazyLoad();
            Id(x => x.DeviceId).Length(200).GeneratedBy.Assigned();
            Map(x => x.TrialStartedDate).Not.Nullable();
        }
    }
}
