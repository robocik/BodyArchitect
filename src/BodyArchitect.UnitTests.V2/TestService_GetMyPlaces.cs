using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetMyPlaces : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<MyPlace> myGyms = new List<MyPlace>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                myGyms.Add(Session.QueryOver<MyPlace>().Where(x=>x.Profile==profiles[0]).SingleOrDefault());
                var gym = CreateMyPlace("gym1",profiles[0]);
                myGyms.Add(gym);
                gym = CreateMyPlace("gym5", profiles[0]);
                myGyms.Add(gym);
                gym = CreateMyPlace("gym3", profiles[0]);
                myGyms.Add(gym);
                CreateMyPlace("gym3", profiles[1]);
                gym = CreateMyPlace("gym7", profiles[0]);
                myGyms.Add(gym);

                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            var param = new MyPlaceSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<MyPlaceDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetMyPlaces(data.Token, param, pageInfo);
            });
            Assert.AreEqual(myGyms.Count, result.AllItemsCount);
        }

        [Test]
        public void AnotherUser()
        {
            var param = new MyPlaceSearchCriteria();
            param.ProfileId = profiles[1].GlobalId;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<MyPlaceDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetMyPlaces(data.Token, param, pageInfo);
            });
            Assert.AreEqual(2, result.AllItemsCount);
        }

        [Test]
        public void SortName_Asc()
        {
            var param = new MyPlaceSearchCriteria();
            param.SortAscending = true;
            param.SortOrder = MyPlaceSortOrder.Name;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<MyPlaceDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetMyPlaces(data.Token, param, pageInfo);
            });
            Assert.AreEqual("Default", result.Items[0].Name);
            Assert.AreEqual("gym1", result.Items[1].Name);
            Assert.AreEqual("gym3", result.Items[2].Name);
            Assert.AreEqual("gym5", result.Items[3].Name);
            Assert.AreEqual("gym7", result.Items[4].Name);
        }

        [Test]
        public void SortName_Desc()
        {
            var param = new MyPlaceSearchCriteria();
            param.SortAscending = false;
            param.SortOrder = MyPlaceSortOrder.Name;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<MyPlaceDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetMyPlaces(data.Token, param, pageInfo);
            });
            Assert.AreEqual("gym7", result.Items[0].Name);
            Assert.AreEqual("gym5", result.Items[1].Name);
            Assert.AreEqual("gym3", result.Items[2].Name);
            Assert.AreEqual("gym1", result.Items[3].Name);
            Assert.AreEqual("Default", result.Items[4].Name);
        }

        [Test]
        public void SortCreationDate_Asc()
        {
            var param = new MyPlaceSearchCriteria();
            param.SortAscending = true;
            param.SortOrder = MyPlaceSortOrder.CreationDate;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<MyPlaceDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetMyPlaces(data.Token, param, pageInfo);
            });
            Assert.AreEqual("Default", result.Items[0].Name);
            Assert.AreEqual("gym1", result.Items[1].Name);
            Assert.AreEqual("gym5", result.Items[2].Name);
            Assert.AreEqual("gym3", result.Items[3].Name);
            Assert.AreEqual("gym7", result.Items[4].Name);
        }
    }
}
