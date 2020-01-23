using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Shared
{
    [Flags]
    public enum EntryObjectOperation
    {
        None = 0,
        Copy = 1,
        Move = 2,
        
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntryObjectOperationsAttribute : Attribute
    {
        EntryObjectOperation operations = EntryObjectOperation.Copy | EntryObjectOperation.Move;

        public EntryObjectOperationsAttribute()
        {
        }

        public EntryObjectOperationsAttribute(EntryObjectOperation operations)
        {
            this.operations = operations;
        }

        public EntryObjectOperation Operations
        {
            get
            {
                return operations;
            }
            set
            {
                operations = value;
            }
        }
    }
}
