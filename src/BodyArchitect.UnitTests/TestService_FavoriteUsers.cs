using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_FavoriteUsers : TestServiceBase
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
                profiles[3].IsDeleted = true;
                Session.Update(profiles[3]);
                tx.Commit();
            }
        }

        #region Statistics

        [Test]
        public void TestAddToFavorites_Statistic()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token,profile2,FavoriteOperation.Add);
            });

            var dbProfile = Session.Get<Profile>(profile2.Id);
            Assert.AreEqual(1, dbProfile.Statistics.FollowersCount);
        }

        [Test]
        public void TestRemoveFromFavorites_Statistic()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token, profile2, FavoriteOperation.Add);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token, profile2, FavoriteOperation.Remove);
            });

            var dbProfile = Session.Get<Profile>(profile2.Id);
            Assert.AreEqual(0, dbProfile.Statistics.FollowersCount);
        }
        #endregion
    }
}
