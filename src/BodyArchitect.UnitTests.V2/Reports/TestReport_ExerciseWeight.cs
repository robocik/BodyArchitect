using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using BodyArchitect.Shared;
using NUnit.Framework;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using ExerciseDoneWay = BodyArchitect.Model.ExerciseDoneWay;
using Privacy = BodyArchitect.Model.Privacy;
using ReportStatus = BodyArchitect.Model.ReportStatus;
using SetType = BodyArchitect.Service.V2.Model.SetType;

namespace BodyArchitect.UnitTests.V2.Reports
{
    [TestFixture]
    public class TestReport_ExerciseWeight:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Exercise>  exercises = new List<Exercise>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                exercises.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var ex = CreateExercise(Session, null, "ex1", "ex1");
                exercises.Add(ex);
                ex = CreateExercise(Session, null, "ex2", "ex2");
                exercises.Add(ex);
                tx.Commit();
            }
        }

        StrengthTrainingEntry addTrainingDaySet(Profile profile, Customer customer, DateTime date, Exercise exercise, params Tuple<int?, decimal?>[] sets)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Customer = customer;
            trainingDay.Profile = profile;
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.MyPlace = GetDefaultMyPlace(profile);
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.Exercise = exercise;
            entry.AddEntry(item);

            foreach (var tuple in sets)
            {
                Serie set1 = new Serie();
                set1.RepetitionNumber = tuple.Item1;
                set1.Weight = tuple.Item2;
                item.AddSerie(set1);
            }
            insertToDatabase(trainingDay);
            return entry;
            
            
        }

        [Test]
        public void EntriesFromManyCustomersInDb_ReturnDataForSpecificCustomer()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            addTrainingDaySet(profiles[0],customer, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0],null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[1], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.CustomerId = customer.GlobalId;
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 40);
            });
        }

        [Test]
        public void EntriesFromManyProfileInDb_ReturnDataForSpecificUser()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[1], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list,40,56);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void ForCustomer_AnotherProfile()
        {
            var customer = CreateCustomer("cust", profiles[0]);
            addTrainingDaySet(profiles[0], customer, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[1], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.CustomerId = customer.GlobalId;
                param.Exercises.Add(exercises[0].GlobalId);
                service.ReportExerciseWeight(data.Token, param);
            });
        }

        [Test]
        public void SkipPlannedEntries()
        {
            var entry=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            entry.Status = EntryObjectStatus.Planned;
            insertToDatabase(entry);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(56, list[0].Weight);
                Assert.AreEqual(DateTime.UtcNow.AddDays(-5).Date, list[0].DateTime);
            });
        }

        [Test]
        public void AllEntries_OneExercise()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    var param = new ReportExerciseWeightParams();
                    param.Exercises.Add(exercises[0].GlobalId);
                    var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(40.0, list[0].Weight);
                Assert.AreEqual(DateTime.UtcNow.AddDays(-10).Date, list[0].DateTime);
                Assert.AreEqual(56, list[1].Weight);
                Assert.AreEqual(DateTime.UtcNow.AddDays(-5).Date, list[1].DateTime);
            });
        }

        [Test]
        public void AllEntries_OneExercise_GetMaxWeight()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(60, list[0].Weight);
                Assert.AreEqual(DateTime.UtcNow.AddDays(-10).Date, list[0].DateTime);
                Assert.AreEqual(56, list[1].Weight);
                Assert.AreEqual(DateTime.UtcNow.AddDays(-5).Date, list[1].DateTime);
            });
        }

        [Test]
        public void AllEntries_TwoExercises_GetMaxWeight()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.Exercises.Add(exercises[1].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list,60,56,67);
            });
        }

        [Test]
        public void AnotherProfile_Private()
        {
            setPrivacy(Privacy.Private);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void AnotherProfile_Friends_ForUnknownUser()
        {
            setPrivacy(Privacy.FriendsOnly);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.UserId = profiles[0].GlobalId;
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(0, list.Count);
            });
        }

        [Test]
        public void AnotherProfile_Friends_ForMyFriend()
        {
            setPrivacy(Privacy.FriendsOnly);
            profiles[0].Friends.Add(profiles[1]);
            profiles[1].Friends.Add(profiles[0]);
            insertToDatabase(profiles[0]);
            insertToDatabase(profiles[1]);

            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, null));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.UserId = profiles[0].GlobalId;
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                Assert.AreEqual(2, list.Count);
            });
        }

        [Test]
        public void UseForReportOnly()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            var entry=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));
            entry.ReportStatus = ReportStatus.SkipInReport;
            insertToDatabase(entry);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.Exercises.Add(exercises[1].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60, 56);
            });
        }

        [Test]
        public void UseAllEntries()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            var entry = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));
            entry.ReportStatus = ReportStatus.SkipInReport;
            insertToDatabase(entry);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.Exercises.Add(exercises[1].GlobalId);
                param.UseAllEntries = true;
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60, 56,67);
            });
        }

        [Test]
        public void FromDate()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.Exercises.Add(exercises[1].GlobalId);
                param.StartDate = DateTime.UtcNow.AddDays(-6);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 56, 67);
            });
        }

        [Test]
        public void ToDate()
        {
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[1], new Tuple<int?, decimal?>(5, 67));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.Exercises.Add(exercises[1].GlobalId);
                param.EndDate = DateTime.UtcNow.AddDays(-6);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60);
            });
        }


        [Test]
        public void SetsWithSuperSlowTrueOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsSuperSlow = true;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.SuperSlow = true;
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 40);
            });
        }

        [Test]
        public void SetsWithSuperSlowFalseOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsSuperSlow = false;
            insertToDatabase(item);
            item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsSuperSlow = true;
            insertToDatabase(item);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.SuperSlow = false;
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60);
            });
        }

        [Test]
        public void SetsWithSuperSlowAny()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsSuperSlow = true;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60, 56);
            });
        }


        [Test]
        public void SetsWithRestPauseTrueOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = true;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.RestPause = true;
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 40);
            });
        }

        [Test]
        public void SetsWithRestPauseFalseOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = false;
            insertToDatabase(item);
            item=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = true;
            insertToDatabase(item);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.RestPause = false;
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60);
            });
        }

        [Test]
        public void SetsWithRestPauseAny()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).IsRestPause = true;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60,56);
            });
        }

        [Test]
        public void SpecificSetType()
        {
            var item=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).Series.ElementAt(0).SetType = Model.SetType.Max;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.SetTypes.Add(SetType.Max);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 40);
            });
        }

        [Test]
        public void EntriesWithDifferentMyPlace_ReturnAll()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item=addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60,56);
            });
        }

        [Test]
        public void EntriesWithDifferentMyPlace_ReturnOnlyForOneMyPlace()
        {
            var myPlace = CreateMyPlace("place1", profiles[0]);
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.MyPlace = myPlace;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(5, 56));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.MyPlaces.Add(myPlace.GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60);
            });
        }

        [Test]
        public void EntriesWithDifferentDoneWays_ReturnAll()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(11, 70));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60, 70);
            });
        }

        [Test]
        public void EntriesWithDifferentDoneWays_ReturnSpecificOnly()
        {
            var item = addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-10).Date, exercises[0], new Tuple<int?, decimal?>(11, 40), new Tuple<int?, decimal?>(5, 60));
            item.Entries.ElementAt(0).DoneWay = ExerciseDoneWay.Barbell;
            insertToDatabase(item);
            addTrainingDaySet(profiles[0], null, DateTime.UtcNow.AddDays(-5).Date, exercises[0], new Tuple<int?, decimal?>(11, 70));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ReportExerciseWeightParams();
                param.Exercises.Add(exercises[0].GlobalId);
                param.DoneWays.Add(Service.V2.Model.ExerciseDoneWay.Barbell);
                var list = service.ReportExerciseWeight(data.Token, param);

                assert(list, 60);
            });
        }

        void assert(IList<WeightExerciseReportResultItem> results, params decimal[] weights)
        {
            Assert.AreEqual(weights.Length,results.Count);
            foreach (var weight in weights)
            {
                Assert.IsTrue(results.Where(x => x.Weight == weight).Count()>0,"Weight {0} is not returned",weight);
            }
        }

        private void setPrivacy(Privacy newPrivacy)
        {
            profiles[0].Privacy.CalendarView = newPrivacy;
            Session.Update(profiles[0]);
            Session.Flush();
            Session.Clear();
        }
    }
}
