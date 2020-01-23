using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Exceptions;
using NUnit.Framework;
using ChampionshipCategoryType = BodyArchitect.Model.ChampionshipCategoryType;
using ChampionshipCustomerType = BodyArchitect.Service.V2.Model.ChampionshipCustomerType;
using ChampionshipTryResult = BodyArchitect.Service.V2.Model.ChampionshipTryResult;
using ChampionshipType = BodyArchitect.Model.ChampionshipType;
using ChampionshipWinningCategories = BodyArchitect.Model.ChampionshipWinningCategories;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using Gender = BodyArchitect.Model.Gender;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;
using ScheduleEntryState = BodyArchitect.Model.ScheduleEntryState;
using SetType = BodyArchitect.Model.SetType;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_Championships : TestServiceBase
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveNewChampionship()
        {
            var champ = new ChampionshipDTO();
            champ.Name = "name";
            var champDTO = champ.Map<ChampionshipDTO>();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SaveChampionship_OtherUser()
        {
            var champ = CreateChampionship(profiles[1],"test");
            var champDTO = champ.Map<ChampionshipDTO>();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_StatusNotDone()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));

            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test",state:ScheduleEntryState.Planned);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            champDTO.Customers.Add(custDto);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = custDto;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        [ExpectedException(typeof(ObjectNotFoundException))]
        public void Save_CustomerWithoutReservations()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));

            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test");

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            champDTO.Customers.Add(custDto);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = custDto;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        public void Save_PerserveReservations()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));

            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, cust);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            champDTO.Customers.Add(custDto);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = custDto;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(1, result.Championship.Reservations.Count);
            var dbChamp=Session.Get<Championship>(result.Championship.GlobalId);
            Assert.AreEqual(1,dbChamp.Reservations.Count);
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void Save_Trojboj_CorrectExercise()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var wrongSqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("1106a130-b811-4e45-9285-f087403615bf"));

            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, cust);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            champDTO.Customers.Add(custDto);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = custDto;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);

            ChampionshipEntryDTO entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = custDto;
            entry1.Try1.Result = ChampionshipTryResult.Fail;
            entry1.Try1.Weight = 40;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = 50;
            champDTO.Entries.Add(entry1);

            ChampionshipEntryDTO entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = wrongSqad.Map<ExerciseLightDTO>();
            entry2.Customer = custDto;
            entry2.Try1.Result = ChampionshipTryResult.NotDone;
            entry2.Try1.Weight = 60;
            entry2.Try2.Result = ChampionshipTryResult.Fail;
            entry2.Try2.Weight = 70;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = 80;
            champDTO.Entries.Add(entry2);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveChampionship(data.Token, champDTO);
            });
        }

        [Test]
        public void Save_SetTotalAndWilksForCustomer()
        {
            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, cust);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            //we set these values but they should be calculated on the server
            custDto.Total = 101;
            custDto.TotalWilks = 102;
            champDTO.Customers.Add(custDto);

     

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(0, result.Championship.Customers[0].Total);
            Assert.AreEqual(0, result.Championship.Customers[0].TotalWilks);
        }

        [Test]
        public void Save_CalculateMaxAndTotalForCustomer()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var cust = CreateCustomer("Test", profiles[0]);
            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, cust);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO custDto = new ChampionshipCustomerDTO();
            custDto.CustomerId = cust.GlobalId;
            champDTO.Customers.Add(custDto);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = custDto;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);

            ChampionshipEntryDTO entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = custDto;
            entry1.Try1.Result = ChampionshipTryResult.Fail;
            entry1.Try1.Weight = 40;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = 50;
            champDTO.Entries.Add(entry1);

            ChampionshipEntryDTO entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = sqad.Map<ExerciseLightDTO>();
            entry2.Customer = custDto;
            entry2.Try1.Result = ChampionshipTryResult.NotDone;
            entry2.Try1.Weight = 60;
            entry2.Try2.Result = ChampionshipTryResult.Fail;
            entry2.Try2.Weight = 70;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = 80;
            champDTO.Entries.Add(entry2);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result=service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(60,result.Championship.Entries.Where(x=>x.Exercise.GlobalId==benchPress.GlobalId).Single().Max);
            Assert.AreEqual(50, result.Championship.Entries.Where(x => x.Exercise.GlobalId == deadlift.GlobalId).Single().Max);
            Assert.AreEqual(80, result.Championship.Entries.Where(x => x.Exercise.GlobalId == sqad.GlobalId).Single().Max);

            Assert.AreEqual(190, result.Championship.Customers[0].Total);
        }

        
        [Test]
        public void Save_CalculateWilksForCustomer_WomanAndMan()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male = CreateCustomer("Test", profiles[0],gender:Gender.Male);
            var female = CreateCustomer("Test1", profiles[0], gender: Gender.Female);

            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, male);
            CreateReservation(champ, female);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO maleDTO = new ChampionshipCustomerDTO();
            maleDTO.CustomerId = male.GlobalId;
            maleDTO.Weight = 56;
            champDTO.Customers.Add(maleDTO);

            ChampionshipCustomerDTO femaleDTO = new ChampionshipCustomerDTO();
            femaleDTO.CustomerId = female.GlobalId;
            femaleDTO.Weight = 56;
            champDTO.Customers.Add(femaleDTO);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = maleDTO;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);

            ChampionshipEntryDTO entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = maleDTO;
            entry1.Try1.Result = ChampionshipTryResult.Fail;
            entry1.Try1.Weight = 40;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = 50;
            champDTO.Entries.Add(entry1);

            ChampionshipEntryDTO entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = sqad.Map<ExerciseLightDTO>();
            entry2.Customer = maleDTO;
            entry2.Try1.Result = ChampionshipTryResult.NotDone;
            entry2.Try1.Weight = 60;
            entry2.Try2.Result = ChampionshipTryResult.Fail;
            entry2.Try2.Weight = 70;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = 80;
            champDTO.Entries.Add(entry2);

            //for female
            entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = femaleDTO;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);

            entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = femaleDTO;
            entry1.Try1.Result = ChampionshipTryResult.Fail;
            entry1.Try1.Weight = 40;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = 50;
            champDTO.Entries.Add(entry1);

            entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = sqad.Map<ExerciseLightDTO>();
            entry2.Customer = femaleDTO;
            entry2.Try1.Result = ChampionshipTryResult.NotDone;
            entry2.Try1.Weight = 60;
            entry2.Try2.Result = ChampionshipTryResult.Fail;
            entry2.Try2.Weight = 70;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = 80;
            champDTO.Entries.Add(entry2);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(54.618m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == benchPress.GlobalId).Single().Wilks);
            Assert.AreEqual(45.515m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == deadlift.GlobalId).Single().Wilks);
            Assert.AreEqual(72.824m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == sqad.GlobalId).Single().Wilks);

            Assert.AreEqual(172.957m, result.Championship.Customers[0].TotalWilks);

            Assert.AreEqual(70.596m, result.Championship.Entries.Where(x => x.Customer.CustomerId == female.GlobalId && x.Exercise.GlobalId == benchPress.GlobalId).Single().Wilks);
            Assert.AreEqual(58.83m, result.Championship.Entries.Where(x => x.Customer.CustomerId == female.GlobalId && x.Exercise.GlobalId == deadlift.GlobalId).Single().Wilks);
            Assert.AreEqual(94.128m, result.Championship.Entries.Where(x => x.Customer.CustomerId == female.GlobalId && x.Exercise.GlobalId == sqad.GlobalId).Single().Wilks);

            Assert.AreEqual(223.554m, result.Championship.Customers[1].TotalWilks);
        }

        [Test]
        public void Save_CalculateWilksForCustomer_WithZeroBodyWeight()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male = CreateCustomer("Test", profiles[0], gender: Gender.Male);

            var champ = CreateChampionship(profiles[0], "test");
            CreateReservation(champ, male);

            var champDTO = champ.Map<ChampionshipDTO>();
            ChampionshipCustomerDTO maleDTO = new ChampionshipCustomerDTO();
            maleDTO.CustomerId = male.GlobalId;
            maleDTO.Weight = 0;
            champDTO.Customers.Add(maleDTO);


            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = maleDTO;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = 60;
            entry.Try2.Result = ChampionshipTryResult.Fail;
            entry.Try2.Weight = 70;
            entry.Try3.Result = ChampionshipTryResult.NotDone;
            entry.Try3.Weight = 80;
            champDTO.Entries.Add(entry);

            ChampionshipEntryDTO entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = maleDTO;
            entry1.Try1.Result = ChampionshipTryResult.Fail;
            entry1.Try1.Weight = 40;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = 50;
            champDTO.Entries.Add(entry1);

            ChampionshipEntryDTO entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = sqad.Map<ExerciseLightDTO>();
            entry2.Customer = maleDTO;
            entry2.Try1.Result = ChampionshipTryResult.NotDone;
            entry2.Try1.Weight = 60;
            entry2.Try2.Result = ChampionshipTryResult.Fail;
            entry2.Try2.Weight = 70;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = 80;
            champDTO.Entries.Add(entry2);
            

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(0m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == benchPress.GlobalId).Single().Wilks);
            Assert.AreEqual(0m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == deadlift.GlobalId).Single().Wilks);
            Assert.AreEqual(0m, result.Championship.Entries.Where(x => x.Customer.CustomerId == male.GlobalId && x.Exercise.GlobalId == sqad.GlobalId).Single().Wilks);

            Assert.AreEqual(0m, result.Championship.Customers[0].TotalWilks);
        }

        #region Results


        ChampionshipCustomerDTO addCustomerEntries(ChampionshipDTO champDTO, Customer customer, decimal weight, decimal ex1Weight, decimal ex2Weight, decimal ex3Weight, ChampionshipGroupDTO customerGroup = null)
        {

            var benchPress = Session.Get<Exercise>(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = Session.Get<Exercise>(new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = Session.Get<Exercise>(new Guid("3e06a130-b811-4e45-9285-f087403615bf"));
            
            ChampionshipCustomerDTO maleDTO = new ChampionshipCustomerDTO();
            maleDTO.CustomerId = customer.GlobalId;
            maleDTO.Weight = weight;
            maleDTO.Group = customerGroup;
            champDTO.Customers.Add(maleDTO);

            ChampionshipEntryDTO entry = new ChampionshipEntryDTO();
            entry.Exercise = benchPress.Map<ExerciseLightDTO>();
            entry.Customer = maleDTO;
            entry.Try1.Result = ChampionshipTryResult.Success;
            entry.Try1.Weight = ex1Weight;
            champDTO.Entries.Add(entry);

            ChampionshipEntryDTO entry1 = new ChampionshipEntryDTO();
            entry1.Exercise = deadlift.Map<ExerciseLightDTO>();
            entry1.Customer = maleDTO;
            entry1.Try2.Result = ChampionshipTryResult.Success;
            entry1.Try2.Weight = ex2Weight;
            champDTO.Entries.Add(entry1);

            ChampionshipEntryDTO entry2 = new ChampionshipEntryDTO();
            entry2.Exercise = sqad.Map<ExerciseLightDTO>();
            entry2.Customer = maleDTO;
            entry2.Try3.Result = ChampionshipTryResult.Success;
            entry2.Try3.Weight = ex3Weight;
            champDTO.Entries.Add(entry2);

            return maleDTO;
        }

        [Test]
        public void Save_SetResultsFromTheClient()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            ChampionshipCustomerDTO maleDTO = new ChampionshipCustomerDTO();
            maleDTO.CustomerId = male1.GlobalId;
            maleDTO.Weight = 100;
            champDTO.Customers.Add(maleDTO);

            ChampionshipResultItemDTO resultItem = new ChampionshipResultItemDTO();
            resultItem.Customer = maleDTO;
            resultItem.Position = 1;
            resultItem.Category = champDTO.Categories[0];
            champDTO.Results.Add(resultItem);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            Assert.AreEqual(0, result.Championship.Results.Count);
        }

        [Test]
        public void Save_Results_Male_Female()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test",categories:new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Open,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male2, 119.30m, 400, 280, 390);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65);
            addCustomerEntries(champDTO, female2, 60, 120, 62.5m, 132.5m);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Position);
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Value);
            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Position);
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Value);
            Assert.AreEqual(2, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Position);
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Value);

            var females = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female).ToList();
            
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Position);
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Value);
            Assert.AreEqual(1, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Value);
            Assert.AreEqual(2, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Value);
        }

        [Test]
        public void Save_Results_Male_Female_SaveTwice()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Open,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male2, 119.30m, 400, 280, 390);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65);
            addCustomerEntries(champDTO, female2, 60, 120, 62.5m, 132.5m);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            int firstResultsCount = Session.QueryOver<ChampionshipResultItem>().RowCount();

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });

            Assert.AreEqual(firstResultsCount, Session.QueryOver<ChampionshipResultItem>().RowCount());
        }

        [Test]
        public void Save_Results_MaleWeight_FemaleWeight()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Weight,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
            }
            );

            //var champ = CreateChampionship(profiles[0], "test",winnerCategories:Model.ChampionshipWinnerCategories.FemaleWeight | Model.ChampionshipWinnerCategories.MaleWeight);
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male2, 119.30m, 400, 280, 390);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65);
            addCustomerEntries(champDTO, female2, 60, 120, 62.5m, 132.5m);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female4, 64, 60, 35, 90);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var category = result.Championship.Categories.Where(x => x.Gender == Service.V2.Model.Gender.Male &&
                    x.Category == Service.V2.Model.ChampionshipWinningCategories.Seniorzy &&
                    x.Type == Service.V2.Model.ChampionshipCategoryType.Weight).Single();
            var males = result.Championship.Results.Where(x => x.Category.Type == Service.V2.Model.ChampionshipCategoryType.Open).ToList();
            Assert.AreEqual(0,males.Count);
            males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Position);
            Assert.AreEqual(52, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Value);
            Assert.AreEqual(category, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Category);

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Position);
            Assert.AreEqual(125, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Value);
            Assert.AreEqual(category, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Category);

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Position);
            Assert.AreEqual(100, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Value);
            Assert.AreEqual(category, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Category);

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Position);
            Assert.AreEqual(56, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Value);
            Assert.AreEqual(category, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Category);

            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == male5.GlobalId).Position);
            Assert.AreEqual(56, males.Single(x => x.Customer.CustomerId == male5.GlobalId).Value);
            Assert.AreEqual(category, males.Single(x => x.Customer.CustomerId == male5.GlobalId).Category);

            category = result.Championship.Categories.Where(x => x.Gender == Service.V2.Model.Gender.Female &&
                    x.Category == Service.V2.Model.ChampionshipWinningCategories.Seniorzy &&
                    x.Type == Service.V2.Model.ChampionshipCategoryType.Weight).Single();
            var females = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female).ToList();
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);
            Assert.AreEqual(44, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Value);
            Assert.AreEqual(category, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Category);

            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Position);
            Assert.AreEqual(60, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Value);
            Assert.AreEqual(category, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Category);

            Assert.AreEqual(2, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Value);
            Assert.AreEqual(category, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Category);

            Assert.AreEqual(1, females.Single(x => x.Customer.CustomerId == female4.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female4.GlobalId).Value);
            Assert.AreEqual(category, females.Single(x => x.Customer.CustomerId == female4.GlobalId).Category);

            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female5.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female5.GlobalId).Value);
            Assert.AreEqual(category, females.Single(x => x.Customer.CustomerId == female5.GlobalId).Category);
        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-16));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-17));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-19));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-21));
            var female6 = CreateCustomer("Test16", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-22));
            var female7 = CreateCustomer("Test17", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.JuniorzyMlodsi,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.JuniorzyMlodsi,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Juniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Juniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);
            CreateReservation(champ, male6);
            CreateReservation(champ, female6);
            CreateReservation(champ, male7);
            CreateReservation(champ, female7);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male2, 119.30m, 400, 280, 390);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65);
            addCustomerEntries(champDTO, female2, 60, 120, 62.5m, 132.5m);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female4, 64, 60, 35, 90);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female6, 62.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female7, 61.7m, 60, 35, 90);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.JuniorzyMlodsi).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Position);
            Assert.AreEqual(52, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Value);

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Position);
            Assert.AreEqual(125, males.Single(x => x.Customer.CustomerId == male2.GlobalId).Value);

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Position);
            Assert.AreEqual(100, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Value);

            males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Juniorzy).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Position);
            Assert.AreEqual(56, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Value);

            Assert.AreEqual(2, males.Single(x => x.Customer.CustomerId == male5.GlobalId).Position);
            Assert.AreEqual(56, males.Single(x => x.Customer.CustomerId == male5.GlobalId).Value);

            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == male6.GlobalId).Position);
            Assert.AreEqual(56, males.Single(x => x.Customer.CustomerId == male6.GlobalId).Value);

            Assert.IsNull( males.SingleOrDefault(x => x.Customer.CustomerId == male7.GlobalId));


            var females = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.JuniorzyMlodsi).ToList();
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);
            Assert.AreEqual(44, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Value);

            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Position);
            Assert.AreEqual(60, females.Single(x => x.Customer.CustomerId == female2.GlobalId).Value);

            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Value);

            females = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Juniorzy).ToList();
            Assert.AreEqual(2, females.Single(x => x.Customer.CustomerId == female4.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female4.GlobalId).Value);

            Assert.AreEqual(1, females.Single(x => x.Customer.CustomerId == female5.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female5.GlobalId).Value);

            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female6.GlobalId).Position);
            Assert.AreEqual(67.5m, females.Single(x => x.Customer.CustomerId == female6.GlobalId).Value);

            Assert.IsNull(females.SingleOrDefault(x => x.Customer.CustomerId == female7.GlobalId));
        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeOpen_NotStrict_WithoutUpperLimitCategory()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var female1 = CreateCustomer("Test1", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-53));
            var female2 = CreateCustomer("Test2", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-47));
            var female3 = CreateCustomer("Test3", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-58));
            var female4 = CreateCustomer("Test4", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-57));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Weterani1,Type = ChampionshipCategoryType.Open,Gender = Gender.Female,IsAgeStrict = false}
            }
            );
            CreateReservation(champ, female1);
            CreateReservation(champ, female2);
            CreateReservation(champ, female3);
            CreateReservation(champ, female4);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, female1, 82.6m, 0, 120, 0);
            addCustomerEntries(champDTO, female2, 59.20m, 0, 0, 95);
            addCustomerEntries(champDTO, female3, 49m, 0, 60, 0);
            addCustomerEntries(champDTO, female4, 58.8m, 0, 55, 0);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Weterani1).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);

            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == female2.GlobalId).Position);

            Assert.AreEqual(2, males.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);

            Assert.AreEqual(3, males.Single(x => x.Customer.CustomerId == female4.GlobalId).Position);
        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeOpen_NotStrict_WithUpperLimitCategory()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var female1 = CreateCustomer("Test1", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-63));
            var female2 = CreateCustomer("Test2", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-47));
            var female3 = CreateCustomer("Test3", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-58));
            var female4 = CreateCustomer("Test4", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-57));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Weterani1,Type = ChampionshipCategoryType.Open,Gender = Gender.Female,IsAgeStrict = false},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Weterani3,Type = ChampionshipCategoryType.Open,Gender = Gender.Female,IsAgeStrict = false}
            }
            );
            CreateReservation(champ, female1);
            CreateReservation(champ, female2);
            CreateReservation(champ, female3);
            CreateReservation(champ, female4);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, female1, 82.6m, 0, 120, 0);
            addCustomerEntries(champDTO, female2, 59.20m, 0, 0, 95);
            addCustomerEntries(champDTO, female3, 49m, 0, 60, 0);
            addCustomerEntries(champDTO, female4, 58.8m, 0, 55, 0);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Weterani1).ToList();
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == female2.GlobalId).Position);

            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);

            Assert.AreEqual(2, males.Single(x => x.Customer.CustomerId == female4.GlobalId).Position);

            males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Weterani3).ToList();

            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);
        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_MistrzMistrzow()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-16));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-17));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-19));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-21));
            var female6 = CreateCustomer("Test16", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-22));
            var female7 = CreateCustomer("Test17", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.JuniorzyMlodsi,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.JuniorzyMlodsi,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Juniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Juniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.MistrzMistrzow,Type=ChampionshipCategoryType.Weight,Gender=Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.MistrzMistrzow,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);
            CreateReservation(champ, male6);
            CreateReservation(champ, female6);
            CreateReservation(champ, male7);
            CreateReservation(champ, female7);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240);

            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365);

            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65);
            addCustomerEntries(champDTO, female2, 44, 45, 27.5m, 65);

            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90);

            addCustomerEntries(champDTO, female4, 64, 60, 35, 90);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female6, 62.7m, 60, 35, 90);
            addCustomerEntries(champDTO, female7, 61.7m, 60, 35, 90);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.MistrzMistrzow).ToList();
            Assert.AreEqual(3,males.Count);
            Assert.AreEqual(0, males.Single(x => x.Customer.CustomerId == male1.GlobalId).Position);
            Assert.AreEqual(2, males.Single(x => x.Customer.CustomerId == male3.GlobalId).Position);
            Assert.AreEqual(1, males.Single(x => x.Customer.CustomerId == male4.GlobalId).Position);

            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == male2.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == male5.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == male6.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == male7.GlobalId));


            var females = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.MistrzMistrzow).ToList();
            Assert.AreEqual(3,females.Count);
            Assert.AreEqual(1, females.Single(x => x.Customer.CustomerId == female1.GlobalId).Position);
            Assert.AreEqual(2, females.Single(x => x.Customer.CustomerId == female3.GlobalId).Position);
            Assert.AreEqual(0, females.Single(x => x.Customer.CustomerId == female6.GlobalId).Position);

            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == female2.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == female4.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == female5.GlobalId));
            Assert.IsNull(males.SingleOrDefault(x => x.Customer.CustomerId == female6.GlobalId));


        }


        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_GenderNotSet()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-16));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-17));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-19));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-21));
            var female6 = CreateCustomer("Test16", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-22));
            var female7 = CreateCustomer("Test17", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.NotSet},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);
            CreateReservation(champ, male6);
            CreateReservation(champ, female6);
            CreateReservation(champ, male7);
            CreateReservation(champ, female7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female2, 44, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female4, 64, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female6, 62.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female7, 61.7m, 60, 35, 90, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(57, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);


        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_Official_CustomerNotClassified()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Open,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male,IsOfficial = true},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            var cust1=addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            cust1.Type = ChampionshipCustomerType.NotClassified;

            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(21, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_NotOfficial_CustomerNotClassified()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Open,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male,IsOfficial = false},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            var cust1 = addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            cust1.Type = ChampionshipCustomerType.NotClassified;

            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(33, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_NotOfficial_CustomerDisqualified()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Open,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male,IsOfficial = false},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            var cust1 = addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            cust1.Type = ChampionshipCustomerType.Disqualified;

            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(21, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(19, males.Single(x => x.Group.Name == group2.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_DistinctForCustomer()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Open,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(33, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_TeamCount5()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-16));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-17));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-19));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-21));
            var female6 = CreateCustomer("Test16", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-22));
            var female7 = CreateCustomer("Test17", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);
            CreateReservation(champ, male6);
            CreateReservation(champ, female6);
            CreateReservation(champ, male7);
            CreateReservation(champ, female7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female2, 44, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female4, 64, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female6, 62.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female7, 61.7m, 60, 35, 90, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(33, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);


            var female = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(1, female.Count);
            Assert.AreEqual(0, female.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(50, female.Single(x => x.Group.Name == group1.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_TeamCount6()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var female1 = CreateCustomer("Test11", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-15));
            var female2 = CreateCustomer("Test12", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-16));
            var female3 = CreateCustomer("Test13", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-17));
            var female4 = CreateCustomer("Test14", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-19));
            var female5 = CreateCustomer("Test15", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-21));
            var female6 = CreateCustomer("Test16", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-22));
            var female7 = CreateCustomer("Test17", profiles[0], gender: Gender.Female, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Weight,Gender=Gender.Female},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Female},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, female1);
            CreateReservation(champ, male2);
            CreateReservation(champ, female2);
            CreateReservation(champ, male3);
            CreateReservation(champ, female3);
            CreateReservation(champ, male4);
            CreateReservation(champ, female4);
            CreateReservation(champ, male5);
            CreateReservation(champ, female5);
            CreateReservation(champ, male6);
            CreateReservation(champ, female6);
            CreateReservation(champ, male7);
            CreateReservation(champ, female7);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";

            var group3 = new ChampionshipGroupDTO();
            group3.Name = "group3";

            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.TeamCount = 6;
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);
            champDTO.Groups.Add(group3);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group3);
            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group1);
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group2);
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group3);
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group1);

            addCustomerEntries(champDTO, female1, 43.2m, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female2, 44, 45, 27.5m, 65, group1);
            addCustomerEntries(champDTO, female3, 66.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female4, 64, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female5, 63.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female6, 62.7m, 60, 35, 90, group1);
            addCustomerEntries(champDTO, female7, 61.7m, 60, 35, 90, group1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(3, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(33, males.Single(x => x.Group.Name == group1.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group3.Name).Position);
            Assert.AreEqual(20, males.Single(x => x.Group.Name == group3.Name).Value);
            Assert.AreEqual(2, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(16, males.Single(x => x.Group.Name == group2.Name).Value);


            var female = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Female && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(1, female.Count);
            Assert.AreEqual(0, female.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(57, female.Single(x => x.Group.Name == group1.Name).Value);

        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_TheSamePoints_NumberOfFirstPlacesImportant()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male8 = CreateCustomer("Test8", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male9 = CreateCustomer("Test9", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            
            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);
            CreateReservation(champ, male8);
            CreateReservation(champ, male9);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";


            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1); //12
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group1); //9

            addCustomerEntries(champDTO, male3, 99.9m, 375, 267.5m, 365, group2); //12
            addCustomerEntries(champDTO, male8, 99.9m, 375, 267.5m, 365, group2); //12

            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group2); //12
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group2);//9
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group1);//8
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group1);//7
            addCustomerEntries(champDTO, male9, 55.80m, 250, 165, 240, group1);//6
            
            


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(2, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(42, males.Single(x => x.Group.Name == group2.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(42, males.Single(x => x.Group.Name == group1.Name).Value);


        }

        [Test]
        public void Save_Results_MaleAgeWeight_FemaleAgeWeight_Druzynowo_TheSamePoints_NumberOfSecondPlacesImportant()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));
            var male2 = CreateCustomer("Test2", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-16));
            var male3 = CreateCustomer("Test3", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-17));
            var male4 = CreateCustomer("Test4", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-19));
            var male5 = CreateCustomer("Test5", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-21));
            var male6 = CreateCustomer("Test6", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-22));
            var male7 = CreateCustomer("Test7", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male8 = CreateCustomer("Test8", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male9 = CreateCustomer("Test9", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male10 = CreateCustomer("Test10", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male11 = CreateCustomer("Test11", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));
            var male12 = CreateCustomer("Test12", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-25));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory(){Category = ChampionshipWinningCategories.Seniorzy,Type = ChampionshipCategoryType.Weight,Gender = Gender.Male},
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Druzynowa,Gender=Gender.Male},
            }
            );
            CreateReservation(champ, male1);
            CreateReservation(champ, male2);
            CreateReservation(champ, male3);
            CreateReservation(champ, male4);
            CreateReservation(champ, male5);
            CreateReservation(champ, male6);
            CreateReservation(champ, male7);
            CreateReservation(champ, male8);
            CreateReservation(champ, male9);
            CreateReservation(champ, male10);
            CreateReservation(champ, male11);
            CreateReservation(champ, male12);

            var group1 = new ChampionshipGroupDTO();
            group1.Name = "group1";

            var group2 = new ChampionshipGroupDTO();
            group2.Name = "group2";


            var champDTO = champ.Map<ChampionshipDTO>();
            champDTO.TeamCount = 10;
            champDTO.Groups.Add(group1);
            champDTO.Groups.Add(group2);

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240, group1); //12
            addCustomerEntries(champDTO, male2, 52m, 250, 165, 240, group1); //9

            addCustomerEntries(champDTO, male3, 99.4m, 375, 267.5m, 365); 
            addCustomerEntries(champDTO, male8, 99.9m, 375, 267.5m, 365,group2); //9

            addCustomerEntries(champDTO, male4, 52.60m, 250, 165, 240, group2); //12
            addCustomerEntries(champDTO, male7, 54.20m, 250, 165, 240, group2);//9
            addCustomerEntries(champDTO, male6, 54.60m, 250, 165, 240, group1);//8
            addCustomerEntries(champDTO, male5, 55.60m, 250, 165, 240, group1);//7
            addCustomerEntries(champDTO, male9, 55.80m, 250, 165, 240, group1);//6
            addCustomerEntries(champDTO, male10, 55.90m, 250, 165, 240, group2);//5
            addCustomerEntries(champDTO, male11, 55.96m, 250, 165, 240, group2);//4
            addCustomerEntries(champDTO, male12, 55.98m, 250, 165, 240, group2);//3




            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var males = result.Championship.Results.Where(x => x.Category.Gender == Service.V2.Model.Gender.Male && x.Category.Category == Service.V2.Model.ChampionshipWinningCategories.Druzynowa).ToList();
            Assert.AreEqual(2, males.Count);
            Assert.AreEqual(0, males.Single(x => x.Group.Name == group2.Name).Position);
            Assert.AreEqual(42, males.Single(x => x.Group.Name == group2.Name).Value);
            Assert.AreEqual(1, males.Single(x => x.Group.Name == group1.Name).Position);
            Assert.AreEqual(42, males.Single(x => x.Group.Name == group1.Name).Value);


        }
        #endregion

        #region Strength training

        [Test]
        public void Save_CreateStrengthTraining_EmptyDay()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test",startDate:DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            champDTO.Entries[0].Try2.Result = ChampionshipTryResult.Fail;
            champDTO.Entries[0].Try2.Weight = 260;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var dbCustomer = Session.Get<ChampionshipCustomer>(result.Championship.Customers[0].GlobalId);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date,dbCustomer.StrengthTraining.TrainingDay.TrainingDate);
            Assert.AreEqual(3, dbCustomer.StrengthTraining.Entries.Count);
            Assert.AreEqual(champ.MyPlace, dbCustomer.StrengthTraining.MyPlace);
            Assert.AreEqual(EntryObjectStatus.System, dbCustomer.StrengthTraining.Status);
            var item=dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Single();
            Assert.AreEqual(2, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(250, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);
            Assert.AreEqual(1, item.Series.ElementAt(1).RepetitionNumber);
            Assert.AreEqual(260, item.Series.ElementAt(1).Weight);
            Assert.AreEqual(SetType.MuscleFailure, item.Series.ElementAt(1).SetType);

            item = dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == deadlift.GlobalId).Single();
            Assert.AreEqual(1, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(165, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);

            item = dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == sqad.GlobalId).Single();
            Assert.AreEqual(1, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(240, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);
        }

        [Test]
        public void Save_CreateStrengthTraining_ExistingDay()
        {
            
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            TrainingDay day = new TrainingDay(DateTime.Now.AddDays(-2).Date);
            day.Profile = profiles[0];
            day.Customer = male1;
            SizeEntry size = new SizeEntry();
            size.Wymiary= new Wymiary();
            size.Wymiary.Klatka = 100;
            day.AddEntry(size);
            insertToDatabase(day);

            var champ = CreateChampionshipEx(profiles[0], "test", startDate: DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            champDTO.Entries[0].Try2.Result = ChampionshipTryResult.Fail;
            champDTO.Entries[0].Try2.Weight = 260;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var dbDay=Session.QueryOver<TrainingDay>().SingleOrDefault();
            Assert.IsNotNull(dbDay);
            
            Assert.AreEqual(2,dbDay.Objects.Count);

        }

        [Test]
        public void Save_UpdateStrengthTraining()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", startDate: DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            result.Championship.Entries[0].Try2.Result = ChampionshipTryResult.Success;
            result.Championship.Entries[0].Try2.Weight = 260;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });
            var dbCustomer = Session.Get<ChampionshipCustomer>(result.Championship.Customers[0].GlobalId);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, dbCustomer.StrengthTraining.TrainingDay.TrainingDate);
            Assert.AreEqual(3, dbCustomer.StrengthTraining.Entries.Count);
            Assert.AreEqual(champ.MyPlace, dbCustomer.StrengthTraining.MyPlace);
            Assert.AreEqual(EntryObjectStatus.System, dbCustomer.StrengthTraining.Status);
            var item = dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Single();
            Assert.AreEqual(2, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(250, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);
            Assert.AreEqual(1, item.Series.ElementAt(1).RepetitionNumber);
            Assert.AreEqual(260, item.Series.ElementAt(1).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(1).SetType);

            item = dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == deadlift.GlobalId).Single();
            Assert.AreEqual(1, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(165, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);

            item = dbCustomer.StrengthTraining.Entries.Where(x => x.Exercise.GlobalId == sqad.GlobalId).Single();
            Assert.AreEqual(1, item.Series.Count);
            Assert.AreEqual(1, item.Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(240, item.Series.ElementAt(0).Weight);
            Assert.AreEqual(SetType.Max, item.Series.ElementAt(0).SetType);

            Assert.AreEqual(1, Session.QueryOver<TrainingDay>().RowCount());
            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingEntry>().RowCount());
            Assert.AreEqual(3, Session.QueryOver<StrengthTrainingItem>().RowCount());
            Assert.AreEqual(4, Session.QueryOver<Serie>().RowCount());
        }

        [Test]
        public void Save_CreateNewRecords()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", startDate: DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            champDTO.Entries[0].Try2.Result = ChampionshipTryResult.Fail;
            champDTO.Entries[0].Try2.Weight = 260;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var records = Session.QueryOver<ExerciseProfileData>().List();
            var exData = records.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Single();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(male1, exData.Customer);
            Assert.AreEqual(250, exData.MaxWeight);
            Assert.AreEqual(1, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, exData.TrainingDate);
            Assert.IsNotNull(exData.Serie);

            exData = records.Where(x => x.Exercise.GlobalId == deadlift.GlobalId).Single();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(male1, exData.Customer);
            Assert.AreEqual(165, exData.MaxWeight);
            Assert.AreEqual(1, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, exData.TrainingDate);
            Assert.IsNotNull(exData.Serie);

            exData = records.Where(x => x.Exercise.GlobalId == sqad.GlobalId).Single();
            Assert.AreEqual(profiles[0], exData.Profile);
            Assert.AreEqual(male1, exData.Customer);
            Assert.AreEqual(240, exData.MaxWeight);
            Assert.AreEqual(1, exData.Repetitions);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, exData.TrainingDate);
            Assert.IsNotNull(exData.Serie);
        }

        [Test]
        public void Save_RecordsInResults()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", startDate: DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            champDTO.Entries[0].Try2.Result = ChampionshipTryResult.Fail;
            champDTO.Entries[0].Try2.Weight = 260;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });
            Assert.AreEqual(3,result.NewRecords.Count);

            var set=result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == benchPress.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(250, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);

            set = result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == deadlift.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(165, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);


            set = result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == sqad.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(240, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);
        }

        [Test]
        public void Save_Update_RecordsInResults()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", startDate: DateTime.Now.AddDays(-2), categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            champDTO.Entries[0].Try2.Result = ChampionshipTryResult.Fail;
            champDTO.Entries[0].Try2.Weight = 260;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            result.Championship.Entries[0].Try1.Weight = 350;

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });
            Assert.AreEqual(3, result.NewRecords.Count);

            var set = result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == benchPress.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(350, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);

            set = result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == deadlift.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(165, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);


            set = result.NewRecords.Where(x => x.StrengthTrainingItem.Exercise.GlobalId == sqad.GlobalId).Single();
            Assert.AreEqual(profiles[0].GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.ProfileId);
            Assert.AreEqual(male1.GlobalId, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.CustomerId);
            Assert.AreEqual(240, set.Weight);
            Assert.AreEqual(1, set.RepetitionNumber);
            Assert.AreEqual(DateTime.Now.AddDays(-2).Date, set.StrengthTrainingItem.StrengthTrainingEntry.TrainingDay.TrainingDate);
        }

        #endregion

        #region Deleting unused data

        [Test]
        public void Save_DeleteOrphansEntries()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            var temp = result.Championship.Entries.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Single();
            result.Championship.Entries.Remove(temp);
            ChampionshipEntryDTO entryDTO = new ChampionshipEntryDTO();
            entryDTO.Customer = result.Championship.Customers[0];
            entryDTO.Exercise = benchPress.Map<ExerciseLightDTO>();
            entryDTO.Try1.Weight = 99;
            entryDTO.Try1.Result = ChampionshipTryResult.Success;
            result.Championship.Entries.Add(entryDTO);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });

            var count=Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(3,count);
        }

        [Test]
        [ExpectedException(typeof(ObjectDeletedException))]
        public void Save_DeleteOrphansCustomer()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            result.Championship.Customers.Clear();
            ChampionshipCustomerDTO customerDTO = new ChampionshipCustomerDTO();
            customerDTO.CustomerId = male1.GlobalId;
            result.Championship.Customers.Add(customerDTO);

            var temp = result.Championship.Entries.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Single();
            result.Championship.Entries.Remove(temp);
            ChampionshipEntryDTO entryDTO = new ChampionshipEntryDTO();
            entryDTO.Customer = result.Championship.Customers[0];
            entryDTO.Exercise = benchPress.Map<ExerciseLightDTO>();
            entryDTO.Try1.Weight = 99;
            entryDTO.Try1.Result = ChampionshipTryResult.Success;
            result.Championship.Entries.Add(entryDTO);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });

            var count = Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(3, count);

            count = Session.QueryOver<ChampionshipCustomer>().RowCount();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Save_DeleteOrphansCustomerAndEntries()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var male1 = CreateCustomer("Test1", profiles[0], gender: Gender.Male, birthday: DateTime.Now.AddYears(-15));

            var champ = CreateChampionshipEx(profiles[0], "test", categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
            }
            );
            CreateReservation(champ, male1);

            var champDTO = champ.Map<ChampionshipDTO>();

            addCustomerEntries(champDTO, male1, 51.60m, 250, 165, 240);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            SaveChampionshipResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, champDTO);
            });

            result.Championship.Customers.Clear();
            result.Championship.Entries.Clear();

            addCustomerEntries(result.Championship, male1, 51.60m, 250, 165, 240);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.SaveChampionship(data.Token, result.Championship);
            });

            var count = Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(3, count);

            count = Session.QueryOver<ChampionshipCustomer>().RowCount();
            Assert.AreEqual(1, count);
        }

        #endregion

        #endregion

        #region Operations

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeReservation_VirtualCustomer()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var champ = CreateChampionshipEx(profiles[0], "test", ChampionshipType.Trojboj, state: ScheduleEntryState.Planned, categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Open,Gender=Gender.Female},
            });
            var customer1 = CreateCustomer("cust", profiles[0]);
            customer1.IsVirtual = true;
            insertToDatabase(customer1);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customer1.GlobalId;
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ConsistencyException))]
        public void StatusDone_WithoutWinningCategories()
        {
            var champ = CreateChampionship(profiles[0], "test", ChampionshipType.ZawodyWyciskanieSztangi);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.StatusDone;
                Service.ReservationsOperation(data.Token, param);
            });
        }

        [Test]
        public void StatusDone_BenchPressChampionship()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var champ = CreateChampionshipEx(profiles[0], "test", ChampionshipType.ZawodyWyciskanieSztangi,state:ScheduleEntryState.Planned,categories:new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Open,Gender=Gender.Female},
            });
            var customer1 = CreateCustomer("cust", profiles[0]);
            var customer2 = CreateCustomer("cust1", profiles[0]);
            CreateReservation(champ, customer1);
            CreateReservation(champ, customer2);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.StatusDone;
                Service.ReservationsOperation(data.Token, param);
            });

            var dbChampionship=Session.Get<Championship>(champ.GlobalId);
            Assert.AreEqual(2, dbChampionship.Customers.Count);

            Assert.AreEqual(2, dbChampionship.Entries.Count);

            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer2.GlobalId).Count());
            foreach (var entryDto in dbChampionship.Entries)
            {
                Assert.AreEqual(benchPress.GlobalId, entryDto.Exercise.GlobalId);
            }
        }

        [Test]
        public void StatusDone_PowerliftingChampionship()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var deadlift = CreateExercise(Session, null, "Deadlift", "H", globalId: new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
            var sqad = CreateExercise(Session, null, "Sqad", "H", globalId: new Guid("3e06a130-b811-4e45-9285-f087403615bf"));


            var champ = CreateChampionshipEx(profiles[0], "test", ChampionshipType.Trojboj, state: ScheduleEntryState.Planned, categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    },
                new ChampionshipCategory(){Category=ChampionshipWinningCategories.Seniorzy,Type=ChampionshipCategoryType.Open,Gender=Gender.Female},
            });
            var customer1 = CreateCustomer("cust", profiles[0]);
            var customer2 = CreateCustomer("cust1", profiles[0]);
            CreateReservation(champ, customer1);
            CreateReservation(champ, customer2);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.StatusDone;
                service.ReservationsOperation(data.Token, param);
            });

            var dbChampionship = Session.Get<Championship>(champ.GlobalId);
            Assert.AreEqual(2, dbChampionship.Customers.Count);

            Assert.AreEqual(6, dbChampionship.Entries.Count);

            Assert.AreEqual(3, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId).Count());
            Assert.AreEqual(3, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer2.GlobalId).Count());

            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId && x.Exercise.GlobalId==benchPress.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId && x.Exercise.GlobalId == deadlift.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId && x.Exercise.GlobalId == sqad.GlobalId).Count());

            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer2.GlobalId && x.Exercise.GlobalId == benchPress.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer2.GlobalId && x.Exercise.GlobalId == deadlift.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer2.GlobalId && x.Exercise.GlobalId == sqad.GlobalId).Count());
        }

        [Test]
        [Ignore]
        public void MakeReservation_BenchPressChampionship()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var champ = CreateChampionshipEx(profiles[0], "test", ChampionshipType.ZawodyWyciskanieSztangi, state: ScheduleEntryState.Planned, categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            });
            var customer1 = CreateCustomer("cust", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customer1.GlobalId;
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                service.ReservationsOperation(data.Token, param);
            });

            var dbChampionship = Session.Get<Championship>(champ.GlobalId);
            Assert.AreEqual(1, dbChampionship.Customers.Count);

            Assert.AreEqual(1, dbChampionship.Entries.Count);

            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Customer.Customer.GlobalId == customer1.GlobalId).Count());
            Assert.AreEqual(1, dbChampionship.Entries.Where(x => x.Exercise.GlobalId == benchPress.GlobalId).Count());
        }

        [Test]
        [Ignore]
        public void UndoReservation()
        {
            var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
            var champ = CreateChampionshipEx(profiles[0], "test", ChampionshipType.ZawodyWyciskanieSztangi, state: ScheduleEntryState.Planned, categories: new ChampionshipCategory[]
            {
                new ChampionshipCategory()
                    {
                        Category = ChampionshipWinningCategories.Seniorzy,
                        Type = ChampionshipCategoryType.Open,
                        Gender = Gender.Male
                    }
            });
            var customer1 = CreateCustomer("cust", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            ReservationOperationResult result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.CustomerId = customer1.GlobalId;
                param.EntryId = champ.GlobalId;
                param.OperationType = ReservationsOperationType.Make;
                result=service.ReservationsOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                ReservationsOperationParam param = new ReservationsOperationParam();
                param.ReservationId = result.Reservation.GlobalId;
                param.OperationType = ReservationsOperationType.Undo;
                Service.ReservationsOperation(data.Token, param);
            });


            var count = Session.QueryOver<ChampionshipCustomer>().RowCount();
            Assert.AreEqual(0, count);
            count = Session.QueryOver<ChampionshipEntry>().RowCount();
            Assert.AreEqual(0, count);
        }

        //[Test]
        //public void Operation_Start_BenchPressOnly()
        //{
        //    var benchPress = CreateExercise(Session, null, "BenchPress", "H", globalId: new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
        //    var champ = CreateChampionship(profiles[0], "test",ChampionshipType.ZawodyWyciskanieSztangi);
        //    var customer1 = CreateCustomer("cust",profiles[0]);
        //    var customer2 = CreateCustomer("cust1", profiles[0]);
        //    CreateReservation(champ, customer1);
        //    CreateReservation(champ, customer2);

        //    var profile = (ProfileDTO)profiles[0].Tag;
        //    SessionData data = CreateNewSession(profile, ClientInformation);
        //    RunServiceMethod(delegate(InternalBodyArchitectService service)
        //    {
        //        ChampionshipOperationParams param = new ChampionshipOperationParams();
        //        param.Operation = ChampionshipOperationType.Start;
        //        param.ChampionshipId = champ.GlobalId;
        //        var result=service.ChampionshipOperation(data.Token, param);
        //        Assert.AreEqual(2,result.Customers.Count);

        //        Assert.AreEqual(2, result.Entries.Count);

        //        Assert.AreEqual(1,result.Entries.Where(x=>x.Customer.CustomerId==customer1.GlobalId).Count());
        //        Assert.AreEqual(1, result.Entries.Where(x => x.Customer.CustomerId == customer2.GlobalId).Count());
        //        foreach (var entryDto in result.Entries)
        //        {
        //            Assert.AreEqual(benchPress.GlobalId,entryDto.Exercise.GlobalId);
        //        }
        //    });
        //}
        #endregion

    }
}
