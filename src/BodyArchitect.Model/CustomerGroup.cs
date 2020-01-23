using System;
using System.Collections.Generic;

namespace BodyArchitect.Model
{
    public enum CustomerGroupRestrictedType
    {
        /// <summary>
        /// One customer can belong to many different groups
        /// </summary>
        None,
        /// <summary>
        /// One customer which belong to Partially restricted group can belong to many Not restricted group only
        /// </summary>
        Partially,
        /// <summary>
        /// One customer which belong to Fully restricted group cannot belong to any other groups
        /// </summary>
        Full
    }

    [Serializable]
    public class CustomerGroup:FMGlobalObject
    {
        public CustomerGroup()
        {
            Customers = new HashSet<Customer>();
            CreationDate = DateTime.UtcNow;
        }

        public virtual string Color { get; set; }

        public virtual string Name { get; set; }

        public virtual Activity DefaultActivity { get; set; }

        public virtual CustomerGroupRestrictedType RestrictedType { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual int MaxPersons { get; set; }

        //UTC
        public virtual DateTime CreationDate { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual int Version { get; set; }
    }
}
