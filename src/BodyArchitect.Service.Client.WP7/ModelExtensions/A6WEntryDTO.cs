using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public partial class A6WEntryDTO
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public BodyArchitect.Service.V2.Model.A6WDay Day
        {
            get
            {
                return this.DayField;
            }
            set
            {
                if (DayField == null || (this.DayField.Equals(value) != true))
                {
                    this.DayField = value;
                    this.RaisePropertyChanged("Day");
                }
            }
        }
    }
}
