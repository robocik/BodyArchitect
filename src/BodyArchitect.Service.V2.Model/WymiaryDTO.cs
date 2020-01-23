using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace BodyArchitect.Service.V2.Model
{
    public enum TimeType
    {
        NotSet,
        OnEmptyStomach,
        BeforeWorkout,
        AfterWorkout,
        BeforeSleep
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class BATimeDTO
    {
        [DataMember]
        public DateTime DateTime
        {
            get; set;
        }

        [DataMember]
        public TimeType TimeType
        {
            get; set;
        }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    [Serializable]
    public class WymiaryDTO:BAGlobalObject
    {
        public WymiaryDTO()
        {
            Time=new BATimeDTO();
        }

        #region Persistance properties

        [DataMember]
        public decimal Weight
        {
            get;
            set;
        }

        [DataMember]
        public decimal LeftBiceps
        {
            get;
            set;
        }

        [DataMember]
        public decimal RightUdo
        {
            get;
            set;
        }

        [DataMember]
        public decimal LeftUdo
        {
            get;
            set;
        }

        [DataMember]
        public decimal RightForearm
        {
            get;
            set;
        }

        [DataMember]
        public decimal LeftForearm
        {
            get;
            set;
        }

        [DataMember]
        public decimal RightBiceps
        {
            get;
            set;
        }

        [DataMember]
        public decimal Pas
        {
            get;
            set;
        }

        [DataMember]
        public decimal Klatka
        {
            get;
            set;
        }

        [DataMember]
        [NotForReportAttribiute]
        [ObjectValidator]
        [NotNullValidator]
        public BATimeDTO Time
        {
            get; set;
        }

        [DataMember]
        public decimal Height
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
                wymiary.GlobalId =GlobalId;
            }
            wymiary.Weight = Weight;
            wymiary.Time.DateTime = Time.DateTime;
            wymiary.Time.TimeType = Time.TimeType;
            wymiary.Klatka = Klatka;
            wymiary.RightBiceps = RightBiceps;
            wymiary.LeftBiceps = LeftBiceps;
            wymiary.Pas = Pas;
            wymiary.RightUdo = RightUdo;
            wymiary.LeftUdo = LeftUdo;
            wymiary.RightForearm = RightForearm;
            wymiary.LeftForearm = LeftForearm;
            wymiary.Height = Height;
            return wymiary;
        }
        #endregion
    }
}
