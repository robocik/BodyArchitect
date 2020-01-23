using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;
using Gender = BodyArchitect.Service.V2.Model.Gender;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;
using ReminderType = BodyArchitect.Model.ReminderType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveCustomer : TestServiceBase
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

        #region Wymiary

        [Test]
        public void SaveNewCustomer_WithWymiary()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.Wymiary = new WymiaryDTO();
            customerDto.Wymiary.Weight = 22;

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(customerDto.Wymiary.Weight,savedCustomer.Wymiary.Weight);
            });
            var dbWymiary = Session.Get<Wymiary>(savedCustomer.Wymiary.GlobalId);
            Assert.AreEqual(customerDto.Wymiary.Weight, dbWymiary.Weight);
        }

        [Test]
        public void UpdateCustomer_WithWymiary()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Wymiary = new Wymiary();
            customer.Wymiary.Height = 100;
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Wymiary.Height = 50;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            var count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(1,count);
            var dbWymiary = Session.Get<Wymiary>(customer.Wymiary.GlobalId);
            Assert.AreEqual(50, dbWymiary.Height);
        }

        [Test]
        public void UpdateCustomer_SetNewhWymiary()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Wymiary = new Wymiary();
            customer.Wymiary.Height = 100;
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Wymiary=new WymiaryDTO();
            customerDto.Wymiary.Height = 50;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                customerDto=service.SaveCustomer(data.Token, customerDto);
            });
            var count = Session.QueryOver<Wymiary>().RowCount();
            Assert.AreEqual(1, count);
            var dbWymiary = Session.Get<Wymiary>(customerDto.Wymiary.GlobalId);
            Assert.AreEqual(50, dbWymiary.Height);
        }

        #endregion

        #region Settings

        [Test]
        public void SaveNewCustomer_WithSettings()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.Settings = new CustomerSettingsDTO();
            customerDto.Settings.AutomaticUpdateMeasurements = false;

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(customerDto.Settings.AutomaticUpdateMeasurements, savedCustomer.Settings.AutomaticUpdateMeasurements);
            });
            var dbSettings = Session.Get<CustomerSettings>(savedCustomer.Settings.GlobalId);
            Assert.AreEqual(customerDto.Settings.AutomaticUpdateMeasurements,dbSettings.AutomaticUpdateMeasurements);
        }

        [Test]
        public void UpdateCustomer_WithSettings()
        {
            var customer = CreateCustomer("name", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Settings.AutomaticUpdateMeasurements = true;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            var count = Session.QueryOver<CustomerSettings>().RowCount();
            Assert.AreEqual(1, count);
            var dbWymiary = Session.Get<CustomerSettings>(customer.Settings.GlobalId);
            Assert.IsTrue( dbWymiary.AutomaticUpdateMeasurements);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateCustomer_SetSettingsToNull()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Settings = new CustomerSettings();
            customer.Settings.AutomaticUpdateMeasurements = false;
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Settings = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                customerDto = service.SaveCustomer(data.Token, customerDto);
            });
        }

        [Test]
        public void UpdateCustomer_SetNewSettings()
        {
            var customer = CreateCustomer("name", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Settings = new CustomerSettingsDTO();
            customerDto.Settings.AutomaticUpdateMeasurements = true;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                customerDto = service.SaveCustomer(data.Token, customerDto);
            });
            var count = Session.QueryOver<CustomerSettings>().RowCount();
            Assert.AreEqual(1, count);
            var dbWymiary = Session.Get<CustomerSettings>(customerDto.Settings.GlobalId);
            Assert.IsTrue(dbWymiary.AutomaticUpdateMeasurements);
        }
        #endregion

        #region Birthday reminder

        [Test]
        public void SaveNewCustomer_WithBirthdayReminder()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.RemindBefore = TimeSpan.FromDays(1);

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(TimeSpan.FromDays(1),savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(savedCustomer.GlobalId);
            Assert.IsNotNull(dbCustomer.Reminder);
            Assert.AreEqual(profile.GlobalId, dbCustomer.Reminder.Profile.GlobalId);
            Assert.AreEqual("CustomerDTO:" + dbCustomer.GlobalId.ToString(), dbCustomer.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.EveryYear, dbCustomer.Reminder.Repetitions);
            Assert.AreEqual(ReminderType.Birthday, dbCustomer.Reminder.Type);
            Assert.AreEqual(customerDto.Birthday.Value.Date, dbCustomer.Reminder.DateTime);
        }

        [Test]
        public void SaveNewCustomer_WithBirthdayReminder_BirthdayNotSet()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = null;
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.RemindBefore = TimeSpan.FromDays(1);

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                Assert.IsNull(savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(savedCustomer.GlobalId);
            Assert.IsNull(dbCustomer.Reminder);
        }

        [Test]
        public void UpdateCustomer_RemindBefore_Set()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.RemindBefore = TimeSpan.FromDays(1);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual( TimeSpan.FromDays(1),savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNotNull(dbCustomer.Reminder);
            Assert.AreEqual(profile.GlobalId, dbCustomer.Reminder.Profile.GlobalId);
            Assert.AreEqual("CustomerDTO:" + dbCustomer.GlobalId.ToString(), dbCustomer.Reminder.ConnectedObject);
            Assert.AreEqual(ReminderRepetitions.EveryYear, dbCustomer.Reminder.Repetitions);
            Assert.AreEqual(customerDto.Birthday.Value.Date, dbCustomer.Reminder.DateTime);
            Assert.AreEqual(ReminderType.Birthday, dbCustomer.Reminder.Type);
        }

        [Test]
        public void UpdateCustomer_RemindBefore_Set_ChangeBirthdayDate()
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
            customerDto.Birthday = DateTime.UtcNow.Date.AddYears(-20).Date;
            customerDto.RemindBefore = TimeSpan.FromDays(1);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(TimeSpan.FromDays(1), savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNotNull(dbCustomer.Reminder);
            Assert.AreEqual(profile.GlobalId, dbCustomer.Reminder.Profile.GlobalId);
            Assert.AreEqual(ReminderRepetitions.EveryYear, dbCustomer.Reminder.Repetitions);
            Assert.AreEqual("CustomerDTO:" + dbCustomer.GlobalId.ToString(), dbCustomer.Reminder.ConnectedObject);
            Assert.AreEqual(customerDto.Birthday.Value.Date, dbCustomer.Reminder.DateTime);
            Assert.AreEqual(1, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void UpdateCustomer_RemindBefore_NotSet()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Reminder = new ReminderItem();
            customer.Reminder.Name = "test";
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.RemindBefore = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                Assert.IsNull(savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNull(dbCustomer.Reminder);
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }

        [Test]
        public void UpdateCustomer_RemindBefore_Set_BirthdaySetToNull()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Reminder = new ReminderItem();
            customer.Reminder.Name = "test";
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Birthday = null;
            customerDto.RemindBefore = TimeSpan.FromDays(1);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                Assert.IsNull(savedCustomer.RemindBefore);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNull(dbCustomer.Reminder);
            Assert.AreEqual(0, Session.QueryOver<ReminderItem>().RowCount());
        }
        #endregion

        [Test]
        public void SaveNewCustomer()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";

            CustomerDTO savedCustomer=null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                savedCustomer = Service.SaveCustomer(data.Token, customerDto);
                customerDto.ProfileId = savedCustomer.ProfileId;
            });
            UnitTestHelper.CompareObjects(customerDto,savedCustomer,true);
            Assert.AreEqual(profile.GlobalId,customerDto.ProfileId);

            var dbCustomer = Session.Get<Customer>(savedCustomer.GlobalId);
            UnitTestHelper.CompareObjects(savedCustomer, dbCustomer.Map<CustomerDTO>());
            Assert.IsNotNull(dbCustomer.Settings);
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateCustomer_OldData()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Version = 0;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
        }

        [Test]
        public void UpdateCustomer()
        {
            var customer=CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO) profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                UnitTestHelper.CompareObjects(customerDto, savedCustomer, true);
            });
            Assert.AreEqual(1,Session.QueryOver<Customer>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateCustomer_IsVirtualChanged()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.IsVirtual = true;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateCustomer_OtherProfile()
        {
            var customer = CreateCustomer("name", profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.ProfileId = profile.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
        }

        [Test]
        public void UpdateCustomer_ConnectedAccount()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount =(ProfileDTO) profiles[1].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedDto=service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(profiles[1].GlobalId, savedDto.ConnectedAccount.GlobalId);
            });

            var db=Session.Get<Customer>(customerDto.GlobalId);
            Assert.AreEqual(profiles[1].GlobalId, db.ConnectedAccount.GlobalId);
        }

        [Test]
        public void UpdateCustomer_ConnectedAccount_CheckCascadeNone()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount = (ProfileDTO)profiles[1].Tag;
            customerDto.ConnectedAccount.UserName = "check";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedDto = service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(profiles[1].GlobalId, savedDto.ConnectedAccount.GlobalId);
            });

            var db = Session.Get<Profile>(customerDto.ConnectedAccount.GlobalId);
            Assert.AreEqual(profiles[1].UserName, db.UserName);
        }

        [Test]
        public void UpdateCustomer_ConnectedAccount_UserSearchBug()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount = profiles[1].Map<UserSearchDTO>();
            customerDto.ConnectedAccount.UserName = "check";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedDto = service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual(profiles[1].GlobalId, savedDto.ConnectedAccount.GlobalId);
            });
        }

        [Test]
        public void UpdateCustomer_SetConnectedAccountToNull()
        {
            var customer = CreateCustomer("name", profiles[0], profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedDto = service.SaveCustomer(data.Token, customerDto);
                Assert.IsNull(savedDto.ConnectedAccount);
            });

            var db = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNull(db.ConnectedAccount);
        }

        [Test]
        [ExpectedException(typeof(AlreadyOccupiedException))]
        public void UpdateCustomer_TwoCustomersWithOneConnectedAccount()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var customer1 = CreateCustomer("name1", profiles[0],profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount = (ProfileDTO)profiles[1].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
        }

        [Test]
        public void UpdateCustomer_TwoCustomersWithOneConnectedAccount_OtherProfile()
        {
            var customer = CreateCustomer("name", profiles[0]);
            CreateCustomer("name1", profiles[1], profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.ConnectedAccount = (ProfileDTO)profiles[1].Tag;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            var db=Session.Get<Customer>(customer.GlobalId);
            Assert.AreEqual(profiles[1].GlobalId, db.ConnectedAccount.GlobalId);
        }

        [Test]
        public void RemoveImage()
        {
            var customer = CreateCustomer("name", profiles[0]);

            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(ImagesFolder, pictureId.ToString())).Close();
            customer.Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var customerDto = customer.Map<CustomerDTO>();
                customerDto.Picture = null;
                service.SaveCustomer(data.Token, customerDto);
            });
            Assert.IsFalse(File.Exists(Path.Combine(ImagesFolder, pictureId.ToString())));
        }

        [Test]
        public void ShouldntRemoveImage()
        {
            var customer = CreateCustomer("name", profiles[0]);
            Guid pictureId = Guid.NewGuid();
            File.CreateText(Path.Combine(ImagesFolder, pictureId.ToString())).Close();
            customer.Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(customer);

            UnitTestHelper.SetProperty(profiles[0].Tag, "Version", profiles[0].Version);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var customerDto = customer.Map<CustomerDTO>();
                customerDto.Picture = new PictureInfoDTO(pictureId, "fdgdf");
                service.SaveCustomer(data.Token, customerDto);
            });
            Assert.IsTrue(File.Exists(Path.Combine(ImagesFolder, pictureId.ToString())));
        }

        [Test]
        public void ExceptionDuringRemoveImage()
        {
            var customer = CreateCustomer("name", profiles[0]);
            Guid pictureId = Guid.NewGuid();
            var file = File.CreateText(Path.Combine(ImagesFolder, pictureId.ToString()));
            customer.Picture = new Picture(pictureId, "dfgdfgdfg");
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var customerDto = customer.Map<CustomerDTO>();
                customerDto.Picture = null;
                customerDto.PhoneNumber = "123";
                service.SaveCustomer(data.Token, customerDto);

            });
            Assert.IsTrue(File.Exists(Path.Combine(ImagesFolder, pictureId.ToString())));
            var dbCustomer = Session.Get<Customer>(customer.GlobalId);
            Assert.AreEqual("123", dbCustomer.PhoneNumber);
            file.Close();
        }

        #region Address

        [Test]
        public void SaveNewCustomerWithAddress()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.Address = new AddressDTO();
            customerDto.Address.City = "Miasto";
            customerDto.Address.Country = "country";
            customerDto.Address.Address1 = "address1";
            customerDto.Address.Address2 = "address2";
            customerDto.Address.PostalCode = "12345";

            CustomerDTO savedCustomer = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedCustomer = service.SaveCustomer(data.Token, customerDto);
                customerDto.ProfileId = savedCustomer.ProfileId;
            });
            UnitTestHelper.CompareObjects(customerDto, savedCustomer, true);
            Assert.AreEqual(profile.GlobalId, customerDto.ProfileId);

            var dbCustomer = Session.Get<Customer>(savedCustomer.GlobalId);
            UnitTestHelper.CompareObjects(savedCustomer, dbCustomer.Map<CustomerDTO>());
            Assert.IsNotNull(dbCustomer.Address);
        }

        [Test]
        public void UpdateCustomer_AddedAddress()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.Address = new AddressDTO();
            customerDto.Address.City = "Miasto";
            customerDto.Address.Country = "country";
            customerDto.Address.Address1 = "address1";
            customerDto.Address.Address2 = "address2";
            customerDto.Address.PostalCode = "12345";

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                UnitTestHelper.CompareObjects(customerDto, savedCustomer, true);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNotNull(dbCustomer.Address);
        }

        [Test]
        public void UpdateCustomer_AddedEmptyAddress()
        {
            var customer = CreateCustomer("name", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.Address = new AddressDTO();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                UnitTestHelper.CompareObjects(customerDto, savedCustomer, true);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNull(dbCustomer.Address);
            var count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void UpdateCustomer_ExistingEmptyAddress()
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
            customerDto.Address.City = "";
            customerDto.Address.Country = "";
            customerDto.Address.Address1 = "";
            customerDto.Address.Address2 = "";
            customerDto.Address.PostalCode = "";

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.IsNull(dbCustomer.Address);
            var count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void UpdateCustomer_ExistingAddress()
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
            customerDto.Address.City = "newCity";

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var savedCustomer = service.SaveCustomer(data.Token, customerDto);
                Assert.AreEqual("newCity",savedCustomer.Address.City);
            });
            var count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(1, count);
            var dbCustomer = Session.Get<Customer>(customerDto.GlobalId);
            Assert.AreEqual("newCity", dbCustomer.Address.City);
        }

        [Test]
        public void UpdateCustomer_SetNewAddress()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Address = new Address();
            customer.Address.City = "Miasto";
            insertToDatabase(customer);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Address = new AddressDTO();
            customerDto.Address.City = "Miasto11";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                customerDto = service.SaveCustomer(data.Token, customerDto);
            });
            var count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(1, count);
            var dbAddress = Session.Get<Address>(customerDto.Address.GlobalId);
            Assert.AreEqual("Miasto11", dbAddress.City);
        }
        #endregion


        [Test]
        public void SaveNewCustomer_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";

            var oldCustomerHash = profiles[0].DataInfo.CustomerHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveCustomer(data.Token, customerDto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldCustomerHash, dbProfile.DataInfo.CustomerHash);
            Assert.AreEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void SaveNewCustomer_WithBirthdayReminder_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = new CustomerDTO();
            customerDto.LastName = customerDto.FirstName = "name";
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";
            customerDto.RemindBefore = TimeSpan.FromDays(1);

            var oldCustomerHash = profiles[0].DataInfo.CustomerHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveCustomer(data.Token, customerDto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldCustomerHash, dbProfile.DataInfo.CustomerHash);
            Assert.AreNotEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }

        [Test]
        public void UpdateCustomer_RemindBefore_Set_BirthdaySetToNull_DataInfo()
        {
            var customer = CreateCustomer("name", profiles[0]);
            customer.Reminder = new ReminderItem();
            customer.Reminder.Name = "test";
            customer.Birthday = DateTime.UtcNow.AddYears(-10);
            insertToDatabase(customer);

            var oldCustomerHash = profiles[0].DataInfo.CustomerHash;
            var oldReminderHash = profiles[0].DataInfo.ReminderHash;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var customerDto = customer.Map<CustomerDTO>();
            customerDto.LastName = "new last";
            customerDto.Birthday = null;
            customerDto.RemindBefore = TimeSpan.FromDays(1);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveCustomer(data.Token, customerDto);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldCustomerHash, dbProfile.DataInfo.CustomerHash);
            Assert.AreNotEqual(oldReminderHash, dbProfile.DataInfo.ReminderHash);
        }
    }
}
