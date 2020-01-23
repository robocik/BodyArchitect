using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using Profile = BodyArchitect.Model.Profile;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_PublishExercise:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        Dictionary<string,Exercise> exercises = new Dictionary<string, Exercise>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                exercises.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var exercise = CreateExercise(Session, profiles[0], "test1-1", "t11",PublishStatus.Private);
                exercise.Url = "http://wp.pl";
                exercises.Add(exercise.Shortcut,exercise);
                exercise = CreateExercise(Session, profiles[0], "test1-2", "t12", PublishStatus.PendingPublish);
                exercises.Add(exercise.Shortcut, exercise);
                exercise = CreateExercise(Session, profiles[0], "test1-3", "t13", PublishStatus.Published);
                exercises.Add(exercise.Shortcut, exercise);

                exercise = CreateExercise(Session, profiles[1], "test2-1", "t21", PublishStatus.Private);
                exercises.Add(exercise.Shortcut, exercise);
                exercise = CreateExercise(Session, profiles[1], "test2-2", "t22", PublishStatus.PendingPublish);
                exercises.Add(exercise.Shortcut, exercise);
                exercise = CreateExercise(Session, profiles[1], "test2-3", "t23", PublishStatus.Private,ExerciseType.NotSet);
                exercises.Add(exercise.Shortcut, exercise);
                tx.Commit();
            }
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void TestPublish_CrossProfile()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t21"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     service.PublishExercise(data.Token, exerciseDto);
                                 });
        }

        [Test]
        public void TestPublish()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t11"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.PublishExercise(data.Token, exerciseDto);
            });
            Assert.AreEqual(PublishStatus.PendingPublish,Session.Get<Exercise>(exerciseDto.GlobalId).Status);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPublish_EmptyUrl()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t21"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.PublishExercise(data.Token, exerciseDto);
            });
            Assert.AreEqual(PublishStatus.PendingPublish, Session.Get<Exercise>(exerciseDto.GlobalId).Status);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPublish_ExerciseTypeNotSet()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t23"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.PublishExercise(data.Token, exerciseDto);
            });
            Assert.AreEqual(PublishStatus.PendingPublish, Session.Get<Exercise>(exerciseDto.GlobalId).Status);
        }

        [Test]
        [ExpectedException(typeof(PublishedObjectOperationException))]
        public void TestPublish_AlreadyPublished()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);

            ExerciseDTO exerciseDto = Mapper.Map<Exercise, ExerciseDTO>(exercises["t13"]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.PublishExercise(data.Token, exerciseDto);
            });
        }
    }
}
