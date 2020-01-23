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
    public class TestService_GetComments : TestServiceBase
    {
        private APIKey apiKey;
        List<Profile> profiles = new List<Profile>();
        List<RatingUserValue> comments = new List<RatingUserValue>();
        private Guid globalId1;

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                comments.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));

                apiKey = new APIKey();
                apiKey.ApiKey = Guid.NewGuid();
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);

                var apiKey1 = new APIKey();
                apiKey1.ApiKey = Guid.NewGuid();
                apiKey1.ApplicationName = "UnitTest";
                apiKey1.EMail = "mail@mail.com";
                apiKey1.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey1);

                LoginData loginData1 = new LoginData();
                loginData1.ApiKey = apiKey;
                loginData1.ApplicationVersion = "1.0.0";
                loginData1.LoginDateTime = DateTime.UtcNow;
                loginData1.ApplicationLanguage = "en";
                loginData1.PlatformVersion = "NUnit";
                insertToDatabase(loginData1);

                var loginData2 = new LoginData();
                loginData2.ApiKey = apiKey1;
                loginData2.ApplicationVersion = "1.0.0";
                loginData2.LoginDateTime = DateTime.UtcNow;
                loginData2.ApplicationLanguage = "en";
                loginData2.PlatformVersion = "NUnit";
                insertToDatabase(loginData2);

                globalId1 = Guid.NewGuid();
                RatingUserValue comment = new RatingUserValue();
                comment.ProfileId = profiles[0].GlobalId;
                comment.ShortComment = "t1";
                comment.Rating = 3;
                comment.RatedObjectId = globalId1;
                comment.LoginData = loginData1;
                comments.Add(comment);
                Session.Save(comment);

                comment = new RatingUserValue();
                comment.ProfileId = profiles[1].GlobalId;
                comment.ShortComment = "t2";
                comment.Rating = 1;
                comment.RatedObjectId = globalId1;
                comment.LoginData = loginData2;
                comments.Add(comment);
                Session.Save(comment);

                comment = new RatingUserValue();
                comment.ProfileId = profiles[2].GlobalId;
                comment.ShortComment = "t2";
                comment.Rating = 5;
                comment.RatedObjectId = globalId1;
                comments.Add(comment);
                Session.Save(comment);
               
                
                tx.Commit();
            }
        }

        [Test]
        public void GetApplicationName()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation, null, apiKey);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CommentEntryDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetComments(data.Token, globalId1, pageInfo);
            });
            Assert.AreEqual(apiKey.ApplicationName, result.Items[0].ApplicationName);
            Assert.AreEqual(apiKey.ApplicationName, result.Items[1].ApplicationName);
            Assert.AreEqual(null, result.Items[2].ApplicationName);
        }

        [Test]
        public void GetComments()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation, null, apiKey);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<CommentEntryDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetComments(data.Token, globalId1, pageInfo);
            });
            Assert.AreEqual(comments.Count, result.AllItemsCount);
            Assert.AreEqual(comments.Count, result.Items.Count);
        }
    }
}
