using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.Cache;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BodyArchitect.UnitTests.WP7
{
    [TestClass]
    public class OfflineModeManagerTest_V2
    {
        #region Retrieve

        [TestMethod]
        public void SimpleRetrieve()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO(){GlobalId = Guid.NewGuid()});
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);

            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            bool res = manager.RetrievedDays(time, 1, retrieved, currentHolder);
            Assert.IsTrue(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
        }

        [TestMethod]
        public void SimpleRetrieveWithModification_ChangedTrainingDay()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);

            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved,currentHolder);
            currentHolder.TrainingDays[time.AddDays(1)] = currentHolder.TrainingDays[time.AddDays(1)].Copy();
            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;
            currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Comment = "test";

            var res = manager.RetrievedDays(time, 1, retrieved, currentHolder);

            Assert.IsFalse(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
            Assert.AreEqual("test", currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
            Assert.AreEqual(true, currentHolder.TrainingDays[time.AddDays(1)].IsConflict);
        }

        TrainingDayDTO createDay(DateTime date, int version = 1)
        {
            var day = new TrainingDayDTO() { TrainingDate = date.Date };
            SizeEntryDTO size = new SizeEntryDTO();
            size.Version = version;
            day.Objects.Add(size);
            size.TrainingDay = day;
            return day;
        }

        [TestMethod]
        public void SimpleRetrieveWithModification_ChangedEntryObject()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(createDay(time.AddDays(1)));
            retrieved.Add(createDay(time.AddDays(3)));
            retrieved.Add(createDay(time.AddDays(5)));
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved,currentHolder);
            currentHolder.TrainingDays[time.AddDays(1)] = currentHolder.TrainingDays[time.AddDays(1)].Copy();
            currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Comment = "test";
            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;

            var res = manager.RetrievedDays(time, 1, retrieved, currentHolder);

            Assert.IsFalse(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
            Assert.AreEqual("test", currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Comment);
            Assert.AreEqual(true, currentHolder.TrainingDays[time.AddDays(1)].IsConflict);
        }

        [TestMethod]
        public void SimpleRetrieveWithModification_DifferentVersion_ButContentTheSame()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(createDay(time.AddDays(1)));
            retrieved.Add(createDay(time.AddDays(3)));
            retrieved.Add(createDay(time.AddDays(5)));
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved, currentHolder);

            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;

            //simulate modification of this entry on the server
            retrieved[0] = createDay(time.AddDays(1), 2);
            var res = manager.RetrievedDays(time, 1, retrieved, currentHolder);

            Assert.IsTrue(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
            Assert.AreEqual(2, currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
            Assert.AreEqual(false, currentHolder.TrainingDays[time.AddDays(1)].IsConflict);
        }

        [TestMethod]
        public void SimpleRetrieveWithModification_DifferentVersion_DifferentContent()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(createDay(time.AddDays(1)));
            retrieved.Add(createDay(time.AddDays(3)));
            retrieved.Add(createDay(time.AddDays(5)));
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved, currentHolder);

            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;

            //simulate modification of this entry on the server
            retrieved[0] = createDay(time.AddDays(1), 2);
            retrieved[0].Objects[0].Comment = "test";
            var res = manager.RetrievedDays(time, 1, retrieved, currentHolder);

            Assert.IsFalse(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
            Assert.AreEqual(null, currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
            Assert.AreEqual(1, currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
            Assert.AreEqual(true, currentHolder.TrainingDays[time.AddDays(1)].IsConflict);
        }

        [TestMethod]
        public void SimpleRetrieveWithModification_DifferentVersion_TheSameContentButDifferentInstanceId()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            var day = createDay(time.AddDays(1));
            day.Objects[0].InstanceId = Guid.NewGuid();
            retrieved.Add(day);
            retrieved.Add(createDay(time.AddDays(3)));
            retrieved.Add(createDay(time.AddDays(5)));
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved, currentHolder);

            currentHolder.TrainingDays[time.AddDays(1)] = currentHolder.TrainingDays[time.AddDays(1)].Copy();
            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;
            currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].InstanceId = Guid.NewGuid();
            var res = manager.RetrievedDays(time, 1, retrieved, currentHolder);

            Assert.IsTrue(res);
            Assert.AreEqual(retrieved.Count, currentHolder.TrainingDays.Count);
            Assert.AreEqual(1, currentHolder.RetrievedMonths.Count);
            Assert.AreEqual(time, currentHolder.RetrievedMonths[0]);
            Assert.AreEqual(null, currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
            Assert.AreEqual(1, currentHolder.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
            Assert.AreEqual(false, currentHolder.TrainingDays[time.AddDays(1)].IsConflict);
        }

        [TestMethod]
        public void SimpleClear()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved, currentHolder);
            manager.ClearTrainingDays();
            Assert.AreEqual(0, currentHolder.TrainingDays.Count);
            Assert.AreEqual(0, currentHolder.RetrievedMonths.Count);
        }

        [TestMethod]
        public void ClearWithModifiedItems()
        {
            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
            DateTime time = DateTime.UtcNow.MonthDate();
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
            var state = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            OfflineModeManager manager = new OfflineModeManager(state, currentHolder.User.GlobalId);
            manager.RetrievedDays(time, 1, retrieved, currentHolder);
            currentHolder.TrainingDays[time.AddDays(1)].IsModified = true;
            manager.ClearTrainingDays();
            Assert.AreEqual(1, currentHolder.TrainingDays.Count);
            Assert.AreEqual(0, currentHolder.RetrievedMonths.Count);
            Assert.IsNotNull(currentHolder.TrainingDays[time.AddDays(1)]);
        }

        #endregion

        #region Merge

        [TestMethod]
        public void Merge_ContentTheSame_day_and_entry_already_saved_IsModified_False()
        {
            bool? answer = null;
            
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;

            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
            
            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day){IsModified = false});

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            serverCopy.Objects[0].Version = 2;
            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate
                                                 {
                                                     throw new Exception("We shouldn't be here!");
                                                 });
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual(null, answer);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_ContentTheSame_day_and_entry_already_saved_IsModified_True()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;

            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day) { IsModified = true });

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            serverCopy.Objects[0].Version = 2;
            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate
            {
                throw new Exception("We shouldn't be here!");
            });
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual(null, answer);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_ContentChanged_day_and_entry_already_saved_answer_true()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day){IsModified = true});

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            serverCopy.Objects[0].Version = 2;
            serverCopy.Objects[0].Comment = "ggggg";
            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate
                                                           {
                                                               answer = true;
                                                               return answer.Value;
                                                           });
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual("ggggg", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual(true, answer);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_ContentChanged_day_and_entry_already_saved_but_still_new_on_client()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day) { IsModified = true });

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            serverCopy.Objects[0].Version = 2;
            serverCopy.Objects[0].GlobalId = Guid.NewGuid();
            serverCopy.GlobalId = Guid.NewGuid();
            serverCopy.Objects[0].Comment = "ggggg";
            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate(ModificationType m)
            {
                answer = false;
                return answer.Value;
            });
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects.Count);
            Assert.AreEqual(0, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects[1].Version);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual("ggggg", state.TrainingDay.TrainingDay.Objects[1].Comment);
            Assert.AreEqual(0, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);

            Assert.AreEqual(Guid.Empty, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.GlobalId);
            Assert.AreEqual(state.TrainingDay.TrainingDay.GlobalId, serverCopy.GlobalId);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_ContentChanged_day_and_entry_already_saved_answer_false()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day){IsModified = true});

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            serverCopy.Objects[0].Version = 2;
            serverCopy.Objects[0].Comment = "ggggg";
            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate(ModificationType m)
                                                           {
                Assert.AreEqual(ModificationType.EntryModified, m);
                answer = false;
                return answer.Value;
            });
            Assert.AreEqual(2, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual(false, answer);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        //[TestMethod]
        //public void Merge_entry_exists_on_server_but_not_on_client_answer_true()
        //{
        //    bool? answer = null;
        //    var state = new ApplicationState();
        //    state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
        //    var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
        //    state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
        //    var days = currentHolder;
        //    state.CurrentBrowsingTrainingDays = days;
        //    DateTime time = DateTime.UtcNow.MonthDate();
        //    days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
        //    days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

        //    var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
        //    day.GlobalId = Guid.NewGuid();
        //    day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
        //    day.Objects[0].Version = 1;
        //    day.Objects[0].GlobalId = Guid.NewGuid();
        //    days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

        //    state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
        //    var serverCopy = day.Copy();
        //    //added new entry on the server
        //    serverCopy.Objects.Add(new SizeEntryDTO(){Version = 1,GlobalId = Guid.NewGuid(),Comment = "size"});

        //    OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
        //    manager.MergeNew(serverCopy, state, false, delegate(ModificationType m)
        //    {
        //        Assert.AreEqual(ModificationType.EntryOnServerButNotOnClient,m);
        //        answer = true;
        //        return answer.Value;
        //    });
        //    Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[0].Version);
        //    Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[1].Version);
        //    Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
        //    Assert.AreEqual("size", state.TrainingDay.TrainingDay.Objects[1].Comment);
        //    Assert.AreEqual(true, answer);
        //    Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
        //    Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
        //    //should be a different references to the same object (copy)
        //    Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

        //    Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
        //    Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        //}

        //[TestMethod]
        //public void Merge_entry_exists_on_server_but_not_on_client_answer_false()
        //{
        //    bool? answer = null;
        //    var state = new ApplicationState();
        //    state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
        //    var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
        //    state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
        //    var days = currentHolder;
        //    state.CurrentBrowsingTrainingDays = days;
        //    DateTime time = DateTime.UtcNow.MonthDate();
        //    days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
        //    days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

        //    var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
        //    day.GlobalId = Guid.NewGuid();
        //    day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
        //    day.Objects[0].Version = 1;
        //    day.Objects[0].GlobalId = Guid.NewGuid();
        //    days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

        //    state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
        //    var serverCopy = day.Copy();
        //    //added new entry on the server
        //    serverCopy.Objects.Add(new SizeEntryDTO() { Version = 1, GlobalId = Guid.NewGuid(), Comment = "size" });

        //    OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
        //    manager.MergeNew(serverCopy, state, false, delegate(ModificationType m)
        //    {
        //        Assert.AreEqual(ModificationType.EntryOnServerButNotOnClient, m);
        //        answer = false;
        //        return answer.Value;
        //    });
        //    Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[0].Version);
        //    Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects.Count);
        //    Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);

        //    Assert.AreEqual(days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].GlobalId, state.TrainingDay.TrainingDay.Objects[0].GlobalId);
        //    Assert.AreEqual(days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.GlobalId, state.TrainingDay.TrainingDay.GlobalId);

        //    Assert.AreEqual(false, answer);
        //    Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
        //    Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
        //    //should be a different references to the same object (copy)
        //    Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

        //    Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
        //    Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        //}

        [TestMethod]
        public void Merge_added_new_entry_on_client_day_and_old_entry_already_saved()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            //added new entry on the client
            state.TrainingDay.TrainingDay.Objects.Add(new SizeEntryDTO() { Version = 0, Comment = "size" });

            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate
            {
                throw new Exception("We shouldn't be here!");
            });
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual(0, state.TrainingDay.TrainingDay.Objects[1].Version);
            Assert.AreEqual(Guid.Empty, state.TrainingDay.TrainingDay.Objects[1].GlobalId);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual("size", state.TrainingDay.TrainingDay.Objects[1].Comment);
            Assert.AreEqual(null, answer);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_removed_entry_on_server_day_and_old_entry_already_saved()
        {
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            //on the client we have one entry more (removed entry on server)
            state.TrainingDay.TrainingDay.Objects.Add(new SizeEntryDTO() { Version = 1, GlobalId = Guid.NewGuid(), Comment = "size" });

            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate
            {
                throw new Exception("We shouldn't be here!");
            });
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects.Count);
            Assert.AreEqual(day.Objects[0].GlobalId, state.TrainingDay.TrainingDay.Objects[0].GlobalId);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);

            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_removed_trainingday_on_server_day_and_old_entry_still_on_the_client()
        {
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();

            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(null, state, false, delegate
            {
                throw new Exception("We shouldn't be here!");
            });
            Assert.AreEqual(0, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual(Guid.Empty, state.TrainingDay.TrainingDay.Objects[0].GlobalId);
            Assert.AreEqual(Guid.Empty, state.TrainingDay.TrainingDay.GlobalId);
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects.Count);
            Assert.AreNotEqual(day.Objects[0].GlobalId, state.TrainingDay.TrainingDay.Objects[0].GlobalId);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);

            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        [TestMethod]
        public void Merge_entry_exists_on_client_but_not_on_server()
        {
            bool? answer = null;
            var state = new ApplicationState();
            state.MyDays = new Dictionary<CacheKey, TrainingDaysHolder>();
            var currentHolder = new TrainingDaysHolder(new UserSearchDTO() { GlobalId = Guid.NewGuid() });
            state.MyDays.Add(new CacheKey() { ProfileId = currentHolder.User.GlobalId }, currentHolder);
            var days = currentHolder;
            state.CurrentBrowsingTrainingDays = days;
            DateTime time = DateTime.UtcNow.MonthDate();
            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));

            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
            day.GlobalId = Guid.NewGuid();
            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
            day.Objects[0].Version = 1;
            day.Objects[0].GlobalId = Guid.NewGuid();
            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

            state.TrainingDay = days.TrainingDays[time.AddDays(3)].Copy();
            var serverCopy = day.Copy();
            //entry on the client
            state.TrainingDay.TrainingDay.Objects.Add(new SizeEntryDTO() { Version = 1, GlobalId = Guid.NewGuid(), Comment = "size" });

            OfflineModeManager manager = new OfflineModeManager(state.MyDays, currentHolder.User.GlobalId);
            manager.MergeNew(serverCopy, state, false, delegate(ModificationType m)
            {
                throw new Exception("Shouldn't be here ");
            });
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, state.TrainingDay.TrainingDay.Objects.Count);
            Assert.AreEqual("Test", state.TrainingDay.TrainingDay.Objects[0].Comment);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay.Objects.Count);
            //should be a different references to the same object (copy)
            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].TrainingDay);

            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsModified);
            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDay.TrainingDate].IsConflict);
        }

        #endregion
    }
}
