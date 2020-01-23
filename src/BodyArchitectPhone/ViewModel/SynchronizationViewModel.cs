using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Newtonsoft.Json;

namespace BodyArchitect.WP7.ViewModel
{
    public class SynchronizationViewModel:ViewModelBase
    {
        private ObservableCollection<SynchronizationItemViewModel> items = new ObservableCollection<SynchronizationItemViewModel>();
        private int currentItemIndex;
        private Visibility progressVisibility = Visibility.Collapsed;
        private int maximum;
        public event EventHandler SynchronizationCompleted;
        private ApplicationState appState;
        public SynchronizationViewModel( ApplicationState appState)
        {
            this.appState = appState;
            //find modified training days
            var modifiedEntries = appState.MyDays.SelectMany(x=>x.Value.GetLocalModifiedEntries());
            foreach (var info in modifiedEntries)
            {
                items.Add(new SynchronizationItemViewModel(info,this));
            }
            //now find all not saved gps points
            var notSavedGpsCoordinates=modifiedEntries.SelectMany(x => x.GPSCoordinates).Where(x => !x.Value.IsSaved).ToList();
            foreach (var info in notSavedGpsCoordinates)
            {
                var tdi=modifiedEntries.SingleOrDefault(x => x.GPSCoordinates.ContainsKey(info.Key));
                var entry=tdi.TrainingDay.Objects.OfType<GPSTrackerEntryDTO>().SingleOrDefault(x => x.GlobalId == info.Key.Id || x.InstanceId == info.Key.Id);
                if (entry != null)
                {
                    items.Add(new SynchronizationItemViewModel(tdi,entry,info.Value, this));
                }
            }
        }

        public ObservableCollection<SynchronizationItemViewModel> Items
        {
            get { return items; }
        }

        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (maximum != value)
                {
                    maximum = value;
                    NotifyPropertyChanged("Maximum");
                }
            }
        }

        public Visibility ProgressVisibility
        {
            get { return progressVisibility; }
            set
            {
                if (progressVisibility != value)
                {
                    progressVisibility = value;
                    NotifyPropertyChanged("ProgressVisibility");
                }
            }
        }
        public int CurrentItemIndex
        {
            get { return currentItemIndex; }
            set
            {
                if(currentItemIndex!=value)
                {
                    currentItemIndex = value;
                    NotifyPropertyChanged("CurrentItemIndex");
                }
            }
        }


        async protected virtual Task uploadGPSCoordinates(SynchronizationItemViewModel day, bool firstTime)
        {

            var result = await BAService.GPSCoordinatesOperationAsync(day.GPSEntry.GlobalId, GPSCoordinatesOperationType.UpdateCoordinates, day.GPSBag.Points);
            if (result.GPSTrackerEntry != null)
            {
                result.GPSTrackerEntry.InstanceId = day.GPSEntry.InstanceId;
                day.GPSEntry.GlobalId = result.GPSTrackerEntry.GlobalId;
            }
            uploadGPSResult(result, day, firstTime);
            
            
            //var m = new ServiceManager<AsyncCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<AsyncCompletedEventArgs> operationCompleted)
            //{
            //    using (OperationContextScope ocs = new OperationContextScope(client1.InnerChannel))
            //    {
            //        var ggg = (IBodyArchitectAccessService)client1;
            //        GPSOperationParam dto = new GPSOperationParam();
            //        var json = JsonConvert.SerializeObject(day.GPSBag.Points);
            //        var bytes = UTF8Encoding.UTF8.GetBytes(json);
            //        dto.CoordinatesStream = bytes.ToZip();

            //        GPSOperationData param = new GPSOperationData();
            //        param.GPSTrackerEntryId = day.GPSEntry.GlobalId;
            //        param.Operation = GPSCoordinatesOperationType.UpdateCoordinates;

            //        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("SessionId", "http://MYBASERVICE.TK/", ApplicationState.Current.SessionData.Token.SessionId));
            //        OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader("Params", "http://MYBASERVICE.TK/", param));
            //        ApplicationState.AddCustomHeaders();

            //        ggg.BeginGPSCoordinatesOperation(dto, delegate(IAsyncResult aRes)
            //        {
            //            var proxy = (IBodyArchitectAccessService)aRes.AsyncState;
            //            //using (OperationContextScope o = new OperationContextScope(((BodyArchitectAccessServiceClient)proxy).InnerChannel))
            //            {
                            
            //                try
            //                {
            //                    GPSCoordinatesOperationResult result = proxy.EndGPSCoordinatesOperation(aRes);
            //                    uploadGPSResult(result, day, firstTime);
            //                }
            //                catch (FaultException<BAServiceException> ex)
            //                {
            //                    if (ex.Detail.ErrorCode == ErrorCode.ObjectNotFound)
            //                    {
            //                        uploadGPSResult(new GPSCoordinatesOperationResult(null), day, firstTime);
            //                        return;
            //                    }
            //                    uploadGPSResult(null, day, firstTime);
            //                    day.State = MergeState.Error;
            //                }
            //                catch (Exception ex)
            //                {
            //                    uploadGPSResult(null, day, firstTime);
            //                    day.State = MergeState.Error;
            //                }
            //                finally
            //                {
            //                    IsBusy = false;
            //                }
            //            }
            //        }, client1);

            //    }


            //});
            //if (!m.Run())
            //{
                
            //}
        }
        public bool IsInProgress { get; private set; }

        private volatile bool cancelled;

        async Task merge(SynchronizationItemViewModel day)
        {
            try
                        {
                            await MergeTrainingDayFromDb(day);
                        }
                        catch (Exception)
                        {
                            day.State = MergeState.Error;
                        }
        }

        protected virtual async Task save(SynchronizationItemViewModel day, bool firstTime)
        {
            if (day.ItemType == ItemType.TrainingDay)
            {
                bool shouldMerge = false;
                try
                {
                    await saveTrainingDay(day);
                }
                catch (OldDataException)
                {
                    
                    day.DayInfo.IsConflict = true;
                    if (this.Action != MergeAction.InConflictSkip && firstTime)
                    {
                        //now we must get the copy from db and merge it
                        shouldMerge = true;
                        //return;
                    }
                    else
                    {
                        day.State = MergeState.Error;
                    }
                    
                    //when we skip in confict or this is the second save attempt the we basically mark this item as IsConflict and leave it
                }
                catch(Exception ex)
                {
                    day.State = MergeState.Error;
                }
                if (shouldMerge)
                {
                    merge(day);
                }
            }
            else 
            {
                if (day.GPSEntry != null) //null is when this gps entry has been removed on the server
                {
                    try
                    {
                        await uploadGPSCoordinates(day, firstTime);
                    }
                    catch (ObjectNotFoundException ex)
                    {
                        day.State = MergeState.Finished;
                        Items.Remove(day);
                    }
                    catch (Exception ex)
                    {
                        day.DayInfo.IsModified = true;
                        day.State = MergeState.Error;
                    }
                    
                }
                else
                {
                    uploadGPSResult(new GPSCoordinatesOperationResult(null), day, firstTime);
                }
            }
        }

        async virtual protected Task saveTrainingDay(SynchronizationItemViewModel day)
        {
            var res=await BAService.SaveTrainingDayAsync(day.DayInfo.TrainingDay);
            await applySave(res, day);
            //var m = new ServiceManager<SaveTrainingDayCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<SaveTrainingDayCompletedEventArgs> operationCompleted)
            //{
            //    client1.SaveTrainingDayCompleted -= operationCompleted;
            //    client1.SaveTrainingDayCompleted += operationCompleted;
            //    client1.SaveTrainingDayAsync(ApplicationState.Current.SessionData.Token, day.DayInfo.TrainingDay);

            //});


            //m.OperationCompleted += (s, a) =>
            //{
            //    saveResult(a.Result, day, firstTime);
            //};

            //if (!m.Run())
            //{
            //    IsBusy = false;
            //    day.State = MergeState.Error;
            //}
        }

        public MergeAction Action { get; set; }

        async protected  virtual Task MergeTrainingDayFromDb(SynchronizationItemViewModel item)
        {
            WorkoutDayGetOperation data = new WorkoutDayGetOperation();
            data.WorkoutDateTime = item.DayInfo.TrainingDay.TrainingDate;
            data.CustomerId = item.DayInfo.TrainingDay.CustomerId;
            data.Operation = GetOperation.Current;
            var trainingDay = await BAService.GetTrainingDayAsync(data, new RetrievingInfo());
            await MergeResult(trainingDay, item);

            //var m = new ServiceManager<GetTrainingDayCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDayCompletedEventArgs> operationCompleted)
            //{
            //    client1.GetTrainingDayCompleted -= operationCompleted;
            //    client1.GetTrainingDayCompleted += operationCompleted;
            //    WorkoutDayGetOperation data = new WorkoutDayGetOperation();
            //    data.WorkoutDateTime = item.DayInfo.TrainingDay.TrainingDate;
            //    data.CustomerId = item.DayInfo.TrainingDay.CustomerId;
            //    data.Operation = GetOperation.Current;
            //    client1.GetTrainingDayAsync(ApplicationState.Current.SessionData.Token, data, new RetrievingInfo());

            //});


            //m.OperationCompleted += (s, a) =>
            //                            {
            //                                await MergeResult(a.Result, item);

            //                            };
            //if (!m.Run())
            //{
            //    IsBusy = false;
            //    item.State = MergeState.Error;
            //}
        }

        async protected virtual Task MergeResult(TrainingDayDTO day, SynchronizationItemViewModel item)
        {
            
            var customerId = item.DayInfo.TrainingDay.CustomerId;
            var holder = appState.GetTrainingDayHolder(customerId);
            OfflineModeManager manager = new OfflineModeManager(appState.MyDays,ApplicationState.Current.SessionData.Profile.GlobalId);
            appState.TrainingDay = item.DayInfo;
            manager.MergeNew(day, appState,true, delegate
            {
                return Action == MergeAction.UseServer;
            });

            if (holder.TrainingDays.ContainsKey(appState.TrainingDay.TrainingDay.TrainingDate) && holder.TrainingDays[appState.TrainingDay.TrainingDay.TrainingDate].IsModified)
            {
                await save(item, false);
            }
            else
            {
                item.State = MergeState.Finished;
            }
            appState.TrainingDay = null;
        }

        protected virtual void uploadGPSResult(GPSCoordinatesOperationResult res, SynchronizationItemViewModel item,bool firstTime)
        {

            
            if (res.GPSTrackerEntry != null)
            {
                var customerId = item.DayInfo.TrainingDay.CustomerId;
                var holder = appState.GetTrainingDayHolder(customerId);

                item.DayInfo.GetGpsCoordinates(res.GPSTrackerEntry).IsSaved = true;

                item.State = MergeState.Finished;
                var dayInCache=holder.TrainingDays.Where(
                    x => x.Value.TrainingDay.Objects.Where(h => h.GlobalId == res.GPSTrackerEntry.GlobalId).Count() == 1)
                      .SingleOrDefault();
                dayInCache.Value.Update(res.GPSTrackerEntry);
            }
            item.State = MergeState.Finished;
            Items.Remove(item);
        }

        
        async protected Task applySave(SaveTrainingDayResult result, SynchronizationItemViewModel item)
        {
            var newDay = result.TrainingDay;
            var customerId = item.DayInfo.TrainingDay.CustomerId;
            var holder = appState.GetTrainingDayHolder(customerId);
            TrainingDayInfo newInfo=null;
            if (newDay != null)
            {
                var oldInfo=holder.TrainingDays[newDay.TrainingDate];
                newDay.FillInstaneId(oldInfo.TrainingDay, true);
                //oldInfo.TrainingDay.GlobalId = newDay.GlobalId;
                //find all gps tracker entries and set correct globalid (we need this in uploading gps coordinates)
                //foreach (var gpsEntry in newDay.Objects.OfType<GPSTrackerEntryDTO>())
                //{
                //    var oldEntry=oldInfo.TrainingDay.Objects.OfType<GPSTrackerEntryDTO>().Where(x => x.Exercise.GlobalId == gpsEntry.Exercise.GlobalId && x.StartDateTime==gpsEntry.StartDateTime && x.EndDateTime==gpsEntry.EndDateTime ).FirstOrDefault();
                //    if (oldEntry != null)
                //    {
                //        oldEntry.GlobalId = gpsEntry.GlobalId;
                //        gpsEntry.InstanceId = oldEntry.InstanceId;
                //    }
                //}
                //now check if gps entry has been removed on the server and if yes remove also gps coordinates uploading
                foreach (var source in Items.Where(x => x.DayInfo == oldInfo && x.ItemType==ItemType.GPSCoordinates).ToList())
                {
                    var oldEntry = newDay.Objects.OfType<GPSTrackerEntryDTO>().Where(x => (source.GPSEntry.GlobalId!=Guid.Empty && x.GlobalId == source.GPSEntry.GlobalId) || (x.Exercise.GlobalId == source.GPSEntry.Exercise.GlobalId && x.StartDateTime == source.GPSEntry.StartDateTime && x.EndDateTime == source.GPSEntry.EndDateTime)).FirstOrDefault();
                    if (oldEntry == null)
                    {
                        source.GPSEntry = null;
                    }
                }
                newInfo = holder.SetTrainingDay(newDay);
                newInfo.IsModified = false;
            }
            else
            {
                holder.TrainingDays.Remove(item.DayInfo.TrainingDay.TrainingDate);
            }
            item.DayInfo.IsConflict = false;
            item.DayInfo = newInfo;
            item.State = MergeState.Finished;
            Items.Remove(item);
        }

        async public Task Synchronize()
        {
            cancelled = false;
            if(IsInProgress)
            {
                return;
            }
            IsInProgress = true;

            Queue<SynchronizationItemViewModel> queue = new Queue<SynchronizationItemViewModel>(Items);
            Maximum = queue.Count;
            ProgressVisibility = Visibility.Visible;
            try
            {

                while (true)
                {
                    if (cancelled)
                    {
                        return;
                    }
                    if (queue.Count > 0)
                    {
                        CurrentItemIndex = Maximum - queue.Count;
                        var item = queue.Dequeue();
                        item.State = MergeState.Processing;
                        await save(item, true);
#if DEBUG
                        await Task.Factory.StartNew(() => Thread.Sleep(1000));
#endif
                    }
                    else
                    {
                        break;
                    }

                }
            }
            finally
            {
                ProgressVisibility = Visibility.Collapsed;
                IsInProgress = false;
                onSynchronizationCompleted();
            }
            
        }

        void onSynchronizationCompleted()
        {
            if(SynchronizationCompleted!=null)
            {
                SynchronizationCompleted(this, EventArgs.Empty);
            }
        }

        public void Cancel()
        {
            if(IsInProgress)
            {
                cancelled = true;
                IsInProgress = false;
                onSynchronizationCompleted();
            }
        }
    }
}
