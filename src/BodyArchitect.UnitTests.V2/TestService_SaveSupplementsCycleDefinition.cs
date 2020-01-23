using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveSupplementsCycleDefinition:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Suplement> supplements = new List<Suplement>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                supplements.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var supplement = CreateSupplement("sup1");
                supplements.Add(supplement);
                supplement = CreateSupplement("steroids");
                supplement.CanBeIllegal = true;//steroids
                supplements.Add(supplement);
                tx.Commit();
            }
        }

        [Test]
        public void SaveNewDefinition()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(1);
                var savedDefinition = service.SaveSupplementsCycleDefinition(data.Token, definition);
                Assert.AreEqual(profile.GlobalId,savedDefinition.Profile.GlobalId);
                Assert.AreEqual(DateTime.UtcNow.Date.AddDays(1), savedDefinition.CreationDate);
                                     definition.CreationDate = savedDefinition.CreationDate;
                UnitTestHelper.CompareObjects(definition, savedDefinition,true);
                
                var dbDef = Session.Get<SupplementCycleDefinition>(savedDefinition.GlobalId);
                UnitTestHelper.CompareObjects(savedDefinition, dbDef.Map<SupplementCycleDefinitionDTO>());
            });
        }

        [Test]
        public void UpdateDefinition()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            definition.Name = "test1";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            Assert.AreEqual(1,Session.QueryOver<SupplementCycleDefinition>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<SupplementCycleDosage>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<SupplementCycleWeek>().RowCount());
            Assert.AreEqual("test1", definition.Name);
        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void UpdateDefinition_StatusPublic()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var def=CreateSupplementsCycleDefinition("def", supplements[0], profiles[0]);

            var dto = def.Map<SupplementCycleDefinitionDTO>();
            dto.Name = "fdgfd";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveSupplementsCycleDefinition(data.Token, dto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateDefinition_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });

            profile = (ProfileDTO)profiles[1].Tag;
            data = CreateNewSession(profile, ClientInformation);
            definition.Name = "test1";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateDefinition_OldData()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            definition.Name = "test1";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            definition.Name = "test2";
            definition.Version = 1;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
        }

        [Test]
        public void SaveNewDefinition_WithCanBeIllegalSupplements()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[1].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.Date.AddDays(1);
                var savedDefinition = service.SaveSupplementsCycleDefinition(data.Token, definition);
                Assert.AreEqual(true, savedDefinition.CanBeIllegal);
            });
        }

        [Test]
        public void UpdateDefinition_AddedCanBeIllegalSupplement()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            definition.Name = "test1";

            dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[1].Map<SuplementDTO>();
            definition.Weeks[0].Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            Assert.AreEqual(true, definition.CanBeIllegal);
        }

        [Test]
        public void UpdateDefinition_RemoveCanBeIllegalSupplement()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);

            dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[1].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            definition.Name = "test1";
            definition.Weeks[0].Dosages.RemoveAt(1);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                definition = service.SaveSupplementsCycleDefinition(data.Token, definition);
            });
            Assert.AreEqual(false, definition.CanBeIllegal);
        }

        #region DataInfo

        [Test]
        public void SaveNewDefinition_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var oldHash = profiles[0].DataInfo.SupplementsCycleDefinitionHash;

            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "name";
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = supplements[0].Map<SuplementDTO>();
            week.Dosages.Add(dosage);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveSupplementsCycleDefinition(data.Token, definition);
            });

            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.SupplementsCycleDefinitionHash);
        }
        #endregion
    }
}
