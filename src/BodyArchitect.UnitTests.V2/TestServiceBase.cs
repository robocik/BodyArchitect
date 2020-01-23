using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AutoMapper;
using BodyArchitect.Client.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Localization;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Service.V2.Services;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using NHibernate;
using AccountType = BodyArchitect.Model.AccountType;
using ChampionshipType = BodyArchitect.Model.ChampionshipType;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Gender = BodyArchitect.Model.Gender;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using PlatformType = BodyArchitect.Model.PlatformType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;
using SupplementCycleDayRepetitions = BodyArchitect.Service.V2.Model.SupplementCycleDayRepetitions;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.V2
{
    public class MockEmailService:IEMailService
    {
        public bool EMailSent { get; private set; }

        public void SendEMail(Profile profile, string subject, string message, params object[] args)
        {
            EMailSent = true;
        }

        public void NewSendEMail(Profile profile, string subject, string message)
        {
            EMailSent = true;
        }

        public void Reset()
        {
            EMailSent = false;
        }
    }

    public class NormalMethodInvoker:IMethodInvoker
    {
        public void Invoke(WaitCallback method,object param)
        {
            method(param);
        }
    }
    class MockPushNotificationService:IPushNotificationService
    {
        public void SendLiveTile(ISession session, Profile user,object param)
        {
            
        }

        public void SendLiveTile(string uri, string backgroundImageUrl, string title, int count)
        {
            
        }
    }

    public abstract class TestServiceBase : NHibernateTestFixtureBase
    {

        private SecurityManager manager;
        private ClientInformation clientInformation;
        private MockTimerService timerService;
        public string ImagesFolder { get; private set; }
        private PaymentsHolder paymentsManager;

        protected void configureLogging()
        {
            BodyArchitect.Logger.Log.EnableExceptionLog = false;
            BodyArchitect.Logger.Log.EnableStandardLog = false;
        }


        public static TrainingPlan CreatePlan(ISession session, Profile profile1, string name, TrainingPlanDifficult difficult = TrainingPlanDifficult.Beginner, TrainingType type = TrainingType.Split, bool isPublished = true, string language = "en", WorkoutPlanPurpose purpose = WorkoutPlanPurpose.Mass, int days = 2)
        {
            var workoutPlan = new TrainingPlan();
            workoutPlan.GlobalId = Guid.NewGuid();
            workoutPlan.Profile = profile1;
            workoutPlan.Name = name;
            workoutPlan.Purpose = purpose;
            workoutPlan.Language = language;
            workoutPlan.TrainingType = type;
            workoutPlan.Difficult = difficult;
            workoutPlan.Author = "test";
            workoutPlan.Status = isPublished ? PublishStatus.Published : PublishStatus.Private;
            if (isPublished)
            {
                workoutPlan.PublishDate = DateTime.UtcNow;
            }
            for (int i = 0; i < days; i++)
            {
                var day = new TrainingPlanDay();
                day.Name = "Day" + i;
                workoutPlan.Days.Add(day);
                day.TrainingPlan = workoutPlan;
            }
            session.Save(workoutPlan);
            session.Flush();
            workoutPlan.Tag = Mapper.Map<TrainingPlan, Service.V2.Model.TrainingPlans.TrainingPlan>(workoutPlan);
            return workoutPlan;
        }

        protected ProfileDTO GetLoggedProfile(Guid profileId)
        {
            Cache myCache = (Cache)SecurityManager.Cache.GetType().GetField("realCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(SecurityManager.Cache);

            foreach (var Item in myCache.CurrentCacheState.Values)
            {
                CacheItem CacheItem = (CacheItem)Item;
                SecurityInfo val = (SecurityInfo)CacheItem.Value;
                if (val.SessionData.Profile.GlobalId == profileId)
                {
                    return val.SessionData.Profile;
                }
            }
            return null;
        }

        public static Exercise CreateExercise(ISession session, Profile profile, string name, string shortCut, ExerciseType exerciseType = ExerciseType.Klatka, MechanicsType mechanicsType = MechanicsType.Compound, ExerciseForceType forceType = ExerciseForceType.Push, ExerciseDifficult difficult = ExerciseDifficult.One,Guid? globalId=null)
        {
            if(globalId==null)
            {
                globalId = Guid.NewGuid();
            }
            Exercise exercise = new Exercise(globalId.Value);
            exercise.Profile = profile;
            exercise.Name = name;
            exercise.Shortcut = shortCut;
            exercise.ExerciseType = exerciseType;
            exercise.ExerciseForceType = forceType;
            exercise.MechanicsType = mechanicsType;
            exercise.Difficult = ExerciseDifficult.NotSet;
            session.Save(exercise);
            session.Flush();
            return exercise;
        }



        protected Customer CreateCustomer(string name,Profile profile,Profile connectedProfile=null,Gender gender=Gender.Male,string email=null,string phone=null,DateTime? birthday=null)
        {
            Customer customer = new Customer();
            customer.FirstName = customer.LastName = name;
            customer.Profile = profile;
            customer.Settings = new CustomerSettings();
            customer.ConnectedAccount = connectedProfile;
            customer.Gender = gender;
            customer.Birthday = birthday;
            customer.Email = email;
            customer.PhoneNumber = phone;
            insertToDatabase(customer);
            return customer;
        }
        public override void createTestFixture()
        {
            base.createTestFixture();
            ObjectsConverter.Configure();
            configureLogging();
            clientInformation = new ClientInformation() { Version = Const.ServiceVersion };
            clientInformation.ApplicationLanguage = "en";
            clientInformation.PlatformVersion = "fgfdgdfg";
            clientInformation.ApplicationVersion = "1.0.0.0";
            var conf = new ConfigurationSourceBuilder();
            conf.ConfigureCaching().ForCacheManagerNamed(SecurityManager.AuthenticationCacheName).StoreInMemory();

            var configSource = new DictionaryConfigurationSource();
            conf.UpdateConfigurationWithReplace(configSource);

            var xmlStream = DateTimeExtension.GetResource("BodyArchitect.UnitTests.V2.BAPointsTests.BAPoints.xml");
            var paymentsManager = new PaymentsManager();
            this.paymentsManager=paymentsManager.Load(xmlStream);
            InternalBodyArchitectService.PaymentsManager = this.paymentsManager;
            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
        }
        protected void RunServiceMethod(Action<InternalBodyArchitectService> method)
        {
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                timerService = new MockTimerService();
                ServiceConfiguration config = new ServiceConfiguration(timerService);
                config.MethodInvoker = new NormalMethodInvoker();
                config.Payments = paymentsManager;
                var service = new InternalBodyArchitectService(serviceSession, SecurityManager, new MockEmailService(), new MockPushNotificationService(), config);
                
                service.Configuration.ImagesFolder = ImagesFolder;
                NHibernateContextExtension wcfExtencsion = new NHibernateContextExtension(serviceSession);
                NHibernateContext.UnitTestContext = wcfExtencsion;
                method(service);
            }
            Session.Clear();
        }

        protected void Run(Action<ISession, ServiceConfiguration> method)
        {
            var serviceSession = sessionFactory.OpenSession();
            using (serviceSession)
            {
                timerService = new MockTimerService();
                ServiceConfiguration config = new ServiceConfiguration(timerService);
                //var service = new InternalBodyArchitectService(serviceSession, SecurityManager, new MockEmailService(), new MockPushNotificationService(), config);
                config.ImagesFolder = ImagesFolder;
                method(serviceSession, config);
            }
            Session.Clear();
        }

        protected MyPlace CreateMyPlace(string name, Profile profile,bool notForRecords=false)
        {
            var gym = new MyPlace();
            gym.Name = name;
            gym.Profile = profile;
            gym.Color = Color.Aqua.ToColorString();
            gym.CreationDate = DateTime.UtcNow;
            gym.NotForRecords = notForRecords;
            insertToDatabase(gym);
            return gym;
        }

        protected  Activity CreateActivity(string name, Profile profile,decimal price=0, TimeSpan? duration = null)
        {
            Activity activity = new Activity();
            activity.Name = name;
            activity.Profile = profile;
            activity.Color = Color.Aqua.ToColorString();
            activity.Price = price;
            if (duration == null)
            {
                duration = TimeSpan.FromMinutes(90);
            }
            activity.Duration = duration.Value;
            insertToDatabase(activity);
            return activity;
        }

        protected ReminderItem CreateReminder(string name, Profile profile, DateTime dateTime, TimeSpan? remindBefore = null, ReminderRepetitions pattern = ReminderRepetitions.Once,ReminderType type=ReminderType.Custom)
        {
            var reminder = new ReminderItem();
            reminder.Name = name;
            reminder.Profile = profile;
            reminder.DateTime = dateTime;
            reminder.Repetitions = pattern;
            reminder.Type = type;
            reminder.RemindBefore = remindBefore;
            insertToDatabase(reminder);
            return reminder;
        }


        protected CustomerGroup CreateCustomerGroup(string name, Profile profile, int maxCustomers = 10, params Customer[] members)
        {
            CustomerGroup group = new CustomerGroup();
            group.Name = name;
            group.Profile = profile;
            group.Color = Color.Aqua.ToColorString();
            group.MaxPersons = maxCustomers;
            foreach (var customer in members)
            {
                group.Customers.Add(customer);    
            }

            insertToDatabase(group);
            return group;
        }

        public static Profile CreateProfile(ISession session, string username,Country country=null,Gender gender=Gender.NotSet,AccountType accountType=AccountType.Instructor)
        {
            Profile profile1 = new Profile();
            if(country!=null)
            {
                profile1.CountryId = country.GeoId;
            }
            profile1.Licence.AccountType = accountType;
            profile1.Licence.LastPointOperationDate = DateTime.UtcNow;
            profile1.Privacy=new ProfilePrivacy();
            profile1.Settings=new ProfileSettings();
            profile1.Gender = gender;
            profile1.UserName = username;
            profile1.Email = username + "@wfg.pl";
            profile1.DataInfo = new DataInfo();
            profile1.Birthday = DateTime.Now.Date;
            profile1.Statistics=new ProfileStatistics();
            session.SaveOrUpdate(profile1.Statistics);
            session.SaveOrUpdate(profile1);
            ProfileDTO dto = profile1.Map<ProfileDTO>();
            profile1.Tag = dto;

            MyPlace defaultPlace = new MyPlace();
            defaultPlace.IsDefault = true;
            defaultPlace.IsSystem = true;
            defaultPlace.Profile = profile1;
            defaultPlace.Color = Color.Aqua.ToColorString();
            defaultPlace.Name = "Default";
            session.Save(defaultPlace);
            session.Flush();
            return profile1;
        }

        protected override void endTestFixture()
        {
            if (Directory.Exists(ImagesFolder))
            {
                Directory.Delete(ImagesFolder,true);
            }
            base.endTestFixture();
        }

        public MockTimerService TimerService
        {
            get { return timerService; }
        }

        public SecurityManager SecurityManager
        {
            get { return manager; }
        }

        public ClientInformation ClientInformation
        {
            get { return clientInformation; }
        }

        public virtual void BuildDatabase()
        {
            
        }


        protected virtual SessionData CreateNewSession(ProfileDTO profile, ClientInformation clientInfo, LoginData loginData = null, APIKey apiKey = null)
        {
            if (loginData == null)
            {
                if (apiKey == null)
                {
                    apiKey = new APIKey();
                    apiKey.ApiKey = Guid.NewGuid();
                    apiKey.ApplicationName = "UnitTest";
                    apiKey.EMail = "test@email.com";
                    Session.Save(apiKey);
                }
                loginData = new LoginData();
                loginData.ApiKey = apiKey;
                loginData.ApplicationLanguage = clientInfo.ApplicationLanguage;
                loginData.ApplicationVersion = clientInfo.ApplicationVersion;
                loginData.ClientInstanceId = clientInfo.ClientInstanceId;
                loginData.ProfileId = profile.GlobalId;
                loginData.LoginDateTime = DateTime.UtcNow;
                loginData.Platform = (PlatformType) clientInfo.Platform;
                loginData.PlatformVersion = clientInfo.PlatformVersion;
                Session.Save(loginData);
                Session.Flush();
            }
            var data= SecurityManager.CreateNewSession(profile, clientInfo, loginData);
            data.Token.Language = "en";
            var securityInfo=SecurityManager.EnsureAuthentication(data.Token);
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            securityInfo.Licence = dbProfile.Licence.Map<LicenceInfoDTO>();
            securityInfo.Licence.CurrentAccountType = securityInfo.Licence.AccountType;
            return data;
        }

        protected override void endTest()
        {
            base.endTest();
            SecurityManager.Cache.Flush();

        }

        public override void setupTest()
        {
            base.setupTest();
            NHibernateContextExtension wcfExtencsion = new NHibernateContextExtension(Session);
            NHibernateContext.UnitTestContext = wcfExtencsion;

            Profile profile = new Profile();
            profile.UserName = ProfileDTO.AdministratorName;
            profile.Email = "fdgfg@gmail.com";
            profile.Birthday = DateTime.Now.AddYears(-30);
            profile.Privacy = new ProfilePrivacy();
            profile.Privacy.Searchable = false;
            profile.CountryId = Country.GetByTwoLetters("PL").GeoId;
            profile.Licence.AccountType = AccountType.Administrator;
            insertToDatabase(profile);

            BuildDatabase();
            Session.Clear();
            manager = new SecurityManager();
           
            ImagesFolder = Path.Combine(Environment.CurrentDirectory, "Images");
            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }
            
            
        }

        protected Payment CreatePayment(Product product,Profile profile)
        {
            var zakup = new Payment();
            zakup.DateTime = DateTime.UtcNow;
            product.Payment = zakup;
            zakup.Price = product.Price;
            product.Payment.Product = product;
            var basket = new PaymentBasket();
            basket.Customer = product.Customer;
            basket.DateTime = DateTime.Now;
            basket.TotalPrice = product.Price;
            basket.Profile = profile;
            basket.Payments.Add((Payment)product.Payment);
            ((Payment)product.Payment).PaymentBasket = basket;
            insertToDatabase(basket);
            insertToDatabase(product);
            Session.Flush();
            return (Payment)product.Payment;
        }

        public Suplement CreateSupplement(string name,bool isProduct=false,bool canBeIllegal=false,Profile profile=null)
        {
            Suplement supplement = new Suplement();
            supplement.Name = name;
            supplement.CanBeIllegal = canBeIllegal;
            supplement.GlobalId = Guid.NewGuid();
            supplement.IsProduct = isProduct;
            if (profile != null)
            {
                supplement.Profile = profile;
            }
            insertToDatabase(supplement);
            return supplement;
        }

        protected SupplementCycleDosageDTO CreateDosageDTO(decimal dosageValue, SuplementDTO supplement, SupplementCycleDayRepetitions repetitions = SupplementCycleDayRepetitions.OnceAWeek, Service.V2.Model.DosageType dosageType = Service.V2.Model.DosageType.MiliGrams, Service.V2.Model.TimeType timeType = Service.V2.Model.TimeType.NotSet)
        {
            var dosage = new SupplementCycleDosageDTO();
            dosage.Dosage = dosageValue;
            dosage.DosageType = dosageType;
            dosage.Repetitions = repetitions;
            dosage.Supplement = supplement;
            dosage.TimeType = timeType;
            return dosage;
        }

        protected SupplementCycleDefinition CreateSupplementsCycleDefinition(string name,Suplement supplement ,Profile profile,PublishStatus status=PublishStatus.Published )
        {
            var cycleDefinition = new SupplementCycleDefinition();
            cycleDefinition.Name = name;
            cycleDefinition.Language = "en";
            cycleDefinition.Status = status;
            var week = new SupplementCycleWeek();
            week.Name = "Training week";
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;
            SupplementCycleDosage dosageDto = new SupplementCycleDosage();
            dosageDto.Supplement = supplement;
            week.Dosages.Add(dosageDto);
            dosageDto.Week = week;
            cycleDefinition.Weeks.Add(week);
            week.Definition = cycleDefinition;

            cycleDefinition.Profile = profile;
            insertToDatabase(cycleDefinition);
            return cycleDefinition;
        }

        protected Message CreateMessage(Profile sender, Profile receiver,DateTime? dateTime=null)
        {
            Message msg = new Message();
            msg.Receiver = receiver;
            msg.Sender = sender;
            if(!dateTime.HasValue)
            {
                dateTime = DateTime.UtcNow;
            }
            msg.CreatedDate = dateTime.Value;
            msg.Topic = "topic";
            msg.Content = "content";
            insertToDatabase(msg);
            return msg;
        }

        protected MyPlace GetDefaultMyPlace(Profile profile)
        {
            return Session.QueryOver<MyPlace>().Where(x => x.Profile == profile && x.IsDefault).SingleOrDefault();
        }

        protected ExerciseProfileData CreateExerciseRecord(Exercise exercise, Profile profile, Tuple<int, decimal> serie, DateTime trainingDate, Customer customer = null)
        {
            var trainingDay = new TrainingDay(trainingDate);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profile);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);

            Serie set1 = new Serie();
            set1.RepetitionNumber = serie.Item1;
            set1.Weight = serie.Item2;
            item.AddSerie(set1);
            insertToDatabase(trainingDay);

            ExerciseProfileData data = new ExerciseProfileData();
            data.Profile = profile;
            data.Customer = customer;
            data.Serie = set1;
            data.TrainingDate = trainingDate;
            data.Repetitions = serie.Item1;
            data.MaxWeight = serie.Item2;
            data.Exercise = exercise;
            insertToDatabase(data);
            return data;
        }

        protected Championship CreateChampionship(Profile profile, string name, ChampionshipType type = ChampionshipType.Trojboj, ScheduleEntryState state = ScheduleEntryState.Done)
        {
            Championship championship = new Championship();
            championship.Name = name;
            championship.MyPlace = GetDefaultMyPlace(profile);
            championship.StartTime = DateTime.Now;
            championship.Profile = profile;
            championship.ChampionshipType = type;
            championship.State = state;
            insertToDatabase(championship);

            return championship;
        }

        protected Championship CreateChampionshipEx(Profile profile, string name, ChampionshipType type = ChampionshipType.Trojboj, ScheduleEntryState state = ScheduleEntryState.Done,DateTime? startDate=null, params ChampionshipCategory[] categories)
        {
            Championship championship = new Championship();
            championship.Name = name;
            championship.MyPlace = GetDefaultMyPlace(profile);
            championship.Profile = profile;
            championship.State = state;
            if(!startDate.HasValue)
            {
                startDate = DateTime.Now;
            }
            championship.StartTime = startDate.Value;
            foreach (var championshipCategory in categories)
            {
                championship.Categories.Add(championshipCategory);    
            }
            
            championship.ChampionshipType = type;
            insertToDatabase(championship);

            return championship;
        }

        protected ScheduleEntryReservation CreateReservation(ScheduleEntryBase entry, Customer customer)
        {
            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Name = "test";
            reservation.ScheduleEntry = entry;
            reservation.Profile = customer.Profile;
            entry.Reservations.Add(reservation);
            reservation.Customer = customer;
            insertToDatabase(reservation);
            return reservation;
        }
    }
}
