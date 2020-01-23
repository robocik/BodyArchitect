using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.Model
{
    public enum UsersSortOrder
    {
        ByName,
        ByTrainingDaysCount,
        ByFollowersCount,
        ByFriendsCount,
        ByBlogCommentsCount,
        ByVotingCount,
        ByLastEntryDate,
        ByLastLoginDate,
        ByWorkoutPlansCount
    }

    public enum PrivacyCriteria
    {
        All,
        Accessible,
        NotAccessible
    }

    public enum UserSearchGroup
    {
        Friends,
        Favorites,
        Others
    }
    public enum PictureCriteria
    {
        All,
        OnlyWithPicture,
        OnlyWithoutPicture
    }

    public enum UserWorkoutPlanCriteria
    {
        All,
        OnlyWithWorkoutPlans,
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class UserSearchCriteria
    {
        public UserSearchCriteria()
        {
            Countries=new List<int>();
            Genders=new List<Gender>();
            UserSearchGroups=new List<UserSearchGroup>();
        }

        public static UserSearchCriteria CreateAllCriteria()
        {
            UserSearchCriteria criteria = new UserSearchCriteria();
            return criteria;
        }

        [DataMember]
        public bool SortAscending { get; set; }

        [DataMember]
        public UsersSortOrder SortOrder { get; set; }

        [DataMember]
        public PrivacyCriteria AccessCalendar { get; set; }

        [DataMember]
        public PrivacyCriteria AccessSizes { get; set; }

        [DataMember]
        public PrivacyCriteria AccessFriends { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        [NotNullValidator]
        public List<UserSearchGroup> UserSearchGroups { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<int> Countries { get; private set; }

        [DataMember]
        [NotNullValidator]
        public List<Gender> Genders { get; set; }

        [DataMember]
        public PictureCriteria Picture { get; set; }

        [DataMember]
        public UserWorkoutPlanCriteria WorkoutPlan { get; set; }
    }
}
