using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_GetBlogComments:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        private BlogEntry entry;
        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));

                TrainingDay day = new TrainingDay(DateTime.Now);
                entry = new BlogEntry();
                entry.AllowComments = true;
                entry.Comment = "test1";
                day.AddEntry(entry);
                day.Profile = profiles[0];
                Session.Save(day);
                BlogComment comment = new BlogComment();
                comment.BlogEntry = entry;
                comment.Comment = "t1";
                comment.Profile = profiles[1];
                Session.SaveOrUpdate(comment);
                comment = new BlogComment();
                comment.BlogEntry = entry;
                comment.Comment = "t2";
                comment.Profile = profiles[0];
                Session.SaveOrUpdate(comment);
                comment = new BlogComment();
                comment.BlogEntry = entry;
                comment.Comment = "t3";
                comment.Profile = profiles[2];
                Session.SaveOrUpdate(comment);

                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);
                Session.Update(profiles[0]);
                Session.Update(profiles[1]);
                tx.Commit();
            }
        }

        [Test]
        public void TestGetCommentsByMyself()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<BlogCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var criteria = Mapper.Map<BlogEntry, BlogEntryDTO>(entry);
                result = Service.GetBlogComments(data.Token, criteria, pageInfo);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(3,result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);
        }

        [Test]
        public void TestGetCommentsByFriend()
        {
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<BlogCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var criteria = Mapper.Map<BlogEntry, BlogEntryDTO>(entry);
                result = Service.GetBlogComments(data.Token, criteria, pageInfo);
            });
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);

        }

        [Test]
        public void TestGetCommentsByOther()
        {
            var profile = (ProfileDTO) profiles[2].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<BlogCommentDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                                 {
                                     var criteria = Mapper.Map<BlogEntry, BlogEntryDTO>(entry);
                                     result = Service.GetBlogComments(data.Token, criteria, pageInfo);
                                 });
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AllItemsCount);
            Assert.AreEqual(3, result.Items.Count);
        }
    }
}
