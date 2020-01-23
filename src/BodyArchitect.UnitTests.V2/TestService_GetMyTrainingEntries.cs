using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NUnit.Framework;
using Privacy = BodyArchitect.Model.Privacy;

namespace BodyArchitect.UnitTests.V2
{
    //[TestFixture]
    //public class TestService_GetMyTrainingEntries : TestServiceBase
    //{
    //    List<Profile> profiles = new List<Profile>();
    //    private APIKey apiKey;
    //    private Guid key = Guid.NewGuid();
    //    List<Customer> customers = new List<Customer>();

    //    public override void BuildDatabase()
    //    {
    //        using (var tx = Session.BeginTransaction())
    //        {
    //            profiles.Clear();
    //            customers.Clear();
    //            profiles.Add(CreateProfile(Session, "test1"));
    //            profiles.Add(CreateProfile(Session, "test2"));
    //            profiles.Add(CreateProfile(Session, "test3"));

    //            //set friendship
    //            profiles[0].Friends.Add(profiles[1]);
    //            profiles[1].Friends.Add(profiles[0]);
    //            Session.Update(profiles[0]);
    //            Session.Update(profiles[1]);

    //            var customer = CreateCustomer("Cust1", profiles[0]);
    //            customers.Add(customer);
    //            customer = CreateCustomer("Cust2", profiles[0]);
    //            customers.Add(customer);
    //            customer = CreateCustomer("Cust3", profiles[1]);
    //            customers.Add(customer);

    //            apiKey = new APIKey();
    //            apiKey.ApiKey = key;
    //            apiKey.ApplicationName = "UnitTest";
    //            apiKey.EMail = "mail@mail.com";
    //            apiKey.RegisterDateTime = DateTime.UtcNow;
    //            insertToDatabase(apiKey);
    //            tx.Commit();
    //        }
    //    }

    //    private void setPrivacy(Privacy newPrivacy)
    //    {
    //        profiles[0].Privacy.CalendarView = newPrivacy;
    //        Session.Update(profiles[0]);
    //        Session.Flush();
    //        Session.Clear();
    //    }

    //    [Test]
    //    public void GetMyTrainingEntries()
    //    {
    //        var profile = (ProfileDTO)profiles[0].Tag;
    //        SessionData data = CreateNewSession(profile, ClientInformation);
    //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
    //        day.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w = new A6WEntryDTO();
    //        a6w.Day = A6WManager.Days[0];
    //        a6w.Completed = true;
    //        a6w.MyTraining = new A6WTrainingDTO();
    //        a6w.MyTraining.Name = "name";
    //        day.AddEntry(a6w);

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day = Service.SaveTrainingDay(data.Token, day);
    //        });
    //        TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
    //        day1.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w1 = new A6WEntryDTO();
    //        a6w1.Day = A6WManager.Days[1];
    //        a6w1.Completed = true;
    //        a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
    //        a6w1.MyTraining.Name = "name";
    //        day1.AddEntry(a6w1);


    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day1 = Service.SaveTrainingDay(data.Token, day1);
    //        });

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var entries = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(2,entries.Count);
    //        });
            
    //    }

    //    [Test]
    //    public void GetMyTrainingEntries_Private()
    //    {
    //        setPrivacy(Privacy.Private);
    //        var profile = (ProfileDTO)profiles[0].Tag;
    //        SessionData data = CreateNewSession(profile, ClientInformation);
    //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
    //        day.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w = new A6WEntryDTO();
    //        a6w.Day = A6WManager.Days[0];
    //        a6w.Completed = true;
    //        a6w.MyTraining = new A6WTrainingDTO();
    //        a6w.MyTraining.Name = "name";
    //        day.AddEntry(a6w);

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day = Service.SaveTrainingDay(data.Token, day);
    //        });
    //        TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
    //        day1.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w1 = new A6WEntryDTO();
    //        a6w1.Day = A6WManager.Days[1];
    //        a6w1.Completed = true;
    //        a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
    //        a6w1.MyTraining.Name = "name";
    //        day1.AddEntry(a6w1);


    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day1 = Service.SaveTrainingDay(data.Token, day1);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list=Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(0,list.Count);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(0, list.Count);
    //        });
    //    }

    //    [Test]
    //    public void GetMyTrainingEntries_FriendsOnly()
    //    {
    //        setPrivacy(Privacy.FriendsOnly);
    //        var profile = (ProfileDTO)profiles[0].Tag;
    //        SessionData data = CreateNewSession(profile, ClientInformation);
    //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
    //        day.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w = new A6WEntryDTO();
    //        a6w.Day = A6WManager.Days[0];
    //        a6w.Completed = true;
    //        a6w.MyTraining = new A6WTrainingDTO();
    //        a6w.MyTraining.Name = "name";
    //        day.AddEntry(a6w);

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day = Service.SaveTrainingDay(data.Token, day);
    //        });
    //        TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
    //        day1.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w1 = new A6WEntryDTO();
    //        a6w1.Day = A6WManager.Days[1];
    //        a6w1.Completed = true;
    //        a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
    //        a6w1.MyTraining.Name = "name";
    //        day1.AddEntry(a6w1);


    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day1 = Service.SaveTrainingDay(data.Token, day1);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(2, list.Count);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(0, list.Count);
    //        });
    //    }

    //    [Test]
    //    public void GetMyTrainingEntries_Public()
    //    {
    //        setPrivacy(Privacy.Public);
    //        var profile = (ProfileDTO)profiles[0].Tag;
    //        SessionData data = CreateNewSession(profile, ClientInformation);
    //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
    //        day.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w = new A6WEntryDTO();
    //        a6w.Day = A6WManager.Days[0];
    //        a6w.Completed = true;
    //        a6w.MyTraining = new A6WTrainingDTO();
    //        a6w.MyTraining.Name = "name";
    //        day.AddEntry(a6w);

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day = Service.SaveTrainingDay(data.Token, day);
    //        });
    //        TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
    //        day1.ProfileId = profile.GlobalId;
    //        A6WEntryDTO a6w1 = new A6WEntryDTO();
    //        a6w1.Day = A6WManager.Days[1];
    //        a6w1.Completed = true;
    //        a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
    //        a6w1.MyTraining.Name = "name";
    //        day1.AddEntry(a6w1);


    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day1 = Service.SaveTrainingDay(data.Token, day1);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(2, list.Count);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[2].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            var list = Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //            Assert.AreEqual(2, list.Count);
    //        });
    //    }

    //    [Test]
    //    [ExpectedException(typeof(CrossProfileOperationException))]
    //    public void ForCustomer_GetMyTrainingEntries_CustomerFromAnotherProfile()
    //    {
    //        setPrivacy(Privacy.Public);
    //        var profile = (ProfileDTO)profiles[0].Tag;
    //        SessionData data = CreateNewSession(profile, ClientInformation);
    //        TrainingDayDTO day = new TrainingDayDTO(DateTime.Now);
    //        day.ProfileId = profile.GlobalId;
    //        day.CustomerId = customers[0].GlobalId;
    //        A6WEntryDTO a6w = new A6WEntryDTO();
    //        a6w.Day = A6WManager.Days[0];
    //        a6w.Completed = true;
    //        a6w.MyTraining = new A6WTrainingDTO();
    //        a6w.MyTraining.CustomerId = customers[0].GlobalId;
    //        a6w.MyTraining.Name = "name";
    //        day.AddEntry(a6w);

    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day = Service.SaveTrainingDay(data.Token, day);
    //        });
    //        TrainingDayDTO day1 = new TrainingDayDTO(DateTime.Now.AddDays(1));
    //        day1.ProfileId = profile.GlobalId;
    //        day1.CustomerId = customers[0].GlobalId;
    //        A6WEntryDTO a6w1 = new A6WEntryDTO();
    //        a6w1.Day = A6WManager.Days[1];
    //        a6w1.Completed = true;
    //        a6w1.MyTraining = day.Objects.ElementAt(0).MyTraining;
    //        a6w1.MyTraining.Name = "name";
    //        day1.AddEntry(a6w1);


    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            day1 = Service.SaveTrainingDay(data.Token, day1);
    //        });

    //        data = CreateNewSession((ProfileDTO)profiles[1].Tag, ClientInformation);
    //        RunServiceMethod(delegate(InternalBodyArchitectService Service)
    //        {
    //            Service.GetMyTrainingEntries(data.Token, a6w1.MyTraining);
    //        });

    //    }


    //}
}
