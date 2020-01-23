//using System;
//using System.Collections.Generic;
//using BodyArchitect.Service.Client.WP7;
//using BodyArchitect.Service.Client.WP7.ModelExtensions;
//using BodyArchitect.Service.V2.Model;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace BodyArchitect.UnitTests.WP7
//{

//    [TestClass]
//    public class OfflineModeManagerTest
//    {
//        [TestMethod]
//        public void SimpleRetrieve()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            bool res = manager.RetrievedDays(time, 1, retrieved);
//            Assert.IsTrue(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//        }

//        [TestMethod]
//        public void SimpleRetrieveWithModification_ChangedTrainingDay()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);
//            state.TrainingDays[time.AddDays(1)] = state.TrainingDays[time.AddDays(1)].Copy();
//            state.TrainingDays[time.AddDays(1)].IsModified = true;
//            state.TrainingDays[time.AddDays(1)].TrainingDay.Comment = "test";

//            var res = manager.RetrievedDays(time, 1, retrieved);

//            Assert.IsFalse(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//            Assert.AreEqual("test", state.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
//            Assert.AreEqual(true, state.TrainingDays[time.AddDays(1)].IsConflict);
//        }

//        TrainingDayDTO createDay(DateTime date,int version=1)
//        {
//            var day = new TrainingDayDTO() {TrainingDate = date.Date};
//            SizeEntryDTO size = new SizeEntryDTO();
//            size.Version = version;
//            day.Objects.Add(size);
//            size.TrainingDay = day;
//            return day;
//        }

//        [TestMethod]
//        public void SimpleRetrieveWithModification_ChangedEntryObject()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(createDay(time.AddDays(1)));
//            retrieved.Add(createDay(time.AddDays(3)));
//            retrieved.Add(createDay(time.AddDays(5)));
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);
//            state.TrainingDays[time.AddDays(1)] = state.TrainingDays[time.AddDays(1)].Copy();
//            state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Comment = "test";
//            state.TrainingDays[time.AddDays(1)].IsModified = true;

//            var res = manager.RetrievedDays(time, 1, retrieved);

//            Assert.IsFalse(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//            Assert.AreEqual("test", state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Comment);
//            Assert.AreEqual(true, state.TrainingDays[time.AddDays(1)].IsConflict);
//        }

//        [TestMethod]
//        public void SimpleRetrieveWithModification_DifferentVersion_ButContentTheSame()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(createDay(time.AddDays(1)));
//            retrieved.Add(createDay(time.AddDays(3)));
//            retrieved.Add(createDay(time.AddDays(5)));
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);

//            state.TrainingDays[time.AddDays(1)].IsModified = true;

//            //simulate modification of this entry on the server
//            retrieved[0] = createDay(time.AddDays(1),2);
//            var res = manager.RetrievedDays(time, 1, retrieved);

//            Assert.IsTrue(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//            Assert.AreEqual(2, state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
//            Assert.AreEqual(false, state.TrainingDays[time.AddDays(1)].IsConflict);
//        }

//        [TestMethod]
//        public void SimpleRetrieveWithModification_DifferentVersion_DifferentContent()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(createDay(time.AddDays(1)));
//            retrieved.Add(createDay(time.AddDays(3)));
//            retrieved.Add(createDay(time.AddDays(5)));
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);

//            state.TrainingDays[time.AddDays(1)].IsModified = true;

//            //simulate modification of this entry on the server
//            retrieved[0] = createDay(time.AddDays(1), 2);
//            retrieved[0].Objects[0].Comment = "test";
//            var res = manager.RetrievedDays(time, 1, retrieved);

//            Assert.IsFalse(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//            Assert.AreEqual(null, state.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
//            Assert.AreEqual(1, state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
//            Assert.AreEqual(true, state.TrainingDays[time.AddDays(1)].IsConflict);
//        }

//        [TestMethod]
//        public void SimpleRetrieveWithModification_DifferentVersion_TheSameContentButDifferentInstanceId()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            var day = createDay(time.AddDays(1));
//            day.Objects[0].InstanceId = Guid.NewGuid();
//            retrieved.Add(day);
//            retrieved.Add(createDay(time.AddDays(3)));
//            retrieved.Add(createDay(time.AddDays(5)));
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);

//            state.TrainingDays[time.AddDays(1)] = state.TrainingDays[time.AddDays(1)].Copy();
//            state.TrainingDays[time.AddDays(1)].IsModified = true;
//            state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].InstanceId = Guid.NewGuid();
//            var res = manager.RetrievedDays(time, 1, retrieved);

//            Assert.IsTrue(res);
//            Assert.AreEqual(retrieved.Count, state.TrainingDays.Count);
//            Assert.AreEqual(1, state.RetrievedMonths.Count);
//            Assert.AreEqual(time, state.RetrievedMonths[0]);
//            Assert.AreEqual(null, state.TrainingDays[time.AddDays(1)].TrainingDay.Comment);
//            Assert.AreEqual(1, state.TrainingDays[time.AddDays(1)].TrainingDay.Objects[0].Version);
//            Assert.AreEqual(false, state.TrainingDays[time.AddDays(1)].IsConflict);
//        }

//        [TestMethod]
//        public void SimpleClear()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);
//            manager.ClearTrainingDays();
//            Assert.AreEqual(0, state.TrainingDays.Count);
//            Assert.AreEqual(0, state.RetrievedMonths.Count);
//        }

//        [TestMethod]
//        public void ClearWithModifiedItems()
//        {
//            List<TrainingDayDTO> retrieved = new List<TrainingDayDTO>();
//            DateTime time = DateTime.UtcNow.MonthDate();
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(1) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(3) });
//            retrieved.Add(new TrainingDayDTO() { TrainingDate = time.AddDays(5) });
//            var state = new TrainingDaysHolder(new UserSearchDTO());
//            OfflineModeManager manager = new OfflineModeManager(state);
//            manager.RetrievedDays(time, 1, retrieved);
//            state.TrainingDays[time.AddDays(1)].IsModified = true;
//            manager.ClearTrainingDays();
//            Assert.AreEqual(1, state.TrainingDays.Count);
//            Assert.AreEqual(0, state.RetrievedMonths.Count);
//            Assert.IsNotNull(state.TrainingDays[time.AddDays(1)]);
//        }

//        #region Merge without local changes

//        [TestMethod]
//        public void Merge_ContentTheSame_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Objects[0].Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//                                                 {
//                                                     answer = true;
//                                                     return answer.Value;
//                                                 });
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(null, answer);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_ContentDifferent_AnswerTrue_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Objects[0].Comment = "another comment";
//            serverCopy.Objects[0].Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//                                            {
//                                                answer = true;
//                                                return answer.Value;
//                                            });
//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual("another comment", state.TrainingDay.Objects[0].Comment);
//            Assert.AreEqual("Test", days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Comment);

//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_ContentDifferent_AnswerFalse_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Objects[0].Comment = "another comment";
//            serverCopy.Objects[0].Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//                                        {
//                                            answer = false;
//                                            return answer.Value;
//                                        });
//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual("Test", state.TrainingDay.Objects[0].Comment);
//            Assert.AreEqual("Test", days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Comment);

//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_RemovedOnServer_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(null, state, false, delegate
//                                {
//                                    answer = true;
//                                    return answer.Value;
//                                });
//            Assert.AreEqual(null, answer);
//            Assert.AreEqual(0, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(Guid.Empty, state.TrainingDay.GlobalId);
//            Assert.AreEqual("Test", state.TrainingDay.Objects[0].Comment);
//            Assert.IsTrue(days.TrainingDays.ContainsKey(state.TrainingDay.TrainingDate));
//        }

//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerFalse_WithoutLocal_TrainingDay_ChangedAlso()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 2;

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });

//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual("local", state.TrainingDay.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerFalse_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 2;

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });

//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }


//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerTrue_WithoutLocal_TrainingDay_ChangedAlso()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 2;

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });

//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual("server", state.TrainingDay.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerTrue_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 2;

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });

//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }
//        /*
//        [TestMethod]
//        public void Merge_SuppleServer_GymSuppleLocal_AnswerFalse_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects.Add(new SuplementsEntryDTO() { Comment = "SuppleLocal" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Version = 2;
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });


//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });

//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual("local", state.TrainingDay.Comment);
//            Assert.AreEqual("SuppleLocal", state.TrainingDay.Supplements.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_SuppleServer_GymSuppleLocal_AnswerTrue_WithoutLocal()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects.Add(new SuplementsEntryDTO() { Comment = "SuppleLocal" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Version = 2;
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });


//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, false, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });

//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual("server", state.TrainingDay.Comment);
//            Assert.AreEqual("Supple", state.TrainingDay.Supplements.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }*/
//        #endregion

//        #region Merge with local changes
//        /*
//        [TestMethod]
//        public void Merge_ContentTheSame()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual(null, answer);
//            Assert.AreEqual(2, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_ContentDifferent_AnswerTrue()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Objects[0].Comment = "another comment";
//            serverCopy.Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });
//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual(2, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual("another comment", state.TrainingDay.Objects[0].Comment);
//            Assert.AreEqual("another comment", days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Comment);

//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_ContentDifferent_AnswerFalse()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = day.Copy();
//            serverCopy.Objects[0].Comment = "another comment";
//            serverCopy.Version = 2;
//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });
//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.AreEqual("Test", state.TrainingDay.Objects[0].Comment);
//            Assert.AreEqual("Test", days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Comment);

//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        public void Merge_RemovedOnServer()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Test" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(null, state, true, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });
//            Assert.AreEqual(null, answer);
//            Assert.AreEqual(0, state.TrainingDay.Version);
//            Assert.AreEqual(Guid.Empty, state.TrainingDay.GlobalId);
//            Assert.AreEqual("Test", state.TrainingDay.Objects[0].Comment);
//            Assert.IsFalse(days.TrainingDays.ContainsKey(state.TrainingDay.TrainingDate));
//        }

//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerFalse()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Version = 1;
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Version = 2;
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });


//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });

//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(2, state.TrainingDay.Version);
//            Assert.AreEqual("local", state.TrainingDay.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }
//        */

//        [TestMethod]
//        public void Merge_SuppleServer_GymLocal_AnswerTrue()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects[0].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = state.TrainingDay.Copy();
//            serverCopy.Objects[0].Version = 2;
//            serverCopy.Objects[0].Comment = "Supple";


//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });

//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(2, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(2, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            Assert.AreEqual("Supple", days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Comment);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }
        
//        [TestMethod]
//        public void Merge_SuppleServer_GymSuppleLocal_AnswerFalse()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects.Add(new SuplementsEntryDTO() { Comment = "SuppleLocal" });
//            day.Objects[0].Version = 1;
//            day.Objects[1].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));

//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 1;
//            serverCopy.Objects[0].GlobalId = Guid.NewGuid();

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = false;
//                return answer.Value;
//            });

//            Assert.AreEqual(false, answer);
//            Assert.AreEqual(1, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, state.TrainingDay.Objects[1].Version);
//            Assert.AreEqual("local", state.TrainingDay.Comment);
//            Assert.AreEqual("SuppleLocal", state.TrainingDay.Supplements.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[1].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }

//        [TestMethod]
//        //day from the server has removed Gym and SuppleLocal but has another Supple object
//        public void Merge_SuppleServer_GymSuppleLocal_AnswerTrue()
//        {
//            bool? answer = null;
//            var days = new TrainingDaysHolder(new UserSearchDTO());
//            var state = new ApplicationState();
//            state.MyDays = days;
//            state.CurrentBrowsingTrainingDays = days;
//            DateTime time = DateTime.UtcNow.MonthDate();
//            days.TrainingDays.Add(time.AddDays(1), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(1) }));
//            days.TrainingDays.Add(time.AddDays(2), new TrainingDayInfo(new TrainingDayDTO() { TrainingDate = time.AddDays(2) }));
//            var day = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            day.Comment = "local";
//            day.Objects.Add(new StrengthTrainingEntryDTO() { Comment = "Gym" });
//            day.Objects.Add(new SuplementsEntryDTO() { Comment = "SuppleLocal" });
//            day.Objects[0].Version = 1;
//            day.Objects[1].Version = 1;
//            days.TrainingDays.Add(time.AddDays(3), new TrainingDayInfo(day));
//            state.TrainingDay = days.TrainingDays[time.AddDays(3)].TrainingDay.Copy();
//            var serverCopy = new TrainingDayDTO() { TrainingDate = time.AddDays(3) };
//            serverCopy.Comment = "server";
//            serverCopy.Objects.Add(new SuplementsEntryDTO() { Comment = "Supple" });
//            serverCopy.Objects[0].Version = 1;
//            serverCopy.Objects[0].GlobalId = Guid.NewGuid();

//            OfflineModeManager manager = new OfflineModeManager(days);
//            manager.Merge(serverCopy, state, true, delegate
//            {
//                answer = true;
//                return answer.Value;
//            });

//            Assert.AreEqual(true, answer);
//            Assert.AreEqual(1, state.TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, state.TrainingDay.Objects[1].Version);
//            Assert.AreEqual("server", state.TrainingDay.Comment);
//            Assert.AreEqual("Supple", state.TrainingDay.Supplements.Comment);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[0].Version);
//            Assert.AreEqual(1, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay.Objects[1].Version);
//            //should be a different references to the same object (copy)
//            Assert.AreNotEqual(state.TrainingDay, days.TrainingDays[state.TrainingDay.TrainingDate].TrainingDay);

//            Assert.IsNotNull(state.TrainingDay.StrengthWorkout);
//            Assert.IsNotNull(state.TrainingDay.Supplements);
//            Assert.AreEqual(true, days.TrainingDays[state.TrainingDay.TrainingDate].IsModified);
//            Assert.AreEqual(false, days.TrainingDays[state.TrainingDay.TrainingDate].IsConflict);
//        }
//        #endregion
//    }
//}
