using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using NHibernate.Linq;
using Profile = BodyArchitect.Model.Profile;
using ProfileNotification = BodyArchitect.Model.ProfileNotification;

namespace BodyArchitect.Service.V2.Services
{
    class VoteService : MessageServiceBase
    {

        public VoteService(ISession session, SecurityInfo SecurityInfo, ServiceConfiguration configuration, IPushNotificationService pushNotification, IEMailService emailService)
            : base(session, SecurityInfo, configuration, pushNotification,emailService)
        {
            
        }

        //public WorkoutPlanDTO VoteWorkoutPlan( WorkoutPlanDTO planDto)
        //{
        //    Log.WriteWarning("VoteWorkoutPlan: Username={0},planId={1}", SecurityInfo.SessionData.Profile.UserName, planDto.GlobalId);

        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        var dbProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        var planFromDb = (from p in session.Query<BodyArchitect.Model.TrainingPlan>()
        //                          where p.GlobalId == planDto.GlobalId
        //                          select p).SingleOrDefault();
        //        saveRating(SecurityInfo, planDto, dbProfile, planFromDb);

        //        tx.Commit();

        //        try
        //        {
        //            //send message only when someone else vote
        //            if (planFromDb.Profile != dbProfile)
        //            {
        //                if (planFromDb.Profile.Settings.NotificationWorkoutPlanVoted)
        //                {
        //                    string param = string.Format("{0},{1},{2},{3}", planFromDb.Name, dbProfile.UserName, DateTime.Now, planDto.UserRating);
        //                    MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
        //                    messageService.SendSystemMessage(param, dbProfile, planFromDb.Profile, BodyArchitect.Model.MessageType.WorkoutPlanVoted);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }

        //        session.Refresh(planFromDb);
        //        Mapper.Map<BodyArchitect.Model.TrainingPlan, WorkoutPlanDTO>(planFromDb, planDto);
        //        return planDto;
        //    }

        //}

        //public ExerciseDTO VoteExercise( ExerciseDTO exercise)
        //{
        //    Log.WriteWarning("VoteExercise: Username={0},planId={1}", SecurityInfo.SessionData.Profile.UserName, exercise.GlobalId);

        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        var dbProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        var planFromDb = (from p in session.Query<BodyArchitect.Model.Exercise>()
        //                          where p.GlobalId == exercise.GlobalId
        //                          select p).SingleOrDefault();
        //        saveRating(SecurityInfo, exercise, dbProfile, planFromDb);

        //        tx.Commit();

        //        try
        //        {
        //            //send message only when someone else vote
        //            if (planFromDb.Profile != null && planFromDb.Profile != dbProfile)
        //            {
        //                if (planFromDb.Profile.Settings.NotificationExerciseVoted)
        //                {
        //                    string param = string.Format("{0},{1},{2},{3}", planFromDb.Name, dbProfile.UserName, DateTime.Now, exercise.UserRating);
        //                    MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
        //                    messageService.SendSystemMessage(param, dbProfile, planFromDb.Profile, BodyArchitect.Model.MessageType.ExerciseVoted);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }

        //        session.Refresh(planFromDb);
        //        Mapper.Map<BodyArchitect.Model.Exercise, ExerciseDTO>(planFromDb, exercise);
        //        return exercise;
        //    }

        //}

        float saveRating(SecurityInfo SecurityInfo, VoteParams ratingable, Profile profile, BodyArchitect.Model.IRatingable globalObject)
        {
            var session = Session;

            if (ratingable.UserRating != null)// && (ratingObject == null || ratingObject != null && ratingObject.Rating != ratingable.UserRating.Value))
            {
                var ratingObject = (from rating in session.Query<RatingUserValue>()
                                    where rating.ProfileId == profile.GlobalId && rating.RatedObjectId == globalObject.GlobalId
                                    select rating).SingleOrDefault();

                if (ratingObject == null)
                {
                    ratingObject = new RatingUserValue();
                    ratingObject.ProfileId = profile.GlobalId;
                    ratingObject.RatedObjectId = globalObject.GlobalId;
                }
                ratingObject.LoginData = SecurityInfo.LoginData;
                ratingObject.Rating = ratingable.UserRating.Value;
                ratingObject.ShortComment = ratingable.UserShortComment;
                ratingObject.VotedDate = Configuration.TimerService.UtcNow;
                session.SaveOrUpdate(ratingObject);
                ProfileStatisticsUpdater.UpdateVotings(session, profile);
                //session.SaveOrUpdate(globalObject);
                //var res = (from t in session.Query<RatingUserValue>() where t.RatedObjectId == globalObject.GlobalId select t).Average(t => t.Rating);
                var res = session.QueryOver<RatingUserValue>().Where(t => t.RatedObjectId == globalObject.GlobalId).
                    SelectList(t => t.SelectAvg(r => r.Rating)).SingleOrDefault<double>();
                globalObject.Rating = (float)res;
                session.SaveOrUpdate(globalObject);

                return (float)res;
            }
            return globalObject.Rating;
        }

        //void saveRating(SecurityInfo SecurityInfo, Service.V2.Model.IRatingable ratingable, Profile profile, BodyArchitect.Model.IRatingable globalObject)
        //{
        //    var session = Session;
        //    var ratingObject = (from rating in session.Query<RatingUserValue>()
        //                        where rating.ProfileId == profile.Id && rating.RatedObjectId == globalObject.GlobalId
        //                        select rating).SingleOrDefault();


        //    if (ratingable.UserRating != null)// && (ratingObject == null || ratingObject != null && ratingObject.Rating != ratingable.UserRating.Value))
        //    {
        //        if (ratingObject == null)
        //        {
        //            ratingObject = new RatingUserValue();
        //            ratingObject.ProfileId = profile.Id;
        //            ratingObject.RatedObjectId = globalObject.GlobalId;
        //        }
        //        ratingObject.LoginData = SecurityInfo.LoginData;
        //        ratingObject.Rating = ratingable.UserRating.Value;
        //        ratingObject.ShortComment = ratingable.UserShortComment;
        //        ratingObject.VotedDate = Configuration.TimerService.UtcNow;
        //        session.SaveOrUpdate(ratingObject);
        //        ProfileStatisticsUpdater.UpdateVotings(session, profile);
        //        //session.SaveOrUpdate(globalObject);
        //        //var res = (from t in session.Query<RatingUserValue>() where t.RatedObjectId == globalObject.GlobalId select t).Average(t => t.Rating);
        //        var res = session.QueryOver<RatingUserValue>().Where(t => t.RatedObjectId == globalObject.GlobalId).
        //            SelectList(t => t.SelectAvg(r => r.Rating)).SingleOrDefault<double>();
        //        globalObject.Rating = (float)res;
        //        ratingable.Rating = (float)res;
        //        session.SaveOrUpdate(globalObject);
        //    }
        //}

        //public SuplementDTO VoteSupplement(SuplementDTO supplement)
        //{
        //    Log.WriteWarning("VoteSupplement: Username={0},globalId={1}", SecurityInfo.SessionData.Profile.UserName, supplement.GlobalId);

        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        var dbProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        var planFromDb = session.Get<Suplement>(supplement.GlobalId);
        //        saveRating(SecurityInfo, supplement, dbProfile, planFromDb);

        //        tx.Commit();

        //        try
        //        {
        //            //send message only when someone else vote
        //            //if (planFromDb.Profile != null && planFromDb.Profile != dbProfile)
        //            //{
        //            //    if (planFromDb.Profile.Settings.NotificationExerciseVoted)
        //            //    {
        //            //        string param = string.Format("{0},{1},{2},{3}", planFromDb.Name, dbProfile.UserName, DateTime.Now, exercise.UserRating);
        //            //        MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
        //            //        messageService.SendSystemMessage(param, dbProfile, planFromDb.Profile, BodyArchitect.Model.MessageType.ExerciseVoted);
        //            //    }
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }

        //        session.Refresh(planFromDb);
        //        Mapper.Map(planFromDb, supplement);
        //        return supplement;
        //    }
        //}

        //public SupplementCycleDefinitionDTO VoteSupplementCycleDefinition(SupplementCycleDefinitionDTO definition)
        //{
        //    Log.WriteWarning("VoteSupplementCycleDefinition: Username={0},globalId={1}", SecurityInfo.SessionData.Profile.UserName, definition.GlobalId);

        //    var session = Session;
        //    using (var tx = session.BeginTransaction())
        //    {
        //        var dbProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.Id);
        //        var planFromDb = session.Get<SupplementCycleDefinition>(definition.GlobalId);
        //        saveRating(SecurityInfo, definition, dbProfile, planFromDb);

        //        tx.Commit();

        //        try
        //        {
        //            //send message only when someone else vote
        //            if (planFromDb.Profile != null && planFromDb.Profile != dbProfile)
        //            {
        //                if (planFromDb.Profile.Settings.NotificationWorkoutPlanVoted)
        //                {
        //                    string param = string.Format("{0},{1},{2},{3}", planFromDb.Name, dbProfile.UserName, DateTime.Now, definition.UserRating);
        //                    MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
        //                    messageService.SendSystemMessage(param, dbProfile, planFromDb.Profile, BodyArchitect.Model.MessageType.SupplementCycleDefinitionVoted);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionHandler.Default.Process(ex);
        //        }

        //        session.Refresh(planFromDb);
        //        Mapper.Map(planFromDb, definition);
        //        return definition;
        //    }
        //}

        public PagedResult<CommentEntryDTO> GetComments( Guid globalId, PartialRetrievingInfo retrievingInfo)
        {
            Log.WriteWarning("GetComments: Username={0},globalId={1}", SecurityInfo.SessionData.Profile.UserName, globalId);
            var session = Session;

            //var res = (from rv in session.Query<RatingUserValue>()
            //          from user in session.Query<Profile>()
            //          where rv.RatedObjectId == globalId && rv.ProfileId == user.Id
            //          orderby rv.VotedDate descending
            //          select new { RatingValue = rv, Profile = user });

            //int count = res.Count();

            //TODO: I split this on two queries because of Fetch and Count problem.
            var resCount = (from rv in session.Query<RatingUserValue>()
                            from user in session.Query<Profile>()
                            where rv.RatedObjectId == globalId && rv.ProfileId == user.GlobalId
                            orderby rv.VotedDate descending
                            select new { RatingValue = rv });

            int count = resCount.Count();

            var res = (from rv in session.Query<RatingUserValue>().Fetch(x => x.LoginData).ThenFetch(x => x.ApiKey)
                       from user in session.Query<Profile>()
                       where rv.RatedObjectId == globalId && rv.ProfileId == user.GlobalId
                       orderby rv.VotedDate descending
                       select new { RatingValue = rv, Profile = user });

            res = res.Skip(retrievingInfo.PageIndex * retrievingInfo.PageSize).Take(retrievingInfo.PageSize);
            List<CommentEntryDTO> comments = new List<CommentEntryDTO>();

            foreach (var item in res.ToList())
            {
                CommentEntryDTO comment = new CommentEntryDTO();
                comment.User = Mapper.Map<Profile, UserDTO>(item.Profile);
                comment.Rating = item.RatingValue.Rating;
                comment.ShortComment = item.RatingValue.ShortComment;
                comment.VotedDate = item.RatingValue.VotedDate;
                if (item.RatingValue.LoginData != null && item.RatingValue.LoginData.ApiKey != null)
                {
                    comment.ApplicationName = item.RatingValue.LoginData.ApiKey.ApplicationName;
                }
                comments.Add(comment);
            }

            PagedResult<CommentEntryDTO> result = new PagedResult<CommentEntryDTO>(comments, count, retrievingInfo.PageIndex);
            return result;

        }

        public VoteResult Vote(VoteParams voteParams)
        {
            Log.WriteWarning("Vote: Username={0},globalId={1},type={2}", SecurityInfo.SessionData.Profile.UserName, voteParams.GlobalId,voteParams.ObjectType);

            var session = Session;
            using (var tx = session.BeginSaveTransaction())
            {
                var dbProfile = session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                bool sendMessage = false;
                
                ISortable planFromDb = null;
                if(voteParams.ObjectType==VoteObject.SupplementCycleDefinition)
                {
                    planFromDb = session.Get<SupplementCycleDefinition>(voteParams.GlobalId);
                    sendMessage = true;
                }
                else if (voteParams.ObjectType == VoteObject.Exercise)
                {
                    planFromDb = session.Get<Exercise>(voteParams.GlobalId);
                    sendMessage = true;
                }
                else if (voteParams.ObjectType == VoteObject.WorkoutPlan)
                {
                    planFromDb = session.Get<TrainingPlan>(voteParams.GlobalId);
                    sendMessage = true;
                }
                else if (voteParams.ObjectType == VoteObject.Supplement)
                {
                    planFromDb = session.Get<Suplement>(voteParams.GlobalId);
                }
                ProfileNotification notificationType = planFromDb.Profile != null ? planFromDb.Profile.Settings.NotificationVoted : ProfileNotification.None;
                VoteResult result = new VoteResult();
                result .Rating= saveRating(SecurityInfo, voteParams, dbProfile, planFromDb);
                

                try
                {
                    //send message only when someone else vote
                    if (planFromDb.Profile != null && planFromDb.Profile != dbProfile && sendMessage)
                    {
                        //SendMessage(notificationType, dbProfile, planFromDb.Profile, messageFormat, messageTypeToSend.Value, "VoteEMailSubject", "VoteEMailMessage", DateTime.Now, dbProfile.UserName, planFromDb.Name, voteParams.UserRating);
                        NewSendMessageEx(notificationType, dbProfile, planFromDb.Profile, "VoteEMailSubject", "VoteEMailMessage", DateTime.Now, dbProfile.UserName, planFromDb.Name, voteParams.UserRating);

                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }

                tx.Commit();
                return result;
            }
        }

        

        public TrainingDayCommentDTO TrainingDayCommentOperation(TrainingDayCommentOperationParam arg)
        {
            
            Log.WriteWarning("TrainingDayCommentOperationParam:Username={0},operation: {1},TrainingDayId={2}", SecurityInfo.SessionData.Profile.UserName, arg.OperationType, arg.TrainingDayId);

            if (SecurityInfo.SessionData.Profile.GlobalId != arg.Comment.Profile.GlobalId)
            {
                throw new CrossProfileOperationException("Cannot add comment for another user");
            }
            var session = Session;
            if (string.IsNullOrWhiteSpace(arg.Comment.Comment))
            {
                throw new ArgumentNullException("Comment cannot be empty");
            }
            var dbComment = Mapper.Map<TrainingDayCommentDTO, TrainingDayComment>(arg.Comment);
            using (var tx = session.BeginSaveTransaction())
            {
                var trainingDay = session.Get<TrainingDay>(arg.TrainingDayId);
                var dbProfile = session.Get<Profile>(arg.Comment.Profile.GlobalId);
                if (trainingDay == null)
                {
                    throw new Shared.ObjectNotFoundException("Training Day doesn't exist");
                }
                if (!trainingDay.AllowComments)
                {
                    throw new InvalidOperationException("Comments are disabled for this training day");
                }

                if (arg.OperationType == TrainingDayOperationType.Add)
                {
                    if (!dbComment.IsNew)
                    {
                        throw new InvalidOperationException("Cannot add again existing comment");
                    }

                    dbComment.DateTime = Configuration.TimerService.UtcNow;
                    dbComment.TrainingDay = trainingDay;
                    dbComment.Profile = dbProfile;
                    dbComment.LoginData = SecurityInfo.LoginData;
                    session.SaveOrUpdate(dbComment);

                    trainingDay.LastCommentDate = dbComment.DateTime;
                    session.SaveOrUpdate(trainingDay);

                }
                ProfileStatisticsUpdater.UpdateBlogComments(session, dbProfile);
                ProfileStatisticsUpdater.UpdateMyBlogComments(session, trainingDay);
                

                try
                {
                    //send message only when someone else add comment (when blog autor add the comment we don't need to send the message)
                    if (trainingDay.Profile != dbProfile)
                    {
                        //if ((trainingDay.Profile.Settings.NotificationBlogCommentAdded & ProfileNotification.Message) == ProfileNotification.Message)
                        //{
                        //    string param = string.Format("{0},{1},{2}", trainingDay.TrainingDate.ToShortDateString(), dbProfile.UserName, dbComment.DateTime);
                        //    MessageService messageService = new MessageService(Session, SecurityInfo, Configuration, pushNotification);
                        //    messageService.SendSystemMessage(param, dbProfile, trainingDay.Profile, MessageType.TrainingDayCommentAdded);
                        //}
                        //if ((trainingDay.Profile.Settings.NotificationBlogCommentAdded & ProfileNotification.Email) == ProfileNotification.Email)
                        //{
                        //    emailService.SendEMail(trainingDay.Profile, "AddTrainingDayCommentEMailSubject", "AddTrainingDayCommentEMailMessage", DateTime.Now, dbProfile.UserName, trainingDay.TrainingDate, arg.Comment);
                        //}
                        //string messageFormat = string.Format("{0},{1},{2}", trainingDay.TrainingDate.ToShortDateString(), dbProfile.UserName, dbComment.DateTime);
                        //SendMessage(trainingDay.Profile.Settings.NotificationBlogCommentAdded, dbProfile, trainingDay.Profile, messageFormat, MessageType.TrainingDayCommentAdded, "AddTrainingDayCommentEMailSubject", "AddTrainingDayCommentEMailMessage", DateTime.Now, dbProfile.UserName, trainingDay.TrainingDate, arg.Comment);
                        NewSendMessageEx(trainingDay.Profile.Settings.NotificationBlogCommentAdded, dbProfile, trainingDay.Profile, "AddTrainingDayCommentEMailSubject", "AddTrainingDayCommentEMailMessage", DateTime.Now, dbProfile.UserName, trainingDay.TrainingDate.ToShortDateString(), arg.Comment.Comment);

                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex);
                }
                tx.Commit();
                return Mapper.Map<TrainingDayComment, TrainingDayCommentDTO>(dbComment);

            }
        }

        public PagedResult<TrainingDayCommentDTO> GetTrainingDayComments(TrainingDayInfoDTO day, PartialRetrievingInfo info)
        {
            Log.WriteWarning("GetTrainingDayComments:Username={0},training day id: {1}", SecurityInfo.SessionData.Profile.UserName, day.GlobalId);

            var session = Session;
            using (var tx = session.BeginGetTransaction())
            {

                var dbDay = session.Load<TrainingDay>(day.GlobalId);

                var query = session.QueryOver<TrainingDayComment>().Fetch(x => x.LoginData).Eager.Fetch(x => x.LoginData.ApiKey).Eager.Where(x => x.TrainingDay == dbDay).OrderBy(x => x.DateTime).Desc;

                var res = query.ToPagedResults<TrainingDayCommentDTO, TrainingDayComment>(info);
                tx.Commit();
                return res;
            }
        }
    }
}
