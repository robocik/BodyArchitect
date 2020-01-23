using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using NHibernate;

namespace BodyArchitect.Service.V2
{
    public static class ProfileStatisticsUpdater
    {
        public static void UpdateTrainindDay(ISession session,Profile profile)
        {
            session.Flush();
            string selectQuery = @"SELECT Max(TrainingDate) FROM TrainingDay WHERE Profile_id=:ProfileId AND Customer_id is null";
            string selectCountQuery = @"SELECT Count(*) FROM TrainingDay WHERE Profile_id=:ProfileId AND Customer_id is null";

            var selectSizeEntryCountQuery =@"SELECT Count(*) FROM TrainingDay td,EntryObject eo,SizeEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId AND td.Customer_id is null";
            var selectA6WEntryCountQuery = @"SELECT Count(*) FROM TrainingDay td,EntryObject eo,A6WEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId AND td.Customer_id is null";
            var selectStrengthTrainingEntryCountQuery = @"SELECT Count(*) FROM TrainingDay td,EntryObject eo,StrengthTrainingEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId";
            var selectSuplementsEntryCountQuery = @"SELECT Count(*) FROM TrainingDay td,EntryObject eo,SuplementsEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId AND td.Customer_id is null";
            var selectBlogEntryCountQuery =@"SELECT Count(*) FROM TrainingDay td,EntryObject eo,BlogEntry e WHERE e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId AND td.Profile_id=:ProfileId AND td.Customer_id is null";
            string selectA6WFullCyclesCountQuery = string.Format(@"select count(*) from A6WTraining where TrainingEnd={1} and PercentageCompleted=100  and profile_id=:ProfileId", profile.GlobalId, (int)TrainingEnd.Complete);

            var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET 
LastEntryDate=({0}),TrainingDaysCount=({1}), SizeEntriesCount=({2}), A6WEntriesCount=({3}), StrengthTrainingEntriesCount=({4}), SupplementEntriesCount=({5}), BlogEntriesCount=({6}), A6WFullCyclesCount=({7}) WHERE GlobalId=:StatisticsId",
 selectQuery, selectCountQuery, selectSizeEntryCountQuery, selectA6WEntryCountQuery, selectStrengthTrainingEntryCountQuery, selectSuplementsEntryCountQuery, selectBlogEntryCountQuery, selectA6WFullCyclesCountQuery));
            query.SetGuid("StatisticsId", profile.Statistics.GlobalId);
            query.SetGuid("ProfileId", profile.GlobalId);
            query.ExecuteUpdate();

        }

        public static void UpdateFollowers(ISession session, Profile profile)
        {
            session.Flush();
            string selectCountQuery = @"select count(*) from favoriteuserstofavoriteusers where child_profile_id=:ProfileId";

            var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET FollowersCount=({0}) WHERE GlobalId=:StatisticsId", selectCountQuery));
            query.SetGuid("StatisticsId", profile.Statistics.GlobalId);
            query.SetGuid("ProfileId", profile.GlobalId);
            query.ExecuteUpdate();
        }

        public static void UpdateFriends(ISession session, Profile inviter,Profile invited)
        {
            session.Flush();
            string selectCountQuery =@"select count(*) from friendsforprofile where child_profile_id=:ProfileId";

            var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET FriendsCount=({0}) WHERE GlobalId=:StatisticsId", selectCountQuery));
            query.SetGuid("StatisticsId", invited.Statistics.GlobalId);
            query.SetGuid("ProfileId", invited.GlobalId);
            query.ExecuteUpdate();

            selectCountQuery = @"select count(*) from friendsforprofile where child_profile_id=:ProfileId";

            query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET FriendsCount=({0}) WHERE GlobalId=:StatisticsId", selectCountQuery));
            query.SetGuid("StatisticsId", inviter.Statistics.GlobalId);
            query.SetGuid("ProfileId", inviter.GlobalId);
            query.ExecuteUpdate();
        }

        public static void UpdateWorkoutPlans(ISession session, Profile profile)
        {
            //session.Flush();
            //string selectCountQuery = string.Format(@"select count(*) from trainingplan where Status={1} and profile_id={0}", profile.Id,(int)PublishStatus.Published);

            //var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET WorkoutPlansCount=({0}) WHERE Id=:StatisticsId", selectCountQuery));
            //query.SetInt32("StatisticsId", profile.Statistics.Id);
            //query.ExecuteUpdate();

            var count = session.QueryOver<TrainingPlan>().Where(x => x.Profile == profile && x.Status == PublishStatus.Published).RowCount();
            profile.Statistics.WorkoutPlansCount = count;
        }

        public static void UpdateSupplementsDefinitions(ISession session, Profile profile)
        {
            //session.Flush();
            //string selectCountQuery = string.Format(@"select count(*) from trainingplan where Status={1} and profile_id={0}", profile.Id,(int)PublishStatus.Published);

            //var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET WorkoutPlansCount=({0}) WHERE Id=:StatisticsId", selectCountQuery));
            //query.SetInt32("StatisticsId", profile.Statistics.Id);
            //query.ExecuteUpdate();

            var count = session.QueryOver<SupplementCycleDefinition>().Where(x => x.Profile == profile && x.Status == PublishStatus.Published).RowCount();
            profile.Statistics.SupplementsDefinitionsCount = count;
        }

        public static void UpdateBlogComments(ISession session, Profile profile)
        {
            //session.Flush();
            //string selectCountQuery = string.Format(@"select count(*) from blogcomment where profile_id={0}", profile.Id, (int)PublishStatus.Published);

            //var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET TrainingDayCommentsCount=({0}) WHERE Id=:StatisticsId", selectCountQuery));
            //query.SetInt32("StatisticsId", profile.Statistics.Id);
            //query.ExecuteUpdate();

            var count = session.QueryOver<TrainingDayComment>().Where(x => x.Profile == profile).RowCount();
            profile.Statistics.TrainingDayCommentsCount = count;
        }

        public static void UpdateMyBlogComments(ISession session, TrainingDay trainingDay)
        {
            session.Flush();
            //var selectBlogEntryCountQuery = string.Format(@"SELECT Count(*) FROM TrainingDayComment bc, TrainingDay td,EntryObject eo,BlogEntry e WHERE bc.Profile_id<>td.Profile_id AND bc.BlogEntry_id=e.EntryObject_id AND e.EntryObject_id=eo.Id AND eo.TrainingDay_id=td.Id AND eo.Id={0}", blogEntry.Id);
            //var selectBlogEntryCountQuery = string.Format(@"SELECT Count(*) FROM TrainingDayComment bc, TrainingDay td,EntryObject eo,BlogEntry e WHERE td.Profile_id={0} AND bc.Profile_id<>td.Profile_id AND bc.BlogEntry_id=e.EntryObject_id AND e.EntryObject_id=eo.GlobalId AND eo.TrainingDay_id=td.GlobalId  AND td.Customer_id is null", blogEntry.TrainingDay.Profile.Id);
            var selectBlogEntryCountQuery = @"SELECT Count(*) FROM TrainingDayComment bc, TrainingDay td WHERE td.Profile_id=:ProfileId AND bc.Profile_id<>td.Profile_id AND bc.TrainingDay_id=td.GlobalId AND td.Customer_id is null";
            var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET MyTrainingDayCommentsCount=({0}) WHERE GlobalId=:StatisticsId", selectBlogEntryCountQuery));
            query.SetGuid("StatisticsId", trainingDay.Profile.Statistics.GlobalId);
            query.SetGuid("ProfileId", trainingDay.Profile.GlobalId);
            query.ExecuteUpdate();
        }

        public static void UpdateVotings(ISession session, Profile profile)
        {
            //session.Flush();
            //string selectCountQuery = string.Format(@"select count(*) from ratinguservalue where profileId={0}", profile.Id, (int)PublishStatus.Published);

            //var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET VotingsCount=({0}) WHERE Id=:StatisticsId", selectCountQuery));
            //query.SetInt32("StatisticsId", profile.Statistics.Id);
            //query.ExecuteUpdate();

            var count = session.QueryOver<RatingUserValue>().Where(x => x.ProfileId == profile.GlobalId).RowCount();
            profile.Statistics.VotingsCount = count;
        }

        public static void UpdateLogins(ISession session, Profile profile)
        {
            //session.Flush();
            //string selectCountQuery = string.Format(@"select count(*) from LoginData where ProfileId={0}", profile.Id);
            var count = session.QueryOver<LoginData>().Where(x => x.ProfileId == profile.GlobalId).RowCount();
            //var query = session.CreateSQLQuery(string.Format(@"UPDATE ProfileStatistics SET LoginsCount=({0}) WHERE Id=:StatisticsId", selectCountQuery));
            //query.SetInt32("StatisticsId", profile.Statistics.Id);
            //query.ExecuteUpdate();
            profile.Statistics.LoginsCount = count;

        }
    }
}
