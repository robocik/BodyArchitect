using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public class ProfileStatistics:FMObject
    {
        public virtual DateTime? LastEntryDate { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }

        public virtual int LoginsCount { get; set; }

        public virtual int A6WEntriesCount { get; set; }


        //red star - Sport
        public virtual int TrainingDaysCount { get; set; }

        public virtual int StrengthTrainingEntriesCount { get; set; }

        public virtual int SupplementEntriesCount { get; set; }

        public virtual int SizeEntriesCount { get; set; }

        public virtual int A6WFullCyclesCount { get; set; }


        //blue star - Famous
        public virtual int FollowersCount { get; set; }

        public virtual int FriendsCount { get; set; }

        public virtual int MyBlogCommentsCount { get; set; }

        public virtual int BlogEntriesCount { get; set; }
        
        
        //green star - Social
        public virtual int VotingsCount { get; set; }

        public virtual int WorkoutPlansCount { get; set; }

        public virtual int BlogCommentsCount { get; set; }

    }
}
