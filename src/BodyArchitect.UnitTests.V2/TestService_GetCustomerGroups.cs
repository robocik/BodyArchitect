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
    public class TestService_GetCustomerGroups : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();
        List<CustomerGroup> groups=new List<CustomerGroup>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                customers.Clear();
                groups.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                profiles.Add(CreateProfile(Session, "test4"));

                var customer = CreateCustomer("cust1", profiles[0], null, Gender.Female, "cust1@mail.com");
                customers.Add(customer);
                customer = CreateCustomer("cust2", profiles[0], null, Gender.Male, "cust2@mail.com", "2");
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

                var group=CreateCustomerGroup("group1", profiles[0], 5, customers[0]);
                groups.Add(group);
                group = CreateCustomerGroup("group2", profiles[0], 3, customers[0], customers[1], customers[5]);
                groups.Add(group);
                group = CreateCustomerGroup("group3", profiles[0], 7);
                groups.Add(group);
                group = CreateCustomerGroup("group4", profiles[1], 6);
                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            var param = new CustomerGroupSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerGroupDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetCustomerGroups(data.Token, param, pageInfo);
            });
            Assert.AreEqual(groups.Count, result.AllItemsCount);
        }

        [Test]
        public void PagedResult()
        {
            var param = new CustomerGroupSearchCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 1;

            PagedResult<CustomerGroupDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetCustomerGroups(data.Token, param, pageInfo);
            });
            Assert.AreEqual(groups.Count, result.AllItemsCount);
            Assert.AreEqual(1, result.Items.Count);
        }

        [Test]
        public void WithMembersOnly()
        {
            var param = new CustomerGroupSearchCriteria();
            param.MembersCriteria = CustomerGroupMembersCriteria.WithMembersOnly;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerGroupDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetCustomerGroups(data.Token, param, pageInfo);
            });
            assert(result,0,1 );
        }

        [Test]
        public void WithoutMembersOnly()
        {
            var param = new CustomerGroupSearchCriteria();
            param.MembersCriteria = CustomerGroupMembersCriteria.WithoutMembersOnly;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CustomerGroupDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetCustomerGroups(data.Token, param, pageInfo);
            });
            assert(result, 2);
        }

        void assert(PagedResult<CustomerGroupDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.AllItemsCount);
            Assert.AreEqual(indexes.Length, pack.Items.Count);
            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == groups[index].GlobalId).SingleOrDefault());
                }
            }

        }
    }
}
