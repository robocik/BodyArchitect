using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ChampionshipEntryMapping : ClassMapping<ChampionshipEntry>
    {
        public ChampionshipEntryMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(true);
                map.Column("Customer_id");
                map.Cascade(Cascade.Persist);
            });

            Component(x => x.Try1, v =>
            {
                v.Property(b => b.Weight, y =>
                {
                    y.Column("Try1_Weight");
                    y.NotNullable(true);

                });
                v.Property(b => b.Result, y =>
                {
                    y.NotNullable(true);
                    y.Column("Try1_WeightDate");
                });
            });

            Component(x => x.Try2, v =>
            {
                v.Property(b => b.Weight, y =>
                {
                    y.Column("Try2_Weight");
                    y.NotNullable(true);

                });
                v.Property(b => b.Result, y =>
                {
                    y.NotNullable(true);
                    y.Column("Try2_WeightDate");
                });
            });

            Component(x => x.Try3, v =>
            {
                v.Property(b => b.Weight, y =>
                {
                    y.Column("Try3_Weight");
                    y.NotNullable(true);

                });
                v.Property(b => b.Result, y =>
                {
                    y.NotNullable(true);
                    y.Column("Try3_WeightDate");
                });
            });


            ManyToOne(x => x.Exercise, map =>
            {
                map.NotNullable(true);
                map.Column("Exercise_id");
            });

            Property(b => b.Max, y => y.NotNullable(true));

            Property(b => b.Wilks, y => y.NotNullable(true));
        }


    }
}
