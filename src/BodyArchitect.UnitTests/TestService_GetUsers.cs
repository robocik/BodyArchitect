using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service;
using BodyArchitect.Service.Model;
using NUnit.Framework;
using Gender = BodyArchitect.Model.Gender;
using Privacy = BodyArchitect.Model.Privacy;
using PublishStatus = BodyArchitect.Model.PublishStatus;

namespace BodyArchitect.UnitTests
{
    [TestFixture]
    public class TestService_GetUsers:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();

        public override void BuildDatabase()
        {
            profiles.Clear();
            using (var tx = Session.BeginTransaction())
            {
                var profile = CreateProfile(Session, "Profile1",Country.Countries[0],Gender.Male);
                profile.Statistics.TrainingDaysCount = 200;
                profile.Privacy.CalendarView = Privacy.Public;
                profile.Privacy.Sizes = Privacy.FriendsOnly;
                profile.Privacy.BirthdayDate = Privacy.Public;
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile2", Country.Countries[0], Gender.Female);
                profile.Statistics.TrainingDaysCount = 100;
                profile.Statistics.FollowersCount = 120;
                profile.Statistics.VotingsCount = 4;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-3).Date;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-1).Date;
                profile.Picture=new Picture(Guid.NewGuid(),"gdfgdfgdgdfgdfgdfgdfg");
                profile.Privacy.Sizes = Privacy.Public;
                profile.Privacy.BirthdayDate = Privacy.FriendsOnly;
                profile.Privacy.Friends = Privacy.Public;
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile3", Country.Countries[0], Gender.Female);
                profile.Statistics.TrainingDaysCount = 140;
                profile.Statistics.VotingsCount = 7;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-7).Date;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-11).Date;
                profile.Statistics.FollowersCount = 30;
                profile.Privacy.CalendarView = Privacy.Public;
                profile.Privacy.Sizes = Privacy.FriendsOnly;
                profile.Privacy.BirthdayDate = Privacy.Public;
                profile.Privacy.Friends = Privacy.FriendsOnly;
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile4", Country.Countries[0], Gender.NotSet);
                profile.Statistics.TrainingDaysCount = 0;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-21).Date;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-1).Date;
                profile.Statistics.FollowersCount = 68;
                profile.Statistics.VotingsCount = 4;
                profile.Privacy.BirthdayDate = Privacy.Public;
                profile.Privacy.Friends = Privacy.FriendsOnly;
                profile.Picture = new Picture(Guid.NewGuid(), "gdfgdfgdgdfgdfgdfgdfg");
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile5", Country.Countries[1], Gender.Female);
                profile.Statistics.TrainingDaysCount = 30;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-13).Date;
                profile.Statistics.VotingsCount = 14;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-5).Date;
                profile.Statistics.FollowersCount = 22;
                profile.Privacy.CalendarView = Privacy.Public;
                profile.Picture = new Picture(Guid.NewGuid(), "gdfgdfgdgdfgdfgdfgdfg");
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile6", Country.Countries[1], Gender.Male);
                profile.Statistics.TrainingDaysCount = 210;
                profile.Statistics.VotingsCount = 1;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-5).Date;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-31).Date;
                profile.Statistics.FollowersCount = 33;
                profile.Privacy.CalendarView = Privacy.Public;
                profile.Privacy.Sizes = Privacy.FriendsOnly;
                profile.Privacy.BirthdayDate = Privacy.Public;
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile7", Country.Countries[2], Gender.Male);
                profile.Statistics.TrainingDaysCount = 200;
                profile.Statistics.VotingsCount = 2;
                profile.Statistics.LastLoginDate = DateTime.Now.AddDays(-13).Date;
                profile.Statistics.FollowersCount = 0;
                profile.Statistics.LastEntryDate = DateTime.Now.AddDays(-2).Date;
                profile.Privacy.CalendarView = Privacy.Public;
                profile.Privacy.Sizes = Privacy.Public;
                profile.Privacy.BirthdayDate = Privacy.Public;
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile8", Country.Countries[2], Gender.Female);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile9", Country.Countries[3], Gender.NotSet);
                profile.Picture = new Picture(Guid.NewGuid(), "gdfgdfgdgdfgdfgdfgdfg");
                Session.Update(profile);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile10", Country.Countries[3], Gender.Female);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile11", Country.Countries[3], Gender.Male);
                profiles.Add(profile);
                profile = CreateProfile(Session, "Profile12", Country.Countries[3], Gender.Female);
                profile.Picture = new Picture(Guid.NewGuid(), "gdfgdfgdgdfgdfgdfgdfg");
                Session.Update(profile);
                profiles.Add(profile);
                //add one non searchable user
                profile = CreateProfile(Session, "Profile13", Country.Countries[3], Gender.Female);
                profile.Privacy.Searchable = false;
                profile.Picture = new Picture(Guid.NewGuid(), "gdfgdfgdgdfgdfgdfgdfg");
                Session.Update(profile);
                profiles.Add(profile);

                //set friends
                profiles[0].Friends.Add(profiles[1]);
                profiles[1].Friends.Add(profiles[0]);

                profiles[0].Friends.Add(profiles[3]);
                profiles[3].Friends.Add(profiles[0]);

                profiles[0].Friends.Add(profiles[5]);
                profiles[5].Friends.Add(profiles[0]);

                profiles[0].FavoriteUsers.Add(profiles[2]);
                profiles[0].FavoriteUsers.Add(profiles[8]);
                profiles[0].FavoriteUsers.Add(profiles[4]);
                profiles[0].FavoriteUsers.Add(profiles[7]);
                Session.Update(profiles[0]);
                Session.Update(profiles[3]);
                Session.Update(profiles[4]);

                //add workout plans
                TrainingPlan plan = new TrainingPlan();
                plan.Language = "en";
                plan.Name = "plan1";
                plan.PlanContent = "fgfdg";
                plan.Author = "fdgfdg";
                plan.GlobalId = Guid.NewGuid();
                plan.Status = PublishStatus.Published;
                plan.Profile = profiles[1];
                Session.Save(plan);

                plan = new TrainingPlan();
                plan.GlobalId = Guid.NewGuid();
                plan.Language = "en";
                plan.Name = "plan1";
                plan.PlanContent = "fgfdg";
                plan.Author = "fdgfdg";
                plan.Profile = profiles[7];
                Session.Save(plan);
                plan = new TrainingPlan();
                plan.Language = "en";
                plan.GlobalId = Guid.NewGuid();
                plan.Name = "plan1";
                plan.PlanContent = "fgfdg";
                plan.Author = "fdgfdg";
                plan.Profile = profiles[7];
                Session.Save(plan);
                plan = new TrainingPlan();
                plan.Language = "en";
                plan.GlobalId = Guid.NewGuid();
                plan.Name = "plan1";
                plan.PlanContent = "fgfdg";
                plan.Author = "fdgfdg";
                plan.Profile = profiles[8];
                Session.Save(plan);
                tx.Commit();
            }
        }

        #region Privacy 

        [Test]
        public void TestUsersWithAccessibleFriends()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessFriends = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 2, "Profile2", "Profile4");
        }

        [Test]
        public void TestUsersWithNoAccessibleFriends()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessFriends = PrivacyCriteria.NotAccessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 9, "Profile3", "Profile5", "Profile6", "Profile7", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestUsersWithAccessibleSizes()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessSizes = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 3, "Profile2", "Profile6", "Profile7");
        }
        [Test]
        public void TestUsersWithNoAccessibleSizes()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessSizes = PrivacyCriteria.NotAccessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 8, "Profile3", "Profile4", "Profile5","Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestUsersWithAccessibleCalendar()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessCalendar = PrivacyCriteria.Accessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 4, "Profile3", "Profile5", "Profile6", "Profile7");
        }
        [Test]
        public void TestUsersWithNoAccessibleCalendar()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.AccessCalendar = PrivacyCriteria.NotAccessible;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 7, "Profile2", "Profile4", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }
        #endregion

        #region SortOrder

        [Test]
        public void TestSortOrder_ByLastLoginDate()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByLastLoginDate;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile4", "Profile2", "Profile3", "Profile5", "Profile7", "Profile6", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestSortOrder_ByVotingCount()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByVotingCount;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile5", "Profile3", "Profile2", "Profile4", "Profile7", "Profile6", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestSortOrder_ByVotingCount_Asc()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByVotingCount;
            criteria.SortAscending = true;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile8", "Profile9", "Profile10", "Profile11", "Profile12" , "Profile6", "Profile7", "Profile2","Profile4", "Profile3", "Profile5");
        }

        [Test]
        public void TestSortOrder_ByLastEntryDate()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByLastEntryDate;
            //criteria.UserSearchGroups.Add(UserSearchGroup.Others);
            //criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile2", "Profile7", "Profile5", "Profile6", "Profile3", "Profile4", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestSortOrder_ByFollowersCount()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByFollowersCount;
            //criteria.UserSearchGroups.Add(UserSearchGroup.Others);
            //criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile2", "Profile4", "Profile6", "Profile3", "Profile5", "Profile7", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestSortOrder_ByTrainingDaysCount()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.SortOrder = UsersSortOrder.ByTrainingDaysCount;
            //criteria.UserSearchGroups.Add(UserSearchGroup.Others);
            //criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResultOrdered(result, "Profile6", "Profile7", "Profile3", "Profile2", "Profile5", "Profile4", "Profile8", "Profile9", "Profile10", "Profile11", "Profile12");
        }

        #endregion

        [Test]
        public void TestUserByName()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserName = profiles[1].UserName;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 1, "Profile2");
        }

        [Test]
        public void TestUserByName_NonSearchable()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserName = profiles[12].UserName;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 1, "Profile13");
        }

        [Test]
        public void TestUsersInFriendsGroup()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 3, "Profile2", "Profile4", "Profile6");
        }

        [Test]
        public void TestUsersInFavoritesGroup()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserSearchGroups.Add(UserSearchGroup.Favorites);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 4, "Profile3", "Profile5", "Profile8", "Profile9");
        }

        [Test]
        public void TestUsersInFavoritesAdnFriendsGroup()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserSearchGroups.Add(UserSearchGroup.Favorites);
            criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 7, "Profile2", "Profile4", "Profile6","Profile3", "Profile5", "Profile8", "Profile9");
        }

        [Test]
        public void TestUsersInOthersGroup()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserSearchGroups.Add(UserSearchGroup.Others);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 4, "Profile7", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestUsersInOthersAndFriendsGroup()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.UserSearchGroups.Add(UserSearchGroup.Others);
            criteria.UserSearchGroups.Add(UserSearchGroup.Friends);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 7,"Profile2", "Profile4", "Profile6", "Profile7", "Profile10", "Profile11", "Profile12");
        }

        [Test]
        public void TestUsersByOneCountry()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Countries.Add(Country.Countries[0].GeoId);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 3,  "Profile2", "Profile3", "Profile4");
        }

        [Test]
        public void TestUsersByTwoCountries()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Countries.Add(Country.Countries[0].GeoId);
            criteria.Countries.Add(Country.Countries[1].GeoId);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 5, "Profile2", "Profile3", "Profile4", "Profile5", "Profile6");
        }

        [Test]
        public void TestUsersByGender()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Genders.Add(Service.Model.Gender.Female);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 6, "Profile2", "Profile3", "Profile5", "Profile8", "Profile10", "Profile12");
        }

        [Test]
        public void TestUsersWithWorkoutPlans()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.WorkoutPlan = UserWorkoutPlanCriteria.OnlyWithWorkoutPlans;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 1, "Profile2");
        }


        [Test]
        public void TestUsersWithPhoto_WithoutDeletedProfiles()
        {
            profiles[3].IsDeleted = true;
            insertToDatabase(profiles[3]);
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Picture = PictureCriteria.OnlyWithPicture;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 4, "Profile2", "Profile5", "Profile9", "Profile12");
        }

        [Test]
        public void TestUsersWithPhoto()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Picture = PictureCriteria.OnlyWithPicture;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 5, "Profile2", "Profile4", "Profile5", "Profile9", "Profile12");
        }

        [Test]
        public void TestUsersWithoutPhoto()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Picture = PictureCriteria.OnlyWithoutPicture;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 6, "Profile3", "Profile6", "Profile7", "Profile8", "Profile10", "Profile11");
        }

        [Test]
        public void TestUsersByGenderAndCountry()
        {
            var criteria = UserSearchCriteria.CreateAllCriteria();
            criteria.Genders.Add(Service.Model.Gender.Female);
            criteria.Countries.Add(Country.Countries[0].GeoId);

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = SecurityManager.CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<UserSearchDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetUsers(data.Token, criteria, pageInfo);
            });
            assertUsersResult(result, 2, "Profile2", "Profile3");
        }

        void assertUsersResult(PagedResult<UserSearchDTO> result, int count, params string[] availableUsers)
        {
            Assert.AreEqual(count,result.AllItemsCount);
            if (availableUsers != null)
            {
                foreach (string availableUser in availableUsers)
                {
                    Assert.IsNotNull(findUser(availableUser, result.Items),
                                     "Cannot find user: " + availableUser);
                }
            }
        }

        void assertUsersResultOrdered(PagedResult<UserSearchDTO> result, params string[] availableUsers)
        {
            Assert.AreEqual(availableUsers.Length, result.AllItemsCount);
            if (availableUsers != null)
            {
                for (int index = 0; index < availableUsers.Length; index++)
                {
                    string availableUser = availableUsers[index];
                    Assert.AreEqual(availableUser,result.Items[index].UserName);
                }
            }
        }
        UserDTO findUser(string name, IList<UserSearchDTO> users)
        {
            return (from plan in users where plan.UserName == name select plan).SingleOrDefault();
        }
    }
}
