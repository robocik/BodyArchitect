using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Service.Model;


namespace BodyArchitect.Module.StrengthTraining
{
    public static class ObjectsReposidory
    {
        private static IDictionary<Guid, ExerciseDTO> dictExercises;
        private static IDictionary<Guid, WorkoutPlanDTO> dictPlans;

        private static ManualResetEvent exercisesEvent = new ManualResetEvent(false);
        private static ManualResetEvent plansEvent = new ManualResetEvent(false);

        static ObjectsReposidory()
        {
            UserContext.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            loadExercises();
            loadPlans();
        }

        static void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (e.Status == LoginStatus.Logged)
            {
                ClearExerciseCache();
                ClearWorkoutPlansCache();
            }
        }

        public static void ClearExerciseCache()
        {
            if (dictExercises != null)
            {
                loadExercises();
            }

        }

        private static void loadExercises()
        {
            exercisesEvent.Reset();
            dictExercises = null; 
            var task=Task.Factory.StartNew(delegate
                {
                    ControlHelper.EnsureThreadLocalized();
                    PagedResultRetriever retriever = new PagedResultRetriever();
                    var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
                          {
                              try
                              {
                                  ExerciseSearchCriteria search = ExerciseSearchCriteria.CreatePersonalCriteria();
                                  return ServiceManager.GetExercises(search, pageInfo);
                              }
                              catch (Exception)
                              {
                                  exercisesEvent.Set();
                                  throw;
                              }
                              
                          });
                    dictExercises = res.ToDictionary(t => t.GlobalId);

                    exercisesEvent.Set();
                }, exercisesEvent);

            LogUnhandledExceptions(task);
        }

        private static Task LogUnhandledExceptions(Task task)
        {
            task.ContinueWith(delegate(Task t)
                                  {
                                      ExceptionHandler.Default.Process(t.Exception);
                                  }, TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        private static void loadPlans()
        {
            plansEvent.Reset();
            dictPlans = null;
            var task=Task.Factory.StartNew(delegate
            {
                ControlHelper.EnsureThreadLocalized();
                PagedResultRetriever retriever = new PagedResultRetriever();
                var res = retriever.GetAll(delegate(PartialRetrievingInfo pageInfo)
                {
                    try
                    {
                        WorkoutPlanSearchCriteria criteria = WorkoutPlanSearchCriteria.CreateFindAllCriteria();
                        criteria.SearchGroups.Remove(WorkoutPlanSearchCriteriaGroup.Other);
                        var list = ServiceManager.GetWorkoutPlans(criteria, pageInfo);
                        return list;
                    }
                    catch (Exception)
                    {
                        plansEvent.Set();
                        throw;
                    }
                    
                });

                dictPlans = res.ToDictionary(t => t.GlobalId);

                plansEvent.Set();
            }, plansEvent);

            LogUnhandledExceptions(task);
        }

        public static void ClearWorkoutPlansCache()
        {
            if(dictPlans!=null)
            {
                loadPlans();
            }
        }
        public static IDictionary<Guid, ExerciseDTO> Exercises
        {
            get
            {
                EnsureExercisesLoaded();
                return dictExercises;
            }
        }

        public static IList<WorkoutPlanDTO> WorkoutPlans
        {
            get
            {
                EnsurePlansLoaded();
                return new List<WorkoutPlanDTO>(dictPlans.Values);
            }
        }



        public static void EnsureExercisesLoaded()
        {
            if (dictExercises == null)
            {
                dictExercises = new Dictionary<Guid, ExerciseDTO>();
                if (ServicesManager.IsDesignMode)
                {
                    return;
                }
                
            }
            exercisesEvent.WaitOne();
        }
        
        public static void EnsurePlansLoaded()
        {
            if (dictPlans == null)
            {
                dictPlans = new Dictionary<Guid, WorkoutPlanDTO>();
                if (ServicesManager.IsDesignMode)
                {
                    return;
                }
                
                
            }
            plansEvent.WaitOne();
        }

        public static ExerciseDTO GetExercise(Guid id)
        {
            EnsureExercisesLoaded();
            if (dictExercises.ContainsKey(id))
            {
                return dictExercises[id];
            }
            return ExerciseDTO.Removed;
        }

        public static WorkoutPlanDTO GetWorkoutPlan(Guid id)
        {
            EnsurePlansLoaded();
            if (dictPlans.ContainsKey(id))
            {
                return dictPlans[id];
            }
            return null;
        }
        
        //public static bool IsFavorite(this ExerciseDTO exercise)
        //{
        //    if(exercise==null)
        //    {
        //        return false;
        //    }
        //    EnsureExercisesLoaded();
        //    var res = (from e in dictExercises.Values where e.GlobalId == exercise.GlobalId && e.Profile != null && !e.IsMine() select e).Count();
        //    return res > 0;
        //}

        public static bool IsFavorite(this WorkoutPlanDTO plan)
        {
            EnsurePlansLoaded();
            if (dictPlans == null)
            {
                return false;
            }
            var res = (from e in dictPlans.Values where e.GlobalId == plan.GlobalId && e.Profile != null && !e.IsMine() select e).Count();
            return res > 0;
        }

        //public static bool CanBeFavorite(this ExerciseDTO exercise)
        //{
        //    if (exercise == null || exercise.Profile == null || exercise.IsMine())
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        public static bool IsEditable(this ExerciseDTO exercise)
        {
            if (exercise==null)
            {
                return false;
            }
            if (exercise.Profile == null || exercise.Status != PublishStatus.Private)
            {
                return false;
            }
            return true;
        }

        internal static void UpdateExercise(ExerciseDTO exerciseDto, bool delete = false)
        {
            if (dictExercises.ContainsKey(exerciseDto.GlobalId))
            {
                if (delete)
                {
                    dictExercises.Remove(exerciseDto.GlobalId);
                }
                else
                {
                    dictExercises[exerciseDto.GlobalId] = exerciseDto;    
                }
                
            }
        }
    }
}
