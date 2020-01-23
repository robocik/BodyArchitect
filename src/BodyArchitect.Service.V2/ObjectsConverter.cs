using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using NHibernate;
using AccountType = BodyArchitect.Model.AccountType;
using Privacy = BodyArchitect.Service.V2.Model.Privacy;
using Profile = BodyArchitect.Model.Profile;
using ProfileStatus = BodyArchitect.Model.ProfileStatus;
using ReportStatus = BodyArchitect.Service.V2.Model.ReportStatus;

namespace BodyArchitect.Service.V2
{
    ///// <summary>
    ///// Custom resolver for objects set to lazy loading in nhibernate
    ///// </summary>
    ///// <typeparam name="TSource">The type of domain object the lazy loaded property is on.</typeparam>
    ///// <typeparam name="TDestination">The type the domsin object resolves to</typeparam>
    //public class SoloLazyInitResolver : ValueResolver<object, object>
    //{
    //    string _propertyName;


    //    /// <summary>
    //    /// Resolves the object property to the specified type
    //    /// </summary>
    //    /// <param name="source">The domain object the property is on</param>
    //    /// <returns>A resovled object or null if the source object is disconnected from the nhibernate session</returns>
    //    protected override object ResolveCore(object source)
    //    {
    //        try
    //        {
    //            if (NHibernateUtil.IsPropertyInitialized(source, _propertyName))
    //            {
    //                return source.GetType().GetProperty(_propertyName).GetValue(source, null);
    //            }
    //            return null;
    //        }
    //        catch (NHibernate.LazyInitializationException)//should never be raised
    //        { }
    //        return null;
    //    }


    //    /// <summary>
    //    /// A delegate with the mapping instruction to invoke when resolving the lazy loaded property
    //    /// </summary>
    //    public delegate dynamic ResolveSoloItemDelegate(dynamic sourceItem);




    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="SoloLazyInitResolver<TSource, TDestination>"/> class.
    //    /// </summary>
    //    /// <param name="resolveSoloItem">The delegate with the mapping instruction to invoke when resolving the peoperty on the object</param>
    //    /// <param name="propertyName">Name of the lazy loaded property on the source domain object.</param>
    //    public SoloLazyInitResolver(string propertyName)
    //    {
    //        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("propertyName may not be null, empty, or whitespace");
    //        _propertyName = propertyName;
    //    }
    //}


    public static class ObjectsConverter
    {
        static bool configured;

        public static void Configure()
        {
            if (configured)
            {
                return;
            }
            Mapper.Initialize(x=>
                                  {
                                      x.ConstructServicesUsing(createServices);
                                  });
            configured = true;
            Mapper.CreateMap<MessageDTO, Message>()
                .ForMember(x => x.Receiver, op => op.Ignore())
                .ForMember(x => x.Sender, op => op.Ignore());
            Mapper.CreateMap<Message, MessageDTO>();

            Mapper.CreateMap<DataInfo, DataInfoDTO>();

            Mapper.CreateMap<Address, AddressDTO>();
            Mapper.CreateMap<AddressDTO, Address>();

            

            Mapper.CreateMap<ChampionshipTry, ChampionshipTryDTO>();
            Mapper.CreateMap<ChampionshipTryDTO, ChampionshipTry>();

            Mapper.CreateMap<ChampionshipCategory, ChampionshipCategoryDTO>();
            Mapper.CreateMap<ChampionshipCategoryDTO, ChampionshipCategory>();

            Mapper.CreateMap<ScheduleEntryBaseDTO, ScheduleEntryBase>()
                .Include<ScheduleEntryDTO, ScheduleEntry>()
                .Include<ScheduleChampionshipDTO, Championship>()
                .ForMember(dst => dst.MyPlace, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyPlace>>().FromMember(src => src.MyPlaceId))
                .ForMember(x => x.Reservations, l => l.Ignore());
            Mapper.CreateMap<ScheduleEntryBase, ScheduleEntryBaseDTO>()
                .Include<ScheduleEntry, ScheduleEntryDTO>()
                .Include<Championship, ScheduleChampionshipDTO>()
                .ForMember(x => x.RemindBefore, y => y.MapFrom(t => t.Reminder != null ? (t.Reminder.RemindBefore ?? (TimeSpan?)TimeSpan.Zero) : null))
                .ForMember(x => x.MyPlaceId, o => o.MapFrom(x => (x.MyPlace.GlobalId)));


            Mapper.CreateMap<ScheduleEntry, ScheduleEntryDTO>()
                .ForMember(x => x.ActivityId, y => y.MapFrom(t => t.Activity.GlobalId))
                .ForMember(x => x.CustomerGroupId, y => y.MapFrom(t => t.CustomerGroup.GlobalId));
            Mapper.CreateMap<ScheduleEntryDTO, ScheduleEntry>()
                .ForMember(dst => dst.Activity, _ => _.ResolveUsing<LoadingGuidEntityResolver<Activity>>().FromMember(src => src.ActivityId))
                .ForMember(dst => dst.CustomerGroup, _ => _.ResolveUsing<LoadingGuidEntityResolver<CustomerGroup>>().FromMember(src => src.CustomerGroupId))
                .ForMember(x => x.Reservations, l => l.Ignore());

            Mapper.CreateMap<ChampionshipResultItem, ChampionshipResultItemDTO>();

            Mapper.CreateMap<Championship, ScheduleChampionshipDTO>();
            Mapper.CreateMap<ScheduleChampionshipDTO, Championship>()
                .ForMember(dst => dst.MyPlace, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyPlace>>().FromMember(src => src.MyPlaceId))
                .ForMember(x => x.Reservations, l => l.Ignore())
                .ForMember(x => x.Results, l => l.Ignore());

            Mapper.CreateMap<Championship, ChampionshipDTO>()
                  .ForMember(x => x.MyPlaceId,l => l.MapFrom(b => (b.MyPlace != null ? b.MyPlace.GlobalId : (Guid?) null)))
                  .ForMember(x => x.RemindBefore,y =>y.MapFrom(t => t.Reminder != null ? (t.Reminder.RemindBefore ?? (TimeSpan?) TimeSpan.Zero) : null));
            Mapper.CreateMap<ChampionshipDTO, Championship>()
                .ForMember(dst => dst.MyPlace, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyPlace>>().FromMember(src => src.MyPlaceId))
                .ForMember(x => x.Reservations, l => l.Ignore())
                .ForMember(x => x.Results, l => l.Ignore());


            

            Mapper.CreateMap<ChampionshipEntry, ChampionshipEntryDTO>();
            Mapper.CreateMap<ChampionshipEntryDTO, ChampionshipEntry>().ForMember(dst => dst.Exercise, _ => _.ResolveUsing<LoadingGuidEntityResolver<Exercise>>().FromMember(src => src.Exercise != null ? (object)src.Exercise.GlobalId : null));

            Mapper.CreateMap<ChampionshipCustomer, ChampionshipCustomerDTO>().ForMember(x => x.CustomerId, o => o.MapFrom(x => (x.Customer != null ? x.Customer.GlobalId : (Guid?)null)));
            Mapper.CreateMap<ChampionshipCustomerDTO, ChampionshipCustomer>().ForMember(dst => dst.Customer, _ => _.ResolveUsing<LoadingGuidEntityResolver<Customer>>().FromMember(src => src.CustomerId)); 

            Mapper.CreateMap<ChampionshipGroup, ChampionshipGroupDTO>();
            Mapper.CreateMap<ChampionshipGroupDTO, ChampionshipGroup>();

            Mapper.CreateMap<MyPlace, MyPlaceLightDTO>().ForMember(x => x.ProfileId, o => o.MapFrom(x => (x.Profile != null ? x.Profile.GlobalId : (Guid?)null)));
            Mapper.CreateMap<MyPlaceLightDTO, MyPlace>();

            Mapper.CreateMap<MyPlace, MyPlaceDTO>().ForMember(x => x.ProfileId, o => o.MapFrom(x => (x.Profile != null ? x.Profile.GlobalId : (Guid?)null)));
            Mapper.CreateMap<MyPlaceDTO, MyPlace>();

            Mapper.CreateMap<Customer, CustomerDTO>()
                .ForMember(x => x.RemindBefore,y =>y.MapFrom(t => t.Reminder != null ? (t.Reminder.RemindBefore ?? (TimeSpan?) TimeSpan.Zero) : null))
                .ForMember(x => x.ProfileId,y =>y.MapFrom(t => t.Profile.GlobalId));
            Mapper.CreateMap<CustomerDTO, Customer>().ForMember(x=>x.ConnectedAccount,k=>k.Ignore());

            Mapper.CreateMap<CustomerGroup, CustomerGroupDTO>()
                .ForMember(x => x.DefaultActivityId, j => j.MapFrom(g => g.DefaultActivity.GlobalId))
                .ForMember(x => x.ProfileId, j => j.MapFrom(g => g.Profile.GlobalId));
            Mapper.CreateMap<CustomerGroupDTO, CustomerGroup>().ForMember(x=>x.Customers,h=>h.Ignore());

            Mapper.CreateMap<ExerciseProfileData, ExerciseRecordsReportResultItem>()
                 .ForMember(x => x.User, j => j.MapFrom(g => g.Profile))
                 .ForMember(x => x.SerieId, j => j.MapFrom(g => g.Serie.GlobalId))
                 .ForMember(x => x.CustomerId, j => j.MapFrom(g => g.Customer!=null?g.Customer.GlobalId:(Guid?)null));

            Mapper.CreateMap<Activity, ActivityDTO>()
                .ForMember(x => x.ProfileId, j => j.MapFrom(g => g.Profile.GlobalId));
            Mapper.CreateMap<ActivityDTO, Activity>();

            Mapper.CreateMap<ReminderItem, ReminderItemDTO>()
                .ForMember(x => x.ProfileId, j => j.MapFrom(g => g.Profile.GlobalId));
            Mapper.CreateMap<ReminderItemDTO, ReminderItem>();

            Mapper.CreateMap<ProfilePrivacyDTO, ProfilePrivacy>();
            Mapper.CreateMap<ProfilePrivacy, ProfilePrivacyDTO>();

            Mapper.CreateMap<PaymentBasketDTO, PaymentBasket>().ForMember(x => x.Payments, j => j.Ignore());
            Mapper.CreateMap<PaymentBasket, PaymentBasketDTO>()
                .ForMember(x=>x.CustomerId,j=>j.MapFrom(g=>g.Customer.GlobalId))
                .ForMember(x => x.ProfileId, j => j.MapFrom(g => g.Profile.GlobalId));

            Mapper.CreateMap<ProfileSettingsDTO, ProfileSettings>();
            Mapper.CreateMap<ProfileSettings, ProfileSettingsDTO>();

            Mapper.CreateMap<BATime, BATimeDTO>();
            Mapper.CreateMap<BATimeDTO, BATime>();

            Mapper.CreateMap<WeatherDTO, Weather>();
            Mapper.CreateMap<Weather, WeatherDTO>();

            Mapper.CreateMap<ProfileStatus, ProfileStatusDTO>();
            Mapper.CreateMap<ProfileStatusDTO, ProfileStatus>();

            Mapper.CreateMap<CustomerSettings, CustomerSettingsDTO>();
            Mapper.CreateMap<CustomerSettingsDTO, CustomerSettings>();

            Mapper.CreateMap<ProfileStatistics, ProfileStatisticsDTO>();

            Mapper.CreateMap<EntryObject, FeaturedEntryObjectDTO>()
                .ForMember(x => x.User, j => j.MapFrom(g => g.TrainingDay.Profile))
                .ForMember(x => x.DateTime,j =>j.MapFrom(g =>g.TrainingDay.TrainingDate));

            Mapper.CreateMap<ExerciseDTO, Exercise>().ForMember(x=>x.Profile,b=>b.Ignore());
            Mapper.CreateMap<Exercise, ExerciseDTO>().ForMember(x => x.Name, y => y.MapFrom(u => u.GetLocalizedName()))
                .ForMember(x => x.Shortcut, y => y.MapFrom(u => u.GetLocalizedShortcut()))
                .ForMember(x => x.Url, y => y.MapFrom(u => u.GetLocalizedUrl()))
                .ForMember(x => x.Description, y => y.MapFrom(u => u.GetLocalizedDescription()))
                .ForMember(x => x.ProfileId, y => y.Ignore());

            Mapper.CreateMap<Product, ProductDTO>()
                .ForMember(x=>x.CustomerId,m=>m.MapFrom(g=>g.Customer.GlobalId))
                .ForMember(x => x.IsPaid, h => h.MapFrom(u => u.Payment != null && u.Payment.GlobalId!=Guid.Empty))
                .Include<ScheduleEntryReservation, ScheduleEntryReservationDTO>();

            Mapper.CreateMap<PaymentBase, PaymentBaseDTO>()
                .Include<Payment, PaymentDTO>();

            Mapper.CreateMap<PaymentBaseDTO, PaymentBase>()
                .Include<PaymentDTO, Payment>()
                .ForMember(x => x.Product, g => g.Ignore());

            Mapper.CreateMap<PaymentDTO, Payment>().ForMember(x => x.Product, g => g.Ignore());
            Mapper.CreateMap<Payment, PaymentDTO>().ForMember(x => x.PaymentBasketId, m => m.MapFrom(h => h.PaymentBasket.GlobalId));

            Mapper.CreateMap<Product, ProductInfoDTO>()
                .ForMember(x => x.Product, m => m.MapFrom(h => h))
                .ForMember(y => y.Payment, m => m.MapFrom(h => h.Payment));

            Mapper.CreateMap<ScheduleEntryReservation, ScheduleEntryReservationDTO>().ForMember(x=>x.ScheduleEntryId,m=>m.MapFrom(h=>h.ScheduleEntry.GlobalId));

            Mapper.CreateMap<Exercise, ExerciseLightDTO>().ForMember(x => x.Name,
                                                                     y => y.MapFrom(u => u.GetLocalizedName()))
                .ForMember(x => x.Shortcut, y => y.MapFrom(u => u.GetLocalizedShortcut()));
            Mapper.CreateMap<ExerciseLightDTO, Exercise>();

            Mapper.CreateMap<PictureInfoDTO, Picture>();
            Mapper.CreateMap<Picture, PictureInfoDTO>();

            Mapper.CreateMap<BodyArchitect.Model.LicenceInfo, LicenceInfoDTO>();
            Mapper.CreateMap<LicenceInfoDTO, BodyArchitect.Model.LicenceInfo>();

            Mapper.CreateMap<Profile, ProfileDTO>();
            Mapper.CreateMap<Profile, UserDTO>().AfterMap((x, y) =>
                {
                    if(x.Licence.AccountType==AccountType.User)
                    {
                        y.Privacy.CalendarView = Privacy.Public;
                        y.Privacy.Sizes = Privacy.Public;
                    }
                });

            Mapper.CreateMap<Profile, UserSearchDTO>().ForMember(x=>x.IsOnline,g=>g.MapFrom(n=>InternalBodyArchitectService.SecurityManager.IsLogged(n.GlobalId)))
                .AfterMap((x, y) =>
                {
                    if (x.Licence.AccountType == AccountType.User)
                    {
                        y.Privacy.CalendarView = Privacy.Public;
                        y.Privacy.Sizes = Privacy.Public;
                    }
                });
            Mapper.CreateMap<ProfileDTO, Profile>()/*.ForMember(x=>x.FavoriteExercises,op=>op.Ignore())*/
                .ForMember(x => x.MyExercises, op => op.Ignore())
                .ForMember(x => x.MyWorkoutPlans, op => op.Ignore())
                .ForMember(x => x.FavoriteUsers, op => op.Ignore())
                .ForMember(x => x.FavoriteWorkoutPlans, op => op.Ignore())
                .ForMember(x => x.Settings, op => op.Ignore())
                .ForMember(x => x.Friends, op => op.Ignore())
            .ForMember(x => x.Licence, op => op.Ignore());
            Mapper.CreateMap<WymiaryDTO, Wymiary>();//.ForMember(x => x.Id, op => op.Ignore());
            Mapper.CreateMap<Wymiary, WymiaryDTO>();

            Mapper.CreateMap<TrainingDay, TrainingDayInfoDTO>()
                .ForMember(x => x.CustomerId, o => o.MapFrom(x => (x.Customer != null ? x.Customer.GlobalId : (Guid?)null)))
                .ForMember(x => x.ProfileId, o => o.MapFrom(x => (x.Profile != null ? x.Profile.GlobalId : (Guid?)null)));
            Mapper.CreateMap<TrainingDayInfoDTO, TrainingDay>()
                .ForMember(dst => dst.Customer, _ => _.ResolveUsing<LoadingGuidEntityResolver<Customer>>().FromMember(src => src.CustomerId));
            Mapper.CreateMap<TrainingDayDTO, TrainingDay>()
                .ForMember(dst => dst.Customer, _ => _.ResolveUsing<LoadingGuidEntityResolver<Customer>>().FromMember(src => src.CustomerId));

            Mapper.CreateMap<FriendInvitation, FriendInvitationDTO>().ForMember(x=>x.CreatedDateTime,t=>t.MapFrom(h=>h.CreateDate));

            Mapper.CreateMap<EntryObjectDTO, EntryObject>()
                .Include<StrengthTrainingEntryDTO, StrengthTrainingEntry>()
                .Include<SuplementsEntryDTO, SuplementsEntry>()
                .Include<A6WEntryDTO, A6WEntry>()
                .Include<SizeEntryDTO, SizeEntry>()
                .Include<BlogEntryDTO, BlogEntry>()
                .Include<GPSTrackerEntryDTO, GPSTrackerEntry>()
                .ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation,
                    _ =>_.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                     .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? (object)src.MyTraining.GlobalId : null));
            Mapper.CreateMap<EntryObject, EntryObjectDTO>()
                .ForMember(x => x.RemindBefore, y => y.MapFrom(t => t.Reminder != null ? (t.Reminder.RemindBefore ?? (TimeSpan?)TimeSpan.Zero) : null))
                .ForMember(x => x.ApplicationName, v => v.MapFrom(u => u.LoginData.ApiKey.ApplicationName))
                .ForMember(x => x.ReservationId, o => o.MapFrom(x => (x.Reservation != null ? x.Reservation.GlobalId : (Guid?)null)))
                .Include<StrengthTrainingEntry, StrengthTrainingEntryDTO>()
                .Include<SuplementsEntry, SuplementsEntryDTO>()
                .Include<A6WEntry, A6WEntryDTO>()
                .Include<SizeEntry, SizeEntryDTO>()
                .Include<GPSTrackerEntry, GPSTrackerEntryDTO>()
                .Include<BlogEntry, BlogEntryDTO>();

            Mapper.CreateMap<TrainingDay, TrainingDayDTO>()
                .ForMember(x => x.ProfileId, o => o.MapFrom(x => x.Profile.GlobalId))
                .ForMember(x => x.CustomerId, o => o.MapFrom(x =>(x.Customer != null ? x.Customer.GlobalId :(Guid?) null)))
                .AfterMap(delegate(TrainingDay s, TrainingDayDTO d)
                {
                    if (d != null)
                    {
                        //if (s != null && s.Profile != null)
                        //{
                        //    d.ProfileId = s.Profile.Id;
                        //}
                        foreach (var dto in d.Objects)
                        {
                            dto.TrainingDay = d;
                        }
                    }
                });
            Mapper.CreateMap<TrainingDayDTO, TrainingDay>()
                .ForMember(x => x.Objects, k => k.Ignore())
                .ForMember(x => x.CommentsCount, opt => opt.Ignore())
                .AfterMap(delegate(TrainingDayDTO s, TrainingDay d)
            {
                if (d != null)
                {
                    //if (s != null && s.Profile != null)
                    //{
                    //    d.ProfileId = s.Profile.Id;
                    //}
                    foreach (var dto in d.Objects)
                    {
                        dto.TrainingDay = d;
                    }
                }
            });
            //Mapper.CreateMap<TrainingPlan, WorkoutPlanDTO>().ForMember(dest => dest.PlanContent, opt => opt.ResolveUsing<SoloLazyInitResolver>().ConstructedBy(() => new SoloLazyInitResolver("PlanContent")));
            Mapper.CreateMap<TrainingPlan, Model.TrainingPlans.TrainingPlan>();
            Mapper.CreateMap<Model.TrainingPlans.TrainingPlan, TrainingPlan>()
                .ForMember(x => x.Profile, g => g.Ignore())
                .AfterMap(delegate(Model.TrainingPlans.TrainingPlan s, TrainingPlan d)
                {
                    if (d != null)
                    {
                        foreach (var dto in d.Days)
                        {
                            dto.TrainingPlan = d;
                        }
                    }
                });
            Mapper.CreateMap<TrainingPlanDay, Model.TrainingPlans.TrainingPlanDay>();
            Mapper.CreateMap<Model.TrainingPlans.TrainingPlanDay, TrainingPlanDay>()
                .AfterMap(delegate(Model.TrainingPlans.TrainingPlanDay s, TrainingPlanDay d)
                {
                    if (d != null)
                    {
                        foreach (var dto in d.Entries)
                        {
                            dto.Day = d;
                        }
                    }
                });


            Mapper.CreateMap<TrainingPlanEntry, Model.TrainingPlans.TrainingPlanEntry>();
            Mapper.CreateMap<Model.TrainingPlans.TrainingPlanEntry, TrainingPlanEntry>()
                .ForMember(dst => dst.Exercise, _ => _.ResolveUsing<LoadingGuidEntityResolver<Exercise>>().FromMember(src =>src.Exercise!=null? (object) src.Exercise.GlobalId:null))
                .AfterMap(delegate(Model.TrainingPlans.TrainingPlanEntry s, TrainingPlanEntry d)
                              {
                                  int i = 0;
                                if (d != null)
                                {
                                    i++;
                                    foreach (var dto in d.Sets)
                                    {
                                        dto.Entry = d;
                                        dto.Position = i;
                                        i++;
                                    }
                                }
                });
            Mapper.CreateMap<TrainingPlanSerie, Model.TrainingPlans.TrainingPlanSerie>();
            Mapper.CreateMap<Model.TrainingPlans.TrainingPlanSerie, TrainingPlanSerie>();



            Mapper.CreateMap<StrengthTrainingEntry, StrengthTrainingEntryDTO>();

            Mapper.CreateMap<StrengthTrainingEntryDTO, StrengthTrainingEntry>().ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation, _ => _.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? src.MyTraining.GlobalId : (Guid?)null))
                .ForMember(dst => dst.MyPlace, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyPlace>>().FromMember(src => src.MyPlace!=null?(object) src.MyPlace.GlobalId:null));
            Mapper.CreateMap<SuplementsEntry, SuplementsEntryDTO>();

            Mapper.CreateMap<SuplementsEntryDTO, SuplementsEntry>()
                .ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation, _ => _.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? src.MyTraining.GlobalId : (Guid?)null))
                .AfterMap((s, d) =>
                {
                    int i = 0;
                    foreach (var c in d.Items)
                    {
                        c.Position = i;
                        i++;
                    }
                });  


            Mapper.CreateMap<BlogEntry, BlogEntryDTO>();

            
            //Mapper.CreateMap<BlogEntryDTO, BlogEntry>().AfterMap((s, d) =>
            //{
            //    foreach (var c in d.BlogComments)
            //        c.BlogEntry = d;
            //});
            Mapper.CreateMap<BlogEntryDTO, BlogEntry>().ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation, _ => _.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? src.MyTraining.GlobalId : (Guid?)null));

            Mapper.CreateMap<TrainingDayComment, TrainingDayCommentDTO>()
                .ForMember(x => x.ApplicationName, v => v.MapFrom(u => u.LoginData.ApiKey.ApplicationName));
            Mapper.CreateMap<TrainingDayCommentDTO, TrainingDayComment>()
                .ForMember(t => t.TrainingDay, t => t.Ignore())
                .ForMember(t => t.Profile, t => t.Ignore());
            //TODO:check these Ignore on SuplementsEntry
            Mapper.CreateMap<SuplementItem, SuplementItemDTO>();
            Mapper.CreateMap<SuplementItemDTO, SuplementItem>()
                .ForMember(dst => dst.Suplement, _ => _.ResolveUsing<LoadingGuidEntityResolver<Suplement>>().FromMember(src => src.Suplement != null ? (object)src.Suplement.GlobalId : null)); 
            Mapper.CreateMap<A6WEntry, A6WEntryDTO>().ForMember(x=>x.Day,h=>h.MapFrom(j=>A6WManager.Days[j.DayNumber-1]));
            Mapper.CreateMap<A6WEntryDTO, A6WEntry>()
                .ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation, _ => _.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src =>src.MyTraining!=null? src.MyTraining.GlobalId:(Guid?)null))

                .ForMember(x => x.DayNumber, h => h.MapFrom(u => u.Day.DayNumber))
                 .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? (object)src.MyTraining.GlobalId : null)); 
            Mapper.CreateMap<GPSTrackerEntry, GPSTrackerEntryDTO>()
                .ForMember(x => x.HasCoordinates, h => h.MapFrom(u => u.Coordinates != null && u.Coordinates.GlobalId != Guid.Empty));

            Mapper.CreateMap<GPSTrackerEntryDTO, GPSTrackerEntry>()
                .ForMember(dst => dst.Exercise,_ => _.ResolveUsing<LoadingGuidEntityResolver<Exercise>>().FromMember(src => src.Exercise!=null?(object) src.Exercise.GlobalId:null));

            Mapper.CreateMap<SizeEntry, SizeEntryDTO>();
            Mapper.CreateMap<SizeEntryDTO, SizeEntry>().ForMember(x => x.TrainingDay, o => o.Ignore())
                .ForMember(dst => dst.Reservation, _ => _.ResolveUsing<LoadingGuidEntityResolver<ScheduleEntryReservation>>().FromMember(src => src.ReservationId))
                .ForMember(dst => dst.MyTraining, _ => _.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining != null ? src.MyTraining.GlobalId : (Guid?)null));


            Mapper.CreateMap<Suplement, SuplementDTO>().
                ForMember(x => x.Name, y => y.MapFrom(u => u.GetLocalizedName()))
                .ForMember(x => x.Url, y => y.MapFrom(u => u.GetLocalizedUrl()))
                .ForMember(x => x.ProfileId, y => y.MapFrom(u => u.Profile!=null?u.Profile.GlobalId:(Guid?)null));

            Mapper.CreateMap<StrengthTrainingItem, StrengthTrainingItemDTO>();

            Mapper.CreateMap<StrengthTrainingItemDTO, StrengthTrainingItem>()
                .ForMember(dst => dst.Exercise, _ => _.ResolveUsing<LoadingGuidEntityResolver<Exercise>>().FromMember(src => src.Exercise != null ? (object)src.Exercise.GlobalId : null))
                .AfterMap((s, d) =>
                              {
                                  int i = 0;
                                  foreach (var c in d.Series)
                                  {
                                      c.Position = i;
                                      i++;
                                  }
                              });
            //.ForMember(t => t.StrengthTrainingEntry, t => t.Ignore()).AfterMap((s, d) =>
            //{
            //    foreach (var c in d.Series)
            //        c.StrengthTrainingItem = d;
            //});
            Mapper.CreateMap<Serie, SerieDTO>().ForMember(x=>x.IsRecord,h=>h.MapFrom(p=>p.ExerciseProfileData!=null));
                //.ForMember(t => t.StrengthTrainingItem, t => t.Ignore());
            Mapper.CreateMap<SerieDTO, Serie>();
            Mapper.CreateMap<MyTraining, MyTrainingDTO>()
                .ForMember(x => x.CustomerId, i => i.MapFrom(o =>o.Customer!=null? o.Customer.GlobalId:(Guid?)null))
                .ForMember(x => x.ProfileId, i => i.MapFrom(o => o.Profile.GlobalId))
                .Include<SupplementCycle,SupplementsCycleDTO >()
            .Include<A6WTraining, A6WTrainingDTO>();
                //.ForMember(x => x.ProfileId, o => o.Ignore()).AfterMap(
                //delegate(MyTraining s, MyTrainingDTO d)
                //{
                //    if (s != null && d != null)
                //    {
                //        d.ProfileId = s.Profile.Id;
                //    }
                //});
            Mapper.CreateMap<MyTrainingDTO, MyTraining>()
                .ForMember(dst => dst.Customer, _ => _.ResolveUsing<LoadingGuidEntityResolver<Customer>>().FromMember(src => src.CustomerId))
                .ForMember(dst => dst.Profile, _ => _.ResolveUsing<LoadingGuidEntityResolver<Profile>>().FromMember(src => src.ProfileId))
                .ForMember(t => t.EntryObjects, t => t.Ignore())
                .Include<SupplementsCycleDTO, SupplementCycle>()
                .Include<A6WTrainingDTO, A6WTraining>();

            Mapper.CreateMap<SupplementCycle, SupplementsCycleDTO>();
            Mapper.CreateMap<SupplementsCycleDTO, SupplementCycle>()
                .ForMember(x => x.SupplementsCycleDefinition, j => j.Ignore());
            //.ForMember(x=>x.Profile,j=>j.Ignore());

            Mapper.CreateMap<A6WTraining, A6WTrainingDTO>();
            Mapper.CreateMap<A6WTrainingDTO, A6WTraining>();

            Mapper.CreateMap<MyTraining, MyTrainingLightDTO>();

            Mapper.CreateMap<SupplementCycleDefinitionDTO, SupplementCycleDefinition>()
                .ForMember(x => x.Profile, j => j.Ignore())
                .AfterMap(delegate(SupplementCycleDefinitionDTO s, SupplementCycleDefinition d)
                {
                    if (d != null)
                    {
                        foreach (var dto in d.Weeks)
                        {
                            dto.Definition = d;
                        }
                    }
                });
            Mapper.CreateMap<SupplementCycleDefinition, SupplementCycleDefinitionDTO>();

            Mapper.CreateMap<SupplementCycleWeek, SupplementCycleWeekDTO>();
            Mapper.CreateMap<SupplementCycleWeekDTO, SupplementCycleWeek>()
                .AfterMap(delegate(SupplementCycleWeekDTO s, SupplementCycleWeek d)
            {
                if (d != null)
                {
                    foreach (var dto in d.Dosages)
                    {
                        dto.Week = d;
                    }
                }
            });

            Mapper.CreateMap<SupplementCycleDosage, SupplementCycleDosageDTO>();
            Mapper.CreateMap<SupplementCycleDosageDTO, SupplementCycleDosage>()
                .ForMember(dst => dst.Supplement, _ => _.ResolveUsing<LoadingGuidEntityResolver<Suplement>>().FromMember(src => src.Supplement != null ? (object)src.Supplement.GlobalId : null));

            Mapper.CreateMap<SupplementCycleMeasurement, SupplementCycleMeasurementDTO>();
            Mapper.CreateMap<SupplementCycleMeasurementDTO, SupplementCycleMeasurement>();

            Mapper.CreateMap<SupplementCycleEntry, SupplementCycleEntryDTO>()
                .Include<SupplementCycleDosage, SupplementCycleDosageDTO>()
                .Include<SupplementCycleMeasurement, SupplementCycleMeasurementDTO>();
            Mapper.CreateMap<SupplementCycleEntryDTO, SupplementCycleEntry>()
                .Include<SupplementCycleDosageDTO, SupplementCycleDosage>()
                .Include<SupplementCycleMeasurementDTO, SupplementCycleMeasurement>();
        }

        private static object createServices(Type t)
        {
            if (t == typeof (ISession))
            {
                return NHibernateFactory.OpenSession();
            }
            else if(t==typeof(LoadingGuidEntityResolver<Exercise>))
            {
                return new LoadingGuidEntityResolver<Exercise>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<Suplement>))
            {
                return new LoadingGuidEntityResolver<Suplement>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<Activity>))
            {
                return new LoadingGuidEntityResolver<Activity>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<Customer>))
            {
                return new LoadingGuidEntityResolver<Customer>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<ScheduleEntryReservation>))
            {
                return new LoadingGuidEntityResolver<ScheduleEntryReservation>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<CustomerGroup>))
            {
                return new LoadingGuidEntityResolver<CustomerGroup>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<Profile>))
            {
                return new LoadingGuidEntityResolver<Profile>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<MyTraining>))
            {
                return new LoadingGuidEntityResolver<MyTraining>(NHibernateContext.Current().Session);
            }
            else if (t == typeof(LoadingGuidEntityResolver<MyPlace>))
            {
                return new LoadingGuidEntityResolver<MyPlace>(NHibernateContext.Current().Session);
            }
            return null;
        }

        public static FriendInvitationDTO ConvertFriendInvitation(Profile profileDb, FriendInvitation invitation)
        {
            FriendInvitationDTO dto = Mapper.Map<FriendInvitation, FriendInvitationDTO>(invitation);
            //we don't need to send the same user who invoke this method
            if (invitation.Invited == profileDb)
            {
                dto.Invited = null;
            }
            else
            {
                dto.Inviter = null;
            }
            return dto;
        }
        
    }
}
