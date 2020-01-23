//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BodyArchitect.Model;
//using BodyArchitect.Service.V2;
//using BodyArchitect.Service.V2.Model;
//using BodyArchitect.Shared;
//using NUnit.Framework;
//using PublishStatus = BodyArchitect.Model.PublishStatus;

//namespace BodyArchitect.UnitTests.V2
//{
//    [TestFixture]
//    public class TestService_ExerciseMapper:TestServiceBase
//    {
//        List<Profile> profiles = new List<Profile>();
//        Dictionary<string,Exercise> exercises= new Dictionary<string, Exercise>();

//        public override void BuildDatabase()
//        {
//            using (var tx = Session.BeginTransaction())
//            {
//                profiles.Clear();
//                exercises.Clear();
//                profiles.Add(CreateProfile(Session, "test1"));
//                profiles.Add(CreateProfile(Session, "test2"));

//                var exercise = CreateExercise(Session, null, "test1", "t1", PublishStatus.Published);
//                exercises.Add(exercise.Shortcut,exercise);

//                exercise = CreateExercise(Session, profiles[0], "test2", "t2", PublishStatus.Published);
//                exercises.Add(exercise.Shortcut, exercise);
//                exercise = CreateExercise(Session, profiles[0], "test3", "t3", PublishStatus.PendingPublish);
//                exercises.Add(exercise.Shortcut, exercise);
//                exercise = CreateExercise(Session, profiles[0], "test4", "t4", PublishStatus.Private);
//                exercises.Add(exercise.Shortcut, exercise);
//                exercise = CreateExercise(Session, profiles[1], "test5", "t5", PublishStatus.Published);
//                exercises.Add(exercise.Shortcut, exercise);
//                exercise = CreateExercise(Session, profiles[1], "test6", "t6", PublishStatus.PendingPublish);
//                exercises.Add(exercise.Shortcut, exercise);
//                exercise = CreateExercise(Session, profiles[1], "test7", "t7", PublishStatus.Private);
//                exercises.Add(exercise.Shortcut, exercise);

//                tx.Commit();
//            }
//        }

//        void createStrengthEntry(DateTime date,Profile profile,params string[] exerciseShortcut)
//        {
//            TrainingDay day = new TrainingDay(date);
//            day.Profile = profile;
//            StrengthTrainingEntry entry = new StrengthTrainingEntry();
//            day.AddEntry(entry);
//            foreach (var shortcut in exerciseShortcut)
//            {
//                StrengthTrainingItem item = new StrengthTrainingItem();
//                item.Exercise = exercises[shortcut];
//                entry.AddEntry(item);
//            }
            
//            insertToDatabase(day);
//        }

//        [Test]
//        public void TestMapExercises_PrivateToGlobal()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0],"t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0],"t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0],"t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0],"t4","t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData=new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId,exercises["t1"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(0,Session.QueryOver<StrengthTrainingItem>().Where(x=>x.Exercise.GlobalId==exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(4, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//        }

//        [Test]
//        public void TestMapExercises_GlobalToPrivate()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperResult result = null;
//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t1"].GlobalId, exercises["t4"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                result=service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(2,result.EntriesAffected);
//            Assert.AreEqual(4, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void TestMapExercises_ToPrivateFromOtherUser()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t1"].GlobalId, exercises["t7"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
           
//        }

//        [Test]
//        public void TestMapExercises_NotChangeEntriesFromOtherUsers()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[1], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperResult result = null;
//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t1"].GlobalId, exercises["t2"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                result=service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(2,result.EntriesAffected);
//            Assert.IsNotNull(Session.Get<Exercise>(exercises["t1"].GlobalId));
//            //one instance from another user still exists
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(2, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t2"].GlobalId).RowCount());
//        }

//        [Test]
//        public void TestMapExercises_TwoExercisesToMap()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperResult result = null;
//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId));
//            mapperData.Entries.Add(new MapperEntry(exercises["t3"].GlobalId, exercises["t2"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                result=service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(3,result.EntriesAffected);
//            Assert.IsNotNull(Session.Get<Exercise>(exercises["t3"].GlobalId));
//            Assert.IsNotNull(Session.Get<Exercise>(exercises["t4"].GlobalId));
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(4, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t2"].GlobalId).RowCount());
//        }

//        [Test]
//        public void TestMapExercises_TwoExercisesToMap_OneToRemove()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperResult result = null;
//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId,MapperEntryOperation.ReMapAndDeleteExercise));
//            mapperData.Entries.Add(new MapperEntry(exercises["t3"].GlobalId, exercises["t2"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                result = service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(3, result.EntriesAffected);
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(4, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t2"].GlobalId).RowCount());

//            Assert.IsNull(Session.Get<Exercise>(exercises["t4"].GlobalId));
//            Assert.IsNotNull(Session.Get<Exercise>(exercises["t3"].GlobalId));
//        }

//        [Test]
//        public void TestMapExercises_TwoExercisesToMap_RemvoingPublicExercise()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t5");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperResult result = null;
//            MapperData mapperData = new MapperData();
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId, MapperEntryOperation.ReMapAndDeleteExercise));
//            mapperData.Entries.Add(new MapperEntry(exercises["t5"].GlobalId, exercises["t2"].GlobalId, MapperEntryOperation.ReMapAndDeleteExercise));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                result = service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(3, result.EntriesAffected);
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(0, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t5"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t2"].GlobalId).RowCount());
//            Assert.AreEqual(3, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());

//            Assert.IsNull(Session.Get<Exercise>(exercises["t4"].GlobalId));
//            Assert.IsNotNull(Session.Get<Exercise>(exercises["t5"].GlobalId));
//        }

//        [Test]
//        public void TestMapExercises_WithStartDate()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData = new MapperData();
//            mapperData.StartDate = DateTime.Now.AddDays(1);
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(3, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//        }

//        [Test]
//        public void TestMapExercises_WithEndDate()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData = new MapperData();
//            mapperData.EndDate = DateTime.Now.AddDays(2);
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(3, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//        }

//        [Test]
//        public void TestMapExercises_WithStartAndEndDate()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData = new MapperData();
//            mapperData.StartDate = DateTime.Now.AddDays(1);
//            mapperData.EndDate = DateTime.Now.AddDays(2);
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
//            Assert.AreEqual(2, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t4"].GlobalId).RowCount());
//            Assert.AreEqual(1, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t3"].GlobalId).RowCount());
//            Assert.AreEqual(2, Session.QueryOver<StrengthTrainingItem>().Where(x => x.Exercise.GlobalId == exercises["t1"].GlobalId).RowCount());
//        }

//        [Test]
//        [ExpectedException(typeof(ValidationException))]
//        public void TestMapExercises_DuplicateFromExercises()
//        {
//            createStrengthEntry(DateTime.Now, profiles[0], "t4");
//            createStrengthEntry(DateTime.Now.AddDays(1), profiles[0], "t3");
//            createStrengthEntry(DateTime.Now.AddDays(2), profiles[0], "t1");
//            createStrengthEntry(DateTime.Now.AddDays(3), profiles[0], "t4", "t1");


//            ProfileDTO profile1 = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile1, ClientInformation);

//            MapperData mapperData = new MapperData();

//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t1"].GlobalId));
//            mapperData.Entries.Add(new MapperEntry(exercises["t4"].GlobalId, exercises["t2"].GlobalId));
//            RunServiceMethod(delegate(InternalBodyArchitectService service)
//            {
//                service.MapExercises(data.Token, mapperData);
//            });
           
//        }
//    }
//}
