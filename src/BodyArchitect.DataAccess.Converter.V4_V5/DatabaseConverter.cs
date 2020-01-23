using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Model.Old;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transform;
using A6WEntry = BodyArchitect.Model.A6WEntry;
using AccountType = BodyArchitect.Model.AccountType;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using Exercise = BodyArchitect.Model.Exercise;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using LicenceInfo = BodyArchitect.Model.LicenceInfo;
using Profile = BodyArchitect.Model.Old.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using Serie = BodyArchitect.Model.Serie;
using SizeEntry = BodyArchitect.Model.SizeEntry;
using StrengthTrainingEntry = BodyArchitect.Model.StrengthTrainingEntry;
using StrengthTrainingItem = BodyArchitect.Model.StrengthTrainingItem;
using Suplement = BodyArchitect.Model.Suplement;
using SuplementsEntry = BodyArchitect.Model.SuplementsEntry;
using TrainingDay = BodyArchitect.Model.TrainingDay;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using BodyArchitect.DataAccess.Converter.V4_V5.SupplementsDefinitions;
using Message = BodyArchitect.Model.Message;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using PlatformType = BodyArchitect.Model.PlatformType;
using WP7PushNotification = BodyArchitect.Model.Old.WP7PushNotification;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    class BACallbackMock : IBADatabaseCallback
    {


        public void ConvertProgressChanged(BADatabaseCallbackParam param)
        {
            if (param.MainOperation != null)
            {
                Log.Write(param.MainOperation, TraceEventType.Information, "PayPal");
            }
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    public class DatabaseConverter : IBADatabaseConverter
    {
        private IBADatabaseCallback callback = new BACallbackMock();
        private Model.Profile deletedProfile;
        private Model.Exercise deletedExercise;
        private PaymentsHolder paymentsHolder;

        static DatabaseConverter()
        {
            //AutoMapperConfiguration.Configure();
        }
        public DatabaseConverter()
        {
            ClearCache = false;
            var paymentsManager = new PaymentsManager() ;
            var xmlStream = Helper.GetResource("BodyArchitect.DataAccess.Converter.V4_V5.BAPoints.xml");
            paymentsHolder=paymentsManager.Load(xmlStream);
            //callback=OperationContext.Current.GetCallbackChannel<IBADatabaseCallback>();
        }
        public DatabaseConverter(IBADatabaseCallback callback, PaymentsHolder payments)
            : this()
        {
            this.callback = callback;
            paymentsHolder = payments;
        }

        public DatabaseConverter(IBADatabaseCallback callback):this()
        {
            ClearCache = false;
            this.callback = callback;
        }

        public bool ClearCache { get; set; }

        public void CreateDb()
        {
            var newDbSession = NHibernateConfigurator.NewSessionFactory.OpenSession();

            SchemaExport exp = new SchemaExport(NHibernateConfigurator.NewDbConfiguration);
            exp.Execute(true, true, false, newDbSession.Connection, Console.Out);
        }

        public void FixTrainingPlanSetsPositions()
        {
            var newSession = NHibernateConfigurator.NewSessionFactory.OpenStatelessSession();
            var oldSession = NHibernateConfigurator.OldSessionFactory.OpenSession();

            try
            {
                using (var newDbTransaction = newSession.BeginTransaction())
                {
                    var oldPlans = oldSession.QueryOver<Model.Old.TrainingPlan>().Fetch(x => x.PlanContent).Eager.List();
                    var sets = newSession.QueryOver<Model.TrainingPlanSerie>().List().ToDictionary(x => x.GlobalId);
                    for (int index = 0; index < oldPlans.Count; index++)
                    {
                        var oldPlan = oldPlans[index];
                        var serializer = new XmlSerializationTrainingPlanFormatter(deletedExercise);
                        var newPlan = serializer.FromXml(newSession, oldPlan.PlanContent);

                        foreach (var trainingPlanDay in newPlan.Days)
                        {
                            foreach (var entry in trainingPlanDay.Entries)
                            {
                                int i = 0;
                                foreach (var trainingPlanSeries in entry.Sets)
                                {
                                    if(sets.ContainsKey(trainingPlanSeries.GlobalId))
                                    {
                                        var set = sets[trainingPlanSeries.GlobalId];
                                        //var set = newSession.Get<Model.TrainingPlanSerie>(trainingPlanSeries.GlobalId);
                                        set.Position = i;
                                        newSession.Update(set);
                                        i++;
                                    }
                                }

                            }

                        }
                    }
                    newDbTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                callback.ConvertProgressChanged(new BADatabaseCallbackParam("Error occured: " + ex.Message));
            }

            
        }

        
        void ensureClearCache(IStatelessSession newDbSession, ISession oldDbSession)
        {
            if (ClearCache)
            {
                oldDbSession.Clear();
            }
        }

        public void Convert()
        {
            var newDbSession = NHibernateConfigurator.NewSessionFactory.OpenStatelessSession();
            var oldDbSession = NHibernateConfigurator.OldSessionFactory.OpenSession();
            profilesMap = new Dictionary<int, Model.Profile>();
            

            try
            {
                using (newDbSession)
                using (oldDbSession)
                using (var newDbTransaction = newDbSession.BeginTransaction(IsolationLevel.Serializable))
                {
                    oldDbSession.FlushMode = FlushMode.Never;
                    
                    Log.Write("START", TraceEventType.Information, "PayPal");
                    callback.ConvertProgressChanged(new BADatabaseCallbackParam("Creating db structure..."));
                    //generate new db schema
                    //#if !UnitTest
                    //var query = newDbSession.CreateSQLQuery("drop schema `2629_ba_test`;");
                    //query.ExecuteUpdate();
                    //query = newDbSession.CreateSQLQuery("CREATE SCHEMA `2629_ba_test` ;");
                    //query.ExecuteUpdate();
                    //query = newDbSession.CreateSQLQuery("USE `2629_ba_test` ;");
                    //query.ExecuteUpdate();

                    //SchemaExport exp = new SchemaExport(NHibernateConfigurator.NewDbConfiguration);
                    //exp.Execute(true, true, false, newDbSession.Connection, Console.Out);
                    //#endif


                    callback.ConvertProgressChanged(new BADatabaseCallbackParam("Db structure created. Adding common objects..."));
                    addCommonObjects(oldDbSession, newDbSession);


                    convertProfiles(oldDbSession, newDbSession, callback);

                    ensureClearCache(newDbSession, oldDbSession);
                    oldDbSession.QueryOver<Model.Old.Profile>().Where(x => !x.IsDeleted).List();
                    convertExercises(oldDbSession, newDbSession, callback);
                    convertSupplements(oldDbSession, newDbSession, callback);
                    convertTrainingPlans(oldDbSession, newDbSession, callback);
                    convertMyTrainings(oldDbSession, newDbSession, callback);
                    var exercisesRecords=convertTrainingDays(oldDbSession, newDbSession, callback);
                    addExercisesRecords(exercisesRecords,oldDbSession, newDbSession, callback);

                    ensureClearCache(newDbSession, oldDbSession);

                    oldDbSession.QueryOver<Model.Old.Profile>().Where(x => !x.IsDeleted).List();
                    addA6WEntriesToNotEndedTrainings(oldDbSession, newDbSession, callback);

                    ensureClearCache(newDbSession, oldDbSession);

                    convertProfileConnectedData(oldDbSession, newDbSession, callback);
                    convertMessages(oldDbSession, newDbSession, callback);
                    convertRatings(oldDbSession, newDbSession, callback);

                    ensureClearCache(newDbSession, oldDbSession);

                    addSupplementsDefinitions(newDbSession);
                    oldDbSession.QueryOver<Model.Old.Profile>().Where(x => !x.IsDeleted).List();
                    convertInvitations(oldDbSession, newDbSession, callback);
                    convertWp7PushNotifications(oldDbSession, newDbSession, callback);
                    convertLoginData(oldDbSession, newDbSession, callback);
                    updateA6WStatistics(oldDbSession, newDbSession, callback);

                    callback.ConvertProgressChanged(new BADatabaseCallbackParam("Convertion completed"));
                    try
                    {
                        newDbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Log.Write("ERROR on commit: {0}", TraceEventType.Warning, "PayPal", ex.Message);
                        ExceptionHandler.Default.Process(ex);
                        throw;
                    }
                    ensureClearCache(newDbSession, oldDbSession);
                    Log.Write("END OK", TraceEventType.Information, "PayPal");
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                callback.ConvertProgressChanged(new BADatabaseCallbackParam("Error occured: " + ex.Message));
                throw;
            }

        }

        private void addSupplementsDefinitions(IStatelessSession newDbSession)
        {
            var profile =newDbSession.QueryOver<Model.Profile>().Where(x => x.UserName == "BodyArchitect").SingleOrDefault();
            SupplementsDefinitionAnabolic1 plan = new SupplementsDefinitionAnabolic1();
            plan.Create(newDbSession, profile);
            var plan1 = new SupplementsDefinitionAnabolic2();
            plan1.Create(newDbSession, profile);
            var plan2 = new SupplementsDefinitionAnabolic3();
            plan2.Create(newDbSession, profile);
            var plan3 = new SupplementsDefinitionAnabolic4();
            plan3.Create(newDbSession, profile);
            var plan4 = new SupplementsDefinitionAnabolic5();
            plan4.Create(newDbSession, profile);
            var plan5 = new SupplementsDefinitionAnabolic6();
            plan5.Create(newDbSession, profile);
            var plan6 = new SupplementsDefinitionBCAA1();
            plan6.Create(newDbSession, profile);
            var plan7 = new SupplementsDefinitionBCAA2();
            plan7.Create(newDbSession, profile);
            var plan8 = new SupplementsDefinitionCreatineCycle_2DT();
            plan8.Create(newDbSession, profile);
            var plan9 = new SupplementsDefinitionStormShockTherapyCycle();
            plan9.Create(newDbSession, profile);
            var plan10 = new SupplementsDefinitionStormShockTherapyCycle_WithIncreasingDosageEveryWeek();
            plan10.Create(newDbSession, profile);
            var plan11 = new SupplementsDefinitionAnabolic7();
            plan11.Create(newDbSession, profile);
            var plan12 = new SupplementsDefinitionAnabolic8();
            plan12.Create(newDbSession, profile);

        }

        private void updateA6WStatistics(ISession oldDbSession, IStatelessSession newDbSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Update A6W statistics..."));
            //var profileWithA6W=newDbSession.QueryOver<A6WTraining>().Fetch(x=>x.Profile).Eager.Fetch(x=>x.Profile.Statistics).Eager.Select(x => x.Profile).TransformUsing(Transformers.DistinctRootEntity).List<Model.Profile>();
            Model.Profile prof = null;
            Model.ProfileStatistics stat = null;

            var profileWithA6W =
                newDbSession.QueryOver<A6WTraining>().JoinAlias(x => x.Profile, () => prof).JoinAlias(x=>prof.Statistics,()=>stat).Select(x => prof.GlobalId,x=>stat.GlobalId).List<object[]>();
            foreach (var profile in profileWithA6W)
            {
                Model.TrainingDay day = null;
                var a6WCount ="SELECT Count(*) FROM TrainingDay td,EntryObject eo,A6WEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId AND td.Customer_id is null";

                var query = newDbSession.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET A6WEntriesCount=({0}) WHERE GlobalId=:StatisticsId", a6WCount));
                query.SetGuid("StatisticsId", (Guid)profile[1]);
                query.SetGuid("ProfileId", (Guid)profile[0]);
                query.ExecuteUpdate();

                //var count =
                //    newDbSession.QueryOver<Model.A6WEntry>().JoinAlias(b => b.TrainingDay, () => day).Fetch(
                //        x => day.Profile).Eager.Where(x => day.Profile == profile).RowCount();    
                //profile.Statistics.A6WEntriesCount = count;
                //newDbSession.Update(profile.Statistics);
            }
            
        }

        private void addExercisesRecords(Dictionary<string, ExerciseProfileData> exercisesRecords, ISession oldSession, IStatelessSession newDbSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Adding exercises records..."));
            int index = 0;
            foreach (var exercisesRecord in exercisesRecords)
            {
                guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, exercisesRecords.Count));
                newDbSession.Insert(exercisesRecord.Value);
                index++;
            }
        }

        void addCommonObjects(ISession oldSession, IStatelessSession newSession)
        {
            deletedProfile = new Model.Profile();
            deletedProfile.UserName = "(Deleted)";
            deletedProfile.Email = "admin1@MYBASERVICE.TK";
            deletedProfile.Birthday = DateTime.Now.AddYears(-30);
            deletedProfile.Privacy = new Model.ProfilePrivacy();
            deletedProfile.Privacy.Searchable = false;
            deletedProfile.IsDeleted = true;
            deletedProfile.CountryId = Country.GetByTwoLetters("PL").GeoId;
            deletedProfile.DataInfo = new Model.DataInfo();
            deletedProfile.Settings = new Model.ProfileSettings();
            deletedProfile.Licence = new Model.LicenceInfo();
            deletedProfile.Licence.AccountType = Model.AccountType.User;
            newSession.Insert(deletedProfile.DataInfo);
            newSession.Insert(deletedProfile.Settings);
            newSession.Insert(deletedProfile);

            var apiKey = new Model.APIKey();
            apiKey.ApiKey = new Guid("14375345-3755-46f7-af3f-0d328e3a2cc0");
            apiKey.ApplicationName = "BodyArchitect";
            apiKey.EMail = "romanp81@gmail.com";
            apiKey.Platform = PlatformType.Windows;
            apiKey.RegisterDateTime = DateTime.UtcNow;
            newSession.Insert(apiKey);

            apiKey = new Model.APIKey();
            apiKey.ApiKey = new Guid("A3C9D236-2566-40CF-A430-0802EE439B9C");
            apiKey.ApplicationName = "BodyArchitect for WP7";
            apiKey.EMail = "romanp81@gmail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            apiKey.Platform = PlatformType.WindowsPhone;
            newSession.Insert(apiKey);

            apiKey = new Model.APIKey();
            apiKey.ApiKey = new Guid("87AFE52C-9EAC-4BAF-949C-08E919285ADA");
            apiKey.ApplicationName = "BodyArchitect for iPhone";
            apiKey.EMail = "romanp81@gmail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            apiKey.Platform = PlatformType.iPhone;
            newSession.Insert(apiKey);

            deletedExercise = new Exercise(Guid.NewGuid());
            deletedExercise.Name = "(Missing)";
            deletedExercise.Shortcut = "__D";
            deletedExercise.IsDeleted = true;
            newSession.Insert(deletedExercise);
        }

        Dictionary<int, Model.Profile> profilesMap = new Dictionary<int, Model.Profile>();
        Dictionary<int, Guid> a6WTrainingsMap = new Dictionary<int, Guid>();
        Dictionary<Guid, MyPlace> profileMyPlacesMap = new Dictionary<Guid, MyPlace>();

        void convertProfiles(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            //retrieve all users with Full version of WP7 app
            var fullVersionProfilesResults=oldSession.QueryOver<Model.Old.LoginData>().WhereRestrictionOn(c => c.ApplicationVersion).IsLike("Full %").
                Select(
                    Projections.Group<Model.Old.LoginData>(e => e.ProfileId),
                    Projections.Count<Model.Old.LoginData>(e => e.ProfileId)).List<object[]>();
            var fullVersionProfiles = fullVersionProfilesResults.ToDictionary(x => (int)x[0], x => (int)x[1]);

            
            //retrieve users with more than 49 logins
            var manyLoginsUsersResults = oldSession.QueryOver<Model.Old.LoginData>().
                Select(
                    Projections.Group<Model.Old.LoginData>(e => e.ProfileId),
                    Projections.Count<Model.Old.LoginData>(e => e.ProfileId)).List<object[]>();
            var manyLoginsUsers = manyLoginsUsersResults.ToDictionary(x => (int)x[0], x => (int)x[1]);


            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting profiles..."));
            var profiles = oldSession.QueryOver<Model.Old.Profile>()
                .Fetch(x => x.Settings).Eager
                .Fetch(x => x.Statistics).Eager
                .Fetch(x => x.Wymiary).Eager.Where(x => !x.IsDeleted).OrderBy(x=>x.Id).Asc.List();
            guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Found {0} profiles...", profiles.Count)));
            for (int index = 0; index < profiles.Count; index++)
            {
                var profile = profiles[index];
                guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Converting profile {0}", profile.UserName), index, profiles.Count));
                convertProfile(profile, oldSession, newSession, guest, fullVersionProfiles, manyLoginsUsers);
            }
            guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Sending to db....")));

        }

        void convertProfileConnectedData(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            
            var newStateSession=NHibernateConfigurator.NewSessionFactory.OpenSession(newSession.Connection);
            guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Converting favorites and friends....")));
            //favorites
            //this query in where we must return only profiles with any Favorites or Friends
            IList<Profile> allOld = null;
            try
            {
                allOld = oldSession.QueryOver<Model.Old.Profile>()
                .Fetch(x => x.FavoriteUsers).Eager
                .Fetch(x => x.Friends).Eager
                .Fetch(x => x.FavoriteWorkoutPlans).Eager
                .Where(x => !x.IsDeleted).List();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                throw;
            }


            for (int index = 0; index < allOld.Count; index++)
            {
                var oldProfile = allOld[index];
                try
                {


                    guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Favorites for profile {0}....", oldProfile.UserName), index, allOld.Count));
                    var profile = getNew(newSession, oldProfile);
                    if (profile == null)
                    {
                        continue;
                    }
                    if (oldProfile.FavoriteUsers.Count > 0)
                    {
                        //convert favorites
                        foreach (var favoriteUser in oldProfile.FavoriteUsers)
                        {
                            var newProfile = getNew(newSession, favoriteUser);
                            if (newProfile != null)
                            {
                                profile.FavoriteUsers.Add(newProfile);
                            }
                        }
                    }
                    if (oldProfile.Friends.Count > 0)
                    {
                        guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Friends for profile {0}....", profile.UserName), index, allOld.Count));
                        foreach (var friend in oldProfile.Friends)
                        {
                            var newProfile = getNew(newSession, friend);
                            if (newProfile != null)
                            {
                                profile.Friends.Add(newProfile);
                            }
                        }
                    }
                    if (oldProfile.FavoriteWorkoutPlans.Count > 0)
                    {
                        guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("FavoriteWorkoutPlans for profile {0}....", profile.UserName), index, allOld.Count));
                        foreach (var plan in oldProfile.FavoriteWorkoutPlans)
                        {
                            var newPlan = newSession.Get<Model.TrainingPlan>(plan.GlobalId);
                            if (newPlan != null)
                            {
                                profile.FavoriteWorkoutPlans.Add(newPlan);
                            }
                        }
                    }
                    newStateSession.Update(profile);
                    newStateSession.Flush();
                    newStateSession.Clear();
                }
                catch (Exception)
                {
                    Log.Write("error in convertProfileConnectedData: {0}", TraceEventType.Warning, "PayPal", oldProfile.Id);
                    throw;
                }
            }
        }

        void convertMyTrainings(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting myTrainings..."));
            var query = oldSession.CreateSQLQuery("update mytraining SEt PercentageCompleted=((( SELECT count(*) From entryobject where MyTraining_Id=mytraining.Id)/42.0)*100)");
            query.ExecuteUpdate();
            var oldDays = oldSession.QueryOver<Model.Old.MyTraining>().Fetch(x => x.Profile).Eager.TransformUsing(Transformers.DistinctRootEntity).List();
            foreach (var oldDay in oldDays)
            {
                var newTraining = oldDay.Map<A6WTraining>();
                if (oldDay.Profile.IsDeleted || !profilesMap.ContainsKey(oldDay.Profile.Id))
                {//for deleted person we can skip such training day
                    continue;
                }
                else
                {
                    newTraining.Profile = getNew(newSession,oldDay.Profile);
                }
                newSession.Insert(newTraining);
                a6WTrainingsMap.Add(oldDay.Id, newTraining.GlobalId);
            }
        }

        

        void addA6WEntriesToNotEndedTrainings(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Adding A6WEntries to not ended myTrainings..."));
            IList<A6WTraining> trainings = null;
            try
            {
                trainings = newSession.QueryOver<Model.A6WTraining>()
              .Fetch(x => x.EntryObjects).Eager
              .Fetch(x => x.Profile).Eager
              .Fetch(x => x.EntryObjects.First().TrainingDay).Eager.Where(x => x.TrainingEnd == TrainingEnd.NotEnded).TransformUsing(Transformers.DistinctRootEntity).List();

            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                throw;
            }

            for (int index = 0; index < trainings.Count; index++)
            {
                var training = trainings[index];
                guest.ConvertProgressChanged(new BADatabaseCallbackParam(string.Format("Adding a6w to {0}", training.Name), index, trainings.Count));
                DateTime lastDayDate = training.StartDate.Date.AddDays(-1);
                Console.WriteLine("InitDate1: "+lastDayDate.ToString());
                if (training.EntryObjects.Count > 0)
                {
                    lastDayDate = training.EntryObjects.OrderBy(x=>x.TrainingDay.TrainingDate).ElementAt(training.EntryObjects.Count - 1).TrainingDay.TrainingDate.Date;
                    Console.WriteLine("InitDate2: " + lastDayDate.ToString());
                }

                for (int i = training.EntryObjects.Count; i < 42; i++)
                {
                    try
                    {
                        var a6W = new Model.A6WEntry();
                        a6W.Status = EntryObjectStatus.Planned;
                        a6W.DayNumber = i + 1;
                        a6W.MyTraining = training;
                        lastDayDate = lastDayDate.Date.AddDays(1);
                        var test = newSession.QueryOver<Model.TrainingDay>()
                           .Fetch(x => x.Objects).Eager
                           .Fetch(x => x.Profile).Eager
                           .Where(x => x.Profile.GlobalId == training.Profile.GlobalId && x.TrainingDate == lastDayDate).TransformUsing(Transformers.DistinctRootEntity).List();
                        TrainingDay currentTrainingDay = test.Distinct().SingleOrDefault();
                        if (currentTrainingDay == null)
                        {
                            currentTrainingDay = new Model.TrainingDay(lastDayDate.Date);
                            currentTrainingDay.Profile = new Model.Profile() {GlobalId = training.Profile.GlobalId,Version=1};
                        }
                        currentTrainingDay.AddEntry(a6W);
                        training.EntryObjects.Add(a6W);
                        a6W.MyTraining = training;
                        if (currentTrainingDay.GlobalId == Guid.Empty)
                        {
                            newSession.Insert(currentTrainingDay);
                        }
                        else
                        {
                            newSession.Update(currentTrainingDay);
                        }
                        newSession.Insert(a6W);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Default.Process(ex);
                        Log.Write("error in addA6WEntriesToNotEndedTrainings: {0}", TraceEventType.Warning, "PayPal", training.GlobalId);
                        throw;
                    }


                }

                int doneEntriesCount = training.EntryObjects.Where(x => x.Status == EntryObjectStatus.Done).Count();
                training.PercentageCompleted = (int)((doneEntriesCount / (double)42) * 100);
                newSession.Update(training);
            }

            

            guest.ConvertProgressChanged(new BADatabaseCallbackParam("FINISH Adding A6WEntries to not ended myTrainings"));
            //trainings = newSession.QueryOver<Model.A6WTraining>().Fetch(x => x.EntryObjects).Eager.Fetch(x => x.EntryObjects.First().TrainingDay).Eager.Where(x => x.TrainingEnd == TrainingEnd.Complete).TransformUsing(Transformers.DistinctRootEntity).List();
            //foreach (var a6WTraining in trainings)
            //{
            //    int doneEntriesCount = a6WTraining.EntryObjects.Where(x => x.Status == EntryObjectStatus.Done).Count();
            //    a6WTraining.PercentageCompleted = (int)((doneEntriesCount / (double)42) * 100);
            //    newSession.SaveOrUpdate(a6WTraining);
            //}
        }

        Dictionary<string, ExerciseProfileData> convertTrainingDays(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting training days..."));
            Model.Old.Profile oldProfile = null;
            IList<Model.Old.TrainingDay> oldDays = null;
            var exercisesRecord = new Dictionary<string, ExerciseProfileData>();

            try
            {
                oldDays=oldSession.QueryOver<Model.Old.TrainingDay>().JoinAlias(x => x.Profile, () => oldProfile).Where(x => !oldProfile.IsDeleted)
                .Fetch(x => x.Objects).Eager
                .Fetch(x => ((Model.Old.SuplementsEntry)x.Objects.First()).Items).Eager
                .Fetch(x => ((Model.Old.BlogEntry)x.Objects.First()).Comments).Eager
                .Fetch(x => ((Model.Old.StrengthTrainingEntry)x.Objects.First()).Entries).Eager
                .Fetch(x => (((Model.Old.StrengthTrainingEntry)x.Objects.First()).Entries.First().Series)).Eager
                .Fetch(x => x.Objects.First().MyTraining).Eager.TransformUsing(Transformers.DistinctRootEntity).List();
                //oldDays = oldSession.QueryOver<Model.Old.TrainingDay>().JoinAlias(x => x.Profile, () => oldProfile).Where(x => !oldProfile.IsDeleted).List();


                foreach (var oldDay in oldDays)
                {
                    oldDay.Objects = oldDay.Objects.Distinct().Cast<Model.Old.EntryObject>().ToList();
                    foreach (var oldEntry in oldDay.Objects)
                    {
                        var oldStrength = oldEntry as Model.Old.StrengthTrainingEntry;
                        var oldSupple = oldEntry as Model.Old.SuplementsEntry;
                        if (oldSupple != null)
                        {
                            oldSupple.Items = oldSupple.Items.Distinct().Cast<Model.Old.SuplementItem>().ToList();
                        }
                        if (oldStrength != null)
                        {
                            oldStrength.Entries = oldStrength.Entries.Distinct().Cast<Model.Old.StrengthTrainingItem>().ToList();
                            foreach (var oldItem in oldStrength.Entries)
                            {
                                oldItem.Series = oldItem.Series.Distinct().Cast<Model.Old.Serie>().ToList();    
                            }
                        }
                    }

                    
                }
                

            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
                throw;
            }

            

            for (int index = 0; index < oldDays.Count; index++)
            {
                var oldDay = oldDays[index];
                if (oldDay.IsEmpty || oldDay.TrainingDate<new DateTime(1200,1,1))
                {
                    continue;
                }
                try
                {


                    guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldDays.Count));
                    var newDay = oldDay.Map<TrainingDay>();
                    if (oldDay.Profile.IsDeleted || !profilesMap.ContainsKey(oldDay.Profile.Id))
                    {//for deleted person we can skip such training day
                        //continue;
                        newDay.Profile = deletedProfile;
                    }
                    else
                    {
                        newDay.Profile = getNew(newSession,oldDay.Profile);
                    }

                    newSession.Insert(newDay);
                    
                    List<TrainingDayComment> comments = new List<TrainingDayComment>();
                    for (int i = 0; i < oldDay.Objects.Count(); i++)
                    {
                        
                        var oldEntry = oldDay.Objects.ElementAt(i);
                        var oldStrength = oldEntry as Model.Old.StrengthTrainingEntry;
                        var oldBlog = oldEntry as Model.Old.BlogEntry;
                        var oldSupple = oldEntry as Model.Old.SuplementsEntry;
                        var oldA6w = oldEntry as Model.Old.A6WEntry;
                        var oldSize = oldEntry as Model.Old.SizeEntry;
                        if(oldSize!=null)
                        {
                            var newSize = (SizeEntry)newDay.Objects.ElementAt(i);
                            newSession.Insert(newSize.Wymiary);
                            newSession.Insert(newSize);
                        }
                        if (oldStrength != null)
                        {
                            var newStrength = (StrengthTrainingEntry)newDay.Objects.ElementAt(i);
                            newStrength.MyPlace = GetDefaultMyPlace(newSession, newDay.Profile);
                            newSession.Insert(newStrength);

                            bool generatePositionFromOrder = false;
                            if (oldStrength.Entries.Count > 1 && oldStrength.Entries.Where(x=>x.Position>0).Count()==0)
                            {//old entries doesn't have Position set (they all have 0 so we must take Postion number from order)
                                generatePositionFromOrder = true;
                            }
                            int itemPosition = 0;
                            for (int index1 = 0; index1 < oldStrength.Entries.Count; index1++)
                            {
                                var oldItem = oldStrength.Entries.ElementAt(index1);
                                var newItem = newStrength.Entries.ElementAt(index1);
                                newItem.Exercise = newSession.Get<Exercise>(oldItem.ExerciseId);
                                if (newItem.Exercise == null)
                                {
                                    newItem.Exercise = deletedExercise;
                                }
                                if (generatePositionFromOrder)
                                {
                                    newItem.Position = itemPosition;
                                }
                                newSession.Insert(newItem);

                                itemPosition++;

                                //SerieValidator serieValidator = new SerieValidator();
                                //var incorrectSets = serieValidator.GetIncorrectSets(newItem);
                                //if (incorrectSets.Count > 0)
                                //{
                                //    StringBuilder builder = new StringBuilder();
                                //    foreach (var series1 in newItem.Series)
                                //    {
                                //        builder.AppendFormat("{0}, ", series1);
                                //    }
                                //    Log.Write("Wrong set: {0} (item id = {1})", TraceEventType.Warning, "PayPal", builder, newItem.GlobalId);
                                //}

                                SerieValidator validator = new SerieValidator();
                                //check correctness of every set
                                foreach (var setDTO in newItem.Series)
                                {
                                    setDTO.IsIncorrect = !validator.IsCorrect(setDTO);
                                }

                                int position = 0;
                                foreach (var series in newStrength.Entries.ElementAt(index1).Series)
                                {
                                    if (newItem.Exercise.ExerciseType==ExerciseType.Cardio)
                                    {//for cardio sets we must replace repetitions with weight
                                        var rep=series.RepetitionNumber;
                                        var weight = series.Weight;
                                        series.RepetitionNumber = weight;
                                        series.Weight = rep;
                                    }
                                    if(series.RepetitionNumber==null)
                                    {
                                        
                                    }
                                    series.Position = position;
                                    ensureExerciseRecord(newItem, series, newDay, exercisesRecord);
                                    newSession.Insert(series);
                                    position++;
                                }
                            }
                        }
                        if (oldBlog != null)
                        {
                            //if(newDay.TrainingDate==new DateTime(2012,5,6))
                            //{
                                
                            //}
                            newSession.Insert(newDay.Objects.ElementAt(i));
                            newDay.LastCommentDate = oldBlog.LastCommentDate;
                            newDay.AllowComments = oldBlog.AllowComments;
                            newSession.Update(newDay);
                            foreach (var blogComment in oldBlog.Comments)
                            {
                                var trainingDayComment = blogComment.Map<TrainingDayComment>();
                                trainingDayComment.TrainingDay = newDay;
                                if (blogComment.Profile.IsDeleted || !profilesMap.ContainsKey(blogComment.Profile.Id))
                                {
                                    trainingDayComment.Profile = deletedProfile;
                                }
                                else
                                {
                                    trainingDayComment.Profile = getNew(newSession,blogComment.Profile);
                                }
                                comments.Add(trainingDayComment);
                            }
                        }
                        if (oldSupple != null)
                        {
                            var newSupple = (SuplementsEntry)newDay.Objects.ElementAt(i);
                            newSession.Insert(newSupple);
                            int u = 0;
                            foreach (var item in newSupple.Items)
                            {
                                item.Suplement = newSession.Get<Suplement>(oldSupple.Items.ElementAt(u).SuplementId);
                                newSession.Insert(item);
                                u++;
                            }
                        }
                        if (oldA6w != null)
                        {
                            var newA6W = (A6WEntry)newDay.Objects.ElementAt(i);
                            newA6W.MyTraining = newSession.Get<A6WTraining>(a6WTrainingsMap[oldA6w.MyTraining.Id]);
                            newSession.Insert(newA6W);
                            //newA6W.MyTraining.EntryObjects.Add(newA6W);
                        }
                    }

                    
                    oldSession.Evict(oldDay);
                    foreach (var dayComment in comments)
                    {
                        try
                        {
                            newSession.Insert(dayComment);
                        }
                        catch (Exception)
                        {
                            Log.Write("error in training day comment: {0} ({1}). Comment id:{2}", TraceEventType.Warning, "PayPal", oldDay.TrainingDate, oldDay.Id, dayComment.GlobalId);
                            throw;
                        }

                    }
                }
                catch (Exception)
                {
                    Log.Write("error in training day: {0} ({1})", TraceEventType.Warning, "PayPal", oldDay.TrainingDate, oldDay.Id);
                    throw;
                }

            }
            return exercisesRecord;
        }


        private static void ensureExerciseRecord(StrengthTrainingItem newItem, Serie series, TrainingDay newDay,
                                                 Dictionary<string, ExerciseProfileData> exercisesRecord)
        {
            if (!newItem.Exercise.IsDeleted && series.Weight.HasValue && ((series.RepetitionNumber.HasValue && series.RepetitionNumber>0) || newItem.Exercise.ExerciseType==ExerciseType.Cardio) && series.Weight>0 )
            {
                SerieValidator serieValidator = new SerieValidator();
                if (!serieValidator.IsCorrect(series))
                {
                    return;
                }
                ExerciseProfileData record = new ExerciseProfileData();
                string key = string.Format("{0}{1}", newDay.Profile.GlobalId, newItem.Exercise.GlobalId);
                if (!exercisesRecord.ContainsKey(key))
                {
                    exercisesRecord.Add(key, record);
                }
                else
                {
                    record = exercisesRecord[key];
                }
                if (record.MaxWeight < series.Weight || (record.MaxWeight == series.Weight && record.Repetitions < series.RepetitionNumber))
                {
                    record.Profile = newDay.Profile;
                    record.Exercise = newItem.Exercise;
                    record.Serie = series;
                    record.MaxWeight = (decimal)series.Weight;
                    record.Repetitions = series.RepetitionNumber;
                    record.TrainingDate = newDay.TrainingDate;
                }
            }
        }

        Model.MyPlace GetDefaultMyPlace(IStatelessSession newSession, Model.Profile profile)
        {
            //return newSession.QueryOver<Model.MyPlace>().Where(x => x.Profile == profile && x.IsDefault).SingleOrDefault();
            return profileMyPlacesMap[profile.GlobalId];
        }

        void convertTrainingPlans(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting training plans..."));
            var oldPlans = oldSession.QueryOver<Model.Old.TrainingPlan>().Fetch(x => x.PlanContent).Eager.Fetch(x => x.Profile).Eager.List();
            for (int index = 0; index < oldPlans.Count; index++)
            {
                var oldPlan = oldPlans[index];
                try
                {
                    guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldPlans.Count));

                    var serializer = new XmlSerializationTrainingPlanFormatter(deletedExercise);
                    var newPlan = serializer.FromXml(newSession, oldPlan.PlanContent);

                    if (oldPlan.Profile != null)
                    {
                        if (oldPlan.Profile.IsDeleted || !profilesMap.ContainsKey(oldPlan.Profile.Id))
                        {
                            newPlan.Profile = deletedProfile;
                        }
                        else
                        {
                            newPlan.Profile = getNew(newSession,oldPlan.Profile);
                        }

                    }
                    if (oldPlan.Status == Model.Old.PublishStatus.Published)
                    {
                        newPlan.Status = PublishStatus.Published;
                        newPlan.PublishDate = oldPlan.PublishDate;
                        newPlan.Rating = oldPlan.Rating;
                    }
                    newSession.Insert(newPlan);
                    foreach (var trainingPlanDay in newPlan.Days)
                    {
                        newSession.Insert(trainingPlanDay);
                        foreach (var entry in trainingPlanDay.Entries)
                        {
                            newSession.Insert(entry);
                            foreach (var trainingPlanSeries in entry.Sets)
                            {
                                newSession.Insert(trainingPlanSeries);
                            }

                        }

                    }
                }
                catch (Exception)
                {
                    Log.Write("error in training plan: {0} ({1})", TraceEventType.Warning, "PayPal", oldPlan.Name, oldPlan.GlobalId);
                    throw;
                }

            }
        }

        void convertExercises(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting exercises..."));
            //TODO:we can remove this fetch
            var oldExercises = oldSession.QueryOver<Model.Old.Exercise>().Fetch(x => x.Profile).Eager.List();
            for (int index = 0; index < oldExercises.Count; index++)
            {
                var oldExercise = oldExercises[index];
                try
                {
                    guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldExercises.Count));
                    Model.Profile newProfile = null;
                    if (oldExercise.Profile != null)
                    {
                        if (oldExercise.Profile.IsDeleted || !profilesMap.ContainsKey(oldExercise.Profile.Id))
                        {
                            continue;
                        }
                        else
                        {
                            newProfile = getNew(newSession,oldExercise.Profile);
                        }

                    }
                    var newExercise = oldExercise.Map<Model.Exercise>();
                    newExercise.CreationDate = DateTime.UtcNow;
                    newExercise.Profile = newProfile;
                    if (newExercise.GlobalId == new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"))
                    {//WYCISKANIE SZTANGI W LEŻENIU NA ŁAWCE POZIOMEJ
                        newExercise.UseInRecords = true;
                    }
                    else if (newExercise.GlobalId == new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"))
                    {//martwy ciąg
                        newExercise.UseInRecords = true;
                    }
                    else if (newExercise.GlobalId == new Guid("3e06a130-b811-4e45-9285-f087403615bf"))
                    {//PRZYSIADY ZE SZTANGĄ NA BARKACH
                        newExercise.UseInRecords = true;
                    }
                    else if (newExercise.GlobalId == new Guid("aef35059-4c40-4de3-9ff3-2df417a9658b"))
                    {//Wyciskanie sztangi zza głowy
                        newExercise.UseInRecords = true;
                    }
                    newSession.Insert(newExercise);
                }
                catch (Exception)
                {
                    Log.Write("error in exercise: {0} ({1})", TraceEventType.Warning, "PayPal", oldExercise.Name, oldExercise.GlobalId);
                    throw;
                }
            }
        }

        void convertInvitations(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting invitations..."));
            var oldInvitations = oldSession.QueryOver<Model.Old.FriendInvitation>().List();
            foreach (var oldInvitation in oldInvitations)
            {
                var newInvitation = oldInvitation.Map<Model.FriendInvitation>();

                if (oldInvitation.Inviter == null || oldInvitation.Invited == null || oldInvitation.Invited.IsDeleted || !profilesMap.ContainsKey(oldInvitation.Invited.Id) || oldInvitation.Inviter.IsDeleted || !profilesMap.ContainsKey(oldInvitation.Inviter.Id))
                {
                    continue;
                }
                newInvitation.Inviter = getNew(newSession,oldInvitation.Inviter);
                newInvitation.Invited = getNew(newSession,oldInvitation.Invited);
                newSession.Insert(newInvitation);
            }
        }


        void convertMessages(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting messages..."));
            var oldMessages = oldSession.QueryOver<Model.Old.Message>().List();
            for (int index = 0; index < oldMessages.Count; index++)
            {
                var oldMessage = oldMessages[index];
                if (oldMessage.MessageType!=MessageType.Custom)
                {
                    continue;
                }
                guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldMessages.Count));
                Model.Profile newSender = null;
                Model.Profile newReceiver = null;
                if (oldMessage.Receiver != null)
                {
                    if (oldMessage.Receiver.IsDeleted || !profilesMap.ContainsKey(oldMessage.Receiver.Id))
                    {
                        //skip this message if the receiver is deleted
                        continue;
                    }
                    
                    newReceiver = getNew(newSession,oldMessage.Receiver);
                }
                if (oldMessage.Sender != null)
                {
                    
                    if (oldMessage.Sender.IsDeleted || !profilesMap.ContainsKey(oldMessage.Sender.Id))
                    {
                        newSender = deletedProfile;
                    }
                    else
                    {
                        newSender = getNew(newSession,oldMessage.Sender);
                    }
                }
                var newMessage = oldMessage.Map<Model.Message>();
                newMessage.Sender = newSender;
                newMessage.Receiver = newReceiver;
                newSession.Insert(newMessage);
            }
        }

        void convertSupplements(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting supplements..."));
            var oldSupplements = oldSession.QueryOver<Model.Old.Suplement>().List();
            for (int index = 0; index < oldSupplements.Count; index++)
            {
                var oldSupplement = oldSupplements[index];
                try
                {
                    var newSupplement = oldSupplement.Map<Model.Suplement>();
                    guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldSupplements.Count));
                    //for steroids and PH mark CanBeIllegal
                    if (newSupplement.GlobalId == new Guid("D8F8FD0D-31E0-4763-9F1E-ED5AE49DFBD8") ||
                        newSupplement.GlobalId == new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9"))
                    {
                        newSupplement.CanBeIllegal = true;
                    }
                    newSupplement.CreationDate = DateTime.UtcNow;
                    newSession.Insert(newSupplement);
                }
                catch (Exception)
                {
                    Log.Write("error in supplement: {0} ({1})", TraceEventType.Warning, "PayPal", oldSupplement.Name, oldSupplement.SuplementId);
                    throw;
                }

            }
        }

        void convertRatings(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting ratings..."));
            var oldRatings = oldSession.QueryOver<Model.Old.RatingUserValue>().List();
            for (int index = 0; index < oldRatings.Count; index++)
            {
                var oldRating = oldRatings[index];
                guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, oldRatings.Count));
                if (profilesMap.ContainsKey(oldRating.ProfileId))
                {
                    var newRating = oldRating.Map<Model.RatingUserValue>();
                    newRating.ProfileId = profilesMap[oldRating.ProfileId].GlobalId;
                    newSession.Insert(newRating);
                }
            }
        }


        Model.Profile getNew(IStatelessSession newSession, Profile oldProfile)
        {
            if (profilesMap.ContainsKey(oldProfile.Id))
            {
                //return newSession.Get<Model.Profile>(profilesMap[oldProfile.Id]);
                return profilesMap[oldProfile.Id];
            }
            return null;
        }

        private Model.Profile adminProfile;

        Model.Profile getAdminProfile(IStatelessSession newSession)
        {
            if (adminProfile == null)
            {
                adminProfile = newSession.QueryOver<Model.Profile>().Where(x => x.UserName == "admin").SingleOrDefault();
            }
            return adminProfile;
        }

        private void convertProfile(Profile profile, ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest, Dictionary<int, int> fullVersionProfiles, Dictionary<int, int> manyLoginsUsers)
        {
            try
            {
                var newProfile = profile.Map<Model.Profile>();
                newProfile.Licence = new LicenceInfo();
                string welcomeMessageKey = "Message_NormalProfile";
                //all normal users will have premium account for 30 days
                newProfile.Licence.AccountType = profile.Role == Model.Old.Role.Administrator ? AccountType.Administrator : AccountType.PremiumUser;
                newProfile.Licence.BAPoints = paymentsHolder.GetPoints(Service.V2.Model.AccountType.PremiumUser).Points * 30 ;
                //for users with Full version of WP7 app we give them 6 month of premium user
                if (fullVersionProfiles.ContainsKey(profile.Id) && fullVersionProfiles[profile.Id]>0)
                {
                    newProfile.Licence.BAPoints *= 6;
                    welcomeMessageKey = "Message_FullWP7Profile";
                }
                //for another users (without paid version) check how many logins this user has
                else if (manyLoginsUsers.ContainsKey(profile.Id) && manyLoginsUsers[profile.Id]>49)
                {
                    newProfile.Licence.BAPoints *= 3;
                    welcomeMessageKey = "Message_AdvancedProfile";
                }
                
                newProfile.Licence.LastPointOperationDate = DateTime.UtcNow;
                newProfile.DataInfo = new DataInfo();
                if (newProfile.Settings != null)
                {
                    newSession.Insert(newProfile.Settings);
                }
                if (newProfile.Statistics != null)
                {
                    newSession.Insert(newProfile.Statistics);
                }
                if (newProfile.Wymiary != null)
                {
                    newSession.Insert(newProfile.Wymiary);
                }
                newSession.Insert(newProfile.DataInfo);
                var t = newSession.Insert(newProfile);
                createDefaultMyPlace(newProfile, oldSession, newSession, callback);
                profilesMap.Add(profile.Id, newProfile);
                if(newProfile.UserName=="admin")
                {
                    adminProfile = newProfile;
                }
                else
                {
                    //add hello message
                    var culture = newProfile.GetProfileCulture();
                    var content = Strings.ResourceManager.GetString(welcomeMessageKey, culture);
                    var topic = Strings.ResourceManager.GetString("Message_Topic", culture);
                    Message msg = new Message();
                    msg.Content = content;
                    msg.Topic = topic;
                    msg.Receiver = newProfile;
                    msg.CreatedDate = DateTime.UtcNow;
                    msg.Priority = MessagePriority.System;
                    msg.Sender = getAdminProfile(newSession);
                    newSession.Insert(msg);
                }

                
            }
            catch (Exception)
            {
                Log.Write("error in profile: {0} ({1})", TraceEventType.Warning, "PayPal", profile.UserName, profile.Id);

                throw;
            }

        }

        void convertWp7PushNotifications(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting WP7PUshNotifications..."));
            var list = oldSession.QueryOver<Model.Old.WP7PushNotification>().List();
            for (int index = 0; index < list.Count; index++)
            {
                var oldWp7 = list[index];
                try
                {

                    if (profilesMap.ContainsKey(oldWp7.ProfileId))
                    {
                        guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, list.Count));
                        var newWp7 = oldWp7.Map<Model.WP7PushNotification>();
                        newWp7.ProfileId = profilesMap[oldWp7.ProfileId].GlobalId;
                        newSession.Insert(newWp7);
                    }
                }
                catch (Exception)
                {
                    Log.Write("error in WP7PushNotification: {0}", TraceEventType.Warning, "PayPal", oldWp7.DeviceID);

                    throw;
                }

            }
        }

        void convertLoginData(ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            guest.ConvertProgressChanged(new BADatabaseCallbackParam("Converting LoginData..."));
            var list = oldSession.QueryOver<Model.Old.LoginData>().List();
            for (int index = 0; index < list.Count; index++)
            {
                var oldLoginData = list[index];
                try
                {

                    if (profilesMap.ContainsKey(oldLoginData.ProfileId))
                    {
                        guest.ConvertProgressChanged(new BADatabaseCallbackParam(null, index, list.Count));
                        var newLoginData = oldLoginData.Map<Model.LoginData>();
                        newLoginData.ProfileId = profilesMap[oldLoginData.ProfileId].GlobalId;
                        newSession.Insert(newLoginData);
                    }
                }
                catch (Exception)
                {
                    Log.Write("error in LoginData: {0}", TraceEventType.Warning, "PayPal", oldLoginData.Id);

                    throw;
                }

            }

        }

        void createDefaultMyPlace(Model.Profile profile, ISession oldSession, IStatelessSession newSession, IBADatabaseCallback guest)
        {
            MyPlace defaultMyPlace = new MyPlace();
            defaultMyPlace.Name = "Default";
            defaultMyPlace.IsSystem = true;
            defaultMyPlace.IsDefault = true;
            defaultMyPlace.Color = Color.LightSteelBlue.ToColorString();
            defaultMyPlace.Profile = profile;
            defaultMyPlace.CreationDate = DateTime.UtcNow;
            newSession.Insert(defaultMyPlace);
            profileMyPlacesMap.Add(profile.GlobalId, defaultMyPlace);
        }
    }
}
