using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;

namespace BodyArchitect.Service.V2.Model
{
    public partial class BAGlobalObject
    {
        public bool IsSame(LocalObjectKey  key)
        {
            if (key == null)
            {
                return false;
            }
            return (key.KeyType==KeyType.InstanceId && InstanceId == key.Id) || GlobalId == key.Id;
        }
        public bool IsSame(BAGlobalObject otherObj)
        {
            if (otherObj == null)
            {
                return false;
            }
            return (IsNew && InstanceId == otherObj.InstanceId) || GlobalId == otherObj.GlobalId;
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [DoNotChecksum]
        [SerializerId]
        public Guid GlobalId
        {
            get
            {
                return this.GlobalIdField;
            }
            set
            {
                if ((this.GlobalIdField.Equals(value) != true))
                {
                    this.GlobalIdField = value;
                    this.RaisePropertyChanged("GlobalId");
                }
            }
        }

        [DoNotChecksum]
        [DoNotSerialize]
        public bool IsNew
        {
            get { return GlobalId == Guid.Empty; }
        }

        [DoNotChecksum]
        [SerializerId]
        private Guid instanceId;

        [DataMember]
        [DoNotChecksum]
        [SerializerId]
        public Guid InstanceId
        {
            get
            {
                if(instanceId==Guid.Empty)
                {
                    instanceId = Guid.NewGuid();
                }
                return instanceId;
            }
            set { instanceId = value; }
        }

        [DoNotChecksum]
        [DoNotSerialize]
        public object Tag { get; set; }
    }
}
