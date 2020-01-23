using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model;
using BodyArchitect.Model.Old;
using NUnit.Framework;
using ContentType = BodyArchitect.Model.Old.ContentType;
using FriendInvitation = BodyArchitect.Model.Old.FriendInvitation;
using FriendInvitationType = BodyArchitect.Model.Old.FriendInvitationType;
using MessagePriority = BodyArchitect.Model.Old.MessagePriority;
using MessageType = BodyArchitect.Model.Old.MessageType;
using Picture = BodyArchitect.Model.Old.Picture;
using Privacy = BodyArchitect.Model.Old.Privacy;
using Profile = BodyArchitect.Model.Profile;
using ProfilePrivacy = BodyArchitect.Model.Old.ProfilePrivacy;
using Wymiary = BodyArchitect.Model.Old.Wymiary;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestConvertProfiles : NHibernateTestFixtureBase
    {
        [Test]
        public void ConvertProfile_MainData()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNotNull(dbProfile);
            Assert.IsNotNull(dbProfile.DataInfo);
            Assert.AreEqual(oldProfile.Email,dbProfile.Email);
            Assert.AreEqual(oldProfile.Birthday, dbProfile.Birthday);
            Assert.AreEqual(oldProfile.CreationDate, dbProfile.CreationDate);
            Assert.AreEqual(oldProfile.CountryId, dbProfile.CountryId);
            Assert.AreEqual(oldProfile.Password, dbProfile.Password);
            Assert.AreEqual(AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreNotEqual(DateTime.MinValue, dbProfile.Licence.LastPointOperationDate);
        }

        [Test]
        public void ConvertProfile_DeletedProfile()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.IsDeleted = true;
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNull(dbProfile);
        }

        [Test]
        public void ConvertProfile_Admin()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "Admin";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.Role = Role.Administrator;
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            oldProfile.Privacy.Searchable = false;
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "Admin").SingleOrDefault();
            Assert.IsNotNull(dbProfile);
            Assert.IsNotNull(dbProfile.Licence);
            Assert.AreEqual(AccountType.Administrator, dbProfile.Licence.AccountType);
            Assert.IsFalse(dbProfile.Privacy.Searchable);
        }

        [Test]
        public void ConvertProfile_DefaultMyPlace()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            var defaultMyPlace=SessionNew.QueryOver<MyPlace>().Where(x => x.Profile == dbProfile).SingleOrDefault();
            Assert.IsTrue(defaultMyPlace.IsDefault);
            Assert.IsNull(defaultMyPlace.Address);
            Assert.AreNotEqual(DateTime.MinValue,defaultMyPlace.CreationDate);
        }

        [Test]
        public void ConvertProfile_Privacy()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            oldProfile.Privacy=new ProfilePrivacy();
            oldProfile.Privacy.BirthdayDate = Privacy.Private;
            oldProfile.Privacy.CalendarView = Privacy.FriendsOnly;
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNotNull(dbProfile.Privacy);
            Assert.AreEqual((int)oldProfile.Privacy.BirthdayDate,(int) dbProfile.Privacy.BirthdayDate);
            Assert.AreEqual((int)oldProfile.Privacy.CalendarView, (int)dbProfile.Privacy.CalendarView);
            Assert.AreEqual((int)oldProfile.Privacy.Friends, (int)dbProfile.Privacy.Friends);
        }

        [Test]
        public void ConvertProfile_Settings()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            oldProfile.Settings.NotificationWorkoutPlanVoted = true;
            oldProfile.Settings.NotificationExerciseVoted = true;
            oldProfile.Settings.NotificationBlogCommentAdded = false;
            oldProfile.Settings.NotificationFriendChangedCalendar = true;
            oldProfile.Settings.AutomaticUpdateMeasurements = true;
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNotNull(dbProfile.Settings);
            Assert.AreEqual(ProfileNotification.None, dbProfile.Settings.NotificationBlogCommentAdded);
            Assert.AreEqual(ProfileNotification.Message, dbProfile.Settings.NotificationFriendChangedCalendar);
            Assert.AreEqual(ProfileNotification.Message, dbProfile.Settings.NotificationSocial);
            Assert.AreEqual(ProfileNotification.Message, dbProfile.Settings.NotificationVoted);
            Assert.IsTrue(dbProfile.Settings.AutomaticUpdateMeasurements);
        }

        [Test]
        public void ConvertProfile_Wymiary()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            oldProfile.Wymiary=new Wymiary();
            oldProfile.Wymiary.DateTime = DateTime.Now.Date;
            oldProfile.Wymiary.Height =100;
            oldProfile.Wymiary.IsNaCzczo = true;
            oldProfile.Wymiary.Klatka = 4;
            oldProfile.Wymiary.LeftBiceps = 3;
            oldProfile.Wymiary.LeftForearm = 31;
            oldProfile.Wymiary.LeftUdo = 44;
            oldProfile.Wymiary.Pas = 23;
            oldProfile.Wymiary.RightBiceps = 66;
            oldProfile.Wymiary.RightForearm = 77;
            oldProfile.Wymiary.RightUdo = 21;
            oldProfile.Wymiary.Weight = (float)7.77778E+31;
            insertToOldDatabase(oldProfile);


            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNotNull(dbProfile.Wymiary);
            Assert.AreEqual(oldProfile.Wymiary.DateTime,dbProfile.Wymiary.Time.DateTime);
            Assert.AreEqual(TimeType.OnEmptyStomach, dbProfile.Wymiary.Time.TimeType);
            Assert.AreEqual(oldProfile.Wymiary.Height,(int) dbProfile.Wymiary.Height);
            Assert.AreEqual(oldProfile.Wymiary.LeftBiceps, (float)dbProfile.Wymiary.LeftBiceps);
            Assert.AreEqual(oldProfile.Wymiary.LeftForearm, (float)dbProfile.Wymiary.LeftForearm);
            Assert.AreEqual(oldProfile.Wymiary.LeftUdo, (float)dbProfile.Wymiary.LeftUdo);
            Assert.AreEqual(oldProfile.Wymiary.RightBiceps, (float)dbProfile.Wymiary.RightBiceps);
            Assert.AreEqual(oldProfile.Wymiary.RightForearm, (float)dbProfile.Wymiary.RightForearm);
            Assert.AreEqual(oldProfile.Wymiary.RightUdo, (float)dbProfile.Wymiary.RightUdo);
            Assert.AreEqual(oldProfile.Wymiary.Pas, (float)dbProfile.Wymiary.Pas);
            Assert.AreEqual(oldProfile.Wymiary.Klatka, (float)dbProfile.Wymiary.Klatka);
            Assert.AreEqual(0, (float)dbProfile.Wymiary.Weight);//here we had overflow (very big value)
        }

        [Test]
        public void ConvertProfile_Picture()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            oldProfile.Picture=new Picture(Guid.NewGuid(),"test");
            insertToOldDatabase(oldProfile);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.IsNotNull(dbProfile.Picture);
            Assert.AreEqual(oldProfile.Picture.PictureId,dbProfile.Picture.PictureId);
            Assert.AreEqual(oldProfile.Picture.Hash, dbProfile.Picture.Hash);
        }

        [Test]
        [Ignore("IDs has been changed because I use mappings from the real service so GlobalId of plans are autogenerated!")]
        public void ConvertProfile_FavoritesWorkoutPlans()
        {
            
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            Model.Old.Profile oldProfile3 = CreateProfile("profile3");

            string plan = @"<TrainingPlan>
  <Purpose>Mass</Purpose>
  <Language>pl</Language>
  <Author>jony0008</Author>
  <Comment><![CDATA[]]></Comment>
  <CreationDate>06/05/2011 21:05:23</CreationDate>
  <Difficult>Beginner</Difficult>
  <GlobalId>00c7057a-0694-4b2c-96f7-2c8d0e1cf445</GlobalId>
  <Name>mój plan FBW</Name>
  <RestSeconds>90</RestSeconds>
  <TrainingType>FBW</TrainingType>
  <Url></Url>
  <Days>
    <Day>
      <GlobalId>4cd10e23-170f-4374-b1ae-06e9774752ce</GlobalId>
      <Name>Dzień 1</Name>
      <Entries>
        <Entry>
          <ExerciseId>3e06a130-b811-4e45-9285-f087403615bf</ExerciseId>
          <GlobalId>aaae4427-e354-4323-bb0a-08053cacde18</GlobalId>
          <RestSeconds>90</RestSeconds>
          <Sets>
            <Set>
              <RepetitionNumberMax>12</RepetitionNumberMax>
              <RepetitionNumberMin>12</RepetitionNumberMin>
              <RepetitionsType>Normalna</RepetitionsType>
              <GlobalId>a6e905c6-a8cc-4840-a8d8-6a25dcafadcb</GlobalId>
              <DropSet>None</DropSet>
            </Set>
          </Sets>
        </Entry>
      </Entries>
      <SuperSets />
    </Day>
  </Days>
</TrainingPlan>";

            var oldPlan = CreateWorkoutPlan(oldProfile2, plan, new Guid("00c7057a-0694-4b2c-96f7-2c8d0e1cf445"));
            oldProfile1.FavoriteWorkoutPlans.Add(oldPlan);
            insertToOldDatabase(oldProfile1);

            Convert();

            var list = SessionNew.QueryOver<Profile>().List();
            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();

            Assert.AreEqual(5, list.Count);
            var dbProfile1 = list.Where(x => x.UserName == oldProfile1.UserName).Single();
            var dbProfile2 = list.Where(x => x.UserName == oldProfile2.UserName).Single();
            var dbProfile3 = list.Where(x => x.UserName == oldProfile3.UserName).Single();
            Assert.AreEqual(1, dbProfile1.FavoriteWorkoutPlans.Count);
            Assert.AreEqual(0, dbProfile2.FavoriteWorkoutPlans.Count);
            Assert.AreEqual(0, dbProfile3.FavoriteWorkoutPlans.Count);
            Assert.IsTrue(dbProfile1.FavoriteWorkoutPlans.Contains(newPlan));
        }

        [Test]
        public void ConvertProfile_Favorites()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            Model.Old.Profile oldProfile3 = CreateProfile("profile3");
            oldProfile1.FavoriteUsers.Add(oldProfile2);
            insertToOldDatabase(oldProfile1);

            Convert();


            var list = SessionNew.QueryOver<Profile>().List();

            Assert.AreEqual(5, list.Count);
            var dbProfile1 = list.Where(x => x.UserName == oldProfile1.UserName).Single();
            var dbProfile2 = list.Where(x => x.UserName == oldProfile2.UserName).Single();
            var dbProfile3 = list.Where(x => x.UserName == oldProfile3.UserName).Single();
            Assert.AreEqual(1,dbProfile1.FavoriteUsers.Count);
            Assert.AreEqual(0, dbProfile2.FavoriteUsers.Count);
            Assert.AreEqual(0, dbProfile3.FavoriteUsers.Count);
            Assert.IsTrue(dbProfile1.FavoriteUsers.Contains(dbProfile2));
        }

        [Test]
        public void ConvertProfile_Friends()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            Model.Old.Profile oldProfile3 = CreateProfile("profile3");
            Model.Old.Profile oldProfile4 = CreateProfile("profile4");
            oldProfile1.Friends.Add(oldProfile2);
            oldProfile1.Friends.Add(oldProfile4);
            oldProfile2.Friends.Add(oldProfile1);
            oldProfile4.Friends.Add(oldProfile1);
            insertToOldDatabase(oldProfile1);
            insertToOldDatabase(oldProfile2);
            insertToOldDatabase(oldProfile4);

            Convert();

            var list = SessionNew.QueryOver<Profile>().List();

            Assert.AreEqual(6, list.Count);
            var dbProfile1 = list.Where(x => x.UserName == oldProfile1.UserName).Single();
            var dbProfile2 = list.Where(x => x.UserName == oldProfile2.UserName).Single();
            var dbProfile3 = list.Where(x => x.UserName == oldProfile3.UserName).Single();
            var dbProfile4 = list.Where(x => x.UserName == oldProfile4.UserName).Single();
            Assert.AreEqual(2, dbProfile1.Friends.Count);
            Assert.AreEqual(1, dbProfile2.Friends.Count);
            Assert.AreEqual(0, dbProfile3.Friends.Count);
            Assert.AreEqual(1, dbProfile4.Friends.Count);
            Assert.IsTrue(dbProfile1.Friends.Contains(dbProfile2));
            Assert.IsTrue(dbProfile1.Friends.Contains(dbProfile4));
            Assert.IsTrue(dbProfile2.Friends.Contains(dbProfile1));
            Assert.IsTrue(dbProfile4.Friends.Contains(dbProfile1));
        }

        [Test]
        public void ConvertProfile_Messages_Custom()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");

            Model.Old.Message msg = new Model.Old.Message();
            msg.Content = "Test";
            msg.ContentType = ContentType.Rtf;
            msg.MessageType = MessageType.Custom;
            msg.Priority = MessagePriority.Hight;
            msg.CreatedDate = DateTime.UtcNow.Date;
            msg.Sender = oldProfile1;
            msg.Receiver = oldProfile2;
            msg.Topic = "Topic";

            insertToOldDatabase(msg);

            Convert();

            var newMsg = SessionNew.QueryOver<Model.Message>().Where(x=>x.Priority!=Model.MessagePriority.System).SingleOrDefault();
            Assert.AreEqual(msg.Content, newMsg.Content);
            Assert.AreEqual(msg.CreatedDate, newMsg.CreatedDate);
            Assert.AreEqual((int)msg.Priority, (int)newMsg.Priority);
            Assert.AreEqual(msg.Topic, newMsg.Topic);
            Assert.AreEqual(msg.Sender.UserName, newMsg.Sender.UserName);
            Assert.AreEqual(msg.Receiver.UserName, newMsg.Receiver.UserName);
        }

        [Test]
        public void ConvertProfile_Messages_FriendProfileDeleted()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");

            Model.Old.Message msg = new Model.Old.Message();
            msg.Content = "Test";
            msg.ContentType = ContentType.Rtf;
            msg.MessageType = MessageType.FriendProfileDeleted;
            msg.Priority = MessagePriority.Hight;
            msg.CreatedDate = DateTime.UtcNow.Date;
            msg.Sender = oldProfile1;
            msg.Receiver = oldProfile2;
            msg.Topic = "Topic";

            insertToOldDatabase(msg);

            Convert();

            var newMsg = SessionNew.QueryOver<Model.Message>().Where(x => x.Priority != Model.MessagePriority.System).SingleOrDefault();
            Assert.IsNull(newMsg);
        }

        [Test]
        public void ConvertProfile_Messages_TrainingDayAdded()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");

            Model.Old.Message msg = new Model.Old.Message();
            msg.Content = "Test";
            msg.ContentType = ContentType.Rtf;
            msg.MessageType = MessageType.TrainingDayAdded;
            msg.Priority = MessagePriority.Hight;
            msg.CreatedDate = DateTime.UtcNow.Date;
            msg.Sender = oldProfile1;
            msg.Receiver = oldProfile2;
            msg.Topic = "Topic";

            insertToOldDatabase(msg);

            Convert();

            var newMsg = SessionNew.QueryOver<Model.Message>().Where(x => x.Priority != Model.MessagePriority.System).SingleOrDefault();
            Assert.IsNull(newMsg);
        }

        [Test]
        public void ConvertProfile_Invitations()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            
            Model.Old.FriendInvitation invitation = new FriendInvitation();
            invitation.Invited = oldProfile1;
            invitation.Inviter = oldProfile2;
            invitation.CreateDate = DateTime.UtcNow.AddDays(-1).Date;
            invitation.InvitationType = FriendInvitationType.RejectFriendship;
            invitation.Message = "test";
            insertToOldDatabase(invitation);

            Convert();

            var newInvitation = SessionNew.QueryOver<Model.FriendInvitation>().SingleOrDefault();
            Assert.AreEqual(invitation.CreateDate, newInvitation.CreateDate);
            Assert.AreEqual((int)invitation.InvitationType, (int)newInvitation.InvitationType);
            Assert.AreEqual(invitation.Message, newInvitation.Message);
            Assert.AreEqual(invitation.Invited.UserName, newInvitation.Invited.UserName);
            Assert.AreEqual(invitation.Inviter.UserName, newInvitation.Inviter.UserName);
            
        }

        [Test]
        public void ConvertProfile_Messages_ReveiverIsDeleted()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            oldProfile2.IsDeleted = true;
            insertToOldDatabase(oldProfile2);

            Model.Old.Message msg = new Model.Old.Message();
            msg.Content = "Test";
            msg.ContentType = ContentType.Rtf;
            msg.MessageType = MessageType.FavoriteProfileDeleted;
            msg.Priority = MessagePriority.Hight;
            msg.CreatedDate = DateTime.UtcNow.Date;
            msg.Sender = oldProfile1;
            msg.Receiver = oldProfile2;
            msg.Topic = "Topic";

            insertToOldDatabase(msg);
            Convert();

            var newMsg = SessionNew.QueryOver<Model.Message>().Where(x => x.Priority != Model.MessagePriority.System).RowCount();
            Assert.AreEqual(0,newMsg);
        }

        [Test]
        public void ConvertProfile_Messages_SenderIsDeleted()
        {
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            oldProfile1.IsDeleted = true;
            insertToOldDatabase(oldProfile1);

            Model.Old.Message msg = new Model.Old.Message();
            msg.Content = "Test";
            msg.ContentType = ContentType.Rtf;
            msg.MessageType = MessageType.Custom;
            msg.Priority = MessagePriority.Hight;
            msg.CreatedDate = DateTime.UtcNow.Date;
            msg.Sender = oldProfile1;
            msg.Receiver = oldProfile2;
            msg.Topic = "Topic";

            insertToOldDatabase(msg);
            Convert();

            var newMsg = SessionNew.QueryOver<Model.Message>().Where(x => x.Priority != Model.MessagePriority.System).SingleOrDefault();
            Assert.AreEqual(msg.Content, newMsg.Content);
            Assert.AreEqual(msg.CreatedDate, newMsg.CreatedDate);
            Assert.AreEqual((int)msg.Priority, (int)newMsg.Priority);
            Assert.AreEqual(msg.Topic, newMsg.Topic);
            Assert.AreEqual("(Deleted)", newMsg.Sender.UserName);
            Assert.IsTrue(newMsg.Sender.IsDeleted);
            Assert.AreEqual(msg.Receiver.UserName, newMsg.Receiver.UserName);
        }

        #region Licences

        [Test]
        public void ConvertProfile_NormalUser()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            Convert();
            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.AreEqual(AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(30, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate.Date);
        }

        [Test]
        public void ConvertProfile_FullVersionUsers()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            var wp71 = new Model.Old.LoginData();
            wp71.ProfileId = oldProfile.Id;
            wp71.ApplicationLanguage = "pl";
            wp71.ApplicationVersion = "Full 2.3.0";
            wp71.ClientInstanceId = Guid.NewGuid();
            wp71.LoginDateTime = DateTime.Now.Date;
            wp71.Platform = Model.Old.PlatformType.WindowsPhone;
            wp71.PlatformVersion = "1.";
            insertToOldDatabase(wp71);

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.AreEqual(AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(180, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate.Date);
        }

        [Test]
        public void ConvertProfile_WithMoreThan50LoginData()
        {
            Model.Old.Profile oldProfile = new Model.Old.Profile();
            oldProfile.UserName = "test";
            oldProfile.Email = "gfg@gfg.pl";
            oldProfile.Birthday = DateTime.Now.Date;
            oldProfile.CreationDate = DateTime.Now.Date.AddDays(-1);
            oldProfile.CountryId = 2;
            oldProfile.Password = "test1";
            oldProfile.Privacy = new Model.Old.ProfilePrivacy();
            oldProfile.Settings = new Model.Old.ProfileSettings();
            insertToOldDatabase(oldProfile);

            for (int i = 0; i < 55; i++)
            {
                var wp71 = new Model.Old.LoginData();
                wp71.ProfileId = oldProfile.Id;
                wp71.ApplicationLanguage = "pl";
                wp71.ApplicationVersion = "Free 2.3.0";
                wp71.ClientInstanceId = Guid.NewGuid();
                wp71.LoginDateTime = DateTime.Now.Date.AddDays(-100+i);
                wp71.Platform = Model.Old.PlatformType.WindowsPhone;
                wp71.PlatformVersion = "1.";
                SessionOld.Save(wp71);
            }
            SessionOld.Flush();
            

            Convert();

            var dbProfile = SessionNew.QueryOver<Profile>().Where(x => x.UserName == "test").SingleOrDefault();
            Assert.AreEqual(AccountType.PremiumUser, dbProfile.Licence.AccountType);
            Assert.AreEqual(90, dbProfile.Licence.BAPoints);
            Assert.AreEqual(DateTime.UtcNow.Date, dbProfile.Licence.LastPointOperationDate.Date);
        }
        #endregion
    }
}
