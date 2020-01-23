using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Reminders.Controls
{
    

    public class ReminderItemViewModel:ViewModelBase
    {
        private ReminderItemDTO reminder;

        public ReminderItemViewModel(ReminderItemDTO reminder)
        {
            this.Reminder = reminder;
        }


        public ReminderItemDTO Reminder
        {
            get { return reminder; }
            set { reminder = value; }
        }

        
    }
}
