using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ChampionshipCustomerMapping : ClassMapping<ChampionshipCustomer>
    {
        public ChampionshipCustomerMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(true);
                map.Column("Customer_id");
            });

            ManyToOne(x => x.Group, map =>
            {
                map.NotNullable(false);
                map.Column("ChampionshipGroup_id");
            });

            ManyToOne(x => x.StrengthTraining, map =>
            {
                map.NotNullable(false);
                map.Column("StrengthTraining_id");
            });

            Property(b => b.Type, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Total, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.TotalWilks, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Weight, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.WeightDateTime, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });
        }

    }
}
