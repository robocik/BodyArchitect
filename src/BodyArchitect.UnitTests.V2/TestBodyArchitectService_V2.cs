using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using NUnit.Framework;
using Gender = BodyArchitect.Service.V2.Model.Gender;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestBodyArchitectService_V2 : TestWcfServiceBase<IBodyArchitectAccessService,BodyArchitectAccessService>
    {
        List<Profile> profiles = new List<Profile>();
        private Guid key = new Guid("EB17BC2A-94FD-4E65-8751-15730F69E7F5");

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test_user"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles[0].Password = "pwd".ToSHA1Hash();

                APIKey apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();

            }
        }

        [Test]
        [ExpectedException(typeof(SecurityAccessDeniedException))]
        public void LoginWithoutAPIKey()
        {
            IBodyArchitectAccessService service = CreateServiceProxy();

            service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
        }

        [Test]
        public void LoginWithAPIKey()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            Assert.IsNotNull(sessionData);
        }

        #region Validation

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_Activity_ColorValidator()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var activity = new ActivityDTO();
            activity.Name = "test";
            activity.Color = "wrong";
            
            service.SaveActivity(sessionData.Token, activity);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveCustomer_FirstNameIsNull()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var customerDto = new CustomerDTO();
            customerDto.Gender = Gender.Male;
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email@net.com";
            customerDto.PhoneNumber = "234553";

            service.SaveCustomer(sessionData.Token, customerDto);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveCustomer_WrongEmail()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var customerDto = new CustomerDTO();
            customerDto.Gender = Gender.Male;
            customerDto.FirstName = "name";
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);
            customerDto.Email = "email";

            service.SaveCustomer(sessionData.Token, customerDto);
        }

        [Test]
        public void Validate_SaveCustomer_NullEmail()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var customerDto = new CustomerDTO();
            customerDto.Gender = Gender.Male;
            customerDto.FirstName = "name";
            customerDto.Birthday = DateTime.UtcNow.AddYears(-20);

            service.SaveCustomer(sessionData.Token, customerDto);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveScheduleEntryRangeParam_ActivityIdIsNull()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow;
            param.EndDay = DateTime.UtcNow.AddDays(2);
            var dto = new ScheduleEntryDTO();
            dto.StartTime = param.StartDay;
            dto.EndTime = dto.StartTime + TimeSpan.FromHours(1);
            param.Entries.Add(dto);
            service.SaveScheduleEntriesRange(sessionData.Token, param);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveScheduleEntryRangeParam_StartDayGreaterEndDay()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow;
            param.EndDay = DateTime.UtcNow.AddDays(-2);

            service.SaveScheduleEntriesRange(sessionData.Token, param);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveScheduleEntryRangeParam_CopyStartGreaterEnd()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow;
            param.EndDay = DateTime.UtcNow.AddDays(2);
            param.CopyStart = DateTime.UtcNow;
            param.CopyEnd = DateTime.UtcNow.AddDays(-2);

            service.SaveScheduleEntriesRange(sessionData.Token, param);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveScheduleEntryRangeParam_CopyStartNotNull_CopyEndNull()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow;
            param.EndDay = DateTime.UtcNow.AddDays(2);
            param.CopyStart = DateTime.UtcNow;
            param.CopyEnd = null;

            service.SaveScheduleEntriesRange(sessionData.Token, param);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void Validate_SaveScheduleEntryRangeParam_CopyStartNull_CopyEndNotNull()
        {
            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            var param = new SaveScheduleEntryRangeParam();
            param.StartDay = DateTime.UtcNow;
            param.EndDay = DateTime.UtcNow.AddDays(2);
            param.CopyStart = null;
            param.CopyEnd = DateTime.UtcNow; ;

            service.SaveScheduleEntriesRange(sessionData.Token, param);
        }

        #region Supplements cycle definition

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void CycleDefinition_WithoutName()
        {
            var res=CreateSupplement("Sup");
            var definition = new SupplementCycleDefinitionDTO();
            definition.Language = "en";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = res.Map<SuplementDTO>();
            week.Dosages.Add(dosage);


            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            service.SaveSupplementsCycleDefinition(sessionData.Token, definition);
            
        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void CycleDefinition_WithoutLanguage()
        {
            var res = CreateSupplement("Sup");
            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "test";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = res.Map<SuplementDTO>();
            week.Dosages.Add(dosage);


            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            service.SaveSupplementsCycleDefinition(sessionData.Token, definition);

        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void CycleDefinition_WithWrongLanguage()
        {
            var res = CreateSupplement("Sup");
            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "test";
            definition.Language = "gfgdf";
            var week = new SupplementCycleWeekDTO();
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = res.Map<SuplementDTO>();
            week.Dosages.Add(dosage);


            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            service.SaveSupplementsCycleDefinition(sessionData.Token, definition);

        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void CycleDefinition_WithWrongCycelWeekStartNumber()
        {
            var res = CreateSupplement("Sup");
            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "test";
            definition.Language = "gfgdf";
            var week = new SupplementCycleWeekDTO();
            week.CycleWeekStart = 2;
            week.CycleWeekEnd = 1;
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosageDTO();
            dosage.Supplement = res.Map<SuplementDTO>();
            week.Dosages.Add(dosage);


            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            service.SaveSupplementsCycleDefinition(sessionData.Token, definition);

        }

        [Test]
        [ExpectedException(typeof(FaultException<ValidationFault>))]
        public void CycleDefinition_WithoutWeeks()
        {
            var definition = new SupplementCycleDefinitionDTO();
            definition.Name = "test";
            definition.Language = "en";

            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            service.SaveSupplementsCycleDefinition(sessionData.Token, definition);

        }
        #endregion
        #endregion

        #region Bugs with WCF

        [Test]
        public void MyTrainingOperation_ProblemWithResultClass()
        {
            var res = CreateSupplement("Sup");
            var definition = new SupplementCycleDefinition();
            definition.Language = "en";
            definition.Profile = profiles[0];
            definition.Name = "dfgdfg";
            var week = new SupplementCycleWeek();
            week.CycleWeekStart = week.CycleWeekEnd = 1;
            week.Definition = definition;
            definition.Weeks.Add(week);
            var dosage = new SupplementCycleDosage();
            dosage.Week = week;
            dosage.Supplement = res;
            week.Dosages.Add(dosage);
            insertToDatabase(definition);

            DateTime date = new DateTime(2012, 03, 26);//monday
            var cycle = new SupplementsCycleDTO();
            cycle.TrainingDays = string.Format("{0}S,{1}S,{2}S", (int)DayOfWeek.Monday, (int)DayOfWeek.Wednesday, (int)DayOfWeek.Friday);
            cycle.Name = "Creatine";
            cycle.StartDate = date;
            cycle.SupplementsCycleDefinitionId = definition.GlobalId;
            var profile1 = (ProfileDTO)profiles[0].Tag;

            MyTrainingDTO result = null;
            MyTrainingOperationParam param = new MyTrainingOperationParam();
            param.Operation = MyTrainingOperationType.Start;
            param.MyTraining = cycle;

            IBodyArchitectAccessService service = CreateServiceProxy(AddressHeader.CreateAddressHeader("APIKey", "", "EB17BC2A-94FD-4E65-8751-15730F69E7F5"));

            var sessionData = service.Login(ClientInformation, "test_user", "pwd".ToSHA1Hash());
            result=service.MyTrainingOperation(sessionData.Token, param);

            Assert.AreEqual(7, result.EntryObjects.Count);
        }

        #endregion
    }
}
