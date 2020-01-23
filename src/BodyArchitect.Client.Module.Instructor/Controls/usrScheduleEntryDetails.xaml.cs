using System;
using System.Collections.Generic;
using System.Globalization;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Converters;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Converters;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for usrScheduleEntryDetails.xaml
    /// </summary>
    public partial class usrScheduleEntryDetails : IEditableControl
    {
        private ScheduleEntryDTO entry;

        public usrScheduleEntryDetails()
        {
            InitializeComponent();
        }

        public void Fill(ScheduleEntryBaseDTO entry)
        {
            Object = entry;
        }

        public object Object
        {
            get { return entry; }
            set
            {
                entry = (ScheduleEntryDTO) value;
                DataContext = entry;
            }
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public object Save()
        {
            return entry;
        }
    }

}
