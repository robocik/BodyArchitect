using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SupplementsCycleDefinitionOperation:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                var profile = CreateProfile(Session, "test1");
                profile.Statistics.SupplementEntriesCount = 40;
                profiles.Add(profile);
                profiles.Add(CreateProfile(Session, "test2"));

                tx.Commit();
            }
        }

        #region Delete

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void DeleteCycleDefinition_Public()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        public void DeleteCycleDefinition_NotUsedInAnyPlaces()
        {
            var supp=CreateSupplement("ttt");
            var def=CreateSupplementsCycleDefinition("test", supp, profiles[0],PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            Assert.AreEqual(0,Session.QueryOver<SupplementCycleDefinition>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycleWeek>().RowCount());
            Assert.AreEqual(0, Session.QueryOver<SupplementCycleDosage>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteCycleDefinition_AnotherProfile()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0]);
            var profile1 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }
        #endregion

        #region Favorites
        
        [Test]
        public void AddToFavorites()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            Assert.AreEqual(1, Session.QueryOver<SupplementCycleDefinition>().RowCount());
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1,dbProfile.FavoriteSupplementCycleDefinitions.Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToFavorites_PrivateCycleDefinition()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1],PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToFavorites_MyOwnCycleDefinition()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        public void RemoveFromFavorites()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);

            profiles[0].FavoriteSupplementCycleDefinitions.Add(def);
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            Assert.AreEqual(1, Session.QueryOver<SupplementCycleDefinition>().RowCount());
            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(0, dbProfile.FavoriteSupplementCycleDefinitions.Count);
        }

        [Test]
        [ExpectedException(typeof(ObjectIsFavoriteException))]
        public void AddToFavorites_AlreadyIsInFavorites()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);

            profiles[0].FavoriteSupplementCycleDefinitions.Add(def);
            insertToDatabase(profiles[0]);

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ObjectIsNotFavoriteException))]
        public void RemoveFromFavorites_IsNotInFavorites()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);


            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }
        #endregion

        #region Publish

        [Test]
        [ExpectedException(typeof(ProfileRankException))]
        public void Publish_NotEnoughSupplementEntries()
        {
            profiles[0].Statistics.SupplementEntriesCount = 20;
            insertToDatabase(profiles[0].Statistics);

            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        public void Publish()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0],PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var cycle = Session.QueryOver<SupplementCycleDefinition>().SingleOrDefault();
            Assert.AreEqual(PublishStatus.Published,cycle.Status);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), cycle.PublishDate);
        }

        [Test]
        public void Publish_Statistics()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profile1.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.SupplementsDefinitionsCount);
        }

        [Test]
        public void Publish_Return()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                var res=service.SupplementsCycleDefinitionOperation(data.Token, param);
                Assert.AreEqual(Service.V2.Model.PublishStatus.Published, res.Status);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), res.PublishDate);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Publish_AnotherProfile()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void Publish_AlreadyPublished()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Published);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });
        }
        #endregion

        #region DataInfo

        [Test]
        public void DeleteCycleDefinition_DataInfo()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }

        [Test]
        public void AddToFavorites_DataInfo()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }

        [Test]
        public void RemoveFromFavorites_DataInfo()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);

            profiles[0].FavoriteSupplementCycleDefinitions.Add(def);
            insertToDatabase(profiles[0]);
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            var profile1 = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.RemoveFromFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }

        [Test]
        public void Publish_DataInfo()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(-1);
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Publish;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }

        [Test]
        public void DeleteCycleDefinition_DataInfo_Owner()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[0], PublishStatus.Private);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.Delete;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }

        [Test]
        public void AddToFavorites_DataInfo_Owner()
        {
            var supp = CreateSupplement("ttt");
            var def = CreateSupplementsCycleDefinition("test", supp, profiles[1]);
            var profile1 = (ProfileDTO)profiles[0].Tag;
            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new SupplementsCycleDefinitionOperationParam();
                param.Operation = SupplementsCycleDefinitionOperation.AddToFavorites;
                param.SupplementsCycleDefinitionId = def.GlobalId;
                service.SupplementsCycleDefinitionOperation(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(oldHash, dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }
        #endregion
    }
}
