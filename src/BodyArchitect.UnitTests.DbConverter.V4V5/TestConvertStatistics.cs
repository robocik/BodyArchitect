using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model.Old;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestConvertStatistics : NHibernateTestFixtureBase
    {
        [Test]
        public void A6WEntries_TrainingNotFullyFinish()
        {
            var profile = CreateProfile("profile");
            MyTraining endedTraining = new MyTraining();
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
            a6w2.Completed = false;
            a6w2.DayNumber = 2;
            a6w2.Set1 = 2;
            a6w2.Set2 = 3;
            day2.AddEntry(a6w2);
            insertToOldDatabase(day2);

            Convert();

            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x=>x.UserName==profile.UserName).SingleOrDefault();
            var count = SessionNew.QueryOver<Model.A6WEntry>().RowCount();
            Assert.AreEqual(count,dbProfile.Statistics.A6WEntriesCount);
        }

        [Test]
        public void ConvertBlogCommentsCount()
        {
            var profile = CreateProfile("profile");
            profile.Statistics.BlogCommentsCount = 4;
            insertToOldDatabase(profile.Statistics);

            Convert();

            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == profile.UserName).SingleOrDefault();
            Assert.AreEqual(4,dbProfile.Statistics.TrainingDayCommentsCount);
        }

        [Test]
        public void ConvertMyBlogCommentsCount()
        {
            var profile = CreateProfile("profile");
            profile.Statistics.MyBlogCommentsCount = 4;
            insertToOldDatabase(profile.Statistics);

            Convert();

            var dbProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == profile.UserName).SingleOrDefault();
            Assert.AreEqual(4, dbProfile.Statistics.MyTrainingDayCommentsCount);
        }
    }
}
