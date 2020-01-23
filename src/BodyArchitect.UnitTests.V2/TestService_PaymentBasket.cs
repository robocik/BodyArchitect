using System;
using System.Collections.Generic;
using System.Linq;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_PaymentBasket : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Activity> activities = new List<Activity>();
        List<ScheduleEntry> entries = new List<ScheduleEntry>();
        List<Customer> customers = new List<Customer>();
        List<PaymentBasket> koszyki = new List<PaymentBasket>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                activities.Clear();
                entries.Clear();
                customers.Clear();
                koszyki.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var customer = CreateCustomer("cust1", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust3", profiles[1]);
                customers.Add(customer);

                var activity = CreateActivity("Swimming", profiles[0],10);
                activities.Add(activity);
                activity = CreateActivity("Strength training", profiles[0],15);
                activities.Add(activity);

                ScheduleEntry entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.Activity = activities[0];
                entry.MaxPersons = 3;
                entries.Add(entry);
                ScheduleEntryReservation reservation = new ScheduleEntryReservation();
                reservation.Customer = customers[0];
                reservation.Name = "test";
                reservation.DateTime = DateTime.UtcNow;
                reservation.Profile = profiles[0];
                reservation.Price = activities[0].Price;
                reservation.ScheduleEntry = entry;
                entry.Reservations.Add(reservation);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.Activity = activities[1];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.MaxPersons = 1;
                entries.Add(entry);
                reservation = new ScheduleEntryReservation();
                reservation.Customer = customers[0];
                reservation.Name = "test";
                reservation.DateTime = DateTime.UtcNow;
                reservation.Profile = profiles[0];
                reservation.Price = activities[1].Price;
                reservation.ScheduleEntry = entry;
                entry.Reservations.Add(reservation);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[1];
                entry.MyPlace = GetDefaultMyPlace(profiles[1]);
                entry.Activity = activities[1];
                entry.StartTime = DateTime.UtcNow.AddDays(-5);
                entry.EndTime = DateTime.UtcNow.AddDays(-5).AddHours(1);
                entry.MaxPersons = 1;
                entry.Activity = activities[1];
                entries.Add(entry);
                reservation = new ScheduleEntryReservation();
                reservation.Customer = customers[2];
                reservation.Name = "test";
                reservation.Profile = profiles[1];
                reservation.DateTime = DateTime.UtcNow;
                reservation.Price = 11;
                reservation.ScheduleEntry = entry;
                entry.Reservations.Add(reservation);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.Activity = activities[1];
                entry.StartTime = DateTime.UtcNow.AddDays(-22);
                entry.EndTime = DateTime.UtcNow.AddDays(-22).AddHours(1);
                entry.MaxPersons = 1;
                entries.Add(entry);
                reservation = new ScheduleEntryReservation();
                reservation.Customer = customers[0];
                reservation.Name = "test";
                reservation.DateTime = DateTime.UtcNow;
                reservation.Profile = profiles[0];
                reservation.Price = activities[1].Price;
                reservation.ScheduleEntry = entry;
                entry.Reservations.Add(reservation);
                insertToDatabase(entry);

                var koszyk = new PaymentBasket();
                koszyk.DateTime = DateTime.UtcNow.AddDays(-10);
                koszyk.Profile = profiles[0];
                koszyk.Customer = customers[0];
                koszyk.TotalPrice = 20;
                var zakup = new Payment();
                zakup.Product = entries[0].Reservations.ElementAt(0);
                entries[0].Reservations.ElementAt(0).Payment = zakup;
                zakup.PaymentBasket = koszyk;
                koszyk.Payments.Add(zakup);
                insertToDatabase(koszyk);
                koszyki.Add(koszyk);

                koszyk = new PaymentBasket();
                koszyk.DateTime = DateTime.UtcNow.AddDays(-8);
                koszyk.Customer = customers[0];
                koszyk.Profile = profiles[0];
                koszyk.TotalPrice = 11;
                zakup = new Payment();
                zakup.Product = entries[1].Reservations.ElementAt(0);
                entries[1].Reservations.ElementAt(0).Payment = zakup;
                zakup.PaymentBasket = koszyk;
                koszyk.Payments.Add(zakup);
                insertToDatabase(koszyk);
                koszyki.Add(koszyk);

                koszyk = new PaymentBasket();
                koszyk.DateTime = DateTime.UtcNow.AddDays(-8);
                koszyk.Profile = profiles[1];
                koszyk.Customer = customers[2];
                koszyk.TotalPrice = 11;
                zakup = new Payment();
                zakup.Product = entries[2].Reservations.ElementAt(0);
                entries[2].Reservations.ElementAt(0).Payment = zakup;
                zakup.PaymentBasket = koszyk;
                koszyk.Payments.Add(zakup);
                insertToDatabase(koszyk);

                tx.Commit();
            }
        }

        void assert(PagedResult<PaymentBasketDTO> result, params int[] indexes)
        {
            Assert.AreEqual(indexes.Length, result.AllItemsCount);
            foreach (int id in indexes)
            {
                var res = result.Items.Where(x => x.GlobalId == koszyki[id].GlobalId).SingleOrDefault();
                Assert.IsNotNull(res);
            }
        }

        #region GetPaymentBaskets

        [Test]
        public void GetAllPaymentBaskets()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetPaymentBasketParam();
                var retrievingInfo = new PartialRetrievingInfo();
                var list = service.GetPaymentBaskets(data.Token,param, retrievingInfo);
                Assert.AreEqual(koszyki.Count, list.AllItemsCount);
                Assert.AreEqual(koszyki.Count, list.Items.Count);
            });
        }

        [Test]
        public void GetForCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new GetPaymentBasketParam();
                var retrievingInfo = new PartialRetrievingInfo();
                param.CustomerId = customers[0].GlobalId;
                var list = service.GetPaymentBaskets(data.Token, param, retrievingInfo);
                assert(list, 0, 1);
            });
        }
        #endregion

        #region PaymentBasketOperation

        [Test]
        [ExpectedException(typeof(ProductAlreadyPaidException))]
        public void PayForAlreadyPaidProduct()
        {
            var zakup = CreatePayment(entries[0].Reservations.ElementAt(0), profiles[0]);
            insertToDatabase(zakup);

            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 10m;
            var zakupdto = new PaymentDTO();
            zakupdto.Count = 1;
            zakupdto.Product = entries[0].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakupdto);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token,koszyk);
            });
        }

        [Test]
        public void PayForOneProduct()
        {
            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 15;
            var zakup = new PaymentDTO();
            zakup.Count = 1;
            zakup.Product = entries[3].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });

            Assert.AreEqual(profiles[0].GlobalId, koszyk.ProfileId);
            Assert.AreNotEqual(Guid.Empty,koszyk.GlobalId);
            Assert.AreEqual(15m, koszyk.TotalPrice);
            var db = Session.Get<PaymentBasket>(koszyk.GlobalId);
            Assert.AreEqual(DateTime.UtcNow.Date, db.DateTime.Date);
            Assert.AreEqual(1, db.Payments.Count);
            Assert.AreEqual(15m, db.TotalPrice);
        }

        [Test]
        public void PayForOneProductWithTwoCount()
        {
            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 30m;
            var zakup = new PaymentDTO();
            zakup.Count = 2;
            zakup.Product = entries[3].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });

            Assert.AreEqual(profiles[0].GlobalId, koszyk.ProfileId);
            Assert.AreNotEqual(Guid.Empty,koszyk.GlobalId);
            var db = Session.Get<PaymentBasket>(koszyk.GlobalId);
            Assert.AreEqual(DateTime.UtcNow.Date, db.DateTime.Date);
            Assert.AreEqual(1, db.Payments.Count);
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void Error_PayLessMoney()
        {
            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 10.3m;
            var zakup = new PaymentDTO();
            zakup.Count = 1;
            zakup.Product = entries[3].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void Error_PayLessMoney_TwoCount()
        {
            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 12.3m;
            var zakup = new PaymentDTO();
            zakup.Count = 2;
            zakup.Product = entries[3].Reservations.ElementAt(0).Map< ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Error_DifferentCustomerCompanyId()
        {
            var wrongCustomer = CreateCustomer("ttt",profiles[1]);
            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = wrongCustomer.GlobalId;
            koszyk.TotalPrice = 12.3m;
            var zakup = new PaymentDTO();
            zakup.Count = 1;
            zakup.Product = entries[0].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });

            Assert.AreEqual(profiles[0].GlobalId, koszyk.ProfileId);
            Assert.AreNotEqual(Guid.Empty, koszyk.GlobalId);
            var db = Session.Get<PaymentBasket>(koszyk.GlobalId);
            Assert.AreEqual(DateTime.UtcNow.Date, db.DateTime.Date);
            Assert.AreEqual(1, db.Payments.Count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Error_DifferentProductCompanyId()
        {

            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 12.3m;
            var zakup = new PaymentDTO();
            zakup.Count = 1;
            zakup.Product = zakup.Product = entries[2].Reservations.ElementAt(0).Map<ProductDTO>();
            koszyk.Payments.Add(zakup);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });
        }
        #endregion
    }
}
