using System;
using System.Collections.Generic;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using ExerciseForceType = BodyArchitect.Service.V2.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Service.V2.Model.ExerciseType;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_SaveDeleteExercise : TestServiceBase
    {
        private List<Profile> profiles = new List<Profile>();

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

        void assert(ExerciseDTO exerciseDto, Exercise dbExercise)
        {
            Assert.AreEqual(exerciseDto.GlobalId, dbExercise.GlobalId);
            Assert.AreEqual(exerciseDto.Name, dbExercise.Name);
            Assert.AreEqual((int)exerciseDto.ExerciseForceType, (int)dbExercise.ExerciseForceType);
            Assert.AreEqual((int)exerciseDto.ExerciseType, (int)dbExercise.ExerciseType);
            Assert.AreEqual(exerciseDto.Description, dbExercise.Description);
            Assert.AreEqual(exerciseDto.ProfileId, dbExercise.Profile.GlobalId);
            Assert.AreEqual((int)exerciseDto.MechanicsType, (int)dbExercise.MechanicsType);
            Assert.AreEqual((int)exerciseDto.Difficult, (int)dbExercise.Difficult);
            Assert.AreEqual(exerciseDto.Url, dbExercise.Url);
        }

        [Test]
        public void CreateExercise()
        {
            var profile = (ProfileDTO) profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            ExerciseDTO exercise = new ExerciseDTO();
            exercise.Name = "test1";
            exercise.Profile = profile;
            exercise.Shortcut = "SC";
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Description = "desc";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Url = "url";

            ExerciseDTO exerciseDto=null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     TimerService.UtcNow = DateTime.UtcNow.AddDays(-2).Date;
                exerciseDto = service.SaveExercise(data.Token, exercise);
                UnitTestHelper.SetProperty(exercise,"Version",1);//for compareobjects only
                UnitTestHelper.SetProperty(exercise, "GlobalId", exerciseDto.GlobalId);//for compareobjects only
                UnitTestHelper.CompareObjects(exercise,exerciseDto);
            });
            var dbExercise=Session.Get<Exercise>(exerciseDto.GlobalId);
            assert(exerciseDto, dbExercise);
            Assert.IsTrue(UnitTestHelper.CompareDateTime(DateTime.UtcNow.AddDays(-2).Date, dbExercise.CreationDate));
        }


        [Test]
        public void CreateExercise_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile1, ClientInformation);
            ExerciseDTO exercise = new ExerciseDTO();
            exercise.Name = "test1";
            exercise.Profile = profile;
            exercise.Shortcut = "SC";
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Description = "desc";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Url = "url";

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                exercise=service.SaveExercise(data.Token, exercise);
            });
            var db=Session.Get<Exercise>(exercise.GlobalId);
            Assert.AreEqual(profile1.GlobalId, db.Profile.GlobalId);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateExercise_AnotherProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            var exercise = CreateExercise(Session, profiles[0], "ex1", "EE");

            SessionData data = CreateNewSession(profile1, ClientInformation);

            ExerciseDTO exerciseDto = exercise.Map<ExerciseDTO>();
            exerciseDto.Name = "new name";
            exerciseDto.Description = "new desc";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveExercise(data.Token, exerciseDto);
            });
        }

        [Test]
        public void UpdateExercise_Rating()
        {
            var profile1 = (ProfileDTO)profiles[1].Tag;

            var exercise = CreateExercise(Session, profiles[1], "ex1", "EE");
            exercise.Rating = 4;
            insertToDatabase(exercise);

            SessionData data = CreateNewSession(profile1, ClientInformation);

            ExerciseDTO savedExercise = null;
            ExerciseDTO exerciseDto = exercise.Map<ExerciseDTO>();
            exerciseDto.Rating = 3;
            exerciseDto.Description = "new desc";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedExercise = service.SaveExercise(data.Token, exerciseDto);
            });
            var dbExercise = Session.Get<Exercise>(exerciseDto.GlobalId);
            Assert.AreEqual(4f,dbExercise.Rating);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateExercise_ChangeName()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "ex1", "EE");

            SessionData data = CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto =  exercise.Map<ExerciseDTO>();
            exerciseDto.Name = "new name";
            exerciseDto.Description = "new desc";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveExercise(data.Token, exerciseDto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateExercise_SecurityBug()
        {
            var profile = (ProfileDTO)profiles[1].Tag;

            var exercise = CreateExercise(Session, profiles[0], "ex1", "EE");

            SessionData data = CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = exercise.Map<ExerciseDTO>();
            exerciseDto.Description = "new desc";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveExercise(data.Token, exerciseDto);
            });
        }

        [Test]
        public void UpdateExercise_ChangeDetails_NameIsNotChanged()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            var exercise = CreateExercise(Session, profiles[0], "ex1", "EE");

            SessionData data = CreateNewSession(profile, ClientInformation);

            ExerciseDTO savedExercise = null;
            ExerciseDTO exerciseDto = exercise.Map<ExerciseDTO>();
            exerciseDto.Description = "new desc";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                savedExercise=service.SaveExercise(data.Token, exerciseDto);
            });
            var dbExercise = Session.Get<Exercise>(savedExercise.GlobalId);
            Assert.AreEqual(exerciseDto.Description,dbExercise.Description);
            Assert.AreEqual(1, Session.QueryOver<Exercise>().RowCount());
        }

        //[Test]
        //public void SaveExercise_UserRatingInResult()
        //{
        //    var profile = (ProfileDTO)profiles[0].Tag;

        //    SessionData data = CreateNewSession(profile, ClientInformation);

        //    var exercise = CreateExercise(Session, profiles[0], "ex1", "EE", PublishStatus.Private);

        //    RatingUserValue rating = new RatingUserValue();
        //    rating.RatedObjectId = exercise.GlobalId;
        //    rating.Rating = 2;
        //    rating.ProfileId = profile.GlobalId;
        //    rating.VotedDate = DateTime.UtcNow;
        //    insertToDatabase(rating);


        //    RunServiceMethod(delegate(InternalBodyArchitectService Service)
        //    {
        //        var exerciseDto = Service.SaveExercise(data.Token, exercise.Map<ExerciseDTO>());

        //        Assert.AreEqual(rating.Rating, exerciseDto.UserRating);
        //        Assert.AreEqual(rating.ShortComment, exerciseDto.UserShortComment);
        //    });
        //}

        [Test]
        public void CreateExercise_DataInfo_Refresh()
        {
            var profile = (ProfileDTO)profiles[0].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation,new LoginData());

            var exercise = CreateExercise(Session,profiles[0],"ex1","EE");
            var oldHash = profiles[0].DataInfo.ExerciseHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.SaveExercise(data.Token, exercise.Map<ExerciseDTO>());
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.ExerciseHash,oldHash);
        }

    }
}
