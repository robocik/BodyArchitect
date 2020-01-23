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
    public class StrengthTrainingEntryMapping : JoinedSubclassMapping<StrengthTrainingEntry>
    {
        public StrengthTrainingEntryMapping()
        {
            Key(x => x.Column("EntryObject_id"));

            //TODO:Change UTC
            Property(b => b.EndTime, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Mood, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.StartTime, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.TrainingPlanItemId, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.TrainingPlanId, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Intensity, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.MyPlace, map =>
            {
                map.Column("MyPlace_id");
                map.NotNullable(true);
            });

            //Map(x => x.EndTime).Nullable();
            //Map(x => x.StartTime).Nullable();
            //Map(x => x.TrainingPlanItemId).Nullable();
            //Map(x => x.TrainingPlanId).Nullable();
            //Map(x => x.Intensity).CustomType<Intensity>().Not.Nullable();
            Set(x => x.Entries, v =>
            {
                //v.Table("UslugaPrice");
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Key(c=>c.Column("StrengthTrainingEntry_id"));
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.NoLazy);
                v.OrderBy(x=>x.Position);
                v.BatchSize(Constants.DefaultBatchSize);
                
            }, h => h.OneToMany());
            //HasMany(x => x.Entries).Cascade.AllDeleteOrphan().Inverse().Not.LazyLoad().Fetch.Subselect().BatchSize(20).OrderBy("Position ASC");
        }
    }
}
