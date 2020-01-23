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
    public class TestService_GetSupplements:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Suplement> supplements = new List<Suplement>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                supplements.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var supplement = CreateSupplement("sup1");
                supplements.Add(supplement);
                supplement = CreateSupplement("sup2");
                supplements.Add(supplement);
                supplement = CreateSupplement("sup3",true);
                supplements.Add(supplement);
                supplement = CreateSupplement("sup4", false,true);
                supplements.Add(supplement);
                tx.Commit();
            }
        }

        void assert(PagedResult<SuplementDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.AllItemsCount);
            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == supplements[index].GlobalId).SingleOrDefault());
                }
            }

        }

        [Test]
        public void All()
        {
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetSuplements(data.Token,new GetSupplementsParam(), new PartialRetrievingInfo());

                Assert.AreEqual(supplements.Count, list.Items.Count);
            });
        }

        [Test]
        public void OnlyProducts()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetSupplementsParam();
                param.ProductCriteria = SupplementProductCriteria.OnlyProducts;
                var list = service.GetSuplements(data.Token, param, new PartialRetrievingInfo());

                assert(list,2);
            });
        }

        [Test]
        public void OnlyIllegal()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetSupplementsParam();
                param.LegalCriteria = CanBeIllegalCriteria.OnlyIllegal;
                var list = service.GetSuplements(data.Token, param, new PartialRetrievingInfo());

                assert(list, 3);
            });
        }

        [Test]
        public void OnlyLegal()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetSupplementsParam();
                param.LegalCriteria = CanBeIllegalCriteria.OnlyLegal;
                var list = service.GetSuplements(data.Token, param, new PartialRetrievingInfo());

                assert(list, 0, 1, 2);
            });
        }

        [Test]
        public void OnlyGeneral()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetSupplementsParam();
                param.ProductCriteria = SupplementProductCriteria.OnlyGeneral;
                var list = service.GetSuplements(data.Token, param, new PartialRetrievingInfo());

                assert(list, 0,1,3);
            });
        }

        [Test]
        public void GetUserRating()
        {
            RatingUserValue rating = new RatingUserValue();
            rating.RatedObjectId = supplements[0].GlobalId;
            rating.Rating = 2;
            rating.ProfileId = profiles[0].GlobalId;
            rating.VotedDate = DateTime.UtcNow;
            insertToDatabase(rating);
             rating = new RatingUserValue();
            rating.RatedObjectId = supplements[0].GlobalId;
            rating.Rating = 5;
            rating.ProfileId = profiles[1].GlobalId;
            rating.VotedDate = DateTime.UtcNow;
            insertToDatabase(rating);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetSuplements(data.Token, new GetSupplementsParam(), new PartialRetrievingInfo());
                Assert.AreEqual(2,list.Items.Where(x=>x.GlobalId==supplements[0].GlobalId).Single().UserRating);
                Assert.AreEqual(null, list.Items.Where(x => x.GlobalId == supplements[1].GlobalId).Single().UserRating);
            });
        }

        //TODO:Add unit tests for sort order
    }
}
