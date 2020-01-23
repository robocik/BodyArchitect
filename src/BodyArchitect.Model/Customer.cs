using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BodyArchitect.Model
{
    [Serializable]
    [DebuggerDisplay("{LastName} {FirstName}")]
    public class Customer : FMGlobalObject, IPerson, IHasReminder
    {
        public Customer()
        {
            Groups = new HashSet<CustomerGroup>();
            CreationDate = DateTime.UtcNow;
        }

        public virtual Address Address { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual bool IsVirtual { get; set; }

        public virtual CustomerSettings Settings
        {
            get;
            set;
        }

        public virtual Wymiary Wymiary
        {
            get;
            set;
        }
        //UTC
        public virtual DateTime CreationDate { get; set; }

        //UTC
        public virtual DateTime? Birthday { get; set; }

        public virtual int Version { get; set; }

        public virtual Profile ConnectedAccount { get; set; }

        public virtual string Email { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual DateTime? BirthdayDate { get { return this.Birthday; } }

        public virtual Gender Gender { get; set; }

        public virtual ICollection<CustomerGroup> Groups { get; set; }

        public virtual ReminderItem Reminder { get; set; }

    }
}
