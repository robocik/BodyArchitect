using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.WP7;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Service.V2.Services;
using BodyArchitect.Shared;
using NHibernate;
using Profile = BodyArchitect.Model.Profile;
using SessionData = BodyArchitect.Service.V2.Model.SessionData;
using Token = BodyArchitect.Service.V2.Model.Token;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using TrainingPlan = BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan;

namespace BodyArchitect.Service.V2
{
    public class 
        InternalBodyArchitectService
    {
        public IEMailService EMailService { get; private set; }
        private ServiceConfiguration configuration;
        

        public IPushNotificationService PushNotificationService { get; private set; }

        static InternalBodyArchitectService()
        {
            ObjectsConverter.Configure();
        }


        public static PaymentsHolder PaymentsManager { get; set; }

        public ISession Session { get; private set; }

        public InternalBodyArchitectService(ISession session)
        {
            Session = session;
            EMailService = new EMailService();
            PushNotificationService = new NotReadedMessagesPushNotification();
            configuration = new ServiceConfiguration();
            configuration.Payments = InternalBodyArchitectService.PaymentsManager;
            configuration.MethodInvoker = new ThreadPoolMethodInvoker();
        }


        public InternalBodyArchitectService(ISession session,SecurityManager security, IEMailService eMailServicea,IPushNotificationService pushService,ServiceConfiguration configuration)
        {
            Session = session;
            securityManager = security;
            EMailService = eMailServicea;
            PushNotificationService = pushService;
            this.configuration = configuration;
        }

        private static SecurityManager securityManager = new SecurityManager();

        public static SecurityManager SecurityManager
        {
            get { return securityManager; }
        }

        //public IList<EntryObjectDTO> GetMyTrainingEntries(Token token, MyTrainingDTO myTraining)
        //{
        //    var securityinfo=SecurityManager.EnsureAuthentication(token);
        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        MyTraining myTrainingDb = session.Get<MyTraining>(myTraining.GlobalId);
        //        Profile profileDb = session.Load<Profile>(securityinfo.SessionData.Profile.Id);
        //        if (myTrainingDb.Customer!=null && myTrainingDb.Customer.Profile != profileDb)
        //        {
        //            throw new CrossProfileOperationException("This customer doesn't belong to your profile");
        //        }
        //        if (myTrainingDb == null || (myTrainingDb.Profile != profileDb && (myTrainingDb.Profile.Privacy.CalendarView == Privacy.Private || myTrainingDb.Profile.Privacy.CalendarView == Privacy.FriendsOnly && !myTrainingDb.Profile.Friends.Contains(profileDb))))
        //        {
        //            tx.Commit();
        //            return new List<EntryObjectDTO>();
        //        }

        //        return Mapper.Map<ICollection<EntryObject>, EntryObjectDTO[]>(myTrainingDb.EntryObjects);
        //    }
        //}

        

        //public MapperResult MapExercises(Token token, MapperData data)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    Log.WriteWarning("MapExercises:Username={0}", securityInfo.SessionData.Profile.UserName);
        //    var session = Session;

        //    ExercisesMapper mapper = new ExercisesMapper(session,Configuration.TimerService);
        //    return mapper.Run(securityInfo.SessionData.Profile.Id, data);
        //}

        
        public void AccountOperation(string userNameOrEmail,AccountOperationType operationType)
        {
            ProfileService profileService = new ProfileService(Session, null, Configuration, SecurityManager, PushNotificationService, EMailService);
            profileService.AccountOperation(userNameOrEmail,operationType);
        }

        public void ProfileOperation(Token token, ProfileOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ProfileService profileService = new ProfileService(Session, securityInfo, Configuration, SecurityManager, PushNotificationService, EMailService);
            profileService.ProfileOperation(token, param);
        }

        public ProfileInformationDTO GetProfileInformation(Token token, GetProfileInformationCriteria criteria)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ProfileService profileService = new ProfileService(Session, securityInfo, Configuration, SecurityManager, PushNotificationService, EMailService);
            return profileService.GetProfileInformation(token, criteria);
        }

        public SessionData Login(ClientInformation clientInfo, string username, string password)
        {
            ProfileService profileService = new ProfileService(Session, null, Configuration, SecurityManager, PushNotificationService, EMailService);
            return profileService.Login(clientInfo, username, password);
        }

        

        public void Logout(Token token)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            Log.WriteInfo("Logout. Username: {0}", securityInfo.SessionData.Profile.UserName);
            SecurityManager.Remove(token);
        }

        

 
        public SessionData CreateProfile(ClientInformation clientInfo, ProfileDTO newProfile)
        {
            ProfileService profileService = new ProfileService(Session, null, Configuration, SecurityManager, PushNotificationService, EMailService);
            return profileService.CreateProfile(clientInfo, newProfile);
        }


        public ServiceConfiguration Configuration
        {
            get { return configuration; }
        }


        public void MessageOperation(Token token, MessageOperationParam arg)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            MessageService service = new MessageService(Session,securityInfo,Configuration,PushNotificationService);
            service.MessageOperation(arg);
            
        }

        public void SendMessage(Token token, MessageDTO message)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            MessageService service = new MessageService(Session, securityInfo, Configuration, PushNotificationService);
            service.SendMessage(message);
        }

        public PagedResult<MessageDTO> GetMessages(Token token, GetMessagesCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            MessageService service = new MessageService(Session, securityInfo, Configuration, PushNotificationService);
            return service.GetMessages(searchCriteria, retrievingInfo);
        }
        

        public ProfileDTO UpdateProfile(Token token, ProfileUpdateData data)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ProfileService service = new ProfileService(Session, securityInfo, Configuration, SecurityManager, PushNotificationService, EMailService);
            return service.UpdateProfile(data);

        }

        public PictureDTO GetImage(PictureInfoDTO pictureInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(pictureInfo);
            PictureService pictureService = new PictureService(Session, securityInfo, Configuration);
            return pictureService.GetImage(pictureInfo);
        }


        public FriendInvitationDTO InviteFriendOperation(Token token, InviteFriendOperationData data)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);

            var service = new InvitationService(Session,securityInfo,Configuration,PushNotificationService,EMailService);
            return service.InviteFriendOperation(data);
        }

        

        public PagedResult<ExerciseDTO> GetExercises(Token token, ExerciseSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ExerciseService service = new ExerciseService(Session, securityInfo, Configuration);
            return service.GetExercises(searchCriteria,retrievingInfo);
        }

        public PagedResult<SuplementDTO> GetSuplements(Token token,GetSupplementsParam param, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementService(Session, securityInfo, Configuration);
            return service.GetSuplements(param,retrievingInfo);
        }

        public ExerciseDTO SaveExercise(Token token, ExerciseDTO exercise)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ExerciseService service = new ExerciseService(Session, securityInfo, Configuration);
            return service.SaveExercise(exercise);
        }

        public SuplementDTO SaveSuplement(Token token, SuplementDTO suplement)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementService(Session, securityInfo, Configuration);
            return service.SaveSuplement(suplement);
        }
        
        public PagedResult<MyTrainingDTO> GetMyTrainings(Token token, GetMyTrainingsParam param, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new MyTrainingService(Session, securityInfo, Configuration);
            return service.GetMyTrainings(param, pageInfo);
        }

        public PictureInfoDTO UploadImage(PictureDTO pictureDto)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(pictureDto);
            PictureService pictureService = new PictureService(Session, securityInfo, Configuration);
            return pictureService.UploadImage(pictureDto);
        }


        public bool CheckProfileNameAvailability(string userName)
        {
            var service = new ProfileService(Session, null,Configuration, null,PushNotificationService, EMailService);
            return service.CheckProfileNameAvailability(userName);
        }

        public PagedResult<TrainingDayCommentDTO> GetTrainingDayComments(Token token, TrainingDayInfoDTO day, PartialRetrievingInfo info)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new VoteService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            return service.GetTrainingDayComments(day,info);
        }

        public TrainingDayCommentDTO TrainingDayCommentOperation(Token token, TrainingDayCommentOperationParam arg)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new VoteService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            return service.TrainingDayCommentOperation(arg);
        }


        //public static Boolean IsDirtyEntity(ISession session, Object entity,Object oldEntity)
        //{

        //    String className = NHibernateProxyHelper.GuessClass(entity).FullName;

        //    ISessionImplementor sessionImpl = session.GetSessionImplementation();

        //    IPersistenceContext persistenceContext = sessionImpl.PersistenceContext;

        //    IEntityPersister persister = sessionImpl.Factory.GetEntityPersister(className);

        //    EntityEntry oldEntry = sessionImpl.PersistenceContext.GetEntry(oldEntity);
            

        //    if ((oldEntry == null) && (entity is INHibernateProxy))
        //    {

        //        INHibernateProxy proxy = entity as INHibernateProxy;

        //        Object obj = sessionImpl.PersistenceContext.Unproxy(proxy);

        //        oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);

        //    }



        //    Object[] oldState = oldEntry.LoadedState;

        //    Object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

        //    Int32[] dirtyProps = persister.FindDirty(currentState, oldState, entity, sessionImpl);


        //    return (dirtyProps != null);

        //}

        public SaveTrainingDayResult SaveTrainingDay(Token token, TrainingDayDTO day)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
           TrainingDayService service = new TrainingDayService(Session,securityInfo,Configuration,PushNotificationService,EMailService);
            return service.SaveTrainingDay(day);
        }

        public PagedResult<TrainingDayDTO> GetTrainingDays(Token token, WorkoutDaysSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            TrainingDayService service = new TrainingDayService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            return service.GetTrainingDays(searchCriteria, retrievingInfo);
        }


        public TrainingDayDTO GetTrainingDay(Token token, WorkoutDayGetOperation operationParams, RetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            TrainingDayService service = new TrainingDayService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            return service.GetTrainingDay(operationParams, retrievingInfo);
        }

        //public TrainingPlan GetWorkoutPlan(Token token, Guid planId, RetrievingInfo retrievingInfo)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    WorkoutPlanService service = new WorkoutPlanService(Session,securityInfo,Configuration);
        //    return service.GetWorkoutPlan(planId, retrievingInfo);
        //}

        public void DeleteTrainingDay(Token token, DeleteTrainingDayParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            TrainingDayService service = new TrainingDayService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            service.DeleteTrainingDay(param);
        }

        //public void DeleteExercise(Token token, ExerciseDTO dto)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    Log.WriteWarning("DeleteExercise:Username={0},exerciseId={1}", securityInfo.SessionData.Profile.UserName, dto.GlobalId);


        //    //var dbPlan = Mapper.Map<WorkoutPlanDTO, BodyArchitect.Model.TrainingPlan>(dto);
        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        ExerciseOperation.DeleteExercise(session, dto.GlobalId, securityInfo.SessionData.Profile.Id,Configuration.TimerService);
        //        tx.Commit();
        //    }
        //}

        //public void DeleteWorkoutPlan(Token token, TrainingPlanInfo dto)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    WorkoutPlanService service = new WorkoutPlanService(Session, securityInfo, Configuration);
        //    service.DeleteWorkoutPlan(dto);
        //}

        public TrainingPlan SaveWorkoutPlan(Token token, TrainingPlan dto)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            WorkoutPlanService service = new WorkoutPlanService(Session, securityInfo, Configuration);
            return service.SaveWorkoutPlan(dto);
        }

        //public WorkoutPlanDTO VoteWorkoutPlan(Token token, WorkoutPlanDTO planDto)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService);
        //    return service.VoteWorkoutPlan(planDto);
        //}

        //public ExerciseDTO VoteExercise(Token token, ExerciseDTO exercise)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService);
        //    return service.VoteExercise(exercise);
        //}

        //public SuplementDTO VoteSupplement(Token token, SuplementDTO supplement)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService);
        //    return service.VoteSupplement(supplement);
        //}

        public VoteResult Vote(Token token, VoteParams param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService,EMailService);
            return service.Vote(param);
        }

        //public SupplementCycleDefinitionDTO VoteSupplementCycleDefinition(Token token, SupplementCycleDefinitionDTO definition)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService);
        //    return service.VoteSupplementCycleDefinition(definition);
        //}

        public PagedResult<UserSearchDTO> GetUsers(Token token, UserSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            ProfileService profileService = new ProfileService(Session, securityInfo, Configuration, SecurityManager, PushNotificationService, EMailService);
            return profileService.GetUsers(searchCriteria, pagerInfo);
        }

        public PagedResult<TrainingPlan> GetWorkoutPlans(Token token, WorkoutPlanSearchCriteria searchCriteria, PartialRetrievingInfo pagerInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            WorkoutPlanService service = new WorkoutPlanService(Session, securityInfo, Configuration);
            return service.GetWorkoutPlans(searchCriteria, pagerInfo);
        }
        
        public PagedResult<CommentEntryDTO> GetComments(Token token, Guid globalId, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo=SecurityManager.EnsureAuthentication(token);
            VoteService service = new VoteService(Session, securityInfo, Configuration, PushNotificationService, EMailService);
            return service.GetComments(globalId, retrievingInfo);
        }

        public void UserFavoritesOperation(Token token, UserDTO userDto, FavoriteOperation operation)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ProfileService(Session, securityInfo, Configuration,SecurityManager, PushNotificationService, EMailService);
            service.UserFavoritesOperation(userDto, operation);
        }

        public void ExerciseOperation(Token token,ExerciseOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ExerciseService(Session, securityInfo, Configuration);
            service.ExerciseOperation(param);
        }

        public TrainingPlan WorkoutPlanOperation(Token token, WorkoutPlanOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new WorkoutPlanService(Session, securityInfo, Configuration);
            return service.WorkoutPlanOperation(param);
        }

        //public ExerciseDTO PublishExercise(Token token, ExerciseDTO exerciseDto)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    Log.WriteWarning("PublishExercise: Username={0},userDto.GlobalId={1}", securityInfo.SessionData.Profile.UserName, exerciseDto.GlobalId);

        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        var dbOriginalExercise = session.Get<Exercise>(exerciseDto.GlobalId);
        //        if (dbOriginalExercise.Profile.Id != securityInfo.SessionData.Profile.Id || dbOriginalExercise.Profile.Id != exerciseDto.Profile.Id)
        //        {
        //            throw new CrossProfileOperationException("Cannot  change an exercise for another profile");
        //        }


        //        //we cannot modify published exercise
        //        if (dbOriginalExercise.Status != PublishStatus.Private)
        //        {
        //            throw new PublishedObjectOperationException("Cannot change published or pending exercise");
        //        }
        //        if (!isValidUrl(dbOriginalExercise.Url))
        //        {
        //            throw new ArgumentException("Published exercise must have Url describing this exercise");
        //        }
        //        if (dbOriginalExercise.ExerciseType == ExerciseType.NotSet)
        //        {
        //            throw new ArgumentException("Published exercise must have exercise type set");
        //        }
        //        dbOriginalExercise.PublishDate = Configuration.TimerService.UtcNow;
        //        dbOriginalExercise.Status = PublishStatus.PendingPublish;

        //        session.Update(dbOriginalExercise);
        //        tx.Commit();
        //        return Mapper.Map<Exercise, ExerciseDTO>(dbOriginalExercise);
        //    }

        //}

        //public void ImportData(ImportDataStream data)
        //{
        //    Log.WriteInfo("ImportData");
        //    MemoryStream stream = new MemoryStream();
        //    data.ImageStream.CopyTo(stream);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    Log.WriteVerbose("Preparing deserializing process");
        //    var ttt = new XmlDictionaryReaderQuotas();
        //    ttt.MaxStringContentLength = 10000000;
        //    XmlDictionaryReader reader =
        //        XmlDictionaryReader.CreateTextReader(stream, ttt);

        //    DataContractSerializer ser = new DataContractSerializer(typeof(ImportDataHolder));

        //    // Deserialize the data and read it from the instance.
        //    ImportDataHolder deserializedPerson =
        //        (ImportDataHolder)ser.ReadObject(reader, false);
        //    stream.Dispose();
        //    Log.WriteVerbose("Data has been deserialized");
        //    DataSet ds = null;
        //    if(deserializedPerson.Schema!=null || deserializedPerson.XmlData!=null)
        //    {
        //        Log.WriteVerbose("Creating dataset");
        //        ds = new DataSet();
        //        ds.ReadXmlSchema(new MemoryStream(System.Text.Encoding.Unicode.GetBytes(deserializedPerson.Schema)));
        //        ds.ReadXml(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(deserializedPerson.XmlData)));
        //        Log.WriteVerbose("Dataset has been created");
        //    }
        //    Log.WriteVerbose("Validating profile");
        //    var validator = new ObjectValidator(typeof(ProfileDTO));
        //    var result = validator.Validate(deserializedPerson.Profile);
        //    if (!result.IsValid)
        //    {
        //        throw new ValidationException(result);
        //    }

        //    var session = Session;
        //    using (var transactionScope = new TransactionManager(true))
        //    {
        //        SessionData sessionData = CreateProfile(deserializedPerson.ClientInformation, deserializedPerson.Profile);
        //        if (sessionData == null)
        //        {
        //            throw new InvalidOperationException("Cannot create a profile");
        //        }
        //        if (ds != null)
        //        {//user wants to import the data
        //            Log.WriteVerbose("Retrieving exercises...");
        //            ExerciseSearchCriteria criteria = ExerciseSearchCriteria.CreatePersonalCriteria();
        //            PartialRetrievingInfo info = new PartialRetrievingInfo();
        //            info.PageSize = PartialRetrievingInfo.AllElementsPageSize;
        //            var exercises = GetExercises(sessionData.Token, criteria, info);
        //            Log.WriteVerbose("Start importing the data");
        //            DataImporter importer = new DataImporter(session, sessionData.Profile, exercises.Items);
        //            importer.Import(ds, deserializedPerson.WymiaryId);
        //        }
        //        transactionScope.CommitTransaction();
        //    }

        //}

        

        

        #region WP7

        public string WP7Register(string deviceid, string uri,Guid profileId)
        {
            WP7Service service= new WP7Service(Session,null,Configuration,PushNotificationService);
            return service.WP7Register(deviceid, uri, profileId);
        }

        public void WP7Unregister(string deviceid)
        {
            WP7Service service = new WP7Service(Session, null, Configuration, PushNotificationService);
            service.WP7Unregister(deviceid);
        }

        public void WP7ClearCounter(Profile profile)
        {
            WP7Service service = new WP7Service(Session, null, Configuration, PushNotificationService);
            service.WP7ClearCounter(profile);
        }

        public void WP7ClearCounter(string deviceid)
        {
            WP7Service service = new WP7Service(Session, null, Configuration, PushNotificationService);
            service.WP7ClearCounter(deviceid);
        }

        public TrialStatusInfo WP7TrialStatus(string deviceId)
        {
            WP7Service service = new WP7Service(Session, null, Configuration, PushNotificationService);
            return service.WP7TrialStatus(deviceId);
        }

        #endregion

        public PagedResult<ActivityDTO> GetActivities(Token token, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ActivityService(Session,securityInfo,Configuration);
            return service.GetActivities(retrievingInfo);
        }

        public void DeleteActivity(Token token,ActivityDTO activity)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ActivityService(Session, securityInfo, Configuration);
            service.DeleteActivity(activity);
        }

        public ActivityDTO SaveActivity(Token token, ActivityDTO activity)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ActivityService(Session, securityInfo, Configuration);
            return service.SaveActivity(activity);
        }

        public ScheduleEntryDTO SaveScheduleEntry(Token token, ScheduleEntryDTO entry)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ScheduleEntryService(Session, securityInfo, Configuration);
            return service.SaveScheduleEntry(entry);
        }

        public void DeleteScheduleEntry(Token token, ScheduleEntryDTO entry)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ScheduleEntryService(Session, securityInfo, Configuration);
            service.DeleteScheduleEntry(entry);
        }

        public IList<ScheduleEntryBaseDTO> SaveScheduleEntriesRange(Token token, SaveScheduleEntryRangeParam saveScheduleEntryRangeParam)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ScheduleEntryService(Session, securityInfo, Configuration);
            return service.SaveScheduleEntryRange(saveScheduleEntryRangeParam);
        }

        public PagedResult<ScheduleEntryBaseDTO> GetScheduleEntries(Token token, GetScheduleEntriesParam getScheduleEntriesParam, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ScheduleEntryService(Session, securityInfo, Configuration);
            return service.GetScheduleEntries(getScheduleEntriesParam, retrievingInfo);
        }

        public PagedResult<CustomerDTO> GetCustomers(Token token, CustomerSearchCriteria customerSearchCriteria, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerService(Session, securityInfo, Configuration);
            return service.GetCustomers(customerSearchCriteria, pageInfo);
        }

        public CustomerDTO SaveCustomer(Token token, CustomerDTO customerDto)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerService(Session, securityInfo, Configuration);
            return service.SaveCustomer(customerDto);
        }

        public void DeleteCustomer(Token token, CustomerDTO customerDto)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerService(Session, securityInfo, Configuration);
            service.DeleteCustomer(customerDto);
        }

        public ReservationOperationResult ReservationsOperation(Token token, ReservationsOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReservationService(Session, securityInfo, Configuration);
            return service.ReservationsOperation(param);
        }

        public PaymentBasketDTO PaymentBasketOperation(Token token, PaymentBasketDTO koszyk)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new PaymentService(Session, securityInfo, Configuration);
            return service.PaymentBasketOperation(koszyk);
        }

        public PagedResult<PaymentBasketDTO> GetPaymentBaskets(Token token, GetPaymentBasketParam getPaymentBasketParam, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new PaymentService(Session, securityInfo, Configuration);
            return service.GetPaymentBaskets(getPaymentBasketParam, retrievingInfo);
        }

        public CustomerGroupDTO SaveCustomerGroup(Token token, CustomerGroupDTO customerGroup)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerGroupService(Session, securityInfo, Configuration);
            return service.SaveCustomerGroup(customerGroup);
        }

        public PagedResult<CustomerGroupDTO> GetCustomerGroups(Token token, CustomerGroupSearchCriteria criteria, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerGroupService(Session, securityInfo, Configuration);
            return service.GetCustomerGroups(criteria, pageInfo);
        }

        public void DeleteCustomerGroup(Token token, CustomerGroupDTO group)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new CustomerGroupService(Session, securityInfo, Configuration);
            service.DeleteCustomerGroup(group);
        }

        public PagedResult<ProductInfoDTO> GetProducts(Token token, GetProductsParam getProductsParam, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ProductService(Session, securityInfo, Configuration);
            return service.GetProducts(getProductsParam, retrievingInfo);
        }

        public ReminderItemDTO SaveReminder(Token token,ReminderItemDTO reminder)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReminderService(Session, securityInfo, Configuration);
            return service.SaveReminder(reminder);
        }

        public PagedResult<ReminderItemDTO> GetReminders(Token token, GetRemindersParam remindersParam, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReminderService(Session, securityInfo, Configuration);
            return service.GetReminders(remindersParam, pageInfo);
        }

        public ReminderItemDTO ReminderOperation(Token token, ReminderOperationParam remindersParam)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReminderService(Session, securityInfo, Configuration);
            return service.ReminderOperation(remindersParam);
        }

        public MyTrainingDTO MyTrainingOperation(Token token, MyTrainingOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementsEngineService(Session, securityInfo, Configuration);
            return service.MyTrainingOperation(param);
        }


        public PagedResult<SupplementCycleDefinitionDTO> GetSupplementsCycleDefinitions(Token token, GetSupplementsCycleDefinitionsParam param, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementsEngineService(Session, securityInfo, Configuration);
            return service.GetSupplementsCycleDefinitions(param, pageInfo);
        }

        public SupplementCycleDefinitionDTO SaveSupplementsCycleDefinition(Token token, SupplementCycleDefinitionDTO definition)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementsEngineService(Session, securityInfo, Configuration);
            return service.SaveSupplementsCycleDefinition(definition);
        }

        public SupplementCycleDefinitionDTO SupplementsCycleDefinitionOperation(Token token, SupplementsCycleDefinitionOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new SupplementsEngineService(Session, securityInfo, Configuration);
            return service.SupplementsCycleDefinitionOperation(param);
        }

        #region Reports

        public IList<WeightExerciseReportResultItem> ReportExerciseWeight(Token token, ReportExerciseWeightParams param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReportService(Session, securityInfo, Configuration);
            return service.ReportExerciseWeight(param);
        }

        public IList<WeightReperitionReportResultItem> ReportWeightRepetitions(Token token, ReportWeightRepetitionsParams param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReportService(Session, securityInfo, Configuration);
            return service.ReportWeightRepetitions(param);
        }

        public IList<MeasurementsTimeReportResultItem> ReportMeasurementsTime(Token token, ReportMeasurementsTimeParams param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReportService(Session, securityInfo, Configuration);
            return service.ReportMeasurementsTime(param);
        }

        public PagedResult<ExerciseRecordsReportResultItem> ReportExerciseRecords(Token token, ExerciseRecordsParams param,PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ReportService(Session, securityInfo, Configuration);
            return service.ReportExerciseRecords(param,pageInfo);
        }

        #endregion

        public void ImportLicence(Token token, string goodLicenceKey)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new LicenceService(Session, securityInfo, Configuration);
            service.ImportLicence(goodLicenceKey);
        }

        public MyPlaceDTO SaveMyPlace(Token token, MyPlaceDTO place)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new MyPlaceService(Session, securityInfo, Configuration);
            return service.SaveMyPlace(place);
        }

        public MyPlaceDTO MyPlaceOperation(Token token, MyPlaceOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new MyPlaceService(Session, securityInfo, Configuration);
            return service.MyPlaceOperation(param);
        }

        public PagedResult<MyPlaceDTO> GetMyPlaces(Token token, MyPlaceSearchCriteria searchCriteria, PartialRetrievingInfo pageInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new MyPlaceService(Session, securityInfo, Configuration);
            return service.GetMyPlaces(searchCriteria, pageInfo);
        }

        public FeaturedData GetFeaturedData(Token token,GetFeaturedDataParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new FeaturedItemsService(Session, securityInfo, Configuration);
            return service.GetFeaturedData(param);
        }

        public SaveChampionshipResult SaveChampionship(Token token, ChampionshipDTO championship)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ChampionshipService(Session, securityInfo, Configuration);
            return service.SaveChampionship(championship);
        }

        
        public PagedResult<ChampionshipDTO> GetChampionships(Token token, GetChampionshipsCriteria criteria, PartialRetrievingInfo retrievingInfo)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(token);
            var service = new ChampionshipService(Session, securityInfo, Configuration);
            return service.GetChampionships(criteria, retrievingInfo);
        }

        //public PagedResult<GPSPoint> GetGPSCoordinates(Token token, Guid entryId, PartialRetrievingInfo pageInfo)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    var service = new GPSTrackerService(Session, securityInfo, Configuration);
        //    return service.GetGPSCoordinates(entryId, pageInfo);
        //}

        public GPSCoordinatesDTO GetGPSCoordinates(GetGPSCoordinatesParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(param);
            var service = new GPSTrackerService(Session, securityInfo, Configuration);
            return service.GetGPSCoordinates(param);
        }

        //public GPSTrackerEntryDTO GPSCoordinatesOperation(Token token, GPSCoordinatesOperationParam param)
        //{
        //    var securityInfo = SecurityManager.EnsureAuthentication(token);
        //    var service = new GPSTrackerService(Session, securityInfo, Configuration);
        //    return service.GPSCoordinatesOperation(param);
        //}

        public GPSCoordinatesOperationResult GPSCoordinatesOperation(GPSOperationParam param)
        {
            var securityInfo = SecurityManager.EnsureAuthentication(param);
            var service = new GPSTrackerService(Session, securityInfo, Configuration);
            return service.GPSCoordinatesOperation(param);
        }
    }
}
