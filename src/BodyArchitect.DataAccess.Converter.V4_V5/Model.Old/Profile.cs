using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
{
    public enum Gender { NotSet, Male, Female };

    public enum Role
    {
        User,
        PremiumUser,
        Instructor,
        Administrator
    }

    [Serializable]
    public class Profile : FMObject,IHasWymiar
    {
        private DateTime creationDate = DateTime.Now;


        public Profile()
        {
            FavoriteWorkoutPlans=new List<TrainingPlan>();
            //FavoriteExercises = new List<Exercise>();
            MyExercises=new List<Exercise>();
            MyWorkoutPlans = new List<TrainingPlan>();
            Friends=new List<Profile>();
            FavoriteUsers=new List<Profile>();
            Privacy=new ProfilePrivacy();
        }

        #region Persistent properties

        public virtual string ActivationId { get; set; }

        public virtual IList<TrainingPlan> FavoriteWorkoutPlans { get; set; }

        //public virtual IList<Exercise> FavoriteExercises { get; set; }

        public virtual IList<Exercise> MyExercises { get; set; }

        public virtual IList<TrainingPlan> MyWorkoutPlans { get; set; }

        public virtual IList<Profile> Friends { get; set; }

        public virtual IList<Profile> FavoriteUsers { get; set; }

        public virtual Guid? PreviousClientInstanceId { get; set; }

        public virtual Role Role
        {
            get;
            set;
        }

        public virtual bool IsDeleted { get; set; }

        public virtual ProfilePrivacy Privacy { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual Wymiary Wymiary
        {
            get;
            set;
        }

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
