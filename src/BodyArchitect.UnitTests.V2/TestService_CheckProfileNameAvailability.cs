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
    public class TestService_CheckProfileNameAvailability : TestServiceBase
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
        public void UsernameExists()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    bool res=service.CheckProfileNameAvailability("test1");
                    Assert.IsFalse(res);
                });
        }

        [Test]
        public void UsernameNotExists()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                bool res = service.CheckProfileNameAvailability("test3");
                Assert.IsTrue(res);
            });
        }

        [Test]
        public void ShortUsername()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                bool res = service.CheckProfileNameAvailability("t");
                Assert.IsFalse(res);
            });
        }

        [Test]
        public void UsernameWithWrongCharacters()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                bool res = service.CheckProfileNameAvailability("t&gfhgf");
                Assert.IsFalse(res);
            });
        }
    }
}
