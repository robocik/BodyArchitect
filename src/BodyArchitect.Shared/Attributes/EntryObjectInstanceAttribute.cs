using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    public enum EntryObjectInstance
    {
        Single,
        Multiple,
        None
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class EntryObjectInstanceAttribute:Attribute
    {
        EntryObjectInstance entryInstance;


        public EntryObjectInstanceAttribute():this(EntryObjectInstance.Single)
        {
        }

        public EntryObjectInstanceAttribute(EntryObjectInstance instance)
        {
            this.entryInstance = instance;
        }

        public EntryObjectInstance Instance
        {
            get
            {
                return entryInstance;
            }
            set 
            { 
                entryInstance = value;
            }
        
        }
    }
}
