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
    public class TrainingDayMapping : ClassMapping<TrainingDay>
    {
        public TrainingDayMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.LastCommentDate, y =>
            {
                y.NotNullable(false);

            });
            Property(b => b.AllowComments, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.CommentsCount, y =>
            {
                y.Formula("(select count(*) from TrainingDayComment where TrainingDayComment.TrainingDay_id = GlobalId)");
            });
            Set(x => x.Comments, v =>
            {
                v.Cascade(Cascade.All);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Subselect);
                v.Key(g => g.Column("TrainingDay_id"));
                v.Lazy(CollectionLazy.Lazy);
                v.Mutable(false);//ReadOnly???
                v.BatchSize(Constants.DefaultBatchSize);
            }, h => h.OneToMany());

            Property(b => b.TrainingDate, y =>
            {//TODO:CHange UTC ?
                y.NotNullable(true);
                y.Index("IdxTrainingDay_TrainingDate");
                y.Type(NHibernateUtil.Date);//TODO: Check this
            });
            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });
            ManyToOne(x => x.Customer, map =>
            {
                map.Lazy(LazyRelation.Proxy);
                map.NotNullable(false);
                map.Column("Customer_id");
            });
            ManyToOne(x => x.Profile, map =>
            {
                map.Lazy(LazyRelation.Proxy);
                map.NotNullable(true);
                map.Column("Profile_id");
            });

            Set(x => x.Objects, v =>
            {
                //v.Table("UslugaPrice");
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.Lazy);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Key(k => k.Column("TrainingDay_id"));
            }, h => h.OneToMany());
            
            //Map(x => x.TrainingDate).Not.Nullable();
            //Map(x => x.Comment).CustomType("StringClob").Nullable();
            //References(x => x.Profile).Not.Nullable().LazyLoad();
            //HasMany(x => x.Objects).Cascade.AllDeleteOrphan().Inverse().Not.LazyLoad().Fetch.Select().BatchSize(Constants.DefaultBatchSize);
            //Version(x => x.Version);
        }
    }
}
