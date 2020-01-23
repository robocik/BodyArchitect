using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class ProfileStatisticsDTO:BAObject
    {
        [DataMember]
        public DateTime? LastEntryDate { get; set; }

        [DataMember]
        public int TrainingDaysCount { get; set; }

        [DataMember]
        public int FollowersCount { get; set; }

        [DataMember]
        public int WorkoutPlansCount { get; set; }

        [DataMember]
        public int BlogCommentsCount { get; set; }

        [DataMember]
        public int VotingsCount { get; set; }

        [DataMember]
        public DateTime? LastLoginDate { get; set; }

        [DataMember]
        public int FriendsCount { get; set; }

        [DataMember]
        public int StrengthTrainingEntriesCount { get; set; }

        [DataMember]
        public int BlogEntriesCount { get; set; }

        [DataMember]
        public int SupplementEntriesCount { get; set; }

        [DataMember]
        public int SizeEntriesCount { get; set; }

        [DataMember]
        public int A6WEntriesCount { get; set; }

        [DataMember]
        public int A6WFullCyclesCount { get; set; }

        [DataMember]
        public int LoginsCount { get; set; }

        [DataMember]
        public int MyBlogCommentsCount { get; set; }
    }
}
