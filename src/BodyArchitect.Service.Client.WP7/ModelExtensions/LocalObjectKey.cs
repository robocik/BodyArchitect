using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Client.WP7.ModelExtensions
{
    public enum KeyType
    {
        GlobalId,
        InstanceId
    };

    public class LocalObjectKey
    {
        public LocalObjectKey()
        {
            
        }
        public LocalObjectKey(Guid id, KeyType keyType)
        {
            Id = id;
            KeyType = keyType;
        }

        public LocalObjectKey(V2.Model.EntryObjectDTO newEntry)
        {
            if (newEntry.IsNew)
            {
                Id = newEntry.InstanceId;
                KeyType = KeyType.InstanceId;
            }
            else
            {
                Id = newEntry.GlobalId;
                KeyType = KeyType.GlobalId;
            }
        }

        public Guid Id { get; set; }

        public KeyType KeyType { get; set; }

        public override bool Equals(object obj)
        {
            LocalObjectKey otherKey = (LocalObjectKey) obj;
            return otherKey != null && otherKey.Id==Id && otherKey.KeyType==KeyType;
        }

        public override int GetHashCode()
        {
            return (Id.ToString() + KeyType.ToString()).GetHashCode();
        }
    }
}
