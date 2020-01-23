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
    public class SupplementCycleWeekMapping : ClassMapping<SupplementCycleWeek>
    {
        public SupplementCycleWeekMapping()
        {
            Id(x => x.GlobalId, map =>
                                    {
                                        map.Generator(Generators.GuidComb);
                                    });

            ManyToOne(x => x.Definition, map =>
                                             {
                                                 map.Cascade(Cascade.None);
                                                 map.NotNullable(true);
                                                 map.Column("SupplementCycleDefinition_id");
                                                 map.Lazy(LazyRelation.Proxy);
                                             });

            Property(b => b.Name, y =>
                                      {
                                          y.NotNullable(false);
                                          y.Length(Constants.NameColumnLength);
                                      });

            Property(b => b.CycleWeekStart, y =>
                                                {
                                                    y.NotNullable(true);
                                                });

            Property(b => b.CycleWeekEnd, y =>
                                              {
                                                  y.NotNullable(true);
                                              });

            Property(b => b.IsRepetitable, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });

            Set(x => x.Dosages, v =>
                                    {
                                        v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                                        v.Inverse(true);
                                        v.Key(c => c.Column("SupplementCycleWeek_id"));
                                        v.Fetch(CollectionFetchMode.Subselect);
                                        v.Lazy(CollectionLazy.Lazy);
                                        v.OrderBy(x=>x.TimeType);
                                    }, h => h.OneToMany());

        }
    }
}
