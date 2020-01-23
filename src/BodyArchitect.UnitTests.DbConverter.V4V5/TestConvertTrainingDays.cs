using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model.Old;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestConvertTrainingDays : NHibernateTestFixtureBase
    {
        [Test]
        public void ConvertTrainingDay()
        {
            var profile = CreateProfile("profile");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new SizeEntry();
            size.Wymiary=new Wymiary();
            size.Wymiary.Klatka = 22;
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay=SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Assert.AreEqual(day.TrainingDate,newDay.TrainingDate);
            Assert.AreEqual(day.Comment, newDay.Comment);
            Assert.AreEqual(day.Profile.UserName, newDay.Profile.UserName);
            Assert.AreEqual(1, newDay.Objects.Count);
        }

        Model.MyPlace GetDefaultMyPlace(Model.Profile profile)
        {
            return SessionNew.QueryOver<Model.MyPlace>().Where(x => x.Profile == profile && x.IsDefault).SingleOrDefault();
        }

        [Test]
        public void ConvertStrengthTrainingEntry_DefaultMyPlace()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test",Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.SuperSetGroup = "rom";
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();
            
            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var defaultMyPlace = GetDefaultMyPlace(newDay.Profile);
            Assert.AreEqual(defaultMyPlace, ((Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0)).MyPlace);
        }

        [Test]
        public void ConvertSupplementEntry()
        {
            var profile = CreateProfile("profile");
            var ex = CreateSupplement("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new SuplementsEntry();
            size.Comment = "test";
            var item = new SuplementItem();
            item.SuplementId = ex.SuplementId;
            size.AddItem(item);
            item.Comment = "com";
            item.Dosage = 44;
            item.DosageType = DosageType.Tablets;
            item.Time = DateTime.UtcNow.Date;
            day.AddEntry(size);
            var item1 = new SuplementItem();
            item1.SuplementId = ex.SuplementId;
            size.AddItem(item1);
            item1.Comment = "co1111m";
            item1.Dosage = 11;
            item1.DosageType = DosageType.Grams;
            item1.Time = DateTime.UtcNow.Date.AddDays(-3);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Model.SuplementsEntry newEntry = (Model.SuplementsEntry) newDay.Objects.Single();
            Assert.AreEqual(2,newEntry.Items.Count);
            var newItem1 = newEntry.Items.Where(x => x.Comment == "com").Single();
            Assert.AreEqual((decimal)item.Dosage, newItem1.Dosage);
            Assert.AreEqual((int)item.DosageType, (int)newItem1.DosageType);
            Assert.AreEqual(item.Time, newItem1.Time.DateTime);
            Assert.AreEqual(BodyArchitect.Model.TimeType.NotSet, newItem1.Time.TimeType);
            Assert.AreEqual(item.SuplementId, newItem1.Suplement.GlobalId);
            var newItem2 = newEntry.Items.Where(x => x.Comment == "co1111m").Single();
            Assert.AreEqual((decimal)item1.Dosage, newItem2.Dosage);
            Assert.AreEqual((int)item1.DosageType, (int)newItem2.DosageType);
            Assert.AreEqual(item1.Time, newItem2.Time.DateTime);
            Assert.AreEqual(BodyArchitect.Model.TimeType.NotSet, newItem2.Time.TimeType);
            Assert.AreEqual(item1.SuplementId, newItem2.Suplement.GlobalId);
        }

        [Test]
        public void ConvertSupplementEntry_Position()
        {
            var profile = CreateProfile("profile");
            var ex = CreateSupplement("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new SuplementsEntry();
            size.Comment = "test";
            var item = new SuplementItem();
            item.SuplementId = ex.SuplementId;
            size.AddItem(item);
            item.Comment = "com";
            item.Dosage = 44;
            item.DosageType = DosageType.Tablets;
            item.Time = DateTime.UtcNow.Date;
            day.AddEntry(size);
            var item1 = new SuplementItem();
            item1.SuplementId = ex.SuplementId;
            size.AddItem(item1);
            item1.Comment = "co1111m";
            item1.Dosage = 11;
            item1.DosageType = DosageType.Grams;
            item1.Time = DateTime.UtcNow.Date.AddDays(-3);
            var item2 = new SuplementItem();
            item2.SuplementId = ex.SuplementId;
            size.AddItem(item2);
            item2.Comment = "co111122m";
            item2.Dosage = 121;
            item2.DosageType = DosageType.Grams;
            item2.Time = DateTime.UtcNow.Date.AddDays(-3);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Model.SuplementsEntry newEntry = (Model.SuplementsEntry)newDay.Objects.Single();
            Assert.AreEqual(3, newEntry.Items.Count);

            Assert.AreEqual((decimal)item.Dosage, newEntry.Items.ElementAt(0).Dosage);
            Assert.AreEqual((decimal)item1.Dosage, newEntry.Items.ElementAt(1).Dosage);
            Assert.AreEqual((decimal)item2.Dosage, newEntry.Items.ElementAt(2).Dosage);
        }

        [Test]
        public void ConvertStrengthTrainingEntry()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry) newDay.Objects.ElementAt(0);
            Assert.AreEqual(1,newStrength.Entries.Count);
            Assert.AreEqual((int)size.Intensity, (int)newStrength.Intensity);
            Assert.AreEqual(size.StartTime, newStrength.StartTime);
            Assert.AreEqual(size.EndTime, newStrength.EndTime);
            Assert.AreEqual(ex.GlobalId, newStrength.Entries.ElementAt(0).Exercise.GlobalId);
            Assert.AreEqual(1, newStrength.Entries.ElementAt(0).Position);
            Assert.AreEqual("rom", newStrength.Entries.ElementAt(0).SuperSetGroup);
            Assert.AreEqual("com", newStrength.Entries.ElementAt(0).Comment);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Cardio()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid(),ExerciseType.Cardio);
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            size.AddEntry(item);
            Serie oldSerie = new Serie();
            oldSerie.Comment = "cm1";
            oldSerie.RepetitionNumber = 100;
            item.AddSerie(oldSerie);

            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(1, newStrength.Entries.Count);
            Assert.AreEqual((int)size.Intensity, (int)newStrength.Intensity);
            Assert.AreEqual(size.StartTime, newStrength.StartTime);
            Assert.AreEqual(size.EndTime, newStrength.EndTime);
            Assert.AreEqual(ex.GlobalId, newStrength.Entries.ElementAt(0).Exercise.GlobalId);
            Assert.AreEqual(1, newStrength.Entries.ElementAt(0).Position);
            Assert.AreEqual(null, newStrength.Entries.ElementAt(0).Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual(100, newStrength.Entries.ElementAt(0).Series.ElementAt(0).Weight);
           
        }

        [Test]
        public void ConvertStrengthTrainingEntry_WithNotExistingExercise()
        {
            var profile = CreateProfile("profile");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = Guid.NewGuid();
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var deletedExercise = SessionNew.QueryOver<Model.Exercise>().Where(x=>x.IsDeleted).SingleOrDefault();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(1, newStrength.Entries.Count);
            Assert.AreEqual((int)size.Intensity, (int)newStrength.Intensity);
            Assert.AreEqual(size.StartTime, newStrength.StartTime);
            Assert.AreEqual(size.EndTime, newStrength.EndTime);
            Assert.AreEqual(deletedExercise.GlobalId, newStrength.Entries.ElementAt(0).Exercise.GlobalId);
            Assert.AreEqual(1, newStrength.Entries.ElementAt(0).Position);
            Assert.AreEqual("rom", newStrength.Entries.ElementAt(0).SuperSetGroup);
            Assert.AreEqual("com", newStrength.Entries.ElementAt(0).Comment);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_DuplicatedResults()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(2, newStrength.Entries.Count);
            Assert.AreEqual(2, newStrength.Entries.ElementAt(0).Series.Count);
            Assert.AreEqual(2, newStrength.Entries.ElementAt(1).Series.Count);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Sets()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(1, newStrength.Entries.ElementAt(0).Series.Count);
            Assert.AreEqual((decimal)oldSerie.RepetitionNumber,(decimal) newStrength.Entries.ElementAt(0).Series.ElementAt(0).RepetitionNumber);
            Assert.AreEqual((decimal)oldSerie.Weight, (decimal)newStrength.Entries.ElementAt(0).Series.ElementAt(0).Weight);
            Assert.AreEqual(oldSerie.IsCiezarBezSztangi, newStrength.Entries.ElementAt(0).Series.ElementAt(0).IsCiezarBezSztangi);
            Assert.AreEqual((int)oldSerie.SetType, (int)newStrength.Entries.ElementAt(0).Series.ElementAt(0).SetType);
            Assert.AreEqual((int)oldSerie.DropSet, (int)newStrength.Entries.ElementAt(0).Series.ElementAt(0).DropSet);
            Assert.AreEqual(oldSerie.Comment, newStrength.Entries.ElementAt(0).Series.ElementAt(0).Comment);
            Assert.IsFalse(newStrength.Entries.ElementAt(0).Series.ElementAt(0).IsIncorrect);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Sets_Incorrect()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x43");
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x530");
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x53");
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(4, newStrength.Entries.ElementAt(0).Series.Count);
            Assert.IsFalse(newStrength.Entries.ElementAt(0).Series.ElementAt(0).IsIncorrect);
            Assert.IsFalse(newStrength.Entries.ElementAt(0).Series.ElementAt(1).IsIncorrect);
            Assert.IsTrue(newStrength.Entries.ElementAt(0).Series.ElementAt(2).IsIncorrect);
            Assert.IsFalse(newStrength.Entries.ElementAt(0).Series.ElementAt(3).IsIncorrect);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Sets_Position()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("1x");
            oldSerie.Comment = "cm1";
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            oldSerie = new Serie("2x");
            oldSerie.Comment = "cm1";
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            oldSerie = new Serie("3x");
            oldSerie.Comment = "cm1";
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(0, newStrength.Entries.ElementAt(0).Series.Where(x=>x.RepetitionNumber==1).Single().Position);
            Assert.AreEqual(1, newStrength.Entries.ElementAt(0).Series.Where(x => x.RepetitionNumber == 2).Single().Position);
            Assert.AreEqual(2, newStrength.Entries.ElementAt(0).Series.Where(x => x.RepetitionNumber == 3).Single().Position);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Items_Position()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 2;
            item.Comment = "Com1";
            size.AddEntry(item);
            item = new StrengthTrainingItem();
            item.Comment = "Com2";
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(2, newStrength.Entries.Where(x=>x.Comment=="Com1").Single().Position);
            Assert.AreEqual(1, newStrength.Entries.Where(x => x.Comment == "Com2").Single().Position);
        }

        [Test]
        public void ConvertStrengthTrainingEntry_Items_Position_OldEntries()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 0;
            item.Comment = "Com1";
            size.AddEntry(item);
            item = new StrengthTrainingItem();
            item.Comment = "Com2";
            item.ExerciseId = ex.GlobalId;
            item.Position = 0;
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(0, newStrength.Entries.Where(x => x.Comment == "Com1").Single().Position);
            Assert.AreEqual(1, newStrength.Entries.Where(x => x.Comment == "Com2").Single().Position);
        }


        [Test]
        public void ConvertStrengthTrainingEntry_TrainingPlanId()
        {
            var profile = CreateProfile("profile");
            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.TrainingPlanId = Guid.NewGuid();
            size.TrainingPlanItemId = Guid.NewGuid();
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.TrainingPlanItemId = Guid.NewGuid();
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("12x23");
            oldSerie.Comment = "cm1";
            oldSerie.IsCiezarBezSztangi = true;
            oldSerie.DropSet = DropSetType.IIIDropSet;
            oldSerie.TrainingPlanItemId = Guid.NewGuid();
            oldSerie.SetType = SetType.PrawieMax;
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newStrength = (Model.StrengthTrainingEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual(size.TrainingPlanId, newStrength.TrainingPlanId);
            Assert.AreEqual(size.TrainingPlanItemId, newStrength.TrainingPlanItemId);
            Assert.AreEqual(item.TrainingPlanItemId,newStrength.Entries.ElementAt(0).TrainingPlanItemId);
            Assert.AreEqual(oldSerie.TrainingPlanItemId, newStrength.Entries.ElementAt(0).Series.ElementAt(0).TrainingPlanItemId);
        }

        [Test]
        public void ConvertTrainingDay_Empty()
        {
            var profile = CreateProfile("profile");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            insertToOldDatabase(day);

            Convert();

            var count = SessionNew.QueryOver<Model.TrainingDay>().RowCount();
            Assert.AreEqual(0,count);
        }

        [Test]
        public void ConvertSizeEntry()
        {
            var profile = CreateProfile("profile");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new SizeEntry();
            size.Comment = "test";
            size.ReportStatus = ReportStatus.SkipInReport;
            size.Wymiary = new Wymiary();
            size.Wymiary.Klatka = 44;
            size.Wymiary.DateTime = DateTime.UtcNow.Date;
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Model.SizeEntry newSize = (Model.SizeEntry) newDay.Objects.ElementAt(0);
            Assert.IsNotNull(newSize.Wymiary);
            Assert.AreEqual("test",newSize.Comment);
            Assert.AreEqual((int)ReportStatus.SkipInReport, (int)newSize.ReportStatus);
            Assert.AreEqual(size.Wymiary.Klatka, newSize.Wymiary.Klatka);
            Assert.AreEqual(size.Wymiary.DateTime, newSize.Wymiary.Time.DateTime);
        }

        [Test]
        public void ConvertBlogEntry()
        {
            var profile = CreateProfile("profile");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new BlogEntry();
            size.Comment = "test";
            size.ReportStatus = ReportStatus.SkipInReport;
            size.AllowComments = true;
            size.LastCommentDate = DateTime.UtcNow.Date;
            day.AddEntry(size);
            insertToOldDatabase(day);

            Convert();

            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            var newSize = (Model.BlogEntry)newDay.Objects.ElementAt(0);
            Assert.AreEqual("test", newSize.Comment);
            Assert.AreEqual((int)ReportStatus.SkipInReport, (int)newSize.ReportStatus);
        }

        [Test]
        public void ConvertBlogEntry_BlogComments()
        {
            var profile1 = CreateProfile("profile1");
            var profile2 = CreateProfile("profile2");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile1;
            var size = new BlogEntry();
            size.Comment = "test";
            size.ReportStatus = ReportStatus.SkipInReport;
            size.AllowComments = true;
            size.LastCommentDate = DateTime.UtcNow.Date;
            day.AddEntry(size);
            insertToOldDatabase(day);
            BlogComment comment1 = new BlogComment();
            comment1.BlogEntry = size;
            comment1.Comment = "Test1";
            comment1.DateTime = DateTime.UtcNow.Date;
            comment1.Profile = profile1;
            insertToOldDatabase(comment1);
            BlogComment comment2 = new BlogComment();
            comment2.BlogEntry = size;
            comment2.Comment = "Test2";
            comment2.DateTime = DateTime.UtcNow.Date.AddDays(-1);
            comment2.Profile = profile2;
            insertToOldDatabase(comment2);

            Convert();

            var result = SessionNew.QueryOver<Model.TrainingDay>().List();
            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Assert.AreEqual(2,newDay.Comments.Count);
            Assert.AreEqual(size.LastCommentDate, newDay.LastCommentDate);
            Assert.AreEqual(2, newDay.CommentsCount);
            
            var newComment1 = newDay.Comments.Where(x => x.Profile.UserName == profile1.UserName).Single();
            Assert.AreEqual(comment1.Comment, newComment1.Comment);
            Assert.AreEqual(comment1.DateTime, newComment1.DateTime);

            var newComment2 = newDay.Comments.Where(x => x.Profile.UserName == profile2.UserName).Single();
            Assert.AreEqual(comment2.Comment, newComment2.Comment);
            Assert.AreEqual(comment2.DateTime, newComment2.DateTime);
        }

        [Test]
        public void ConvertBlogEntry_BlogComments_Bug()
        {
            var profile1 = CreateProfile("profile1");
            var profile2 = CreateProfile("profile2");
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day.Comment = "cmt";
            day.Profile = profile1;

            SizeEntry size2 = new SizeEntry();
            size2.Wymiary = new Wymiary();
            size2.Wymiary.Height = 100;
            day.AddEntry(size2);

            var size = new BlogEntry();
            size.Comment = "test";
            size.ReportStatus = ReportStatus.SkipInReport;
            size.AllowComments = true;
            size.LastCommentDate = DateTime.UtcNow.Date;
            day.AddEntry(size);

            SizeEntry size1 = new SizeEntry();
            size1.Wymiary = new Wymiary();
            size1.Wymiary.Height = 100;
            day.AddEntry(size1);

            insertToOldDatabase(day);
            BlogComment comment1 = new BlogComment();
            comment1.BlogEntry = size;
            comment1.Comment = "Test1";
            comment1.DateTime = DateTime.UtcNow.Date;
            comment1.Profile = profile1;
            insertToOldDatabase(comment1);
            BlogComment comment2 = new BlogComment();
            comment2.BlogEntry = size;
            comment2.Comment = "Test2";
            comment2.DateTime = DateTime.UtcNow.Date.AddDays(-1);
            comment2.Profile = profile2;
            insertToOldDatabase(comment2);

            Convert();

            var result = SessionNew.QueryOver<Model.TrainingDay>().List();
            var newDay = SessionNew.QueryOver<Model.TrainingDay>().SingleOrDefault();
            Assert.AreEqual(2, newDay.Comments.Count);
            Assert.AreEqual(size.LastCommentDate, newDay.LastCommentDate);
            Assert.AreEqual(2, newDay.CommentsCount);
        }

        [Test]
        public void ConvertA6WTraining_Completed()
        {
            var profile = CreateProfile("profile");
            MyTraining endedTraining=new MyTraining();
            endedTraining.EndDate = DateTime.UtcNow.AddDays(-1).Date;
            endedTraining.StartDate = DateTime.UtcNow.AddDays(-3).Date;
            endedTraining.TrainingEnd = TrainingEnd.Break;
            endedTraining.Name = "TempName";
            endedTraining.PercentageCompleted = null;
            endedTraining.Profile = profile;
            insertToOldDatabase(endedTraining);

            var day1 = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day1.Comment = "cmt";
            day1.Profile = profile;
            var a6w1 = new A6WEntry();
            a6w1.Comment = "test";
            a6w1.ReportStatus = ReportStatus.SkipInReport;
            a6w1.Completed = true;
            a6w1.MyTraining = endedTraining;
            a6w1.DayNumber = 1;
            day1.AddEntry(a6w1);
            insertToOldDatabase(day1);

            var day2 = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day2.Comment = "cmt";
            day2.Profile = profile;
            var a6w2 = new A6WEntry();
            a6w2.Comment = "test";
            a6w2.MyTraining = endedTraining;
            a6w2.ReportStatus = ReportStatus.SkipInReport;
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            insertToOldDatabase(day2);

            Convert();

            var newDays= SessionNew.QueryOver<Model.TrainingDay>().List();
            var a6WTraining = SessionNew.QueryOver<Model.A6WTraining>().SingleOrDefault();
            var newA6w1 = (Model.A6WEntry)newDays[0].Objects.ElementAt(0);
            var newA6w2 = (Model.A6WEntry)newDays[1].Objects.ElementAt(0);

            Assert.AreEqual(null,a6WTraining.Customer);
            Assert.AreEqual(endedTraining.EndDate, a6WTraining.EndDate);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(endedTraining.Name, a6WTraining.Name);
            Assert.AreEqual(4, a6WTraining.PercentageCompleted);
            Assert.AreEqual(newDays[0].Profile, a6WTraining.Profile);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(Model.TrainingEnd.Complete, a6WTraining.TrainingEnd);
            Assert.AreEqual(2, a6WTraining.EntryObjects.Count);

            Assert.AreEqual(a6w1.Comment, newA6w1.Comment);
            Assert.AreEqual((int)a6w1.ReportStatus, (int)newA6w1.ReportStatus);
            Assert.AreEqual(a6w1.Completed, newA6w1.Completed);
            Assert.AreEqual(a6w1.DayNumber, newA6w1.DayNumber);
            Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w1.Status);
            Assert.AreEqual(a6w1.Set1, newA6w1.Set1);
            Assert.AreEqual(a6w1.Set2, newA6w1.Set2);
            Assert.AreEqual(a6w1.Set3, newA6w1.Set3);
            Assert.AreEqual(a6w1.Set3, newA6w1.Set3);
            Assert.AreEqual(a6WTraining, newA6w1.MyTraining);

            Assert.AreEqual(a6w2.Comment, newA6w2.Comment);
            Assert.AreEqual((int)a6w2.ReportStatus, (int)newA6w2.ReportStatus);
            Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w2.Status);
            Assert.AreEqual(a6w2.Completed, newA6w2.Completed);
            Assert.AreEqual(a6w2.DayNumber, newA6w2.DayNumber);
            Assert.AreEqual(a6w2.Set1, newA6w2.Set1);
            Assert.AreEqual(a6w2.Set2, newA6w2.Set2);
            Assert.AreEqual(a6w2.Set3, newA6w2.Set3);
            Assert.AreEqual(a6w2.Set3, newA6w2.Set3);
            Assert.AreEqual(a6WTraining, newA6w2.MyTraining);
        }

        [Test]
        public void ConvertA6WTraining_NotEnded()
        {
            var profile = CreateProfile("profile");
            MyTraining endedTraining = new MyTraining();
            endedTraining.StartDate = DateTime.UtcNow.AddDays(-3).Date;
            endedTraining.TrainingEnd = TrainingEnd.NotEnded;
            endedTraining.Name = "TempName";
            endedTraining.PercentageCompleted = null;
            endedTraining.Profile = profile;
            insertToOldDatabase(endedTraining);

            var day1 = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day1.Comment = "cmt";
            day1.Profile = profile;
            var a6w1 = new A6WEntry();
            a6w1.Comment = "test";
            a6w1.ReportStatus = ReportStatus.SkipInReport;
            a6w1.Completed = true;
            a6w1.MyTraining = endedTraining;
            a6w1.DayNumber = 1;
            day1.AddEntry(a6w1);
            endedTraining.EntryObjects.Add(a6w1);
            insertToOldDatabase(day1);

            var day2 = new TrainingDay(DateTime.UtcNow.AddDays(-9));
            day2.Comment = "cmt";
            day2.Profile = profile;
            var a6w2 = new A6WEntry();
            a6w2.Comment = "test";
            a6w2.MyTraining = endedTraining;
            a6w2.ReportStatus = ReportStatus.SkipInReport;
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            endedTraining.EntryObjects.Add(a6w2);
            insertToOldDatabase(day2);
            insertToOldDatabase(endedTraining);

            Convert();

            SessionNew.Clear();
            var newDays = SessionNew.QueryOver<Model.TrainingDay>().List();
            var a6WTraining = SessionNew.QueryOver<Model.A6WTraining>().SingleOrDefault();

            Assert.AreEqual(null, a6WTraining.Customer);
            Assert.AreEqual(null, a6WTraining.EndDate);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(endedTraining.Name, a6WTraining.Name);
            Assert.AreEqual(4, a6WTraining.PercentageCompleted);
            Assert.AreEqual(newDays[0].Profile, a6WTraining.Profile);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(Model.TrainingEnd.NotEnded, a6WTraining.TrainingEnd);
            Assert.AreEqual(42, a6WTraining.EntryObjects.Count);
            Assert.AreEqual(42, newDays.Count);

            var entries = a6WTraining.EntryObjects.Cast<Model.A6WEntry>().OrderBy(x => x.DayNumber);
            for (int i = 0; i < 42; i++)
            {
                var newA6w = entries.ElementAt(i);
                if (i > 1)
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Planned, newA6w.Status);
                    Assert.AreEqual(false, newA6w.Completed);
                }
                else
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w.Status);
                }
                Assert.AreEqual(i+1, newA6w.DayNumber);
                Assert.AreEqual(a6WTraining, newA6w.MyTraining);
            }
        }

        [Test]
        public void ConvertA6WTraining_Bug1()
        {
            var profile = CreateProfile("profile");
            MyTraining endedTraining = new MyTraining();
            endedTraining.StartDate = DateTime.UtcNow.AddDays(-3).Date;
            endedTraining.TrainingEnd = TrainingEnd.NotEnded;
            endedTraining.Name = "TempName";
            endedTraining.PercentageCompleted = null;
            endedTraining.Profile = profile;
            insertToOldDatabase(endedTraining);

            var day1 = new TrainingDay(DateTime.UtcNow.AddDays(-10));
            day1.Comment = "cmt";
            day1.Profile = profile;
            var a6w1 = new A6WEntry();
            a6w1.Comment = "test";
            a6w1.ReportStatus = ReportStatus.SkipInReport;
            a6w1.Completed = true;
            a6w1.MyTraining = endedTraining;
            a6w1.DayNumber = 1;
            day1.AddEntry(a6w1);
            endedTraining.EntryObjects.Add(a6w1);
            insertToOldDatabase(day1);

            var day2 = new TrainingDay(DateTime.UtcNow.AddDays(-9));
            day2.Comment = "cmt";
            day2.Profile = profile;
            var a6w2 = new A6WEntry();
            a6w2.Comment = "test";
            a6w2.MyTraining = endedTraining;
            a6w2.ReportStatus = ReportStatus.SkipInReport;
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            endedTraining.EntryObjects.Add(a6w2);
            insertToOldDatabase(day2);
            insertToOldDatabase(endedTraining);

            var ex = CreateExercise("test", Guid.NewGuid());
            Model.Old.TrainingDay day = new TrainingDay(DateTime.UtcNow.AddDays(-8));
            day.Comment = "cmt";
            day.Profile = profile;
            var size = new StrengthTrainingEntry();
            size.StartTime = DateTime.UtcNow.AddHours(11).Date;
            size.EndTime = DateTime.UtcNow.AddDays(11).Date;
            size.Intensity = Intensity.Medium;
            var item = new StrengthTrainingItem();
            item.ExerciseId = ex.GlobalId;
            item.Position = 1;
            item.Comment = "com";
            item.SuperSetGroup = "rom";
            Serie oldSerie = new Serie("12x23");
            item.AddSerie(oldSerie);
            oldSerie = new Serie("12x23");
            item.AddSerie(oldSerie);
            size.AddEntry(item);
            day.AddEntry(size);
            insertToOldDatabase(day);


            Convert();

            SessionNew.Clear();
            var newDays = SessionNew.QueryOver<Model.TrainingDay>().List();
            var a6WTraining = SessionNew.QueryOver<Model.A6WTraining>().SingleOrDefault();

            Assert.AreEqual(null, a6WTraining.Customer);
            Assert.AreEqual(null, a6WTraining.EndDate);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(endedTraining.Name, a6WTraining.Name);
            Assert.AreEqual(4, a6WTraining.PercentageCompleted);
            Assert.AreEqual(newDays[0].Profile, a6WTraining.Profile);
            Assert.AreEqual(endedTraining.StartDate, a6WTraining.StartDate);
            Assert.AreEqual(Model.TrainingEnd.NotEnded, a6WTraining.TrainingEnd);
           // Assert.AreEqual(42, a6WTraining.EntryObjects.Count);
           // Assert.AreEqual(42, newDays.Count);

            
            var entries = a6WTraining.EntryObjects.Cast<Model.A6WEntry>().OrderBy(x => x.DayNumber);
            Dictionary<DateTime,object> test = new Dictionary<DateTime, object>();
            foreach (var entry in entries)
            {
                Console.WriteLine(entry.TrainingDay.TrainingDate.ToShortDateString());
                test.Add(entry.TrainingDay.TrainingDate,entry);
            }
            var testDateUniquness=entries.ToDictionary(x => x.TrainingDay.TrainingDate);
            for (int i = 0; i < 42; i++)
            {
                var newA6w = entries.ElementAt(i);
                if (i > 1)
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Planned, newA6w.Status);
                    Assert.AreEqual(false, newA6w.Completed);
                }
                else
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w.Status);
                }
                Assert.AreEqual(i + 1, newA6w.DayNumber);
                Assert.AreEqual(a6WTraining, newA6w.MyTraining);
            }
        }

        [Test]
        public void ConvertA6WTraining_NotEnded_TrainingDayExists()
        {
            var profile = CreateProfile("profile");
            MyTraining endedTraining = new MyTraining();
            endedTraining.StartDate = DateTime.UtcNow.AddDays(-3).Date;
            endedTraining.TrainingEnd = TrainingEnd.NotEnded;
            endedTraining.Name = "TempName";
            endedTraining.PercentageCompleted = null;
            endedTraining.Profile = profile;
            insertToOldDatabase(endedTraining);

            var day1 = new TrainingDay(DateTime.UtcNow.AddDays(-10).Date);
            day1.Comment = "cmt";
            day1.Profile = profile;
            var a6w1 = new A6WEntry();
            a6w1.Comment = "test";
            a6w1.ReportStatus = ReportStatus.SkipInReport;
            a6w1.Completed = true;
            a6w1.MyTraining = endedTraining;
            a6w1.DayNumber = 1;
            day1.AddEntry(a6w1);
            endedTraining.EntryObjects.Add(a6w1);
            insertToOldDatabase(day1);

            var day2 = new TrainingDay(DateTime.UtcNow.AddDays(-9).Date);
            day2.Comment = "cmt";
            day2.Profile = profile;
            var a6w2 = new A6WEntry();
            a6w2.Comment = "test";
            a6w2.MyTraining = endedTraining;
            a6w2.ReportStatus = ReportStatus.SkipInReport;
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            endedTraining.EntryObjects.Add(a6w2);
            insertToOldDatabase(day2);
            insertToOldDatabase(endedTraining);

            var day3 = new TrainingDay(DateTime.UtcNow.AddDays(-8).Date);
            day3.Comment = "cmt";
            day3.Profile = profile;
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary=new Wymiary();
            sizeEntry.Wymiary.Klatka = 22;
            day3.AddEntry(sizeEntry);
            insertToOldDatabase(day3);

            Convert();

            SessionNew.Clear();

            var newDays = SessionNew.QueryOver<Model.TrainingDay>().List();
            var a6WTraining = SessionNew.QueryOver<Model.A6WTraining>().SingleOrDefault();

            Assert.AreEqual(42, a6WTraining.EntryObjects.Count);
            Assert.AreEqual(42, newDays.Count);
            var newDay3=newDays.Where(x => x.TrainingDate == DateTime.UtcNow.AddDays(-8).Date).Single();
            Assert.AreEqual(2, newDay3.Objects.Count);

            var entries = a6WTraining.EntryObjects.Cast<Model.A6WEntry>().OrderBy(x => x.DayNumber);
            for (int i = 0; i < 42; i++)
            {
                var newA6w = entries.ElementAt(i);
                if (i > 1)
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Planned, newA6w.Status);
                    Assert.AreEqual(false, newA6w.Completed);
                }
                else
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w.Status);
                }
                Assert.AreEqual(i + 1, newA6w.DayNumber);
                Assert.AreEqual(a6WTraining, newA6w.MyTraining);
            }
        }

        [Test]
        public void ConvertA6WTraining_NotEnded_TrainingDayExists_CrossProfileProblem()
        {
            var profile = CreateProfile("profile");
            var profile1 = CreateProfile("profile1");
            MyTraining endedTraining = new MyTraining();
            endedTraining.StartDate = DateTime.UtcNow.AddDays(-3).Date;
            endedTraining.TrainingEnd = TrainingEnd.NotEnded;
            endedTraining.Name = "TempName";
            endedTraining.PercentageCompleted = null;
            endedTraining.Profile = profile;
            insertToOldDatabase(endedTraining);

            var day1 = new TrainingDay(DateTime.UtcNow.AddDays(-10).Date);
            day1.Comment = "cmt";
            day1.Profile = profile;
            var a6w1 = new A6WEntry();
            a6w1.Comment = "test";
            a6w1.ReportStatus = ReportStatus.SkipInReport;
            a6w1.Completed = true;
            a6w1.MyTraining = endedTraining;
            a6w1.DayNumber = 1;
            day1.AddEntry(a6w1);
            endedTraining.EntryObjects.Add(a6w1);
            insertToOldDatabase(day1);

            var day2 = new TrainingDay(DateTime.UtcNow.AddDays(-9).Date);
            day2.Comment = "cmt";
            day2.Profile = profile;
            var a6w2 = new A6WEntry();
            a6w2.Comment = "test";
            a6w2.MyTraining = endedTraining;
            a6w2.ReportStatus = ReportStatus.SkipInReport;
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            endedTraining.EntryObjects.Add(a6w2);
            insertToOldDatabase(day2);
            insertToOldDatabase(endedTraining);

            var day3 = new TrainingDay(DateTime.UtcNow.AddDays(-8).Date);
            day3.Comment = "cmt";
            day3.Profile = profile1;
            var sizeEntry = new SizeEntry();
            sizeEntry.Wymiary = new Wymiary();
            sizeEntry.Wymiary.Klatka = 22;
            day3.AddEntry(sizeEntry);
            insertToOldDatabase(day3);

            Convert();

            SessionNew.Clear();
            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile").SingleOrDefault();
            var all = SessionNew.QueryOver<Model.TrainingDay>().List();
            var forProfile1 = all.Where(x => x.Profile.UserName == "profile").ToList();
            foreach (var trainingDay in all)
            {
                Console.WriteLine(trainingDay.Profile.UserName +":"+trainingDay.TrainingDate);
            }
            if (all.Count<43)
            {
                
                Assert.Fail("Test!");
            }
            for (int i = 1; i <= 42; i++)
            {
                var test=forProfile1.Where(x1 => x1.Objects.Cast<Model.A6WEntry>().Where(x => x.DayNumber == i).Count() == 1).
                    SingleOrDefault();
                if(test==null)
                {
                    Assert.Fail(i.ToString());
                }
            }
            var forProfile2 = all.Where(x => x.Profile.UserName == "profile1").ToList();
            var count = SessionNew.QueryOver<Model.TrainingDay>().RowCount();
            Assert.AreEqual(43,count);

            var newDays = SessionNew.QueryOver<Model.TrainingDay>().Where(x => x.Profile == dbProfile).List();
            var a6WTraining = SessionNew.QueryOver<Model.A6WTraining>().SingleOrDefault();

            Assert.AreEqual(42, a6WTraining.EntryObjects.Count);
            Assert.AreEqual(42, newDays.Count);
            var newDay3 = newDays.Where(x => x.TrainingDate == DateTime.UtcNow.AddDays(-8).Date).Single();
            Assert.AreEqual(1, newDay3.Objects.Count);

            var entries = a6WTraining.EntryObjects.Cast<Model.A6WEntry>().OrderBy(x => x.DayNumber);
            for (int i = 0; i < 42; i++)
            {
                var newA6w = entries.ElementAt(i);
                if (i > 1)
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Planned, newA6w.Status);
                    Assert.AreEqual(false, newA6w.Completed);
                }
                else
                {
                    Assert.AreEqual(Model.EntryObjectStatus.Done, newA6w.Status);
                }
                Assert.AreEqual(i + 1, newA6w.DayNumber);
                Assert.AreEqual(a6WTraining.GlobalId, newA6w.MyTraining.GlobalId);
            }
        }
    }
}
