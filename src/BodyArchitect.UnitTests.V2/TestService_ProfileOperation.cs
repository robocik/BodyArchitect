using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NUnit.Framework;
using AccountType = BodyArchitect.Service.V2.Model.AccountType;
using ChampionshipTryResult = BodyArchitect.Model.ChampionshipTryResult;
using ChampionshipType = BodyArchitect.Model.ChampionshipType;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_ProfileOperation:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private Profile deletedprofile;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));
                deletedprofile = CreateProfile(Session, "(Deleted)");
                deletedprofile.IsDeleted = true;
                insertToDatabase(deletedprofile);
 
                

                //Exercise exercise = new Exercise(Guid.NewGuid());
                //exercise.Profile = profiles[0];
                //exercise.Name = "Private";
                //exercise.Shortcut = "PP";
                //Session.Save(exercise);

                //exercise = new Exercise(Guid.NewGuid());
                //exercise.Profile = profiles[0];
                //exercise.Name = "Public";
                //exercise.Shortcut = "PP";
                //Session.Save(exercise);

                //TrainingDay day = new TrainingDay(DateTime.Now);
                //day.Profile = profiles[0];
                //StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
                //strengthEntry.MyPlace = GetDefaultMyPlace(profiles[0]);
                //StrengthTrainingItem item = new StrengthTrainingItem();
                //strengthEntry.AddEntry(item);
                //item.Exercise = exercise;
                //Serie serie = new Serie("10x20");
                //item.AddSerie(serie);
                //day.AddEntry(strengthEntry);
                //SizeEntry sizeEntry = new SizeEntry();
                //sizeEntry.Wymiary=new Wymiary();
                //sizeEntry.Wymiary.Height = 100;
                //day.AddEntry(sizeEntry);
                //SuplementsEntry suplementsEntry = new SuplementsEntry();
                //SuplementItem suplementItem = new SuplementItem();
                //suplementItem.Suplement = CreateSupplement("supp");
                //suplementItem.Name = "fdgdfg";
                //suplementsEntry.AddItem(suplementItem);
                //day.AddEntry(suplementsEntry);
                //BlogEntry blogEntry= new BlogEntry();
                //day.AddEntry(blogEntry);
                //A6WEntry a6WEntry = new A6WEntry();
                //a6WEntry.DayNumber = 1;
                //a6WEntry.MyTraining = new A6WTraining();
                //a6WEntry.MyTraining.Name = "fdgdfg";
                ////a6WEntry.MyTraining.TypeId = A6WEntry.EntryTypeId;
                //a6WEntry.MyTraining.Profile = profiles[0];
                //day.AddEntry(a6WEntry);
                //Session.Save(day);

                //TrainingDayComment comment = new TrainingDayComment();
                //comment.TrainingDay = day;
                //comment.Profile = profiles[1];
                //comment.Comment = "fgdfgd";
                //day.Comments.Add(comment);
                //Session.Save(comment);
                //comment = new TrainingDayComment();
                //comment.TrainingDay = day;
                //comment.Comment = "fgdfgd";
                //comment.Profile = profiles[0];
                //day.Comments.Add(comment);
                //Session.Save(comment);
                

                //Session.Update(blogEntry);

                

                //RatingUserValue exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[1].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = exercise.GlobalId;
                //Session.Save(exComment);

                //exercise = new Exercise(Guid.NewGuid());
                //exercise.Profile = profiles[1];
                //exercise.Name = "Public1";
                //exercise.Shortcut = "PP1";
                //Session.Save(exercise);

                //exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[0].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = exercise.GlobalId;
                //Session.Save(exComment);

                //TrainingPlan plan = new TrainingPlan();
                //plan.Profile = profiles[0];
                //plan.GlobalId = Guid.NewGuid();
                //plan.Language = "pl";
                //plan.Author = "dfgdfg";
                //plan.Name = "dfgdfg";
                //Session.Save(plan);

                //plan = new TrainingPlan();
                //plan.Profile = profiles[0];
                //plan.Language = "pl";
                //plan.GlobalId = Guid.NewGuid();
                //plan.Author = "dfgdfg";
                //plan.Status = PublishStatus.Published;
                //plan.PublishDate = DateTime.UtcNow;
                //plan.Name = "dfgdfg";
                //Session.Save(plan);

                //exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[1].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = plan.GlobalId;
                //Session.Save(exComment);

                

                //exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[0].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = plan.GlobalId;
                //Session.Save(plan);


                //exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[1].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = plan.GlobalId;
                //Session.Save(exComment);

                //exComment = new RatingUserValue();
                //exComment.ProfileId = profiles[0].GlobalId;
                //exComment.ShortComment = "dffgdfgdf";
                //exComment.Rating = 2;
                //exComment.RatedObjectId = plan.GlobalId;
                //Session.Save(plan);

                

                

                
                
                
                

                
                //Session.Update(profiles[0]);
                //Session.Update(profiles[1]);
                //Session.Update(profiles[2]);
                tx.Commit();
            }
        }

        #region Delete
        [Test]
        public void TestProfileOperation_Delete_RemoveProfile()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var profile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.IsNull(profile);
        }

        [Test]
        public void TestProfileOperation_Delete_LoginData()
        {
            var loginData = new LoginData();
            loginData.ProfileId = profiles[0].GlobalId;
            loginData.LoginDateTime = DateTime.Now;
            loginData.PlatformVersion = "fdgdfg";
            loginData.ApplicationVersion = "1.0";
            loginData.ApplicationLanguage = "pl";
            loginData.ClientInstanceId = Guid.NewGuid();
            insertToDatabase(loginData);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<LoginData>().RowCount();
            Assert.AreEqual(0,count);
        }

        [Test]
        public void TestProfileOperation_Delete_BAPoints()
        {
            var loginData = new LoginData();
            loginData.ProfileId = profiles[0].GlobalId;
            loginData.LoginDateTime = DateTime.Now;
            loginData.PlatformVersion = "fdgdfg";
            loginData.ApplicationVersion = "1.0";
            loginData.ApplicationLanguage = "pl";
            loginData.ClientInstanceId = Guid.NewGuid();
            insertToDatabase(loginData);

            var point = new BAPoints();
            point.Profile = profiles[0];
            point.LoginData = loginData;
            point.Identifier = "gfdgdfg";
            insertToDatabase(point);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<BAPoints>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_RemoveSettings()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            Assert.IsNotNull(profiles[0].Settings);

            var beforeCount = Session.QueryOver<ProfileSettings>().RowCount();
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<ProfileSettings>().RowCount();
            Assert.Less(count, beforeCount);
        }

        [Test]
        public void TestProfileOperation_Delete_RemoveStatistics()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            var beforeCount = Session.QueryOver<ProfileStatistics>().RowCount();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<ProfileStatistics>().RowCount();
            Assert.Less(count, beforeCount);
        }

        [Test]
        public void TestProfileOperation_Delete_FriendsStatistics()
        {
            profiles[0].Friends.Add(profiles[1]);
            profiles[1].Friends.Add(profiles[0]);
            profiles[1].Statistics.FriendsCount = 1;
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var profile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(1, profile.Statistics.FriendsCount);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            profile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(0, profile.Statistics.FriendsCount);
        }

        [Test]
        public void TestProfileOperation_Delete_FavoritesStatistics()
        {
            profiles[0].FavoriteUsers.Add(profiles[1]);
            profiles[1].Statistics.FollowersCount = 1;
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var profile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(1, profile.Statistics.FollowersCount);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            profile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(0, profile.Statistics.FollowersCount);
        }

        [Test]
        public void TestProfileOperation_Delete_FavoritesForAnotherUser()
        {
            profiles[2].FavoriteUsers.Add(profiles[0]);
            insertToDatabase(profiles[2]);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var profile = Session.Get<Profile>(profiles[2].GlobalId);
            Assert.AreEqual(0, profile.FavoriteUsers.Count);
            var count =Session.QueryOver<Message>().Where(x => x.Receiver == profiles[2]).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_DataInfo()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var beforeCount = Session.QueryOver<DataInfo>().RowCount();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<DataInfo>().RowCount();
            Assert.Less(count, beforeCount);
        }

        [Test]
        [ExpectedException(typeof(BodyArchitect.Portable.Exceptions.AuthenticationException))]
        public void TestProfileOperation_Delete_AutomaticLogout()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.GetUsers(data.Token, UserSearchCriteria.CreateAllCriteria(),
                                                      new PartialRetrievingInfo());
                                 });
        }

        [Test]
        public void TestProfileOperation_Delete_RemoveImage()
        {
            //InternalBodyArchitectService tmp = new InternalBodyArchitectService(Session);
            //string folder = tmp.Configuration.ImagesFolder;
            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(ImagesFolder, pictureId.ToString())).Close();
            profiles[0].Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(profiles[0]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            Assert.IsFalse(File.Exists(Path.Combine(ImagesFolder, pictureId.ToString())));
        }

        [Test]
        public void TestProfileOperation_Delete_Invitations()
        {
            FriendInvitation invitation = new FriendInvitation();
            invitation.Invited = profiles[0];
            invitation.Inviter = profiles[2];
            invitation.CreateDate = DateTime.UtcNow;
            Session.Save(invitation);

            invitation = new FriendInvitation();
            invitation.Invited = profiles[3];
            invitation.Inviter = profiles[0];
            invitation.CreateDate = DateTime.UtcNow;
            insertToDatabase(invitation);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<FriendInvitation>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_FavoritePlans()
        {
            var plan = new TrainingPlan();
            plan.Profile = profiles[1];
            plan.Language = "pl";
            //plan.GlobalId = Guid.NewGuid();
            plan.Author = "dfgdfg";
            plan.Status = PublishStatus.Published;
            plan.PublishDate = DateTime.UtcNow;
            plan.Name = "dfgdfg";
            insertToDatabase(plan);
            profiles[0].FavoriteWorkoutPlans.Add(plan);
            insertToDatabase(profiles[0]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var profile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.IsNull(profile);
        }

        [Test]
        public void TestProfileOperation_Delete_Friends()
        {
            profiles[0].Friends.Add(profiles[1]);
            profiles[1].Friends.Add(profiles[0]);
            profiles[1].Statistics.FriendsCount = 1;
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var profile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(0, profile.Friends.Count);
        }

        [Test]
        public void TestProfileOperation_Delete_Messages()
        {
            Message msg = new Message();
            msg.Receiver = profiles[0];
            msg.Sender = profiles[1];
            msg.Topic = "dfgdfgdf";
            insertToDatabase(msg);
            msg = new Message();
            msg.Receiver = profiles[1];
            msg.Sender = profiles[0];
            msg.Topic = "dfgdfgdf";
            insertToDatabase(msg);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<Message>().RowCount();
            Assert.AreEqual(1, count);
            count= Session.QueryOver<Message>().Where(x => x.Receiver == profiles[1]).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_SendMessagesToFriends()
        {
            profiles[0].Friends.Add(profiles[1]);
            profiles[1].Friends.Add(profiles[0]);
            profiles[1].Statistics.FriendsCount = 1;
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<Message>().Where(x => x.Receiver == profiles[1]).RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_CommentsForOtherUsersSupplementsDefinitionCycle()
        {
            var supplement = CreateSupplement("sup");
            var definition = CreateSupplementsCycleDefinition("test", supplement,profiles[1]);


            var exComment = new RatingUserValue();
            exComment.ProfileId = profiles[1].GlobalId;
            exComment.ShortComment = "dffgdfgdf";
            exComment.Rating = 2;
            exComment.RatedObjectId = definition.GlobalId;
            Session.Save(exComment);

            var exCommen1 = new RatingUserValue();
            exCommen1.ProfileId = profiles[0].GlobalId;
            exCommen1.ShortComment = "dffgdfgdf";
            exCommen1.Rating = 2;
            exCommen1.RatedObjectId = definition.GlobalId;
            Session.Save(exCommen1);

            var exComment2 = new RatingUserValue();
            exComment2.ProfileId = profiles[1].GlobalId;
            exComment2.ShortComment = "dffgdfgdf";
            exComment2.Rating = 2;
            exComment2.RatedObjectId = definition.GlobalId;
            Session.Save(exComment2);

            var exComment3 = new RatingUserValue();
            exComment3.ProfileId = profiles[0].GlobalId;
            exComment3.ShortComment = "dffgdfgdf";
            exComment3.Rating = 2;
            exComment3.RatedObjectId = definition.GlobalId;
            insertToDatabase(exComment3);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var planId = definition.GlobalId;
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == planId).RowCount();
            Assert.AreEqual(4, count);
            var dbRating = Session.Get<RatingUserValue>(exCommen1.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
            dbRating = Session.Get<RatingUserValue>(exComment3.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_CommentsForOtherUsersWorkoutPlans()
        {
            var plan = new TrainingPlan();
            plan.Profile = profiles[0];
            plan.Language = "pl";
            plan.GlobalId = Guid.NewGuid();
            plan.Author = "dfgdfg";
            plan.Status = PublishStatus.Published;
            plan.PublishDate = DateTime.UtcNow;
            plan.Name = "dfgdfg";
            Session.Save(plan);

            var exComment = new RatingUserValue();
            exComment.ProfileId = profiles[1].GlobalId;
            exComment.ShortComment = "dffgdfgdf";
            exComment.Rating = 2;
            exComment.RatedObjectId = plan.GlobalId;
            Session.Save(exComment);

            var exCommen1 = new RatingUserValue();
            exCommen1.ProfileId = profiles[0].GlobalId;
            exCommen1.ShortComment = "dffgdfgdf";
            exCommen1.Rating = 2;
            exCommen1.RatedObjectId = plan.GlobalId;
            Session.Save(exCommen1);

            var exComment2 = new RatingUserValue();
            exComment2.ProfileId = profiles[1].GlobalId;
            exComment2.ShortComment = "dffgdfgdf";
            exComment2.Rating = 2;
            exComment2.RatedObjectId = plan.GlobalId;
            Session.Save(exComment2);

            var exComment3 = new RatingUserValue();
            exComment3.ProfileId = profiles[0].GlobalId;
            exComment3.ShortComment = "dffgdfgdf";
            exComment3.Rating = 2;
            exComment3.RatedObjectId = plan.GlobalId;
            insertToDatabase(exComment3);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var planId = plan.GlobalId;
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == planId).RowCount();
            Assert.AreEqual(4, count);
            var dbRating = Session.Get<RatingUserValue>(exCommen1.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
            dbRating = Session.Get<RatingUserValue>(exComment3.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_WorkoutPlans()
        {
            var plan = new TrainingPlan();
            plan.Profile = profiles[1];
            //plan.GlobalId = Guid.NewGuid();
            plan.Language = "pl";
            plan.Author = "dfgdfg";
            plan.Name = "df1gdfg";
            insertToDatabase(plan);

            var plan1 = new TrainingPlan();
            plan1.Profile = profiles[0];
            //plan1.GlobalId = Guid.NewGuid();
            plan1.Language = "pl";
            plan1.Author = "dfgdfg";
            plan1.Name = "dfgdfg";
            insertToDatabase(plan1);

            var plan2 = new TrainingPlan();
            plan2.Profile = profiles[0];
            plan2.Language = "pl";
            //plan2.GlobalId = Guid.NewGuid();
            plan2.Author = "dfgdfg";
            plan2.Status = PublishStatus.Published;
            plan2.PublishDate = DateTime.UtcNow;
            plan2.Name = "dfgdfg";
            insertToDatabase(plan2);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<TrainingPlan>().RowCount();
            Assert.AreEqual(2, count);
            var dbPlan=Session.Get<TrainingPlan>(plan2.GlobalId);
            Assert.AreNotEqual(profiles[0], dbPlan.Profile);
            Assert.IsTrue(dbPlan.Profile.IsDeleted);
            dbPlan = Session.Get<TrainingPlan>(plan.GlobalId);
            Assert.IsNotNull(dbPlan);
            dbPlan = Session.Get<TrainingPlan>(plan2.GlobalId);
            Assert.IsNotNull(dbPlan);
            dbPlan = Session.Get<TrainingPlan>(plan1.GlobalId);
            Assert.IsNull(dbPlan);
        }

        [Test]
        public void TestProfileOperation_Delete_SupplementsCycleDefinitions()
        {
            var supplement=CreateSupplement("Supp");
            var definition1=CreateSupplementsCycleDefinition("test",supplement,profiles[0],PublishStatus.Published);
            var definition2 = CreateSupplementsCycleDefinition("test1", supplement, profiles[0], PublishStatus.Private);


            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<SupplementCycleDefinition>().RowCount();
            Assert.AreEqual(1, count);
            var dbPlan = Session.Get<SupplementCycleDefinition>(definition1.GlobalId);
            Assert.AreEqual(deletedprofile, dbPlan.Profile);
            dbPlan = Session.Get<SupplementCycleDefinition>(definition2.GlobalId);
            Assert.IsNull(dbPlan);
        }

        [Test]
        public void TestProfileOperation_Delete_CommentsForOtherUsersExercises()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex", "ex");
            RatingUserValue exComment = new RatingUserValue();
            exComment.ProfileId = profiles[1].GlobalId;
            exComment.ShortComment = "dffgdfgdf";
            exComment.Rating = 2;
            exComment.RatedObjectId = exercise.GlobalId;
            Session.Save(exComment);

            var exComment1 = new RatingUserValue();
            exComment1.ProfileId = profiles[0].GlobalId;
            exComment1.ShortComment = "dffgdfgdf";
            exComment1.Rating = 2;
            exComment1.RatedObjectId = exercise.GlobalId;
            insertToDatabase(exComment1);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            var exerciseId = exercise.GlobalId;
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exerciseId).RowCount();
            Assert.AreEqual(2, count);
            var dbRating = Session.Get<RatingUserValue>(exComment1.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_CommentsForMyExercises()
        {
            var exercise = CreateExercise(Session, profiles[0], "ex", "ex");
            RatingUserValue exComment = new RatingUserValue();
            exComment.ProfileId = profiles[1].GlobalId;
            exComment.ShortComment = "dffgdfgdf";
            exComment.Rating = 2;
            exComment.RatedObjectId = exercise.GlobalId;
            Session.Save(exComment);

            var exComment1 = new RatingUserValue();
            exComment1.ProfileId = profiles[0].GlobalId;
            exComment1.ShortComment = "dffgdfgdf";
            exComment1.Rating = 2;
            exComment1.RatedObjectId = exercise.GlobalId;
            insertToDatabase(exComment1);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var exerciseId = exercise.GlobalId;
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exerciseId).RowCount();
            Assert.AreEqual(2, count);
            var dbRating = Session.Get<RatingUserValue>(exComment1.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId,dbRating.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_CommentsForSupplements()
        {
            var supplement = CreateSupplement( "ex");
            var loginData = new LoginData();
            loginData.ProfileId = profiles[0].GlobalId;
            loginData.LoginDateTime = DateTime.Now;
            loginData.PlatformVersion = "fdgdfg";
            loginData.ApplicationVersion = "1.0";
            loginData.ApplicationLanguage = "pl";
            loginData.ClientInstanceId = Guid.NewGuid();
            insertToDatabase(loginData);


            RatingUserValue exComment = new RatingUserValue();
            exComment.ProfileId = profiles[1].GlobalId;
            exComment.ShortComment = "dffgdfgdf";
            exComment.Rating = 2;
            exComment.RatedObjectId = supplement.GlobalId;
            Session.Save(exComment);

            var exComment1 = new RatingUserValue();
            exComment1.ProfileId = profiles[0].GlobalId;
            exComment1.LoginData = loginData;
            exComment1.ShortComment = "dffgdfgdf";
            exComment1.Rating = 2;
            exComment1.RatedObjectId = supplement.GlobalId;
            insertToDatabase(exComment1);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var exerciseId = supplement.GlobalId;
            var count = Session.QueryOver<RatingUserValue>().Where(x => x.RatedObjectId == exerciseId).RowCount();
            Assert.AreEqual(2, count);
            var dbRating = Session.Get<RatingUserValue>(exComment1.GlobalId);
            Assert.AreEqual(deletedprofile.GlobalId, dbRating.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_NotUsed_Exercises()
        {
            var ex = CreateExercise(Session, profiles[0], "ex","ex");
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_UsedInPublicTrainingPlan_Exercises()
        {
            var ex = CreateExercise(Session, profiles[0], "ex", "ex");
            var plan = new TrainingPlan();
            plan.Profile = profiles[0];
            //plan.GlobalId = Guid.NewGuid();
            plan.Language = "pl";
            plan.Author = "dfgdfg";
            plan.Name = "df1gdfg";
            plan.Status = PublishStatus.Published;
            TrainingPlanDay day = new TrainingPlanDay();
            day.Name = "test";
            day.TrainingPlan = plan;
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = ex;
            entry.Day = day;
            day.Entries.Add(entry);
            plan.Days.Add(day);
            insertToDatabase(plan);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_OtherUserStrengthTraining_Exercises()
        {
            var ex = CreateExercise(Session, profiles[0], "ex", "ex");
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[1];
            StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
            strengthEntry.MyPlace = GetDefaultMyPlace(profiles[1]);
            StrengthTrainingItem item = new StrengthTrainingItem();
            strengthEntry.AddEntry(item);
            item.Exercise = ex;
            Serie serie = new Serie("10x20");
            item.AddSerie(serie);
            day.AddEntry(strengthEntry);
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_OtherUserPrivatePlan_Exercises()
        {
            var ex = CreateExercise(Session, profiles[0], "ex", "ex");
            var plan = new TrainingPlan();
            plan.Profile = profiles[1];
            //plan.GlobalId = Guid.NewGuid();
            plan.Language = "pl";
            plan.Author = "dfgdfg";
            plan.Name = "df1gdfg";
            TrainingPlanDay day = new TrainingPlanDay();
            day.Name = "test";
            day.TrainingPlan = plan;
            TrainingPlanEntry entry = new TrainingPlanEntry();
            entry.Exercise = ex;
            entry.Day = day;
            day.Entries.Add(entry);
            plan.Days.Add(day);
            insertToDatabase(plan);


            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestProfileOperation_Delete_CrossProfileException()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile2.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
        }

        [Test]
        public void TestProfileOperation_Delete_StrengthTrainingEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
            strengthEntry.MyPlace = GetDefaultMyPlace(profiles[0]);
            StrengthTrainingItem item = new StrengthTrainingItem();
            strengthEntry.AddEntry(item);
            item.Exercise = CreateExercise(Session,null,"ex1","ex");
            Serie serie = new Serie("10x20");
            item.AddSerie(serie);
            day.AddEntry(strengthEntry);
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<StrengthTrainingEntry>().RowCount();
            Assert.AreEqual(0,count);
            count = Session.QueryOver<StrengthTrainingItem>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Serie>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ProfileAndWymiary()
        {
            profiles[0].Wymiary = new Wymiary();
            profiles[0].Wymiary.Pas = 102;
            Session.SaveOrUpdate(profiles[0]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            var count = Session.QueryOver<Profile>().Where(x=>x.UserName!="admin" && !x.IsDeleted).RowCount();
            Assert.AreEqual(4, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var profile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.IsNull(profile);
            count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(0, count);
            
        }

        [Test]
        public void TestProfileOperation_Delete_CustomerReminderItems()
        {
            CreateReminder("test",profiles[0],DateTime.UtcNow.AddDays(-1));
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_EntryObjectReminderItems()
        {
            var supplement = CreateSupplement("test");
            var reminder=CreateReminder("test", profiles[0], DateTime.UtcNow.AddDays(-1));
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SuplementsEntry entry = new SuplementsEntry();
            entry.Reminder = reminder;
            day.AddEntry(entry);
            entry.AddItem(new SuplementItem() { Suplement = supplement });
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_MyPlace()
        {
            var myPlace=GetDefaultMyPlace(profiles[0]);
            myPlace.Address = new Address();
            insertToDatabase(myPlace);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0]).RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0]).RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_MyPlaceUsedInTrainingDay()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            StrengthTrainingEntry strengthEntry = new StrengthTrainingEntry();
            strengthEntry.MyPlace = GetDefaultMyPlace(profiles[0]);
            StrengthTrainingItem item = new StrengthTrainingItem();
            strengthEntry.AddEntry(item);
            item.Exercise = CreateExercise(Session, null, "ex1", "ex");
            Serie serie = new Serie("10x20");
            item.AddSerie(serie);
            day.AddEntry(strengthEntry);
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0]).RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_A6WEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            A6WEntry a6WEntry = new A6WEntry();
            a6WEntry.DayNumber = 1;
            a6WEntry.MyTraining = new A6WTraining();
            a6WEntry.MyTraining.Name = "fdgdfg";
            //a6WEntry.MyTraining.TypeId = A6WEntry.EntryTypeId;
            a6WEntry.MyTraining.Profile = profiles[0];
            day.AddEntry(a6WEntry);
            insertToDatabase(day);


            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<A6WEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<MyTraining>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_SupplementCycle()
        {
            var supplement = CreateSupplement("supp");
            var cycle = new SupplementCycle();
            cycle.SupplementsCycleDefinition = CreateSupplementsCycleDefinition("cycle", supplement, profiles[1]);
            cycle.Profile = profiles[0];
            cycle.Name = "test";
            cycle.StartDate = DateTime.UtcNow;
            insertToDatabase(cycle);

            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SuplementsEntry entry = new SuplementsEntry();
            day.AddEntry(entry);
            entry.AddItem(new SuplementItem(){Suplement=supplement});
            entry.MyTraining=cycle;
            insertToDatabase(day);


            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<SuplementsEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<SupplementCycle>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<MyTraining>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_SizeEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SizeEntry sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Height = 100;
            day.AddEntry(sizeEntry);
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            var count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<SizeEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_WP7PushNotification()
        {
            WP7PushNotification push = new WP7PushNotification();
            push.DeviceID = Guid.NewGuid().ToString();
            push.URI = "fgfdgfdg";
            push.ProfileId = profiles[0].GlobalId;
            insertToDatabase(push);
            push = new WP7PushNotification();
            push.URI = "fgfdg1fdg";
            push.ProfileId = profiles[1].GlobalId;
            push.DeviceID = Guid.NewGuid().ToString();
            insertToDatabase(push);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<WP7PushNotification>().RowCount();
            Assert.AreEqual(2, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<WP7PushNotification>().RowCount();
            Assert.AreEqual(1, count);
            var dbPush = Session.QueryOver<WP7PushNotification>().SingleOrDefault();
            Assert.AreEqual(profiles[1].GlobalId, dbPush.ProfileId);
        }

        [Test]
        public void TestProfileOperation_Delete_SupplementEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            SuplementsEntry suplementsEntry = new SuplementsEntry();
            SuplementItem suplementItem = new SuplementItem();
            suplementItem.Suplement = CreateSupplement("supp");
            suplementItem.Name = "fdgdfg";
            suplementsEntry.AddItem(suplementItem);
            day.AddEntry(suplementsEntry);

            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<SuplementsEntry>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<SuplementsEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<SuplementItem>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ExerciseProfileData()
        {
            var exercise = CreateExercise(Session, null, "test","test");
            var trainingDay = new TrainingDay(DateTime.Now);
            trainingDay.Profile = profiles[0];
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            insertToDatabase(trainingDay);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(1, 22),trainingDay.TrainingDate);


            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<ExerciseProfileData>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<ExerciseProfileData>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_BlogEntry()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            BlogEntry blogEntry = new BlogEntry();
            day.AddEntry(blogEntry);
            insertToDatabase(day);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<BlogEntry>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<BlogEntry>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_OtherTrainingDay_Comments()
        {
            var loginData = new LoginData();
            loginData.ProfileId = profiles[0].GlobalId;
            loginData.LoginDateTime = DateTime.Now;
            loginData.PlatformVersion = "fdgdfg";
            loginData.ApplicationVersion = "1.0";
            loginData.ApplicationLanguage = "pl";
            loginData.ClientInstanceId = Guid.NewGuid();
            insertToDatabase(loginData);

            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[1];
            BlogEntry blogEntry = new BlogEntry();
            day.AddEntry(blogEntry);
            insertToDatabase(day);
            TrainingDayComment comment = new TrainingDayComment();
            comment.TrainingDay = day;
            comment.Profile = profiles[1];
            comment.Comment = "fgdfgd";
            Session.Save(comment);
            comment = new TrainingDayComment();
            comment.TrainingDay = day;
            comment.Comment = "fgdfgd";
            comment.Profile = profiles[0];
            comment.LoginData = loginData;
            insertToDatabase(comment);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<TrainingDayComment>().RowCount();
            Assert.AreEqual(2, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<TrainingDayComment>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestProfileOperation_Delete_MyTrainingDay_Comments()
        {
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            BlogEntry blogEntry = new BlogEntry();
            day.AddEntry(blogEntry);
            insertToDatabase(day);
            TrainingDayComment comment = new TrainingDayComment();
            comment.TrainingDay = day;
            comment.Profile = profiles[1];
            comment.Comment = "fgdfgd";
            Session.Save(comment);
            comment = new TrainingDayComment();
            comment.TrainingDay = day;
            comment.Comment = "fgdfgd";
            comment.Profile = profiles[0];
            insertToDatabase(comment);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(1, count);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });

            count = Session.QueryOver<TrainingDayComment>().RowCount();
            Assert.AreEqual(0, count);
        }

        #region Instructor objects

        [Test]
        public void TestProfileOperation_Delete_Championship()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var customer = CreateCustomer("cust", profiles[0]);
            customer.Address = new Address();
            customer.Wymiary = new Wymiary();
            insertToDatabase(customer);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var championship=CreateChampionshipEx(profiles[0], "champ1",ChampionshipType.ZawodyWyciskanieSztangi,ScheduleEntryState.Done,DateTime.Now,new ChampionshipCategory());
            CreateReservation(championship, customer);
            ChampionshipGroup championshipGroup = new ChampionshipGroup();
            championshipGroup.Name = "group";
            championship.Groups.Add(championshipGroup);
            
            addCustomerEntries(championship, customer, 100, 100, 200, 120, championshipGroup);
            insertToDatabase(championship);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<Championship>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ChampionshipCategory>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ChampionshipCustomer>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ChampionshipGroup>().RowCount();
            Assert.AreEqual(0, count);

            count = Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(3, count);

        }

        [Test]
        public void TestProfileOperation_Delete_Championship_AnotherUser()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var customer = CreateCustomer("cust", profiles[0]);
            customer.Address = new Address();
            customer.Wymiary = new Wymiary();
            insertToDatabase(customer);

            ProfileDTO profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var championship = CreateChampionshipEx(profiles[0], "champ1", ChampionshipType.ZawodyWyciskanieSztangi, ScheduleEntryState.Done, DateTime.Now, new ChampionshipCategory());
            CreateReservation(championship, customer);
            ChampionshipGroup championshipGroup = new ChampionshipGroup();
            championshipGroup.Name = "group";
            championship.Groups.Add(championshipGroup);
            
            addCustomerEntries(championship, customer, 100, 100, 200, 120, championshipGroup);
            insertToDatabase(championship);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<Championship>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<ChampionshipCategory>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<ChampionshipCustomer>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<ChampionshipGroup>().RowCount();
            Assert.AreEqual(1, count);

            count = Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(3, count);
            count = Session.QueryOver<Exercise>().RowCount();
            Assert.AreEqual(3, count);

        }

        ChampionshipCustomer addCustomerEntries(Championship champ, Customer customer, decimal weight, decimal ex1Weight, decimal ex2Weight, decimal ex3Weight, ChampionshipGroup customerGroup = null)
        {

            var benchPress = Session.Get<Exercise>(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = Session.Get<Exercise>(new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = Session.Get<Exercise>(new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male = new ChampionshipCustomer();
            male.Customer = customer;
            male.Weight = weight;
            male.Group = customerGroup;
            champ.Customers.Add(male);

            var entry = new ChampionshipEntry();
            entry.Exercise = benchPress;
            entry.Customer = male;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = ex1Weight;
            champ.Entries.Add(entry);

            var entry1 = new ChampionshipEntry();
            entry1.Exercise = deadlift;
            entry1.Customer = male;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = ex2Weight;
            champ.Entries.Add(entry1);

            var entry2 = new ChampionshipEntry();
            entry2.Exercise = sqad;
            entry2.Customer = male;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = ex3Weight;
            champ.Entries.Add(entry2);

            return male;
        }


        [Test]
        public void TestProfileOperation_Delete_Customer()
        {
            var customer=CreateCustomer("cust",profiles[0]);
            customer.Address=new Address();
            customer.Wymiary=new Wymiary();
            insertToDatabase(customer);
            CreateCustomer("cust", profiles[1]);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Customer>().RowCount();
            Assert.AreEqual(2, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Customer>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(0, count);

        }

        [Test]
        public void TestProfileOperation_Delete_CustomerGroup()
        {
            CreateCustomerGroup("cust", profiles[0]);
            CreateCustomerGroup("cust", profiles[1]);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(2, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_CustomerGroupWithCustomer()
        {
            var customer = CreateCustomer("test", profiles[0]);
            CreateCustomerGroup("cust", profiles[0], 10, customer);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Customer>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_Activity()
        {
            CreateActivity("cust", profiles[0]);
            CreateActivity("cust", profiles[1]);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(2, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void TestProfileOperation_Delete_CustomerGroupWithDefaultActivity()
        {
            var activity = CreateActivity("test", profiles[0]);
            var group=CreateCustomerGroup("cust", profiles[0]);
            group.DefaultActivity = activity;
            insertToDatabase(group);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ScheduleEntry_WithActivityAndGroup()
        {
            var group = CreateCustomerGroup("cust", profiles[0]);
            var activity = CreateActivity("test", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Activity = activity;
            entry.CustomerGroup = group;
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.Now;
            entry.EndTime = DateTime.Now.AddHours(1);
            insertToDatabase(entry);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<ScheduleEntry>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ScheduleEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<MyPlace>().Where(x=>x.Profile==profiles[0]).RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ScheduleReservationEntry()
        {
            var customer = CreateCustomer("cust",profiles[0]);
            var activity = CreateActivity("test", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Activity = activity;
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.Now;
            entry.EndTime = DateTime.Now.AddHours(1);
            insertToDatabase(entry);

            ScheduleEntryReservation reservation= new ScheduleEntryReservation();
            reservation.Customer = customer;
            reservation.Name = "test";
            reservation.DateTime = DateTime.Now;
            reservation.Profile = profiles[0];
            reservation.ScheduleEntry = entry;
            insertToDatabase(reservation);
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<ScheduleEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ScheduleReservationEntry_WithTrainingDay()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var activity = CreateActivity("test", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Activity = activity;
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.Now;
            entry.EndTime = DateTime.Now.AddHours(1);
            insertToDatabase(entry);

            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Customer = customer;
            reservation.Name = "test";
            reservation.DateTime = DateTime.Now;
            reservation.Profile = profiles[0];
            reservation.ScheduleEntry = entry;
            insertToDatabase(reservation);

            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            day.Customer = customer;
            SizeEntry size = new SizeEntry();
            size.Reservation = reservation;
            reservation.EntryObjects.Add(size);
            day.AddEntry(size);
            insertToDatabase(day);
            insertToDatabase(reservation);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);


            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            var count = Session.QueryOver<ScheduleEntry>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<TrainingDay>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void TestProfileOperation_Delete_ScheduleReservationEntry_WithPayment()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var activity = CreateActivity("test", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Activity = activity;
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.Now;
            entry.EndTime = DateTime.Now.AddHours(1);
            insertToDatabase(entry);

            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Customer = customer;
            reservation.Name = "test";
            reservation.DateTime = DateTime.Now;
            reservation.Profile = profiles[0];
            reservation.ScheduleEntry = entry;
            insertToDatabase(reservation);

            CreatePayment(reservation, profiles[0]);

            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            var count = Session.QueryOver<Payment>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<PaymentBasket>().RowCount();
            Assert.AreEqual(1, count);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile1.GlobalId;
                d.Operation = ProfileOperation.Delete;
                service.ProfileOperation(data.Token, d);
            });
            count = Session.QueryOver<Payment>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<PaymentBasket>().RowCount();
            Assert.AreEqual(0, count);
        }
        #endregion
        #endregion

        [Test]
        public void SetStatus()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var dbOldProfile = Session.Get<Profile>(profile.GlobalId);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.SetStatus;
                d.Status = new ProfileStatusDTO() { Status = "New status" };
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual("New status", dbProfile.Statistics.Status.Status);
            //changing status should change a version
            Assert.AreEqual(dbOldProfile.Version, dbProfile.Version);
        }

        #region ChangeAccountType
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeAccountType_FromUserToAdministrator()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Administrator;
                service.ProfileOperation(data.Token, d);
            });
        }


        [Test]
        public void ChangeAccountType_FromInstructorToUser_TwoClients()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            SessionData data1 = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.User;
                service.ProfileOperation(data.Token, d);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     var param = new GetProfileInformationCriteria();
                                     param.UserId = profile.GlobalId;
                                     var info1=service.GetProfileInformation(data.Token, param);
                                     Assert.AreEqual(AccountType.User, info1.Licence.AccountType);
                                     Assert.AreEqual(AccountType.User, info1.Licence.CurrentAccountType);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProfileInformationCriteria();
                param.UserId = profile.GlobalId;
                var info1 = service.GetProfileInformation(data1.Token, param);
                Assert.AreEqual(AccountType.User, info1.Licence.AccountType);
                Assert.AreEqual(AccountType.User, info1.Licence.CurrentAccountType);
            });
        }

        [Test]
        public void ChangeAccountType_FromUserToPremium_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    ((MockTimerService) service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.PremiumUser;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(9, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromUserToInstructor_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(8, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromPremiumToInstructor_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(9, dbProfile.Licence.BAPoints);//we get only 1 point because 1 point has been already taken for premium account
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromInstructorToPremium_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.PremiumUser;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(15, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_Promotion_FromInstructorToPremium_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.PremiumUser;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(15, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_Promotion_FromUserToInstructor_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_Promotion_FromUserToPremium_ButLoggedAsInstructorFromPromotion_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.PremiumUser;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_Promotion_FromPremiumToInstructor_BothHavePromotion_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 5,PromotionPoints = 4,PromotionStartDate = DateTime.UtcNow.Date} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 3 ,PromotionPoints =1,PromotionStartDate = DateTime.UtcNow.Date} },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(27, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_Promotion_FromPremiumToInstructor_OnePromotionIsInFuture_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 5,PromotionPoints = 4,PromotionStartDate = DateTime.UtcNow.AddDays(2)} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 3 ,PromotionPoints =1,PromotionStartDate = DateTime.UtcNow.Date} },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(26, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.Instructor, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromInstructorToUser_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.User;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.User, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromPremiumToUser_EnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.User;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(15, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.User, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_FromInstructorToUser_NotEnoughBAPoints()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 29;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.User;
                service.ProfileOperation(data.Token, d);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
            Assert.AreEqual(Model.AccountType.User, dbProfile.Licence.AccountType);
        }

        [Test]
        public void ChangeAccountType_SecuriyManagerUpdated_AfterChanges()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 30;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.User;
                service.ProfileOperation(data.Token, d);

                var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
                Assert.AreEqual(AccountType.User, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(15, securityInfo.Licence.BAPoints);
            });

        }

        [Test]
        public void ChangeAccountType_SecuriyManagerUpdated_AfterException()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            profiles[0].Licence.BAPoints = 1;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                var d = new ProfileOperationParam();
                d.ProfileId = profile.GlobalId;
                d.Operation = ProfileOperation.AccountType;
                d.AccountType = AccountType.Instructor;
                try
                {
                    service.ProfileOperation(data.Token, d);
                }
                catch (Exception)
                {
                }
                
            });

            var securityInfo = SecurityManager.EnsureAuthentication(data.Token);
            Assert.AreEqual(AccountType.User, securityInfo.Licence.CurrentAccountType);
            Assert.AreEqual(1, securityInfo.Licence.BAPoints);
        }
        #endregion
    }
}
