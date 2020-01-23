using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using NUnit.Framework;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetMessages : TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Message>  messages = new List<Message>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                messages.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));
                profiles.Add(CreateProfile(Session, "test3"));
                
                var message = CreateMessage(profiles[0],profiles[1]);
                messages.Add(message);
                message = CreateMessage(profiles[0], profiles[2]);
                messages.Add(message);
                message = CreateMessage(profiles[1], profiles[0],DateTime.UtcNow.AddDays(-2));
                messages.Add(message);
                message = CreateMessage(profiles[2], profiles[0]);
                messages.Add(message);
                message = CreateMessage(profiles[2], profiles[1]);
                messages.Add(message);
                message = CreateMessage(profiles[2], profiles[0], DateTime.UtcNow.AddDays(2));
                messages.Add(message);
                tx.Commit();
            }
        }

        [Test]
        public void All()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(result.Items.Count, result.AllItemsCount);
            assert(result,2,3,5);
        }

        [Test]
        public void FromDate()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            criteria.FromDate = DateTime.UtcNow.AddDays(-1);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(result.Items.Count, result.AllItemsCount);
            assert(result,  3, 5);
        }

        [Test]
        public void ToDate()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            criteria.ToDate = DateTime.UtcNow.AddDays(1);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(result.Items.Count,result.AllItemsCount);
            assert(result, 2,3);
        }

        [Test]
        public void Paging()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 2;

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(3, result.AllItemsCount);
            assert(result, 5, 3);
        }

        [Test]
        public void SortByDate_Desc()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            criteria.SortAscending = false;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(messages[5].GlobalId,result.Items[0].GlobalId);
            Assert.AreEqual(messages[3].GlobalId, result.Items[1].GlobalId);
            Assert.AreEqual(messages[2].GlobalId, result.Items[2].GlobalId);
        }

        [Test]
        public void SortByDate_Asc()
        {
            GetMessagesCriteria criteria = new GetMessagesCriteria();
            criteria.SortAscending = true;
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();

            PagedResult<MessageDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetMessages(data.Token, criteria, pageInfo);
            });
            Assert.AreEqual(messages[2].GlobalId, result.Items[0].GlobalId);
            Assert.AreEqual(messages[3].GlobalId, result.Items[1].GlobalId);
            Assert.AreEqual(messages[5].GlobalId, result.Items[2].GlobalId);
        }

        void assert(PagedResult<MessageDTO> pack,params int[] availableIndexes)
        {
            Assert.AreEqual(pack.Items.Count, pack.Items.Count);
            if (availableIndexes != null)
            {
                foreach (int index in availableIndexes)
                {
                    Assert.IsNotNull(pack.Items.Where(x => x.GlobalId == messages[index].GlobalId).SingleOrDefault());
                }
            }
        }
    }
}
