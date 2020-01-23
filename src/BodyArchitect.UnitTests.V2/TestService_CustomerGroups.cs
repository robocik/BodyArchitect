using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using CustomerGroupRestrictedType = BodyArchitect.Service.V2.Model.Instructor.CustomerGroupRestrictedType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_CustomerGroups : TestServiceBase
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

        #region Save
        [Test]
        public void SimpeSaveNewCustomerGroup()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.MaxPersons = 10;

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup=service.SaveCustomerGroup(data.Token, group);
                Assert.AreEqual(profile.GlobalId,savedGroup.ProfileId);
                group.ProfileId = savedGroup.ProfileId;
                Assert.AreNotEqual(Guid.Empty,savedGroup.GlobalId);
                Assert.AreEqual(1,savedGroup.Version);
            });
            UnitTestHelper.CompareObjects(group,savedGroup,true);
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(profile.GlobalId,db.Profile.GlobalId);
        }

        [Test]
        public void SimpeSaveNewCustomerGroup_DefaultColor()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = null;//we don't set a color. server should set default color in this case
            group.MaxPersons = 10;

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);
            });
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(Constants.DefaultColor, db.Color);
            Assert.AreEqual(Constants.DefaultColor, savedGroup.Color);
        }

        [Test]
        public void SaveNewCustomerGroup_WithDefaultActivity()
        {
            var activity=CreateActivity("Activity",profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.MaxPersons = 10;
            group.DefaultActivityId = activity.GlobalId;

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);
                Assert.AreEqual(activity.GlobalId, savedGroup.DefaultActivityId);
                
            });

            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(activity.GlobalId, db.DefaultActivity.GlobalId);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveNewCustomerGroup_WithDefaultActivity_OtherProfile()
        {
            var activity = CreateActivity("Activity", profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.DefaultActivityId = activity.GlobalId;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);

            });
        }

        [Test]
        public void SaveCustomerGroup_WithCustomers_Bug()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var customer1 = CreateCustomer("cust1", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());
            group.Customers.Add(customer1.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });
            savedGroup.Customers.Clear();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, savedGroup);

            });
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(0, db.Customers.Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveNewCustomerGroup_WithVirtualCustomer()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            customer.IsVirtual = true;
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
        }

        [Test]
        public void SaveNewCustomerGroup_WithCustomers()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);
                Assert.AreEqual(customer.GlobalId, savedGroup.Customers[0].GlobalId);
                
            });
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(customer.GlobalId, db.Customers.ElementAt(0).GlobalId);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveNewCustomerGroup_WithCustomers_OtherProfile()
        {
            var customer = CreateCustomer("cust", profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
        }

        [Test]
        public void SaveNewCustomerGroup_WithCustomers_ChangedCustomer()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            var dto = customer.Map<CustomerDTO>();
            dto.LastName = "testName";
            group.Customers.Add(dto);

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);
                Assert.AreEqual(customer.LastName, savedGroup.Customers[0].LastName);

            });
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(customer.LastName, db.Customers.ElementAt(0).LastName);
        }

        [Test]
        [ExpectedException(typeof(UniqueException))]
        public void SaveCustomerGroup_Validation_UniqueName()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });

            group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
        }

        [Test]
        public void SaveCustomerGroup_Validation_UniqueName_DifferentUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
            data = CreateNewSession(profile1, ClientInformation);
            group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.MaxPersons = 10;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
            Assert.AreEqual(2,Session.QueryOver<CustomerGroup>().RowCount());
        }

        [Test]
        public void UpdateTwiceCustomerGroup_WithCustomer()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, savedGroup);

            });
            var db = Session.QueryOver<CustomerGroup>().SingleOrDefault();
            Assert.AreEqual(1, db.Customers.Count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateCustomerGroup_OtherProfile()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = group.Map<CustomerGroupDTO>();
            dto.Name = "name";
            dto.ProfileId = profile.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateCustomerGroup_OldData()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = group.Map<CustomerGroupDTO>();
            dto.Version = 0;
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                service.SaveCustomerGroup(data.Token, dto);
            });

        }

        [Test]
        public void UpdateCustomerGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = group.Map<CustomerGroupDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                var res = service.SaveCustomerGroup(data.Token, dto);
                UnitTestHelper.CompareDateTime(res.CreationDate, group.CreationDate);
                UnitTestHelper.CompareObjects(dto, res, true);
            });
            Assert.AreEqual(1, Session.QueryOver<CustomerGroup>().RowCount());

        }

        #region Restricted tests

        [Test]
        public void Restricted_CustomerInTwoGroups_NotRestricted_NotRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });
            var db = Session.QueryOver<Customer>().Fetch(x=>x.Groups).Eager.SingleOrDefault();
            Assert.AreEqual(2, db.Groups.Count);
            Assert.AreEqual(customer.Version, db.Version);

        }

        [Test]
        public void Restricted_CustomerInTwoGroups_NotRestricted_PartiallyRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });
            var db = Session.QueryOver<Customer>().Fetch(x => x.Groups).Eager.SingleOrDefault();
            Assert.AreEqual(2, db.Groups.Count);
        }

        [Test]
        public void Restricted_CustomerInTwoGroups_PartiallyRestricted_NotRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });
            var db = Session.QueryOver<Customer>().Fetch(x => x.Groups).Eager.SingleOrDefault();
            Assert.AreEqual(2, db.Groups.Count);
        }

        [Test]
        public void Restricted_CustomerInTwoGroups_PartiallyRestricted_NotRestricted_UpdateBug()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, savedGroup);

            });
            var db = Session.QueryOver<Customer>().Fetch(x => x.Groups).Eager.SingleOrDefault();
            Assert.AreEqual(1, db.Groups.Count);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Restricted_CustomerInTwoGroups_PartiallyRestricted_PartiallyRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Restricted_CustomerInTwoGroups_FullyRestricted_NoneRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.RestrictedType = CustomerGroupRestrictedType.Full;
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);

            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Restricted_CustomerInTwoGroups_FullyRestricted_PartiallyRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.Full;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.RestrictedType = CustomerGroupRestrictedType.Partially;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);

            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Restricted_CustomerInTwoGroups_FullyRestricted_FullyRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.RestrictedType = CustomerGroupRestrictedType.Full;
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.RestrictedType = CustomerGroupRestrictedType.Full;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);

            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void Restricted_CustomerInTwoGroups_NotRestricted_FullyRestricted()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.Color = System.Drawing.Color.Aqua.ToColorString();
            group.RestrictedType = CustomerGroupRestrictedType.None;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            CustomerGroupDTO savedGroup = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedGroup = service.SaveCustomerGroup(data.Token, group);

            });

            group = new CustomerGroupDTO();
            group.Name = "test";
            group.RestrictedType = CustomerGroupRestrictedType.Full;
            group.MaxPersons = 10;
            group.Customers.Add(customer.Map<CustomerDTO>());

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);

            });
        }

        #endregion
        #endregion

        #region Delete

        [Test]
        public void DeleteEmptyGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void DeleteGroupWithDefaultActivity()
        {
            var activity = CreateActivity("activity",profiles[0]);
            var group = CreateCustomerGroup("test", profiles[0]);
            group.DefaultActivity = activity;
            insertToDatabase(group);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
            var count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(1, count);
            count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void DeleteGroupWithCustomers()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var group = CreateCustomerGroup("test", profiles[0],10, customer);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
            var count = Session.QueryOver<CustomerGroup>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<Customer>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteCustomerGroup_OtherUser()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var dto = group.Map<CustomerGroupDTO>();
                Service.DeleteCustomerGroup(data.Token, dto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteCustomerGroup_SecurityBug()
        {
            
            var group = CreateCustomerGroup("test", profiles[0], 10);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var dto = group.Map<CustomerGroupDTO>();
                dto.ProfileId = profile.GlobalId;
                Service.DeleteCustomerGroup(data.Token, dto);
            });
            
        }

        #endregion

        #region DataInfo

        [Test]
        public void SaveNewCustomerGroup_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            CustomerGroupDTO group = new CustomerGroupDTO();
            group.Name = "Children";
            group.MaxPersons = 10;
            group.Color = System.Drawing.Color.Aqua.ToColorString();

            var oldHash = profiles[0].DataInfo.CustomerGroupHash;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, group);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.CustomerGroupHash);
        }

        [Test]
        public void UpdateCustomerGroup_WithModification_DataInfo()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.CustomerGroupHash;

            var dto = group.Map<CustomerGroupDTO>();
            dto.Name = "gfdgdg";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomerGroup(data.Token, dto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.CustomerGroupHash);

        }

        [Test]
        public void DeleteGroup()
        {
            var group = CreateCustomerGroup("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.CustomerGroupHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteCustomerGroup(data.Token, group.Map<CustomerGroupDTO>());
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.CustomerGroupHash);
        }
        #endregion
    }
}
