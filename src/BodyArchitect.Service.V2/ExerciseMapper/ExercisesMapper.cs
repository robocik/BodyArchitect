//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using BodyArchitect.Logger;
//using BodyArchitect.Model;
//using BodyArchitect.Service.V2.Model;
//using BodyArchitect.Service.V2.Services;
//using BodyArchitect.Shared;
//using NHibernate;
//using PublishStatus = BodyArchitect.Model.PublishStatus;

//namespace BodyArchitect.Service.V2.ExerciseMapper
//{
//    public class ExercisesMapper
//    {
//        private ISession session;
//        private ITimerService timerService;
//        public ExercisesMapper(ISession session, ITimerService timerService)
//        {
//            this.session = session;
//            this.timerService = timerService;
//        }

        

//        public MapperResult Run(int profileId, MapperData data)
//        {
//            var wrongList=data.Validate();
//            if(wrongList.Count>0)
//            {
//                throw new ValidationException("Mapper data contains some duplicated From exercises");
//            }
//            using (var tx = session.BeginTransaction())
//            {
//                int rowAffected = 0;
//                Profile dbProfile = session.Load<Profile>(profileId);
//                foreach (var mapperEntry in data.Entries)
//                {
//                    Exercise fromEx = session.Load<Exercise>(mapperEntry.FromExerciseId);
//                    Exercise toEx = session.Get<Exercise>(mapperEntry.ToExerciseId);
//                    if(toEx.Profile!=dbProfile && toEx.Status!=PublishStatus.Published)
//                    {
//                        throw new InvalidOperationException("Cannot map to non public exercise from another user");
//                    }

//                    string startDate = "",endDate="";
//                    if(data.StartDate.HasValue)
//                    {
//                        startDate = " AND td.TrainingDate>=:startDate ";
//                    }
//                    if (data.EndDate.HasValue)
//                    {
//                        endDate = " AND td.TrainingDate<=:endDate ";
//                    }
//                    string selectQuery =string.Format(@"SELECT ste.EntryObject_id from  StrengthTrainingEntry ste
//                        INNER JOIN EntryObject eo ON eo.Id=ste.EntryObject_id
//                        INNER JOIN TrainingDay td ON td.Id=eo.TrainingDay_id WHERE td.Profile_id=:Profile {0} {1}",startDate,endDate);

//                    var query = session.CreateSQLQuery(string.Format(@"UPDATE StrengthTrainingItem 
//SET ExerciseId=:TO WHERE ExerciseId=:FROM AND StrengthTrainingEntry_id in ({0})", selectQuery));
//                    query.SetGuid("TO", toEx.GlobalId);
//                    query.SetGuid("FROM", fromEx.GlobalId);
//                    query.SetInt32("Profile",profileId);
//                    if (data.StartDate.HasValue)
//                    {
//                        query.SetDateTime("startDate", data.StartDate.Value.Date);
//                    }
//                    if (data.EndDate.HasValue)
//                    {
//                        query.SetDateTime("endDate", data.EndDate.Value.Date);
//                    }
//                    rowAffected+= query.ExecuteUpdate();
//                    if(mapperEntry.Operation==MapperEntryOperation.ReMapAndDeleteExercise)
//                    {
//                        try
//                        {
//                            ExerciseOperation.DeleteExercise(session, fromEx.GlobalId, profileId,timerService);
//                        }
//                        catch (Exception ex)
//                        {
//                            ExceptionHandler.Default.Process(ex);
//                        }
                        
//                    }
//                    var profile=session.Get<Profile>(profileId);
//                    profile.DataInfo.LastExerciseModification = timerService.UtcNow;

//                }
//                tx.Commit();
//                MapperResult result = new MapperResult(rowAffected);
//                return result;
//            }
//        }
//    }
//}
