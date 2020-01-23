using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using BodyArchitect.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Size.Model;
using BodyArchitect.StrengthTraining.Model;
using NHibernate;

namespace BodyArchitect
{
    class DatabaseUpdater11To20 : DatabaseUpdaterBase
    {
        public DatabaseVersion Upgrade(string oldDbFile)
        {
            
            //ISession session=NHibernateSessionManager.SessionFactory.OpenSession();
            using (var scope =new SessionScope())
            {
                //var transaction = session.BeginTransaction();
                try
                {
                    Log.WriteInfo("DatabaseUpdater11To20. OldFile={0}", oldDbFile);
                    if (File.Exists(oldDbFile))
                    {
                        Log.WriteInfo("File exists. Start converting db from 1.1.0.1 to 2.0.0.0");
                        importProfiles(oldDbFile);
                    }
                    Log.WriteInfo("Set database version to 2.0.0.0");
                    DatabaseVersion version = new DatabaseVersion("2.0.0.0");
                    //session.SaveOrUpdate(version);
                    version.Create();
                    //transaction.Commit();
                    Log.WriteInfo("DatabaseUpdater11To20. Everything is ok");
                    return version;
                }
                catch (Exception)
                {
                    //transaction.Rollback();
                    throw;
                }
                finally
                {
                    //session.Close();
                }
            }
            
        }

        private void importProfiles( string oldDbFile)
        {
            SqlCeConnection oldConnection= new SqlCeConnection(string.Format("Data Source={0};", oldDbFile));
            try
            {
                Log.WriteInfo("Start migration");
                oldConnection.Open();
                Log.WriteInfo("Connection opened");
                SqlCeCommand cmd = new SqlCeCommand();
                cmd.Connection = oldConnection;

                importExercises(oldConnection);

                Log.WriteInfo("Importing profiles");
                cmd.CommandText = "SELECT ID,Name,Password,Birthday,CreationDate,WymiaryId FROM Profile";
                var reader = cmd.ExecuteReader();
                Log.WriteInfo("Profiles reader executed");
                while (reader.Read())
                {
                    int id = getInt32(reader, "ID").Value;
                    Log.WriteInfo("Converting profile id {0}.",id);
                    Profile profile = new Profile(reader.GetDateTime(reader.GetOrdinal("CreationDate")));
                    profile.Name = getString(reader, "Name");
                    profile.Password = getString(reader, "Password");
                    profile.Birthday = getDateTime(reader, "Birthday");
                    int? wymiaryId = getInt32(reader, "WymiaryId");
                    if (wymiaryId.HasValue)
                    {
                        Log.WriteInfo("Profile has wymiar id {0}.", wymiaryId.Value);
                        profile.Wymiary = importWymiar(oldConnection, wymiaryId.Value);
                    }

                    profile.Save();

                    Log.WriteInfo("Profile id {0} has been saved. Starts importing training days", id);
                    importTrainingDays(id, profile, oldConnection);
                }
            }
            finally
            {
                oldConnection.Close();
            }
        }


        void importExercises( SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing exercises");
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT GlobalID, Name,Description, Url,Shortcut,ExerciseType,IsDefault,Difficult FROM Exercise";
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("Exercises reader executed");
            while(reader.Read())
            {
                Guid globalId = getGuid(reader, "GlobalID").Value;
                Log.WriteInfo("Importing exercise GlobalId={0}",globalId);
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
                exercise.Save();
            }
            Log.WriteInfo("Importing exercises complete");
        }

        void importTrainingDays(int oldProfileId, Profile newProfile, SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing training days for profile id {0}, name{1}",newProfile.Id,newProfile.Name);
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT ID,TrainingDate,StartTime,EndTime,Comment,WymiaryId FROM TrainingDay WHERE ProfileId="+oldProfileId;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("TrainingDay reader executed");
            while (reader.Read())
            {
                int oldTrainingDayId = getInt32(reader, "ID").Value;
                DateTime date=getDateTime(reader, "TrainingDate").Value;
                Log.WriteInfo("Training day id={0},TrainingDate={1}", oldTrainingDayId, date);
                TrainingDay day = new TrainingDay(date);
                day.ProfileId = newProfile.Id;
                day.Comment = getString(reader, "Comment");
                int? wymiaryId = getInt32(reader, "WymiaryId");
                if(wymiaryId.HasValue)
                {
                    Log.WriteInfo("TrainingDay has wymiary id={0}",wymiaryId.Value);
                    SizeEntry sizeEntry = new SizeEntry();
                    sizeEntry.Wymiary = importWymiar(oldConnection, wymiaryId.Value);
                    day.AddEntry(sizeEntry);
                    //session.SaveOrUpdate(sizeEntry);
                }
                importStrengthTraining(day, oldTrainingDayId, getDateTime(reader,"StartTime"),getDateTime(reader,"EndTime"),day.Comment,oldConnection);
                day.Save();
                Log.WriteInfo("TrainingDay saved");
            }
            Log.WriteInfo("TrainingDay import complete");
        }

        private void importStrengthTraining(TrainingDay day, int oldTrainingDayId,DateTime? startTime,DateTime? endTime ,string comment,SqlCeConnection oldConnection)
        {
            Log.WriteInfo("Importing strength training for trainingdate={0},oldTrainingDayId={1}",day.TrainingDate,oldTrainingDayId);
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText = "SELECT ID,Comment,ExerciseID,Position FROM TrainingDayEntry WHERE TrainingDayID=" + oldTrainingDayId+" ORDER BY Position";
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("StrengthTraining reader executed");
            StrengthTrainingEntry entry = new StrengthTrainingEntry();
            entry.Comment = comment;
            entry.StartTime = startTime;
            entry.EndTime = endTime;
            
            while (reader.Read())
            {
                int strengthTrainingId = getInt32(reader, "ID").Value;
                Log.WriteInfo("Importing strenght training id={0}", strengthTrainingId);
                StrengthTrainingItem item = new StrengthTrainingItem();
                item.Comment = getString(reader, "Comment");
                entry.AddEntry(item);
                item.ExerciseId = getGuid(reader, "ExerciseID").Value;

                SqlCeCommand serieCommand = new SqlCeCommand("SELECT  * FROM Serie WHERE TrainingDayEntryID=" + strengthTrainingId, oldConnection);
                var serieReader = serieCommand.ExecuteReader();
                Log.WriteInfo("Serie reader executed");
                while (serieReader.Read())
                {
                    Serie serie = new Serie();
                    serie.Comment = getString(serieReader, "Comment");
                    serie.RepetitionNumber = getInt32(serieReader, "RepetitionNumber");
                    serie.Weight = getFloat(serieReader, "Weight");
                    serie.IsLastRepetitionWithHelp = getBoolean(serieReader, "IsLastRepetitionWithHelp").Value;
                    serie.IsCiezarBezSztangi = getBoolean(serieReader, "IsCiezarBezSztangi").Value;
                    Log.WriteInfo("Serie {0}",serie);
                    item.AddSerie(serie);
                }
            }
            Log.WriteInfo("Strength entries count={0}", entry.Entries.Count);
            if (entry.Entries.Count > 0)
            {
                
                day.AddEntry(entry);
            }
            Log.WriteInfo("StrengthTraining import complete");
        }

        Wymiary importWymiar( SqlCeConnection oldConnection, int wymiarId)
        {
            Log.WriteInfo("Importing wymiar id={0}",wymiarId);
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = oldConnection;
            cmd.CommandText ="SELECT DateTime, NaCzczo,Klatka,Pas,Udo,Weight,RightBiceps,LeftBiceps FROM Wymiar WHERE ID=" + wymiarId;
            var reader = cmd.ExecuteReader();
            Log.WriteInfo("wymiar reader executed");
            if(reader.Read())
            {
                Log.WriteInfo("Wymiar exists");
                Wymiary wymiary = new Wymiary();
                wymiary.DateTime = getDateTime(reader,"DateTime").Value;
                wymiary.IsNaCzczo = getBoolean(reader,"NaCzczo").Value;
                wymiary.Klatka = getFloat(reader,"Klatka").Value;
                wymiary.LeftBiceps = getFloat(reader, "LeftBiceps").Value;
                wymiary.LeftUdo = wymiary.RightUdo = getFloat(reader,"Udo").Value;
                wymiary.Pas  = getFloat(reader,"Pas").Value;
                wymiary.RightBiceps = getFloat(reader,"RightBiceps").Value;
                wymiary.Weight = getFloat(reader,"Weight").Value;
                wymiary.Save();
                Log.WriteInfo("Wymiar saved");
                return wymiary;
            }
            return null;
        }

        
    }
}
