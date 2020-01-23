using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Reports;
using NUnit.Framework;
using Privacy = BodyArchitect.Model.Privacy;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.V2
{
    public class TestService_GetFeaturedData : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles[0].Privacy.CalendarView = Privacy.Public;
                profiles[0].Statistics.TrainingDaysCount = 60;
                profiles.Add(CreateProfile(Session, "test2"));
                profiles[1].Statistics.TrainingDaysCount = 60;
                profiles[1].Privacy.CalendarView = Privacy.Public;
                profiles.Add(CreateProfile(Session, "test3"));
                profiles[2].Statistics.TrainingDaysCount = 60;
                profiles[2].Privacy.CalendarView = Privacy.Public;
                profiles.Add(CreateProfile(Session, "test4"));
                profiles[3].Statistics.TrainingDaysCount = 60;
                profiles[3].Privacy.CalendarView = Privacy.Public;
                profiles.Add(CreateProfile(Session, "test5"));
                profiles[4].Statistics.TrainingDaysCount = 60;
                profiles[4].Privacy.CalendarView = Privacy.Public;

                tx.Commit();
            }
        }

        #region Training plans

        [Test]
        public void Latest_TakeCorrectPlans()
        {
            var workoutPlan1 = CreatePlan(Session, profiles[0], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan1.PublishDate = DateTime.UtcNow.Date;
            workoutPlan1.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan1);
            var workoutPlan2 = CreatePlan(Session, profiles[1], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
            workoutPlan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            workoutPlan2.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan2);
            var workoutPlan3 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            workoutPlan3.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan3);
            var workoutPlan4 = CreatePlan(Session, profiles[2], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 3);
            var workoutPlan5 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan5.PublishDate = DateTime.UtcNow.Date.AddDays(-13);
            workoutPlan5.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan5);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token,new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestTrainingPlans.Count);
                Assert.AreEqual(workoutPlan2.GlobalId,result.LatestTrainingPlans[0].GlobalId);
                Assert.AreEqual(workoutPlan3.GlobalId, result.LatestTrainingPlans[1].GlobalId);

                Assert.AreEqual(1, result.RandomTrainingPlans.Count);
                Assert.AreEqual(workoutPlan5.GlobalId, result.RandomTrainingPlans[0].GlobalId);
            });
        }

        [Test]
        public void Latest_DoNotRetrieveWorkoutPlans()
        {
            var workoutPlan1 = CreatePlan(Session, profiles[0], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan1.PublishDate = DateTime.UtcNow.Date;
            workoutPlan1.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan1);
            var workoutPlan2 = CreatePlan(Session, profiles[1], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
            workoutPlan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            workoutPlan2.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan2);
            var workoutPlan3 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            workoutPlan3.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan3);
            var workoutPlan4 = CreatePlan(Session, profiles[2], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 3);
            var workoutPlan5 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan5.PublishDate = DateTime.UtcNow.Date.AddDays(-13);
            workoutPlan5.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan5);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam(){LatestTrainingPlansCount = 0,RandomWorkoutPlansCount = 0});

                Assert.AreEqual(0, result.LatestTrainingPlans.Count);
                Assert.AreEqual(0, result.RandomTrainingPlans.Count);
            });
        }

        [Test]
        public void Latest_OnlyTwoResults()
        {
            var workoutPlan1 = CreatePlan(Session, profiles[0], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan1.PublishDate = DateTime.UtcNow.Date;
            workoutPlan1.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan1);
            var workoutPlan2 = CreatePlan(Session, profiles[1], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
            workoutPlan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            workoutPlan2.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan2);
            var workoutPlan3 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            workoutPlan3.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan3);
            var workoutPlan4 = CreatePlan(Session, profiles[2], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 3);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestTrainingPlans.Count);
                Assert.AreEqual(workoutPlan2.GlobalId, result.LatestTrainingPlans[0].GlobalId);
                Assert.AreEqual(workoutPlan3.GlobalId, result.LatestTrainingPlans[1].GlobalId);

                Assert.AreEqual(0, result.RandomTrainingPlans.Count);
                
            });
        }

        [Test]
        public void Latest_ZeroResults()
        {
            var workoutPlan1 = CreatePlan(Session, profiles[0], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, true, Language.Languages[0].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            workoutPlan1.PublishDate = DateTime.UtcNow.Date;
            workoutPlan1.Status = PublishStatus.Published;
            insertToDatabase(workoutPlan1);
            var workoutPlan2 = CreatePlan(Session, profiles[1], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[0].Shortcut, WorkoutPlanPurpose.Mass, 3);
            var workoutPlan3 = CreatePlan(Session, profiles[1], "test1-1", TrainingPlanDifficult.Beginner, TrainingType.HST, false, Language.Languages[1].Shortcut, WorkoutPlanPurpose.FatLost, 3);
            var workoutPlan4 = CreatePlan(Session, profiles[2], "test1-2", TrainingPlanDifficult.Advanced, TrainingType.HST, false, Language.Languages[2].Shortcut, WorkoutPlanPurpose.Mass, 3);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(0, result.LatestTrainingPlans.Count);
                Assert.AreEqual(0, result.RandomTrainingPlans.Count);
            });
        }
        #endregion

        #region Supplements definitions

        [Test]
        public void Latest_TakeCorrectSupplementsDefinitions()
        {
            var supplement = CreateSupplement("sup");

            var plan1 = CreateSupplementsCycleDefinition("plan1", supplement,profiles[0],PublishStatus.Published);
            plan1.PublishDate = DateTime.UtcNow.Date;
            plan1.Status = PublishStatus.Published;
            insertToDatabase(plan1);

            var plan2 = CreateSupplementsCycleDefinition("plan2", supplement, profiles[1], PublishStatus.Published);
            plan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            plan2.Status = PublishStatus.Published;
            insertToDatabase(plan2);

            var plan3 = CreateSupplementsCycleDefinition("plan3", supplement, profiles[1], PublishStatus.Published);
            plan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            plan3.Status = PublishStatus.Published;
            insertToDatabase(plan3);

            var plan4 = CreateSupplementsCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Private);

            var plan5 = CreateSupplementsCycleDefinition("plan5", supplement, profiles[1], PublishStatus.Published);
            plan5.PublishDate = DateTime.UtcNow.Date.AddDays(-13);
            plan5.Status = PublishStatus.Published;
            insertToDatabase(plan5);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.SupplementsDefinitions.Count);
                Assert.AreEqual(plan2.GlobalId, result.SupplementsDefinitions[0].GlobalId);
                Assert.AreEqual(plan3.GlobalId, result.SupplementsDefinitions[1].GlobalId);

                Assert.AreEqual(1, result.RandomSupplementsDefinitions.Count);
                Assert.AreEqual(plan5.GlobalId, result.RandomSupplementsDefinitions[0].GlobalId);
            });
        }

        [Test]
        public void Latest_DoNotRetrieveDefinitions()
        {
            var supplement = CreateSupplement("sup");

            var plan1 = CreateSupplementsCycleDefinition("plan1", supplement, profiles[0], PublishStatus.Published);
            plan1.PublishDate = DateTime.UtcNow.Date;
            plan1.Status = PublishStatus.Published;
            insertToDatabase(plan1);

            var plan2 = CreateSupplementsCycleDefinition("plan2", supplement, profiles[1], PublishStatus.Published);
            plan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            plan2.Status = PublishStatus.Published;
            insertToDatabase(plan2);

            var plan3 = CreateSupplementsCycleDefinition("plan3", supplement, profiles[1], PublishStatus.Published);
            plan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            plan3.Status = PublishStatus.Published;
            insertToDatabase(plan3);

            var plan4 = CreateSupplementsCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Private);

            var plan5 = CreateSupplementsCycleDefinition("plan5", supplement, profiles[1], PublishStatus.Published);
            plan5.PublishDate = DateTime.UtcNow.Date.AddDays(-13);
            plan5.Status = PublishStatus.Published;
            insertToDatabase(plan5);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam(){LatestSupplementsDefinitionsCount = 0,RandomSupplementsDefinitionsCount = 0});

                Assert.AreEqual(0, result.SupplementsDefinitions.Count);
                Assert.AreEqual(0, result.RandomSupplementsDefinitions.Count);
            });
        }

        [Test]
        public void Latest_OnlyTwoResultsSupplementsDefinitions()
        {
            var supplement = CreateSupplement("sup");

            var plan1 = CreateSupplementsCycleDefinition("plan1", supplement, profiles[0], PublishStatus.Published);
            plan1.PublishDate = DateTime.UtcNow.Date;
            plan1.Status = PublishStatus.Published;
            insertToDatabase(plan1);

            var plan2 = CreateSupplementsCycleDefinition("plan2", supplement, profiles[1], PublishStatus.Published);
            plan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            plan2.Status = PublishStatus.Published;
            insertToDatabase(plan2);

            var plan3 = CreateSupplementsCycleDefinition("plan3", supplement, profiles[1], PublishStatus.Published);
            plan3.PublishDate = DateTime.UtcNow.Date.AddDays(-11);
            plan3.Status = PublishStatus.Published;
            insertToDatabase(plan3);

            var plan4 = CreateSupplementsCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Private);


            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.SupplementsDefinitions.Count);
                Assert.AreEqual(plan2.GlobalId, result.SupplementsDefinitions[0].GlobalId);
                Assert.AreEqual(plan3.GlobalId, result.SupplementsDefinitions[1].GlobalId);

                Assert.AreEqual(0, result.RandomSupplementsDefinitions.Count);
            });
        }

        [Test]
        public void Latest_ZeroResultsSupplementsDefinitions()
        {
            var supplement = CreateSupplement("sup");

            var plan1 = CreateSupplementsCycleDefinition("plan1", supplement, profiles[0], PublishStatus.Published);
            plan1.PublishDate = DateTime.UtcNow.Date;
            plan1.Status = PublishStatus.Published;
            insertToDatabase(plan1);

            
            var plan4 = CreateSupplementsCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Private);
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(0, result.SupplementsDefinitions.Count);

                Assert.AreEqual(0, result.RandomSupplementsDefinitions.Count);
            });
        }

        [Test]
        public void Latest_IllegalSupplementsDefinition()
        {
            var supplement = CreateSupplement("sup");

            var plan1 = CreateSupplementsCycleDefinition("plan1", supplement, profiles[0], PublishStatus.Published);
            plan1.PublishDate = DateTime.UtcNow.Date;
            plan1.Status = PublishStatus.Published;
            insertToDatabase(plan1);
            var plan2 = CreateSupplementsCycleDefinition("plan2", supplement, profiles[1], PublishStatus.Published);
            plan2.PublishDate = DateTime.UtcNow.Date.AddDays(-1);
            plan2.CanBeIllegal = true;
            plan2.Status = PublishStatus.Published;
            insertToDatabase(plan2);

            var plan4 = CreateSupplementsCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Private);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(0, result.SupplementsDefinitions.Count);

                Assert.AreEqual(0, result.RandomSupplementsDefinitions.Count);
            });
        }

        SupplementCycleDefinition createSupplementCycleDefinition(string name,Suplement supplement,Profile profile,PublishStatus status,DateTime publishDate)
        {
            var plan2 = CreateSupplementsCycleDefinition(name, supplement, profile);
            plan2.PublishDate = publishDate;
            plan2.Status = status;
            var week = new SupplementCycleWeek();
            var dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            plan2.Weeks.Add(week);
            week.Definition = plan2;
            week = new SupplementCycleWeek();
            dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            dosage = new SupplementCycleDosage();
            dosage.Supplement = supplement;
            week.Dosages.Add(dosage);
            dosage.Week = week;
            plan2.Weeks.Add(week);
            week.Definition = plan2;
            insertToDatabase(plan2);
            return plan2;
        }

        [Test]
        public void Latest_Distinct_Many_SupplementsDefinitions()
        {
            var supplement = CreateSupplement("sup");

            var plan1=createSupplementCycleDefinition("plan1",supplement,profiles[0],PublishStatus.Published,DateTime.Now.Date);
            var plan2 = createSupplementCycleDefinition("plan2", supplement, profiles[1], PublishStatus.Published, DateTime.Now.Date.AddDays(-1));
            var plan3 = createSupplementCycleDefinition("plan3", supplement, profiles[2], PublishStatus.Published, DateTime.Now.Date.AddDays(-2));
            var plan4 = createSupplementCycleDefinition("plan4", supplement, profiles[2], PublishStatus.Published, DateTime.Now.Date.AddDays(-12));
            var plan5 = createSupplementCycleDefinition("plan5", supplement, profiles[1], PublishStatus.Published, DateTime.Now.Date.AddDays(-10));
            

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.SupplementsDefinitions.Count);
                Assert.AreEqual(plan2.GlobalId, result.SupplementsDefinitions[0].GlobalId);
                Assert.AreEqual(plan3.GlobalId, result.SupplementsDefinitions[1].GlobalId);

                Assert.GreaterOrEqual(result.RandomSupplementsDefinitions.Count,1);
                Assert.AreNotEqual(result.SupplementsDefinitions[0].GlobalId, result.RandomSupplementsDefinitions[0].GlobalId);
                if (result.RandomSupplementsDefinitions.Count > 1)
                {
                    Assert.AreNotEqual(result.SupplementsDefinitions[1].GlobalId, result.RandomSupplementsDefinitions[0].GlobalId);
                }
                
            });
        }
        #endregion

        #region Entry objects

        BlogEntry createBlog(DateTime date,Profile profile)
        {
            TrainingDay day = new TrainingDay(date);
            day.Profile = profile;
            BlogEntry blog = new BlogEntry();
            blog.Comment = "dfgdfg dsdfg dsfgsdfg sdfg sfss sfgf";
            day.AddEntry(blog);
            insertToDatabase(day);
            return blog;
        }

        StrengthTrainingEntry addTrainingDaySet(Profile profile,  DateTime date, Exercise exercise, params Tuple<int?, decimal?>[] sets)
        {
            var trainingDay = new TrainingDay(date);
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
        public void Take3LatestBlogs()
        {
            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[2]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[3]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[4]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestBlogs.Count);
                Assert.AreEqual(blog2.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(profiles[1].GlobalId, result.LatestBlogs[0].User.GlobalId);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[1].GlobalId);
                Assert.AreEqual(blog4.GlobalId, result.LatestBlogs[2].GlobalId);
            });
        }

        [Test]
        public void Blogs_DoNotRetrieveBlogs()
        {
            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[2]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[3]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[4]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam(){LatestBlogsCount = 0});

                Assert.AreEqual(0, result.LatestBlogs.Count);
            });
        }

        [Test]
        public void LatestBlog_TakeOnlyOneForEachUser()
        {
            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[1]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[2]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[2]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestBlogs.Count);
                Assert.AreEqual(blog2.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[1].GlobalId);
            });
        }

        [Test]
        public void LatestBlog_Bug1()
        {
            var ex = CreateExercise(Session, null, "name", "ex");
            addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-1), ex);
            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[1]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[2]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[2]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestBlogs.Count);
                Assert.AreEqual(blog2.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[1].GlobalId);
            });
        }

        [Test]
        public void LatestBlogs_PrivateCalendar()
        {
            profiles[1].Privacy.CalendarView = Privacy.Private;
            insertToDatabase(profiles[1]);
            profiles[2].Privacy.CalendarView = Privacy.Private;
            insertToDatabase(profiles[2]);

            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[2]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[3]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[4]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestBlogs.Count);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(blog4.GlobalId, result.LatestBlogs[1].GlobalId);
            });
        }

        [Test]
        public void LatestBlogs_TooLittleTrainingDaysInCalendar()
        {
            profiles[1].Statistics.TrainingDaysCount = 40;
            insertToDatabase(profiles[1].Statistics);
            profiles[2].Statistics.TrainingDaysCount = 1;
            insertToDatabase(profiles[2].Statistics);

            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[2]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[3]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[4]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestBlogs.Count);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(blog4.GlobalId, result.LatestBlogs[1].GlobalId);
            });
        }

        [Test]
        public void LatestBlogs_TooShortBlogContent()
        {
            var blog1 = createBlog(DateTime.Now.Date.AddDays(-1), profiles[0]);
            var blog2 = createBlog(DateTime.Now.Date.AddDays(-2), profiles[1]);
            blog2.Comment = "test";
            var blog3 = createBlog(DateTime.Now.Date.AddDays(-7), profiles[2]);
            var blog4 = createBlog(DateTime.Now.Date.AddDays(-4), profiles[3]);
            var blog5 = createBlog(DateTime.Now.Date.AddDays(-3), profiles[4]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestBlogs.Count);
                Assert.AreEqual(blog5.GlobalId, result.LatestBlogs[0].GlobalId);
                Assert.AreEqual(blog4.GlobalId, result.LatestBlogs[1].GlobalId);
                Assert.AreEqual(blog3.GlobalId, result.LatestBlogs[2].GlobalId);
            });
        }

        [Test]
        public void LatestStrengthTrainings_DoNotRetrieve()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));
            var entry6 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-27), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam(){LatestStrengthTrainingsCount = 0});

                Assert.AreEqual(0, result.LatestStrengthTrainings.Count);
            });
        }

        [Test]
        public void LatestStrengthTrainings_TakeOneForEachUserOnly()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));
            var entry6 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-27), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry3.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
                Assert.AreEqual(entry6.GlobalId, result.LatestStrengthTrainings[2].GlobalId);
            });
        }


        [Test]
        public void LatestStrengthTrainings_Bug1()
        {
            createBlog(DateTime.Now.Date.AddDays(-1), profiles[2]);
            var ex = CreateExercise(Session, null, "ex", "ex");
            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[4], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry3.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
                Assert.AreEqual(entry5.GlobalId, result.LatestStrengthTrainings[2].GlobalId);
            });
        }

        [Test]
        public void LatestStrengthTrainings_Take3()
        {
            var ex = CreateExercise(Session, null, "ex","ex");
            var entry1=addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1),ex,new Tuple<int?, decimal?>(2,4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[4], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry3.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
                Assert.AreEqual(entry5.GlobalId, result.LatestStrengthTrainings[2].GlobalId);
            });
        }

        [Test]
        public void LatestStrengthTrainings_Zero()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());
            });
        }

        [Test]
        public void LatestStrengthTrainings_PrivateCalendar()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            profiles[1].Privacy.CalendarView = Privacy.Private;
            insertToDatabase(profiles[1]);
            profiles[2].Privacy.CalendarView = Privacy.Private;
            insertToDatabase(profiles[2]);

            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[4], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry5.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
            });
        }

        [Test]
        public void LatestStrengthTrainings_TooLittleTrainingDaysInCalendar()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            profiles[1].Statistics.TrainingDaysCount = 40;
            insertToDatabase(profiles[1].Statistics);
            profiles[2].Statistics.TrainingDaysCount = 2;
            insertToDatabase(profiles[2].Statistics);

            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4));
            var entry3 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4));
            var entry4 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4));
            var entry5 = addTrainingDaySet(profiles[4], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(2, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry5.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
            });
        }


        [Test]
        public void LatestStrengthTrainings_Distinct()
        {
            var ex = CreateExercise(Session, null, "ex", "ex");
            var entry1 = addTrainingDaySet(profiles[0], DateTime.Now.Date.AddDays(-1), ex, new Tuple<int?, decimal?>(2, 4), new Tuple<int?, decimal?>(1, 4), new Tuple<int?, decimal?>(2, 14));
            var entry2 = addTrainingDaySet(profiles[1], DateTime.Now.Date.AddDays(-11), ex, new Tuple<int?, decimal?>(2, 4), new Tuple<int?, decimal?>(1, 4), new Tuple<int?, decimal?>(2, 14));
            var entry3 = addTrainingDaySet(profiles[2], DateTime.Now.Date.AddDays(-2), ex, new Tuple<int?, decimal?>(2, 4), new Tuple<int?, decimal?>(1, 4), new Tuple<int?, decimal?>(2, 14));
            var entry4 = addTrainingDaySet(profiles[3], DateTime.Now.Date.AddDays(-3), ex, new Tuple<int?, decimal?>(2, 4), new Tuple<int?, decimal?>(1, 4), new Tuple<int?, decimal?>(2, 14));
            var entry5 = addTrainingDaySet(profiles[4], DateTime.Now.Date.AddDays(-7), ex, new Tuple<int?, decimal?>(2, 4), new Tuple<int?, decimal?>(1, 4), new Tuple<int?, decimal?>(2, 14));

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var result = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                Assert.AreEqual(3, result.LatestStrengthTrainings.Count);
                Assert.AreEqual(entry3.GlobalId, result.LatestStrengthTrainings[0].GlobalId);
                Assert.AreEqual(entry4.GlobalId, result.LatestStrengthTrainings[1].GlobalId);
                Assert.AreEqual(entry5.GlobalId, result.LatestStrengthTrainings[2].GlobalId);
            });
        }
        #endregion

        #region Records

        [Test]
        public void Records()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date));
            });
        }

        [Test]
        public void Records_MoreExercises()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex11", "ex11");
            exercise1.UseInRecords = true;
            insertToDatabase(exercise1);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise1, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());
                //this sorting in first param is only to simplify test.order from WS is not importat 
                assert(list.Records.OrderByDescending(x=>x.MaxWeight).ToList(), new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date), new Tuple<Profile,
                    decimal, DateTime>(profiles[2], 40, DateTime.UtcNow.AddDays(-11).Date));
            });
        }

        [Test]
        public void Records_SkipUsersWithSmallerStatistics()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            profiles[1].Statistics.StrengthTrainingEntriesCount = 0;
            profiles[1].Statistics.TrainingDaysCount = 0;
            insertToDatabase(profiles[1].Statistics);

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date));
            });
        }

        [Test]
        public void Records_SkipRecords()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam(){SkipRecords = true});

                Assert.AreEqual(0,list.Records.Count);
            });
        }

        [Test]
        public void Records_SkipCustomers()
        {
            var customer = CreateCustomer("cust",profiles[0]);
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date, customer: customer);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date));
            });
        }

        [Test]
        public void Records_GetMoreReps()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 67), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[1], 67, DateTime.UtcNow.AddDays(-4).Date));
            });
        }

        [Test]
        public void Records_GetMoreRepsAndLowerDate()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-5).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[3], 67, DateTime.UtcNow.AddDays(-4).Date));
            });
        }

        [Test]
        public void Records_SkipExercisesNotForRecords()
        {
            var exercise = CreateExercise(Session, null, "ex1", "ex1");
            exercise.UseInRecords = true;
            insertToDatabase(exercise);
            var exercise1 = CreateExercise(Session, null, "ex11", "ex12");
            CreateExerciseRecord(exercise, profiles[0], new Tuple<int, decimal>(11, 40), DateTime.UtcNow.AddDays(-10).Date);
            CreateExerciseRecord(exercise, profiles[2], new Tuple<int, decimal>(3, 40), DateTime.UtcNow.AddDays(-11).Date);

            CreateExerciseRecord(exercise1, profiles[1], new Tuple<int, decimal>(5, 67), DateTime.UtcNow.AddDays(-4).Date);

            CreateExerciseRecord(exercise, profiles[3], new Tuple<int, decimal>(3, 61), DateTime.UtcNow.AddDays(-5).Date);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var list = service.GetFeaturedData(data.Token, new GetFeaturedDataParam());

                assert(list.Records, new Tuple<Profile, decimal, DateTime>(profiles[3], 61, DateTime.UtcNow.AddDays(-5).Date));
            });
        }

        void assert(IList<ExerciseRecordsReportResultItem> results, params Tuple<Profile, decimal, DateTime>[] weights)
        {
            Assert.AreEqual(weights.Length, results.Count);
            for (int i = 0; i < weights.Length; i++)
            {
                Assert.IsTrue(results[i].MaxWeight == weights[i].Item2);
                Assert.IsTrue(results[i].User.GlobalId == weights[i].Item1.GlobalId);
                Assert.IsTrue(results[i].TrainingDate == weights[i].Item3);
            }
        }
        #endregion
    }
}
