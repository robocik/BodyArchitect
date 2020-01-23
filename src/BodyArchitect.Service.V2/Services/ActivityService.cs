using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;
using NHibernate;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitect.Service.V2.Services
{
    public class ActivityService: ServiceBase
    {
        public ActivityService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<ActivityDTO> GetActivities( PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetActivities:Username={0}", SecurityInfo.SessionData.Profile.UserName);
            var session = Session;


            using (var tx = session.BeginGetTransaction())
            {
                Profile _profile = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var queryExercises = session.QueryOver<Activity>().Where(x=>x.Profile==_profile);
                

                var listPack = queryExercises.ToPagedResults<ActivityDTO, Activity>(retrievingInfo);
                tx.Commit();
                return listPack;
            }
        }

        public ActivityDTO SaveActivity(ActivityDTO activity)
        {
            Log.WriteWarning("SaveActivity:Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName,activity.GlobalId);
            var dbActivity = activity.Map<Activity>();
            using (var trans = Session.BeginSaveTransaction())
            {
                //Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
                //Activity db = null;
                //if(activity.GlobalId!=Constants.UnsavedGlobalId)
                //{
                //    db = Session.Get<Activity>(activity.GlobalId);
                //    Mapper.Map(activity, db);
                //}
                //else
                //{
                //    db = activity.Map<Activity>();
                //    db.CreationDate = Configuration.TimerService.UtcNow;
                //    db.Profile = dbProfile;
                //}

                //if (SecurityInfo.SessionData.Profile.Id != db.Profile.Id)
                //{
                //    throw new CrossProfileOperationException("Cannot modify activity for another user");
                //}
                Profile dbProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);

                if(dbActivity.IsNew)
                {
                    dbActivity.CreationDate = Configuration.TimerService.UtcNow;
                }
                else
                {
                    var db = Session.Get<Activity>(activity.GlobalId);
                    if (db != null)
                    {
                        if (SecurityInfo.SessionData.Profile.GlobalId != db.Profile.GlobalId)
                        {
                            throw new CrossProfileOperationException("Cannot modify Activity for another user");
                        }
                    }
                }
                
                
                dbActivity.Profile = dbProfile;
                if (string.IsNullOrEmpty(dbActivity.Color))
                {
                    dbActivity.Color = Constants.DefaultColor;
                }

                int res = Session.QueryOver<Activity>().Where(x => x.Name == dbActivity.Name && x.GlobalId != dbActivity.GlobalId && x.Profile == dbProfile).RowCount();
                if (res > 0)
                {
                    throw new UniqueException("Activity with the same name is already exist");
                }

                dbActivity = Session.Merge(dbActivity);
                dbProfile.DataInfo.ActivityHash = Guid.NewGuid();
                trans.Commit();
                return dbActivity.Map<ActivityDTO>();
            }

            
        
        }

        public void DeleteActivity(ActivityDTO activity)
        {
            Log.WriteWarning("DeleteActivity: Username={0},GlobalId={1}", SecurityInfo.SessionData.Profile.UserName, activity.GlobalId);

            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var dbActivity = session.Get<Activity>(activity.GlobalId);
                if (SecurityInfo.SessionData.Profile.GlobalId != dbActivity.Profile.GlobalId)
                {
                    throw new CrossProfileOperationException("Cannot modify activity for another user");
                }
                session.Delete(dbActivity);
                dbActivity.Profile.DataInfo.ActivityHash = Guid.NewGuid();
                //now update cache modification date
                //var dbProfile = session.Get<Profile>(dto.Profile.Id);
                //dbProfile.DataInfo.LastPlanModification = Configuration.TimerService.UtcNow;
                //session.Update(dbProfile);
                tx.Commit();
            }
        }
    }
}
