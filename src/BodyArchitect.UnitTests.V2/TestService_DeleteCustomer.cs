using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_DeleteCustomer : TestServiceBase
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
        public void SimpleDeleteCustomer()
        {
            var customer = CreateCustomer("name", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(),Is.EqualTo(0));
        }

        [Test]
        public void DeleteCustomer_WithBirthdayReminder()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            customer.Reminder = new ReminderItem();
            customer.Reminder.Name = "test";
            customer.Reminder.DateTime = customer.Birthday.Value.Date;
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<ReminderItem>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteCustomer_WithCalendarEntries()
        {
            var customer = CreateCustomer("name", profiles[0]);
            TrainingDay day = new TrainingDay(DateTime.Now);
            day.Profile = profiles[0];
            day.Customer = customer;
            BlogEntry blogEntry = new BlogEntry();
            day.AddEntry(blogEntry);
            insertToDatabase(day);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(), Is.EqualTo(0));
            Assert.That(Session.QueryOver<TrainingDay>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteCustomer_WithExerciseProfileData()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var exercise = CreateExercise(Session, null, "test", "test");
            var trainingDay = new TrainingDay(DateTime.Now);
            trainingDay.Customer = customer;
            trainingDay.Profile = profiles[0];
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);
            insertToDatabase(trainingDay);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(1, 22), trainingDay.TrainingDate,customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(), Is.EqualTo(0));
            Assert.That(Session.QueryOver<ExerciseProfileData>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        [ExpectedException(typeof(GenericADOException))]
        public void DeleteCustomer_WithReservation()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            var activity = CreateActivity("test", profiles[0]);
            ScheduleEntry entry = new ScheduleEntry();
            entry.Activity = activity;
            entry.Profile = profiles[0];
            entry.MyPlace = GetDefaultMyPlace(profiles[0]);
            entry.StartTime = DateTime.Now;
            entry.EndTime = DateTime.Now.AddHours(1);
            insertToDatabase(entry);

            ScheduleEntryReservation reservation = new ScheduleEntryReservation();
            reservation.Customer = customer;
            reservation.Name = "test";
            reservation.DateTime = DateTime.Now;
            reservation.Profile = profiles[0];
            reservation.ScheduleEntry = entry;
            insertToDatabase(reservation);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
        }

        [Test]
        public void DeleteCustomer_WithMyTrainings()
        {
            var customer = CreateCustomer("name", profiles[0]);
            A6WTraining myTraining = new A6WTraining();

            myTraining.Name = "dfgdfg";
            myTraining.Customer = customer;
            myTraining.Profile = profiles[0];
            insertToDatabase(myTraining);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(), Is.EqualTo(0));
            Assert.That(Session.QueryOver<A6WTraining>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteCustomer_MemberOfAGroup()
        {
            
            var customer = CreateCustomer("name", profiles[0]);
            CreateCustomerGroup("gr", profiles[0],10,customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Customer>().RowCount(), Is.EqualTo(0));
            Assert.That(Session.QueryOver<CustomerGroup>().RowCount(), Is.EqualTo(1));
        }

        [Test]
        public void DeleteCustomer_WithAddress()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Address = new Address();
            customer.Address.City = "Miasto";
            customer.Address.Country = "country";
            customer.Address.Address1 = "address1";
            customer.Address.Address2 = "address2";
            customer.Address.PostalCode = "12345";
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            Assert.That(Session.QueryOver<Address>().RowCount(), Is.EqualTo(0));
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteCustomer_OtherUser()
        {
            var customer = CreateCustomer("name", profiles[0]);

            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var dto = customer.Map<CustomerDTO>();
                service.DeleteCustomer(data.Token, dto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteCustomer_SecurityBug()
        {
            var customer = CreateCustomer("name", profiles[0]);

            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var dto = customer.Map<CustomerDTO>();
                dto.ProfileId = profile.GlobalId;
                service.DeleteCustomer(data.Token, dto);
            });
        }

        [Test]
        public void DeleteCustomer_DataInfo()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var oldCustomerHash = profiles[0].DataInfo.CustomerHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldCustomerHash, dbProfile.DataInfo.CustomerHash);
            Assert.AreEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }



        [Test]
        public void DeleteCustomer_WithBirthdayReminder_DataInfo()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            customer.Reminder = new ReminderItem();
            customer.Reminder.Name = "test";
            customer.Reminder.DateTime = customer.Birthday.Value.Date;
            insertToDatabase(customer);

            var oldCustomerHash = profiles[0].DataInfo.CustomerHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.DeleteCustomer(data.Token, customerDto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldCustomerHash, dbProfile.DataInfo.CustomerHash);
            Assert.AreNotEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }
    }
}
