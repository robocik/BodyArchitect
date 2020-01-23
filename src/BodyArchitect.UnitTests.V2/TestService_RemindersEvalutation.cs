//using System;
//using System.Collections.Generic;
//using BodyArchitect.Model;
//using BodyArchitect.Service.V2;
//using BodyArchitect.Service.V2.Model;
//using BodyArchitect.Service.V2.Services;
//using NUnit.Framework;

//namespace BodyArchitect.UnitTests.V2
//{
//    [TestFixture]
//    public class TestService_RemindersEvalutation:TestServiceBase
//    {
//        List<Profile> profiles = new List<Profile>();

//        public override void BuildDatabase()
//        {
//            using (var tx = Session.BeginTransaction())
//            {
//                profiles.Clear();
//                profiles.Add(CreateProfile(Session, "test1"));
//                profiles.Add(CreateProfile(Session, "test2"));

                

//                tx.Commit();
//            }
//        }

//        [Test]
//        public void AtStartTime()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            var reminder1 = CreateReminder("rem1", profiles[0], baseDate.AddDays(10), null);
//            var reminder2 = CreateReminder("rem2", profiles[0], baseDate.AddDays(5), null);
//            var reminder3 = CreateReminder("rem3", profiles[0], baseDate.AddDays(1), null);
//            var reminder4 = CreateReminder("rem4", profiles[0], baseDate.AddHours(1), null);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x,u) =>
//                    {
//                        TimerService.UtcNow = baseDate.AddHours(1);
//                        SecurityInfo info = new SecurityInfo(data,new LoginData());
//                        var service = new ReminderService(x, info, u);
//                        var reminders=service.GetCurrentReminders();
//                        Assert.AreEqual(1,reminders.Count);
//                        Assert.AreEqual(reminder4.GlobalId, reminders[0].GlobalId);
//                    });
//        }

//        [Test]
//        public void AtStartTime_PastReminderSkipped()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            var reminder1 = CreateReminder("rem1", profiles[0], baseDate.AddDays(10),null);
//            var reminder2 = CreateReminder("rem2", profiles[0], baseDate.AddDays(5), null);
//            var reminder3 = CreateReminder("rem3", profiles[0], baseDate.AddDays(1), null);
//            var reminder4 = CreateReminder("rem4", profiles[0], baseDate.AddHours(1), null);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddDays(1);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//                Assert.AreEqual(reminder3.GlobalId, reminders[0].GlobalId);
//            });
//        }

//        [Test]
//        public void RemindBefore_Equal15min_Limit()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            var reminder1 = CreateReminder("rem3", profiles[0], baseDate.AddDays(1),null);
//            var reminder2 = CreateReminder("rem4", profiles[0], baseDate.AddHours(1),TimeSpan.FromMinutes(15));

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMinutes(45);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//                Assert.AreEqual(reminder2.GlobalId, reminders[0].GlobalId);
//            });
//        }

//        [Test]
//        public void RemindBefore_Less15min_Middle()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            var reminder1 = CreateReminder("rem3", profiles[0], baseDate.AddDays(1),null);
//            var reminder2 = CreateReminder("rem4", profiles[0], baseDate.AddHours(1), TimeSpan.FromMinutes(15));

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMinutes(52);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//                Assert.AreEqual(reminder2.GlobalId, reminders[0].GlobalId);
//            });
//        }

//        [Test]
//        public void RemindBefore_AtStartTime()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            var reminder1 = CreateReminder("rem3", profiles[0], baseDate.AddDays(1),null);
//            var reminder2 = CreateReminder("rem4", profiles[0], baseDate.AddHours(1), TimeSpan.FromMinutes(15));

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddHours(1);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//                Assert.AreEqual(reminder2.GlobalId, reminders[0].GlobalId);
//            });
//        }

//        [Test]
//        public void RemindBefore_AtStartTime_TwoResults()
//        {
//            var baseDate = DateTime.UtcNow.Date;
//            CreateReminder("rem3", profiles[0], baseDate.AddHours(1), TimeSpan.FromMinutes(30));
//            CreateReminder("rem4", profiles[0], baseDate.AddMinutes(30),null);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMinutes(30);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(2, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_MondayOnly_DateTimeNull()
//        {

//            var reminder1 = CreateReminder("rem1", profiles[0], null, null, DayOfWeek.Monday);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//                    {
//                        TimerService.UtcNow = new DateTime(2012, 03, 21);//wednestday;
//                        SecurityInfo info = new SecurityInfo(data, new LoginData());
//                        var service = new ReminderService(x, info, u);
//                        var reminders = service.GetCurrentReminders();
//                        Assert.AreEqual(0, reminders.Count);
//                    });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26);//monday;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26,4,12,33);//monday, middle of day;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_MondayOnly_DateTimeNull_ForManyWeeks()
//        {

//            CreateReminder("rem1", profiles[0], null, null, DayOfWeek.Monday);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26);//monday;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 4, 2);//monday next week;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 4, 9);//monday next next week;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_MondayAndFriday_DateTimeNull_ForManyWeeks()
//        {

//            CreateReminder("rem1", profiles[0], null, null, DayOfWeek.Monday, DayOfWeek.Friday);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26);//monday;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 3, 30);//friday
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 4, 2);//monday next week;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 4, 6);//friday next week;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_MondayOnly_DateTimeSet_TimeImportantOnly_RemindAtStartTimeOnly()
//        {

//            CreateReminder("rem1", profiles[0], DateTime.UtcNow.Date.AddHours(4), null, DayOfWeek.Monday);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//                    {
//                        var date = new DateTime(2012, 03, 21);
//                        date = date.AddHours(4);
//                        TimerService.UtcNow = date;//wednestday - exactly the same time;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26);//monday but time is wrong;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                var date = new DateTime(2012, 03, 26);//monday - time is good
//                date = date.AddHours(4);
//                TimerService.UtcNow = date;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_MondayOnly_DateTimeSet_TimeImportantOnly_RemindAtStartDateInTheFuture()
//        {

//            CreateReminder("rem1", profiles[0], DateTime.UtcNow.Date.AddDays(7).AddHours(4), null, DayOfWeek.Monday);

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                var date = new DateTime(2012, 03, 21);
//                date = date.AddHours(4);
//                TimerService.UtcNow = date;//wednestday - exactly the same time;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = new DateTime(2012, 03, 26);//monday but time is wrong;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                var date = new DateTime(2012, 03, 26);//monday - time is good but date in before Date in reminder (starting date)
//                date = date.AddHours(4);
//                TimerService.UtcNow = date;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                var date = new DateTime(2012, 4, 2);//monday - time is good and date good
//                date = date.AddHours(4);
//                TimerService.UtcNow = date;
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//        }

//        [Test]
//        public void RemindPattern_EveryMonth()
//        {
//            var baseDate = new DateTime(2012,2,10,12,10,0);
//            CreateReminder("rem1", profiles[0], baseDate, null, "M:");

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate;//specified date
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//                    {
//                        TimerService.UtcNow = baseDate.AddDays(7);//next week
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMonths(1);
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMonths(1).AddYears(1);//next year and next month
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }

//        [Test]
//        public void RemindPattern_EveryYear()
//        {
//            var baseDate = new DateTime(2012, 2, 10, 12, 10, 0);
//            CreateReminder("rem1", profiles[0], baseDate, null, "Y:");

//            var profile = (ProfileDTO)profiles[0].Tag;
//            SessionData data = CreateNewSession(profile, ClientInformation);

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate;//specified date
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddMonths(1);//next month
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(0, reminders.Count);
//            });

//            Run((x, u) =>
//                    {
//                        TimerService.UtcNow = baseDate.AddYears(2);//after two years
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });

//            Run((x, u) =>
//            {
//                TimerService.UtcNow = baseDate.AddYears(3);//after 3 years
//                SecurityInfo info = new SecurityInfo(data, new LoginData());
//                var service = new ReminderService(x, info, u);
//                var reminders = service.GetCurrentReminders();
//                Assert.AreEqual(1, reminders.Count);
//            });
//        }
//    }
//}