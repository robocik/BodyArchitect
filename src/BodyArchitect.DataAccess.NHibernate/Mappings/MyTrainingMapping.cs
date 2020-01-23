using System;
using BodyArchitect.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace BodyArchitect.DataAccess.NHibernate.Mappings
{
    public class A6WTrainingMapping : UnionSubclassMapping<A6WTraining>
    {
        public A6WTrainingMapping()
        {

        }
    }

    public class MyTrainingMapping : ClassMapping<MyTraining>
    {
        public MyTrainingMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.StartDate, y =>
            {
                y.NotNullable(true);
                
            });
            Property(b => b.EndDate, y =>
            {
                y.NotNullable(false);
                
            });
            Property(b => b.RemindBefore, y =>
            {
                y.NotNullable(false);
            });
            //Property(b => b.TypeId, y =>
            //{
            //    y.NotNullable(true);
            //});
            ManyToOne(x => x.Profile, map =>
            {
                map.NotNullable(true);
                map.Column("Profile_id");
            });
            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(false);
                map.Column("Customer_id");
            });
            Property(b => b.TrainingEnd, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.PercentageCompleted, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });

            Set(x => x.EntryObjects, v =>
            {
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                //v.BatchSize(Constants.DefaultBatchSize);
                //v.Fetch(CollectionFetchMode.Subselect);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("MyTraining_id"));
            }, h => h.OneToMany());
        }
    }
}
