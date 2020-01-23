using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Service.V2.Model.EntryObjectStatus;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseDoneWay = BodyArchitect.Service.V2.Model.ExerciseDoneWay;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Gender = BodyArchitect.Service.V2.Model.Gender;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using SetType = BodyArchitect.Service.V2.Model.SetType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveTrainingDay:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();
        private APIKey apiKey;
        private Guid key = Guid.NewGuid();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                customers.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);

                profiles[2].FavoriteUsers.Add(profiles[0]);

                var customer = CreateCustomer("Cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("Cust3", profiles[1]);
                customers.Add(customer);

                apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }
        /*
        #region A6W integrity

        [Test]
        public void TestA6W_BreakTraining_DateOfLastElement()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            TrainingDayDTO day3 = new TrainingDayDTO(DateTime.Now.AddDays(2));
            day3.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w2 = new A6WEntryDTO();
            a6w2.Day = A6WManager.Weeks[3];
            a6w2.Completed = true;
            a6w2.MyTraining = day1.Objects.ElementAt(0).MyTraining;
            a6w2.MyTraining.Name = "name";
            day3.AddEntry(a6w2);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day3 = Service.SaveTrainingDay(data.Token, day3);
            });

            ((A6WEntryDTO)day3.Objects.ElementAt(0)).MyTraining.Abort();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day3 = Service.SaveTrainingDay(data.Token, day3);
            });
            var myTrainingDb = Session.Get<MyTraining>(day3.Objects.ElementAt(0).MyTraining.GlobalId);
            Assert.AreEqual(day3.TrainingDate,myTrainingDb.EndDate.Value);
        }

        

        [Test]
        public void TestA6W_CreateANewTraining()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Completed = true;
            a6w.Day = A6WManager.Weeks[0];
            a6w.MyTraining=new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            
            var dbDay=Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == day.TrainingDate).SingleOrDefault();
            Assert.IsNotNull(dbDay);
            Assert.AreEqual(dbDay.Objects.Count, day.Objects.Count);
            Assert.AreEqual(profile.GlobalId, day.Objects.ElementAt(0).MyTraining.ProfileId);
        }

        [Test]
        public void TestA6W_CompleteTraining()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            MyTrainingDTO training = new MyTrainingDTO();

            foreach (A6WDay a6WDay in A6WManager.Weeks)
            {
                TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-44).AddDays(a6WDay.DayNumber));
                day.ProfileId = profile.GlobalId;
                A6WEntryDTO a6w = new A6WEntryDTO();
                a6w.Completed = true;
                a6w.MyTraining = training;
                a6w.Day = a6WDay;
                a6w.MyTraining.Name = "name";
                day.AddEntry(a6w);
                RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    if(a6w.Day.DayNumber==42)
                    {
                        training.Complete();
                    }
                    day=service.SaveTrainingDay(data.Token, day);
                    training = day.Objects.ElementAt(0).MyTraining;
                });
            }

            var dbTraining = Session.Get<MyTraining>(training.GlobalId);
            Assert.AreEqual(TrainingEnd.Complete,dbTraining.TrainingEnd);
            Assert.IsNotNull(dbTraining.EndDate);
        }

        [Test]
        public void TestA6W_TwoA6WEntries_ModifyFirst_Bug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            ((A6WEntryDTO)day.Objects.ElementAt(0)).Completed = false;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbA6w = Session.Get<A6WEntry>(day.Objects.ElementAt(0).Id);
            Assert.IsFalse(dbA6w.Completed);
        }

        [Test]
        public void TestA6W_StopTraining_Bug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            day1.Objects.ElementAt(0).MyTraining.Abort();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day1);
            });
            var dbA6w = Session.Get<A6WEntry>(day.Objects.ElementAt(0).Id);
            Assert.AreEqual(TrainingEnd.Break,dbA6w.MyTraining.TrainingEnd);
            Assert.IsNotNull(dbA6w.MyTraining.EndDate);
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void TestA6W_IntegrErr_AddBefore()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     day=Service.SaveTrainingDay(data.Token, day);
                                 });
            

            //create a new training day with a6w but before the preious one (inetgration error we should get)
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day1);
            });

           
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void TestA6W_IntegrErr_DeleteBefore()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1=Service.SaveTrainingDay(data.Token, day1);
            });
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.Id;

                Service.DeleteTrainingDay(data.Token, param);
            });

            

        }

        [Test]
        public void TestA6W_RemovingMyTrainingAutomatically()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1=Service.SaveTrainingDay(data.Token, day1);
            });

            var myTraining = Session.Get<MyTraining>(a6w1.MyTraining.GlobalId);
            Assert.IsNotNull(myTraining);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day1.Id;

                Service.DeleteTrainingDay(data.Token, param);
            });
            myTraining = Session.Get<MyTraining>(a6w1.MyTraining.GlobalId);
            Assert.IsNotNull(myTraining);

            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.Id;

                Service.DeleteTrainingDay(data.Token, param);
            });

            myTraining = Session.Get<MyTraining>(a6w1.MyTraining.GlobalId);
            Assert.IsNull(myTraining);

        }
        #endregion
        */
        #region Concurency

        [Test]
        public void TestCreateTwoTrainingDaysOnTheSameDateForDifferentProfile_BugFixing()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            data = CreateNewSession(profile1, ClientInformation);

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile1.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            var count=Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == DateTime.Now.Date).RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void TestCreateTwoTrainingDaysOnTheSameDate_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary=new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            Assert.Fail();
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void RemovedOneEntryOnTheServerButNotOnTheClient_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            var entry1 = new SizeEntryDTO();
            entry1.Wymiary = new WymiaryDTO();
            entry1.Wymiary.Height = 100;
            day.AddEntry(entry1);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var clone = day.StandardClone();
            day.Objects.RemoveAt(0);//remove one entry
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            //on the server we have new value but the client has still day with two entries and tries to save it
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, clone);
            });
        }

        [Test]
        [Ignore]
        //problemem jest to że nie wiemy czy usunięto ten wpis na kliencie i zapisujemy czyz innej aplikacji utorzylismy nowy wpis i powinnismy go zostawić
        public void EntryExistOnTheServerButNotOnTheClient()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            var entry1 = new SizeEntryDTO();
            entry1.Wymiary = new WymiaryDTO();
            entry1.Wymiary.Height = 100;
            day.AddEntry(entry1);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var clone = day.StandardClone();
            clone.Objects.RemoveAt(0);//remove one entry on the client

            //on the server we have new value but the client has still day with two entries and tries to save it
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, clone);
                clone = result.TrainingDay;
            });
            Assert.AreEqual(2,clone.Objects.Count);
        }
        #endregion

        #region Blog entries

        [Test]
        public void TestAddTrainingDayComment_SendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.AllowComments = true;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0]).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.IsNotNull(messages.Content);
            Assert.AreEqual(MessagePriority.System, messages.Priority);
        }

        [Test]
        public void TestAddTrainingDayComment_SendMessage_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.AllowComments = true;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            var oldHash = dbProfile.DataInfo.MessageHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.MessageHash, oldHash);
        }

        [Test]
        public void TestAddBlogComment_DisableSendMessage()
        {
            profiles[0].Settings.NotificationBlogCommentAdded = ProfileNotification.None;
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.AllowComments = true;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).SingleOrDefault();
            Assert.IsNull(messages);
        }

        [Test]
        public void TestAddBlogCommentByOwner_DontSendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] ).SingleOrDefault();
            Assert.IsNull(messages);

        }

        [Test]
        public void TestDeleteTrainingDayWithComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            data = CreateNewSession(profile, ClientInformation);
            comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg1";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbEntry = Session.QueryOver<TrainingDay>().SingleOrDefault();
            var dbComments = Session.QueryOver<TrainingDayComment>().List();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual(2, dbComments.Count);

            day.Objects.Remove(day.Objects.OfType<BlogEntryDTO>().Single());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                DeleteTrainingDayParam param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;
                Service.DeleteTrainingDay(data.Token, param);
            });
            dbEntry = Session.QueryOver<TrainingDay>().SingleOrDefault();
            dbComments = Session.QueryOver<TrainingDayComment>().List();
            Assert.IsNull(dbEntry);
            Assert.AreEqual(0, dbComments.Count);

        }
        
        [Test]
        public void TestDeleteBlogEntryWithComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);
            data = CreateNewSession(profile, ClientInformation);
            comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg1";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            var dbComments = Session.QueryOver<TrainingDayComment>().List();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual(2, dbComments.Count);

            //remove blogentry from training day
            day.Objects.Remove(day.Objects.OfType<BlogEntryDTO>().Single());
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            dbComments = Session.QueryOver<TrainingDayComment>().List();
            Assert.IsNull(dbEntry);
            Assert.AreEqual(2, dbComments.Count);
            
        }

        [Test]
        public void TestCreateBlogEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            var dbEntry=Session.QueryOver<BlogEntry>().SingleOrDefault();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual(blogEntry.Comment,dbEntry.Comment);

        }

        [Test]
        public void TestCreateBlogComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO) profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
            var dbComment = Session.QueryOver<TrainingDayComment>().SingleOrDefault();
            Assert.IsNotNull(dbDay);
            Assert.IsNotNull(dbComment);
            Assert.AreEqual(comment.Comment, dbComment.Comment);
            Assert.AreEqual(profile1.GlobalId, dbComment.Profile.GlobalId);
            Assert.AreEqual(dbComment.DateTime,dbDay.LastCommentDate);
        }

        [Test]
        public void TestCreateTwoBlogComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
            //sleep is to check LastCommentDateTime
            //Thread.Sleep(300);

            data = CreateNewSession(profile, ClientInformation);
            comment = new TrainingDayCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg1";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
            var dbComments = Session.QueryOver<TrainingDayComment>().List();
            Assert.IsNotNull(dbDay);
            Assert.AreEqual(2, dbComments.Count);
            Assert.AreEqual(comment.Comment, dbComments[1].Comment);
            Assert.AreEqual(profile.GlobalId, dbComments[1].Profile.GlobalId);
            Assert.AreEqual(dbComments[1].DateTime, dbDay.LastCommentDate);
        }

        [Test]
        public void TestEditBlogEntryWithComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });


            data = CreateNewSession(profile, ClientInformation);
            day.Objects.ElementAt(0).Comment = "newComment";
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            var dbDay = Session.QueryOver<TrainingDay>().SingleOrDefault();
            Assert.IsNotNull(dbDay);
            Assert.AreEqual("newComment", dbDay.Objects.ElementAt(0).Comment);
            Assert.AreEqual(1, dbDay.CommentsCount);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestCreateBlogComments_CrossProfileException()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = true;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateBlogCommentsInNotAllowedBlogEntry_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AllowComments = false;
            day.AddEntry(blogEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });
        }
        #endregion

        #region Strength workout

        [Test]
        public void Bug_MappingSets_NullStrengthTrainingitem()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbSet=Session.QueryOver<Serie>().List().First();
            Assert.IsNotNull(dbSet.StrengthTrainingItem);
            Assert.IsFalse(dbSet.IsIncorrect);
        }


        [Test]
        public void Save_EmptyTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            item.DoneWay = ExerciseDoneWay.Dumbbell;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            ((StrengthTrainingEntryDTO)day.Objects[0]).Entries.Clear();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            Assert.IsNull(day);
            Assert.AreEqual(0,Session.QueryOver<TrainingDay>().RowCount());
        }


        [Test]
        public void Cardio_CalculateWeightFromStartEndTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO();
            serie.StartTime = DateTime.Today;
            serie.EndTime = DateTime.Today.AddSeconds(10);
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(10, dbItem.Series.ElementAt(0).Weight.Value);
        }

        [Test]
        public void SetDoneWay_StrengthTrainingitem()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            item.DoneWay = ExerciseDoneWay.Dumbbell;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Assert.AreEqual(ExerciseDoneWay.Dumbbell,((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].DoneWay);
            var dbItem=Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.ExerciseDoneWay.Dumbbell, dbItem.DoneWay);
        }

        [Test]
        public void SeriesPosition_IsIncorrect_True()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Biceps, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x100");
            item.AddSerie(serie);
            serie = new SerieDTO("3x30");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 10).Single().IsIncorrect);
            Assert.IsTrue(dbItem.Series.Where(x => x.Weight == 100).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 30).Single().IsIncorrect);
        }

        [Test]
        public void SeriesPosition_IsIncorrect_SetByClient()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Biceps, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x100");
            item.AddSerie(serie);
            serie = new SerieDTO("3x30");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            ((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 100).Single().IsIncorrect = false;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 10).Single().IsIncorrect);
            Assert.IsTrue(dbItem.Series.Where(x => x.Weight == 100).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 30).Single().IsIncorrect);
        }

        [Test]
        public void SeriesPosition_IsIncorrect_WithNullRepetitions()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Biceps, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x10");
            item.AddSerie(serie);
            serie = new SerieDTO("x20");
            item.AddSerie(serie);
            serie = new SerieDTO("3x30");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 10).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 20).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 30).Single().IsIncorrect);
        }

        [Test]
        public void SeriesPosition_IsIncorrect_WithNullWeight()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Biceps, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x10");
            item.AddSerie(serie);
            serie = new SerieDTO("3x");
            item.AddSerie(serie);
            serie = new SerieDTO("3x30");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 10).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == null).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.Weight == 30).Single().IsIncorrect);
        }


        [Test]
        public void SeriesPosition_IsIncorrect_False()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x");
            item.AddSerie(serie);
            serie = new SerieDTO("2x");
            item.AddSerie(serie);
            serie = new SerieDTO("3x");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.IsFalse(dbItem.Series.Where(x => x.RepetitionNumber == 1).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.RepetitionNumber == 2).Single().IsIncorrect);
            Assert.IsFalse(dbItem.Series.Where(x => x.RepetitionNumber == 3).Single().IsIncorrect);
        }

        [Test]
        public void FixAutoMapperConfiguration()
        {//after upgrade AutoMapper then we get problems with this: 
            //.ForMember(dst => dst.MyTraining,_ =>_.ResolveUsing<LoadingGuidEntityResolver<MyTraining>>().FromMember(src => src.MyTraining.GlobalId));
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x");
            item.AddSerie(serie);
            serie = new SerieDTO("2x");
            item.AddSerie(serie);
            serie = new SerieDTO("3x");
            item.AddSerie(serie);
            SizeEntryDTO size=new SizeEntryDTO();
            day.AddEntry(size);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(0, dbItem.Series.Where(x => x.RepetitionNumber == 1).Single().Position);
            Assert.AreEqual(1, dbItem.Series.Where(x => x.RepetitionNumber == 2).Single().Position);
            Assert.AreEqual(2, dbItem.Series.Where(x => x.RepetitionNumber == 3).Single().Position);

            Assert.AreEqual(1, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series[0].RepetitionNumber);
            Assert.AreEqual(2, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series[1].RepetitionNumber);
            Assert.AreEqual(3, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series[2].RepetitionNumber);
        }

        [Test]
        public void SeriesPosition_StrengthTrainingitem()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("1x");
            item.AddSerie(serie);
            serie = new SerieDTO("2x");
            item.AddSerie(serie);
            serie = new SerieDTO("3x");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var dbItem=Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(0,dbItem.Series.Where(x=>x.RepetitionNumber==1).Single().Position);
            Assert.AreEqual(1, dbItem.Series.Where(x => x.RepetitionNumber == 2).Single().Position);
            Assert.AreEqual(2, dbItem.Series.Where(x => x.RepetitionNumber == 3).Single().Position);

            Assert.AreEqual(1,((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].Series[0].RepetitionNumber);
            Assert.AreEqual(2, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series[1].RepetitionNumber);
            Assert.AreEqual(3, ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series[2].RepetitionNumber);
        }

        [Test]
        public void SeriesPosition_RemovesMiddleSerie()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("1x");
            serie1.Position = 0;
            item.AddSerie(serie1);
            var serie2 = new Serie("2x");
            serie2.Position = 1;
            item.AddSerie(serie2);
            var serie3 = new Serie("3x");
            serie3.Position = 2;
            item.AddSerie(serie3);
            insertToDatabase(day);

            var dayDto = day.Map<TrainingDayDTO>();
            ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series.RemoveAt(1);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });

            var dbItem = Session.Get<StrengthTrainingItem>(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].GlobalId);
            Assert.AreEqual(0, dbItem.Series.Where(x => x.RepetitionNumber == 1).Single().Position);
            Assert.AreEqual(1, dbItem.Series.Where(x => x.RepetitionNumber == 3).Single().Position);

            Assert.AreEqual(1, ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[0].RepetitionNumber);
            Assert.AreEqual(3, ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series[1].RepetitionNumber);

        }
        #endregion

        #region Send messages to friends

        [Test]
        public void TestSaveTrainingDay_CreateEntry_DisableSendingMessage_Friends()
        {
            var friend = profiles[3];
            friend.Settings.NotificationFriendChangedCalendar = ProfileNotification.None;
            insertToDatabase(friend);

            var profile = (ProfileDTO)profiles[0].Tag;
            

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNull(message);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];
            friend.Settings.NotificationFriendChangedCalendar = ProfileNotification.Message;
            insertToDatabase(friend);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message=Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNotNull(message);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual(profile.GlobalId, message.Sender.GlobalId);
            Assert.IsNotNull(message.Content);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendEMailToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];
            friend.Settings.NotificationFriendChangedCalendar = ProfileNotification.Email;
            insertToDatabase(friend);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNull(message);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendMessageAndEMailToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];
            friend.Settings.NotificationFriendChangedCalendar =ProfileNotification.Email| ProfileNotification.Message;
            insertToDatabase(friend);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNotNull(message);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.IsNotNull( message.Content);
        }

        [Test]
        public void TestSaveTrainingDay_EditEntry_NoSendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == friend).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestSaveTrainingDay_AddNewEntry_SendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            SizeEntryDTO size=new SizeEntryDTO();
            size.Wymiary=new WymiaryDTO();
            size.Wymiary.Klatka = 100;
            day.AddEntry(size);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var test=Session.QueryOver<Message>().Where(x => x.Receiver == friend).List();
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == friend).RowCount();
            Assert.AreEqual(2, count);
        }
        #endregion

        #region Send messages to followers

        [Test]
        public void TestSaveTrainingDay_CreateEntry_DisableSendingMessage_Followers()
        {
            var follower = profiles[2];
            follower.Settings.NotificationFollowersChangedCalendar = ProfileNotification.None;
            insertToDatabase(follower);

            var profile = (ProfileDTO)profiles[0].Tag;


            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == follower).SingleOrDefault();
            Assert.IsNull(message);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendMessageToFollowers()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var follower = profiles[2];
            follower.Settings.NotificationFollowersChangedCalendar = ProfileNotification.Message;
            insertToDatabase(follower);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsFalse(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == follower).SingleOrDefault();
            Assert.IsNotNull(message);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual(profile.GlobalId, message.Sender.GlobalId);
            Assert.IsNotNull(message.Content);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendEMailToFollowers()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var follower = profiles[2];
            follower.Settings.NotificationFollowersChangedCalendar = ProfileNotification.Email;
            insertToDatabase(follower);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == follower).SingleOrDefault();
            Assert.IsNull(message);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendMessageAndEMailToFollowers()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var follower = profiles[2];
            follower.Settings.NotificationFollowersChangedCalendar = ProfileNotification.Email | ProfileNotification.Message;
            insertToDatabase(follower);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsTrue(((MockEmailService)Service.EMailService).EMailSent);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == follower).SingleOrDefault();
            Assert.IsNotNull(message);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.IsNotNull(message.Content);
        }

        [Test]
        public void TestSaveTrainingDay_EditEntry_NoSendMessageToFollowers()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var follower = profiles[2];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == follower).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestSaveTrainingDay_AddNewEntry_SendMessageToFollowers()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var follower = profiles[2];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            SizeEntryDTO size = new SizeEntryDTO();
            size.Wymiary = new WymiaryDTO();
            size.Wymiary.Klatka = 100;
            day.AddEntry(size);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == follower).RowCount();
            Assert.AreEqual(2, count);
        }
        #endregion

        #region Statistics

        [Test]
        public void ForCustomer_AddBlogEntry_Statistics()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.BlogEntriesCount);
        }

       
        [Test]
        public void ForCustomer_AddNewTrainingDay_StatShouldntBeChanged()
        {
            var customer=CreateCustomer("cust",profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.TrainingDaysCount);
            Assert.IsNull( dbProfile.Statistics.LastEntryDate);
        }

        
        [Test]
        public void TestAddNewTrainingDay_Latest()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile=Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1,dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date,dbProfile.Statistics.LastEntryDate.Value);

            date = DateTime.Now.AddDays(-1);
            day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestAddNewTrainingDay_LatestNotChange()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            day = new TrainingDayDTO(DateTime.Now.AddDays(-4));//we add older entry so this shouldn't change the last entry date
            day.ProfileId = profile.GlobalId;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            //last entry should be from the first inserted item
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }


        [Test]
        public void TestAddBlogEntry_Statistics()
        {
            var profile = (ProfileDTO) profiles[0].Tag;
            var profile1 = (ProfileDTO) profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.BlogEntriesCount);
        }

        [Test]
        public void TestAddBlogComment_Statistics()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            day.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });


            data = CreateNewSession(profile1, ClientInformation);
            TrainingDayCommentDTO comment = new TrainingDayCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TrainingDayCommentOperationParam arg = new TrainingDayCommentOperationParam();
                arg.TrainingDayId = day.GlobalId;
                arg.Comment = comment;
                arg.OperationType = TrainingDayOperationType.Add;
                Service.TrainingDayCommentOperation(data.Token, arg);
            });

            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDayCommentsCount);
        }
        #endregion

        #region Empty training day

        [Test]
        public void TestCreateEmptyTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            int count = Session.QueryOver<TrainingDay>().RowCount();

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Assert.IsNull(day);
            Assert.AreEqual(count, Session.QueryOver<TrainingDay>().RowCount());
        }

        [Test]
        public void TestSaveEmptyTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            day.Objects.Clear();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Assert.IsNull(day);
            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
        }
        #endregion

        #region Measurements

        [Test]
        public void SaveSizeEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbSize=Session.QueryOver<Wymiary>().SingleOrDefault();
            Assert.AreEqual(100, dbSize.Height);
        }

        [Test]
        public void UpdateSizeEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            ((SizeEntryDTO) day.Objects[0]).Wymiary.Height = 30;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbSize = Session.QueryOver<Wymiary>().SingleOrDefault();
            Assert.AreEqual(30, dbSize.Height);
        }

     
        #endregion

        #region DataInfo

        [Test]
        public void SaveTrainingDay_UpdateEntry_ChangeReminder_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var oldTrainingDayHash = profiles[0].DataInfo.TrainingDayHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day.Objects[0].RemindBefore = TimeSpan.FromMinutes(15);
                day = Service.SaveTrainingDay(data.Token, day).TrainingDay;
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldTrainingDayHash, dbProfile.DataInfo.TrainingDayHash);
            Assert.AreNotEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void SaveTrainingDay_UpdateEntry_RemoveReminder_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Session.Refresh(profiles[0].DataInfo);
            var oldTrainingDayHash = profiles[0].DataInfo.TrainingDayHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day.Objects[0].RemindBefore = null;
                Service.SaveTrainingDay(data.Token, day);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldTrainingDayHash, dbProfile.DataInfo.TrainingDayHash);
            Assert.AreNotEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void CreateNew_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            var oldTrainingDayHash = profiles[0].DataInfo.TrainingDayHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldTrainingDayHash, dbProfile.DataInfo.TrainingDayHash);
            Assert.AreEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void Update_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            var oldHash = dbProfile.DataInfo.TrainingDayHash;

            day.Comment = "change";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(1);
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });


            dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.TrainingDayHash);
        }

        #endregion

        #region LoginData

        [Test]
        public void CreateNew_SetLoginData()
        {
            LoginData loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation, loginData);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     Service.Configuration.CurrentApiKey = key;
                                     var result = Service.SaveTrainingDay(data.Token, day);
                                     day = result.TrainingDay;
            });



            var dbDay = Session.Get<TrainingDay>(day.GlobalId);
            Assert.AreEqual(key,dbDay.Objects.ElementAt(0).LoginData.ApiKey.ApiKey);
        }

        [Test]
        public void Update_LoginData_should_be_not_changed()
        {
            LoginData loginData = new LoginData();
            loginData.ApiKey = apiKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation, loginData);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.Configuration.CurrentApiKey = key;
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            //simulate another login from another device
            APIKey newKey = new APIKey();
            newKey.ApplicationName = "AnotherTest";
            newKey.EMail = "gfdgdfg@dfgdg.pl";
            newKey.RegisterDateTime = DateTime.UtcNow;
            newKey.ApiKey = Guid.NewGuid();
            insertToDatabase(newKey);
            loginData = new LoginData();
            loginData.ApiKey = newKey;
            loginData.ApplicationVersion = "1.0.0";
            loginData.LoginDateTime = DateTime.UtcNow;
            loginData.ApplicationLanguage = "en";
            loginData.PlatformVersion = "NUnit";
            insertToDatabase(loginData);
            data = CreateNewSession(profile, ClientInformation, loginData);
            day.Objects[0].Comment = "test";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.Configuration.CurrentApiKey = key;
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);
            Assert.AreEqual(key, dbDay.Objects.ElementAt(0).LoginData.ApiKey.ApiKey);
        }
        #endregion

        #region For Customer

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForCustomer_SaveTrainingDay_ChangeCustomerForExistingEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId, day.CustomerId.Value);
            });

            day.CustomerId = customers[1].GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void ForCustomer_SaveTrainingDay_2TrainingDays_OneCustomer_SameDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId, day.CustomerId.Value);
            });

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
        }
        [Test]
        public void ForCustomer_SaveTrainingDay_2TrainingDays_OneForInstructorAndOneForTheCustomer_SameDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId,day.CustomerId.Value);
            });

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.IsNull(day.CustomerId);
            });
            Assert.AreEqual(2, Session.QueryOver<TrainingDay>().RowCount());
        }

        [Test]
        public void ForCustomer_SaveTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId, day.CustomerId.Value);
            });
            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            var dbDay=Session.Get<TrainingDay>(day.GlobalId);
            Assert.AreEqual(customers[0].GlobalId, dbDay.Customer.GlobalId);
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_BugInUpdate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId, day.CustomerId.Value);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);
            Assert.AreEqual(customers[0].GlobalId, dbDay.Customer.GlobalId);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void ForCustomer_SaveTrainingDay_CustomerFromAnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[2].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(customers[0].GlobalId, day.CustomerId.Value);
            });
            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_CreateEntry_NotSendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t",ExerciseType.Nogi, MechanicsType.Isolation,ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNull(message);
        }
        /*
        [Test]
        public void ForCustomer_A6W_IntegrCheck_AddBefore()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            
            //create a new training day with a6w but before the preious one (inetgration error we should get)
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day1.ProfileId = profile.GlobalId;
            day1.CustomerId = customers[0].GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.MyTraining = new MyTrainingDTO();
            a6w1.MyTraining.Name = "name";
            a6w1.MyTraining.CustomerId = customers[0].GlobalId;
            a6w1.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            day1.AddEntry(a6w1);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day1);
            });
            var count=Session.QueryOver<A6WEntry>().RowCount();
            Assert.AreEqual(2,count);

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ForCustomer_A6W_MyTraining_DifferentCustomerId()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Day = A6WManager.Weeks[0];
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            //create a new training day with a6w but before the preious one (inetgration error we should get)
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day1.ProfileId = profile.GlobalId;
            day1.CustomerId = customers[0].GlobalId;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.MyTraining = new MyTrainingDTO();
            a6w1.MyTraining.Name = "name";
            a6w.Day = A6WManager.Weeks[1];
            a6w1.Completed = true;
            day1.AddEntry(a6w1);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day1);
            });

        }
        */
        [Test]
        public void ForCustomer_SaveTrainingDay_WithReservation()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Customer = customers[0];
            reservation.Name = "test";
            reservation.Profile = profiles[0];
            ScheduleEntry se = new ScheduleEntry();
            se.Profile = profiles[0];
            se.MyPlace = GetDefaultMyPlace(profiles[0]);
            se.StartTime = DateTime.Now;
            se.EndTime = DateTime.Now.AddHours(1);
            se.Activity = CreateActivity("test",profiles[0]);
            se.Reservations.Add(reservation);
            insertToDatabase(se);

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            entry.ReservationId = reservation.GlobalId;
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(reservation.GlobalId, day.Objects[0].ReservationId);
            });
            var dbEntry=Session.QueryOver<StrengthTrainingEntry>().SingleOrDefault();
            Assert.AreEqual(reservation.GlobalId,dbEntry.Reservation.GlobalId);
        }

        #endregion

        #region Reminders

        [Test]
        public void SaveTrainingDay_AddSizesWithReminder_SetTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            SizeEntryDTO size = new SizeEntryDTO();
            size.Wymiary = new WymiaryDTO();
            size.RemindBefore = TimeSpan.FromMinutes(30);
            size.Wymiary.Time.DateTime = DateTime.Today.AddHours(10);
            size.Wymiary.LeftUdo = 33;
            day.AddEntry(size);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(TimeSpan.FromMinutes(30), day.Objects[0].RemindBefore);
            });


            var dbEntry = Session.Get<EntryObject>(day.Objects[0].GlobalId);
            Assert.AreEqual(size.Wymiary.Time.DateTime, dbEntry.Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), dbEntry.Reminder.RemindBefore);
            Assert.AreEqual("EntryObjectDTO:" + dbEntry.GlobalId, dbEntry.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbEntry.Reminder.Repetitions);
            Assert.AreEqual(Model.ReminderType.EntryObject, dbEntry.Reminder.Type);
        }

        [Test]
        public void SaveTrainingDay_AddStrengthTrainingWithReminder_SetStartTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.StartTime = DateTime.Today.AddHours(10);
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(TimeSpan.FromMinutes(30), day.Objects[0].RemindBefore);
            });


            var dbEntry = Session.Get<EntryObject>(day.Objects[0].GlobalId);
            Assert.AreEqual(entry.StartTime.Value, dbEntry.Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), dbEntry.Reminder.RemindBefore);
            Assert.AreEqual("EntryObjectDTO:" + dbEntry.GlobalId, dbEntry.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbEntry.Reminder.Repetitions);
            Assert.AreEqual(Model.ReminderType.EntryObject, dbEntry.Reminder.Type);
        }

        [Test]
        public void SaveTrainingDay_AddEntryWithReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation,ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(TimeSpan.FromMinutes(30),day.Objects[0].RemindBefore);
            });


            var dbEntry = Session.Get<EntryObject>(day.Objects[0].GlobalId);
            Assert.AreEqual(dbEntry.TrainingDay.TrainingDate, dbEntry.Reminder.DateTime);
            Assert.AreEqual(TimeSpan.FromMinutes(30), dbEntry.Reminder.RemindBefore);
            Assert.AreEqual("EntryObjectDTO:" + dbEntry.GlobalId, dbEntry.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbEntry.Reminder.Repetitions);
            Assert.AreEqual(Model.ReminderType.EntryObject, dbEntry.Reminder.Type);
        }

        [Test]
        public void SaveTrainingDay_UpdateEntry_ChangeReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",  ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     day.Objects[0].RemindBefore = TimeSpan.FromMinutes(15);
                                     var result = Service.SaveTrainingDay(data.Token, day);
                                     day = result.TrainingDay;
            });

            var dbEntry = Session.Get<EntryObject>(day.Objects[0].GlobalId);
            Assert.AreEqual(dbEntry.TrainingDay.TrainingDate, dbEntry.Reminder.DateTime.Date);
            Assert.AreEqual(TimeSpan.FromMinutes(15), dbEntry.Reminder.RemindBefore);
            Assert.AreEqual("EntryObjectDTO:" + dbEntry.GlobalId, dbEntry.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.Once, dbEntry.Reminder.Repetitions);
            Assert.AreEqual(1, Session.QueryOver<ReminderItem>().RowCount());
            Assert.AreEqual(Model.ReminderType.EntryObject, dbEntry.Reminder.Type);
        }

        [Test]
        public void SaveTrainingDay_UpdateEntry_RemoveReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t",ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.RemindBefore = TimeSpan.FromMinutes(30);
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day.Objects[0].RemindBefore = null;
                Service.SaveTrainingDay(data.Token, day);
            });

            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        #endregion

        #region MyPlace

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveTrainingDay_AddEntryWithMyPlace_AnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[1]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.MyPlace = myGym.Map<MyPlaceDTO>();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            
        }

        [Test]
        public void SaveTrainingDay_ChangeTrainingDate_CutPaste()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);
            var entry = addTrainingDaySet(profiles[0], null, DateTime.Today, exercise, new Tuple<int?, decimal?>(10, 20));
            SessionData data = CreateNewSession(profile, ClientInformation);
            var dayDTO=entry.TrainingDay.Map<TrainingDayDTO>();

            dayDTO.ChangeDate(DateTime.Today.AddDays(-2));
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDTO);
                dayDTO = result.TrainingDay;
            });

            var count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(1,count);
            var dbDay = Session.Get<TrainingDay>(dayDTO.GlobalId);
            Assert.AreEqual(DateTime.Today.AddDays(-2), dbDay.TrainingDate);

        }

        [Test]
        public void SaveTrainingDay_AddEntryWithMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var myGym = CreateMyPlace("gym", profiles[0]);
            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.MyPlace = myGym.Map<MyPlaceDTO>();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });


            var dbEntry = Session.Get<StrengthTrainingEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(myGym.GlobalId, dbEntry.MyPlace.GlobalId);

        }

        [Test]
        public void SaveTrainingDay_AddEntryWithEmptryMyPlace_UseDefault()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var defaultPlace=Session.QueryOver<MyPlace>().Where(x=>x.Profile==profiles[0] && x.IsDefault).SingleOrDefault();
            var dbEntry = Session.Get<StrengthTrainingEntry>(day.Objects[0].GlobalId);
            Assert.AreEqual(defaultPlace.GlobalId, dbEntry.MyPlace.GlobalId);

        }



        #endregion


        #region Exercise records

        StrengthTrainingEntry addTrainingDaySet(Profile profile, Customer customer, DateTime date, Exercise exercise, params Tuple<int?, decimal?>[] sets)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profile);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);

            foreach (var tuple in sets)
            {
                Serie set1 = new Serie();
                set1.RepetitionNumber = tuple.Item1;
                set1.Weight = tuple.Item2;
                item.AddSerie(set1);
            }
            insertToDatabase(trainingDay);
            return entry;
        }

        [Test]
        public void ExerciseRecord_TakeOnlyForThisProfile_OtherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[1], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(10, 70));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_TakeOnlyForThisProfile_Customer_SavingForProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            var customer = CreateCustomer("cust", profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], customer, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(10, 70));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.IsNull(exData.Customer);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_TakeOnlyForThisProfile_Customer_SavingForCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            var customer = CreateCustomer("cust", profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 80));
            addTrainingDaySet(profiles[0], customer, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(10, 70));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;
            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(customer, exData.Customer);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(70, exData.MaxWeight);
            Assert.AreEqual(10, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_Repetitions0()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(0, 70));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_Weight0()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);


            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            var serie = new SerieDTO("2x0");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }

        [Test]
        public void ExerciseRecord_RepetitionsNull()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(null, 70));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_WeightNull()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(19, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_UseExerciseFirstTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0],exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(2, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
            Assert.AreEqual(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x=>x.Weight==40).Single().GlobalId, exData.Serie.GlobalId);
            Assert.IsTrue(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().IsRecord);
        }

        [Test]
        public void ExerciseRecord_ChangeExercise()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);
            var exercise2 = CreateExercise(Session, null, "test2", "t2", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(2, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
            Assert.AreEqual(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().GlobalId, exData.Serie.GlobalId);
            Assert.IsTrue(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().IsRecord);

            ((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].Exercise = exercise2.Map<ExerciseLightDTO>();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();

            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise2, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(2, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
            Assert.AreEqual(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().GlobalId, exData.Serie.GlobalId);
            Assert.IsTrue(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().IsRecord);
        }

       
        [Test]
        public void ExerciseRecord_SkipPlanned()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.Status = EntryObjectStatus.Planned;
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }

        [Test]
        public void ExerciseRecord_ForCustomer_UseExerciseFirstTime()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var customer = CreateCustomer("cust", profiles[0]);
            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customer.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(customer, exData.Customer);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(2, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
            Assert.AreEqual(((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single().GlobalId, exData.Serie.GlobalId);
        }

        [Test]
        public void ExerciseRecord_UpdateRecord_SameTrainingDay_ChangeMaxSet_Up()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("2x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie2;
            exData.MaxWeight = 40;
            exData.Repetitions = 2;
            exData.TrainingDate = day.TrainingDate;
            insertToDatabase(exData);

            var dayDto = day.Map<TrainingDayDTO>();
            var serie2Dto=((StrengthTrainingEntryDTO) dayDto.Objects[0]).Entries[0].Series.Where(x => x.GlobalId == serie2.GlobalId).Single();
            serie2Dto.Weight = 50;
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(50m, exData.MaxWeight);
            Assert.AreEqual(2, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
            Assert.AreEqual(((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series.Where(x => x.Weight == 50).Single().GlobalId, exData.Serie.GlobalId);
        }

        [Test]
        public void ExerciseRecord_RemoveMaxSet()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var day = new TrainingDay(DateTime.Now.Date.AddDays(-1));
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("7x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            var day1 = new TrainingDay(DateTime.Now.Date);
            day1.Profile = profiles[0];

            entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day1.AddEntry(entry);
            item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie11 = new Serie("10x5");
            item.AddSerie(serie11);
            var serie12 = new Serie("1x50");
            item.AddSerie(serie12);
            var serie13 = new Serie("4x11");
            item.AddSerie(serie13);
            insertToDatabase(day1);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie12;
            exData.MaxWeight = 50;
            exData.Repetitions = 1;
            exData.TrainingDate = day1.TrainingDate;
            insertToDatabase(exData);

            TrainingDayDTO dayDto = day1.Map<TrainingDayDTO>();
            var serie2Dto = ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series.Where(x => x.GlobalId == serie12.GlobalId).Single();
            ((StrengthTrainingEntryDTO) dayDto.Objects[0]).Entries[0].Series.Remove(serie2Dto);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(7, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date.AddDays(-1), exData.TrainingDate);
            Assert.AreEqual(serie2.GlobalId, exData.Serie.GlobalId);
        }

        [Test]
        public void ExerciseRecord_EmptyTrainingDayWithMaxSet()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var day = new TrainingDay(DateTime.Now.Date.AddDays(-1));
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("7x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            var day1 = new TrainingDay(DateTime.Now.Date);
            day1.Profile = profiles[0];

            entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day1.AddEntry(entry);
            item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie11 = new Serie("10x5");
            item.AddSerie(serie11);
            var serie12 = new Serie("1x50");
            item.AddSerie(serie12);
            var serie13 = new Serie("4x11");
            item.AddSerie(serie13);
            insertToDatabase(day1);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie12;
            exData.MaxWeight = 50;
            exData.Repetitions = 1;
            exData.TrainingDate = day1.TrainingDate;
            insertToDatabase(exData);

            TrainingDayDTO dayDto = day1.Map<TrainingDayDTO>();
            dayDto.Objects.Clear();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(exercise, exData.Exercise);
            Assert.AreEqual(40m, exData.MaxWeight);
            Assert.AreEqual(7, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date.AddDays(-1), exData.TrainingDate);
            Assert.AreEqual(serie2.GlobalId, exData.Serie.GlobalId);
        }

        [Test]
        public void ExerciseRecord_RemoveLastEntryForExercise()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);


            var day1 = new TrainingDay(DateTime.Now.Date);
            day1.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day1.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie11 = new Serie("10x5");
            item.AddSerie(serie11);
            var serie12 = new Serie("1x50");
            item.AddSerie(serie12);
            var serie13 = new Serie("4x11");
            item.AddSerie(serie13);
            insertToDatabase(day1);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie12;
            exData.MaxWeight = 50;
            exData.Repetitions = 1;
            exData.TrainingDate = day1.TrainingDate;
            insertToDatabase(exData);

            TrainingDayDTO dayDto = day1.Map<TrainingDayDTO>();
            dayDto.Objects.Clear();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, dayDto);
            });
            exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }


        [Test]
        public void ExerciseRecord_UpdateRecord_TwoExercises()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,ExerciseForceType.Pull);
            var exercise1 = CreateExercise(Session, null, "test1", "t1", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            var day = new TrainingDay(DateTime.Now.Date);
            day.Profile = profiles[0];

            var entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            day.AddEntry(entry);
            var item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            var serie1 = new Serie("10x10");
            item.AddSerie(serie1);
            var serie2 = new Serie("2x40");
            item.AddSerie(serie2);
            var serie3 = new Serie("4x20");
            item.AddSerie(serie3);
            insertToDatabase(day);

            var item2 = new StrengthTrainingItem();
            item2.Exercise = exercise1;
            entry.AddEntry(item2);
            var serie21 = new Serie("10x110");
            item2.AddSerie(serie21);
            var serie22 = new Serie("12x140");
            item2.AddSerie(serie22);
            var serie23 = new Serie("4x120");
            item2.AddSerie(serie23);
            insertToDatabase(day);

            ExerciseProfileData exData = new ExerciseProfileData();
            exData.Exercise = exercise;
            exData.Profile = profiles[0];
            exData.Serie = serie2;
            exData.MaxWeight = 40;
            exData.Repetitions = 2;
            exData.TrainingDate = day.TrainingDate;
            insertToDatabase(exData);

            var dayDto = day.Map<TrainingDayDTO>();
            var serie2Dto = ((StrengthTrainingEntryDTO)dayDto.Objects[0]).Entries[0].Series.Where(x => x.GlobalId == serie2.GlobalId).Single();
            serie2Dto.Weight = 50;

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, dayDto);
                dayDto = result.TrainingDay;
            });
            var exDatas = Session.QueryOver<ExerciseProfileData>().List();
            Assert.AreEqual(2, exDatas.Count);
            Assert.AreEqual(profiles[0], exDatas[0].Profile);
            Assert.AreEqual(exercise, exDatas[0].Exercise);
            Assert.AreEqual(50m, exDatas[0].MaxWeight);
            Assert.AreEqual(2, exDatas[0].Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exDatas[0].TrainingDate);
            Assert.AreEqual(serie2.GlobalId, exDatas[0].Serie.GlobalId);

            Assert.AreEqual(profiles[0], exDatas[1].Profile);
            Assert.AreEqual(exercise1, exDatas[1].Exercise);
            Assert.AreEqual(140m, exDatas[1].MaxWeight);
            Assert.AreEqual(12, exDatas[1].Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exDatas[1].TrainingDate);
            Assert.AreEqual(serie22.GlobalId, exDatas[1].Serie.GlobalId);
        }



        [Test]
        public void ExerciseRecord_SerieValidator_SavingIncorrectValue()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 52));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x320");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(60m, exData.MaxWeight);
            Assert.AreEqual(3, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_SerieValidator_IncorrectValueInDb()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            var item1=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 330));
            item1.Entries.ElementAt(0).Series.Where(x => x.Weight == 330).Single().IsIncorrect = true;
            insertToDatabase(item1);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x70");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(70m, exData.MaxWeight);
            Assert.AreEqual(4, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_SaveStrenghtTraining_MyPlaceWithNotForRecords_SkipRecalculateRecord()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var anotherPlace = CreateMyPlace("notforrecord",profiles[0],true);
            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 52));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            entry.MyPlace = anotherPlace.Map<MyPlaceLightDTO>();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x61");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }

        [Test]
        public void ExerciseRecord_ExistingStrenghtTraining_MyPlaceWithNotForRecords_SkipRecalculateRecord()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var anotherPlace = CreateMyPlace("notforrecord", profiles[0], true);
            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dbEntry=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 80));
            dbEntry.MyPlace = anotherPlace;
            insertToDatabase(dbEntry);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 52));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x61");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(61m, exData.MaxWeight);
            Assert.AreEqual(4, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_Serie_WithMuscleFailure()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 61));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x70");
            serie.SetType = SetType.MuscleFailure;
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(61, exData.MaxWeight);
            Assert.AreEqual(14, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_SaveStrenghtTraining_NotDefaultExerciseDoneWay_SkipRecalculateRecord()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 52));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            item.DoneWay = ExerciseDoneWay.Machine;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x61");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.IsNull(exData);
        }

        [Test]
        public void ExerciseRecord_ExistingStrenghtTraining_NotDefaultDoneWay_SkipRecalculateRecord()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            var dbEntry = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 80));
            dbEntry.Entries.ElementAt(0).DoneWay = Model.ExerciseDoneWay.Machine;
            insertToDatabase(dbEntry);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(14, 52));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);
            serie = new SerieDTO("4x61");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(61m, exData.MaxWeight);
            Assert.AreEqual(4, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.Date, exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_WeightNull_Cardio()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var ex = CreateExercise(Session, null, "test", "t", ExerciseType.Cardio, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, decimal?>(10, 30), new Tuple<int?, decimal?>(5, 50));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(10, 20), new Tuple<int?, decimal?>(null, 100));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(3, 60), new Tuple<int?, decimal?>(5, 40));

            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = ex.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);


            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });

            var exData = Session.QueryOver<ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(ex, exData.Exercise);
            Assert.AreEqual(100, exData.MaxWeight);
            Assert.AreEqual(null, exData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), exData.TrainingDate);
        }

        [Test]
        public void ExerciseRecord_Result_NewRecords()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                var record=((StrengthTrainingEntryDTO) day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single();
                Assert.AreEqual(1,result.NewRecords.Count);
                Assert.AreEqual(record, result.NewRecords[0]);
            });


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                Assert.AreEqual(0, result.NewRecords.Count);
            });
        }

        [Test]
        public void ExerciseRecord_Result_NewRecords_Change()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.Date);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            serie = new SerieDTO("2x40");
            item.AddSerie(serie);
            serie = new SerieDTO("4x20");
            item.AddSerie(serie);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
                var record = ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 40).Single();
                Assert.AreEqual(1, result.NewRecords.Count);
                Assert.AreEqual(record, result.NewRecords[0]);
            });

            var set1 = ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 10).Single();
            set1.Weight = 41;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;

                var record = ((StrengthTrainingEntryDTO)day.Objects[0]).Entries[0].Series.Where(x => x.Weight == 41).Single();
                Assert.AreEqual(1, result.NewRecords.Count);
                Assert.AreEqual(record, result.NewRecords[0]);
            });
        }
        //[Test]
        //[Category("Shims")]
        //public void ExerciseRecord_ShouldntBeInvoked()
        //{
        //    using (ShimsContext.Create())
        //    {
        //        int counter = 0;
        //        BodyArchitect.Service.V2.Services.Fakes.ShimTrainingDayService.AllInstances.
        //                                     calculateExerciseRecordsProfileListOfGuid = (td, p, exercises) =>
        //                                                                                     {
        //                                                                                         counter++;
        //                                                                                     };

        //        var profile = (ProfileDTO)profiles[0].Tag;
        //        SessionData data = CreateNewSession(profile, ClientInformation);

        //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
        //        day.ProfileId = profile.GlobalId;
        //        SizeEntryDTO entry = new SizeEntryDTO();
        //        entry.Wymiary = new WymiaryDTO();
        //        entry.Wymiary.Height = 100;
        //        day.AddEntry(entry);


        //        RunServiceMethod(delegate(InternalBodyArchitectService service)
        //                             {


        //                                 service.SaveTrainingDay(data.Token, day);
        //                                 Assert.AreEqual(0, counter);

        //                             });
        //    }
        //}

        #endregion

        #region System entries

        [Test]
        public void System_ModifySystemEntry()
        {
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry entry = new SizeEntry();
            entry.Wymiary = new Wymiary();
            entry.Wymiary.Height = 100;
            entry.Status = Model.EntryObjectStatus.System;
            day.AddEntry(entry);

            insertToDatabase(day);
            var dayDTO = day.Map<TrainingDayDTO>();

            ((SizeEntryDTO) dayDTO.Objects[0]).Wymiary.Weight = 200;
            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, dayDTO);
            });

            Assert.AreEqual(0,((SizeEntryDTO)result.TrainingDay.Objects[0]).Wymiary.Weight);
            var dbEntry=Session.Get<SizeEntry>(result.TrainingDay.Objects[0].GlobalId);
            Assert.AreEqual(0, (dbEntry).Wymiary.Weight);
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void System_AddedSystemEntry()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            entry.Status = EntryObjectStatus.System;
            day.AddEntry(entry);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void System_ChangeStatusToSystem()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry entry = new SizeEntry();
            entry.Wymiary = new Wymiary();
            entry.Wymiary.Height = 100;
            entry.Status = Model.EntryObjectStatus.Done;
            day.AddEntry(entry);

            insertToDatabase(day);
            var dayDTO = day.Map<TrainingDayDTO>();

            dayDTO.Objects[0].Status = EntryObjectStatus.System;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, dayDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void System_ChangeSystemStatusToDone()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry entry = new SizeEntry();
            entry.Wymiary = new Wymiary();
            entry.Wymiary.Height = 100;
            entry.Status = Model.EntryObjectStatus.System;
            day.AddEntry(entry);

            insertToDatabase(day);
            var dayDTO = day.Map<TrainingDayDTO>();

            dayDTO.Objects[0].Status = EntryObjectStatus.Done;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, dayDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void System_DeleteEntryFromTraining()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry entry = new SizeEntry();
            entry.Wymiary = new Wymiary();
            entry.Wymiary.Height = 100;
            entry.Status = Model.EntryObjectStatus.System;
            day.AddEntry(entry);
            var entry1 = new SizeEntry();
            entry1.Wymiary = new Wymiary();
            entry1.Wymiary.Height = 100;
            entry1.Status = Model.EntryObjectStatus.Done;
            day.AddEntry(entry1);
            insertToDatabase(day);
            var dayDTO = day.Map<TrainingDayDTO>();

            var systemEntryDTO = dayDTO.Objects.Where(x => x.GlobalId == entry.GlobalId).Single();
            dayDTO.Objects.Remove(systemEntryDTO);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, dayDTO);
            });
        }
        #endregion

        #region Calculated value

        [Test]
        public void CalculateCaloriesAndWilks()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Gender = Model.Gender.Male;
            Wymiary wymiary = new Wymiary();
            wymiary.Weight = 90;
            profiles[0].Wymiary = wymiary;
            insertToDatabase(profiles[0]);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation,ExerciseForceType.Pull);
            var cardio = CreateExercise(Session, null, "cardio", "td", ExerciseType.Cardio, MechanicsType.Isolation, ExerciseForceType.Pull);
            cardio.Met = 4;
            insertToDatabase(cardio);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x80");
            item.AddSerie(serie);
            item = new StrengthTrainingItemDTO();
            item.Exercise = cardio.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("00:40:00");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);

            //Assert.AreEqual(51.072M, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(null, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(243m, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == cardio.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
        }

        [Test]
        public void CalculateCaloriesAndWilks_ForCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Gender = Model.Gender.Male;
            Wymiary wymiary = new Wymiary();
            wymiary.Weight = 90;
            profiles[0].Wymiary = wymiary;
            insertToDatabase(profiles[0]);

            wymiary = new Wymiary();
            wymiary.Weight = 70;
            customers[0].Wymiary = wymiary;
            insertToDatabase(customers[0]);

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);
            var cardio = CreateExercise(Session, null, "cardio", "td", ExerciseType.Cardio, MechanicsType.Isolation, ExerciseForceType.Pull);
            cardio.Met = 4;
            insertToDatabase(cardio);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x80");
            item.AddSerie(serie);
            item = new StrengthTrainingItemDTO();
            item.Exercise = cardio.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("00:40:00");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);

            //Assert.AreEqual(59.952m, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(null, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(190m, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == cardio.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
        }

        [Test]
        public void CalculateCaloriesAndWilks_NullWymiary()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);
            var cardio = CreateExercise(Session, null, "cardio", "td", ExerciseType.Cardio, MechanicsType.Isolation, ExerciseForceType.Pull);
            cardio.Met = 4;
            insertToDatabase(cardio);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x80");
            item.AddSerie(serie);
            item = new StrengthTrainingItemDTO();
            item.Exercise = cardio.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO("00:40:00");
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);

            Assert.AreEqual(null, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(259m, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == cardio.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
        }

        [Test]
        public void CalculateCaloriesAndWilks_NullWeight()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, null, "test", "t", ExerciseType.Nogi, MechanicsType.Isolation, ExerciseForceType.Pull);
            var cardio = CreateExercise(Session, null, "cardio", "td", ExerciseType.Cardio, MechanicsType.Isolation, ExerciseForceType.Pull);
            cardio.Met = 4;
            insertToDatabase(cardio);

            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.Exercise = exercise.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x");
            item.AddSerie(serie);
            item = new StrengthTrainingItemDTO();
            item.Exercise = cardio.Map<ExerciseLightDTO>();
            entry.AddEntry(item);
            serie = new SerieDTO();
            item.AddSerie(serie);

            SaveTrainingDayResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveTrainingDay(data.Token, day);
                day = result.TrainingDay;
            });
            var dbDay = Session.Get<TrainingDay>(day.GlobalId);

            Assert.AreEqual(null, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == exercise.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
            Assert.AreEqual(null, dbDay.Objects.OfType<StrengthTrainingEntry>().Single().Entries.Where(x => x.Exercise.GlobalId == cardio.GlobalId).Single().Series.ElementAt(0).CalculatedValue);
        }
        #endregion
    }
}
