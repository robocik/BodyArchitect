using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class MeasurementsViewModel:ViewModelBase
    {
        private SizeEntryDTO entry;

        public MeasurementsViewModel(SizeEntryDTO entry)
        {
            this.entry = entry;
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine; }
        }

        public bool ReadOnly
        {
            get { return !EditMode; }
        }

        public string TrainingDate
        {
            get { return Entry.TrainingDay.TrainingDate.ToLongDateString(); }
        }

        public SizeEntryDTO Entry
        {
            get { return entry; }
        }
    }
}
