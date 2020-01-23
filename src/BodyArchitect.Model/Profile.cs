using System;
using System.Collections.Generic;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum Gender { NotSet, Male, Female };

    public enum AccountType
    {
        User,
        PremiumUser,
        Instructor,
        Administrator
    }

    [Serializable]
    public class LicenceInfo
    {
        public DateTime LastPointOperationDate { get; set; }

        public AccountType AccountType { get; set; }

        public int BAPoints { get; set; }
    }

    [Serializable]
    public class Profile : FMGlobalObject,IPerson,IVersionable
    {
        private DateTime creationDate = DateTime.UtcNow;
        
        public Profile()
        {
            FavoriteWorkoutPlans = new HashSet<TrainingPlan>();
            FavoriteExercises = new HashSet<Exercise>();
            MyExercises = new HashSet<Exercise>();
            MyWorkoutPlans = new HashSet<TrainingPlan>();
            MySupplementsDefinitions = new HashSet<SupplementCycleDefinition>();
            Friends = new HashSet<Profile>();
            FavoriteUsers = new HashSet<Profile>();
            FavoriteSupplementCycleDefinitions = new HashSet<SupplementCycleDefinition>();
            BAPoints = new HashSet<BAPoints>();
            Privacy=new ProfilePrivacy();
            Licence = new LicenceInfo();
        }

        #region Persistent properties

        

        public virtual LicenceInfo Licence { get; set; }
        
        public virtual string ActivationId { get; set; }

        public virtual ICollection<TrainingPlan> FavoriteWorkoutPlans { get; set; }

        public virtual ICollection<BAPoints> BAPoints { get; set; }

        public virtual ICollection<SupplementCycleDefinition> MySupplementsDefinitions { get; set; }

        public virtual ICollection<SupplementCycleDefinition> FavoriteSupplementCycleDefinitions { get; set; }

        public virtual ICollection<Exercise> FavoriteExercises { get; set; }

        public virtual ICollection<Exercise> MyExercises { get; set; }

        public virtual ICollection<TrainingPlan> MyWorkoutPlans { get; set; }

        public virtual ICollection<Profile> Friends { get; set; }

        public virtual ICollection<Profile> FavoriteUsers { get; set; }

        public virtual Guid? PreviousClientInstanceId { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual ProfilePrivacy Privacy { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual Wymiary Wymiary
        {
            get;
            set;
        }

        public virtual DataInfo DataInfo { get; set; }

        public virtual ProfileSettings Settings
        {
            get;
            set;
        }

        public virtual string UserName
        {
            get;
            set;
        }

        public virtual Gender Gender { get; set; }

        public virtual string Password
        {
            get;
            set;
        }

        public virtual DateTime Birthday
        {
            get;
            set;
        }

        public virtual DateTime? BirthdayDate { get { return this.Birthday; } }

        public virtual int CountryId { get; set; }

        public virtual string Email { get; set; }

        public virtual DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        public virtual int Version { get; set; }


        public virtual string AboutInformation
        {
            get; set; }

        public virtual object Tag { get; set; }

        public virtual bool IsActivated
        {
            get { return string.IsNullOrWhiteSpace(ActivationId); }
            
        }

        public virtual ProfileStatistics Statistics { get; set; }

        #endregion

    }
}
