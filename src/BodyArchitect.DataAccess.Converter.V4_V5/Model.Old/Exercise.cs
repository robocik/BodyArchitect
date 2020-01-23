using System;
using System.Collections.Generic;
using BodyArchitect.Model.Old;

namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class Exercise : IRatingable
    {
        private ExerciseDifficult difficult=ExerciseDifficult.One;
        public readonly static Exercise Removed;

        static Exercise()
        {
            Removed = new Exercise(Guid.Empty);
            Removed.Name = "";
        }

        private Exercise()
        {
        }

        public Exercise(Guid globalId)
        {
            this.GlobalId = globalId;
        }


        #region Persistent properties

        public virtual float Rating
        {
            get; set;
        }

        /// <summary>
        /// UTC TIME
        /// </summary>
        public virtual DateTime? PublishDate
        {
            get;
            set;
        }


        public virtual PublishStatus Status { get; set; }

        //[Property(NotNull = true, Length = Constants.NameColumnLength)]
        //[ValidateNonEmpty(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "Exercise_Name_Empty")]
        //[ValidateLength(1, Constants.NameColumnLength, ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "Exercise_Name_Length")]
        public virtual string Name
        {
            get; set; }

        public virtual int Version { get; set; }

        public virtual Profile Profile { get; set; }

        //[Property(SqlType = "ntext", ColumnType = "StringClob")]
        public virtual string Description
        {
            get; set; }

        //[Property(Length=1000)]
        //[ValidateLength(0, 1000, ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "Exercise_Url_Length")]
        public virtual string Url
        {
            get; set; }

        //[Property(NotNull = true)]
        public virtual MechanicsType MechanicsType
        {
            get; set;
        }

        //[Property(NotNull = true)]
        public virtual ExerciseForceType ExerciseForceType
        {
            get;
            set;
        }

        //[Property(NotNull = true)]
        public virtual ExerciseType ExerciseType
        {
            get;
            set;
        }


        //[PrimaryKey(PrimaryKeyType.Assigned)]
        public virtual Guid GlobalId
        {
            get; set; }

        //[Property(NotNull = true, Unique = true)]
        //[ValidateIsUnique(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "Exercise_Shortcut_Unique")]
        //[ValidateNonEmpty(ResourceType = typeof(LocalizedPropertyGridStrings), ErrorMessageKey = "Exercise_Shortcut_Empty")]
        public virtual string Shortcut
        {
            get; set; }


        //[Property(NotNull=true)]
        public virtual ExerciseDifficult Difficult
        {
            get { return difficult; }
            set { difficult = value; }
        }
        #endregion

        #region Methods

        public virtual Exercise Clone()
        {
            Exercise newExercise = new Exercise(GlobalId);
            newExercise.Url = Url;
            newExercise.Name = Name;
            newExercise.Shortcut = Shortcut;
            newExercise.Description = Description;
            newExercise.ExerciseType = ExerciseType;
            newExercise.Difficult = Difficult;
            return newExercise;
        }

        #endregion

        //#region Static methods

        //public static IList<Exercise> GetAll()
        //{
        //    return (IList<Exercise>)FindAll(typeof(Exercise));
        //}
        //#endregion
    }
}
