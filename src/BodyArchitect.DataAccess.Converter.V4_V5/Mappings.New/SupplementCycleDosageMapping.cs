using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class SupplementCycleEntryMapping : ClassMapping<SupplementCycleEntry>
    {
        public SupplementCycleEntryMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Discriminator(x=>x.Type(NHibernateUtil.String));

            ManyToOne(x => x.Week, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(true);
                map.Column("SupplementCycleWeek_id");
                map.Lazy(LazyRelation.Proxy);
            });

            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });

            //Property(b => b.Type, y => y.NotNullable(true));

            Property(b => b.Repetitions, y => y.NotNullable(true));

            Property(b => b.TimeType, y => y.NotNullable(true));
        }
    }

    public class SupplementCycleMeasurementMapping : SubclassMapping<SupplementCycleMeasurement>
    {
        public SupplementCycleMeasurementMapping()
        {
            DiscriminatorValue("Measurements");

        }
    }

    public class SupplementCycleDosageMapping: SubclassMapping<SupplementCycleDosage>
    {
        public SupplementCycleDosageMapping()
        {
            DiscriminatorValue("Supplement");


            Property(b => b.Dosage, y => y.NotNullable(false));

            Property(b => b.DosageType, y => y.NotNullable(false));

            Property(b => b.DosageUnit, y => y.NotNullable(false));

            Property(b => b.Name, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.NameColumnLength);
            });

            ManyToOne(x => x.Supplement, map =>
            {
                map.Cascade(Cascade.None);
                map.NotNullable(false);
                map.Column("Supplement_id");
                map.Lazy(LazyRelation.Proxy);
            });
        }
    }
}
