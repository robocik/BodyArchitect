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
    public class TestService_GetChampionships : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                
                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            var championShip1 = CreateChampionship(profiles[0], "name1");
            var championShip2 = CreateChampionship(profiles[0], "name2");
            var championShip3 = CreateChampionship(profiles[0], "name3");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     var criteria = new GetChampionshipsCriteria();
                                     var res=service.GetChampionships(data.Token, criteria,new PartialRetrievingInfo());

                                     Assert.AreEqual(3,res.AllItemsCount);
                                     Assert.AreEqual(1, res.Items.Where(x=>x.GlobalId==championShip1.GlobalId).Count());
                                     Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip2.GlobalId).Count());
                                     Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip3.GlobalId).Count());
            });
        }

        [Test]
        public void ById()
        {
            var championShip1 = CreateChampionship(profiles[0], "name1");
            var championShip2 = CreateChampionship(profiles[0], "name2");
            var championShip3 = CreateChampionship(profiles[0], "name3");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                criteria.ChampionshipId = championShip2.GlobalId;
                var res = service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo());

                Assert.AreEqual(1, res.AllItemsCount);
                Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip2.GlobalId).Count());
            });
        }

        [Test]
        public void OtherProfile()
        {
            var championShip1 = CreateChampionship(profiles[0], "name1");
            var championShip2 = CreateChampionship(profiles[0], "name2");
            var championShip3 = CreateChampionship(profiles[1], "name3");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                var res = service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo());

                Assert.AreEqual(2, res.AllItemsCount);
                Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip1.GlobalId).Count());
                Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip2.GlobalId).Count());
            });
        }

        [Test]
        public void PagedResults()
        {
            CreateChampionship(profiles[0], "name1");
            CreateChampionship(profiles[0], "name2");
            CreateChampionship(profiles[0], "name3");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                var res = service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo(){PageSize = 2});

                Assert.AreEqual(3, res.AllItemsCount);
                Assert.AreEqual(2, res.Items.Count);
            });
        }

        [Test]
        public void DistinctResults()
        {
            var customer1 = CreateCustomer("cust", profiles[0]);
            var customer2 = CreateCustomer("cust1", profiles[0]);
            

            var championShip1 = CreateChampionship(profiles[0], "name1");

            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));

            CreateReservation(championShip1, customer1);
            CreateReservation(championShip1, customer2);
            var champCust1 = new ChampionshipCustomer();
            champCust1.Customer = customer1;
            championShip1.Customers.Add(champCust1);
            var champCust2 = new ChampionshipCustomer();
            champCust2.Customer = customer2;
            championShip1.Customers.Add(champCust2);
            var champCust3 = new ChampionshipCustomer();
            champCust3.Customer = customer1;
            championShip1.Customers.Add(champCust3);
            var champCust4 = new ChampionshipCustomer();
            champCust4.Customer = customer2;
            championShip1.Customers.Add(champCust4);
            var champGroup = new ChampionshipGroup();
            champGroup.Name = "gr1";
            champCust1.Group = champGroup;
            champCust2.Group = champGroup;
            champGroup.Members.Add(champCust1);
            champGroup.Members.Add(champCust2);
            championShip1.Groups.Add(champGroup);
            var champGroup1 = new ChampionshipGroup();
            champGroup1.Name = "gr2";
            champCust3.Group = champGroup1;
            champCust4.Group = champGroup1;
            champGroup1.Members.Add(champCust3);
            champGroup1.Members.Add(champCust4);
            championShip1.Groups.Add(champGroup1);

            var entry = new ChampionshipEntry();
            entry.Customer = champCust1;
            entry.Exercise = benchPress;
            championShip1.Entries.Add(entry);
            var entry1 = new ChampionshipEntry();
            entry1.Customer = champCust2;
            entry1.Exercise = benchPress;
            championShip1.Entries.Add(entry1);
            var entry3 = new ChampionshipEntry();
            entry3.Customer = champCust3;
            entry3.Exercise = benchPress;
            championShip1.Entries.Add(entry3);
            insertToDatabase(championShip1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var criteria = new GetChampionshipsCriteria();
                var res = service.GetChampionships(data.Token, criteria, new PartialRetrievingInfo());

                Assert.AreEqual(1, res.AllItemsCount);
                Assert.AreEqual(1, res.Items.Where(x => x.GlobalId == championShip1.GlobalId).Count());

                Assert.AreEqual(3, res.Items[0].Entries.Count);
                Assert.AreEqual(2, res.Items[0].Groups.Count);
                Assert.AreEqual(4, res.Items[0].Customers.Count);
                Assert.AreEqual(2, res.Items[0].Groups[0].Members.Count);
                Assert.AreEqual(2, res.Items[0].Groups[1].Members.Count);
            });
        }
    }
}
