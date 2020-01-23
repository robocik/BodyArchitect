using System;
using System.Collections.Generic;
using BodyArchitect.Model;

namespace BodyArchitect.Model
{
    [Serializable]
    public class Exercise : FMGlobalObject, ISortable
    {
        private ExerciseDifficult difficult=ExerciseDifficult.One;
        public readonly static Exercise Removed;

        static Exercise()
        {
            Removed = new Exercise(Guid.Empty);
            Removed.Name = "";

        }

        protected Exercise()
        {
        }

        public Exercise(Guid globalId)
        {
            this.GlobalId = globalId;
            Met = 5;//according to https://sites.google.com/site/compendiumofphysicalactivities/Activity-Categories/conditioning-exercise
        }
        

        #region Persistent properties

        //UTC
        public virtual DateTime CreationDate { get; set; }

        public virtual decimal Met { get; set; }

        public virtual float Rating
        {
            get; set;
        }

        public virtual string Name
        {
            get; set; 
        }

        public virtual bool UseInRecords
        {
            get;
            set;
        }
        public virtual bool IsDeleted { get; set; }

        public virtual int Version { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual string Description
        {
            get; set; 
        }


        public virtual string Url
        {
            get; set; 
        }

        public virtual MechanicsType MechanicsType
        {
            get; set;
        }

        public virtual ExerciseForceType ExerciseForceType
        {
            get;
            set;
        }

        public virtual ExerciseType ExerciseType
        {
            get;
            set;
        }

        public virtual string Shortcut
        {
            get; set;
        }


        public virtual ExerciseDifficult Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }
        #endregion
    }
}
