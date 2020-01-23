using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Module.Instructor;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using Quartz;
using Quartz.Impl.Triggers;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestReminderBuilder
    {
        private MockTimerService timer;
        private ReminderBuilder builder;

        [SetUp]
        public void setup()
        {
            timer = new MockTimerService();
            builder = new ReminderBuilder(timer);
        }

        private ReminderItemDTO CreateReminder(DateTime dateTime, ReminderRepetitions pattern, DateTime? lastShown = null,TimeSpan? remindBefore=null)
        {
            ReminderItemDTO item = new ReminderItemDTO();
            item.DateTime = dateTime;
            item.Repetitions = pattern;
            item.LastShown = lastShown;
            item.RemindBefore = remindBefore;
            return item;
        }

        public enum TrainingType
        {
            Strength,
            Cardio,
            Both
        }
  
        #region EveryYear
        [Test]
        public void BirthdayReminder_BirthdayDay_EveryYear_AllDay()
        {
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryYear);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl) scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.FireOnceNow, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Year, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void BirthdayReminder_BirthdayDay_EveryYear_AllDay_SecondTimeAfterClose()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryYear, timer.UtcNow);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Year, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void BirthdayReminder_EveryYear_AllDay_NotBirthdayDay()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryYear);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Year, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryYear_SpecifiedTime()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow, ReminderRepetitions.EveryYear);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(calendarTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Year, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }
        #endregion

        #region Every month

        [Test]
        public void EveryMonth_ThisDay_AllDay()
        {
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryMonth);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.FireOnceNow, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Month, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryMonth_ThisDay_AllDay_SecondTimeAfterClose()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryMonth, timer.UtcNow);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Month, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryMonth_AllDay_NotThisDay()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryMonth);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Month, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryMonth_SpecifiedTime()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow, ReminderRepetitions.EveryMonth);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(calendarTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Month, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }
        #endregion

        #region Every week

        [Test]
        public void EveryWeek_ThisDay_AllDay()
        {
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryWeek);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.FireOnceNow, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Week, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryWeek_ThisDay_AllDay_SecondTimeAfterClose()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryWeek, timer.UtcNow);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Week, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryWeek_AllDay_NotThisDay()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.EveryWeek);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.AreEqual(item.DateTime.AddDays(1).AddSeconds(-1), calendarTrigger.EndTimeUtc.Value.DateTime);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Week, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }

        [Test]
        public void EveryWeek_SpecifiedTime()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow, ReminderRepetitions.EveryWeek);
            var scheduleJob = builder.Import(item);
            var calendarTrigger = (CalendarIntervalTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, calendarTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(calendarTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.CalendarIntervalTrigger.DoNothing, calendarTrigger.MisfireInstruction);
            Assert.AreEqual(IntervalUnit.Week, calendarTrigger.RepeatIntervalUnit);
            Assert.AreEqual(1, calendarTrigger.RepeatInterval);
        }
        #endregion

        #region Only once

        [Test]
        public void OldReminder()
        {
            var item = CreateReminder(DateTime.UtcNow.Date.AddDays(-2), ReminderRepetitions.Once);
            var scheduleJob = builder.Import(item);
            Assert.AreEqual(0,scheduleJob.Count);
        }
        [Test]
        public void OnlyOnce_ThisDay_AllDay()
        {
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.Once);
            var scheduleJob = builder.Import(item);
            var simpleTrigger = (SimpleTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, simpleTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(simpleTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.SimpleTrigger.FireNow, simpleTrigger.MisfireInstruction);
            Assert.AreEqual(TimeSpan.Zero, simpleTrigger.RepeatInterval);
            Assert.AreEqual(0, simpleTrigger.RepeatCount);
        }

        [Test]
        public void OnlyOnce_ThisDay_AllDay_SecondTimeAfterClose()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.Once, timer.UtcNow);
            var scheduleJob = builder.Import(item);
            var simpleTrigger = (SimpleTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, simpleTrigger.StartTimeUtc.DateTime);
            Assert.IsNull( simpleTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.SimpleTrigger.FireNow, simpleTrigger.MisfireInstruction);
            Assert.AreEqual(TimeSpan.Zero, simpleTrigger.RepeatInterval);
            Assert.AreEqual(0, simpleTrigger.RepeatCount);
        }

        [Test]
        public void OnlyOnce_AllDay_NotThisDay_After()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow.Date, ReminderRepetitions.Once);
            var scheduleJob = builder.Import(item);
            var simpleTrigger = (SimpleTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, simpleTrigger.StartTimeUtc.DateTime);
            Assert.IsNull( simpleTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.SimpleTrigger.FireNow, simpleTrigger.MisfireInstruction);
            Assert.AreEqual(TimeSpan.Zero, simpleTrigger.RepeatInterval);
            Assert.AreEqual(0, simpleTrigger.RepeatCount);
        }

        [Test]
        public void OnlyOnce_SpecifiedTime()
        {
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(DateTime.UtcNow, ReminderRepetitions.Once);
            var scheduleJob = builder.Import(item);
            var simpleTrigger = (SimpleTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(item.DateTime, simpleTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(simpleTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.SimpleTrigger.FireNow, simpleTrigger.MisfireInstruction);
            Assert.AreEqual(TimeSpan.Zero, simpleTrigger.RepeatInterval);
            Assert.AreEqual(0, simpleTrigger.RepeatCount);
        }

        [Test]
        public void OnlyOnce_SpecifiedTime_RemindBefore15min()
        {
            DateTime baseTime = DateTime.UtcNow;
            timer.UtcNow = DateTime.UtcNow.Date.AddDays(6).AddHours(1);
            var item = CreateReminder(baseTime, ReminderRepetitions.Once, null, TimeSpan.FromMinutes(15));
            var scheduleJob = builder.Import(item);
            var simpleTrigger = (SimpleTriggerImpl)scheduleJob[0].Item2;
            Assert.AreEqual(baseTime.AddMinutes(-15), simpleTrigger.StartTimeUtc.DateTime);
            Assert.IsNull(simpleTrigger.EndTimeUtc);
            Assert.AreEqual(MisfireInstruction.SimpleTrigger.FireNow, simpleTrigger.MisfireInstruction);
            Assert.AreEqual(TimeSpan.Zero, simpleTrigger.RepeatInterval);
            Assert.AreEqual(0, simpleTrigger.RepeatCount);
        }
        #endregion
    }
}