using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class TrainingPlanMapping : ClassMapping<TrainingPlan>
    {
        public TrainingPlanMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));

            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.Author, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.Url, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.UrlLength);
            });
            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.RestSeconds, y =>
            {
                y.NotNullable(true);

            });
            ManyToOne(x => x.Profile, g =>
            {
                g.NotNullable(false);
                g.Column("Profile_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });
            Property(b => b.Rating, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Language, y =>
            {
                y.NotNullable(true);
                y.Length(10);
            });
            Property(b => b.PublishDate, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);

            });
            Property(b => b.TrainingType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Purpose, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Status, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Difficult, y =>
            {
                y.NotNullable(true);
            });
            Set(x => x.Days, v =>
            {
                v.OrderBy(x => x.Position);
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Select);
                v.Lazy(CollectionLazy.Lazy);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Key(k => k.Column("TrainingPlan_id"));
            }, h => h.OneToMany());
        }
    }

    public class TrainingPlanSerieMapping : ClassMapping<TrainingPlanSerie>
    {
        public TrainingPlanSerieMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.IsSuperSlow, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.IsRestPause, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Position, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.RepetitionNumberMin, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.RepetitionNumberMax, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);

            });
            Property(b => b.RepetitionsType, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.DropSet, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.Entry, g =>
            {
                g.NotNullable(true);
                g.Column("TrainingPlanEntry_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });
        }
    }

    public class TrainingPlanEntryMapping : ClassMapping<TrainingPlanEntry>
    {
        public TrainingPlanEntryMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.RestSeconds, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.DoneWay, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.Position, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.GroupName, y =>
            {
                y.NotNullable(false);
                y.Length(Constants.NameColumnLength);
            });
            Property(b => b.Comment, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);

            });

            ManyToOne(x => x.Exercise, g =>
            {
                g.NotNullable(true);
                g.Column("Exercise_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Day, g =>
            {
                g.NotNullable(true);
                g.Column("TrainingPlanDay_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });

            Set(x => x.Sets, v =>
            {
                v.OrderBy(x => x.Position);
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Select);
                v.Lazy(CollectionLazy.Lazy);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Key(k => k.Column("TrainingPlanEntry_id"));
            }, h => h.OneToMany());
        }
    }

    public class TrainingPlanDayMapping : ClassMapping<TrainingPlanDay>
    {
        public TrainingPlanDayMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });
            
            Property(b => b.Position, y =>
            {
                y.NotNullable(false);
            });

            ManyToOne(x => x.TrainingPlan, g =>
            {
                g.NotNullable(true);
                g.Column("TrainingPlan_id");
                g.Lazy(LazyRelation.Proxy);
                g.Cascade(Cascade.None);
            });

            Set(x => x.Entries, v =>
            {
                v.OrderBy(x => x.Position);
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Select);
                v.Lazy(CollectionLazy.Lazy);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Key(k => k.Column("TrainingPlanDay_id"));
            }, h => h.OneToMany());
        }
    }
}
