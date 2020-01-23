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
    public class TestService_SaveDeleteMyPlace : TestServiceBase
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
        public void SaveNewMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var Place = new MyPlaceDTO();
            Place.Name = "name";
            Place.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                var res = service.SaveMyPlace(data.Token, Place);
                Assert.Greater(res.CreationDate, DateTime.MinValue);
                Assert.AreNotEqual(Guid.Empty, res.GlobalId);
                Assert.AreEqual(profile.GlobalId,res.ProfileId);
            });
            Assert.AreEqual(2, Session.QueryOver<MyPlace>().Where(x=>x.Profile==profiles[0]).RowCount());
        }

        [Test]
        public void SaveNewMyPlace_DefaultColor()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var Place = new MyPlaceDTO();
            Place.Name = "name";
            Place.Color = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                Place = service.SaveMyPlace(data.Token, Place);
                Assert.AreEqual(Constants.DefaultColor, Place.Color);
            });
            var db = Session.Get<MyPlace>(Place.GlobalId);
            Assert.AreEqual(Constants.DefaultColor, db.Color);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void UpdateMyPlace_OtherProfile()
        {
            MyPlace myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Name = "name";
            dto.ProfileId = profile.GlobalId;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveMyPlace(data.Token, dto);
            });

        }
        

        [Test]
        public void UpdateMyPlace()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                var res = service.SaveMyPlace(data.Token, dto);
                UnitTestHelper.CompareDateTime(res.CreationDate, myPlace.CreationDate);
                Assert.Greater(res.CreationDate, DateTime.MinValue);
                myPlace.CreationDate.CompareDateTime(res.CreationDate);
                Assert.AreEqual(dto.Name,res.Name);
            });
            Assert.AreEqual(2, Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0]).RowCount());

        }

        [Test]
        [ExpectedException(typeof(StaleObjectStateException))]
        public void UpdateMyPlace_OldData()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = Place.Map<MyPlaceDTO>();
            dto.Version = 0;
            dto.Name = "name";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                service.SaveMyPlace(data.Token, dto);
            });

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteSystemMyPlace()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var systemPlace=Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0] && x.IsSystem).SingleOrDefault();
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = systemPlace.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
        }

        [Test]
        public void DeleteDefaultMyPlace_SystemMyPlaceShouldBeDefaultAgain()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            Place.IsDefault = true;
            insertToDatabase(Place);
            var systemPlace=Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0] && x.IsSystem).SingleOrDefault();
            systemPlace.IsDefault = false;
            insertToDatabase(systemPlace);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
            systemPlace = Session.QueryOver<MyPlace>().Where(x=>x.Profile==profiles[0] && x.IsSystem).SingleOrDefault();
            Assert.IsTrue(systemPlace.IsDefault);
        }


        [Test]
        public void DeleteEmptyMyPlace()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
                {
                    var param = new MyPlaceOperationParam();
                    param.MyPlaceId = Place.GlobalId;
                    param.Operation = MyPlaceOperationType.Delete;
                    Service.MyPlaceOperation(data.Token, param);
            });
            var item = Session.Get<MyPlace>(Place.GlobalId);
            Assert.AreEqual(null, item);
        }

        [Test]
        [ExpectedException(typeof(DeleteConstraintException))]
        public void DeleteMyPlaceWithEntries()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var day = new TrainingDay(DateTime.Now.AddDays(-2));
            day.Profile = profiles[0];
            var entry = new StrengthTrainingEntry();
            entry.MyPlace = Place;
            day.AddEntry(entry);
            insertToDatabase(day);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteMyPlace_OtherUser()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void DeleteMyPlace_SecurityBug()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            //we login as a different user
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
        }

        [Test]
        public void SetDefault()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.SetDefault;
                Service.MyPlaceOperation(data.Token, param);
            });
            var dbPlace=Session.Get<MyPlace>(Place.GlobalId);
            Assert.IsTrue(dbPlace.IsDefault);
            var count = Session.QueryOver<MyPlace>().Where(x => x.Profile == profiles[0] && x.IsDefault).RowCount();
            Assert.AreEqual(1,count);
        }

        [Test]
        [ExpectedException(typeof(CrossProfileOperationException))]
        public void SetDefault_AnotherProfile()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[1].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.SetDefault;
                Service.MyPlaceOperation(data.Token, param);
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateMyPlace_SetDefaultChanges()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Name = "name";
            dto.IsDefault = true;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                service.SaveMyPlace(data.Token, dto);
                
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateMyPlace_IsSystemChanged()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Name = "name";
            dto.IsSystem = true;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                TimerService.UtcNow = DateTime.UtcNow.AddHours(2);
                service.SaveMyPlace(data.Token, dto);
            });
        }

        #region Address

        [Test]
        public void SaveNewMyPlace_WithAddress()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var Place = new MyPlaceDTO();
            Place.Name = "name";
            Place.Color = System.Drawing.Color.Aqua.ToColorString();
            Place.Address = new AddressDTO();
            Place.Address.City = "Miasto";
            Place.Address.Country = "country";
            Place.Address.Address1 = "address1";
            Place.Address.Address2 = "address2";
            Place.Address.PostalCode = "12345";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                Place = service.SaveMyPlace(data.Token, Place);
                Assert.IsNotNull(Place.Address);
            });
            Assert.AreEqual(1, Session.QueryOver<Address>().RowCount());
            Assert.IsNotNull(Session.Get<MyPlace>(Place.GlobalId).Address);
        }

        [Test]
        public void UpdateMyPlace_AddedAddress()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Address = new AddressDTO();
            dto.Address.City = "Miasto";
            dto.Address.Country = "country";
            dto.Address.Address1 = "address1";
            dto.Address.Address2 = "address2";
            dto.Address.PostalCode = "12345";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dto = service.SaveMyPlace(data.Token, dto);
                Assert.IsNotNull(dto.Address);
            });
            Assert.AreEqual(1, Session.QueryOver<Address>().RowCount());
            Assert.IsNotNull(Session.Get<MyPlace>(dto.GlobalId).Address);
        }

        [Test]
        public void UpdateMyPlace_AddedEmptyAddress()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Address = new AddressDTO();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dto = service.SaveMyPlace(data.Token, dto);
                Assert.IsNull(dto.Address);
            });
            Assert.AreEqual(0, Session.QueryOver<Address>().RowCount());
            Assert.IsNull(Session.Get<MyPlace>(dto.GlobalId).Address);
        }

        [Test]
        public void UpdateMyPlace_ExistingEmptyAddress()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            myPlace.Address = new Address();
            myPlace.Address.City = "Miasto";
            myPlace.Address.Country = "country";
            myPlace.Address.Address1 = "address1";
            myPlace.Address.Address2 = "address2";
            myPlace.Address.PostalCode = "12345";
            insertToDatabase(myPlace);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Address.City = "";
            dto.Address.Country = "";
            dto.Address.Address1 = "";
            dto.Address.Address2 = "";
            dto.Address.PostalCode = "";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dto = service.SaveMyPlace(data.Token, dto);
                Assert.IsNull(dto.Address);
            });
            Assert.AreEqual(0, Session.QueryOver<Address>().RowCount());
            Assert.IsNull(Session.Get<MyPlace>(dto.GlobalId).Address);
        }

        [Test]
        public void UpdateMyPlace_ExistingAddress()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            myPlace.Address = new Address();
            myPlace.Address.City = "Miasto";
            myPlace.Address.Country = "country";
            myPlace.Address.Address1 = "address1";
            myPlace.Address.Address2 = "address2";
            myPlace.Address.PostalCode = "12345";
            insertToDatabase(myPlace);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Address.City = "NewCity";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dto = service.SaveMyPlace(data.Token, dto);
                Assert.AreEqual("NewCity",dto.Address.City);
            });
            Assert.AreEqual(1, Session.QueryOver<Address>().RowCount());
            Assert.AreEqual("NewCity",Session.Get<MyPlace>(dto.GlobalId).Address.City);
        }

        [Test]
        public void UpdateMyPlace_SetNewAddress()
        {
            var myPlace = CreateMyPlace("test", profiles[0]);
            myPlace.Address = new Address();
            myPlace.Address.City = "Miasto";
            myPlace.Address.Country = "country";
            myPlace.Address.Address1 = "address1";
            myPlace.Address.Address2 = "address2";
            myPlace.Address.PostalCode = "12345";
            insertToDatabase(myPlace);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);

            var dto = myPlace.Map<MyPlaceDTO>();
            dto.Address=new AddressDTO();
            dto.Address.City = "NewCity";
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                dto = service.SaveMyPlace(data.Token, dto);
            });
            var count = Session.QueryOver<Address>().RowCount();
            Assert.AreEqual(1, count);
            var dbAddress = Session.Get<Address>(dto.Address.GlobalId);
            Assert.AreEqual("NewCity", dbAddress.City);
        }
        #endregion

        #region DataInfo

        [Test]
        public void SaveNewMyPlace_DataInfo()
        {
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.MyPlaceHash;

            var Place = new MyPlaceDTO();
            Place.Name = "name";
            Place.Color = System.Drawing.Color.Aqua.ToColorString();
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.SaveMyPlace(data.Token, Place);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.MyPlaceHash);
        }

        [Test]
        public void SetDefault_DataInfo()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.MyPlaceHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.SetDefault;
                Service.MyPlaceOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.MyPlaceHash);
        }

        [Test]
        public void DeleteEmptyMyPlace_DataInfo()
        {
            var Place = CreateMyPlace("test", profiles[0]);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            var oldHash = profiles[0].DataInfo.MyPlaceHash;

            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                var param = new MyPlaceOperationParam();
                param.MyPlaceId = Place.GlobalId;
                param.Operation = MyPlaceOperationType.Delete;
                Service.MyPlaceOperation(data.Token, param);
            });
            var dbProfile = Session.Get<Profile>(profile.GlobalId);
            Assert.AreNotEqual(oldHash, dbProfile.DataInfo.MyPlaceHash);
        }
        #endregion
    }
}
