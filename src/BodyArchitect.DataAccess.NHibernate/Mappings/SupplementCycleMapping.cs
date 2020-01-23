using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class SupplementCycleMapping : UnionSubclassMapping<SupplementCycle>
    {
        public SupplementCycleMapping()
        {
            Property(x => x.TrainingDays, map =>
            {
                map.NotNullable(false);
                map.Length(20);
            });

            Property(x => x.Weight, map =>
            {
                map.NotNullable(true);
            });

            Property(x => x.TotalWeeks, map =>
            {
                map.NotNullable(true);
            });

            ManyToOne(x => x.SupplementsCycleDefinition, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(true);
                map.Lazy(LazyRelation.Proxy);
                map.Column("SupplementsCycleDefinition_id");
            });
        }
    }
}
