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
    public class TestService_AutomaticMeasurementsUpdate : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Customer> customers = new List<Customer>();
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                customers.Clear();
                var profile = CreateProfile(Session, "test1");
                profiles.Add(profile);
                profile.Settings.AutomaticUpdateMeasurements = true;
                Session.Update(profile);

                var customer = CreateCustomer("Cust1", profiles[0]);
                customer.Settings.AutomaticUpdateMeasurements = true;
                Session.Update(customer);
                customers.Add(customer);

                tx.Commit();
            }
        }

        #region For profile
        [Test]
        public void Test_SaveTrainingDay_AutomaticUpdateDisable()
        {
            profiles[0].Settings.AutomaticUpdateMeasurements = false;
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.IsNull(dbProfile.Wymiary);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO) profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.SaveTrainingDay(data.Token, day);
                                 });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(50, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimesDifferentSizes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Weight = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
            Assert.AreEqual(50, dbProfile.Wymiary.Weight);
        }

        [Test]
        public void Test_SaveTrainingDay_UpdateSize_TwoTimes_SecondInThePast()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }

        [Test]
        public void Test_DeleteTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                day = service.SaveTrainingDay(data.Token, day).TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                service.DeleteTrainingDay(data.Token, param);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(100, dbProfile.Wymiary.Height);
        }

        #endregion

        #region For customer
        [Test]
        public void ForCustomer_SaveTrainingDay_AutomaticUpdateDisable()
        {
            customers[0].Settings.AutomaticUpdateMeasurements = false;
            insertToDatabase(customers[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.IsNull(dbCustomer.Wymiary);
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.AreEqual(100, dbCustomer.Wymiary.Height);
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_UpdateSize_TwoTimes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.CustomerId = customers[0].GlobalId;
            day.ProfileId = profile.GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.AreEqual(50, dbCustomer.Wymiary.Height);
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_UpdateSize_TwoTimesDifferentSizes()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Weight = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.AreEqual(100, dbCustomer.Wymiary.Height);
            Assert.AreEqual(50, dbCustomer.Wymiary.Weight);
        }

        [Test]
        public void ForCustomer_SaveTrainingDay_UpdateSize_TwoTimes_SecondInThePast()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });
            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.AreEqual(100, dbCustomer.Wymiary.Height);
        }

        [Test]
        public void ForCustomer_DeleteTrainingDay_UpdateSize()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            TrainingDayDTO day = new TrainingDayDTO(DateTime.Now.AddDays(-2));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            SizeEntryDTO entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 100;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveTrainingDay(data.Token, day);
            });

            day = new TrainingDayDTO(DateTime.Now.AddDays(-1));
            day.ProfileId = profile.GlobalId;
            day.CustomerId = customers[0].GlobalId;
            entry = new SizeEntryDTO();
            entry.Wymiary = new WymiaryDTO();
            entry.Wymiary.Height = 50;
            day.AddEntry(entry);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                day = service.SaveTrainingDay(data.Token, day).TrainingDay;
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new DeleteTrainingDayParam();
                param.TrainingDayId = day.GlobalId;

                service.DeleteTrainingDay(data.Token, param);
            });

            var dbCustomer = Session.Get<Customer>(customers[0].GlobalId);
            Assert.AreEqual(100, dbCustomer.Wymiary.Height);
        }
        #endregion
    }
}
