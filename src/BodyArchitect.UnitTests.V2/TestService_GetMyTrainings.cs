using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using Privacy = BodyArchitect.Model.Privacy;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetMyTrainings : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<MyTraining>  myTrainings = new List<MyTraining>();
        private SupplementCycleDefinition definition;
        private Customer customer;
        private MyTraining customerTraining;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                myTrainings.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));

                //set friendship
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                customer = CreateCustomer("Cust1", profiles[0]);

                var sup=CreateSupplement("sup");
                definition=CreateSupplementsCycleDefinition("def", sup,profiles[0]);
                var myTraining = CreateTraining<SupplementCycle>("sup1", DateTime.UtcNow.Date.AddDays(-3), TrainingEnd.Complete, 21, profiles[0], DateTime.UtcNow.Date);
                myTrainings.Add(myTraining);

                myTraining = CreateTraining<SupplementCycle>("sup2", DateTime.UtcNow.Date.AddDays(-7), TrainingEnd.Complete, 100, profiles[0], DateTime.UtcNow.Date.AddDays(-4));
                myTrainings.Add(myTraining);

                myTraining = CreateTraining<SupplementCycle>("sup3", DateTime.UtcNow.Date.AddDays(-5), TrainingEnd.NotEnded, 2, profiles[0]);
                myTrainings.Add(myTraining);

                var a6WTraining = CreateTraining<A6WTraining>("a6w1", DateTime.UtcNow.Date.AddDays(-4), TrainingEnd.NotEnded, 12, profiles[0]);
                myTrainings.Add(a6WTraining);

                a6WTraining = CreateTraining<A6WTraining>("a6w2", DateTime.UtcNow.Date.AddDays(-14), TrainingEnd.Complete, 100, profiles[0], DateTime.UtcNow.Date.AddDays(-10));
                myTrainings.Add(a6WTraining);

                customerTraining = CreateTraining<A6WTraining>("a6w3", DateTime.UtcNow.Date.AddDays(-14), TrainingEnd.Complete, 100, profiles[0], DateTime.UtcNow.Date.AddDays(-10), customer);
                tx.Commit();
            }
        }


        protected T CreateTraining<T>(string name, DateTime startDate, TrainingEnd trainingEnd, int percentage, Profile profile, DateTime? endDate = null, Customer customer = null) where T:MyTraining,new()
        {
            T cycle = new T();
            cycle.SetData(startDate, endDate, trainingEnd);
            cycle.Profile = profile;
            cycle.Name = name;
            cycle.Customer = customer;
            cycle.PercentageCompleted = percentage;
            SupplementCycle supplementCycle = cycle as SupplementCycle;
            if(supplementCycle!=null)
            {
                supplementCycle.SupplementsCycleDefinition = definition;
            }
            insertToDatabase(cycle);
            return cycle;
        }

        private void setPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }

        [Test]
        public void All()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                var entries = service.GetMyTrainings(data.Token, param,new PartialRetrievingInfo());
                Assert.AreEqual(myTrainings.Count, entries.AllItemsCount);
                Assert.AreEqual(myTrainings.Count, entries.Items.Count);
            });
        }

        [Test]
        public void PagedResult()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo(){PageSize = 2});
                Assert.AreEqual(myTrainings.Count, entries.AllItemsCount);
                Assert.AreEqual(2, entries.Items.Count);
            });
        }

        [Test]
        public void ById()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.MyTrainingId = myTrainings[1].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(1, entries.AllItemsCount);
                Assert.AreEqual(myTrainings[1].Name, entries.Items[0].Name);
            });
        }

        [Test]
        public void FromStartDate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.StartDate = DateTime.UtcNow.Date.AddDays(-4);
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());

                assert(entries, 0, 3);
            });
        }

        [Test]
        public void WithSpecifiedTrainingEnd()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.TrainingEnds.Add(Service.V2.Model.TrainingEnd.Complete);
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());

                assert(entries, 0, 1,4);
            });
        }


        [Test]
        public void GetMyTrainings_Private()
        {
            setPrivacy(Privacy.Private);

            var data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());

                Assert.AreEqual(0, entries.AllItemsCount);
            });

            data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(0, entries.AllItemsCount);
            });
        }

        [Test]
        public void GetMyTrainings_FriendsOnly()
        {
            setPrivacy(Privacy.FriendsOnly);

            var data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());

                Assert.AreEqual(myTrainings.Count, entries.AllItemsCount);
            });

            data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(0, entries.AllItemsCount);
            });
        }

        [Test]
        public void GetMyTrainings_Public()
        {
            setPrivacy(Privacy.Public);

            var data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());

                Assert.AreEqual(myTrainings.Count, entries.AllItemsCount);
            });

            data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.UserId = profiles[0].GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(myTrainings.Count, entries.AllItemsCount);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void ForCustomer_GetMyTrainings_CustomerFromAnotherProfile()
        {
            setPrivacy(Privacy.Public);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.CustomerId = customer.GlobalId;
                service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
            });

        }

        [Test]
        public void ById_GetMyTrainings_CustomerFromAnotherProfile()
        {
            setPrivacy(Privacy.Public);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.MyTrainingId = customerTraining.GlobalId;
                var result=service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(0,result.AllItemsCount);
            });

        }

        [Test]
        public void ByIdAndSpecifiedProfileId_GetMyTrainings_CustomerFromAnotherProfile()
        {
            setPrivacy(Privacy.Public);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.MyTrainingId = customerTraining.GlobalId;
                param.UserId = profiles[0].GlobalId;
                var result = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(0, result.AllItemsCount);
            });

        }

        [Test]
        public void ForCustomer()
        {
            setPrivacy(Privacy.Public);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                GetMyTrainingsParam param = new GetMyTrainingsParam();
                param.CustomerId = customer.GlobalId;
                var entries = service.GetMyTrainings(data.Token, param, new PartialRetrievingInfo());
                Assert.AreEqual(1,entries.AllItemsCount);
                Assert.AreEqual("a6w3", entries.Items[0].Name);
            });

        }

        void assert(PagedResult<MyTrainingDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.AllItemsCount);
            Assert.AreEqual(indexes.Length, pack.Items.Count);
            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == myTrainings[index].GlobalId).SingleOrDefault());
                }
            }

        }
    }
}
