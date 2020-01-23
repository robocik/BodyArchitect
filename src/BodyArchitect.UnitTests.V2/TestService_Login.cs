using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Payments;
using BodyArchitect.Shared;
using NUnit.Framework;
using AccountType = BodyArchitect.Service.V2.Model.AccountType;
using Constants = BodyArchitect.Portable.Constants;
using LicenceType = BodyArchitect.Service.V2.LicenceType;
using ReminderRepetitions = BodyArchitect.Model.ReminderRepetitions;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_Login:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        Guid key = new Guid("EB17BC2A-94FD-4E65-8751-15730F69E7F5");

        public override void BuildDatabase()
        {
            profiles.Clear();
            
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1");
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2");
                profile.IsDeleted = true;
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3");
                profile.Password = CryptographyHelper.ToSHA1Hash(profile.UserName);
                Session.Update(profile);
                profiles.Add(profile);
                
                APIKey apiKey = new APIKey();
                apiKey.ApiKey = key;
                apiKey.ApplicationName = "UnitTest";
                apiKey.EMail = "mail@mail.com";
                apiKey.RegisterDateTime = DateTime.UtcNow;
                insertToDatabase(apiKey);
                tx.Commit();
            }
        }

        [Test]
        public void TestLogin()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session=service.Login(info, "Profile1", password);
                Assert.IsNotNull(session);
            });
        }

        [Test]
        public void TestLogin_WrongPassword()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("wrong pwd");

                var session = service.Login(info, "Profile1", password);
                Assert.IsNull(session);
            });
        }

        [Test]
        [ExpectedException(typeof(ProfileDeletedException))]
        public void TestLogin_DeletedProfile()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile2");

                service.Login(info, "Profile2", password);
            });
        }

        [Test]
        public void TestLogin_Statistics()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.IsNotNull(dbProfile.Statistics.LastLoginDate);
        }

        [Test]
        public void SetLoginData()
        {
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();
            Assert.AreEqual("pl",loginData.ApplicationLanguage);
            Assert.AreEqual(Constants.Version, loginData.ApplicationVersion);
        }

        [Test]
        public void SetApiKeyInLoginData()
        {
            
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var loginData = Session.QueryOver<LoginData>().SingleOrDefault();
            Assert.AreEqual(key, loginData.ApiKey.ApiKey);
        }


        #region BAPoints

        #region Promotions

        [Test]
        public void Promotion_PremiumUserLoggedAsInstructor_NotZeroCostPromotion()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(1);
                //set promotion cost of instructor account to 1
                var manager= new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 1,PromotionStartDate = DateTime.UtcNow} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, "Profile1", password);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(9, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(1), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Promotion_InstructorLoggedAsInstructor_PromotionStarted()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //set promotion cost of instructor account to 0. promotion started in the middle of period to calculate
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date.AddDays(2)} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments = manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(4);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, "Profile1", password);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.Instructor, dbProfile.Licence.AccountType);
            Assert.AreEqual(7, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(4), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Promotion_InstructorButPromotionIsForPremium()
        {
            profiles[0].Licence.BAPoints =10;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //set promotion cost of instructor account to 0
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow} },
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points =3 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(1);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, "Profile1", password);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.Instructor, dbProfile.Licence.AccountType);
            Assert.AreEqual(7, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(1), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Promotion_UserLoggedAsInstructor_PromotionStartInPast()
        {
            profiles[0].Licence.BAPoints = 0;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow.Date;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //set promotion cost of instructor account to 0
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.AddDays(-3)} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.Instructor, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(0, securityInfo.Licence.BAPoints);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.User, dbProfile.Licence.AccountType);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Promotion_UserLoggedAsInstructor()
        {
            profiles[0].Licence.BAPoints = 0;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //set promotion cost of instructor account to 0
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.Instructor, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(0, securityInfo.Licence.BAPoints);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.User, dbProfile.Licence.AccountType);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Promotion_PremiumUserLoggedAsInstructor()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                //set promotion cost of instructor account to 0
                var manager = new PaymentsManager();
                var accountTypes = new Dictionary<AccountType, PaymentAccountType>()
                                       {
                                           { AccountType.Instructor, new PaymentAccountType(){AccountType = AccountType.Instructor,Points = 3,PromotionPoints = 0,PromotionStartDate = DateTime.UtcNow.Date} },
                                           { AccountType.PremiumUser, new PaymentAccountType(){AccountType = AccountType.PremiumUser,Points = 1 } },
                                       };
                var itemsToBuy = new Dictionary<string, int>();
                service.Configuration.Payments=manager.Load(accountTypes, itemsToBuy, 15);

                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.Instructor, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(10, securityInfo.Licence.BAPoints);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(10, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }
        #endregion

        [Test]
        public void Points_LoginFirstTimeAfterBuyPoints()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                {
                    ((MockTimerService) service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(10, securityInfo.Licence.BAPoints);
                
            });
            var dbProfile=Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(10, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginTwoTimesTheSameDay()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date;
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                service.Logout(session.Token);

                session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(10, securityInfo.Licence.BAPoints);

            });
            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(10, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter5Days_Premium()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(5);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(5, securityInfo.Licence.BAPoints);
                
            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(5, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(5), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter10Days_Premium()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(10);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.PremiumUser, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(0, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(10), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter12Days_BAPointsAreGone_Premium()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.PremiumUser;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(12);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.User, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(0, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.User, dbProfile.Licence.AccountType);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(12), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter2Days_Instructor()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(2);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.Instructor, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(6, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.Instructor, dbProfile.Licence.AccountType);
            Assert.AreEqual(6, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(2), dbProfile.Licence.LastPointOperationDate);
        }
        
        [Test]
        public void Points_LoginAfter6Days_BAPointsAreGone_Instructor()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.Instructor;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(6);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.User, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(0, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(BodyArchitect.Model.AccountType.User, dbProfile.Licence.AccountType);
            Assert.AreEqual(0, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(6), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter5Days_User_BAPointsAreNotChanged()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.User;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(5);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.User, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(10, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(Model.AccountType.User, dbProfile.Licence.AccountType);
            Assert.AreEqual(10, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(5), dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void Points_LoginAfter5Days_Administrator_BAPointsAreNotChanged()
        {
            profiles[0].Licence.BAPoints = 10;
            profiles[0].Licence.AccountType = Model.AccountType.Administrator;
            profiles[0].Licence.LastPointOperationDate = DateTime.UtcNow;
            insertToDatabase(profiles[0]);
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                ((MockTimerService)service.Configuration.TimerService).UtcNow = DateTime.UtcNow.Date.AddDays(5);
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                var session = service.Login(info, "Profile1", password);
                var securityInfo = SecurityManager.EnsureAuthentication(session.Token);
                Assert.AreEqual(AccountType.Administrator, securityInfo.Licence.CurrentAccountType);
                Assert.AreEqual(10, securityInfo.Licence.BAPoints);

            });

            var dbProfile = Session.Get<Profile>(profiles[0].GlobalId);
            Assert.AreEqual(Model.AccountType.Administrator, dbProfile.Licence.AccountType);
            Assert.AreEqual(10, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date.AddDays(5), dbProfile.Licence.LastPointOperationDate);
        }
        #endregion

        #region Remove old reminders

        [Test]
        public void RemoveOldRepetitionsOnceReminders_CustomeReminder()
        {
            var reminderFuture=CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(-3));

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var dbReminder=Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(dbReminder.GlobalId,reminderFuture.GlobalId);
        }

        [Test]
        public void RemoveOldRepetitionsEveryDayReminders_CustomeReminder()
        {
            var reminderFuture = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(-3),pattern:ReminderRepetitions.EveryDay);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(2,count);
        }

        [Test]
        public void RemoveOldRepetitionsEveryYearReminders_CustomeReminder()
        {
            var reminderFuture = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(-3), pattern: ReminderRepetitions.EveryYear);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void RemoveOldRepetitionsOnceReminders_AnotherProfile()
        {
            var reminderFuture = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[1], DateTime.UtcNow.Date.AddDays(-3));

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var count = Session.QueryOver<ReminderItem>().RowCount();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void RemoveOldRepetitionsOnceReminders_ConnectedObjectReminder()
        {
            var reminderFuture = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(-3));
            TrainingDay day =new TrainingDay(DateTime.Now);
            SizeEntry size = new SizeEntry();
            size.Wymiary=new Wymiary();
            day.AddEntry(size);
            day.Profile = profiles[0];
            size.Reminder = reminderPast;
            insertToDatabase(day);
            reminderPast.ConnectedObject="EntryObjectDTO:"+size.GlobalId.ToString();
            insertToDatabase(reminderPast);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var dbReminder = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(dbReminder.GlobalId, reminderFuture.GlobalId);

            var dbSize = Session.Get<SizeEntry>(size.GlobalId);
            Assert.IsNull(dbSize.Reminder);
        }

        [Test]
        public void RemoveOldRepetitionsOnceReminders_ConnectedObjectReminder_ForCustomer()
        {
            var cust = CreateCustomer("tesst",profiles[0]);
            var reminderFuture = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(2));
            var reminderPast = CreateReminder("test", profiles[0], DateTime.UtcNow.Date.AddDays(-3));
            TrainingDay day = new TrainingDay(DateTime.Now);
            SizeEntry size = new SizeEntry();
            size.Wymiary = new Wymiary();
            day.AddEntry(size);
            day.Customer = cust;
            day.Profile = profiles[0];
            size.Reminder = reminderPast;
            insertToDatabase(day);
            reminderPast.ConnectedObject = "EntryObjectDTO:" + size.GlobalId.ToString();
            insertToDatabase(reminderPast);

            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                service.Configuration.CurrentApiKey = key;
                ClientInformation info = new ClientInformation();
                info.ApplicationVersion = Constants.Version;
                info.ApplicationLanguage = "pl";
                info.Version = Const.ServiceVersion;
                info.ClientInstanceId = Guid.NewGuid();
                string password = CryptographyHelper.ToSHA1Hash("Profile1");

                service.Login(info, profiles[0].UserName, password);
            });

            var dbReminder = Session.QueryOver<ReminderItem>().SingleOrDefault();
            Assert.AreEqual(dbReminder.GlobalId, reminderFuture.GlobalId);

            var dbSize = Session.Get<SizeEntry>(size.GlobalId);
            Assert.IsNull(dbSize.Reminder);
        }
        #endregion
    }
}
