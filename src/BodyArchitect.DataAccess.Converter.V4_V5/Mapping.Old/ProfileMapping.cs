using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Shared;
using FluentNHibernate.Mapping;
using BodyArchitect.Model.Old;

namespace BodyArchitect.DataAccess.Converter.V4_V5.Mappings.Old
{
    public class PictureMapping : ComponentMap<Picture>
    {
        public PictureMapping()
        {
            Map(x => x.PictureId);
            Map(x => x.Hash);
        }
    }

    public class ProfilePrivacyMapping : ComponentMap<ProfilePrivacy>
    {
        public ProfilePrivacyMapping()
        {
            Map(x => x.CalendarView).CustomType<Privacy>().Not.Nullable();
            Map(x => x.Sizes).CustomType<Privacy>().Not.Nullable();
            Map(x => x.Friends).CustomType<Privacy>().Not.Nullable();
            Map(x => x.BirthdayDate).CustomType<Privacy>().Not.Nullable();
            Map(x => x.Searchable).Not.Nullable();
        }
    }
    public class ProfileMapping : ClassMap<Profile>
    {
        public ProfileMapping()
        {
            this.LazyLoad();
            Id(x => x.Id);
            Map(x => x.PreviousClientInstanceId);
            Map(x => x.ActivationId).Length(40);
            Map(x => x.UserName).Length(Constants.NameColumnLength).Not.Nullable().Unique();
            Map(x => x.Password).Length(100).Nullable();
            Map(x => x.Gender).CustomType<Gender>().Nullable();
            Map(x => x.Birthday).Not.Nullable();
            Map(x => x.CountryId).Not.Nullable();
            Map(x => x.Role).CustomType<Role>().Not.Nullable();
            Map(x => x.AboutInformation).Nullable().CustomType("StringClob");
            Component(x => x.Picture);
            Component(x => x.Privacy);
            Map(x => x.Email).Length(255).Not.Nullable().Unique();
            Map(x => x.IsDeleted).Not.Nullable();
            Map(x => x.CreationDate).Not.Nullable().Access.CamelCaseField();
            Version(x => x.Version);
            HasManyToMany(x => x.FavoriteWorkoutPlans).LazyLoad();
            HasMany(x => x.MyExercises).ReadOnly().Inverse().LazyLoad();
            HasMany(x => x.MyWorkoutPlans).ReadOnly().Inverse().LazyLoad();
            HasManyToMany(x => x.Friends).Table("FriendsForProfile").ParentKeyColumn("parent_profile_id").ChildKeyColumn("child_profile_id").LazyLoad();
            HasManyToMany(x => x.FavoriteUsers).Table("favoriteuserstofavoriteusers").ParentKeyColumn("parent_profile_id").ChildKeyColumn("child_profile_id").LazyLoad();

            References(x => x.Wymiary).LazyLoad().Cascade.All();
            References(x => x.Statistics).LazyLoad().Cascade.Delete();
            References(x => x.Settings).LazyLoad().Cascade.All();
        }
    }
}
