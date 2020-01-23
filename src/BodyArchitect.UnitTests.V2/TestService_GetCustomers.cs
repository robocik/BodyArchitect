using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using NUnit.Framework;
using Gender = BodyArchitect.Model.Gender;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetCustomers : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                customers.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));

                var customer = CreateCustomer("cust1",profiles[0],null,Gender.Female,"cust1@mail.com");
                customers.Add(customer);
                customer = CreateCustomer("cust2", profiles[0], null, Gender.Male, "cust2@mail.com","2");
                customers.Add(customer);
                customer = CreateCustomer("cust3", profiles[0], profiles[1], Gender.Male, "cust3@mail.com", "3");
                customers.Add(customer);
                customer = CreateCustomer("cust3", profiles[1], null, Gender.Male, "cust3@mail.com", "3");
                customer = CreateCustomer("cust4", profiles[0], profiles[2], Gender.Male, "cust4@mail.com", "4");
                customers.Add(customer);
                customer = CreateCustomer("cust5", profiles[0], null, Gender.NotSet, null, "5");
                customers.Add(customer);
                customer = CreateCustomer("cust6", profiles[0], profiles[2], Gender.Female);
                customer.IsVirtual = true;
                Session.Update(customer);
                customers.Add(customer);
                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            CustomerSearchCriteria param = new CustomerSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetCustomers(data.Token, param, pageInfo);
            });
            Assert.AreEqual(customers.Count,result.AllItemsCount);
        }

        [Test]
        public void FilterByGender()
        {
            CustomerSearchCriteria param = new CustomerSearchCriteria();
            param.Gender = Service.V2.Model.Gender.Female;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetCustomers(data.Token, param, pageInfo);
            });
            assert(result,  0, 5);
        }


        [Test]
        public void OnlyVirtual()
        {
            CustomerSearchCriteria param = new CustomerSearchCriteria();
            param.CustomerVirtualCriteria = CustomerVirtualCriteria.VirtualOnly;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetCustomers(data.Token, param, pageInfo);
            });
            assert(result, 5);
        }

        [Test]
        public void OnlyStandard()
        {
            CustomerSearchCriteria param = new CustomerSearchCriteria();
            param.CustomerVirtualCriteria = CustomerVirtualCriteria.StandardOnly;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetCustomers(data.Token, param, pageInfo);
            });
            assert(result,0,1,2,3,4);
        }
        void assert(PagedResult<CustomerDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.AllItemsCount);
            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == customers[index].GlobalId).SingleOrDefault());
                }
            }

        }

    }
}
