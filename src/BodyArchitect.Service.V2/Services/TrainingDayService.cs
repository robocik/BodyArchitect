using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Client.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ExerciseDoneWay = BodyArchitect.Model.ExerciseDoneWay;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Gender = BodyArchitect.Model.Gender;
using Profile = BodyArchitect.Model.Profile;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;
using SetType = BodyArchitect.Model.SetType;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.V2.Services
{
    public class TrainingDayService : MessageServiceBase
    {
        public TrainingDayService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration, IPushNotificationService pushNotificationService, IEMailService emailService)
            : base(session, securityInfo, configuration, pushNotificationService,emailService)
        {
        }

        public SaveTrainingDayResult SaveTrainingDay(TrainingDayDTO day)
        {
            Log.WriteWarning("SaveTrainingDay:Username={0},dayId={1},day.TrainingDate={2}", SecurityInfo.SessionData.Profile.UserName, day.GlobalId, day.TrainingDate);

            if (day.TrainingDate > DateTime.UtcNow.Date && !SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }
            
            SaveTrainingDayResult result = new SaveTrainingDayResult();
            var session = Session;
            var dbNewDay = Mapper.Map<TrainingDayDTO, TrainingDay>(day);
            DateTime? updateMeasurementsInProfileTime = null;
            bool sendMessage = false;

            using (var tx = session.BeginSaveTransaction())
            {
                var dbProfile = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                Customer dbCustomer = null;
                if (day.CustomerId.HasValue)
                {
                    dbCustomer = session.Get<Customer>(day.CustomerId.Value);
                    if (dbCustomer != null && dbCustomer.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                    }
                    if (dbCustomer != null && !SecurityInfo.Licence.IsInstructor)
                    {
                        throw new LicenceException("This feature is allowed for Instructor account");
                    }
                }

                var dbDay = session.QueryOver<TrainingDay>().Fetch(x => x.Objects).Eager
                    .Fetch(x => ((SuplementsEntry) x.Objects.First()).Items).Eager
                        .Fetch(x => (((SuplementsEntry) x.Objects.First()).Items).First().Suplement).Eager
                        .Fetch(x => ((StrengthTrainingEntry) x.Objects.First()).Entries).Eager
                        .Fetch(x => ((StrengthTrainingEntry) x.Objects.First()).MyPlace).Eager
                        .Fetch(x => (((StrengthTrainingEntry) x.Objects.First()).Entries).First().Exercise).Eager
                        .Fetch(x => (((StrengthTrainingEntry) x.Objects.First()).Entries.First().Series)).Eager.
                    Fetch(x => x.Objects.First().MyTraining).Eager.
                    Where(x => x.TrainingDate == day.TrainingDate && x.Profile == dbProfile && x.Customer == dbCustomer).SingleOrDefault();
                if (dbDay == null)
                {
                    if (!day.IsNew)
                    {
                        dbDay = session.QueryOver<TrainingDay>().Where(x => x.GlobalId == day.GlobalId).SingleOrDefault();
                        if (dbDay!=null && dbDay.Customer != dbCustomer)
                        {
                            throw new InvalidOperationException("Cannot change the customer id for existing day");
                        }
                        session.Evict(dbDay);
                    }
                    //if (!day.IsNew)
                    //{
                    //    throw new StaleObjectStateException("Selected date has an entry already", day.GlobalId);
                    //}
                    dbDay = dbNewDay;
                }
                else
                {
                    //dbDay = dbDay.StandardClone();
                    dbNewDay.Comments = dbDay.Comments;
                }
                
                if(!dbDay.IsNew && dbDay.Customer!=dbCustomer)
                {
                    throw new InvalidOperationException("Cannot change the customer id for existing day");
                }
                dbDay.Customer = dbCustomer;

                if (dbDay.Profile != null && dbProfile != dbDay.Profile)
                {
                    throw new CrossProfileOperationException("This training day doesn't belong to your profile");
                }
                if (dbDay.GlobalId != day.GlobalId)
                {
                    throw new StaleObjectStateException("Selected date has an entry already", day.GlobalId);
                }
                dbNewDay.Profile = dbProfile;

                Model.Validators.SerieValidator validator = new Model.Validators.SerieValidator();
                //check correctness of every set
                foreach(var setDTO in day.Objects.OfType<StrengthTrainingEntryDTO>().SelectMany(x => x.Entries).SelectMany(x => x.Series).ToList())
                {
                    setDTO.IsIncorrect=!validator.IsCorrect(setDTO);
                }

                for (int index = day.Objects.Count - 1; index >= 0; index--)
                {
                    var dtoEntry = day.Objects[index];

                    var entry = dtoEntry.Map<EntryObject>();
                    var origEntry = entry;

                    if (entry.IsNew)
                    {//we send the messages only when user added a NEW entry only and not when we add entry for the customer. Only personal entries should be notified to the friends
                        sendMessage = dbDay.Customer == null;
                        entry.LoginData = SecurityInfo.LoginData;
                        if (entry.Status == EntryObjectStatus.System)
                        {
                            throw new ConsistencyException("Cannot create system entry");
                        }
                    }
                    else
                    {
                        //entry.LoginData = session.Get<EntryObject>(entry.GlobalId).LoginData;
                        //origEntry = Session.Get<EntryObject>(entry.GlobalId);
                        origEntry = Session.Get<EntryObject>(entry.GlobalId);
                        if (origEntry==null)
                        {
                            throw new StaleObjectStateException("EntryObject",entry.GlobalId);
                        }
                        //if old or new entry is system then old and new must have the same status (system) because client cannot change system status
                        if ((entry.Status == EntryObjectStatus.System || origEntry.Status==EntryObjectStatus.System) && entry.Status!=origEntry.Status)
                        {
                            throw new ConsistencyException("Cannot create system entry");
                        }
                        entry.LoginData = origEntry.LoginData;
                        
                    }

                    if (origEntry.Status == EntryObjectStatus.System)
                    {//for system entries we should  basically copy orignal entries (reject any changes from the user)
                        dbNewDay.AddEntry(origEntry);
                        entry = origEntry;
                    }
                    else
                    {
                        dbNewDay.AddEntry(entry);
                    }

                    if (entry.IsEmpty)
                    {
                        dbNewDay.RemoveEntry(entry);
                        continue;
                    }

                    prepareMyTraining(session, dbCustomer, dbProfile, dbNewDay, entry, dtoEntry.MyTraining);
                    
                    SizeEntry size = entry as SizeEntry;
                    if (size != null)
                    {
                        updateMeasurementsInProfileTime = size.Wymiary.Time.DateTime;
                    }
                    GPSTrackerEntry gpsEntry = entry as GPSTrackerEntry;
                    if (gpsEntry != null && origEntry!=null)
                    {
                        fillGpsTrackerEntry(dbCustomer, dbProfile, origEntry, gpsEntry);
                    }

                    prepareStrengthTrainingEntry(dbProfile,dbCustomer, entry, origEntry);
                    prepareGpsTrackerEntry(dbProfile, dbCustomer, entry, origEntry);

                    var reminderService = new ReminderService(session, SecurityInfo, Configuration);
                    reminderService.PrepareReminder(dbProfile, dtoEntry, entry, origEntry, getStartTime(entry), ReminderType.EntryObject, ReminderRepetitions.Once);
                }
                

                var comparer = new CollectionComparer<EntryObject>(dbNewDay.Objects, dbDay.Objects);
                foreach (var deletedEntry in comparer.RemovedItems)
                {
                    if (deletedEntry.MyTraining != null)
                    {
                        throw new TrainingIntegrationException("Cannot delete entries from training");
                    }
                    if (deletedEntry.Status == EntryObjectStatus.System)
                    {
                        throw new ConsistencyException("Cannot delete system entry");
                    }
                }

                dbProfile.DataInfo.TrainingDayHash = Guid.NewGuid();
                IList<Guid> newRecordSets = new List<Guid>();
                if (dbNewDay.Objects.Count > 0)
                {
                    var oldStrengthTraining = dbDay.Objects.OfType<StrengthTrainingEntry>();
                    var oldRecordSets = oldStrengthTraining.SelectMany(x => x.Entries).SelectMany(x => x.Series).Where(x => x.ExerciseProfileData != null).ToList();

                    dbNewDay = (TrainingDay)session.Merge(dbNewDay);
                    session.Flush();
                    //update reminders (ConnectedObjectId)
                    foreach (var entryObject in dbNewDay.Objects.Where(x=>x.Reminder!=null))
                    {
                        entryObject.Reminder.ConnectedObject = string.Format("EntryObjectDTO:{0}", entryObject.GlobalId);
                        Session.Update(entryObject);
                    }

                    newRecordSets = postEntryObjectProcessing(dbNewDay, dbDay, dbProfile, oldRecordSets);
                }
                else if (!dbNewDay.IsNew)
                {
                    session.Delete(dbDay);
                    postEntryObjectProcessing(dbNewDay, dbDay, dbProfile,new List<Serie>());
                    dbNewDay = null;
                }
                else
                {
                    dbNewDay = null;
                }


                
                if (dbCustomer == null)
                {//update statistics only when we edit profile data (not customer)
                    ProfileStatisticsUpdater.UpdateTrainindDay(session, dbProfile);
                }

                try
                {
                    if (updateMeasurementsInProfileTime != null)
                    {
                        MeasurementsAutomaticUpdater.Update(session, dbProfile, dbCustomer, updateMeasurementsInProfileTime.Value);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }
                
                tx.Commit();

                
                if (dbNewDay != null)
                {
                    //now we send messages to all friends 
                    if (sendMessage)
                    {
                        Configuration.MethodInvoker.Invoke((x) =>
                        {
                            sendMessageAboutCalendar(dbProfile, dbNewDay);
                        }, null);
                    }
                    var test = Mapper.Map<TrainingDay, TrainingDayDTO>(dbNewDay);
                    test.ChangeDate(new DateTime(dbNewDay.TrainingDate.Ticks, DateTimeKind.Unspecified), true);
                    result.TrainingDay= test;
                    result.NewRecords =result.TrainingDay.Objects.OfType<StrengthTrainingEntryDTO>().SelectMany(x => x.Entries).SelectMany(x => x.Series).Where(x => newRecordSets.Contains(x.GlobalId)).ToList();

                }
                return result;
            }
        }

        private void prepareGpsTrackerEntry(Profile dbProfile, Customer dbCustomer, EntryObject entry, EntryObject origEntry)
        {
            var gpsTrackerEntry = entry as GPSTrackerEntry;
            var orgGpsTrackerEntry = origEntry as GPSTrackerEntry;
            if (gpsTrackerEntry != null)
            {
                if (!SecurityInfo.Licence.IsPremium)
                {
                    if (gpsTrackerEntry.Weather != null && !gpsTrackerEntry.Weather.IsEmpty && (gpsTrackerEntry.IsNew ||
                         (orgGpsTrackerEntry.Weather.Condition != gpsTrackerEntry.Weather.Condition || orgGpsTrackerEntry.Weather.Temperature != gpsTrackerEntry.Weather.Temperature)))
                    {
                        throw new LicenceException("Weather can be set only in Premium or Instructor account");
                    }
                }
            }
        }

        private void fillGpsTrackerEntry(Customer dbCustomer, Profile dbProfile, EntryObject origEntry, GPSTrackerEntry gpsEntry)
        {
            gpsEntry.Coordinates = ((GPSTrackerEntry) origEntry).Coordinates;
            if(gpsEntry.Duration==null && gpsEntry.StartDateTime.HasValue && gpsEntry.EndDateTime.HasValue)
            {
                gpsEntry.Duration = (decimal?) (gpsEntry.EndDateTime.Value - gpsEntry.StartDateTime.Value).TotalSeconds;
            }
            //calculate average speed
            if (gpsEntry.Duration.HasValue && gpsEntry.Distance.HasValue)
            {
                gpsEntry.AvgSpeed = gpsEntry.Distance.Value/gpsEntry.Duration;
            }
            CalculateCaloriesBurned(gpsEntry, dbCustomer!=null?(IPerson) dbCustomer:dbProfile);
        }

        static internal decimal CalculateCalories(decimal met,decimal? duration, IPerson person)
        {
            DateTime? birthday = null;
            Gender gender = Gender.Male;
            decimal weight = 0;
            decimal height = 0;

            if (person.BirthdayDate != null)
            {
                birthday = person.BirthdayDate;
            }
            gender = person.Gender;
            if (person.Wymiary != null)
            {
                weight = person.Wymiary.Weight;
                height = person.Wymiary.Height;
            }
            if (gender == Gender.NotSet)
            {
                gender = Gender.Male;
            }

            if (weight == 0)
            {//set default weight if not defined
                if (gender == Gender.Female)
                {
                    weight = 70;
                }
                else
                {
                    weight = 100;
                }
            }
            if (gender == Gender.Male)
            {
                if (height == 0)
                {
                    height = 177.4M;
                }
            }
            else
            {//female
                if (height == 0)
                {
                    height = 163M;
                }
            }
            if (birthday == null)
            {//if birthday is not set then assume 30 years old
                birthday = DateTime.UtcNow.AddYears(-30).AddDays(-1);
            }

            int age = birthday.Value.GetAge();
            return WilksFormula.CalculateCaloriesUsingMET(gender == Gender.Male, met, TimeSpan.FromSeconds((double)duration), age, weight, height);
        }

        static internal void CalculateCaloriesBurned(GPSTrackerEntry gpsEntry, IPerson person)
        {
            if(gpsEntry.Duration==null)
            {//we don't have time of exercising then we cannot calculate calories
                return;
            }
            gpsEntry.Calories = CalculateCalories(gpsEntry.Exercise.Met, gpsEntry.Duration.Value, person);
        }

        DateTime getStartTime(EntryObject entryObject)
        {
            SizeEntry size = entryObject as SizeEntry;
            StrengthTrainingEntry strength = entryObject as StrengthTrainingEntry;

            if (size != null)
            {
                return size.TrainingDay.TrainingDate + size.Wymiary.Time.DateTime.TimeOfDay;
            }
            if (strength != null && strength.StartTime != null)
            {
                return strength.TrainingDay.TrainingDate + strength.StartTime.Value.TimeOfDay;
            }
            return entryObject.TrainingDay.TrainingDate;
        }

        private IList<Guid> postEntryObjectProcessing(TrainingDay dbNewDay, TrainingDay dbDay, Profile dbProfile,IList<Serie> oldRecordSets)
        {
            var newStrengthTrainings = dbNewDay.Objects.OfType<StrengthTrainingEntry>().ToList();

            var strengthTraining = dbNewDay.Objects.OfType<StrengthTrainingEntry>().ToList();
            strengthTraining.AddRange(dbDay.Objects.OfType<StrengthTrainingEntry>());
            EnsureRecords(dbProfile, dbNewDay.Customer, strengthTraining);

            var newRecordSets = newStrengthTrainings.SelectMany(x => x.Entries).SelectMany(x => x.Series).Where(x => x.ExerciseProfileData != null).ToList();
            var currentRecords = newRecordSets.Except(oldRecordSets).ToList();
            return currentRecords.Select(x => x.GlobalId).ToList();
        }


        //private void sendMessageToFriends(ISession session, Profile dbProfile, TrainingDay dbNewDay)
        //{
            
        //    try
        //    {
        //        Profile profileAlias = null;
        //        Profile friendAlias = null;
        //        var resut = session.QueryOver<Profile>(() => profileAlias)
        //            .JoinAlias(() => profileAlias.Friends, () => friendAlias)
        //            .Where(() => friendAlias.GlobalId == dbProfile.GlobalId).List();
        //        foreach (var profile in resut)
        //        {
        //            string param = string.Format("{0},{1},{2}", dbNewDay.TrainingDate.ToShortDateString(), dbProfile.UserName, DateTime.Now);
        //            SendMessage(profile.Settings.NotificationFriendChangedCalendar, dbProfile, profile, param, BodyArchitect.Model.MessageType.TrainingDayAdded,
        //                "FriendsTrainingDayChangedEMailSubject", "FriendsTrainingDayChangedEMailMessage", dbProfile.UserName, dbNewDay.TrainingDate.ToShortDateString(), DateTime.Now);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.Default.Process(ex);
        //    }
        //}

        void sendMessageAboutCalendar(Profile dbProfile,TrainingDay dbNewDay)
        {
            var maintenanceSession = NHibernateFactory.OpenSession();
            using (var tx = maintenanceSession.BeginSaveTransaction())
            {
                newSendMessageToFriends(maintenanceSession, dbProfile, dbNewDay);
                newSendMessageToFavorites(maintenanceSession, dbProfile, dbNewDay);
                tx.Commit();
            }
            maintenanceSession.Close();
        }

        private void newSendMessage(ISession session,IList<Profile> profiles,Func<Profile,ProfileNotification> profileNotification, string subjectKey, string messageKey, dynamic arg)
        {
            var templateCache = new Dictionary<CultureInfo, Tuple<string, string>>();

            foreach (var profile in profiles)
            {
                var culture = profile.GetProfileCulture();
                if (!templateCache.ContainsKey(culture))
                {
                    var subject = LocalizedStrings.ResourceManager.GetString(subjectKey, culture);
                    var message = LocalizedStrings.ResourceManager.GetString(messageKey, culture);

                    //message = Razor.Parse(message, arg);
                    //subject = Razor.Parse(subject, arg);
                    StringBuilder builder = new StringBuilder();
                    foreach (Serie record in arg.Records)
                    {
                        builder.AppendLine(string.Format("{0:#} - {1}",record.StrengthTrainingItem.Exercise.Name,record));
                    }
                    message = string.Format(message, arg.User.UserName, arg.TrainingDay.TrainingDate.ToShortDateString(), arg.DateTime, builder.ToString());
                    subject = string.Format(subject, arg.User.UserName, arg.TrainingDay.TrainingDate.ToShortDateString(), arg.DateTime);
                    templateCache.Add(culture, new Tuple<string, string>(subject, message));
                }

                var currentMessage = templateCache[culture];

                NewSendMessage(session,profileNotification(profile), arg.User, profile, currentMessage.Item1, currentMessage.Item2);
            }
        }
        private void newSendMessageToFriends(ISession session, Profile dbProfile, TrainingDay dbNewDay)
        {
            try
            {
                Profile profileAlias = null;
                Profile friendAlias = null;
                var resut = session.QueryOver<Profile>(() => profileAlias)
                    .JoinAlias(() => profileAlias.Friends, () => friendAlias)
                    .Where(() => friendAlias.GlobalId == dbProfile.GlobalId).Fetch(x => friendAlias.Statistics).Eager.List();

                var arg = new
                {
                    DateTime = DateTime.Now,
                    User = dbProfile,
                    TrainingDay = dbNewDay,
                    Records = dbNewDay.Objects.OfType<StrengthTrainingEntry>().SelectMany(x => x.Entries).SelectMany(x => x.Series).
                        Where(x => x.ExerciseProfileData != null).ToList()
                };


                newSendMessage(session,resut, p => p.Settings.NotificationFriendChangedCalendar, "FriendsTrainingDayChangedEMailSubject", "FriendsTrainingDayChangedEMailMessage", arg);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
        }

        private void newSendMessageToFavorites(ISession session, Profile dbProfile, TrainingDay dbNewDay)
        {

            try
            {
                Profile profileAlias = null;
                Profile followerAlias = null;
                var resut = session.QueryOver<Profile>(() => profileAlias)
                    .JoinAlias(() => profileAlias.FavoriteUsers, () => followerAlias)
                    .Where(() => followerAlias.GlobalId == dbProfile.GlobalId).Fetch(x => followerAlias.Statistics).Eager.List();

                var arg = new
                {
                    DateTime = DateTime.Now,
                    User = dbProfile,
                    TrainingDay = dbNewDay,
                    Records = dbNewDay.Objects.OfType<StrengthTrainingEntry>().SelectMany(x => x.Entries).SelectMany(x => x.Series).
                        Where(x => x.ExerciseProfileData != null).ToList()
                };

                newSendMessage(session,resut, p => p.Settings.NotificationFollowersChangedCalendar, "FollowersTrainingDayChangedEMailSubject", "FollowersTrainingDayChangedEMailMessage", arg);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }
        }

        //private void sendMessageToFavorites(ISession session, Profile dbProfile, TrainingDay dbNewDay)
        //{

        //    try
        //    {
        //        Profile profileAlias = null;
        //        Profile followerAlias = null;
        //        var resut = session.QueryOver<Profile>(() => profileAlias)
        //            .JoinAlias(() => profileAlias.FavoriteUsers, () => followerAlias)
        //            .Where(() => followerAlias.GlobalId == dbProfile.GlobalId).List();
        //        foreach (var profile in resut)
        //        {
        //            string param = string.Format("{0},{1},{2}", dbNewDay.TrainingDate.ToShortDateString(), dbProfile.UserName, DateTime.Now);
        //            SendMessage(profile.Settings.NotificationFollowersChangedCalendar, dbProfile, profile, param, BodyArchitect.Model.MessageType.TrainingDayAdded,
        //                "FollowersTrainingDayChangedEMailSubject", "FollowersTrainingDayChangedEMailMessage", dbProfile.UserName, dbNewDay.TrainingDate.ToShortDateString(), DateTime.Now);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.Default.Process(ex);
        //    }
        //}

        private void prepareMyTraining(ISession session, Customer dbCustomer, Profile dbProfile, TrainingDay dbNewDay, EntryObject entry,MyTrainingLightDTO myTrainingDto)
        {
            if (entry.MyTraining != null)
            {
                if (entry.MyTraining.GlobalId == Guid.Empty)
                {
                    throw new TrainingIntegrationException("Cannot start training using SaveTrainingDay method");
                }
                entry.MyTraining.Profile = dbProfile;
                if (entry.MyTraining.Customer != dbCustomer)
                {
                    throw new ArgumentException("Customer for MyTraining is different that Customer for training day");
                }
                processMyTraining(session, entry, dbNewDay, false, dbProfile, myTrainingDto);
            }
        }

        //private void prepareBlogEntry(ISession session, TrainingDay dbDay, EntryObject entry)
        //{
        //    BlogEntry blog = entry as BlogEntry;
        //    if (blog != null && !blog.IsNew)
        //    {
        //        var orginalBlog =
        //            (from en in dbDay.Objects where en.GlobalId == blog.GlobalId select en).Cast<BlogEntry>().
        //                SingleOrDefault();
        //        if (orginalBlog != null)
        //        {
        //            blog.LastCommentDate = orginalBlog.LastCommentDate;
        //        }
        //        session.Refresh(blog);
        //    }
        //}
        
        MyPlace getDefaultMyPlace(Profile dbProfile)
        {
            var defaultMyPlace = Session.QueryOver<MyPlace>().Where(x => x.Profile == dbProfile && x.IsDefault).SingleOrDefault();
            return defaultMyPlace;
        }

        void ensureStrengthTrainingItems(StrengthTrainingEntry newEntry,StrengthTrainingEntry oldEntry)
        {
            foreach (StrengthTrainingItem item in newEntry.Entries)
            {
                if(item.IsNew && item.DoneWay!=ExerciseDoneWay.Default)
                {
                    throw new LicenceException("DoneWay can be set only in Premium or Instructor account");
                }
                StrengthTrainingItem oldItem = null;
                if(!item.IsNew)
                {
                    oldItem = oldEntry.Entries.Where(x => x.GlobalId == item.GlobalId).SingleOrDefault();
                    if (oldItem.DoneWay != item.DoneWay)
                    {
                        throw new LicenceException("DoneWay can be set only in Premium or Instructor account");
                    }
                }
                
                foreach (var serie in item.Series)
                {
                    if (serie.IsNew && (serie.IsRestPause || serie.IsSuperSlow))
                    {
                        throw new LicenceException("Rest pause or SuperSlow can be set only in Premium or Instructor account");
                    }
                    if (!serie.IsNew && oldItem!=null)
                    {
                        var oldSet = oldItem.Series.Where(x => x.GlobalId == serie.GlobalId).SingleOrDefault();
                        if (oldSet.IsRestPause != serie.IsRestPause || oldSet.IsSuperSlow != serie.IsSuperSlow)
                        {
                            throw new LicenceException("Rest pause or SuperSlow can be set only in Premium or Instructor account");
                        }
                    }
                }
            }
        }

        private void prepareStrengthTrainingEntry(Profile dbProfile,Customer dbCustomer, EntryObject entry, EntryObject originalEntry)
        {
            StrengthTrainingEntry strengthEntry = entry as StrengthTrainingEntry;
            StrengthTrainingEntry origStrengthEntry = originalEntry as StrengthTrainingEntry;
            if (strengthEntry != null)
            {
                var dbMyPlace = getDefaultMyPlace(dbProfile);
                if (strengthEntry.MyPlace != null)
                {
                    if (strengthEntry.MyPlace.Profile != dbProfile)
                    {
                        throw new CrossProfileOperationException("This MyPlace doesn't belongs to this profile");
                    }
                }
                else
                {
                    strengthEntry.MyPlace =dbMyPlace;
                }

                if(!SecurityInfo.Licence.IsPremium)
                {
                    ensureStrengthTrainingItems(strengthEntry, origStrengthEntry);
                    if (strengthEntry.MyPlace != null && (strengthEntry.IsNew && strengthEntry.MyPlace != dbMyPlace || origStrengthEntry.MyPlace != strengthEntry.MyPlace))
                    {
                        throw new LicenceException("MyPlace can be set only in Premium or Instructor account");
                    }
                }

                var person = dbCustomer != null ? (IPerson) dbCustomer : dbProfile;
                //calculate wilks points and burned calories
                calculateCaloriesAndWilksForSets(strengthEntry, person);

                foreach (var strengthTrainingItem in strengthEntry.Entries)
                {
                    if (strengthTrainingItem.Exercise.ExerciseType == ExerciseType.Cardio)
                    {
                        foreach (var series in strengthTrainingItem.Series)
                        {
                            if (series.Weight == null && series.StartTime.HasValue && series.EndTime.HasValue && series.EndTime.Value>series.StartTime.Value)
                            {
                                series.Weight = (decimal?) (series.EndTime.Value - series.StartTime.Value).TotalSeconds;
                            }
                        }
                    }
                }
                //Session.QueryOver<Exercise>().WhereRestrictionOn(x => x.GlobalId).IsIn(
                //    strengthEntry.Entries.Select(x1 => x1.Exercise.GlobalId).ToList()).List();

                //SerieValidator validator = new SerieValidator();
                ////validate sets
                //foreach(var set in strengthEntry.Entries.SelectMany(x=>x.Series))
                //{
                //    set.IsIncorrect=!validator.IsCorrect(set);
                //}
            }
        }

        private static void calculateCaloriesAndWilksForSets(StrengthTrainingEntry strengthEntry, IPerson person)
        {
            foreach (var strengthTrainingItem in strengthEntry.Entries)
            {
                foreach (var series in strengthTrainingItem.Series)
                {
                    if (series.Weight.HasValue && series.Weight > 0)
                    {
                        if (strengthTrainingItem.Exercise.ExerciseType == ExerciseType.Cardio)
                        {
//calculate calories for cardio
                            series.CalculatedValue = CalculateCalories(strengthTrainingItem.Exercise.Met, series.Weight.Value,
                                                                       person);
                        }
                        else
                        {
//calculate wilks points

                            //todo:should we calculate wilks?
                            //series.CalculatedValue = calculateWilks(person, series.Weight.Value);
                        }
                    }
                }
            }
        }

        decimal? calculateWilks(IPerson customer, decimal exerciseWeight)
        {
            if (customer.Wymiary == null || customer.Gender==Gender.NotSet)
            {
                return null;
            }
            if (customer.Wymiary.Weight <= 0)
            {
                return 0;
            }
            if (customer.Gender == Gender.Male)
            {
                return WilksFormula.CalculateForMenUsingTables(customer.Wymiary.Weight, exerciseWeight);
            }
            if (customer.Gender == Gender.Female)
            {
                return WilksFormula.CalculateForWomenUsingTables(customer.Wymiary.Weight, exerciseWeight);
            }
            throw new ArgumentException("Customer without gender!");
        }
      
        List<Guid> exercisesToRecalculateRecords(IList<StrengthTrainingEntry> strengthTrainings)
        {
            var exerciseIds = strengthTrainings.Where(x=>!x.MyPlace.NotForRecords).SelectMany(x => x.Entries)
                .Where(x=>x.DoneWay==ExerciseDoneWay.Default).Select(x => x.Exercise.GlobalId).Distinct().ToList();
            return exerciseIds;
        }

        public void EnsureRecords(Profile dbProfile,Customer customer,IList<StrengthTrainingEntry> strengthTrainings)
        {
            var exerciseIds = exercisesToRecalculateRecords(strengthTrainings);
            if (exerciseIds.Count==0)
            {
                return;
            }
            calculateExerciseRecords(dbProfile, customer, exerciseIds);


            //now do remove old ExerciseProfileData for this entry. This is required when user change an exercise for existing strength entry item
            Serie tmpSerie = null;
            StrengthTrainingItem tmpItem = null;
            StrengthTrainingEntry tmpEntry = null;

            var res = Session.QueryOver<ExerciseProfileData>().JoinAlias(x => x.Serie, () => tmpSerie).JoinAlias(x => tmpSerie.StrengthTrainingItem, () => tmpItem)
                .JoinAlias(x => tmpItem.StrengthTrainingEntry, () => tmpEntry).WhereRestrictionOn(a => a.Serie.GlobalId).IsIn(strengthTrainings.SelectMany(x => x.Entries).SelectMany(x => x.Series).Select(x => x.GlobalId).Distinct().ToList()).List();
            foreach (var exerciseProfileData in res.Distinct())
            {
                if (/*exerciseProfileData.Serie.ExerciseProfileData==null || */exerciseProfileData.GlobalId != exerciseProfileData.Serie.ExerciseProfileData.GlobalId)
                {
                    Session.Delete(exerciseProfileData);
                }
            }    
        }

        private void calculateExerciseRecords(Profile dbProfile, Customer customer, List<Guid> exerciseIds)
        {
            Session.Flush();
            var exercisesData = Session.QueryOver<ExerciseProfileData>().Where(x => x.Profile == dbProfile && x.Customer == customer)
                .WhereRestrictionOn(a => a.Exercise.GlobalId).IsIn(exerciseIds).List();
            Dictionary<Guid,IFutureValue<object[]>> maxWeightsForExercises=new Dictionary<Guid, IFutureValue<object[]>>();

           

            foreach (var exerciseId in exerciseIds)
            {
                TrainingDay day = null;
                Serie serie = null;
                StrengthTrainingEntry ste = null;
                StrengthTrainingItem item = null;
                Exercise exercise = null;
                MyPlace place = null;
                
                var obj = Session.QueryOver<TrainingDay>(() => day)
                    .JoinAlias(x => x.Objects, () => ste)
                    .JoinAlias(x => ste.Entries, () => item)
                    .JoinAlias(x => item.Exercise, () => exercise)
                    .JoinAlias(x => ste.MyPlace, () => place)
                    .JoinAlias(x => item.Series, () => serie)
                    .Where(x => !place.NotForRecords &&  ste.Status!=EntryObjectStatus.Planned && day.Profile == dbProfile && day.Customer == customer && item.DoneWay==ExerciseDoneWay.Default && item.Exercise.GlobalId == exerciseId &&
                        !serie.IsIncorrect && serie.SetType != SetType.MuscleFailure && serie.Weight != null && ((serie.RepetitionNumber != null && serie.Weight > 0 && serie.RepetitionNumber > 0) || exercise.ExerciseType == ExerciseType.Cardio)).
                    OrderBy(x => serie.Weight).Desc.ThenBy(x=>serie.RepetitionNumber).Desc
                    .SelectList(l =>l.Select(x => day.TrainingDate).Select(x => serie.GlobalId).Select(x => serie.Weight).Select(x => serie.RepetitionNumber))
                    .Take(1).FutureValue<object[]>();
                maxWeightsForExercises.Add(exerciseId, obj);
            }
            foreach (var exerciseId in exerciseIds)
            {
                try
                {
                    if (exercisesData.Where(x => x.Exercise == null).Count() > 0)
                    {
                        var temp = exercisesData.Where(x => x.Exercise == null).First();
                        ExceptionHandler.Default.Process(new Exception(string.Format("MY TEST EXCEPTION CALCULATION:{0},{1},{2}", 
                            temp.Profile!=null?temp.Profile.UserName:"NULL",
                            temp.Serie != null ? temp.Serie.GlobalId.ToString() : "NULL",
                            temp.TrainingDate )));
                        continue;
                    }
                    var exerciseData = exercisesData.SingleOrDefault(x => x.Exercise.GlobalId == exerciseId);
                    var maxWeightRow = maxWeightsForExercises[exerciseId].Value;
                    if (maxWeightRow != null)
                    {
                        if (exerciseData == null)
                        {
                            exerciseData = new ExerciseProfileData();
                            exerciseData.Exercise = Session.Load<Exercise>(exerciseId);
                            exerciseData.Profile = dbProfile;
                            exerciseData.Customer = customer;
                        }
                        else
                        {
                            //exerciseData.Serie = Session.Get<Serie>(exerciseData.Serie.GlobalId);
                            exerciseData.Serie.ExerciseProfileData = exerciseData;
                        }

                        if (exerciseData.MaxWeight < (decimal)maxWeightRow[2] || (exerciseData.Repetitions != null && exerciseData.MaxWeight == (decimal)maxWeightRow[2] && exerciseData.Repetitions < (decimal)maxWeightRow[3]))
                        {
                            if (exerciseData.Serie != null)
                            {
                                //Session.Delete(exerciseData.Serie.ExerciseProfileData);
                                exerciseData.Serie.ExerciseProfileData = null; //remove from old set
                            }
                            exerciseData.Serie = Session.Get<Serie>(maxWeightRow[1]);
                            //if (exerciseData.Serie.ExerciseProfileData != null)
                            //{
                            //    Session.Delete(exerciseData.Serie.ExerciseProfileData);
                            //    exerciseData.Serie.ExerciseProfileData = null; //remove from old set
                            //}
                            exerciseData.Serie.ExerciseProfileData = exerciseData;
                            exerciseData.MaxWeight = (decimal)maxWeightRow[2];
                            exerciseData.Repetitions = (decimal?)maxWeightRow[3];
                            exerciseData.TrainingDate = (DateTime)maxWeightRow[0];
                            Session.SaveOrUpdate(exerciseData);
                        }
                    }
                    else if (exerciseData != null)
                    {
                        Session.Delete(exerciseData);
                    }
                }
                catch (Exception ex)
                {//exception can occured in calculating records but this shouldn't prevent saving entry (I think)
                    ExceptionHandler.Default.Process(ex);
                }
                
            }
        }


        public void DeleteTrainingDay(DeleteTrainingDayParam param)
        {
            Log.WriteWarning("DeleteTrainingDay:Username={0},dayId={1}", SecurityInfo.SessionData.Profile.UserName, param.TrainingDayId);
            bool updateMeasurementsInProfile = false;
            
            
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var dbDay = session.QueryOver<TrainingDay>().Where(x=>x.GlobalId==param.TrainingDayId).Fetch(x => x.Objects).Eager
                        .Fetch(x => (((StrengthTrainingEntry)x.Objects.First()).Entries)).Eager.SingleOrDefault();
                var dbProfile = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if (dbProfile != dbDay.Profile)
                {
                    throw new CrossProfileOperationException("Cannot delete training day for another user");
                }
                if (dbDay.Customer!=null && !SecurityInfo.Licence.IsInstructor)
                {
                    throw new LicenceException("This feature is allowed for Instructor account");
                }
                if (dbDay.TrainingDate > DateTime.UtcNow.Date && !SecurityInfo.Licence.IsPremium)
                {
                    throw new LicenceException("This feature is allowed for Premium account");
                }
                var sizeEntry = dbDay.GetSpecifiedEntries<SizeEntry>();
                updateMeasurementsInProfile = sizeEntry.Count > 0;

                var systemEntries = dbDay.Objects.Where(x => x.Status ==EntryObjectStatus.System).ToList();
                var entriesWithTraining = dbDay.Objects.Where(x => x.MyTraining != null).ToList();

                if (param.Mode == DeleteTrainingDayMode.All)
                {
                    if (systemEntries.Count > 0)
                    {
                        throw new ConsistencyException("Cannot delete training day with system entries");
                    }
                    if (entriesWithTraining.Count > 0)
                    {
                        throw new TrainingIntegrationException("Cannot delete training day because it contains entries from training");
                    }
                }

                if (param.Mode == DeleteTrainingDayMode.OnlyWithoutMyTraining)
                {
                    for (int i = dbDay.Objects.Count - 1; i >= 0; i--)
                    {
                        var entry = dbDay.Objects.ElementAt(i);
                        if (entry.Status != EntryObjectStatus.System && entry.MyTraining == null)
                        {
                            dbDay.RemoveEntry(entry);
                        }
                    }
                    Session.Update(dbDay);
                }
                

                if (entriesWithTraining.Count==0 || param.Mode == DeleteTrainingDayMode.All || dbDay.Objects.Count == 0)
                {
                    session.Delete(dbDay);
                    
                    EnsureRecords(dbProfile,dbDay.Customer, dbDay.Objects.OfType<StrengthTrainingEntry>().ToList());
                    session.Flush();
                    ProfileStatisticsUpdater.UpdateTrainindDay(session, dbProfile);
                }
                //var a6wList = dbDay.GetSpecifiedEntries<A6WEntry>();
                

                //if (a6wList.Count > 0)
                //{
                //    var a6w = a6wList[0];
                //    ensureIntegrationA6W(session, a6w, dbDay, true);
                //    if (a6w.MyTraining.EntryObjects.Count == 1)
                //    {
                //        session.Delete(a6w.MyTraining);
                //    }
                //}
                dbProfile.DataInfo.TrainingDayHash = Guid.NewGuid();
                
                tx.Commit();
                if (updateMeasurementsInProfile)
                {
                    MeasurementsAutomaticUpdater.Update(session, dbProfile,dbDay.Customer, dbDay.TrainingDate);
                }
                session.Flush();
            }
        }

        private void processMyTraining(ISession session, EntryObject a6w, TrainingDay day, bool isDeleting, Profile dbProfile, MyTrainingLightDTO myTrainingDto)
        {
            //refresh mytraining EntryObjects collection
            var newMyTraining = a6w.MyTraining;
            a6w.MyTraining = session.Get<MyTraining>(newMyTraining.GlobalId);
            if (a6w.MyTraining.Profile != dbProfile)
            {
                throw new CrossProfileOperationException("This training doesn't belong to your profile");
            }
            DateTime? dateTime = newMyTraining.EndDate;
            
            //var existingLastEntry = a6w.MyTraining.EntryObjects.OrderBy(x => x.TrainingDay.TrainingDate).Last();
            TrainingDay _day = null;
            var entries=session.QueryOver<EntryObject>().JoinAlias(x=>x.TrainingDay,()=>_day).Where(x => x.MyTraining == a6w.MyTraining).OrderBy(()=>_day.TrainingDate).Asc.List();
            var existingLastEntry = entries.Last();
            //var existingLastEntry=session.QueryOver<EntryObject>().JoinAlias(x=>x.TrainingDay,()=>_day).Where(x => x.MyTraining == a6w.MyTraining).OrderBy(()=>_day.TrainingDate).Desc.Take(1).SingleOrDefault();)
            if (existingLastEntry.GlobalId == a6w.GlobalId && a6w.MyTraining.TrainingEnd == TrainingEnd.NotEnded && a6w.Status == EntryObjectStatus.Done)
            {
                newMyTraining.Complete(Configuration.TimerService);
                dateTime = newMyTraining.EndDate;
            }
            var existingEntry = entries.Where(x => x.GlobalId == a6w.GlobalId).SingleOrDefault();
            if (existingEntry == null)
            {
                throw new TrainingIntegrationException("Cannot add any entries to existing training");
            }
            //update a status for current saved entryobject (because MyTraining.EntryObjects collection comes from db and there is old Status yet)
            existingEntry.Status = a6w.Status;

            a6w.MyTraining.SetData(newMyTraining.StartDate, dateTime, newMyTraining.TrainingEnd);
            int doneEntriesCount = entries.Where(x => x.Status == EntryObjectStatus.Done).Count();
            a6w.MyTraining.PercentageCompleted = (int)((doneEntriesCount / (double)entries.Count) * 100);

            if (myTrainingDto.TrainingEnd == Model.TrainingEnd.Complete)
            {
                if (a6w.MyTraining.TrainingEnd == TrainingEnd.NotEnded)
                {
                    SupplementsEngineService service = new SupplementsEngineService(Session, SecurityInfo, Configuration);
                    service.StopMyTraining(newMyTraining.GlobalId, dbProfile);
                    var temp=session.Get<A6WEntry>(a6w.GlobalId);

                    //bug fixing: there is unit test which fail without this
                    a6w.Version = temp.Version;
                    return;
                    //dateTime = a6w.MyTraining.EntryObjects.Last().TrainingDay.TrainingDate;
                    //dateTime = a6w.MyTraining.EntryObjects[a6w.MyTraining.EntryObjects.Count - 1].TrainingDay.TrainingDate;
                }

            }

            //var entries = a6w.MyTraining.EntryObjects;

            //if (entries.Count > 0)
            //{
            //    var maxDay = entries.Max(t => t.TrainingDay.TrainingDate);
            //    if ((isDeleting || a6w.Id == Constants.UnsavedObjectId) && maxDay > day.TrainingDate)
            //    {
            //        throw new TrainingIntegrationException("Integration training error");
            //    }
            //}
        }

        //private void ensureIntegrationA6W(ISession session, A6WEntry a6w, TrainingDay day, bool isDeleting)
        //{

        //    if (!a6w.MyTraining.IsNew)
        //    {//refresh mytraining EntryObjects collection
        //        var newMyTraining = a6w.MyTraining;
        //        a6w.MyTraining = session.Get<MyTraining>(newMyTraining.GlobalId);
        //        DateTime? dateTime = newMyTraining.EndDate;
        //        if (newMyTraining.TrainingEnd == TrainingEnd.Break)
        //        {
        //            if (a6w.MyTraining.TrainingEnd == TrainingEnd.NotEnded)
        //            {
        //                dateTime = a6w.MyTraining.EntryObjects.Last().TrainingDay.TrainingDate;
        //                //dateTime = a6w.MyTraining.EntryObjects[a6w.MyTraining.EntryObjects.Count - 1].TrainingDay.TrainingDate;
        //            }

        //        }
        //        else if (a6w.DayNumber == A6WManager.LastDay.DayNumber && a6w.MyTraining.TrainingEnd == TrainingEnd.NotEnded)
        //        {
        //            newMyTraining.Complete(Configuration.TimerService);
        //            dateTime = newMyTraining.EndDate;
        //        }


        //        a6w.MyTraining.SetData(newMyTraining.StartDate, dateTime, newMyTraining.TrainingEnd);
        //        a6w.MyTraining.PercentageCompleted = newMyTraining.PercentageCompleted;
        //    }
        //    var entries = a6w.MyTraining.EntryObjects;

        //    if (entries.Count > 0)
        //    {
        //        var maxDay = entries.Max(t => t.TrainingDay.TrainingDate);
        //        if ((isDeleting || a6w.Id == Constants.UnsavedObjectId) && maxDay > day.TrainingDate)
        //        {
        //            throw new TrainingIntegrationException("Integration training error");
        //        }
        //    }
        //}

        public PagedResult<TrainingDayDTO> GetTrainingDays( WorkoutDaysSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetTrainingDays:Username={0},searchCriteria.UserId={1}", SecurityInfo.SessionData.Profile.UserName, searchCriteria.UserId);

            var session = Session;
            using (var tx = session.BeginGetTransaction())
            {
                var idsQuery =session.QueryOver<TrainingDay>();
                idsQuery = getTrainingDaysCriterias(searchCriteria, idsQuery);
                if (idsQuery == null)
                {
                    return new PagedResult<TrainingDayDTO>(new List<TrainingDayDTO>(), 0, 0);
                }
                idsQuery=idsQuery.OrderBy(x => x.TrainingDate).Asc;
                idsQuery.ApplyPaging(retrievingInfo);
                var result1 =
                    session.QueryOver<TrainingDay>()
                        .Fetch(x => x.Objects).Eager
                        .Fetch(x => ((SuplementsEntry) x.Objects.First()).Items).Eager
                        .Fetch(x => (((SuplementsEntry) x.Objects.First()).Items).First().Suplement).Eager
                        .Fetch(x => ((StrengthTrainingEntry) x.Objects.First()).Entries).Eager
                        .Fetch(x => ((StrengthTrainingEntry) x.Objects.First()).MyPlace).Eager
                        .Fetch(x => (((StrengthTrainingEntry) x.Objects.First()).Entries).First().Exercise).Eager
                        .Fetch(x => (((StrengthTrainingEntry) x.Objects.First()).Entries.First().Series)).Eager

                        .Fetch(x => x.Objects.First().LoginData).Eager
                        .Fetch(x => x.Objects.First().MyTraining).Eager
                        .Fetch(x => x.Objects.First().LoginData.ApiKey).Eager
                        .Fetch(x => x.Objects.First().Reminder).Eager.OrderBy(x=>x.TrainingDate).Asc;
                    
                //if (getTrainingDaysCriterias(searchCriteria,  result)==null)
                //{
                //    return new PagedResult<TrainingDayDTO>(new List<TrainingDayDTO>(),0,0 );
                //}
                //result1=getTrainingDaysCriterias(searchCriteria, result1);

                //first query is only for retrieving all required data (because of joins we will have duplicated results)
                //var fetchQuery=result1.ApplyPaging(retrievingInfo);
                //fetchQuery.Future();
                //the second query is only for retrieving AllCount and Count items values
                //var res = result.ToPagedResults<TrainingDayDTO, TrainingDay>( retrievingInfo, null,null, result1);
                var res = result1.ToExPagedResults<TrainingDayDTO, TrainingDay>(retrievingInfo, idsQuery);
                tx.Commit();
                return res;
            }

        }

        private IQueryOver<TrainingDay, TrainingDay> getTrainingDaysCriterias(WorkoutDaysSearchCriteria searchCriteria, IQueryOver<TrainingDay, TrainingDay> result)
        {
            result = result.ApplyUser(searchCriteria, Session, SecurityInfo);

            if (result == null)
            {
                return null;
            }
            if (searchCriteria.StartDate.HasValue)
            {
                result = result.Where(day => day.TrainingDate >= searchCriteria.StartDate);
            }
            if (searchCriteria.EndDate.HasValue)
            {
                result = result.Where(day => day.TrainingDate <= searchCriteria.EndDate);
            }
            return result;
        }

        public TrainingDayDTO GetTrainingDay(WorkoutDayGetOperation operationParams, RetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetTrainingDay:Username={0},operation={1}", SecurityInfo.SessionData.Profile.UserName, operationParams.Operation);

            var userIdentifier = SecurityInfo.SessionData.Profile.GlobalId;
            if (operationParams.UserId.HasValue)
            {
                userIdentifier = operationParams.UserId.Value;
            }
            var session = Session;
            using (var tx = session.BeginGetTransaction())
            {
                Profile userDb = session.Get<Profile>(userIdentifier);
                Profile profileDb = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                if (ServiceHelper.IsPrivateCalendar(userDb, profileDb))
                {
                    tx.Commit();
                    return null;
                }

                Customer dbCustomer = null;
                if (operationParams.CustomerId.HasValue)
                {
                    dbCustomer = session.Get<Customer>(operationParams.CustomerId.Value);
                    if (dbCustomer != null && dbCustomer.Profile != profileDb)
                    {
                        throw new CrossProfileOperationException("This customer doesn't belong to your profile");
                    }
                }
                //TODO:Refactor. For now there are two separate queries. try to use only one

                //var result = QueryOver.Of<TrainingDay>().Where(day => day.Customer == dbCustomer && day.Profile == userDb);
                var result = session.QueryOver<TrainingDay>().Where(day => day.Customer == dbCustomer && day.Profile == userDb);
                
                if (operationParams.Operation == GetOperation.Next)
                {
                    result = result.Where(td => td.TrainingDate > operationParams.WorkoutDateTime.Value).OrderBy(td => td.TrainingDate).Asc;
                }
                else if (operationParams.Operation == GetOperation.Previous)
                {
                    result = result.Where(td => td.TrainingDate < operationParams.WorkoutDateTime.Value).OrderBy(td => td.TrainingDate).Desc;
                }
                else if (operationParams.Operation == GetOperation.First)
                {
                    result = result.OrderBy(td => td.TrainingDate).Asc;
                }
                else if (operationParams.Operation == GetOperation.Last)
                {
                    result = result.OrderBy(td => td.TrainingDate).Desc;
                }
                else if (operationParams.Operation == GetOperation.Current)
                {
                    result = result.Where(td => td.TrainingDate == operationParams.WorkoutDateTime.Value).OrderBy(td => td.TrainingDate).Desc;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("GetOperation is not implemented");
                }
                result = (QueryOver<TrainingDay, TrainingDay>) result.Take(1);

                TrainingDay _temp = result.SingleOrDefault();
                if (_temp != null)
                {
                    var fetchQuery = session.QueryOver<TrainingDay>(() => _temp)
                        .Fetch(x => x.Objects).Eager
                        .Fetch(x => ((StrengthTrainingEntry)x.Objects.First()).MyPlace).Eager
                        .Fetch(x => x.Objects.First().LoginData).Eager
                        .Fetch(x => x.Objects.First().LoginData.ApiKey).Eager
                        .Fetch(x => x.Objects.First().Reminder).Eager
                        //.WithSubquery.WhereExists(result.Where(x => x.GlobalId == _temp.GlobalId).Select(x => x.GlobalId).Take(1));
                        .Where(x => x.GlobalId == _temp.GlobalId);
                    //fetchQuery = (IQueryOver<TrainingDay, TrainingDay>) fetchQuery;
                    var res = fetchQuery.SingleOrDefault();
                    if (res != null)
                    {
                        return Mapper.Map<TrainingDay, TrainingDayDTO>(res);
                    }
                }
                return null;
            }
        }
    }
}
