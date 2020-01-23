using BodyArchitect.Service.Client.WP7.Cache;

namespace BodyArchitect.Service.Client.WP7
{
    public class ObjectsReposidory
    {
        private ExercisesReposidory exercises;
        private SupplementsReposidory supplements;
        private TrainingPlansReposidory trainingPlans;
        private MessagesReposidory messages;
        private MyPlacesReposidory myPlaces;
        private RemindersReposidory reminders;
        private CustomersReposidory customers;
        private FeaturedDataReposidory featured;
        //private IDictionary<Guid, ExerciseDTO> dictExercises;
        //private IDictionary<Guid, TrainingPlanInfo> dictPlans;
        //private IDictionary<Guid, SuplementDTO> dictSupplements;
        //public event EventHandler LoadedExercises;
        //public event EventHandler LoadedSupplements;
        //public event EventHandler LoadedWorkoutPlans;

        //public DateTime? ExerciseDate { get; set; }
        //public DateTime? SupplementDate { get; set; }
        //public DateTime? PlanDate { get; set; }

        public void Clear()
        {
            //ExerciseDate = null;
            //SupplementDate = null;
            //PlanDate = null;
            //dictExercises = null;
            //dictSupplements = null;
            //dictPlans = null;
            Exercises = null;
            Supplements = null;
            TrainingPlans = null;
            Messages = null;
            MyPlaces = null;
            Reminders = null;
            Customers = null;
            Featured = null;
        }

        public void ClearAfterLogin()
        {
            if (!ApplicationState.Current.IsOffline)
            {
                Featured = null;
                Messages = null;
            }
        }

        public FeaturedDataReposidory Featured
        {
            get
            {
                if (featured == null)
                {
                    featured = new FeaturedDataReposidory();
                }
                return featured;
            }
            set { featured = value; }
        }

        public ObjectsReposidory Copy()
        {
            //ObjectsReposidory cache = new ObjectsReposidory();
            //if (ApplicationState.Current.Cache.Exercises.IsLoaded)
            //{
            //    cache.Exercises = ApplicationState.Current.Cache.Exercises;
            //}
            //if (ApplicationState.Current.Cache.Supplements.IsLoaded)
            //{
            //    cache.Supplements = ApplicationState.Current.Cache.Supplements;
            //}
            //if (ApplicationState.Current.Cache.TrainingPlans.IsLoaded)
            //{
            //    cache.TrainingPlans = ApplicationState.Current.Cache.TrainingPlans;
            //}
            //if (ApplicationState.Current.Cache.Messages.IsLoaded)
            //{
            //    cache.Messages = ApplicationState.Current.Cache.Messages;
            //}
            //return cache;
            //TODO:CHECK THIS
            return this;
        }

        public CustomersReposidory Customers
        {
            get
            {
                if (customers == null)
                {
                    customers = new CustomersReposidory();
                }
                return customers;
            }
            set { customers = value; }
        }

        public RemindersReposidory Reminders
        {
            get
            {
                if (reminders == null)
                {
                    reminders = new RemindersReposidory();
                }
                return reminders;
            }
            set { reminders = value; }
        }

        public MyPlacesReposidory MyPlaces
        {
            get
            {
                if (myPlaces == null)
                {
                    myPlaces = new MyPlacesReposidory();
                }
                return myPlaces;
            }
            set { myPlaces = value; }
        }

        public MessagesReposidory Messages
        {
            get
            {
                if (messages == null)
                {
                    messages = new MessagesReposidory();
                }
                return messages;
            }
            set { messages = value; }
        }

        public SupplementsReposidory Supplements
        {
            get
            {
                if (supplements == null)
                {
                    supplements = new SupplementsReposidory();
                }
                return supplements;
            }
            set { supplements = value; }
        }

        public ExercisesReposidory Exercises
        {
            get
            {
                if(exercises==null)
                {
                    exercises=new ExercisesReposidory();
                }
                return exercises;
            }
            set { exercises=value; }
        }

        public TrainingPlansReposidory TrainingPlans
        {
            get
            {
                if (trainingPlans == null)
                {
                    trainingPlans = new TrainingPlansReposidory();
                }
                return trainingPlans;
            }
            set { trainingPlans = value; }
        }

        //public void RefreshSupplements()
        //{
        //    SupplementDate = null;
        //    dictSupplements = null;
        //    LoadSupplements();
        //}

        //public void RefreshWorkoutPlans()
        //{
        //    dictPlans = null;
        //    PlanDate = null;
        //    LoadWorkoutPlans();
        //}

        //public void RefreshExercises()
        //{
        //    ExerciseDate = null;
        //    dictExercises = null;
        //    LoadExercises();
        //}

        

        //public bool ExerciseLoaded
        //{
        //    get { return !ShouldRefreshExercises; }
        //    private set{}
        //}

        //public bool SupplementsLoaded
        //{
        //    get { return !ShouldRefreshSupplements; }
        //    private set { }
        //}

        //public bool WorkoutPlansLoaded
        //{
        //    get { return !ShouldRefreshPlans; }
        //    private set { }
        //}

        //public void LoadWorkoutPlans()
        //{
        //    if (dictPlans == null || ShouldRefreshPlans)
        //    {
        //        WorkoutPlanSearchCriteria criteria = new WorkoutPlanSearchCriteria();
        //        criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
        //        criteria.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
        //        PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
        //        pageInfo.PageSize = 0;

        //        var m = new ServiceManager<GetWorkoutPlansCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetWorkoutPlansCompletedEventArgs> operationCompleted)
        //        {
        //            client1.GetWorkoutPlansAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
        //            client1.GetWorkoutPlansCompleted -= operationCompleted;
        //            client1.GetWorkoutPlansCompleted += operationCompleted;

        //        });
        //        m.OperationCompleted += (s, a) =>
        //        {
        //            if(a.Error!=null)
        //            {
        //                FaultException<ValidationFault> faultEx = a.Error as FaultException<ValidationFault>;
        //                if (faultEx != null)
        //                {
        //                    BAMessageBox.ShowError(faultEx.Detail.Details[0].Key + ":" + faultEx.Detail.Details[0].Message);
        //                    return;
        //                }
        //                BAMessageBox.ShowError(ApplicationStrings.ObjectsReposidory_ErrRetrieveWorkoutPlans);
        //            }
        //            PlanDate = DateTime.UtcNow;
        //            dictPlans = a.Result.Result.Items.ToDictionary(t => t.GlobalId);
        //            onLoadedPlans();
        //        };
        //        if(!m.Run())
        //        {
        //            if (ApplicationState.Current.IsOffline)
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
        //            }
        //            else
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
        //            }
        //            onLoadedPlans();
        //        }
        //    }
        //    else
        //    {
        //        onLoadedPlans();
        //    }
        //}

        //public void LoadExercises()
        //{
        //    if (dictExercises == null || ShouldRefreshExercises)
        //    {
        //        ExerciseSearchCriteria criteria = new ExerciseSearchCriteria();
        //        criteria.SearchGroups = new List<ExerciseSearchCriteriaGroup>();
        //        criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Mine);
        //        criteria.SearchGroups.Add(ExerciseSearchCriteriaGroup.Global);
        //        criteria.ExerciseTypes = new List<ExerciseType>();
        //        PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
        //        pageInfo.PageSize = 0;

        //        var m = new ServiceManager<GetExercisesCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetExercisesCompletedEventArgs> operationCompleted)
        //        {
        //            client1.GetExercisesAsync(ApplicationState.Current.SessionData.Token, criteria, pageInfo);
        //            client1.GetExercisesCompleted -= operationCompleted;
        //            client1.GetExercisesCompleted += operationCompleted;

        //        });
        //        m.OperationCompleted += (s, a) =>
        //        {
        //            if(a.Error!=null)
        //            {
        //                onLoadedExercises();
        //                return;
        //            }
        //            ExerciseDate = DateTime.UtcNow;
        //            dictExercises = a.Result.Result.Items.ToDictionary(t => t.GlobalId);
        //            onLoadedExercises();
        //        };
        //        if(!m.Run())
        //        {
        //            if (ApplicationState.Current.IsOffline)
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
        //            }
        //            else
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
        //            }
        //            onLoadedExercises();
        //        }
                
        //    }
        //    else
        //    {
        //        onLoadedExercises();
        //    }
        //}

        //bool shouldRefreshImplementation(DateTime? date,object dict)
        //{
        //    if (date == null || dict==null)// || ApplicationState.IsFree)
        //    {
        //        return true;
        //    }
        //    if(Settings.RefreshFrequencyDays==-1)
        //    {//setting never
        //        return false;
        //    }
        //    int refreshDays = Settings.RefreshFrequencyDays;
        //    if(refreshDays==0)
        //    {//for refresh Every login we assume for this calculation that the period is 1 day but during login basically we delete the cache
        //        refreshDays = 1;
        //    }
        //    TimeSpan time = DateTime.UtcNow - date.Value;
        //    return !ApplicationState.Current.IsOffline &&  time.TotalDays > refreshDays;
        //}

        //private bool ShouldRefreshPlans
        //{
        //    get { return shouldRefreshImplementation(PlanDate,dictPlans); }
        //    set { }
        //}

        //private bool ShouldRefreshExercises
        //{
        //    get { return shouldRefreshImplementation(ExerciseDate,dictExercises); }
        //    set { }
        //}

        //private bool ShouldRefreshSupplements
        //{
        //    get { return shouldRefreshImplementation(SupplementDate,dictSupplements); }
        //    set { }
        //}

        //public void LoadSupplements()
        //{
        //    if (dictSupplements == null || ShouldRefreshSupplements)
        //    {
        //        PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
        //        pageInfo.PageSize = 0;

        //        var m = new ServiceManager<GetSuplementsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetSuplementsCompletedEventArgs> operationCompleted)
        //                                                                        {
        //            var param =new GetSupplementsParam();
        //            client1.GetSuplementsAsync(ApplicationState.Current.SessionData.Token,param,  pageInfo);
        //            client1.GetSuplementsCompleted -= operationCompleted;
        //            client1.GetSuplementsCompleted += operationCompleted;

        //        });
        //        m.OperationCompleted += (s, a) =>
        //        {
        //            if(a.Error!=null)
        //            {
        //                onLoadedSupplements();
        //                return;
        //            }
        //            SupplementDate = DateTime.UtcNow;
        //            dictSupplements = a.Result.Result.Items.ToDictionary(t => t.GlobalId);
        //            onLoadedSupplements();
        //        };
        //        if(!m.Run())
        //        {
        //            if (ApplicationState.Current.IsOffline)
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
        //            }
        //            else
        //            {
        //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
        //            }
        //            onLoadedSupplements();
        //        }
        //    }
        //    else
        //    {
        //        onLoadedSupplements();
        //    }
        //}

        //private void onLoadedExercises()
        //{
        //    if(LoadedExercises!=null)
        //    {
        //        LoadedExercises(null, EventArgs.Empty);
        //    }
        //}

        //private void onLoadedPlans()
        //{
        //    if (LoadedWorkoutPlans != null)
        //    {
        //        LoadedWorkoutPlans(null, EventArgs.Empty);
        //    }
        //}

        //private void onLoadedSupplements()
        //{
        //    if (LoadedSupplements != null)
        //    {
        //        LoadedSupplements(null, EventArgs.Empty);
        //    }
        //}

        //public IDictionary<Guid, SuplementDTO> Supplements
        //{
        //    get
        //    {
        //        return dictSupplements;
        //    }
        //    set { dictSupplements = value; }
        //}

        //public IDictionary<Guid, TrainingPlanInfo> Plans
        //{
        //    get
        //    {
        //        return dictPlans;
        //    }
        //    set { dictPlans = value; }
        //}

        //public IDictionary<Guid, ExerciseDTO> Exercises
        //{
        //    get
        //    {
        //        return dictExercises;
        //    }
        //    set { dictExercises = value; }
        //}

        //public ExerciseDTO GetExercise(Guid id)
        //{
        //    if (dictExercises.ContainsKey(id))
        //    {
        //        return dictExercises[id];
        //    }
        //    return ExerciseDTO.Deleted;
        //}



        //public SuplementDTO GetSupplement(Guid id)
        //{
        //    if (dictSupplements.ContainsKey(id))
        //    {
        //        return dictSupplements[id];
        //    }
        //    return null;
        //}

    }
}
