using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_UserFavorites:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        public override void BuildDatabase()
        {
            

            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                var profile=CreateProfile(Session, "test1");
                profiles.Add(profile);
                profile = CreateProfile(Session, "test2");
                profiles.Add(profile);
                profile = CreateProfile(Session, "test3");
                profiles.Add(profile);
                tx.Commit();
            }

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddYourselfToFavorites_Exception()
        {
            ProfileDTO profile1=(ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Add);
            });
        }

        [Test]
        public void TestAddToFavorites()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile2, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Add);
            });

            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var info = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(1, info.FavoriteUsers.Count);
                Assert.AreEqual(profile1.GlobalId, info.FavoriteUsers[0].GlobalId);
            });
            
        }

        [Test]
        [ExpectedException(typeof(ObjectIsNotFavoriteException))]
        public void TestRemoveFromFavorites_UserWhichIsNotFavorite_Exception()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Remove);
            });
        }

        [Test]
        [ExpectedException(typeof(ObjectIsFavoriteException))]
        public void TestAddToFavorites_SecondTime_Exception()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Add);
            });
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Add);
            });
        }

        [Test]
        public void TestRemoveFromFavorites()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile2, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Add);
            });
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.UserFavoritesOperation(data.Token, profile1, FavoriteOperation.Remove);
            });
            GetProfileInformationCriteria criteria = new GetProfileInformationCriteria();
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var info = Service.GetProfileInformation(data.Token, criteria);
                Assert.AreEqual(0, info.FavoriteUsers.Count);
            });
            
        }
    }
}
