using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
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
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token,profile2,FavoriteOperation.Add);
            });

            var dbProfile = Session.Get<Profile>(profile2.GlobalId);
            Assert.AreEqual(1, dbProfile.Statistics.FollowersCount);
        }

        [Test]
        public void TestRemoveFromFavorites_Statistic()
        {
            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
            ProfileDTO profile2 = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile1, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token, profile2, FavoriteOperation.Add);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.UserFavoritesOperation(data.Token, profile2, FavoriteOperation.Remove);
            });

            var dbProfile = Session.Get<Profile>(profile2.GlobalId);
            Assert.AreEqual(0, dbProfile.Statistics.FollowersCount);
        }
        #endregion
    }
}
