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
using ScheduleEntryState = BodyArchitect.Model.ScheduleEntryState;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_ReservationsOperation : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Activity> activities = new List<Activity>();
        List<ScheduleEntry> entries = new List<ScheduleEntry>();
        List<Customer> customers = new List<Customer>();
        List<CustomerGroup> groups = new List<CustomerGroup>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                entries.Clear();
                activities.Clear();
                customers.Clear();
                groups.Clear();

                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                
                var customer =CreateCustomer("cust1",profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust2", profiles[0]);
                customers.Add(customer);
                customer = CreateCustomer("cust3", profiles[1]);
                customers.Add(customer);
                customer = CreateCustomer("cust4", profiles[0]);
                customer.IsVirtual = true;
                Session.SaveOrUpdate(customer);
                customers.Add(customer);

                var group = CreateCustomerGroup("gr1", profiles[0], 10, customers[0], customers[1]);
                insertToDatabase(group);
                groups.Add(group);
                group = CreateCustomerGroup("gr2", profiles[0], 10, customers[0], customers[3]);
                insertToDatabase(group);
                groups.Add(group);
                group = CreateCustomerGroup("gr3", profiles[1], 10, customers[2]);
                insertToDatabase(group);
                groups.Add(group);

                var act=CreateActivity("act1", profiles[0],20);
                insertToDatabase(act);
                activities.Add(act);
                act = CreateActivity("act2", profiles[0]);
                insertToDatabase(act);
                activities.Add(act);

                ScheduleEntry entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.Activity = activities[0];
                entry.Price = entry.Activity.Price;
                entry.MaxPersons = 3;
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[1];
                entry.MyPlace = GetDefaultMyPlace(profiles[1]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.MaxPersons = 1;
                entry.Activity = activities[0];
                entry.Price = entry.Activity.Price;
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[1];
                entry.MyPlace = GetDefaultMyPlace(profiles[1]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.MaxPersons = 30;
                entry.Activity = activities[1];
                entry.Price = entry.Activity.Price;
                entries.Add(entry);
                insertToDatabase(entry);

                entry = new ScheduleEntry();
                entry.Profile = profiles[0];
                entry.MyPlace = GetDefaultMyPlace(profiles[0]);
                entry.StartTime = DateTime.UtcNow.AddDays(-10);
                entry.EndTime = DateTime.UtcNow.AddDays(-10).AddHours(1);
                entry.Activity = activities[0];
                entry.Price = entry.Activity.Price;
                entry.MaxPersons = 3;
                entries.Add(entry);
                insertToDatabase(entry);

                tx.Commit();
            }
        }

        #region Mapper

        [Test]
        public void Mapper_CustomerId()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                var res=Service.ReservationsOperation(data.Token, param);
                Assert.AreEqual(customers[0].GlobalId,res.Reservation.CustomerId);
            });

        }

        [Test]
        public void Mapper_ScheduleEntryId()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                var res = Service.ReservationsOperation(data.Token, param);
                Assert.AreEqual(entries[0].GlobalId, res.Reservation.ScheduleEntryId);
            });

        }
        #endregion

        #region Make
        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Make_ScheduleEntry_AnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[1].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void Make_Customer_AnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[2].GlobalId;
                param.EntryId = entries[1].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        public void MakeGroup()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = groups[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.MakeGroup;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(2, db.Reservations.Count);
            var reservation=db.Reservations.Where(x => x.Customer.GlobalId == customers[0].GlobalId).Single();
            Assert.IsNull(reservation.LeaveDateTime);
            Assert.IsNull(reservation.EnterDateTime);
            Assert.IsNull(reservation.Payment);
            Assert.AreEqual(activities[0].Price, reservation.Price);
            Assert.AreEqual(profile.GlobalId, reservation.Profile.GlobalId);
            Assert.Greater(reservation.DateTime, DateTime.MinValue);

            reservation = db.Reservations.Where(x => x.Customer.GlobalId == customers[1].GlobalId).Single();
            Assert.IsNull(reservation.LeaveDateTime);
            Assert.IsNull(reservation.EnterDateTime);
            Assert.IsNull(reservation.Payment);
            
        }

        [Test]
        public void MakeGroup_OneCustomerHasAlreadyReservation()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = groups[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.MakeGroup;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(2, db.Reservations.Count);
            Assert.AreEqual(customers[0].GlobalId, db.Reservations.ElementAt(0).Customer.GlobalId);
            Assert.IsNull(db.Reservations.ElementAt(0).LeaveDateTime);
            Assert.IsNull(db.Reservations.ElementAt(0).EnterDateTime);
            Assert.IsNull(db.Reservations.ElementAt(0).Payment);
            Assert.AreEqual(customers[1].GlobalId, db.Reservations.ElementAt(1).Customer.GlobalId);
            Assert.IsNull(db.Reservations.ElementAt(1).LeaveDateTime);
            Assert.IsNull(db.Reservations.ElementAt(1).EnterDateTime);
            Assert.IsNull(db.Reservations.ElementAt(1).Payment);
            Assert.AreEqual(activities[0].Price, db.Reservations.ElementAt(0).Price);
            Assert.AreEqual(profile.GlobalId, db.Reservations.ElementAt(0).Profile.GlobalId);
            Assert.Greater(db.Reservations.ElementAt(0).DateTime, DateTime.MinValue);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void MakeGroup_AnotherUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = groups[2].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.MakeGroup;
                Service.ReservationsOperation(data.Token, param);
            });
        }



        [Test]
        public void Make()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(1, db.Reservations.Count);
            Assert.AreEqual(customers[0].GlobalId, db.Reservations.ElementAt(0).Customer.GlobalId);
            Assert.IsNull(db.Reservations.ElementAt(0).LeaveDateTime);
            Assert.IsNull(db.Reservations.ElementAt(0).EnterDateTime);
            Assert.IsNull(db.Reservations.ElementAt(0).Payment);
            Assert.AreEqual(activities[0].Price, db.Reservations.ElementAt(0).Price);
            Assert.AreEqual(profile.GlobalId, db.Reservations.ElementAt(0).Profile.GlobalId);
            Assert.Greater(db.Reservations.ElementAt(0).DateTime,DateTime.MinValue);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Make_StatusDone()
        {
            entries[0].State = ScheduleEntryState.Done;
            insertToDatabase(entries[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Make_StatusCancelled()
        {
            entries[0].State = ScheduleEntryState.Cancelled;
            insertToDatabase(entries[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        public void Make_ReturnResults()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                var res=Service.ReservationsOperation(data.Token, param);
                Assert.AreEqual(1,res.ScheduleEntry.Reservations.Count);
                Assert.AreEqual(res.Reservation.ScheduleEntryId, res.ScheduleEntry.GlobalId);
            });
        }

        [Test]
        public void Make_ProductNameSet()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });


            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(string.Format("{0}:{1}", activities[0].Name, entries[0].StartTime), db.Reservations.ElementAt(0).Name);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void MakeReservationTwiceForTheDifferentActivitiesByOneCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[3].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void MakeReservationTwiceForTheSameActivity()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

        }

        [Test]
        public void MakeReservation_ProductNameSet()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(string.Format("{0}:{1}",activities[0].Name,entries[0].StartTime), db.Reservations.ElementAt(0).Name);
        }

        [Test]
        public void MakeReservationTwiceForTheDifferentActivitiesByOneVirtualCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[3].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            var count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(2,count);
        }

        [Test]
        public void MakeReservationTwiceForTheSameActivity_VirtualCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
            var count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(2, count);

        }

        [Test]
        public void MakeReservationThreeForTheSameActivity_VirtualCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
            var count = Session.QueryOver<ScheduleEntryReservation>().RowCount();
            Assert.AreEqual(3, count);

        }
        #endregion

        #region Undo

        [Test]
        public void Undo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult res = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                res=Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = res.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                Service.ReservationsOperation(data.Token, param);
            });


            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(0, db.Reservations.Count);
        }

        [Test]
        public void Undo_ForChampionships()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var championship = CreateChampionship(profiles[0], "test");
            championship.State = ScheduleEntryState.Planned;
            insertToDatabase(championship);

            ReservationOperationResult res = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = championship.GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                res = Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = res.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                res=Service.ReservationsOperation(data.Token, param);
            });

            Assert.IsNotNull(res.ScheduleEntry as ScheduleChampionshipDTO);
            var db = Session.Get<Championship>(championship.GlobalId);
            Assert.AreEqual(0, db.Reservations.Count);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Undo_StatusDone()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult res = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                res = Service.ReservationsOperation(data.Token, param);
            });

            entries[0] = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            entries[0].State = ScheduleEntryState.Done;
            insertToDatabase(entries[0]);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = res.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        public void Undo_ForVirtualCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult res = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[3].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                res=Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = res.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                Service.ReservationsOperation(data.Token, param);
            });


            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(0, db.Reservations.Count);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void UndoReservationWithoutReservation()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                Service.ReservationsOperation(data.Token, param);
            });
        }
        #endregion

        #region Payment

        [Test]
        public void Payment_MakeReservation()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                var res = Service.ReservationsOperation(data.Token, param);
                Assert.AreEqual(activities[0].Price, res.Payment.Product.Price);
            });

        }

        [Test]
        public void Payment_MakeReservation_WithoutPrice()
        {
            entries[0].Price = 0;
            insertToDatabase(entries[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                var res = Service.ReservationsOperation(data.Token, param);
                Assert.IsNull(res.Payment);
            });
        }

        [Test]
        public void PayForReservation()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ScheduleEntryReservationDTO reservation = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                reservation=service.ReservationsOperation(data.Token, param).Reservation;
            });

            

            var koszyk = new PaymentBasketDTO();
            koszyk.CustomerId = customers[0].GlobalId;
            koszyk.TotalPrice = 20m;
            var zakup = new PaymentDTO();
            zakup.Count = 1;
            zakup.Product = reservation;
            koszyk.Payments.Add(zakup);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                koszyk = service.PaymentBasketOperation(data.Token, koszyk);
            });
            Assert.AreEqual(koszyk.GlobalId, koszyk.Payments[0].PaymentBasketId);
            GetScheduleEntriesParam param1 = new GetScheduleEntriesParam();
            param1.EntryId = entries[0].GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var entryData = service.GetScheduleEntries(data.Token,param1, new PartialRetrievingInfo());
                Assert.IsTrue(entryData.Items[0].Reservations.ElementAt(0).IsPaid);
            });
            var dbZakup = Session.Get<Payment>(koszyk.Payments[0].GlobalId);
            Assert.Greater(dbZakup.DateTime, DateTime.MinValue);
        }
        #endregion

        #region Statuses

        [Test]
        public void StatusDone()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.StatusDone;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual( ScheduleEntryState.Done, db.State);
        }

        [Test]
        public void StatusCancelled()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.StatusCancelled;
                Service.ReservationsOperation(data.Token, param);
            });

            var db = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            Assert.AreEqual(ScheduleEntryState.Cancelled, db.State);
        }

        #endregion

        #region Present and absent

        [Test]
        public void MakeAndPresent_StateDone()
        {
            entries[0].State = ScheduleEntryState.Planned;
            insertToDatabase(entries[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                result=Service.ReservationsOperation(data.Token, param);
            });
            entries[0] = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            entries[0].State = ScheduleEntryState.Done;
            insertToDatabase(entries[0]);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = result.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Presnet;
                result = Service.ReservationsOperation(data.Token, param);
                UnitTestHelper.CompareDateTime(entries[0].StartTime, result.Reservation.EnterDateTime.Value);
                UnitTestHelper.CompareDateTime(entries[0].EndTime, result.Reservation.LeaveDateTime.Value);
            });
            var dbReservation=Session.Get<ScheduleEntryReservation>(result.Reservation.GlobalId);
            UnitTestHelper.CompareDateTime(entries[0].StartTime, dbReservation.EnterDateTime.Value);
            UnitTestHelper.CompareDateTime(entries[0].EndTime, dbReservation.LeaveDateTime.Value);
        }

        [Test]
        public void MakePresentAbsent_StateDone()
        {
            entries[0].State = ScheduleEntryState.Planned;
            insertToDatabase(entries[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                result = Service.ReservationsOperation(data.Token, param);
            });
            entries[0] = Session.Get<ScheduleEntry>(entries[0].GlobalId);
            entries[0].State = ScheduleEntryState.Done;
            insertToDatabase(entries[0]);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = result.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Presnet;
                result = Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = result.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Absent;
                result = Service.ReservationsOperation(data.Token, param);
                Assert.IsNull(result.Reservation.EnterDateTime);
                Assert.IsNull( result.Reservation.LeaveDateTime);
            });
            var dbReservation = Session.Get<ScheduleEntryReservation>(result.Reservation.GlobalId);
            Assert.IsNull(dbReservation.EnterDateTime);
            Assert.IsNull(dbReservation.LeaveDateTime);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeAndPresent_StatePlanned()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customers[0].GlobalId;
                param.EntryId = entries[0].GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                result = Service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = result.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Presnet;
                result = Service.ReservationsOperation(data.Token, param);
                UnitTestHelper.CompareDateTime(entries[0].StartTime, result.Reservation.EnterDateTime.Value);
                UnitTestHelper.CompareDateTime(entries[0].EndTime, result.Reservation.LeaveDateTime.Value);
            });
            var dbReservation = Session.Get<ScheduleEntryReservation>(result.Reservation.GlobalId);
            UnitTestHelper.CompareDateTime(entries[0].StartTime, dbReservation.EnterDateTime.Value);
            UnitTestHelper.CompareDateTime(entries[0].EndTime, dbReservation.LeaveDateTime.Value);
        }

        #endregion
    }
}
