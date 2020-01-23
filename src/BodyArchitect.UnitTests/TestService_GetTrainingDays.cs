using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NHibernate;
using NUnit.Framework;
using Privacy = BodyArchitect.Model.Privacy;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_GetTrainingDays:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<TrainingDay> trainingDays = new List<TrainingDay>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            trainingDays.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile=CreateProfile(Session, "Profile1");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3");
                profiles.Add(profile);

                //set friendship
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                //create some training day entries
                var day = new TrainingDay(DateTime.Now.AddDays(-2));
                day.Profile = profiles[0];
                var sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 213;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                trainingDays.Add(day);

                day = new TrainingDay(DateTime.Now.AddDays(-1));
                day.Profile = profiles[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 113;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                trainingDays.Add(day);
                
                day = new TrainingDay(DateTime.Now);
                day.Profile = profiles[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 100;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                trainingDays.Add(day);
                
                day = new TrainingDay(DateTime.Now.AddDays(1));
                day.Profile = profiles[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 100;
                day.AddEntry(sizeEntry);
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Weight = 60;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                trainingDays.Add(day);

                day = new TrainingDay(DateTime.Now.AddDays(2));
                day.Profile = profiles[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 153;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                trainingDays.Add(day);
                tx.Commit();
            }
        }

        #region GetTrainingDays
        
        [Test]
        public void TestGetTrainingDays_Public()
        {
            setPrivacy(Privacy.Public);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria=new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {

                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo .AllElementsPageSize});
            });
            Assert.AreEqual(5,days.AllItemsCount);
            Assert.AreEqual(6, getEntriesCount(days));

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(5, days.AllItemsCount);
            Assert.AreEqual(6, getEntriesCount(days));

        }

        private void setPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }

        [Test]
        public void TestGetTrainingDays_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);
            PagedResult<TrainingDayDTO> days = null;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(5, days.AllItemsCount);
            Assert.AreEqual(6, getEntriesCount(days));

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

        }

        [Test]
        public void TestGetTrainingDays_Private()
        {
            setPrivacy(Privacy.Private);

            PagedResult<TrainingDayDTO> days = null;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(0, days.AllItemsCount);

        }

        #endregion
        
        #region GetTrainingDay

        [Test]
        public void TestGetTrainingDay_Current_NotExists()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Current;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = DateTime.Now.AddDays(-41);
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }

        [Test]
        public void TestGetTrainingDay_Current_Myself()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Current;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[2].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_Next_Myself()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Next;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[3].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_Previous_Myself()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Previous;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[1].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_First_Myself()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.First;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_Last_Myself()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profile1.Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count-1].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_Last_Public()
        {
            setPrivacy(Privacy.Public);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profiles[0].Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count - 1].TrainingDate, day.TrainingDate);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count - 1].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_First_Public()
        {
            setPrivacy(Privacy.Public);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.First;
            op.UserId = profiles[0].Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);
        }

        [Test]
        public void TestGetTrainingDay_First_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.First;
            op.UserId = profiles[0].Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }

        [Test]
        public void TestGetTrainingDay_Last_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profiles[0].Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count-1].TrainingDate, day.TrainingDate);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }

        [Test]
        public void TestGetTrainingDay_Current_Private()
        {
            setPrivacy(Privacy.Private);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;

            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profiles[0].Id;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);

            data = SecurityManager.CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);
        }
        #endregion

        #region Mappers

        [Test]
        public void TestBugWithCircuralReferences_StrengthTraining()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;

            //create some training day entries
            var day = new TrainingDay(DateTime.Now.AddDays(-2));
            day.Profile = profiles[0];
            var sizeEntry = new StrengthTrainingEntry();
            day.AddEntry(sizeEntry);
            var strengthItem = new StrengthTrainingItem();
            sizeEntry.AddEntry(strengthItem);
            var set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            strengthItem = new StrengthTrainingItem();
            sizeEntry.AddEntry(strengthItem);
            set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            Session.Save(day);
            trainingDays.Add(day);

            day = new TrainingDay(DateTime.Now.AddDays(-1));
            day.Profile = profiles[0];
            sizeEntry = new StrengthTrainingEntry();
            day.AddEntry(sizeEntry);
            strengthItem = new StrengthTrainingItem();
            sizeEntry.AddEntry(strengthItem);
            set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            strengthItem = new StrengthTrainingItem();
            sizeEntry.AddEntry(strengthItem);
            set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            Session.Save(day);
            trainingDays.Add(day);

            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].Id;
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria,
                                               new PartialRetrievingInfo()
                                               {
                                                   PageSize = PartialRetrievingInfo.AllElementsPageSize
                                               });
            });
            foreach (var item in ((StrengthTrainingEntryDTO)days.Items[5].Objects[0]).Entries)
            {
                Assert.AreEqual(days.Items[5].Objects[0], item.StrengthTrainingEntry);
                foreach (var tSet in item.Series)
                {
                    Assert.AreEqual(item, tSet.StrengthTrainingItem);
                }
            }

            foreach (var item in ((StrengthTrainingEntryDTO)days.Items[6].Objects[0]).Entries)
            {
                Assert.AreEqual(days.Items[6].Objects[0], item.StrengthTrainingEntry);
                foreach (var tSet in item.Series)
                {
                    Assert.AreEqual(item, tSet.StrengthTrainingItem);
                }
            }

        }


       [Test]
       public void TestBugWithCircuralReferences_EntryObject_TrainingDay()
       {
           var profile1 = (ProfileDTO) profiles[0].Tag;

           //create some training day entries
           var day = new TrainingDay(DateTime.Now.AddDays(-2));
           day.Profile = profiles[0];
           var sizeEntry = new SizeEntry();
           sizeEntry.Wymiary = new Wymiary();
           sizeEntry.Wymiary.Height = 213;
           day.AddEntry(sizeEntry);
           Session.Save(day);
           trainingDays.Add(day);

           day = new TrainingDay(DateTime.Now.AddDays(-1));
           day.Profile = profiles[0];
           sizeEntry = new SizeEntry();
           sizeEntry.Wymiary = new Wymiary();
           sizeEntry.Wymiary.Height = 113;
           day.AddEntry(sizeEntry);
           Session.Save(day);
           trainingDays.Add(day);

           SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
           WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
           searchCriteria.UserId = profiles[0].Id;
           PagedResult<TrainingDayDTO> days = null;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                {
                                    days = Service.GetTrainingDays(data.Token, searchCriteria,
                                                                   new PartialRetrievingInfo()
                                                                       {
                                                                           PageSize =PartialRetrievingInfo.AllElementsPageSize
                                                                       });
                                });
           Assert.AreEqual(days.Items[0], days.Items[0].Objects[0].TrainingDay);
           Assert.AreEqual(days.Items[1], days.Items[1].Objects[0].TrainingDay);
       }

       #endregion
        
        private int getEntriesCount(PagedResult<TrainingDayDTO> pack)
        {
            int count = 0;
            foreach (var day in pack.Items)
            {
                count += day.Objects.Count;
            }
            return count;
        }
    }
}
