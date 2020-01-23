using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using MessagePriority = BodyArchitect.Model.MessagePriority;
using MessageType = BodyArchitect.Model.MessageType;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_SaveTrainingDay:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);
                tx.Commit();
            }
        }

        #region A6W integrity

        [Test]
        public void TestA6W_BreakTraining_DateOfLastElement()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            TrainingDayDTO day3 = new TrainingDayDTO(DateTime.Now.AddDays(2));
            day3.ProfileId = profile.Id;
            A6WEntryDTO a6w2 = new A6WEntryDTO();
            a6w2.DayNumber = 3;
            a6w2.Completed = true;
            a6w2.MyTraining = day1.Objects[0].MyTraining;
            a6w2.MyTraining.Name = "name";
            day3.AddEntry(a6w2);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day3 = Service.SaveTrainingDay(data.Token, day3);
            });

            ((A6WEntryDTO)day3.Objects[0]).MyTraining.Abort();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day3 = Service.SaveTrainingDay(data.Token, day3);
            });
            var myTrainingDb=Session.Get<MyTraining>(day3.Objects[0].MyTraining.Id);
            Assert.AreEqual(day3.TrainingDate,myTrainingDb.EndDate.Value);
        }

        

        [Test]
        public void TestA6W_CreateANewTraining()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Completed = true;
            a6w.MyTraining=new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });
            
            var dbDay=Session.QueryOver<TrainingDay>().Where(x => x.TrainingDate == day.TrainingDate).SingleOrDefault();
            Assert.IsNotNull(dbDay);
            Assert.AreEqual(dbDay.Objects.Count, day.Objects.Count);
        }

        [Test]
        public void TestA6W_CompleteTraining()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            MyTrainingDTO training = new MyTrainingDTO();

            foreach (A6WDay a6WDay in A6WManager.Days)
            {
                TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-44).AddDays(a6WDay.DayNumber));
                day.ProfileId = profile.Id;
                A6WEntryDTO a6w = new A6WEntryDTO();
                a6w.Completed = true;
                a6w.MyTraining = training;
                a6w.DayNumber = a6WDay.DayNumber;
                a6w.MyTraining.Name = "name";
                day.AddEntry(a6w);
                RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    if(a6w.DayNumber==42)
                    {
                        training.Complete();
                    }
                    day=service.SaveTrainingDay(data.Token, day);
                    training=day.Objects[0].MyTraining;
                });
            }

            var dbTraining=Session.Get<MyTraining>(training.Id);
            Assert.AreEqual(TrainingEnd.Complete,dbTraining.TrainingEnd);
            Assert.IsNotNull(dbTraining.EndDate);
        }

        [Test]
        public void TestA6W_TwoA6WEntries_ModifyFirst_Bug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            ((A6WEntryDTO) day.Objects[0]).Completed = false;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbA6w = Session.Get<A6WEntry>(day.Objects[0].Id);
            Assert.IsFalse(dbA6w.Completed);
        }

        [Test]
        public void TestA6W_StopTraining_Bug()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1 = Service.SaveTrainingDay(data.Token, day1);
            });

            day.Objects[0].MyTraining.Abort();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbA6w = Session.Get<A6WEntry>(day.Objects[0].Id);
            Assert.AreEqual(TrainingEnd.Break,dbA6w.MyTraining.TrainingEnd);
            Assert.IsNotNull(dbA6w.MyTraining.EndDate);
        }

        [Test]
        [ExpectedException(typeof(TrainingIntegrationException))]
        public void TestA6W_IntegrErr_AddBefore()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
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
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
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
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1=Service.SaveTrainingDay(data.Token, day1);
            });
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day);
            });

            

        }

        [Test]
        public void TestA6W_RemovingMyTrainingAutomatically()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.DayNumber = 1;
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
            day1.ProfileId = profile.Id;
            A6WEntryDTO a6w1 = new A6WEntryDTO();
            a6w1.DayNumber = 2;
            a6w1.Completed = true;
            a6w1.MyTraining = day.Objects[0].MyTraining;
            a6w1.MyTraining.Name = "name";
            day1.AddEntry(a6w1);
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day1=Service.SaveTrainingDay(data.Token, day1);
            });

            var myTraining=Session.Get<MyTraining>(a6w1.MyTraining.Id);
            Assert.IsNotNull(myTraining);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day1);
            });
            myTraining = Session.Get<MyTraining>(a6w1.MyTraining.Id);
            Assert.IsNotNull(myTraining);

            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteTrainingDay(data.Token, day);
            });

            myTraining = Session.Get<MyTraining>(a6w1.MyTraining.Id);
            Assert.IsNull(myTraining);

        }
        #endregion

        #region Concurency

        [Test]
        public void TestCreateTwoTrainingDaysOnTheSameDateForDifferentProfile_BugFixing()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile1.Id;
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
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary=new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
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
        #endregion

        #region Blog entries

        [Test]
        public void TestAddBlogComment_SendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });
            
            var messages = Session.QueryOver<Message>().Where(x=>x.Receiver==profiles[0] && x.MessageType==MessageType.BlogCommentAdded).SingleOrDefault();
            Assert.IsNotNull(messages);
            Assert.AreEqual(3,messages.Content.Split(',').Length);
            Assert.AreEqual(profile1.UserName, messages.Content.Split(',')[1]);
        }

        [Test]
        public void TestAddBlogComment_DisableSendMessage()
        {
            profiles[0].Settings.NotificationBlogCommentAdded = false;
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.BlogCommentAdded).SingleOrDefault();
            Assert.IsNull(messages);
        }

        [Test]
        public void TestAddBlogCommentByOwner_DontSendMessage()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var messages = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[0] && x.MessageType == MessageType.BlogCommentAdded).SingleOrDefault();
            Assert.IsNull(messages);

        }

        [Test]
        public void TestDeleteBlogEntryWithComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });
            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            comment = new BlogCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg1";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            var dbComments = Session.QueryOver<BlogComment>().List();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual(2, dbComments.Count);

            //remove blogentry from training day
            day.Objects.RemoveAt(0);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            dbComments = Session.QueryOver<BlogComment>().List();
            Assert.IsNull(dbEntry);
            Assert.AreEqual(0, dbComments.Count);
            
        }

        [Test]
        public void TestCreateBlogEntry()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
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
            Assert.AreEqual(0, dbEntry.BlogCommentsCount);
            Assert.AreEqual(null, dbEntry.LastCommentDate);

        }

        [Test]
        public void TestCreateBlogComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO) profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            blogEntry.AllowComments = true;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            var dbComment = Session.QueryOver<BlogComment>().SingleOrDefault();
            Assert.IsNotNull(dbEntry);
            Assert.IsNotNull(dbComment);
            Assert.AreEqual(comment.Comment, dbComment.Comment);
            Assert.AreEqual(profile1.Id, dbComment.Profile.Id);
            Assert.AreEqual(dbComment.DateTime,dbEntry.LastCommentDate);
        }

        [Test]
        public void TestCreateTwoBlogComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });
            //sleep is to check LastCommentDateTime
            Thread.Sleep(300);

            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            comment = new BlogCommentDTO();
            comment.Profile = profile;
            comment.Comment = "msg1";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            var dbComments = Session.QueryOver<BlogComment>().List();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual(2, dbComments.Count);
            Assert.AreEqual(comment.Comment, dbComments[1].Comment);
            Assert.AreEqual(profile.Id, dbComments[1].Profile.Id);
            Assert.AreEqual(dbComments[1].DateTime, dbEntry.LastCommentDate);
        }

        [Test]
        public void TestEditBlogEntryWithComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            blogEntry.AllowComments = true;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            data = SecurityManager.CreateNewSession(profile, ClientInformation);
            day.Objects[0].Comment = "newComment";
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            var dbEntry = Session.QueryOver<BlogEntry>().SingleOrDefault();
            Assert.IsNotNull(dbEntry);
            Assert.AreEqual("newComment", dbEntry.Comment);
            Assert.AreEqual(1, dbEntry.BlogCommentsCount);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestCreateBlogComments_CrossProfileException()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            blogEntry.AllowComments = true;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateBlogCommentsInNotAllowedBlogEntry_Exception()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            blogEntry.AllowComments = false;
            day.AddEntry(blogEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });
        }
        #endregion

        #region Strength workout

        [Test]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void TestSaveStrengthTrainingWithPendingExerciseFromOtherUser()
        {
            var profile = (ProfileDTO) profiles[0].Tag;
            var profile1 = (ProfileDTO) profiles[1].Tag;

            var exercise = CreateExercise(Session, profiles[1], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);
            

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     day = Service.SaveTrainingDay(data.Token, day);
                                 });
        }

        [Test]
        public void TestSaveStrengthTrainingWithPendingExercise()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            Assert.AreEqual(1,Session.QueryOver<TrainingDay>().RowCount());
        }

        #endregion

        #region Send messages

        [Test]
        public void TestSaveTrainingDay_CreateEntry_DisableSendingMessage()
        {
            var friend = profiles[3];
            friend.Settings.NotificationFriendChangedCalendar = false;
            insertToDatabase(friend);

            var profile = (ProfileDTO)profiles[0].Tag;
            

            var exercise = CreateExercise(Session, profiles[0], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var message = Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNull(message);
        }

        [Test]
        public void TestSaveTrainingDay_CreateEntry_SendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var message=Session.QueryOver<Message>().Where(x => x.Receiver == friend).SingleOrDefault();
            Assert.IsNotNull(message);
            Assert.AreEqual(MessageType.TrainingDayAdded, message.MessageType);
            Assert.AreEqual(MessagePriority.System, message.Priority);
            Assert.AreEqual(3, message.Content.Split(',').Length);
            Assert.AreEqual(profile.UserName, message.Content.Split(',')[1]);
        }

        [Test]
        public void TestSaveTrainingDay_EditEntry_NoSendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == friend).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestSaveTrainingDay_AddNewEntry_SendMessageToFriend()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var friend = profiles[3];

            var exercise = CreateExercise(Session, profiles[0], "test", "t", PublishStatus.PendingPublish, ExerciseType.Nogi, MechanicsType.Isolation,
                           ExerciseForceType.Pull, ExerciseDifficult.One);

            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            day.AddEntry(entry);
            StrengthTrainingItemDTO item = new StrengthTrainingItemDTO();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);
            SerieDTO serie = new SerieDTO("10x10");
            item.AddSerie(serie);


            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            SizeEntryDTO size=new SizeEntryDTO();
            size.Wymiary=new WymiaryDTO();
            size.Wymiary.Klatka = 100;
            day.AddEntry(size);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == friend).RowCount();
            Assert.AreEqual(2, count);
        }
        #endregion

        #region Statistics

        [Test]
        public void TestAddNewTrainingDay_Latest()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile=Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1,dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date,dbProfile.Statistics.LastEntryDate.Value);

            date = DateTime.Now.AddDays(-1);
            day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestAddNewTrainingDay_LatestNotChange()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            DateTime date = DateTime.Now.AddDays(-2);
            TrainingDayDTO day = new TrainingDayDTO(date);
            day.ProfileId = profile.Id;

            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1, dbProfile.Statistics.TrainingDaysCount);
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);

            day = new TrainingDayDTO(DateTime.Now.AddDays(-4));//we add older entry so this shouldn't change the last entry date
            day.ProfileId = profile.Id;

            sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(2, dbProfile.Statistics.TrainingDaysCount);
            //last entry should be from the first inserted item
            Assert.AreEqual(date.Date, dbProfile.Statistics.LastEntryDate.Value);
        }

        [Test]
        public void TestA6W_Statistics()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            A6WEntryDTO a6w = new A6WEntryDTO();
            a6w.Completed = true;
            a6w.MyTraining = new MyTrainingDTO();
            a6w.MyTraining.Name = "name";
            day.AddEntry(a6w);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveTrainingDay(data.Token, day);
            });

            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1, dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        public void TestAddBlogEntry_Statistics()
        {
            var profile = (ProfileDTO) profiles[0].Tag;
            var profile1 = (ProfileDTO) profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     day = Service.SaveTrainingDay(data.Token, day);
                                 });
            var dbProfile = Session.Get<Profile>(profile.Id);
            Assert.AreEqual(1, dbProfile.Statistics.BlogEntriesCount);
        }

        [Test]
        public void TestAddBlogComment_Statistics()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;


            BlogEntryDTO blogEntry = new BlogEntryDTO();
            blogEntry.Comment = "jakis tekst";
            day.AddEntry(blogEntry);
            blogEntry.AllowComments = true;
            SizeEntryDTO sizeEntry = new SizeEntryDTO();
            sizeEntry.Wymiary = new WymiaryDTO();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });


            data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            BlogCommentDTO comment = new BlogCommentDTO();
            comment.Profile = profile1;
            comment.Comment = "msg";

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                BlogCommentOperation arg = new BlogCommentOperation();
                arg.BlogEntryId = day.Objects[0].Id;
                arg.BlogComment = comment;
                arg.OperationType = BlogCommentOperationType.Add;
                Service.BlogCommentOperation(data.Token, arg);
            });

            var dbProfile = Session.Get<Profile>(profile1.Id);
            Assert.AreEqual(1, dbProfile.Statistics.BlogCommentsCount);
        }
        #endregion

        #region Empty training day

        [Test]
        public void TestCreateEmptyTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;

            int count = Session.QueryOver<TrainingDay>().RowCount();

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            Assert.IsNull(day);
            Assert.AreEqual(count, Session.QueryOver<TrainingDay>().RowCount());
        }

        [Test]
        public void TestSaveEmptyTrainingDay()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.Id;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day=Service.SaveTrainingDay(data.Token, day);
            });
            day.Objects.Clear();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                day = Service.SaveTrainingDay(data.Token, day);
            });
            Assert.IsNull(day);
            Assert.AreEqual(0, Session.QueryOver<TrainingDay>().RowCount());
        }
        #endregion
    }
}
