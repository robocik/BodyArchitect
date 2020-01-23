using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model
{
    public enum ProfileStatusType
    {//TODO:Add more statuses
        Normal
    }

    public class ProfileStatus
    {
        public ProfileStatusType Type { get; set; }

        public string Status { get; set; }
    }

    public class ProfileStatistics:FMGlobalObject
    {
        public ProfileStatistics()
        {
            Status= new ProfileStatus();
        }

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

        public virtual int MyTrainingDayCommentsCount { get; set; }

        public virtual int BlogEntriesCount { get; set; }
        
        
        //green star - Social
        public virtual int VotingsCount { get; set; }

        public virtual int SupplementsDefinitionsCount { get; set; }

        public virtual int WorkoutPlansCount { get; set; }

        public virtual int TrainingDayCommentsCount { get; set; }

        public virtual ProfileStatus Status { get; set; }
    }
}
