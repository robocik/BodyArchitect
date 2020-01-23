using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using ExerciseType = BodyArchitect.Model.ExerciseType;


namespace BodyArchitect.Service.V2.Services
{
    public class ExerciseService : ServiceBase
    {
        public ExerciseService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<ExerciseDTO> GetExercises( ExerciseSearchCriteria searchCriteria, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetExercises:Username={0},Search-UserId:{1},SearchGroups:{2}", SecurityInfo.SessionData.Profile.UserName, searchCriteria.UserId, searchCriteria.SearchGroups.Count);
            var session = Session;

            //Profile _profile = null;
            Exercise _exercise = null;

            using (var transactionScope = new TransactionManager(true))
            {
                var myProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if(searchCriteria.UserId.HasValue)
                {
                    myProfile = session.Load<Profile>(searchCriteria.UserId.Value);
                }
                
                var ids = myProfile.FavoriteExercises.Select(x => x.GlobalId).ToList();

                var queryExercises = session.QueryOver<Exercise>(() => _exercise).Where(x=>!x.IsDeleted);

                if (searchCriteria.ExerciseTypes.Count > 0)
                {
                    var langOr = Restrictions.Disjunction();
                    foreach (var lang in searchCriteria.ExerciseTypes)
                    {
                        langOr.Add<Exercise>(x => x.ExerciseType == (ExerciseType) lang);
                    }
                    queryExercises = queryExercises.And(langOr);

                }

                if(!string.IsNullOrEmpty(searchCriteria.Name))
                {
                    queryExercises = queryExercises.Where(Restrictions.InsensitiveLike("Name", searchCriteria.Name+"%"));
                }

                
                if (searchCriteria.SearchGroups.Count > 0)
                {
                    Disjunction mainOr = Restrictions.Disjunction();
                    if (searchCriteria.SearchGroups.IndexOf(ExerciseSearchCriteriaGroup.Global) > -1)
                    {
                        Log.WriteVerbose("Search: global");
                        mainOr.Add<Exercise>(dto => dto.Profile ==null);
                    }
                    if (searchCriteria.SearchGroups.IndexOf(ExerciseSearchCriteriaGroup.Mine) > -1)
                    {
                        Log.WriteVerbose("Search: mine");
                        mainOr.Add<Exercise>(dto => dto.Profile == myProfile);
                    }
                    if (searchCriteria.SearchGroups.IndexOf(ExerciseSearchCriteriaGroup.Other) > -1)
                    {
                        Log.WriteVerbose("Search: other");
                        mainOr.Add<Exercise>(dto =>dto.Profile!=null &&  dto.Profile != myProfile);
                    }
                    if (searchCriteria.SearchGroups.IndexOf(ExerciseSearchCriteriaGroup.Favorites) > -1)
                    {
                        if (ids.Count > 0)
                        {
                            mainOr.Add<BodyArchitect.Model.TrainingPlan>(x => x.GlobalId.IsIn((ICollection)ids));
                        }
                    }
                    queryExercises = queryExercises.Where(mainOr);
                }
                
                //if (searchCriteria.UserId.HasValue)
                //{
                //    queryExercises = queryExercises.Where(dto => dto.Profile == myProfile);
                //}
                queryExercises = queryExercises.ApplySorting(searchCriteria.SortOrder, searchCriteria.SortAscending);

                var res1 = (from rv in session.Query<RatingUserValue>()
                            from tp in session.Query<Exercise>()
                            where tp.GlobalId == rv.RatedObjectId &&
                                rv.ProfileId == SecurityInfo.SessionData.Profile.GlobalId
                            select rv).ToDictionary(t => t.RatedObjectId);

                var listPack = queryExercises.ToPagedResults<ExerciseDTO, Exercise>(retrievingInfo,null,
                          delegate(IEnumerable<Exercise> list)
                          {
                              var output = new List<ExerciseDTO>();
                              foreach (var planDto in list)
                              {

                                  var tmp = planDto.Map<ExerciseDTO>();
                                  if (res1.ContainsKey(planDto.GlobalId))
                                  {
                                      tmp.UserRating = res1[planDto.GlobalId].Rating;
                                      tmp.UserShortComment = res1[planDto.GlobalId].ShortComment;
                                  }
                                  output.Add(tmp);
                              }
                              return output.ToArray();
                          });
                transactionScope.CommitTransaction();
                return listPack;
            }
        }

        public ExerciseDTO SaveExercise( ExerciseDTO exercise)
        {
            Log.WriteWarning("SaveExercise:Username={0},Exercise: {1}", SecurityInfo.SessionData.Profile.UserName, exercise.GlobalId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }
            var dbExercise = exercise.Map<Exercise>();
            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                dbExercise.Profile = session.Get<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                if(!exercise.IsNew)
                {
                    var oldExercise = Session.Load<Exercise>(exercise.GlobalId);
                    if (oldExercise.Profile != dbExercise.Profile)
                    {
                        throw new CrossProfileOperationException("This exercise belongs to another user");
                    }
                    if(oldExercise.Name!=dbExercise.Name)
                    {
                        throw new InvalidOperationException("Cannot change exercise's name");
                    }
                    dbExercise.Rating = oldExercise.Rating;
                }
                else
                {
                    dbExercise.GlobalId = Guid.NewGuid();
                    dbExercise.CreationDate = Configuration.TimerService.UtcNow;
                    Log.WriteVerbose("New exercise: GlobaId={0}", dbExercise.GlobalId);
                }
                //var dbOriginalExercise = session.Get<Exercise>(exercise.GlobalId);
                

                //if (dbOriginalExercise != null)
                //{
                //    Log.WriteVerbose("Existing exercise");
                //    if (dbOriginalExercise.Profile == null)
                //    {
                //        throw new CrossProfileOperationException("Cannot change default exercises");
                //    }
                //    if (dbOriginalExercise.Profile.Id != SecurityInfo.SessionData.Profile.Id ||
                //        dbOriginalExercise.Profile.Id != dbExercise.Profile.Id)
                //    {
                //        throw new CrossProfileOperationException("Cannot change an exercise for another profile");
                //    }

                //    //we cannot modify published exercise
                //    if (dbOriginalExercise.Status == PublishStatus.Published)
                //    {
                //        throw new PublishedObjectOperationException("Cannot change published exercise");
                //    }

                //    //save operation shouldn't change Rating
                //    dbExercise.Rating = dbOriginalExercise.Rating;
                //}
                //else
                

                if (dbExercise.ExerciseType == ExerciseType.NotSet)
                {
                    throw new ArgumentException("Exercise must have ExerciseType set.", "ExerciseType");
                }
                //dbExercise.Profile.DataInfo.LastExerciseModification = Configuration.TimerService.UtcNow;
                dbExercise = session.Merge(dbExercise);
                dbExercise.Profile.DataInfo.ExerciseHash = Guid.NewGuid();

                //now retrieve user rating if exists
                //var rating=session.QueryOver<RatingUserValue>().Where(
                //    x => x.RatedObjectId == dbExercise.GlobalId && x.ProfileId == SecurityInfo.SessionData.Profile.Id).
                //    SingleOrDefault();
                tx.Commit();
                var exerciseDto = dbExercise.Map<ExerciseDTO>();
                //if (rating!=null)
                //{
                //    exerciseDto.UserRating = rating.Rating;
                //    exerciseDto.UserShortComment = rating.ShortComment;
                //}
                Log.WriteInfo("Save complete");
                return exerciseDto;
            }
            

        }

        public void ExerciseOperation(ExerciseOperationParam param)
        {
            Log.WriteWarning("ExerciseOperationParam: Username={0},userDto.GlobalId={1}, Exercise={2}", SecurityInfo.SessionData.Profile.UserName, param.Operation,param.ExerciseId);

            if (!SecurityInfo.Licence.IsPremium)
            {
                throw new LicenceException("This feature is allowed for Premium account");
            }

            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var profileDb = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                var exercise = session.Get<Exercise>(param.ExerciseId);

                if (exercise.Profile == profileDb)
                {
                    throw new InvalidOperationException("Cannot add/remove your exercise to the favorite list");
                }
                if (param.Operation == FavoriteOperation.Add)
                {
                    if (profileDb.FavoriteExercises.Contains(exercise))
                    {
                        throw new ObjectIsFavoriteException("Exercise is in favorites list already");
                    }
                    profileDb.FavoriteExercises.Add(exercise);
                }
                else
                {
                    if (profileDb.FavoriteExercises.Contains(exercise))
                    {
                        profileDb.FavoriteExercises.Remove(exercise);
                    }
                    else
                    {
                        throw new ObjectIsNotFavoriteException("Exercise is not in favorites list");
                    }
                    
                }
                profileDb.DataInfo.ExerciseHash = Guid.NewGuid();
                session.SaveOrUpdate(profileDb);
                tx.Commit();
            }
        }
    }
}
