using System;


namespace BodyArchitect.Model.Old
{
    [Serializable]
    public class Wymiary : FMObject
    {
        private DateTime dateTime=System.DateTime.Now;
        private bool isNaCzczo=true;

        #region Persistance properties

        public virtual float Weight
        {
            get; set;
        }

        public virtual float LeftBiceps
        {
            get; set;
        }

        public virtual float RightUdo
        {
            get; set;
        }

        public virtual float LeftUdo
        {
            get; set;
        }

        public virtual float RightForearm
        {
            get; set;
        }

        public virtual float LeftForearm
        {
            get; set;
        }

        public virtual float RightBiceps
        {
            get; set;
        }

        public virtual float Pas
        {
            get; set;
        }

        public virtual float Klatka
        {
            get; set;
        }

        public virtual bool IsNaCzczo
        {
            get { return isNaCzczo; }
            set { isNaCzczo = value; }
        }

        public virtual DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public virtual int Height
        {
            get; set;
        }

        #endregion

        #region Properties

        public virtual bool IsEmpty
        {
            get
            {
                return Height == 0 && Weight == 0 && LeftBiceps == 0 && RightBiceps == 0 && Klatka == 0 && RightUdo == 0 && LeftUdo == 0 && RightForearm == 0 && LeftForearm == 0 && Pas == 0;
            }
        }
        #endregion

        #region Methods

        public virtual Wymiary Clone(bool withId)
        {
            Wymiary wymiary = new Wymiary();
            if (withId)
            {
                wymiary.Id = Id;
            }
            wymiary.Weight = Weight;
            wymiary.isNaCzczo = isNaCzczo;
            wymiary.Klatka = Klatka;
            wymiary.RightBiceps = RightBiceps;
            wymiary.LeftBiceps = LeftBiceps;
            wymiary.Pas = Pas;
            wymiary.RightUdo = RightUdo;
            wymiary.LeftUdo = LeftUdo;
            wymiary.RightForearm = RightForearm;
            wymiary.LeftForearm = LeftForearm;
            wymiary.dateTime = dateTime;
            wymiary.Height = Height;
            return wymiary;
        }
        #endregion

    }
}
