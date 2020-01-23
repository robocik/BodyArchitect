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
    public class WymiaryDTO:BAObject
    {
        private DateTime dateTime = System.DateTime.Now;
        private bool isNaCzczo = true;

        #region Persistance properties

        [DataMember]
        public float Weight
        {
            get;
            set;
        }

        [DataMember]
        public float LeftBiceps
        {
            get;
            set;
        }

        [DataMember]
        public float RightUdo
        {
            get;
            set;
        }

        [DataMember]
        public float LeftUdo
        {
            get;
            set;
        }

        [DataMember]
        public float RightForearm
        {
            get;
            set;
        }

        [DataMember]
        public float LeftForearm
        {
            get;
            set;
        }

        [DataMember]
        public float RightBiceps
        {
            get;
            set;
        }

        [DataMember]
        public float Pas
        {
            get;
            set;
        }

        [DataMember]
        public float Klatka
        {
            get;
            set;
        }

        [DataMember]
        [NotForReportAttribiute]
        public bool IsNaCzczo
        {
            get { return isNaCzczo; }
            set { isNaCzczo = value; }
        }
        [DataMember]
        [NotForReportAttribiute]
        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        [DataMember]
        public int Height
        {
            get;
            set;
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

        #region Methods

        public virtual WymiaryDTO Clone(bool withId)
        {
            WymiaryDTO wymiary = new WymiaryDTO();
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
