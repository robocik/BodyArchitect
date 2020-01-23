using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using Microsoft.Phone.Net.NetworkInformation;
using PartialRetrievingInfo = BodyArchitect.Service.V2.Model.PartialRetrievingInfo;
using ProfileInformationDTO = BodyArchitect.Service.V2.Model.ProfileInformationDTO;
using TrainingDayDTO = BodyArchitect.Service.V2.Model.TrainingDayDTO;

namespace BodyArchitect.Service.Client.WP7
{
    

    public class ApplicationState
    {
        private static ApplicationState current;

        private ObjectsReposidory objecsRepo;
        
        public static event EventHandler OfflineModeChanged;

        
        private Dictionary<CacheKey, TrainingDaysHolder> myDays = new Dictionary<CacheKey, TrainingDaysHolder>();
        //private TrainingDaysHolder myDays;

        public Dictionary<CacheKey, TrainingDaysHolder> MyDays
        {
            get { return myDays; }
            set { myDays = value; }
        }

        public static string CurrentLanguage
        {
            get { return CultureInfo.CurrentUICulture.Name; }
        }

        public static string CurrentServiceLanguage
        {
            get
            {
                if(CurrentLanguage=="pl-PL")
                {
                    return "pl";
                }
                return "en";
            }
        }


        private TrainingDaysHolder _currentBrowsingTrainingDays;

        public TrainingDaysHolder CurrentBrowsingTrainingDays
        {
            get
            {
                if (_currentBrowsingTrainingDays==null)
                {
                    return this.GetTrainingDayHolder(CurrentViewCustomer != null ? CurrentViewCustomer.GlobalId : (Guid?) null);
                }
                return _currentBrowsingTrainingDays;
            }
            set { _currentBrowsingTrainingDays = value; }
        }

        public List<string> UpdateVersions
        {
            get { return _updateVersions; }
            set { _updateVersions = value; }
        }

        //[DoNotSerialize]
        public bool IsTimerEnabled { get; set; }

        //[DoNotSerialize]
        public DateTime? TimerStartTime { get; set; }

        public bool IsOffline
        {
            get;
            set;
        }

        public bool IsPremium
        {
            get { return ProfileInfo!=null&& ProfileInfo.Licence.CurrentAccountType != AccountType.User; }
        }

        public bool IsInstructor
        {
            get { return ProfileInfo != null && ProfileInfo.Licence.CurrentAccountType != AccountType.User && ProfileInfo.Licence.CurrentAccountType != AccountType.PremiumUser; }
        }

        public bool Crash { get; set; }

        
        public static ApplicationState Current
        {
            get
            {
                return current;
            }
            set
            { 
                if(current!=value)
                {
                    current = value;    
                    if(OfflineModeChanged!=null)
                    {
                        OfflineModeChanged(current, EventArgs.Empty);
                    }
                }
                
            }
        }

        public static ApplicationState LoadState()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(Constants.StateFileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.StateFileName, FileMode.Open, store))
                        {
                            //DataContractSerializer serializer = new DataContractSerializer(typeof(ApplicationState));
                            //return (ApplicationState)serializer.ReadObject(stream);
                            var state= (ApplicationState)SilverlightSerializer.Deserialize(stream);
                            return state;
                        }
                    }

                }
                
            }
            catch(Exception ex)
            {
#if DEBUG
                //throw;
#endif
            }

            return null;
        }

        //this method is invoked mostly at the application startup. In normal case (when application runs without crash) these fields are nulled automatically. But when we have a crash then when
        //application is started again there could be a situation that for example CurrentViewCustomer is set to some customer. In this case when user press Gym in today he will create an entry
        //for this customer and not for himself (bug)
        public void ResetCurrents()
        {
            TrainingDay = null;
            CurrentEntryId = null;
            this.CurrentViewCustomer = null;
            CurrentViewUser = null;
            TimerStartTime = null;
            IsTimerEnabled = false;
        }

        public void SaveState(bool crash)
        {
            try
            {
                Crash = crash;
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(Constants.StateFileName, FileMode.Create, store))
                    {
                        //DataContractSerializer serializer = new DataContractSerializer(typeof(ApplicationState));
                        //serializer.WriteObject(stream, this);
                        SilverlightSerializer.Serialize(this,stream);
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                //throw;
#endif
            }
        }

        


        public TrainingPlan CurrentTrainingPlan { get; set; }


        private SessionData _sessionData;
        private List<string> _updateVersions = new List<string>();

        public SessionData SessionData
        {
            get { return _sessionData; }
            set { _sessionData = value; }
        }

        public string TempUserName { get; set; }
        public string TempPassword { get; set; }

        public ObjectsReposidory Cache
        {
            get
            {
                if (objecsRepo == null)
                {
                    objecsRepo = new ObjectsReposidory();
                }
                return objecsRepo;
            }
            set { objecsRepo = value; }
        }

        public static BodyArchitectAccessServiceClient CreateService()
        {
            var endPoint = "Production";

            if (IsolatedStorageSettings.ApplicationSettings.Contains("EndPoint"))
            {
                endPoint = (string)IsolatedStorageSettings.ApplicationSettings["EndPoint"];
            }
            BodyArchitectAccessServiceClient client = new BodyArchitectAccessServiceClient(endPoint);
            
            return client;
        }

        public static void AddCustomHeaders()
        {

            MessageHeader head = MessageHeader.CreateHeader(Portable.Constants.HeaderAPIKey, string.Empty, "A3C9D236-2566-40CF-A430-0802EE439B9C");
            OperationContext.Current.OutgoingMessageHeaders.Add(head);
            MessageHeader language = MessageHeader.CreateHeader(Portable.Constants.HeaderLanguage, string.Empty, CurrentLanguage);
            OperationContext.Current.OutgoingMessageHeaders.Add(language);
        }

        public LocalObjectKey CurrentEntryId { get; set; }

        private TrainingDayInfo trainingDayInfo;

        public TrainingDayInfo TrainingDay
        {
            get { return trainingDayInfo; }
            set
            {
                trainingDayInfo = value;
                if (trainingDayInfo == null)
                {
                    CurrentEntryId = null;
                }
            }
        }

        public ProfileInformationDTO ProfileInfo
        {
            get;
            set;
        }

        public ProfileInformationDTO EditProfileInfo
        {
            get;
            set;
        }

        public static SynchronizationContext SynchronizationContext
        {
            get; set; }

        public CustomerDTO CurrentViewCustomer
        {
            get;
            set;
        }

        public UserSearchDTO CurrentViewUser
        {
            get; set;
        }

        public event EventHandler<DateEventArgs> TrainingDaysRetrieved;

        void onTrainingDaysRetrieved(DateTime monthDate)
        {
            if(TrainingDaysRetrieved!=null)
            {
                TrainingDaysRetrieved(this, new DateEventArgs(monthDate));
            }
        }

        public async Task RetrieveMonthAsync(DateTime monthDate, TrainingDaysHolder holder)
        {
            DateTime monthTOShow = monthDate;
            WorkoutDaysSearchCriteria search = new WorkoutDaysSearchCriteria();
            search.UserId = holder.User != null ? (Guid?)holder.User.GlobalId : null;
            search.CustomerId = holder.CustomerId;
            search.EndDate = monthDate.AddMonths(1);
            if (Settings.NumberOfMonthToRetrieve > 1)
            {//-1 because we need to subtract months
                monthDate = monthDate.AddMonths(-1 * (Settings.NumberOfMonthToRetrieve - 1));
            }
            search.StartDate = monthDate;
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 0;//we want to download all entries in selected period of time

            try
            {
                var result = await BAService.GetTrainingDaysAsync(search, pageInfo);
                RetrievedDays(monthDate, Settings.NumberOfMonthToRetrieve, result.Items, holder);
            }
            catch (NetworkException)
            {
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowWarning(ApplicationStrings.EntryObjectPageBase_SavedLocallyOnly);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
            catch (Exception)
            {
                BAMessageBox.ShowError(ApplicationStrings.ApplicationState_ErrRetrieveTrainingDays);
            }




            //var m = new ServiceManager<GetTrainingDaysCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDaysCompletedEventArgs> operationCompleted)
            //{
            //    client1.GetTrainingDaysCompleted -= operationCompleted;
            //    client1.GetTrainingDaysCompleted += operationCompleted;
            //    client1.GetTrainingDaysAsync(ApplicationState.Current.SessionData.Token, search, pageInfo);
            //});
            //m.OperationCompleted += (s, a) =>
            //   {

            //    if (a.Error != null)
            //    {
            //        onTrainingDaysRetrieved(monthTOShow);
            //        BAMessageBox.ShowError(ApplicationStrings.ApplicationState_ErrRetrieveTrainingDays);
            //        return;
            //    }
            //    if (a.Result != null && a.Result.Result!=null )
            //    {
            //        RetrievedDays(monthDate, Settings.NumberOfMonthToRetrieve, a.Result.Result.Items, holder);
            //        onTrainingDaysRetrieved(monthTOShow);
            //    }
            //};

            //if(!m.Run())
            //{
            //    if(ApplicationState.Current.IsOffline)
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
            //    }
            //    else
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);    
            //    }
            //    onTrainingDaysRetrieved(monthTOShow);
            //}
        }
 
        public async void RetrieveMonth(DateTime monthDate,TrainingDaysHolder holder )
        {
            DateTime monthTOShow = monthDate;
            WorkoutDaysSearchCriteria search = new WorkoutDaysSearchCriteria();
            search.UserId = holder.User != null ? (Guid?)holder.User.GlobalId : null;
            search.CustomerId = holder.CustomerId;
            search.EndDate = monthDate.AddMonths(1);
            if (Settings.NumberOfMonthToRetrieve>1)
            {//-1 because we need to subtract months
                monthDate = monthDate.AddMonths(-1*(Settings.NumberOfMonthToRetrieve - 1));
            }
            search.StartDate = monthDate;
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 0;//we want to download all entries in selected period of time



            var m = new ServiceManager<GetTrainingDaysCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDaysCompletedEventArgs> operationCompleted)
            {
                client1.GetTrainingDaysCompleted -= operationCompleted;
                client1.GetTrainingDaysCompleted += operationCompleted;
                client1.GetTrainingDaysAsync(ApplicationState.Current.SessionData.Token, search, pageInfo);
            });
            m.OperationCompleted += (s, a) =>
               {

                   if (a.Error != null)
                   {
                       onTrainingDaysRetrieved(monthTOShow);
                       BAMessageBox.ShowError(ApplicationStrings.ApplicationState_ErrRetrieveTrainingDays);
                       return;
                   }
                   if (a.Result != null && a.Result.Result != null)
                   {
                       RetrievedDays(monthDate, Settings.NumberOfMonthToRetrieve, a.Result.Result.Items, holder);
                       onTrainingDaysRetrieved(monthTOShow);
                   }
               };

            if (!m.Run())
            {
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
                onTrainingDaysRetrieved(monthTOShow);
            }
        }

        public void RetrievedDays(DateTime startMonth, int months, IEnumerable<TrainingDayDTO> days, TrainingDaysHolder holder)
        {
            OfflineModeManager manager = new OfflineModeManager(MyDays,SessionData.Profile.GlobalId);
            manager.RetrievedDays(startMonth, months, days, holder);
        }

        public static void GoToOfflineMode()
        {
            try
            {
                ApplicationState state = LoadState();
                if(state==null )
                {
                    if (Current == null)
                    {
                        throw new InvalidOperationException("You must at lease login once to use offline mode");
                    }
                    else
                    {
                        state=new ApplicationState();
                    }
                }


                if (ApplicationState.Current != null)
                {
                    state.Cache = ApplicationState.Current.Cache.Copy();
                    state.ProfileInfo = Current.ProfileInfo;
                    state.SessionData = Current.SessionData;
                    if(Current.MyDays!=null)
                    {
                        state.MyDays = Current.MyDays;
                    }

                    state.TempUserName = Current.TempUserName;
                    state.TempPassword = Current.TempPassword;
                }

                state.CurrentBrowsingTrainingDays = null;
                state.IsOffline = true;
                if (Deployment.Current.Dispatcher.CheckAccess())
                {
                    ApplicationState.Current = state;
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => ApplicationState.Current = state);    
                }
                

            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("You must at lease login once to use offline mode");
            }
            
        }

        public static void ClearOffline()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (store.FileExists(Constants.StateFileName))
                    {
                        store.DeleteFile(Constants.StateFileName);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }


        

        public void ClearTrainingDays()
        {
            OfflineModeManager manager = new OfflineModeManager(myDays, Current.SessionData.Profile.GlobalId);
            manager.ClearTrainingDays();
        }

        public TrainingDaysHolder GetTrainingDayHolder(Guid? customerId)
        {
            CacheKey key=new CacheKey(){CustomerId=customerId,ProfileId = Current.SessionData.Profile.GlobalId};
            if (!myDays.ContainsKey(key))
            {
                myDays.Add(key,new TrainingDaysHolder(customerId));
            }
            return myDays[key];
        }

    }
}
