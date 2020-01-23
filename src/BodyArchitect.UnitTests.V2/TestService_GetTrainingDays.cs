using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using Privacy = BodyArchitect.Model.Privacy;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetTrainingDays:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();
        List<TrainingDay> trainingDays = new List<TrainingDay>();
        List<TrainingDay> customersTrainingDays = new List<TrainingDay>();
        private Guid key = Guid.NewGuid();
        private APIKey apiKey;

        public override void BuildDatabase()
        {
            profiles.Clear();
            trainingDays.Clear();
            customers.Clear();
            customersTrainingDays.Clear();
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

                var myGym = CreateMyPlace("gym", profiles[0]);

                var customer = CreateCustomer("Cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust3", profiles[1]);
                customers.Add(customer);

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
                var strength = new StrengthTrainingEntry();
                strength.MyPlace = myGym;
                day.AddEntry(strength);
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

                day = new TrainingDay(DateTime.Now.AddDays(2));
                day.Profile = profiles[0];
                day.Customer = customers[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 153;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                customersTrainingDays.Add(day);

                day = new TrainingDay(DateTime.Now.AddDays(3));
                day.Profile = profiles[0];
                day.Customer = customers[0];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 153;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                customersTrainingDays.Add(day);

                day = new TrainingDay(DateTime.Now.AddDays(2));
                day.Profile = profiles[0];
                day.Customer = customers[1];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 153;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                customersTrainingDays.Add(day);

                day = new TrainingDay(DateTime.Now.AddDays(2));
                day.Profile = profiles[1];
                day.Customer = customers[2];
                sizeEntry = new SizeEntry();
                sizeEntry.Wymiary = new Wymiary();
                sizeEntry.Wymiary.Height = 153;
                day.AddEntry(sizeEntry);
                Session.Save(day);
                customersTrainingDays.Add(day);

                apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        #region GetTrainingDays

        [Test]
        public void GetApplicationName()
        {
            LoginData loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[0].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[0]);
            var profile1 = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.StartDate = searchCriteria.EndDate = trainingDays[0].TrainingDate;
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     Service.Configuration.CurrentApiKey = key;
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(apiKey.ApplicationName, days.Items[0].Objects.ElementAt(0).ApplicationName);
        }

        [Test]
        public void GetApplicationName_ManyItems()
        {
            var apiKey = new APIKey();
            apiKey.ApiKey = Guid.NewGuid();
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);
            LoginData loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[0].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[0]);

            apiKey = new APIKey();
            apiKey.ApiKey = Guid.NewGuid();
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);
            loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[1].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[1]);

            apiKey = new APIKey();
            apiKey.ApiKey = Guid.NewGuid();
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);
            loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[2].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[2]);

            apiKey = new APIKey();
            apiKey.ApiKey = Guid.NewGuid();
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);
            loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[3].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[3]);

            apiKey = new APIKey();
            apiKey.ApiKey = Guid.NewGuid();
            apiKey.ApplicationName = "UnitTest";
            apiKey.EMail = "mail@mail.com";
            apiKey.RegisterDateTime = DateTime.UtcNow;
            insertToDatabase(apiKey);
            loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            trainingDays[4].Objects.ElementAt(0).LoginData = loginData;
            insertToDatabase(trainingDays[4]);
            var profile1 = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.Configuration.CurrentApiKey = key;
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(apiKey.ApplicationName, days.Items[0].Objects.ElementAt(0).ApplicationName);
        }

        [Test]
        public void TestGetTrainingDays_Public()
        {
            setPrivacy(Privacy.Public);

            var profile1 = (ProfileDTO)profiles[1].Tag;
            var profile2 = (ProfileDTO)profiles[2].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria=new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {

                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo .AllElementsPageSize});
            });
            Assert.AreEqual(5,days.AllItemsCount);
            Assert.AreEqual(7, getEntriesCount(days));

            data = CreateNewSession(profile2, ClientInformation);
            searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
            });
            Assert.AreEqual(5, days.AllItemsCount);
            Assert.AreEqual(7, getEntriesCount(days));

        }

        private void setPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }


        #endregion
        
        #region GetTrainingDay

        [Test]
        public void TestGetTrainingDay_Bug_ReturnOnlyOneEntry()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Current;
            op.WorkoutDateTime = DateTime.Now.AddDays(1);
            TrainingDayDTO day = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(2,day.Objects.Count);
        }

        [Test]
        public void TestGetTrainingDay_Current_NotExists()
        {
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Current;
            op.UserId = profile1.GlobalId;
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Current;
            op.UserId = profile1.GlobalId;
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Next;
            op.UserId = profile1.GlobalId;
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Previous;
            op.UserId = profile1.GlobalId;
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.First;
            op.UserId = profile1.GlobalId;
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
            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDayGetOperation op = new WorkoutDayGetOperation();
            op.Operation = GetOperation.Last;
            op.UserId = profile1.GlobalId;
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
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count - 1].TrainingDate, day.TrainingDate);

            data = CreateNewSession(profile2, ClientInformation);
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
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);

            data = CreateNewSession(profile2, ClientInformation);
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
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[0].TrainingDate, day.TrainingDate);

            data = CreateNewSession(profile2, ClientInformation);
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
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.AreEqual(trainingDays[trainingDays.Count-1].TrainingDate, day.TrainingDate);

            data = CreateNewSession(profile2, ClientInformation);
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
            op.UserId = profiles[0].GlobalId;
            op.WorkoutDateTime = trainingDays[2].TrainingDate;
            TrainingDayDTO day = null;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
            });
            Assert.IsNull(day);

            data = CreateNewSession(profile2, ClientInformation);
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
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            //create some training day entries
            var day = new TrainingDay(DateTime.Now.AddDays(-2));
            day.Profile = profiles[0];
            var sizeEntry = new StrengthTrainingEntry();
            sizeEntry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(sizeEntry);
            var strengthItem = new StrengthTrainingItem();
            strengthItem.Exercise = exercise;
            sizeEntry.AddEntry(strengthItem);
            var set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            strengthItem = new StrengthTrainingItem();
            strengthItem.Exercise = exercise;
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
            sizeEntry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(sizeEntry);
            strengthItem = new StrengthTrainingItem();
            strengthItem.Exercise = exercise;
            sizeEntry.AddEntry(strengthItem);
            set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            strengthItem = new StrengthTrainingItem();
            strengthItem.Exercise = exercise;
            sizeEntry.AddEntry(strengthItem);
            set = new Serie();
            strengthItem.AddSerie(set);
            set = new Serie();
            strengthItem.AddSerie(set);
            Session.Save(day);
            trainingDays.Add(day);

            SessionData data = CreateNewSession(profile1, ClientInformation);
            WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
            searchCriteria.UserId = profiles[0].GlobalId;
            PagedResult<TrainingDayDTO> days = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                days = Service.GetTrainingDays(data.Token, searchCriteria,
                                               new PartialRetrievingInfo()
                                               {
                                                   PageSize = PartialRetrievingInfo.AllElementsPageSize
                                               });
            });
            foreach (var item in ((StrengthTrainingEntryDTO)days.Items[5].Objects.ElementAt(0)).Entries)
            {
                Assert.AreEqual(days.Items[5].Objects.ElementAt(0), item.StrengthTrainingEntry);
                foreach (var tSet in item.Series)
                {
                    Assert.AreEqual(item, tSet.StrengthTrainingItem);
                }
            }

            foreach (var item in ((StrengthTrainingEntryDTO)days.Items[6].Objects.ElementAt(0)).Entries)
            {
                Assert.AreEqual(days.Items[6].Objects.ElementAt(0), item.StrengthTrainingEntry);
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

           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
           searchCriteria.UserId = profiles[0].GlobalId;
           PagedResult<TrainingDayDTO> days = null;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                {
                                    days = Service.GetTrainingDays(data.Token, searchCriteria,
                                                                   new PartialRetrievingInfo()
                                                                       {
                                                                           PageSize =PartialRetrievingInfo.AllElementsPageSize
                                                                       });
                                });
           Assert.AreEqual(days.Items[0], days.Items[0].Objects.ElementAt(0).TrainingDay);
           Assert.AreEqual(days.Items[1], days.Items[1].Objects.ElementAt(0).TrainingDay);
       }

       #endregion

       #region For customer

       [Test]
       public void ForCustomer_GetTrainingDays()
       {
           var profile1 = (ProfileDTO)profiles[0].Tag;

           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
           searchCriteria.UserId = profiles[0].GlobalId;
           searchCriteria.CustomerId = customers[0].GlobalId;
           PagedResult<TrainingDayDTO> days = null;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
           {
               days = Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
           });
           Assert.AreEqual(2, days.AllItemsCount);
           Assert.AreEqual(customersTrainingDays[0].GlobalId, days.Items[0].GlobalId);
           Assert.AreEqual(customersTrainingDays[1].GlobalId, days.Items[1].GlobalId);
       }

       [Test]
       [ExpectedException(typeof(CrossProfileOperationException))]
       public void ForCustomer_GetTrainingDays_CustomerFromAnotherProfile()
       {
           var profile1 = (ProfileDTO)profiles[0].Tag;

           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
           searchCriteria.UserId = profiles[0].GlobalId;
           searchCriteria.CustomerId = customers[2].GlobalId;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
           {
               Service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
           });
       }

       [Test]
       [ExpectedException(typeof(CrossProfileOperationException))]
       public void ForCustomer_GetTrainingDays_CustomerFromAnotherProfile_SecurityBug()
       {
           var profile1 = (ProfileDTO)profiles[0].Tag;

           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDaysSearchCriteria searchCriteria = new WorkoutDaysSearchCriteria();
           searchCriteria.UserId = profiles[1].GlobalId;
           searchCriteria.CustomerId = customers[2].GlobalId;
           RunServiceMethod(delegate(InternalBodyArchitectService service)
           {
               service.GetTrainingDays(data.Token, searchCriteria, new PartialRetrievingInfo() { PageSize = PartialRetrievingInfo.AllElementsPageSize });
           });
       }

       [Test]
       public void ForCustomer_GetTrainingDay_Current()
       {
           var profile1 = (ProfileDTO)profiles[0].Tag;
           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDayGetOperation op = new WorkoutDayGetOperation();
           op.Operation = GetOperation.Current;
           op.UserId = profile1.GlobalId;
           op.CustomerId = customers[0].GlobalId;
           op.WorkoutDateTime = customersTrainingDays[0].TrainingDate;
           TrainingDayDTO day = null;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
           {
               day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
           });
           Assert.AreEqual(customersTrainingDays[0].TrainingDate, day.TrainingDate);
       }

       [Test]
       public void ForCustomer_GetTrainingDay_Next()
       {
           var profile1 = (ProfileDTO)profiles[0].Tag;
           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDayGetOperation op = new WorkoutDayGetOperation();
           op.Operation = GetOperation.Next;
           op.UserId = profile1.GlobalId;
           op.CustomerId = customers[0].GlobalId;
           op.WorkoutDateTime = customersTrainingDays[0].TrainingDate;
           TrainingDayDTO day = null;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
           {
               day = Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
           });
           Assert.AreEqual(customersTrainingDays[1].TrainingDate, day.TrainingDate);
       }

       [Test]
       [ExpectedException(typeof(CrossProfileOperationException))]
       public void ForCustomer_GetTrainingDay_Current_AnotherProfile()
       {
           setPrivacy(Privacy.Public);
           var profile1 = (ProfileDTO)profiles[1].Tag;
           SessionData data = CreateNewSession(profile1, ClientInformation);
           WorkoutDayGetOperation op = new WorkoutDayGetOperation();
           op.Operation = GetOperation.Current;
           op.UserId = profiles[0].GlobalId;
           op.CustomerId = customers[0].GlobalId;
           op.WorkoutDateTime = customersTrainingDays[0].TrainingDate;
           RunServiceMethod(delegate(InternalBodyArchitectService Service)
           {
               Service.GetTrainingDay(data.Token, op, new RetrievingInfo());
           });
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
