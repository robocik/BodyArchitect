using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using NUnit.Framework;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_AccountOperation : TestServiceBase
    {
        [Test]
        [ExpectedException(typeof(ProfileDeletedException))]
        public void SendReminderEmailToDeletedProfile()
        {
            var profile = CreateProfile(Session, "test1");
            profile.IsDeleted = true;
            insertToDatabase(profile);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.AccountOperation("test1",AccountOperationType.RestorePassword);
            });
        }
    }
}
