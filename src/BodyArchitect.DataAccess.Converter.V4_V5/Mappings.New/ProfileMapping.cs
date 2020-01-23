using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;
using BodyArchitect.Model;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings
{
    public class PictureMapping : ComponentMapping<Picture>
    {
        public PictureMapping()
        {
            Property(b => b.PictureId, y => y.NotNullable(false));
            Property(b => b.Hash, y =>
                                      {
                                          y.NotNullable(false);
                                          y.Column("HashValue");
                                      });
        }
    }

    public class LicenceInfoMapping : ComponentMapping<LicenceInfo>
    {
        public LicenceInfoMapping()
        {
            Property(b => b.BAPoints, y => y.NotNullable(true));
            Property(b => b.AccountType, y => y.NotNullable(true));
            Property(b => b.LastPointOperationDate, y => y.NotNullable(true));
        }
    }

    public class ProfilePrivacyMapping : ComponentMapping<ProfilePrivacy>
    {
        public ProfilePrivacyMapping()
        {
            Property(b => b.CalendarView, y => y.NotNullable(true));
            Property(b => b.Sizes, y => y.NotNullable(true));
            Property(b => b.Friends, y => y.NotNullable(true));
            Property(b => b.BirthdayDate, y => y.NotNullable(true));
            Property(b => b.Searchable, y => y.NotNullable(true));
        }
    }
    public class ProfileMapping : ClassMapping<Profile>
    {
        public ProfileMapping()
        {
            Id(x => x.GlobalId, map =>
            {
                map.Generator(Generators.GuidComb);
            });
            Property(b => b.PreviousClientInstanceId, y => y.NotNullable(false));
            Property(b => b.PreviousClientInstanceId, y => y.Length(40));
            Property(b => b.UserName,y =>
            {
                y.Length(Constants.NameColumnLength);
                y.NotNullable(true);
                y.Unique(true);
            });
            Property(b => b.Password, y =>
            {
                y.Length(100);
                y.NotNullable(false);
            });
            Property(b => b.Gender, y =>
            {
                y.NotNullable(false);
            });
            Property(b => b.Birthday, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.CountryId, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.AboutInformation, y =>
            {
                y.NotNullable(false);
                y.Type(NHibernateUtil.StringClob);
            });
            
            //Component(x => x.Licence,j=>
            //    {
            //        j.Property(b=>b.LicenceType,map=>map.NotNullable(true));
            //        j.Property(b => b.ImportedDate, map => map.NotNullable(false));
            //    });
            Property(b => b.Email, y =>
            {
                y.Length(255);
                y.NotNullable(true);
                y.Unique(true);
            });
            Property(b => b.IsDeleted, y =>
            {
                y.NotNullable(true);
            });
            Property(b => b.CreationDate, y =>
            {
                y.NotNullable(true);
                y.Access(Accessor.Field);
                
            });
            //Map(x => x.CreationDate).Not.Nullable().Access.CamelCaseField();
            Version(x => x.Version, map => map.Generated(VersionGeneration.Never));


            //References(x => x.Statistics).LazyLoad().Cascade.Delete();
            ManyToOne(x => x.Statistics, map =>
            {
                map.Cascade(Cascade.Remove | Cascade.DeleteOrphans);
                map.NotNullable(false);
                map.Column("Statistics_id");
            });
            ManyToOne(x => x.Wymiary, map =>
            {
                map.Cascade(Cascade.All);
                map.NotNullable(false);
                map.Column("Wymiary_id");
            });

            ManyToOne(x => x.DataInfo, map =>
            {
                map.Cascade(Cascade.All);
                map.NotNullable(false);
                map.Column("DataInfo_id");
            });

            Set(x => x.FavoriteSupplementCycleDefinitions, v =>
            {
                v.Table("supplementcycledefinitiontoprofile");
                v.Lazy(CollectionLazy.Lazy);
                v.Key(g => g.Column("profile_id"));
            }, h => h.ManyToMany(l => l.Column("SupplementCycleDefinition_id")));
            
            Set(x => x.FavoriteWorkoutPlans, v =>
            {
                v.Table("trainingplantoprofile");
                v.Lazy(CollectionLazy.Lazy);
                v.Key(g => g.Column("profile_id"));
            }, h => h.ManyToMany(l => l.Column("TrainingPlan_id")));

            Set(x => x.FavoriteExercises, v =>
            {
                v.Table("exercisetoprofile");
                v.Lazy(CollectionLazy.Lazy);
                v.Key(g => g.Column("profile_id"));
            }, h => h.ManyToMany(l => l.Column("Exercise_id")));

            Set(x => x.MyExercises, v =>
            {
                v.Inverse(true);
                v.Key(c => c.Column("Profile_id"));
                v.Mutable(true);
                v.Lazy(CollectionLazy.Lazy);
            }, h => h.OneToMany());

            Set(x => x.MyWorkoutPlans, v =>
            {
                v.Inverse(true);
                v.Key(c => c.Column("Profile_id"));
                v.Mutable(true);
                v.Lazy(CollectionLazy.Lazy);
            }, h => h.OneToMany());

            Set(x => x.Friends, v =>
            {
                v.Table("FriendsForProfile");
                v.Lazy(CollectionLazy.Lazy);
                v.Key(g => g.Column("parent_profile_id"));

            }, h => h.ManyToMany(l => l.Column("child_profile_id")));
            Set(x => x.FavoriteUsers, v =>
            {
                v.Table("favoriteuserstofavoriteusers");
                v.Lazy(CollectionLazy.Lazy);
                v.Key(g => g.Column("parent_profile_id"));

            }, h => h.ManyToMany(l => l.Column("child_profile_id")));

            ManyToOne(x => x.Settings, map =>
            {
                map.Cascade(Cascade.All);
                map.NotNullable(false);
                map.Column("Settings_id");
            });

            Component(x => x.Picture);
            Component(x => x.Privacy);
            Component(x => x.Licence);

            //HasMany(x => x.MyExercises).ReadOnly().Inverse().LazyLoad();
            //HasMany(x => x.MyWorkoutPlans).ReadOnly().Inverse().LazyLoad();
            //HasManyToMany(x => x.FavoriteWorkoutPlans).LazyLoad();
            //HasManyToMany(x => x.Friends).Table("FriendsForProfile").ParentKeyColumn("parent_profile_id").ChildKeyColumn("child_profile_id").LazyLoad();
            //HasManyToMany(x => x.FavoriteUsers).ParentKeyColumn("parent_profile_id").ChildKeyColumn("child_profile_id").LazyLoad();

            //References(x => x.Wymiary).LazyLoad().Cascade.All();
            //References(x => x.Settings).LazyLoad().Cascade.All();
        }
    }
}
