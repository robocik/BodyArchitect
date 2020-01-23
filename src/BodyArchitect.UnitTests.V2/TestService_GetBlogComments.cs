using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetTrainingDayComments:TestServiceBase
    {
        private APIKey apiKey;
        List<Profile> profiles = new List<Profile>();
        private TrainingDay day;
        List<TrainingDayComment> comments = new List<TrainingDayComment>();
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                comments.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));

                day = new TrainingDay(DateTime.Now);
                var entry = new BlogEntry();
                day.AllowComments = true;
                entry.Comment = "test1";
                day.AddEntry(entry);
                day.Profile = profiles[0];
                Session.Save(day);
                TrainingDayComment comment = new TrainingDayComment();
                comment.TrainingDay = day;
                comment.Comment = "t1";
                comment.Profile = profiles[1];
                comments.Add(comment);
                Session.SaveOrUpdate(comment);
                comment = new TrainingDayComment();
                comment.TrainingDay = day;
                comment.Comment = "t2";
                comment.Profile = profiles[0];
                comments.Add(comment);
                Session.SaveOrUpdate(comment);
                comment = new TrainingDayComment();
                comment.TrainingDay = day;
                comment.Comment = "t3";
                comment.Profile = profiles[2];
                comments.Add(comment);
                Session.SaveOrUpdate(comment);

                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);

                apiKey = new APIKey();
                apiKey.ApiKey = Guid.NewGuid();
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        [Test]
        public void GetApplicationName()
        {

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation,null, apiKey);
            var loginData=Session.QueryOver<LoginData>().SingleOrDefault();
            comments[0].LoginData = loginData;
            insertToDatabase(comments[0]);
            comments[1].LoginData = loginData;
            insertToDatabase(comments[1]);

            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<TrainingDayCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var criteria = Mapper.Map<TrainingDay, TrainingDayDTO>(day);
                result = Service.GetTrainingDayComments(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(apiKey.ApplicationName, result.Items[0].ApplicationName);
            Assert.AreEqual(apiKey.ApplicationName, result.Items[1].ApplicationName);
            Assert.AreEqual(null, result.Items[2].ApplicationName);
        }

        [Test]
        public void TestGetCommentsByMyself()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<TrainingDayCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var criteria = Mapper.Map<TrainingDay, TrainingDayDTO>(day);
                result = Service.GetTrainingDayComments(data.Token, criteria, pageInfo);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(3,result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);
        }

        [Test]
        public void TestGetCommentsByFriend()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<TrainingDayCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var criteria = Mapper.Map<TrainingDay, TrainingDayDTO>(day);
                result = Service.GetTrainingDayComments(data.Token, criteria, pageInfo);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);

        }

        [Test]
        public void TestGetCommentsByOther()
        {
            var profile = (ProfileDTO) profiles[2].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<TrainingDayCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     var criteria = Mapper.Map<TrainingDay, TrainingDayDTO>(day);
                                     result = Service.GetTrainingDayComments(data.Token, criteria, pageInfo);
                                 });
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);
        }
    }
}
