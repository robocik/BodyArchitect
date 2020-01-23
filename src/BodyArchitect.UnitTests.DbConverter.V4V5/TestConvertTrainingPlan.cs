using BodyArchitect.DataAccess.Converter.V4_V5;
using BodyArchitect.Model.Old;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.UnitTests.DbConverter.V4V5
{
    public class TestConvertTrainingPlan : NHibernateTestFixtureBase
    {
        [Test]
        public void ConvertPlan1()
        {
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
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            var oldExercise = CreateExercise("test", new Guid("3e06a130-b811-4e45-9285-f087403615bf"));
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Purpose = WorkoutPlanPurpose.Strength;
            oldPlan.Language = "pl";
            oldPlan.Status = PublishStatus.Published;
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            oldPlan.Difficult = TrainingPlanDifficult.Advanced;
            oldPlan.TrainingType = TrainingType.HST;
            insertToOldDatabase(oldPlan);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            var newProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            Assert.AreEqual("jony0008", newPlan.Author);
            //most data must be taken from the XML content (not from object)
            Assert.AreEqual((int)WorkoutPlanPurpose.Mass, (int)newPlan.Purpose);
            Assert.AreEqual((int)TrainingType.FBW, (int)newPlan.TrainingType);
            Assert.AreEqual((int)TrainingPlanDifficult.Beginner, (int)newPlan.Difficult);
            Assert.AreEqual(DateTime.Parse("06/05/2011 21:05:23"), newPlan.CreationDate);
            Assert.AreEqual("mój plan FBW", newPlan.Name);
            Assert.AreEqual(90, newPlan.RestSeconds);
            Assert.AreEqual("pl", newPlan.Language);
            Assert.AreEqual(Model.PublishStatus.Published, newPlan.Status);
            Assert.AreEqual(oldPlan.PublishDate, newPlan.PublishDate);
            Assert.AreEqual(newProfile, newPlan.Profile);
            Assert.AreEqual(1, newPlan.Days.Count);
            Assert.AreEqual("Dzień 1", newPlan.Days.ElementAt(0).Name);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Position);
            Assert.AreEqual(1, newPlan.Days.ElementAt(0).Entries.Count);
            Assert.AreEqual(oldExercise.GlobalId, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Exercise.GlobalId);
            Assert.AreEqual(BodyArchitect.Model.ExerciseDoneWay.Default, newPlan.Days.ElementAt(0).Entries.ElementAt(0).DoneWay);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Position);
            Assert.AreEqual(90, newPlan.Days.ElementAt(0).Entries.ElementAt(0).RestSeconds);
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(0).GroupName);
            Assert.AreEqual(1, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.Count);
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).Comment);
            Assert.AreEqual(12, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).RepetitionNumberMax);
            Assert.AreEqual(12, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).RepetitionNumberMin);
            Assert.AreEqual((int)SetType.Normalna,(int) newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).DropSet);
        }

        [Test]
        public void ConvertPlan2()
        {
            var oldExercise1 = CreateExercise("test1", new Guid("7b1982b9-a9ea-4d39-84ff-db6764b66ec0"));
            var oldExercise2 = CreateExercise("test2", new Guid("f0ab1656-b94d-4665-9ac9-f02f100f6e8c"));
            var oldExercise3 = CreateExercise("test3", new Guid("2166400e-f37f-464c-8403-c1566952d6e8"));
            var oldExercise4 = CreateExercise("test4", new Guid("841dbc08-8f5c-48ed-a57b-f2fd3a0f2098"));
            var oldExercise5 = CreateExercise("test5", new Guid("d4664345-db3f-4310-8f97-fcf8cac4b2d9"));
            var oldExercise6 = CreateExercise("test6", new Guid("652c3fa6-a0cc-40bc-bbc8-eedb8a422cba"));
            var oldExercise7 = CreateExercise("test7", new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

            string plan = @"<TrainingPlan><Purpose>Mass</Purpose><Language>pl</Language><Author>AnimalPak</Author><Comment><![CDATA[To podstawu]]></Comment>
<CreationDate>03/13/2011 14:28:01</CreationDate><Difficult>Advanced</Difficult><GlobalId>50a0c56e-4683-4f50-8de7-8174fc5c0e5f</GlobalId>
<Name>AnimalPak - Plan Treningowy 10</Name><RestSeconds>0</RestSeconds><TrainingType>Split</TrainingType><Url>http://animalpak.pl/trening/112/plan-treningowy-10.html</Url>
<Days><Day><GlobalId>15d895be-e491-4267-a199-0b66f1be6c12</GlobalId><Name>Dzień 1: klatka, łydki, brzuch</Name>
    <Entries>
    <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>bddac762-b7d2-449b-b31a-1c9221912aab</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionsType>Rozgrzewkowa</RepetitionsType><GlobalId>0dab9901-f87e-44d2-889b-5780f2f4c02e</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>a052112a-646f-4ab4-900c-a28d31606082</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>5faf73a4-ac09-49b8-937e-a83984b4578c</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>80dd873f-9c6e-4069-9c3d-4132454cc798</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>a7af5e72-3978-429d-8454-450caa2c267a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>de2865f4-071f-4171-ad61-0b5c14b43ac5</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>

<Entry><ExerciseId>2166400e-f37f-464c-8403-c1566952d6e8</ExerciseId><GlobalId>d129ea56-49e6-4ae7-a4b0-f2fba86f499d</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>2037d32f-752e-492d-bda9-ebaa0c0b7226</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>64a04b6d-55ba-4892-a6d5-c5f3172a2640</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>1ab94276-f48a-455e-b32b-a85f2912347d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>1c0ab153-a748-42dd-b942-de3c5392c698</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>


    <Entry><Comment><![CDATA[natychmiast po ostatniej serii rozpiętek, jedna seria do upadku mięśniowego.]]></Comment><ExerciseId>841dbc08-8f5c-48ed-a57b-f2fd3a0f2098</ExerciseId><GlobalId>61871d76-2133-4fdf-8ac1-076e3d0c1d7e</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>12</RepetitionNumberMax><RepetitionNumberMin>12</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>34240243-59fb-4ebf-8476-78c37f51c4b3</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e6709951-9e4f-4398-a7e1-bb5bb2bde5ab</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>79eb2878-362f-4efc-85cb-881c74b50e97</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    <Entry><Comment><![CDATA[natychmiast po ostatniej serii rozpiętek, jedna seria do upadku mięśniowego.]]></Comment><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>56f36ef9-0c10-4971-8866-535c38fa7280</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionsType>MuscleFailure</RepetitionsType><GlobalId>2acacbda-8033-4489-b479-91892dbf96ec</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    <Entry><ExerciseId>d4664345-db3f-4310-8f97-fcf8cac4b2d9</ExerciseId><GlobalId>19586eec-c634-4180-8c38-167e4d606ecf</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>20</RepetitionNumberMax><RepetitionNumberMin>20</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>33db170e-52f5-4f93-afc7-cf2518e65f0d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>86ef20fd-edcd-4a3d-83a8-82381b7568e4</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>a54b661f-e43c-45d9-bd7f-1c02e248b991</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>12</RepetitionNumberMax><RepetitionNumberMin>12</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>79d970fb-72a5-468f-b866-61b7e93c5b7e</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    </Entries><SuperSets /></Day>
<Day><GlobalId>5fe59d58-6589-4330-9678-b6fd23b97b53</GlobalId><Name>Dzień 2: nogi</Name><Entries>
    <Entry><Comment><![CDATA[15 minut rozgrzewki]]></Comment><ExerciseId>652c3fa6-a0cc-40bc-bbc8-eedb8a422cba</ExerciseId><GlobalId>90e69098-3f52-4ae5-8c78-b697259d59bf</GlobalId><RestSeconds>0</RestSeconds><Sets /></Entry>
    <Entry><ExerciseId>3e06a130-b811-4e45-9285-f087403615bf</ExerciseId><GlobalId>061e4eb6-5de0-4314-a1a1-ccb361867c8a</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>12</RepetitionNumberMax><RepetitionNumberMin>12</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>82318b87-7d3b-4279-b6fb-9f8c3ede408b</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>c510a341-c2bd-4ec5-9ea8-a167b83b2e7b</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>dc50ffd7-5b63-410d-be50-623f405630a2</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>b9ea336b-655c-45f7-9ee7-6c8fff5cd31d</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>5bd358a6-4f74-4ecc-b692-6de40067e266</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>7b53d02a-1b2b-454c-b99c-7ac13955fb42</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>8f48d905-3955-48d0-80bc-fca41eca156a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>de8a2305-66f5-4376-9473-6af7b147dae1</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>6</RepetitionNumberMax><RepetitionNumberMin>6</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e7c93322-6cec-4879-8719-660af3d9418b</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
    <Entry><ExerciseId>2166400e-f37f-464c-8403-c1566952d6e8</ExerciseId><GlobalId>8993ce50-c1bd-4643-9695-c7583144dc89</GlobalId><RestSeconds>0</RestSeconds>
        <Sets><Set><RepetitionNumberMax>10</RepetitionNumberMax><RepetitionNumberMin>10</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>b1538b3c-bb8e-4da6-a4bd-1ce0665d1e76</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>088aeba6-7e6e-4af5-b6bb-95b8ffc19f26</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>8</RepetitionNumberMax><RepetitionNumberMin>8</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>af642e9d-0e9b-46aa-a6c3-73fc66af15fb</GlobalId><DropSet>None</DropSet></Set></Sets></Entry></Entries><SuperSets /></Day></Days></TrainingPlan>";
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Purpose = WorkoutPlanPurpose.Strength;
            oldPlan.Language = "pl";
            oldPlan.Status = PublishStatus.Private;
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            oldPlan.Difficult = TrainingPlanDifficult.Advanced;
            oldPlan.TrainingType = TrainingType.HST;
            insertToOldDatabase(oldPlan);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            var newProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            Assert.AreEqual("AnimalPak", newPlan.Author);
            Assert.AreEqual("pl", newPlan.Language);
            //most data must be taken from the XML content (not from object)
            Assert.AreEqual((int)WorkoutPlanPurpose.Mass, (int)newPlan.Purpose);
            Assert.AreEqual((int)TrainingType.Split, (int)newPlan.TrainingType);
            Assert.IsNotNull(newPlan.Comment);
            Assert.IsNull(newPlan.PublishDate);
            Assert.AreEqual((int)TrainingPlanDifficult.Advanced, (int)newPlan.Difficult);
            Assert.AreEqual(DateTime.Parse("03/13/2011 14:28:01"), newPlan.CreationDate);
            Assert.AreEqual("AnimalPak - Plan Treningowy 10", newPlan.Name);
            Assert.AreEqual("http://animalpak.pl/trening/112/plan-treningowy-10.html", newPlan.Url);
            Assert.AreEqual(0, newPlan.RestSeconds);
            Assert.AreEqual(Model.PublishStatus.Private, newPlan.Status);
            Assert.AreEqual(newProfile, newPlan.Profile);
            Assert.AreEqual(2, newPlan.Days.Count);

            //day 1
            Assert.AreEqual("Dzień 1: klatka, łydki, brzuch", newPlan.Days.ElementAt(0).Name);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Position);
            Assert.AreEqual(6, newPlan.Days.ElementAt(0).Entries.Count);
            //day 1 entry 1
            Assert.AreEqual(oldExercise1.GlobalId, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Exercise.GlobalId);
            Assert.AreEqual(BodyArchitect.Model.ExerciseDoneWay.Default, newPlan.Days.ElementAt(0).Entries.ElementAt(0).DoneWay);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Position);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Entries.ElementAt(0).RestSeconds);
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(0).GroupName);
            Assert.AreEqual(1, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.Count);
            //day 1 entry 1 set 1
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).Comment);
            Assert.AreEqual((int)SetType.Rozgrzewkowa, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(0).Sets.ElementAt(0).DropSet);

            //day 1 entry 2
            Assert.AreEqual(oldExercise2.GlobalId, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Exercise.GlobalId);
            Assert.AreEqual(BodyArchitect.Model.ExerciseDoneWay.Default, newPlan.Days.ElementAt(1).Entries.ElementAt(0).DoneWay);
            Assert.AreEqual(1, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Position);
            Assert.AreEqual(0, newPlan.Days.ElementAt(0).Entries.ElementAt(1).RestSeconds);
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(1).GroupName);
            Assert.AreEqual(4, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.Count);

            //day 1 entry 2 set 1
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(0).Comment);
            Assert.AreEqual(10, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(0).RepetitionNumberMax);
            Assert.AreEqual(10, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(0).RepetitionNumberMin);
            Assert.AreEqual((int)SetType.Normalna, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(0).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(0).DropSet);

            //day 1 entry 2 set 2
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(1).Comment);
            Assert.AreEqual(8, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(1).RepetitionNumberMax);
            Assert.AreEqual(8, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(1).RepetitionNumberMin);
            Assert.AreEqual((int)SetType.Normalna, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(1).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(1).DropSet);

            //day 1 entry 2 set 3
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(2).Comment);
            Assert.AreEqual(6, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(2).RepetitionNumberMax);
            Assert.AreEqual(6, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(2).RepetitionNumberMin);
            Assert.AreEqual((int)SetType.Normalna, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(2).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(2).DropSet);

            //day 1 entry 2 set 4
            Assert.AreEqual(null, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(3).Comment);
            Assert.AreEqual(6, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(3).RepetitionNumberMax);
            Assert.AreEqual(6, newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(3).RepetitionNumberMin);
            Assert.AreEqual((int)SetType.Normalna, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(3).RepetitionsType);
            Assert.AreEqual((int)DropSetType.None, (int)newPlan.Days.ElementAt(0).Entries.ElementAt(1).Sets.ElementAt(3).DropSet);

            //day 1 entry 3
            Assert.AreEqual(oldExercise3.GlobalId, newPlan.Days.ElementAt(0).Entries.ElementAt(2).Exercise.GlobalId);
            Assert.AreEqual(2, newPlan.Days.ElementAt(0).Entries.ElementAt(2).Position);
            Assert.AreEqual(4, newPlan.Days.ElementAt(0).Entries.ElementAt(2).Sets.Count);

            //day 2
            var day = newPlan.Days.ElementAt(1);
            Assert.AreEqual("Dzień 2: nogi", day.Name);
            Assert.AreEqual(1, day.Position);
            Assert.AreEqual(4, day.Entries.Count);
            //day 2 entry 1
            var entry = day.Entries.ElementAt(0);
            Assert.AreEqual(oldExercise6.GlobalId, entry.Exercise.GlobalId);
            Assert.AreEqual(0, entry.Position);
            Assert.AreEqual(0, entry.Sets.Count);
            Assert.IsNotNull(entry.Comment);

            //day 2 entry 2
            entry = day.Entries.ElementAt(1);
            Assert.AreEqual(oldExercise7.GlobalId, entry.Exercise.GlobalId);
            Assert.AreEqual(1, entry.Position);
            Assert.AreEqual(4, entry.Sets.Count);
        }

        [Test]
        public void ConvertPlan3_WithSuperSets()
        {
            var oldExercise1 = CreateExercise("test1", new Guid("7b1982b9-a9ea-4d39-84ff-db6764b66ec0"));
            var oldExercise2 = CreateExercise("test2", new Guid("f0ab1656-b94d-4665-9ac9-f02f100f6e8c"));

            string plan = @"<TrainingPlan><Purpose>NotSet</Purpose><Language>pl</Language><Author>BodyArchitect</Author><Comment><![CDATA[]]></Comment><CreationDate>12/09/2010 07:01:14</CreationDate><Difficult>Advanced</Difficult><GlobalId>e8ec0028-a9a8-4c9f-a40f-8fb4aea43372</GlobalId><Name>Plan na wytrzymałość</Name><RestSeconds>0</RestSeconds><TrainingType>Split</TrainingType><Url>http://blog.myba.tk/?p=36</Url>
<Days>
    <Day><GlobalId>3ed3deaf-ef7b-40f5-9c0e-4e96565306a6</GlobalId><Name>Dzień 1</Name><Entries>
        <Entry><Comment><![CDATA[Kąt 45 stopni]]></Comment><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>e2a4198f-7cd9-443c-8e44-1605277e1afa</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>67333bfe-771c-452e-8e89-c9ff128712b1</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>2f313788-1126-4621-8081-d2ae52fb6793</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>3ea05350-96a4-4022-9068-440fc327dda2</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>3194c203-4083-462d-b7df-2564c195041f</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>83ccef8a-6e42-4c74-9b5f-5942c606bfb5</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>972e725c-cee7-4308-bd60-90d3a87a123d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e66af240-44fe-4774-9dec-4f65eb853d19</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>9f09f3dc-c2b6-433d-a05d-658b8a81276a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>d189ff4d-9f09-4b05-8b2b-dae54ecf6acd</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><Comment><![CDATA[Kąt 45 stopni]]></Comment><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>48984fa0-aab7-4314-b193-b6158b67e548</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>5ec2ef5f-6fc6-409f-8011-987a63f47667</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>1f9f0114-a4e7-42df-814d-adaebba669e7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>bf18315d-e0be-4b4d-aa0d-c07ab0604b37</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>2bfde702-cc44-4c81-9686-9dfe8608eaa8</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>4c4d8820-755f-4e3c-abeb-3434703845df</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e838b51d-d6d7-47e9-887a-06ba662bf40f</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>dfff566a-cb6c-4e1c-8de5-6fdb5c95b1ed</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>ea283fb2-b11a-4995-a301-e2fa42752556</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f133b0b3-ef1c-4c81-ab60-a140b7407e33</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>84581d77-5274-4f3b-99dc-b3b0766d505f</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f8fd5a6e-d8c1-4148-8e96-7aa17d9b975e</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>319899bb-0a7e-4783-9074-125e8299c8b9</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>7945faf9-acd2-4266-bd01-8eedae6749f7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>ff0b56c6-362c-42a4-8b2a-f41cccb7f73a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>d31ba4a6-d1de-40cc-9b84-5cf678f0eb16</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>c110f30b-82e6-4ea1-b2f8-1cac3b32bf96</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f6ea1a8d-c671-4f8c-af6b-ff5d1a9410b7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>9f01080f-1d67-41e0-821f-c7dd3320eb4d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>528425be-97bb-4ec9-9609-db04f378eea5</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>6e2167a7-f41d-4b82-b968-08068d2c70fb</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>ee609440-4a69-4562-b05d-cb2a202f35ef</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>99f8a670-f748-430c-b503-c92d6206273d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>82dd9dd0-07f5-45b9-bf94-45fcc9ca5ff5</GlobalId><DropSet>None</DropSet></Set></Sets></Entry></Entries>
        <SuperSets>
            <SuperSet><SuperSetId>ba85dcbd-589e-49cc-94ad-29e4b07db7e3</SuperSetId>    <Entries>
                <EntryId>ea283fb2-b11a-4995-a301-e2fa42752556</EntryId>
                <EntryId>319899bb-0a7e-4783-9074-125e8299c8b9</EntryId></Entries></SuperSet></SuperSets></Day></Days></TrainingPlan>";
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Language = "pl";
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            insertToOldDatabase(oldPlan);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            var newProfile = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            Assert.AreEqual(newProfile, newPlan.Profile);
            Assert.AreEqual(1, newPlan.Days.Count);
            
            var day = newPlan.Days.ElementAt(0);
            Assert.AreEqual(8, day.Entries.Count);
            var superSetEntries = newPlan.Days.SelectMany(x => x.Entries).Where(x => !string.IsNullOrEmpty(x.GroupName)).ToList();
            Assert.AreEqual(2, superSetEntries.Count);
            var firstEntryInSuperSet=day.Entries.Where(x => x.Position == 4).Single();
            var secondEntryInSuperSet = day.Entries.Where(x => x.Position == 5).Single();
            Assert.AreEqual(firstEntryInSuperSet.GroupName,secondEntryInSuperSet.GroupName);

            Assert.IsNotNull(day.Entries.ElementAt(0).Comment);
            Assert.IsNotNull(day.Entries.ElementAt(2).Comment);
        }

        [Test]
        [Ignore("IDs has been changed because I use mappings from the real service so GlobalId of plans are autogenerated!")]
        public void ConvertPlan3_TakeOldIds()
        {
            var oldExercise1 = CreateExercise("test1", new Guid("7b1982b9-a9ea-4d39-84ff-db6764b66ec0"));
            var oldExercise2 = CreateExercise("test2", new Guid("f0ab1656-b94d-4665-9ac9-f02f100f6e8c"));

            string plan = @"<TrainingPlan><Purpose>NotSet</Purpose><Language>pl</Language><Author>BodyArchitect</Author><Comment><![CDATA[]]></Comment><CreationDate>12/09/2010 07:01:14</CreationDate><Difficult>Advanced</Difficult><GlobalId>e8ec0028-a9a8-4c9f-a40f-8fb4aea43372</GlobalId><Name>Plan na wytrzymałość</Name><RestSeconds>0</RestSeconds><TrainingType>Split</TrainingType><Url>http://blog.myba.tk/?p=36</Url>
<Days>
    <Day><GlobalId>3ed3deaf-ef7b-40f5-9c0e-4e96565306a6</GlobalId><Name>Dzień 1</Name><Entries>
        <Entry><Comment><![CDATA[Kąt 45 stopni]]></Comment><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>e2a4198f-7cd9-443c-8e44-1605277e1afa</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>67333bfe-771c-452e-8e89-c9ff128712b1</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>2f313788-1126-4621-8081-d2ae52fb6793</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>3ea05350-96a4-4022-9068-440fc327dda2</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>3194c203-4083-462d-b7df-2564c195041f</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>83ccef8a-6e42-4c74-9b5f-5942c606bfb5</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>972e725c-cee7-4308-bd60-90d3a87a123d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e66af240-44fe-4774-9dec-4f65eb853d19</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>9f09f3dc-c2b6-433d-a05d-658b8a81276a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>d189ff4d-9f09-4b05-8b2b-dae54ecf6acd</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><Comment><![CDATA[Kąt 45 stopni]]></Comment><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>48984fa0-aab7-4314-b193-b6158b67e548</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>5ec2ef5f-6fc6-409f-8011-987a63f47667</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>1f9f0114-a4e7-42df-814d-adaebba669e7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>bf18315d-e0be-4b4d-aa0d-c07ab0604b37</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>2bfde702-cc44-4c81-9686-9dfe8608eaa8</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>4c4d8820-755f-4e3c-abeb-3434703845df</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>e838b51d-d6d7-47e9-887a-06ba662bf40f</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>dfff566a-cb6c-4e1c-8de5-6fdb5c95b1ed</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>7b1982b9-a9ea-4d39-84ff-db6764b66ec0</ExerciseId><GlobalId>ea283fb2-b11a-4995-a301-e2fa42752556</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f133b0b3-ef1c-4c81-ab60-a140b7407e33</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>84581d77-5274-4f3b-99dc-b3b0766d505f</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f8fd5a6e-d8c1-4148-8e96-7aa17d9b975e</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>319899bb-0a7e-4783-9074-125e8299c8b9</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>7945faf9-acd2-4266-bd01-8eedae6749f7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>ff0b56c6-362c-42a4-8b2a-f41cccb7f73a</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>d31ba4a6-d1de-40cc-9b84-5cf678f0eb16</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>c110f30b-82e6-4ea1-b2f8-1cac3b32bf96</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>f6ea1a8d-c671-4f8c-af6b-ff5d1a9410b7</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>9f01080f-1d67-41e0-821f-c7dd3320eb4d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>528425be-97bb-4ec9-9609-db04f378eea5</GlobalId><DropSet>None</DropSet></Set></Sets></Entry>
        <Entry><ExerciseId>f0ab1656-b94d-4665-9ac9-f02f100f6e8c</ExerciseId><GlobalId>6e2167a7-f41d-4b82-b968-08068d2c70fb</GlobalId><RestSeconds>0</RestSeconds><Sets><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>ee609440-4a69-4562-b05d-cb2a202f35ef</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>99f8a670-f748-430c-b503-c92d6206273d</GlobalId><DropSet>None</DropSet></Set><Set><RepetitionNumberMax>15</RepetitionNumberMax><RepetitionNumberMin>15</RepetitionNumberMin><RepetitionsType>Normalna</RepetitionsType><GlobalId>82dd9dd0-07f5-45b9-bf94-45fcc9ca5ff5</GlobalId><DropSet>None</DropSet></Set></Sets></Entry></Entries>
        <SuperSets>
            <SuperSet><SuperSetId>ba85dcbd-589e-49cc-94ad-29e4b07db7e3</SuperSetId>    <Entries>
                <EntryId>ea283fb2-b11a-4995-a301-e2fa42752556</EntryId>
                <EntryId>319899bb-0a7e-4783-9074-125e8299c8b9</EntryId></Entries></SuperSet></SuperSets></Day></Days></TrainingPlan>";
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.GlobalId = new Guid("e8ec0028-a9a8-4c9f-a40f-8fb4aea43372");
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Language = "pl";
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            insertToOldDatabase(oldPlan);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            Assert.AreEqual(oldPlan.GlobalId, newPlan.GlobalId);
            var day = newPlan.Days.ElementAt(0);
            Assert.AreEqual(new Guid("3ed3deaf-ef7b-40f5-9c0e-4e96565306a6"), day.GlobalId);
            Assert.AreEqual(new Guid("e2a4198f-7cd9-443c-8e44-1605277e1afa"), day.Entries.ElementAt(0).GlobalId);
            Assert.AreEqual(new Guid("67333bfe-771c-452e-8e89-c9ff128712b1"), day.Entries.ElementAt(0).Sets.ElementAt(0).GlobalId);
        }

        [Test]
        public void ConvertPlan_WithDeletedExercise()
        {
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
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.Purpose = WorkoutPlanPurpose.Strength;
            oldPlan.Language = "pl";
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            oldPlan.Difficult = TrainingPlanDifficult.Advanced;
            oldPlan.TrainingType = TrainingType.HST;
            insertToOldDatabase(oldPlan);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            var deletedExercise=SessionNew.QueryOver<Model.Exercise>().Where(x => x.IsDeleted).SingleOrDefault();
            Assert.AreEqual(deletedExercise.GlobalId, newPlan.Days.ElementAt(0).Entries.ElementAt(0).Exercise.GlobalId);
            
        }

        [Test]
        [Ignore("IDs has been changed because I use mappings from the real service so GlobalId of plans are autogenerated!")]
        public void ConvertPlan_Ratings()
        {
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
            Model.Old.Profile oldProfile1 = CreateProfile("profile1");
            Model.Old.Profile oldProfile2 = CreateProfile("profile2");
            var oldExercise = CreateExercise("test", new Guid("3e06a130-b811-4e45-9285-f087403615bf"));
            Model.Old.TrainingPlan oldPlan = new TrainingPlan();
            oldPlan.GlobalId = new Guid("00c7057a-0694-4b2c-96f7-2c8d0e1cf445");
            oldPlan.Profile = oldProfile1;
            oldPlan.PlanContent = plan;
            oldPlan.PublishDate = DateTime.Now.Date;
            oldPlan.Purpose = WorkoutPlanPurpose.Strength;
            oldPlan.Language = "pl";
            oldPlan.Status = PublishStatus.Published;
            oldPlan.Name = "yyyyy";
            oldPlan.Author = "Roemk";
            oldPlan.CreationDate = DateTime.UtcNow.Date.AddHours(1);
            oldPlan.Difficult = TrainingPlanDifficult.Advanced;
            oldPlan.TrainingType = TrainingType.HST;
            insertToOldDatabase(oldPlan);

            RatingUserValue rating = new RatingUserValue();
            rating.ProfileId = oldProfile1.Id;
            rating.Rating = 3;
            rating.RatedObjectId = oldPlan.GlobalId;
            rating.ShortComment = "Comment";
            rating.VotedDate = DateTime.Now.Date;
            insertToOldDatabase(rating);

            rating = new RatingUserValue();
            rating.ProfileId = oldProfile2.Id;
            rating.Rating = 5;
            rating.RatedObjectId = oldPlan.GlobalId;
            rating.ShortComment = "Comment111";
            rating.VotedDate = DateTime.Now.Date;
            insertToOldDatabase(rating);

            //another plan
            rating = new RatingUserValue();
            rating.ProfileId = oldProfile2.Id;
            rating.Rating = 1;
            rating.RatedObjectId = Guid.NewGuid();
            rating.ShortComment = "Comment111";
            rating.VotedDate = DateTime.Now.Date;
            insertToOldDatabase(rating);

            Convert();

            var newPlan = SessionNew.QueryOver<Model.TrainingPlan>().SingleOrDefault();
            Assert.AreEqual(4, newPlan.Rating);
            var ratings = SessionNew.QueryOver<Model.RatingUserValue>().Where(x=>x.RatedObjectId==newPlan.GlobalId).List();
            Assert.AreEqual(2, ratings.Count);
            var rating1 = ratings.Where(x => x.Rating == 3).Single();
            Assert.AreEqual("Comment", rating1.ShortComment);
            Assert.AreEqual(DateTime.Now.Date, rating1.VotedDate);
            Assert.AreEqual(newPlan.GlobalId, rating1.RatedObjectId);
            var newProfile1 = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile1").SingleOrDefault();
            Assert.AreEqual(newProfile1.GlobalId, rating1.ProfileId);


            var rating2 = ratings.Where(x => x.Rating == 5).Single();
            Assert.AreEqual("Comment111", rating2.ShortComment);
            Assert.AreEqual(DateTime.Now.Date, rating2.VotedDate);
            Assert.AreEqual(newPlan.GlobalId, rating2.RatedObjectId);
            var newProfile2 = SessionNew.QueryOver<Model.Profile>().Where(x => x.UserName == "profile2").SingleOrDefault();
            Assert.AreEqual(newProfile2.GlobalId, rating2.ProfileId);
        }
    }
}
