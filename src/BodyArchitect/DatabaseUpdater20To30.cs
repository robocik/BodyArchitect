using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using BodyArchitect.A6W.Model;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Size.Model;
using BodyArchitect.StrengthTraining.Model;
using NHibernate;

namespace BodyArchitect
{
    class DatabaseUpdater20To30Schema : DatabaseUpdaterBase
    {
        public DatabaseVersion Upgrade(string oldDbFile)
        {
            using (var scope = new TransactionScope())
            {
                Log.WriteInfo("DatabaseUpdater20To30. OldFile={0}", oldDbFile);
                if (File.Exists(oldDbFile))
                {
                    Log.WriteInfo("File exists. Start converting db from 2.0.0.0 to 3.0.0.0");
                    SqlCeConnection oldConnection = new SqlCeConnection(string.Format("Data Source={0};", oldDbFile));
                    Log.WriteInfo("Start migration");
                    oldConnection.Open();
                    Log.WriteInfo("Connection opened");
                    SqlCeCommand cmd = new SqlCeCommand();
                    cmd.Connection = oldConnection;
                    cmd.CommandText = "ALTER TABLE A6WEntry ADD   MyTraining_id uniqueidentifier";
                }

                Log.WriteInfo("Set database version to 3.0.0.0");
                DatabaseVersion version = new DatabaseVersion("3.0.0.0");
                version.Create();
                scope.VoteCommit();
                Log.WriteInfo("DatabaseUpdater20To30. Everything is ok");
                return version;
            }
        }

    }

    class DatabaseUpdater20To30 : DatabaseUpdaterBase
    {
        public DatabaseVersion Upgrade(string oldDbFile)
        {
            using (var scope=new TransactionScope())
            {

                Log.WriteInfo("DatabaseUpdater20To30. OldFile={0}", oldDbFile);
                if (File.Exists(oldDbFile))
                {
                    Log.WriteInfo("File exists. Start converting db from 2.0.0.0 to 3.0.0.0");
                    importProfiles(oldDbFile);
                }
                Log.WriteInfo("Set database version to 3.0.0.0");
                DatabaseVersion version = new DatabaseVersion("3.0.0.0");
                version.Create();
                scope.VoteCommit();
                Log.WriteInfo("DatabaseUpdater20To30. Everything is ok");
                return version;
            }
        }

        private void importProfiles(string oldDbFile)
        {
            SqlCeConnection oldConnection = new SqlCeConnection(string.Format("Data Source={0};", oldDbFile));
            try
            {
                Log.WriteInfo("Start migration");
                oldConnection.Open();
                Log.WriteInfo("Connection opened");
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = oldConnection;

                importExercises(oldConnection);

                Log.WriteInfo("Importing profiles");
                cmd.CommandText = "SELECT ID,Name,Password,Birthday,CreationDate,Wymiary_Id,Gender FROM Profile";
                var reader = cmd.ExecuteReader();
                Log.WriteInfo("Profiles reader executed");
                while (reader.Read())
                {
                    int id = getInt32(reader, "ID").Value;
                    Log.WriteInfo("Converting profile id {0}.", id);
                    Profile profile = new Profile(reader.GetDateTime(reader.GetOrdinal("CreationDate")));
                    profile.Name = getString(reader, "Name");
                    profile.Password = getString(reader, "Password");
                    profile.Gender = (Gender)getInt32(reader, "Gender").Value;
                    profile.Birthday = getDateTime(reader, "Birthday");
                    int? wymiaryId = getInt32(reader, "Wymiary_Id");
                    if (wymiaryId.HasValue)
                    {
                        Log.WriteInfo("Profile has wymiar id {0}.", wymiaryId.Value);
                        profile.Wymiary = importWymiar(oldConnection, wymiaryId.Value);
                    }

                    profile.CreateAndFlush();
                    importTrainingDays(id, profile, oldConnection);
                    Log.WriteInfo("Profile id {0} has been saved. Starts importing training days", id);
                }
            }
            finally
            {
                oldConnection.Close();
            }
        }

        void importTrainingDays(int oldProfileId, Profile newProfile, SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing training days for profile id {0}, name{1}", newProfile.Id, newProfile.Name);
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT ID,TrainingDate,Comment FROM TrainingDay WHERE ProfileId=" + oldProfileId;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("TrainingDay reader executed");
            while (reader.Read())
            {
                int oldTrainingDayId = getInt32(reader, "ID").Value;
                DateTime date = getDateTime(reader, "TrainingDate").Value;
                Log.WriteInfo("Training day id={0},TrainingDate={1}", oldTrainingDayId, date);
                TrainingDay day = new TrainingDay(date);
                day.ProfileId = newProfile.Id;
                day.Comment = getString(reader, "Comment");
                
                importA6w(day, newProfile, oldTrainingDayId, oldConnection);
                importStrengthTraining(day, oldTrainingDayId,oldConnection);
                importSizes(day, oldTrainingDayId, oldConnection);
                day.CreateAndFlush();
                if(day.IsEmpty)
                {
                    day.Delete();
                }
                Log.WriteInfo("TrainingDay saved");
            }
            Log.WriteInfo("TrainingDay import complete");
        }

        private void importA6w(TrainingDay day,Profile profile, int oldTrainingDayId, SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing a6w for trainingdate={0},oldTrainingDayId={1}", day.TrainingDate, oldTrainingDayId);
            SqlCeCommand cmd = new SqlCeCommand();

            cmd.Connection = oldConnection;

            cmd.CommandText = "SELECT SpecificEntryObject_id,Completed,DayNumber,Set1,Set2,Set3,MyTraining_Id FROM A6WEntry,EntryObject e WHERE e.Id=SpecificEntryObject_id AND e.TrainingDay_ID=" + oldTrainingDayId;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("A6W reader executed");


            while (reader.Read())
            {
                int strengthTrainingId = getInt32(reader, "SpecificEntryObject_id").Value;
                Log.WriteInfo("Importing A6W id={0}", strengthTrainingId);
                A6WEntry entry = new A6WEntry();

                entry.Set1 = getInt32(reader, "Set1");
                entry.Set2 = getInt32(reader, "Set2");
                entry.Set3 = getInt32(reader, "Set3");
                entry.DayNumber = getInt32(reader, "DayNumber").Value;
                entry.Completed = getBoolean(reader, "Completed").Value;
                var myTrainingId = getGuid(reader, "MyTraining_id").Value;
                entry.MyTraining = importMyTraining(myTrainingId, profile, oldConnection); 

                string entryObjectSelect = "SELECT Id,Comment,Name,ReportStatus FROM EntryObject WHERE Id=" + strengthTrainingId;
                SqlCeCommand entryObjectCommand = new SqlCeCommand(entryObjectSelect, oldConnection);
                var entryObjectReader = entryObjectCommand.ExecuteReader();
                entryObjectReader.Read();
                entry.Comment = getString(entryObjectReader, "Comment");
                entry.Name = getString(entryObjectReader, "Name");
                entry.ReportStatus = (ReportStatus)getInt32(entryObjectReader, "ReportStatus").Value;
                entryObjectReader.Close();
                day.AddEntry(entry);
 
            }
            reader.Close();
            Log.WriteInfo("A6W import complete");
        }
        Dictionary<Guid,int> myTrainings = new Dictionary<Guid, int>();

        private MyTraining importMyTraining(Guid guid,Profile profie, SqlCeConnection oldConnection)
        {
            if (myTrainings.ContainsKey(guid))
            {
                return MyTraining.GetById(myTrainings[guid]);
            }
            Log.WriteInfo("Importing MyTraining id={0}", guid);
            SqlCeCommand cmd = new SqlCeCommand();

            cmd.Connection = oldConnection;

            cmd.CommandText = string.Format("SELECT Id,StartDate,EndDate,GlobalId,EntryType,ProfileId,TrainingEnd,PercentageCompleted,Name FROM MyTraining WHERE GlobalId='{0}'",guid);
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("MyTraining reader executed");

            if(reader.Read())
            {
                MyTraining myTraining = new MyTraining();
                myTraining.Name = getString(reader, "Name");
                var endDate = getDateTime(reader, "EndDate");
                var startDate = getDateTime(reader, "StartDate").Value;
                var trainingEnd =(TrainingEnd) getInt32(reader, "TrainingEnd").Value;
                myTraining.SetData(startDate,endDate,trainingEnd);
                myTraining.EntryType = getString(reader, "EntryType");
                myTraining.ProfileId = profie.Id;
                myTraining.PercentageCompleted = getInt32(reader, "PercentageCompleted");
                myTraining.CreateAndFlush();
                myTrainings.Add(guid,myTraining.Id);
                return myTraining;
            }
            return null;
        }

        private void importSizes(TrainingDay day, int oldTrainingDayId, SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing sizes for trainingdate={0},oldTrainingDayId={1}", day.TrainingDate, oldTrainingDayId);
            SqlCeCommand cmd = new SqlCeCommand();
            
            cmd.Connection = oldConnection;

            cmd.CommandText = "SELECT SpecificEntryObject_id,Wymiary_id FROM SizeEntry,EntryObject e WHERE e.Id=SpecificEntryObject_id AND e.TrainingDay_ID=" + oldTrainingDayId ;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("sizes reader executed");


            while (reader.Read())
            {
                SizeEntry entry = new SizeEntry();
                entry.Wymiary = importWymiar(oldConnection, getInt32(reader, "Wymiary_id").Value);
                if(!entry.IsEmpty)
                {
                    day.AddEntry(entry);
                }
            }

            reader.Close();
        }

        private void importStrengthTraining(TrainingDay day, int oldTrainingDayId,  SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing strength training for trainingdate={0},oldTrainingDayId={1}", day.TrainingDate, oldTrainingDayId);
            SqlCeCommand cmd = new SqlCeCommand();
            
            cmd.Connection = oldConnection;

            cmd.CommandText = "SELECT SpecificEntryObject_id,StartTime,EndTime,TrainingPlanItemId,Intensity FROM StrengthTrainingEntry,EntryObject e WHERE e.Id=SpecificEntryObject_id AND e.TrainingDay_ID=" + oldTrainingDayId ;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("StrengthTraining reader executed");
            

            while (reader.Read())
            {
                int strengthTrainingId = getInt32(reader, "SpecificEntryObject_id").Value;
                Log.WriteInfo("Importing strenght training id={0}", strengthTrainingId);
                StrengthTrainingEntry entry = new StrengthTrainingEntry();
                
                entry.StartTime = getDateTime(reader,"StartTime");
                entry.EndTime = getDateTime(reader, "EndTime");
                entry.TrainingPlanItemId = getGuid(reader, "TrainingPlanItemId");
                entry.Intensity = (Intensity) getInt32(reader, "Intensity").Value;

                string entryObjectSelect = "SELECT Id,Comment,Name,ReportStatus FROM EntryObject WHERE Id=" + strengthTrainingId;
                SqlCeCommand entryObjectCommand = new SqlCeCommand(entryObjectSelect, oldConnection);
                var entryObjectReader=entryObjectCommand.ExecuteReader();
                entryObjectReader.Read();
                entry.Comment = getString(entryObjectReader, "Comment");
                entry.Name = getString(entryObjectReader, "Name");
                entry.ReportStatus = (ReportStatus)getInt32(entryObjectReader, "ReportStatus").Value;
                entryObjectReader.Close();

                string strengthItemQuery = "SELECT Id,Position,ExerciseId,TrainingPlanItemId,SuperSetGroup,Comment FROM StrengthTrainingItem WHERE StrengthTrainingEntry_id="+strengthTrainingId;
                var strengthItemCommand = new SqlCeCommand(strengthItemQuery, oldConnection);
                var strengthItemReaer = strengthItemCommand.ExecuteReader();
                while (strengthItemReaer.Read())
                {
                    int strengthItemId = getInt32(strengthItemReaer, "Id").Value;
                    StrengthTrainingItem item =new StrengthTrainingItem();
                    item.Comment = getString(strengthItemReaer, "Comment");
                    item.ExerciseId = getGuid(strengthItemReaer, "ExerciseId").Value;
                    item.Position = getInt32(strengthItemReaer, "Position").Value;
                    item.SuperSetGroup = getString(strengthItemReaer, "SuperSetGroup");
                    item.TrainingPlanItemId = getGuid(strengthItemReaer, "TrainingPlanItemId");
                    entry.AddEntry(item);

                    SqlCeCommand serieCommand = new SqlCeCommand("SELECT  * FROM Serie WHERE StrengthTrainingItem_id=" + strengthItemId, oldConnection);
                    var serieReader = serieCommand.ExecuteReader();
                    Log.WriteInfo("Serie reader executed");
                    while (serieReader.Read())
                    {
                        Serie serie = new Serie();
                        serie.Comment = getString(serieReader, "Comment");
                        serie.RepetitionNumber = getInt32(serieReader, "RepetitionNumber");
                        serie.Weight = getFloat(serieReader, "Weight");
                        serie.TrainingPlanItemId = getGuid(serieReader, "TrainingPlanItemId");
                        serie.IsLastRepetitionWithHelp = getBoolean(serieReader, "IsLastRepetitionWithHelp").Value;
                        serie.IsCiezarBezSztangi = getBoolean(serieReader, "IsCiezarBezSztangi").Value;
                        Log.WriteInfo("Serie {0}", serie);
                        item.AddSerie(serie);
                    }
                }
                strengthItemReaer.Close();

                Log.WriteInfo("Strength entries count={0}", entry.Entries.Count);
                if (entry.Entries.Count > 0)
                {
                    day.AddEntry(entry);
                }
  
            }
            reader.Close();
            Log.WriteInfo("StrengthTraining import complete");
        }
        
        void importExercises(SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing exercises");
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT GlobalID, Name,Description, Url,Shortcut,ExerciseType,IsDefault,Difficult FROM Exercise";
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("Exercises reader executed");
            while (reader.Read())
            {
                Guid globalId = getGuid(reader, "GlobalID").Value;
                Log.WriteInfo("Importing exercise GlobalId={0}", globalId);
                Exercise exercise = new Exercise(globalId);
                exercise.Name = getString(reader, "Name");
                exercise.Shortcut = getString(reader, "Shortcut");
                exercise.Url = getString(reader, "Url");
                exercise.Description = getString(reader, "Description");
                exercise.Difficult = (ExerciseDifficult)getInt32(reader, "Difficult").Value;
                exercise.Muscle = (Muscle)getInt32(reader, "ExerciseType").Value;

                if (exercise.GlobalId == new Guid("0685238E-116A-477C-8881-F39D6F25C686"))
                {
                    Log.WriteInfo("Exercise is szrugsy so change the exercise type");
                    exercise.Muscle = Muscle.Czworoboczny;
                }
                exercise.Create();
            }
            Log.WriteInfo("Importing exercises complete");
        }

        Wymiary importWymiar(SqlCeConnection oldConnection, int wymiarId)
        {
            Log.WriteInfo("Importing wymiar id={0}", wymiarId);
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT DateTime, IsNaCzczo,Klatka,Pas,Weight,RightBiceps,LeftBiceps,RightForearm, LeftForearm,RightUdo,LeftUdo,Height FROM Wymiary WHERE ID=" + wymiarId;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("wymiar reader executed");
            if (reader.Read())
            {
                Log.WriteInfo("Wymiar exists");
                Wymiary wymiary = new Wymiary();
                wymiary.DateTime = getDateTime(reader, "DateTime").Value;
                wymiary.IsNaCzczo = getBoolean(reader, "IsNaCzczo").Value;
                wymiary.Klatka = getFloat(reader, "Klatka").Value;
                wymiary.LeftBiceps = getFloat(reader, "LeftBiceps").Value;
                wymiary.LeftUdo = getFloat(reader, "LeftUdo").Value;
                wymiary.RightUdo = getFloat(reader, "RightUdo").Value;
                wymiary.Pas = getFloat(reader, "Pas").Value;
                wymiary.RightBiceps = getFloat(reader, "RightBiceps").Value;
                wymiary.Weight = getFloat(reader, "Weight").Value;
                wymiary.Height = getInt32(reader, "Height").Value;
                wymiary.LeftForearm = getFloat(reader, "LeftForearm").Value;
                wymiary.RightForearm = getFloat(reader, "RightForearm").Value;
                wymiary.Save();
                Log.WriteInfo("Wymiar saved");
                return wymiary;
            }
            return null;
        }
    }
}
