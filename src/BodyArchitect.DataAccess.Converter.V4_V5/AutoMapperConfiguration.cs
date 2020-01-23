using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    static class AutoMapperConfiguration
    {
        public static TRes Map<T, TRes>(this T obj)
        {
            return Mapper.Map<T, TRes>(obj);
        }

        public static TRes Map<TRes>(this object obj)
        {
            return Mapper.Map<TRes>(obj);
        }

        public static void Configure()
        {
            Mapper.CreateMap<float, decimal>().ConvertUsing(g=>g.ToSafe());
            Mapper.CreateMap<double, decimal>().ConvertUsing(g => g.ToSafe());
            Mapper.CreateMap<int, decimal>().ConvertUsing(g => g.ToSafe());

            //Mapper.CreateMap<Model.Old.Profile, Model.Profile>()
            //    .ForMember(x => x.Friends, l => l.Ignore())
            //    .ForMember(x => x.FavoriteUsers, l => l.Ignore())
            //    .ForMember(x => x.MyWorkoutPlans, l => l.Ignore())
            //    .ForMember(x => x.FavoriteWorkoutPlans, l => l.Ignore())
            //    .ForMember(x => x.FavoriteExercises, l => l.Ignore())
            //    .ForMember(x => x.MyExercises, l => l.Ignore());

            //Mapper.CreateMap<Model.Old.ProfilePrivacy, Model.ProfilePrivacy>();
            //Mapper.CreateMap<Model.Old.ProfileSettings, Model.ProfileSettings>()
            //    .ForMember(x => x.NotificationBlogCommentAdded, o => o.MapFrom(h => h.NotificationBlogCommentAdded ? ProfileNotification.Message : ProfileNotification.None))
            //    .ForMember(x => x.NotificationFriendChangedCalendar, o => o.MapFrom(h => h.NotificationFriendChangedCalendar ? ProfileNotification.Message : ProfileNotification.None))
            //    .ForMember(x => x.NotificationSocial, o => o.MapFrom(h => h.NotificationFriendChangedCalendar ? ProfileNotification.Message : ProfileNotification.None))
            //    .ForMember(x=>x.NotificationVoted,o=>o.MapFrom(h=>h.NotificationWorkoutPlanVoted?ProfileNotification.Message:ProfileNotification.None));
            //Mapper.CreateMap<Model.Old.ProfileStatistics, Model.ProfileStatistics>()
            //    .ForMember(x=>x.TrainingDayCommentsCount,b=>b.MapFrom(y=>y.BlogCommentsCount))
            //    .ForMember(x => x.MyTrainingDayCommentsCount, b => b.MapFrom(y => y.MyBlogCommentsCount));

            //Mapper.CreateMap<Model.Old.Picture, Model.Picture>();
            //Mapper.CreateMap<Model.Old.Message, Model.Message>()
            //    .ForMember(x => x.Sender, h => h.Ignore())
            //    .ForMember(x => x.Receiver, h => h.Ignore());
            //Mapper.CreateMap<Model.Old.Exercise, Model.Exercise>().ForMember(x=>x.Profile,h=>h.Ignore());
            //Mapper.CreateMap<Model.Old.RatingUserValue, Model.RatingUserValue>().ForMember(x=>x.ProfileId,k=>k.Ignore());
            //Mapper.CreateMap<Model.Old.Wymiary, Model.Wymiary>()
            //    .ForMember(x => x.Time,j =>j.MapFrom(y =>new BATime(){DateTime = y.DateTime,TimeType = y.IsNaCzczo ? TimeType.OnEmptyStomach : TimeType.NotSet}));

            //Mapper.CreateMap<Model.Old.Suplement, Model.Suplement>().ForMember(x => x.GlobalId, h => h.MapFrom(g => g.SuplementId)).ForMember(x => x.Profile, h => h.Ignore());

            //Mapper.CreateMap<Model.Old.TrainingDay, TrainingDay>().ForMember(x=>x.Profile,l=>l.Ignore());

            //Mapper.CreateMap<Model.Old.EntryObject, EntryObject>()
            //    .Include<Model.Old.StrengthTrainingEntry, StrengthTrainingEntry>()
            //    .Include<Model.Old.SuplementsEntry, SuplementsEntry>()
            //    .Include<Model.Old.A6WEntry, A6WEntry>()
            //    .Include<Model.Old.SizeEntry, SizeEntry>()
            //    .Include<Model.Old.BlogEntry, BlogEntry>();

            //Mapper.CreateMap<Model.Old.BlogComment, TrainingDayComment>()
            //    .ForMember(x=>x.Profile,g=>g.Ignore());

            //Mapper.CreateMap<Model.Old.StrengthTrainingEntry, StrengthTrainingEntry>();

            //Mapper.CreateMap<Model.Old.MyTraining, A6WTraining>()
            //    .ForMember(x=>x.EntryObjects,l=>l.Ignore())
            //    .ForMember(x => x.TrainingEnd, l => l.MapFrom(b=>b.TrainingEnd==TrainingEnd.NotEnded?Model.TrainingEnd.NotEnded:Model.TrainingEnd.Complete))
            //    .ForMember(x => x.Profile, l => l.Ignore());

            //Mapper.CreateMap<Model.Old.FriendInvitation, FriendInvitation>()
            //    .ForMember(x => x.Inviter, k => k.Ignore()).ForMember(x => x.Invited, k => k.Ignore());

            //Mapper.CreateMap<Model.Old.SuplementsEntry, SuplementsEntry>().AfterMap((s, d) =>
            //{
            //    int i = 0;
            //    foreach (var c in d.Items)
            //    {
            //        c.Position = i;
            //        i++;
            //    }
            //});

            //Mapper.CreateMap<Model.Old.BlogEntry, BlogEntry>();

            //Mapper.CreateMap<Model.Old.WP7PushNotification, WP7PushNotification>().ForMember(x=>x.ProfileId,k=>k.Ignore());

            //Mapper.CreateMap<Model.Old.SuplementItem, SuplementItem>().ForMember(x => x.Time, j => j.MapFrom(y => new BATime() { DateTime = y.Time, TimeType = TimeType.NotSet })); ;

            //Mapper.CreateMap<Model.Old.A6WEntry, A6WEntry>().ForMember(x=>x.MyTraining,h=>h.Ignore());

            //Mapper.CreateMap<Model.Old.SizeEntry, SizeEntry>();

            //Mapper.CreateMap<Model.Old.LoginData, LoginData>().ForMember(x => x.ProfileId, h => h.Ignore());

            Mapper.CreateMap<StrengthTrainingItem, StrengthTrainingItem>();
            //Mapper.CreateMap<Model.Old.StrengthTrainingItem, StrengthTrainingItem>();
            //Mapper.CreateMap<Model.Old.Serie, Serie>();
        }
    }
}
