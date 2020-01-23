using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class GetRemindersParam
    {
        public GetRemindersParam()
        {
            Types = new List<ReminderType>();
        }

        //[DataMember]
        //public TimeSpan? ValidForTime { get; set; }

        [DataMember]
        public List<ReminderType> Types { get; set; }
    }
}
