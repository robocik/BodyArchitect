using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_Activities : TestServiceBase
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
        public void GetActivities()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            Activity activity1 = CreateActivity("test1", profiles[0]);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var list =Service.GetActivities(data.Token,new PartialRetrievingInfo());

                Assert.AreEqual(2, list.AllItemsCount);
                Assert.AreEqual(2, list.Items.Count);
                Assert.AreEqual(profile.GlobalId, list.Items[0].ProfileId);
                Assert.AreEqual(profile.GlobalId, list.Items[1].ProfileId);
            });
        }

        [Test]
        public void GetActivities_RetrieveOnlyForLoggedUser()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            activity = CreateActivity("test1", profiles[0]);
            activity = CreateActivity("test", profiles[1]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var list = Service.GetActivities(data.Token, new PartialRetrievingInfo());

                Assert.AreEqual(2, list.AllItemsCount);
                Assert.AreEqual(2, list.Items.Count);
                Assert.AreEqual(profile.GlobalId, list.Items[0].ProfileId);
                Assert.AreEqual(profile.GlobalId, list.Items[1].ProfileId);
            });
        }

        [Test]
        public void DeleteActivity()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteActivity(data.Token, activity.Map<ActivityDTO>());
            });
            var count = Session.QueryOver<Activity>().RowCount();
            Assert.AreEqual(0, count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteActivity_OtherUser()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var dto = activity.Map<ActivityDTO>();
                Service.DeleteActivity(data.Token, dto);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteActivity_SecurityBug()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     var dto = activity.Map<ActivityDTO>();
                                     dto.ProfileId = profile.GlobalId;
                                     //we fake profile id in dto
                                     Service.DeleteActivity(data.Token, dto);
            });
        }

        [Test]
        public void SaveNewActivity_DefaultColor()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var activity = new ActivityDTO();
            activity.Name = "name";
            activity.Color = "";
            activity.Duration = TimeSpan.FromMinutes(90);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveActivity(data.Token, activity);
                Assert.AreEqual(Constants.DefaultColor, res.Color);
            });
            var db = Session.QueryOver<Activity>().SingleOrDefault();
            Assert.AreEqual(Constants.DefaultColor, db.Color);
            
        }

        [Test]
        public void SaveNewActivity()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var activity = new ActivityDTO();
            activity.Name = "name";
            activity.Color = System.Drawing.Color.Aqua.ToColorString();
            activity.Duration = TimeSpan.FromMinutes(90);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveActivity(data.Token,activity);
                Assert.Greater(res.CreationDate,DateTime.MinValue);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                activity.CreationDate = res.CreationDate;
                activity.ProfileId = res.ProfileId;
                Assert.IsFalse(activity.IsModified(res));
            });
            Assert.AreEqual(1, Session.QueryOver<Activity>().RowCount());
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateActivity_OtherProfile()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = activity.Map<ActivityDTO>();
            dto.Name = "name";
            dto.ProfileId = profile.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
               service.SaveActivity(data.Token, dto);
            });

        }

        [Test]
        public void UpdateActivity()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = activity.Map<ActivityDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                var res = service.SaveActivity(data.Token, dto);
                UnitTestHelper.CompareDateTime(res.CreationDate, activity.CreationDate);
                Assert.Greater(res.CreationDate, DateTime.MinValue);
                Assert.IsFalse(dto.IsModified(res));
            });
            Assert.AreEqual(1, Session.QueryOver<Activity>().RowCount());

        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateActivity_OldData()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = activity.Map<ActivityDTO>();
            dto.Version = 0;
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                service.SaveActivity(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(UniqueException))]
        public void SaveActivity_Validation_UniqueName()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var activity = new ActivityDTO();
            activity.Name = "name";
            activity.Duration = TimeSpan.FromMinutes(90);
            activity.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveActivity(data.Token, activity);
            });

            activity = new ActivityDTO();
            activity.Name = "name";
            activity.Duration = TimeSpan.FromMinutes(90);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveActivity(data.Token, activity);
            });
        }

        [Test]
        public void SaveActivity_Validation_UniqueName_DifferentUser()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            var profile1 = (ProfileDTO)profiles[1].Tag;

            SessionData data = CreateNewSession(profile, ClientInformation);
            var activity = new ActivityDTO();
            activity.Name = "name";
            activity.Duration = TimeSpan.FromMinutes(90);
            activity.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveActivity(data.Token, activity);
            });
            data = CreateNewSession(profile1, ClientInformation);
            activity = new ActivityDTO();
            activity.Name = "name";
            activity.Color = System.Drawing.Color.Aqua.ToColorString();
            activity.Duration = TimeSpan.FromMinutes(90);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveActivity(data.Token, activity);
            });
            Assert.AreEqual(2, Session.QueryOver<Activity>().RowCount());
        }

        [Test]
        public void DeleteActivity_DataInfo()
        {
            Activity activity = CreateActivity("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.ActivityHash;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                Service.DeleteActivity(data.Token, activity.Map<ActivityDTO>());
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash,dbProfile.DataInfo.ActivityHash);
        }


        [Test]
        public void SaveNewActivity_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var oldHash = profiles[0].DataInfo.ActivityHash;

            var activity = new ActivityDTO();
            activity.Name = "name";
            activity.Color = System.Drawing.Color.Aqua.ToColorString();
            activity.Duration = TimeSpan.FromMinutes(90);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveActivity(data.Token, activity);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.ActivityHash);
        }
    }
}
