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
    public class ChampionshipMapping : JoinedSubclassMapping<Championship>
    {
        public ChampionshipMapping()
        {
            Key(x => x.Column("ScheduleEntryBase_id"));

            Property(b => b.Name, y =>
            {
                y.NotNullable(true);
                y.Length(Constants.NameColumnLength);
            });

            Property(b => b.Comment, y =>
            {
                y.Type(NHibernateUtil.StringClob);
                y.NotNullable(false);
            });

            Property(b => b.ChampionshipType, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.TeamCount, y =>
            {
                y.NotNullable(true);
            });

            Set(x => x.Customers, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("Championship_id"));
            }, h => h.OneToMany());

            Set(x => x.Results, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("Championship_id"));
            }, h => h.OneToMany());

            Set(x => x.Groups, v =>
            {
                v.Cascade(Cascade.All);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("Championship_id"));
            }, h => h.OneToMany());

            Set(x => x.Entries, v =>
            {
                v.Cascade(Cascade.All | Cascade.DeleteOrphans);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("Championship_id"));
            }, h => h.OneToMany());

            Set(x => x.Categories, v =>
            {
                v.Cascade(Cascade.All);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("Championship_id"));
            }, h => h.OneToMany());

        }
    }

    public class ChampionshipCategoryMapping : ClassMapping<ChampionshipCategory>
    {
        public ChampionshipCategoryMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.IsOfficial, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Type, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.IsAgeStrict, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Category, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Gender, y =>
            {
                y.NotNullable(true);
            });
        }
    }

    public class ChampionshipResultItemMapping : ClassMapping<ChampionshipResultItem>
    {
        public ChampionshipResultItemMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });

            Property(b => b.Position, y =>
            {
                y.NotNullable(true);
            });

            Property(b => b.Value, y =>
            {
                y.NotNullable(true);
            });

            ManyToOne(x => x.Group, map =>
            {
                map.NotNullable(false);
                map.Column("ChampionshipGroup_id");
            });

            ManyToOne(x => x.Customer, map =>
            {
                map.NotNullable(false);
                map.Column("ChampionshipCustomer_id");
            });

            ManyToOne(x => x.Category, map =>
            {
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Column("ChampionshipCategory_id");
            });
        }
    }

    public class ChampionshipGroupMapping : ClassMapping<ChampionshipGroup>
    {
        public ChampionshipGroupMapping()
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

            Set(x => x.Members, v =>
            {
                v.Cascade(Cascade.None);
                v.Inverse(true);
                v.Fetch(CollectionFetchMode.Subselect);
                v.BatchSize(Constants.DefaultBatchSize);
                v.Lazy(CollectionLazy.Lazy);
                v.Key(b => b.Column("ChampionshipGroup_id"));
            }, h => h.OneToMany());
        }
    }

    

    //public class ChampionshipTryMapping : ComponentMapping<ChampionshipTry>
    //{
    //    public ChampionshipTryMapping()
    //    {
    //        Property(b => b.Weight, y =>
    //        {
    //            y.NotNullable(true);

    //        });
    //        Property(b => b.Result, y => y.NotNullable(true));
    //    }
    //}

    
}
