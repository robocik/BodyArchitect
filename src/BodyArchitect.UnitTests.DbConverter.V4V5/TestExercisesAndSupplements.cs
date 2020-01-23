using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model.Old;
using NHibernate;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestExercisesAndSupplements : NHibernateTestFixtureBase
    {
        #region Exercises
        [Test]
        public void ConvertExercises_Public()
        {
            Model.Old.Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Name = "Exercise name";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.Description = "desc";
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Shortcut = "EX";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.Status = PublishStatus.Published;
            exercise.Profile = null;
            exercise.Url = "Test";
            insertToOldDatabase(exercise);

            Convert();

            var count = SessionNew.QueryOver<Model.Exercise>().RowCount();
            Assert.AreEqual(2,count);
            var newExercise = SessionNew.Get<Model.Exercise>(exercise.GlobalId);
            Assert.AreEqual(exercise.Name, newExercise.Name);
            Assert.AreEqual(exercise.Description, newExercise.Description);
            Assert.AreEqual(exercise.Url, newExercise.Url);
            Assert.AreEqual((int)exercise.ExerciseType,(int) newExercise.ExerciseType);
            Assert.AreEqual((int)exercise.MechanicsType, (int)newExercise.MechanicsType);
            Assert.AreEqual((int)exercise.ExerciseForceType, (int)newExercise.ExerciseForceType);
            Assert.AreEqual((int)exercise.Difficult, (int)newExercise.Difficult);
            Assert.IsNull(newExercise.Profile);
            Assert.AreEqual(exercise.Shortcut, newExercise.Shortcut);
            Assert.AreEqual(exercise.Rating, newExercise.Rating);
        }

        [Test]
        public void ConvertExercises_Private()
        {
            var profile = CreateProfile("profile");
            Model.Old.Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Name = "Exercise name";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.Description = "desc";
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Shortcut = "EX";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.Status = PublishStatus.Private;
            exercise.Profile = profile;
            exercise.Url = "Test";
            insertToOldDatabase(exercise);

            Convert();

            var count = SessionNew.QueryOver<Model.Exercise>().RowCount();
            Assert.AreEqual(2, count);
            var newExercise = SessionNew.Get<Model.Exercise>(exercise.GlobalId);
            Assert.AreEqual(exercise.Name, newExercise.Name);
            Assert.AreEqual(exercise.Description, newExercise.Description);
            Assert.AreEqual(exercise.Url, newExercise.Url);
            Assert.AreEqual((int)exercise.ExerciseType, (int)newExercise.ExerciseType);
            Assert.AreEqual((int)exercise.MechanicsType, (int)newExercise.MechanicsType);
            Assert.AreEqual((int)exercise.ExerciseForceType, (int)newExercise.ExerciseForceType);
            Assert.AreEqual((int)exercise.Difficult, (int)newExercise.Difficult);
            Assert.IsNotNull(newExercise.Profile);
            Assert.AreEqual(exercise.Profile.UserName, newExercise.Profile.UserName);
            Assert.AreEqual(exercise.Shortcut, newExercise.Shortcut);
            Assert.AreEqual(exercise.Rating, newExercise.Rating);
        }

        [Test]
        public void ConvertExercises_WithRatings()
        {
            var profile1 = CreateProfile("profile1");
            var profile2 = CreateProfile("profile2");

            Model.Old.Exercise exercise = new Exercise(Guid.NewGuid());
            exercise.Name = "Exercise name";
            exercise.Shortcut = "EX";
            insertToOldDatabase(exercise);

            RatingUserValue rating = new RatingUserValue();
            rating.ProfileId = profile1.Id;
            rating.Rating = 3;
            rating.RatedObjectId = exercise.GlobalId;
            rating.ShortComment = "Comment";
            rating.VotedDate = DateTime.Now.Date;
            insertToOldDatabase(rating);

            rating = new RatingUserValue();
            rating.ProfileId = profile2.Id;
            rating.Rating = 5;
            rating.RatedObjectId = exercise.GlobalId;
            rating.ShortComment = "Comment111";
            rating.VotedDate = DateTime.Now.Date;
            insertToOldDatabase(rating);

            Convert();


            var newExercise = SessionNew.Get<Model.Exercise>(exercise.GlobalId);
            Assert.AreEqual(4,newExercise.Rating);
            var ratings = SessionNew.QueryOver<Model.RatingUserValue>().List();
            Assert.AreEqual(2, ratings.Count);
            var rating1 = ratings.Where(x => x.Rating == 3).Single();
            Assert.AreEqual("Comment",rating1.ShortComment);
            Assert.AreEqual(DateTime.Now.Date, rating1.VotedDate);
            Assert.AreEqual(newExercise.GlobalId, rating1.RatedObjectId);
            var newProfile1 =SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            Assert.AreEqual(newProfile1.GlobalId, rating1.ProfileId);


            var rating2 = ratings.Where(x => x.Rating == 5).Single();
            Assert.AreEqual("Comment111", rating2.ShortComment);
            Assert.AreEqual(DateTime.Now.Date, rating2.VotedDate);
            Assert.AreEqual(newExercise.GlobalId, rating2.RatedObjectId);
            var newProfile2 = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile2").SingleOrDefault();
            Assert.AreEqual(newProfile2.GlobalId, rating2.ProfileId);
        }
        #endregion

        #region Supplements

        [Test]
        public void ConvertSupplement()
        {
            var supplement = new Suplement();
            supplement.Name = "supp";
            supplement.Comment = "dfgdfgdfg";
            supplement.SuplementId = Guid.NewGuid();
            supplement.Url = "test";
            insertToOldDatabase(supplement);

            Convert();

            var count = SessionNew.QueryOver<Model.Suplement>().RowCount();
            Assert.AreEqual(2, count);
            var newSupplement = SessionNew.Get<Model.Suplement>(supplement.SuplementId);
            Assert.AreEqual(supplement.Name, newSupplement.Name);
            Assert.AreEqual(supplement.Comment, newSupplement.Comment);
            Assert.AreEqual(supplement.Url, newSupplement.Url);
            Assert.IsFalse( newSupplement.CanBeIllegal);
            Assert.IsFalse(newSupplement.IsProduct);
        }

        [Test]
        public void ConvertSupplement_Steroids()
        {
            var sup = SessionOld.QueryOver<Model.Old.Suplement>().Where(x=>x.SuplementId==new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9")).SingleOrDefault();
            SessionOld.Delete(sup);
            SessionOld.Flush();
            var supplement = new Suplement();
            supplement.Name = "supp";
            supplement.Comment = "dfgdfgdfg";
            supplement.SuplementId = new Guid("F099FF98-BB78-4E99-AFA3-EE0355974CD9");
            supplement.Url = "test";
            insertToOldDatabase(supplement);

            Convert();

            var newSupplement = SessionNew.Get<Model.Suplement>(supplement.SuplementId);
            Assert.AreEqual(supplement.Name, newSupplement.Name);
            Assert.AreEqual(supplement.Comment, newSupplement.Comment);
            Assert.AreEqual(supplement.Url, newSupplement.Url);
            Assert.IsTrue(newSupplement.CanBeIllegal);
            Assert.IsFalse(newSupplement.IsProduct);
        }

        [Test]
        public void ConvertSupplement_PH()
        {
            var supplement = new Suplement();
            supplement.Name = "supp";
            supplement.Comment = "dfgdfgdfg";
            supplement.SuplementId = new Guid("D8F8FD0D-31E0-4763-9F1E-ED5AE49DFBD8");
            supplement.Url = "test";
            insertToOldDatabase(supplement);

            Convert();

            var newSupplement = SessionNew.Get<Model.Suplement>(supplement.SuplementId);
            Assert.AreEqual(supplement.Name, newSupplement.Name);
            Assert.AreEqual(supplement.Comment, newSupplement.Comment);
            Assert.AreEqual(supplement.Url, newSupplement.Url);
            Assert.IsTrue(newSupplement.CanBeIllegal);
            Assert.IsFalse(newSupplement.IsProduct);
        }
        #endregion

        #region Exercise records

        [Test]
        public void CreateExerciseProfileData_ForUserWithoutAnyStrengthTrainingEntries()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());

            Convert();

            var count = SessionNew.QueryOver<Model.ExerciseProfileData>().RowCount();
            Assert.AreEqual(0,count);
        }

        [Test]
        public void CreateExerciseProfileData_ForUserWithStrengthTrainingEntries()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30),new Tuple<int?, float?>(5,50));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(4, 70));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 70).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profile.UserName, data.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, data.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, data.Serie.GlobalId);
            Assert.AreEqual(70, data.MaxWeight);
            Assert.AreEqual(4, data.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), data.TrainingDate);
        }

        [Test]
        public void CreateExerciseProfileData_NullRepetitions()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30), new Tuple<int?, float?>(5, 50));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(null, 70));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 60).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profile.UserName, data.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, data.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, data.Serie.GlobalId);
            Assert.AreEqual(60, data.MaxWeight);
            Assert.AreEqual(3, data.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), data.TrainingDate);
        }

        [Test]
        public void CreateExerciseProfileData_NullRepetitions_ForCardio()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid(),ExerciseType.Cardio);
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30), new Tuple<int?, float?>(5, 50));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(70,null));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var count = SessionNew.QueryOver<Model.Serie>().RowCount();
            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 70).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profile.UserName, data.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, data.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, data.Serie.GlobalId);
            Assert.AreEqual(70, data.MaxWeight);
            Assert.AreEqual(null, data.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), data.TrainingDate);
        }

        [Test]
        public void CreateExerciseProfileData_SetValidator()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid(), ExerciseType.Biceps);
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30), new Tuple<int?, float?>(5, 50));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-2), ex, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(12, 370));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 60).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().SingleOrDefault();
            Assert.AreEqual(profile.UserName, data.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, data.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, data.Serie.GlobalId);
            Assert.AreEqual(60, data.MaxWeight);
            Assert.AreEqual(3, data.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), data.TrainingDate);
        }

        [Test]
        public void CreateExerciseProfileData_NullWeight()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, null), new Tuple<int?, float?>(5, null));

            Convert();

            var count = SessionNew.QueryOver<Model.ExerciseProfileData>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void CreateExerciseProfileData_0Weight()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex,  new Tuple<int?, float?>(5, 0));

            Convert();

            var count = SessionNew.QueryOver<Model.ExerciseProfileData>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void CreateExerciseProfileData_0Repetitions()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(0, 30));

            Convert();

            var count = SessionNew.QueryOver<Model.ExerciseProfileData>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        public void CreateExerciseProfileData_TwoExercises()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            var ex1 = CreateExercise("test1", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30), new Tuple<int?, float?>(5, 50));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-2), ex1, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(4, 70));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 70).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().List();
            Assert.AreEqual(2, data.Count);
            var dbData = data.Where(x => x.MaxWeight == 70).Single();
            Assert.AreEqual(profile.UserName, dbData.Profile.UserName);
            Assert.AreEqual(ex1.GlobalId, dbData.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, dbData.Serie.GlobalId);
            Assert.AreEqual(70, dbData.MaxWeight);
            Assert.AreEqual(4, dbData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), dbData.TrainingDate);

            dbData = data.Where(x => x.MaxWeight == 60).Single();
            serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 60).SingleOrDefault();
            Assert.AreEqual(profile.UserName, dbData.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, dbData.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, dbData.Serie.GlobalId);
            Assert.AreEqual(60, dbData.MaxWeight);
            Assert.AreEqual(3, dbData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), dbData.TrainingDate);
        }

        [Test]
        public void CreateExerciseProfileData_TwoExercisesForTwoUsers()
        {
            var profile = CreateProfile("profile");
            var profile1 = CreateProfile("profile1");
            var ex = CreateExercise("test", Guid.NewGuid());
            var ex1 = CreateExercise("test1", Guid.NewGuid());
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-5), ex, new Tuple<int?, float?>(10, 30), new Tuple<int?, float?>(5, 50));
            addTrainingDaySet(profile1, DateTime.UtcNow.Date.AddDays(-2), ex1, new Tuple<int?, float?>(10, 20), new Tuple<int?, float?>(4, 70));
            addTrainingDaySet(profile, DateTime.UtcNow.Date.AddDays(-1), ex, new Tuple<int?, float?>(3, 60), new Tuple<int?, float?>(5, 40));

            Convert();

            var serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 70).SingleOrDefault();
            var data = SessionNew.QueryOver<Model.ExerciseProfileData>().List();
            Assert.AreEqual(2,data.Count);

            var dbData = data.Where(x => x.MaxWeight == 70).Single();
            Assert.AreEqual(profile1.UserName, dbData.Profile.UserName);
            Assert.AreEqual(ex1.GlobalId, dbData.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, dbData.Serie.GlobalId);
            Assert.AreEqual(70, dbData.MaxWeight);
            Assert.AreEqual(4, dbData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-2), dbData.TrainingDate);

            serie = SessionNew.QueryOver<Model.Serie>().Where(x => x.Weight == 60).SingleOrDefault();
            dbData = data.Where(x => x.MaxWeight == 60).Single();
            Assert.AreEqual(profile.UserName, dbData.Profile.UserName);
            Assert.AreEqual(ex.GlobalId, dbData.Exercise.GlobalId);
            Assert.AreEqual(serie.GlobalId, dbData.Serie.GlobalId);
            Assert.AreEqual(60, dbData.MaxWeight);
            Assert.AreEqual(3, dbData.Repetitions);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), dbData.TrainingDate);
        }
        StrengthTrainingEntry addTrainingDaySet(Model.Old.Profile profile,  DateTime date, Exercise exercise, params Tuple<int?, float?>[] sets)
        {
            var trainingDay = new TrainingDay(date);
            trainingDay.Profile = profile;
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            trainingDay.AddEntry(entry);
            StrengthTrainingItem item = new StrengthTrainingItem();
            item.ExerciseId = exercise.GlobalId;
            entry.AddEntry(item);

            foreach (var tuple in sets)
            {
                Serie set1 = new Serie();
                set1.RepetitionNumber = tuple.Item1;
                set1.Weight = tuple.Item2;
                item.AddSerie(set1);
            }
            insertToOldDatabase(trainingDay);
            return entry;
        }
        #endregion
    }
}
