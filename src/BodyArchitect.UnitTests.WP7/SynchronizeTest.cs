using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

namespace BodyArchitect.UnitTests.WP7
{
    [TestClass]
    public class SynchronizeTest : SilverlightTest
    {
        [TestMethod]
        public void InitialState()
        {
            ApplicationState appState = new ApplicationState();
            ApplicationState.Current = appState;
            appState.SessionData=new SessionData();
            appState.SessionData.Profile=new ProfileDTO(){GlobalId = Guid.NewGuid()};
            appState.MyDays=new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder=appState.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }) { IsModified = true });
            holder.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
            holder.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(3) }) { IsModified = true });
            var tdi = new TrainingDayInfo(new TrainingDayDTO() {TrainingDate = time.AddDays(4)}) {IsModified = true};
            //these points should be detected to save
            GPSTrackerEntryDTO gpsEntry= new GPSTrackerEntryDTO();
            tdi.TrainingDay.Objects.Add(gpsEntry);
            gpsEntry.TrainingDay = tdi.TrainingDay;
            var points = new List<GPSPoint>();
            points.Add(new GPSPoint(1,2,3,4,6));
            tdi.GPSCoordinates.Add(new LocalObjectKey(gpsEntry.InstanceId,KeyType.InstanceId),new GPSPointsBag(points,false) );

            //these shouldn't be
            var entry1 = new GPSTrackerEntryDTO();
            tdi.TrainingDay.Objects.Add(entry1);
            entry1.TrainingDay = tdi.TrainingDay;
            points = new List<GPSPoint>();
            points.Add(new GPSPoint(11, 12, 13, 14, 7));
            tdi.GPSCoordinates.Add(new LocalObjectKey(entry1.InstanceId, KeyType.InstanceId), new GPSPointsBag(points, true));

            holder.TrainingDays.Add(time.AddDays(4), tdi);

            holder.TrainingDays.Add(time.AddDays(5), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(5) }));
            holder.TrainingDays.Add(time.AddDays(6), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(6) }));

            SynchronizationViewModel viewModel = new SynchronizationViewModel(appState);
            Assert.AreEqual(4,viewModel.Items.Count);
            Assert.AreEqual(Visibility.Collapsed, viewModel.ProgressVisibility);
            Assert.AreEqual(holder.TrainingDays[time.AddDays(1)], viewModel.Items[0].DayInfo);
            Assert.AreEqual(ItemType.TrainingDay, viewModel.Items[0].ItemType);

            Assert.AreEqual(holder.TrainingDays[time.AddDays(3)], viewModel.Items[1].DayInfo);
            Assert.AreEqual(ItemType.TrainingDay, viewModel.Items[1].ItemType);

            Assert.AreEqual(holder.TrainingDays[time.AddDays(4)], viewModel.Items[2].DayInfo);
            Assert.AreEqual(ItemType.TrainingDay, viewModel.Items[2].ItemType);

            Assert.AreEqual(holder.TrainingDays[time.AddDays(4)].GPSCoordinates[new LocalObjectKey(gpsEntry.InstanceId,KeyType.InstanceId)], viewModel.Items[3].GPSBag);
            Assert.AreEqual(ItemType.GPSCoordinates, viewModel.Items[3].ItemType);
            Assert.AreEqual(holder.TrainingDays[time.AddDays(4)], viewModel.Items[3].DayInfo);
            Assert.AreEqual(gpsEntry, viewModel.Items[3].GPSEntry);
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() {TrainingDate = time.AddDays(1)};
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });
            holder.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
            holder.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(3) }) { IsModified = true });
            holder.TrainingDays.Add(time.AddDays(4), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(4) }) { IsModified = true });
            holder.TrainingDays.Add(time.AddDays(5), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(5) }));
            holder.TrainingDays.Add(time.AddDays(6), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(6) }));

            bool eventCompleted=false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.SynchronizationCompleted += delegate
                                                      {
                                                          eventCompleted = true;
                                                      };

            await viewModel.Synchronize();
            Assert.AreEqual(3,viewModel.Maximum);
            Assert.AreEqual(3, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_Correct()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            var size = new SizeEntryDTO();
            day.Objects.Add(size);
            size.TrainingDay = day;
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();

            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsFalse(items[0].DayInfo.IsModified);
            Assert.IsTrue(viewModel.IsInProgressGood);
            Assert.AreNotEqual(Guid.Empty, day.GlobalId);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_UploadGPSCoordinates()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            var tdi = new TrainingDayInfo(day) { IsModified = true };
            //these points should be detected to save
            GPSTrackerEntryDTO gpsEntry = new GPSTrackerEntryDTO();
            gpsEntry.Exercise=new ExerciseLightDTO(){GlobalId = Guid.NewGuid()};
            tdi.TrainingDay.Objects.Add(gpsEntry);
            gpsEntry.TrainingDay = tdi.TrainingDay;
            var points = new List<GPSPoint>();
            points.Add(new GPSPoint(1, 2, 3, 4, 8));
            tdi.GPSCoordinates.Add(new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId), new GPSPointsBag(points, false));

            holder.TrainingDays.Add(time.AddDays(1), tdi);


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(2, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.AreEqual(1, viewModel.GPSUploadCount);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.AreEqual(MergeState.Finished, items[1].State);
            Assert.AreNotEqual(Guid.Empty, day.GlobalId);
            Assert.AreNotEqual(Guid.Empty, gpsEntry.GlobalId);
            Assert.AreEqual(items[1].GPSEntry.GlobalId, gpsEntry.GlobalId);
            Assert.AreEqual(items[1].GPSEntry.InstanceId, gpsEntry.InstanceId);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_ErrorUploadGPSCoordinates()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            var tdi = new TrainingDayInfo(day) { IsModified = true };
            //these points should be detected to save
            GPSTrackerEntryDTO gpsEntry = new GPSTrackerEntryDTO();
            gpsEntry.Exercise = new ExerciseLightDTO() { GlobalId = Guid.NewGuid() };
            tdi.TrainingDay.Objects.Add(gpsEntry);
            gpsEntry.TrainingDay = tdi.TrainingDay;
            var points = new List<GPSPoint>();
            points.Add(new GPSPoint(1, 2, 3, 4, 5));
            tdi.GPSCoordinates.Add(new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId), new GPSPointsBag(points, false));

            holder.TrainingDays.Add(time.AddDays(1), tdi);


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.UploadGPSError = ErrorCode.NullReferenceException;
            
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(2, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.AreEqual(1, viewModel.GPSUploadCount);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.AreEqual(MergeState.Error, items[1].State);
            Assert.AreNotEqual(Guid.Empty, day.GlobalId);
            Assert.AreNotEqual(Guid.Empty, gpsEntry.GlobalId);
            Assert.AreEqual(items[1].GPSEntry.GlobalId, gpsEntry.GlobalId);
            Assert.AreEqual(items[1].GPSEntry.InstanceId, gpsEntry.InstanceId);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            Assert.IsTrue(items[0].DayInfo.IsModified);
            Assert.IsFalse(items[0].DayInfo.GPSCoordinates.First().Value.IsSaved);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_UploadGPSCoordinates_RemovedGPSTrackerEntry()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            var tdi = new TrainingDayInfo(day) { IsModified = true };
            //these points should be detected to save
            GPSTrackerEntryDTO gpsEntry = new GPSTrackerEntryDTO();
            gpsEntry.Exercise = new ExerciseLightDTO() { GlobalId = Guid.NewGuid() };
            tdi.TrainingDay.Objects.Add(gpsEntry);
            gpsEntry.TrainingDay = tdi.TrainingDay;
            var points = new List<GPSPoint>();
            points.Add(new GPSPoint(1, 2, 3, 4, 6));
            tdi.GPSCoordinates.Add(new LocalObjectKey(gpsEntry.InstanceId, KeyType.InstanceId), new GPSPointsBag(points, false));

            holder.TrainingDays.Add(time.AddDays(1), tdi);


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.UploadGPSError = ErrorCode.ObjectNotFound;
            viewModel.RemoveFromServer = typeof (GPSTrackerEntryDTO);
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(2, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.AreEqual(0, viewModel.GPSUploadCount);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.AreEqual(MergeState.Finished, items[1].State);
            Assert.AreNotEqual(Guid.Empty, day.GlobalId);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_NeedsMerge_SkipMerge()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.NeedsMergeException = true;
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.SecondSave);
            Assert.IsTrue(items[0].DayInfo.IsConflict);
            Assert.IsTrue(items[0].DayInfo.IsModified);
            Assert.AreEqual(MergeState.Error, items[0].State);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_NeedsMerge_ErrorDuringRetrieving()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.ErrorDuringRetrieve = true;
            viewModel.NeedsMergeException = true;
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };

            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsFalse(viewModel.SecondSave);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsTrue(viewModel.Items[0].DayInfo.IsConflict);
            Assert.IsTrue(viewModel.Items[0].DayInfo.IsModified);
            Assert.AreEqual(MergeState.Error,viewModel.Items[0].State);
            Assert.IsFalse(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod,Asynchronous]
        async public void SyncOperation_NeedsMerge_Merged()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.Action = MergeAction.UseServer;
            viewModel.NeedsMergeException = true;
            viewModel.FromServer = day.Copy();
            viewModel.FromServer.Comment = "test";
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(2, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsTrue(viewModel.SecondSave);
            Assert.IsFalse(items[0].DayInfo.IsModified);
            Assert.IsFalse(items[0].DayInfo.IsConflict);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.IsTrue(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_NeedsMerge_Merged_ServerVersionIsNull()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.Action = MergeAction.UseServer;
            viewModel.NeedsMergeException = true;
            viewModel.FromServer = null;
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };
            var items = viewModel.Items.ToArray();
            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.SecondSave);
            Assert.AreEqual(MergeState.Finished, items[0].State);
            Assert.IsTrue(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }

        [TestMethod, Asynchronous]
        async public void SyncOperation_NeedsMerge_ErrorDuringSecondSave()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.Action = MergeAction.UseServer;
            viewModel.NeedsMergeException = true;
            viewModel.ErrorInSecondSave = true;
            viewModel.FromServer = day.Copy();
            viewModel.FromServer.Comment = "test";
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };

            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(2, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsTrue(viewModel.SecondSave);
            Assert.IsTrue(viewModel.Items[0].DayInfo.IsModified);
            Assert.IsFalse(viewModel.Items[0].DayInfo.IsConflict);
            Assert.AreEqual(MergeState.Error, viewModel.Items[0].State);
            Assert.IsTrue(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }


        [TestMethod, Asynchronous]
        async public void SyncOperation_NeedsMerge_DontNeedSave()
        {
            ApplicationState state = new ApplicationState();
            ApplicationState.Current = state;
            state.SessionData = new SessionData();
            state.SessionData.Profile = new ProfileDTO() { GlobalId = Guid.NewGuid() };
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var holder = state.GetTrainingDayHolder(null);
            DateTime time = ExtensionMethods.MonthDate(DateTime.UtcNow);
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(1) };
            holder.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });
            //state.MyDays.Add(time.AddDays(1), new TrainingDayInfo(day) { IsModified = true });


            bool eventCompleted = false;

            MockSynchronizationViewModel viewModel = new MockSynchronizationViewModel(state);
            viewModel.Action = MergeAction.UseServer;
            viewModel.NeedsMergeException = true;
            viewModel.FromServer = day;
            viewModel.ErrorInSecondSave = true;
            viewModel.SynchronizationCompleted += delegate
            {
                eventCompleted = true;
            };

            await viewModel.Synchronize();
            Assert.AreEqual(1, viewModel.Maximum);
            Assert.AreEqual(1, viewModel.SaveCount);
            Assert.IsTrue(eventCompleted);
            Assert.IsTrue(viewModel.FirstSave);
            Assert.IsFalse(viewModel.SecondSave);
            Assert.IsFalse(viewModel.Items[0].DayInfo.IsConflict);
            Assert.IsFalse(viewModel.Items[0].DayInfo.IsModified);
            Assert.AreEqual(MergeState.Finished, viewModel.Items[0].State);
            Assert.IsTrue(viewModel.Merged);
            Assert.IsTrue(viewModel.IsInProgressGood);
            EnqueueTestComplete();
        }
    }

    class MockSynchronizationViewModel:SynchronizationViewModel
    {
        public int SaveCount { get; private set; }
        public bool IsInProgressGood { get; private set; }
        public bool Merged { get; private set; }
        public bool FirstSave { get; private set; }
        public bool SecondSave { get; private set; }
        public bool NeedsMergeException { get; set; }
        public bool ErrorInSecondSave { get; set; }
        public bool ErrorDuringRetrieve { get; set; }
        public int GPSUploadCount { get; private set; }
        public ErrorCode? UploadGPSError { get; set; }
        public TrainingDayDTO FromServer
        {
            get; set; }

        public Type RemoveFromServer
        {
            get; set;
        }

        public MockSynchronizationViewModel(ApplicationState state) : base(state)
        {
            IsInProgressGood = true;
        }

        async protected override Task uploadGPSCoordinates(SynchronizationItemViewModel day, bool firstTime)
        {
            GPSUploadCount++;
            if (!IsInProgress)
            {
                IsInProgressGood = false;
            }

            if (UploadGPSError == null)
            {
                GPSCoordinatesOperationResult result = new GPSCoordinatesOperationResult(day.GPSEntry.Copy());
                uploadGPSResult(result, day, firstTime);    
            }
            else if(UploadGPSError==ErrorCode.ObjectNotFound)
            {
                throw new ObjectNotFoundException();
            }
            else
            {
                throw new Exception();
            }
            
        }

        protected override async Task save(SynchronizationItemViewModel day, bool firstTime)
        {
            if (firstTime)
            {
                FirstSave = true;
            }
            else
            {
                SecondSave = true;
            }
            await base.save(day, firstTime);
        }

        async protected override Task saveTrainingDay(SynchronizationItemViewModel day)
        {
           
            SaveCount++;
            if(!IsInProgress)
            {
                IsInProgressGood = false;
            }
            
            Exception ex = NeedsMergeException && FirstSave&&!SecondSave ? new OldDataException() : null;
            if (ErrorInSecondSave && SecondSave)
            {
                ex = new Exception("Test");
            }

            var newDay = day.DayInfo.TrainingDay.Copy();
            if (ex == null)
            {//imitate creating a new GlobalId on the server
                newDay.GlobalId = Guid.NewGuid();
                foreach (var entryObjectDto in newDay.Objects)
                {
                    entryObjectDto.GlobalId = Guid.NewGuid();
                }
            }
            if (RemoveFromServer != null)
            {
                if (RemoveFromServer == typeof (TrainingDayDTO))
                {
                    newDay = null;
                }
                var itemToDelete=newDay.Objects.Where(x=>x.GetType()==RemoveFromServer).SingleOrDefault();
                if (itemToDelete != null)
                {
                    newDay.Objects.Remove(itemToDelete);
                }
            }

            SaveTrainingDayCompletedEventArgs res = new SaveTrainingDayCompletedEventArgs(new object[] { new SaveTrainingDayResult() { TrainingDay = newDay } }, ex, false, null);
            await applySave(res.Result,day);
        }

        
        async protected override Task MergeTrainingDayFromDb(SynchronizationItemViewModel item)
        {
            Exception ex = ErrorDuringRetrieve ? new Exception() : null;
            Merged = true;
            //GetTrainingDayCompletedEventArgs e = new GetTrainingDayCompletedEventArgs(new object[] { FromServer }, ex, false, null);
            if (ex == null)
            {
                await MergeResult(FromServer, item);
            }
            else
            {
                throw ex;
            }
        }
    }
}
