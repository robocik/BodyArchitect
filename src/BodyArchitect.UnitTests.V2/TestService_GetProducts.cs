using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetProducts : TestServiceBase
    {
        private List<Profile> profiles = new List<Profile>();
        private List<Customer> customers = new List<Customer>();
        private List<Product> products = new List<Product>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                products.Clear();
                customers.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var customer = CreateCustomer("cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust3", profiles[1]);
                customers.Add(customer);

                var activity=CreateActivity("Activity", profiles[0]);
                ScheduleEntry entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.Activity = activity;
                Session.Save(entry);
                ScheduleEntryReservation reservation = new ScheduleEntryReservation();
                reservation.Profile = profiles[0];
                reservation.DateTime = DateTime.UtcNow.Date.AddDays(-1);
                reservation.Customer = customers[0];
                reservation.Name = "test1";
                reservation.EnterDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.LeaveDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.ScheduleEntry = entry;
                products.Add(reservation);
                insertToDatabase(reservation);

                reservation = new ScheduleEntryReservation();
                reservation.Profile = profiles[0];
                reservation.DateTime = DateTime.UtcNow.Date.AddDays(-2);
                reservation.Customer = customers[0];
                reservation.Name = "test2";
                reservation.EnterDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.LeaveDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.ScheduleEntry = entry;
                insertToDatabase(reservation);
                products.Add(reservation);
                reservation.Payment = CreatePayment(reservation, profiles[0]);

                reservation = new ScheduleEntryReservation();
                reservation.Profile = profiles[0];
                reservation.DateTime = DateTime.UtcNow.Date.AddDays(-3);
                reservation.Customer = customers[1];
                reservation.Name = "test3";
                reservation.EnterDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.LeaveDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.ScheduleEntry = entry;
                products.Add(reservation);
                insertToDatabase(reservation);

                reservation = new ScheduleEntryReservation();
                reservation.Profile = profiles[1];
                reservation.DateTime = DateTime.UtcNow.Date.AddDays(-2);
                reservation.Customer = customers[1];
                reservation.Name = "test4";
                reservation.EnterDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.LeaveDateTime = DateTime.UtcNow.AddDays(-1);
                reservation.ScheduleEntry = entry;
                insertToDatabase(reservation);
                tx.Commit();
            }
        }

        [Test]
        public void Payment()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                Assert.IsNotNull(list.Items.Where(x => x.Product.GlobalId == products[1].GlobalId).Single().Payment);
            });
        }

        #region SortOrder

        [Test]
        public void Sort_ByPaid_Asc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted = products.OrderBy(x => x.Payment!=null).ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.SortOrder = ProductsSortOrder.ByPaid;
                param.SortAscending = true;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                Assert.AreEqual("test2", list.Items[2].Product.Name);
            });
        }

        [Test]
        public void Sort_ByPaid_Desc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted = products.OrderBy(x => x.Payment != null).Reverse().ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                param.SortOrder = ProductsSortOrder.ByPaid;
                param.SortAscending = false;
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                Assert.AreEqual("test2",list.Items[0].Product.Name);
            });
        }

        [Test]
        public void Sort_ByDate_Asc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted = products.OrderBy(x => x.DateTime).ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.SortOrder = ProductsSortOrder.ByDate;
                param.SortAscending = true;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assertOrder(list, sorted);
            });
        }

        [Test]
        public void Sort_ByDate_Desc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted = products.OrderBy(x => x.DateTime).Reverse().ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                param.SortOrder = ProductsSortOrder.ByDate;
                param.SortAscending = false;
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assertOrder(list, sorted);
            });
        }

        [Test]
        public void Sort_ByName_Asc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted=products.OrderBy(x => x.Name).ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.SortOrder = ProductsSortOrder.ByName;
                param.SortAscending = true;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assertOrder(list, sorted);
            });
        }

        [Test]
        public void Sort_ByName_Desc()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var sorted = products.OrderBy(x => x.Name).Reverse().ToList();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                param.SortOrder = ProductsSortOrder.ByName;
                param.SortAscending = false;
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assertOrder(list, sorted);
            });
        }
        #endregion

        #region Payment

        [Test]
        public void Payments_All()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.PaymentCriteria = PaymentCriteria.Any;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list, 0, 1, 2);
            });
        }

        [Test]
        public void Payments_WithPayment()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.PaymentCriteria = PaymentCriteria.WithPayment;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list,1);
            });
        }

        [Test]
        public void Payments_WithoutPayment()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.PaymentCriteria = PaymentCriteria.WithoutPayment;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list, 0,2);
            });
        }

        #endregion

        [Test]
        public void All()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list, 0, 1, 2);
            });
        }

        [Test]
        public void FromDate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.StartTime = DateTime.UtcNow.AddDays(-2).Date;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list, 0, 1);
            });
        }

        [Test]
        public void ToDate()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.EndTime = DateTime.UtcNow.AddDays(-2).Date;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list,  1,2);
            });
        }

        [Test]
        public void ForCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.CustomerId = customers[0].GlobalId;
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetProducts(data.Token, param, retrievingInfo);
                assert(list,0,1);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void ForCustomer_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetProductsParam();
                param.CustomerId = customers[2].GlobalId;
                var retrievingInfo = new PartialRetrievingInfo();
                service.GetProducts(data.Token, param, retrievingInfo);
            });
        }

        void assertOrder(PagedResult<ProductInfoDTO> pack, IList<Product> sorted)
        {
            Assert.AreEqual(sorted.Count, pack.AllItemsCount);
            Assert.AreEqual(sorted.Count, pack.Items.Count);
            for (int i = 0; i < sorted.Count; i++)
            {
                Assert.AreEqual(sorted[i].Name, pack.Items[i].Product.Name);
            }

        }

        void assert(PagedResult<ProductInfoDTO> pack, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, pack.AllItemsCount);
            Assert.AreEqual(indexes.Length, pack.Items.Count);
            if (indexes != null)
            {
                foreach (var index in indexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.Product.GlobalId == products[index].GlobalId).SingleOrDefault());
                }
            }

        }
    }
}
