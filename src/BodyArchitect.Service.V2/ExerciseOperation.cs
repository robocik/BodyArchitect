using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Services;
using BodyArchitect.Shared;
using NHibernate;

namespace BodyArchitect.Service.V2
{
    //class ExerciseOperation
    //{
    //    public static void DeleteExercise(ISession session,Guid exerciseId,int profileId,ITimerService timerService)
    //    {
            
    //        var dbExercise = session.Get<BodyArchitect.Model.Exercise>(exerciseId);
    //        if (profileId != dbExercise.Profile.Id)
    //        {
    //            throw new CrossProfileOperationException("Cannot modify exercise for another user");
    //        }

    //        if (dbExercise.Status == PublishStatus.Published)
    //        {
    //            throw new PublishedObjectOperationException("Cannot delete published exercise");
    //        }
    //        var profile=session.Get<Profile>(profileId);
    //        profile.DataInfo.LastExerciseModification = timerService.UtcNow;
    //        session.Delete("FROM RatingUserValue WHERE RatedObjectId=?",exerciseId,
    //                       NHibernate.NHibernateUtil.Guid);
    //        session.Delete(dbExercise);
    //    }
    //}
}
