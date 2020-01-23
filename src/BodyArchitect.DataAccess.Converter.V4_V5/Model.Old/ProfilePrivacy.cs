using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Model.Old
{
    public enum Privacy
    {
        Private,
        FriendsOnly,
        Public
    }

    public class ProfilePrivacy
    {
        public ProfilePrivacy()
        {
            Searchable = true;
        }

        public Privacy CalendarView { get; set; }

        public Privacy Sizes { get; set; }

        public Privacy Friends { get; set; }

        public Privacy BirthdayDate { get; set; }
        
        public bool Searchable { get; set; }

    }
}
