using System;
using BodyArchitect.Shared;

namespace BodyArchitect.Model
{
    public enum TimeType
    {
        NotSet,
        OnEmptyStomach,
        BeforeWorkout,
        AfterWorkout,
        BeforeSleep
    }

    public class BATime
    {
        public virtual DateTime DateTime
        {
            get; set; 
        }

        public virtual TimeType TimeType
        {
            get; set;
        }
    }

    [Serializable]
    public class Wymiary : FMGlobalObject
    {
        public Wymiary()
        {
            Time=new BATime();
        }

        #region Persistance properties

        public virtual decimal Weight
        {
            get; set;
        }

        public virtual decimal LeftBiceps
        {
            get; set;
        }

        public virtual decimal RightUdo
        {
            get; set;
        }

        public virtual decimal LeftUdo
        {
            get; set;
        }

        public virtual decimal RightForearm
        {
            get; set;
        }

        public virtual decimal LeftForearm
        {
            get; set;
        }

        public virtual decimal RightBiceps
        {
            get; set;
        }

        public virtual decimal Pas
        {
            get; set;
        }

        public virtual decimal Klatka
        {
            get; set;
        }

        [NotForReportAttribiute]
        public virtual BATime Time
        {
            get; set;
        }


        public virtual decimal Height
        {
            get; set;
        }

        #endregion

        #region Properties

        [NotForReportAttribiute]
        public virtual bool IsEmpty
        {
            get
            {
                return Height == 0 && Weight == 0 && LeftBiceps == 0 && RightBiceps == 0 && Klatka == 0 && RightUdo == 0 && LeftUdo == 0 && RightForearm == 0 && LeftForearm == 0 && Pas == 0;
            }
        }
        #endregion


    }
}
