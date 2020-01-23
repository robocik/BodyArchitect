using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class ExerciseProfileDataMapping: ClassMapping<ExerciseProfileData>
    {
        public ExerciseProfileDataMapping()
        {
            Id(x => x.GlobalId, map =>
                                    {
                                        map.Generator(Generators.GuidComb);
                                    });

            Property(x => x.MaxWeight, map =>
                                           {
                                               map.NotNullable(true);
                                           });
            Property(x => x.Repetitions, map =>
            {
                map.NotNullable(false);
            });
            Property(x => x.TrainingDate, map =>
                                              {
                                                  map.NotNullable(true);
                                              });

            ManyToOne(x => x.Profile, map =>
                                          {
                                              map.Column("Profile_id");
                                              map.NotNullable(true);
                                              map.Cascade(Cascade.None);
                                              map.Lazy(LazyRelation.Proxy);
                                          });

            ManyToOne(x => x.Customer, map =>
            {
                map.Column("Customer_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Lazy(LazyRelation.Proxy);
            });

            ManyToOne(x => x.Exercise, map =>
                                          {
                                              map.Column("Exercise_id");
                                              map.NotNullable(true);
                                              map.Cascade(Cascade.None);
                                              map.Lazy(LazyRelation.Proxy);
                                          });

            ManyToOne(x => x.Serie, map =>
                                          {
                                              map.Column("Serie_id");
                                              map.NotNullable(false);
                                              map.Cascade(Cascade.Detach);
                                              map.Lazy(LazyRelation.Proxy);
                                          });
        }
    }
}
