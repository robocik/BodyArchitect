using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_ExerciseOperation:TestServiceBase
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

        [Test]
        public void AddToFavorites()
        {
            var exercise=CreateExercise(Session, profiles[1], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     var param = new ExerciseOperationParam();
                                     param.ExerciseId = exercise.GlobalId;
                                     param.Operation = FavoriteOperation.Add;
                                     service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(1,dbProfile.FavoriteExercises.Count);
            Assert.AreEqual(exercise.GlobalId, dbProfile.FavoriteExercises.ElementAt(0).GlobalId);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddToFavorites_ExerciseFromCurrentProfile()
        {
            var exercise = CreateExercise(Session, profiles[0], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(ObjectIsFavoriteException))]
        public void AddToFavorites_TwiceTheSameExercise()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });
        }

        [Test]
        public void AddToFavorites_DataInfo()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash=profiles[0].DataInfo.ExerciseHash;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.ExerciseHash, oldHash);
        }

        [Test]
        public void AddToFavorites_DataInfo_Owner()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[1].DataInfo.ExerciseHash;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Add;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(dbProfile.DataInfo.ExerciseHash, oldHash);
        }

        [Test]
        public void RemoveFromFavorites()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");
            profiles[0].FavoriteExercises.Add(exercise);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Remove;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreEqual(0, dbProfile.FavoriteExercises.Count);
        }

        [Test]
        [ExpectedException(typeof(ObjectIsNotFavoriteException))]
        public void RemoveFromFavorites_Twice()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");
            profiles[0].FavoriteExercises.Add(exercise);
            insertToDatabase(profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Remove;
                service.ExerciseOperation(data.Token, param);
            });
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Remove;
                service.ExerciseOperation(data.Token, param);
            });
        }

        [Test]
        public void RemoveFromFavorites_DataInfo()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");
            profiles[0].FavoriteExercises.Add(exercise);
            insertToDatabase(profiles[0]);

            var oldHash=profiles[0].DataInfo.ExerciseHash;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Remove;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(dbProfile.DataInfo.ExerciseHash, oldHash);
        }

        [Test]
        public void RemoveFromFavorites_DataInfo_Owner()
        {
            var exercise = CreateExercise(Session, profiles[1], "ex1", "ex1");
            profiles[0].FavoriteExercises.Add(exercise);
            insertToDatabase(profiles[0]);

            var oldHash = profiles[1].DataInfo.ExerciseHash;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var param = new ExerciseOperationParam();
                param.ExerciseId = exercise.GlobalId;
                param.Operation = FavoriteOperation.Remove;
                service.ExerciseOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profiles[1].GlobalId);
            Assert.AreEqual(dbProfile.DataInfo.ExerciseHash, oldHash);
        }
    }
}
