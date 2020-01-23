using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_DeleteProfile:TestServiceBase
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
 
                profiles[0].Wymiary=new Wymiary();
                profiles[0].Wymiary.Pas = 102;

                Session.SaveOrUpdate(profiles[0]);

                

                TrainingDay day = new TrainingDay(DateTime.Now);
                day.Profile = profiles[0];
                StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
                StrengthTrainingItem item = new StrengthTrainingItem();
                strengthEntry.AddEntry(item);
                item.ExerciseId=Guid.NewGuid();
                Serie serie = new Serie("10x20");
                item.AddSerie(serie);
                day.AddEntry(strengthEntry);
                SizeEntry sizeEntry = new SizeEntry();
                sizeEntry.Wymiary=new Wymiary();
                sizeEntry.Wymiary.Height = 100;
                day.AddEntry(sizeEntry);
                SuplementsEntry suplementsEntry = new SuplementsEntry();
                SuplementItem suplementItem = new SuplementItem();
                suplementItem.SuplementId = Guid.NewGuid();
                suplementItem.Name = "fdgdfg";
                suplementsEntry.AddItem(suplementItem);
                day.AddEntry(suplementsEntry);
                BlogEntry blogEntry= new BlogEntry();
                day.AddEntry(blogEntry);
                A6WEntry a6WEntry = new A6WEntry();
                a6WEntry.DayNumber = 1;
                a6WEntry.MyTraining= new MyTraining();
                a6WEntry.MyTraining.Name = "fdgdfg";
                a6WEntry.MyTraining.TypeId = A6WEntry.EntryTypeId;
                a6WEntry.MyTraining.Profile = profiles[0];
                day.AddEntry(a6WEntry);
                Session.Save(day);

                BlogComment comment = new BlogComment();
                comment.BlogEntry = blogEntry;
                comment.Profile = profiles[1];
                comment.Comment = "fgdfgd";
                Session.Save(comment);
                comment = new BlogComment();
                comment.BlogEntry = blogEntry;
                comment.Comment = "fgdfgd";
                comment.Profile = profiles[0];
                Session.Save(comment);

                Session.Update(blogEntry);

                Exercise exercise=new Exercise(Guid.NewGuid());
                exercise.Profile = profiles[0];
                exercise.Name = "Private";
                exercise.Shortcut = "PP";
                Session.Save(exercise);

                exercise = new Exercise(Guid.NewGuid());
                exercise.Profile = profiles[0];
                exercise.Name = "Public";
                exercise.Shortcut = "PP";
                exercise.Status = PublishStatus.Published;
                exercise.PublishDate = DateTime.UtcNow;
                Session.Save(exercise);

                RatingUserValue exComment = new RatingUserValue();
                exComment.ProfileId = profiles[1].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = exercise.GlobalId;
                Session.Save(exComment);

                exercise = new Exercise(Guid.NewGuid());
                exercise.Profile = profiles[1];
                exercise.Name = "Public1";
                exercise.Shortcut = "PP1";
                exercise.Status = PublishStatus.Published;
                exercise.PublishDate = DateTime.UtcNow;
                Session.Save(exercise);

                exComment = new RatingUserValue();
                exComment.ProfileId = profiles[0].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = exercise.GlobalId;
                Session.Save(exComment);

                TrainingPlan plan = new TrainingPlan();
                plan.Profile = profiles[0];
                plan.GlobalId = Guid.NewGuid();
                plan.Language = "pl";
                plan.Author = "dfgdfg";
                plan.Name = "dfgdfg";
                plan.PlanContent = "fgdgdfg";
                Session.Save(plan);

                plan = new TrainingPlan();
                plan.Profile = profiles[0];
                plan.Language = "pl";
                plan.GlobalId = Guid.NewGuid();
                plan.Author = "dfgdfg";
                plan.Status = PublishStatus.Published;
                plan.PublishDate = DateTime.UtcNow;
                plan.Name = "dfgdfg";
                plan.PlanContent = "fgdgdfg";
                Session.Save(plan);

                exComment = new RatingUserValue();
                exComment.ProfileId = profiles[1].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = plan.GlobalId;
                Session.Save(exComment);

                plan = new TrainingPlan();
                plan.Profile = profiles[1];
                plan.Language = "pl";
                plan.GlobalId = Guid.NewGuid();
                plan.Author = "dfgdfg";
                plan.Status = PublishStatus.Published;
                plan.PublishDate = DateTime.UtcNow;
                plan.Name = "dfgdfg";
                plan.PlanContent = "fgdgdfg";
                Session.Save(plan);

                exComment = new RatingUserValue();
                exComment.ProfileId = profiles[0].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = plan.GlobalId;
                Session.Save(plan);


                exComment = new RatingUserValue();
                exComment.ProfileId = profiles[1].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = plan.GlobalId;
                Session.Save(exComment);

                exComment = new RatingUserValue();
                exComment.ProfileId = profiles[0].Id;
                exComment.ShortComment = "dffgdfgdf";
                exComment.Rating = 2;
                exComment.RatedObjectId = plan.GlobalId;
                Session.Save(plan);

                Message msg = new Message();
                msg.Receiver = profiles[0];
                msg.Sender = profiles[1];
                msg.Topic = "dfgdfgdf";
                Session.Save(msg);
                msg = new Message();
                msg.Receiver = profiles[1];
                msg.Sender = profiles[0];
                msg.Topic = "dfgdfgdf";
                Session.Save(msg);

                FriendInvitation invitation = new FriendInvitation();
                invitation.Invited = profiles[0];
                invitation.Inviter = profiles[2];
                invitation.CreateDate = DateTime.UtcNow;
                Session.Save(invitation);

                invitation = new FriendInvitation();
                invitation.Invited = profiles[3];
                invitation.Inviter = profiles[0];
                invitation.CreateDate = DateTime.UtcNow;
                Session.Save(invitation);

                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                profiles[1].Statistics.FriendsCount = 1;
                
                profiles[0].FavoriteUsers.Add(profiles[1]);
                profiles[1].Statistics.FollowersCount = 1;
                profiles[0].FavoriteWorkoutPlans.Add(plan);

                profiles[2].FavoriteUsers.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);
                Session.Update(profiles[2]);
                tx.Commit();
            }
        }

        [Test]
        public void TestDeleteProfile_RemoveSettings()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[0].Id);
            Assert.IsNull(profile.Settings);
        }

        [Test]
        public void TestDeleteProfile_RemoveStatistics()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[0].Id);
            Assert.IsNull(profile.Statistics);
        }

        [Test]
        public void TestDeleteProfile_FriendsStatistics()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[1].Id);
            Assert.AreEqual(0, profile.Statistics.FriendsCount);
        }

        [Test]
        public void TestDeleteProfile_FavoritesStatistics()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[1].Id);
            Assert.AreEqual(0, profile.Statistics.FollowersCount);
        }

        [Test]
        public void TestDeleteProfile_FavoritesForAnotherUser()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[2].Id);
            Assert.AreEqual(0, profile.FavoriteUsers.Count);
            var count =Session.QueryOver<Message>().Where(x => x.Receiver == profiles[2]).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        [ExpectedException(typeof(AuthenticationException))]
        public void TestDeleteProfile_AutomaticLogout()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.GetUsers(data.Token, UserSearchCriteria.CreateAllCriteria(),
                                                      new PartialRetrievingInfo());
                                 });
        }

        [Test]
        public void TestDeleteProfile_RemoveImage()
        {
            InternalBodyArchitectService tmp = new InternalBodyArchitectService(Session);
            string folder = tmp.ImagesFolder;
            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(folder, pictureId.ToString())).Close();
            profiles[0].Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(profiles[0]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[0].Id);
            Assert.AreEqual(0, profile.FavoriteUsers.Count);
            Assert.AreEqual(0, profile.FavoriteWorkoutPlans.Count);

            Assert.IsFalse(File.Exists(Path.Combine(folder, pictureId.ToString())));
        }

        [Test]
        public void TestDeleteProfile_Invitations()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDeleteProfile_Favorites()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[0].Id);
            Assert.AreEqual(0, profile.FavoriteUsers.Count);
            Assert.AreEqual(0, profile.FavoriteWorkoutPlans.Count);
        }

        [Test]
        public void TestDeleteProfile_Friends()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[1].Id);
            Assert.AreEqual(0, profile.Friends.Count);
        }

        [Test]
        public void TestDeleteProfile_Messages()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(3, count);
            count= Session.QueryOver<Message>().Where(x => x.Receiver == profiles[1]).RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestDeleteProfile_SendMessagesToFriends()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[1]).RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestDeleteProfile_CommentsForOtherUsersWorkoutPlans()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var exercise = Session.QueryOver<TrainingPlan>().Where(x => x.Profile == profiles[1]).SingleOrDefault();
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exercise.GlobalId).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeleteProfile_WorkoutPlans()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<TrainingPlan>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestDeleteProfile_CommentsForOtherUsersExercises()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var exercise = Session.QueryOver<Exercise>().Where(x => x.Profile == profiles[1]).SingleOrDefault();
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exercise.GlobalId).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeleteProfile_CommentsForMyExercises()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var exercise = Session.QueryOver<Exercise>().Where(x => x.Profile == profiles[0]).SingleOrDefault();
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exercise.GlobalId).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestDeleteProfile_Exercises()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestDeleteProfile_CrossProfileException()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token,profile2);
            });
        }

        [Test]
        public void TestDeleteProfile_StrengthTrainingEntry()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<StrengthTrainingEntry>().RowCount();
            Assert.AreEqual(0,count);
            count = Session.QueryOver<StrengthTrainingItem>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Serie>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDeleteProfile_ProfileAndWymiary()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            var count = Session.QueryOver<Profile>().Where(x=>x.UserName!="admin").RowCount();
            Assert.AreEqual(4, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var profile = Session.Get<Profile>(profiles[0].Id);
            Assert.AreEqual(true, profile.IsDeleted);
            count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(0, count);
            
        }

        [Test]
        public void TestDeleteProfile_A6WEntry()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            var count = Session.QueryOver<A6WEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<MyTraining>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDeleteProfile_SizeEntry()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);
            var count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(2, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });
            count = Session.QueryOver<SizeEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestDeleteProfile_BlogEntry()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<BlogEntry>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<BlogComment>().RowCount();
            Assert.AreEqual(2, count);
            var list=Session.QueryOver<BlogComment>().List();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteProfile(data.Token, profile1);
            });

            count = Session.QueryOver<BlogEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<BlogComment>().RowCount();
            Assert.AreEqual(0, count);
        }
    }
}
