using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.A6W.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    /// <summary>
    /// Interaction logic for usrA6W.xaml
    /// </summary>
    public partial class usrA6W : IValidationControl
    {
        private bool IsTrainingInProgress1;
        private Dictionary<int, A6WEntryDTO> entries;

        public usrA6W()
        {
            InitializeComponent();
        }

        public usrA6WDetails DetailsControl
        {
            get { return (usrA6WDetails) this.detailsControl; }
        }

        public override bool HasDetailsPane
        {
            get { return true; }
        }


        public override bool HasProgressPane
        {
            get { return true; }
        }

        public A6WEntryDTO A6WEntry
        {
            get { return (A6WEntryDTO)Entry; }
        }

        protected override UI.UserControls.usrEntryObjectUserControl CreateProgressControl()
        {
            return new usrA6WProgress();
        }

        protected override UI.UserControls.usrEntryObjectDetailsBase CreateDetailsControl()
        {
            var control= new usrA6WDetails();
            control.SetA6WControl(this);
            return control;
        }

        public override void AfterSave(bool isWindowClosing)
        {
            var cache = MyTrainingsReposidory.GetCache(A6WEntry.TrainingDay.CustomerId, A6WEntry.TrainingDay.ProfileId);
            cache.ClearCache();
        }


        public MyTrainingDTO MyTraining
        {
            get
            {
                var cache = MyTrainingsReposidory.GetCache(A6WEntry.TrainingDay.CustomerId, A6WEntry.TrainingDay.ProfileId);
                return cache.GetItem(A6WEntry.MyTraining.GlobalId);
            }
        }

        private void fillDaysCombo()
        {
            lvA6W.Items.Clear();
            MyTrainingDTO fullMyTraining=null;

            if (!A6WEntry.MyTraining.IsNew)
            {
                fullMyTraining = MyTraining;
            }
            
            if(fullMyTraining==null)
            {
                return;
            }

            entries = fullMyTraining.EntryObjects.Cast<A6WEntryDTO>().ToDictionary(x => x.Day.DayNumber);
            
            foreach (A6WDay day in A6WManager.Days)
            {
                A6WItemViewModel item = new A6WItemViewModel(day, A6WEntry);

                if (entries.ContainsKey(day.DayNumber))
                {
                    string icon = "pack://application:,,,/BodyArchitect.Client.Module.A6W;component/Images/{0}";
                    item.Icon = string.Format(icon, entries[day.DayNumber].Completed ? "Completed.png" : "PartialCompleted.png");
                }

                lvA6W.Items.Add(item);
            }
        }

        internal A6WDay? GetSelectedA6wDay()
        {
            if (!Dispatcher.CheckAccess())
            {
                return (A6WDay?)Dispatcher.Invoke(new Func<A6WDay?>(GetSelectedA6wDay));
            }
            else
            {
                foreach (A6WItemViewModel item in lvA6W.Items)
                {
                    if (item.IsCurrent)
                    {
                        return item.A6wDay;
                    }
                }
                return null;
            }
        }

        public bool IsTrainingInProgress
        {
            get { return IsTrainingInProgress1; }
            set
            {
                IsTrainingInProgress1 = value;
                updateReadOnlyForTrainingProgress(value);
            }
        }

        private void updateReadOnlyForTrainingProgress(bool readOnly)
        {
            if (DetailsControl != null)
            {
                DetailsControl.usrMyTrainingStatus1.ReadOnly = readOnly || entries.Count == 0;
                DetailsControl.panel1.IsEnabled = !readOnly;
                DetailsControl.txtComment.IsReadOnly = readOnly;
                DetailsControl.usrReportStatus1.ReadOnly = readOnly;
            }
        }

        protected override void FillImplementation(SaveTrainingDayResult originalEntry)
        {
            ReadOnly = ReadOnly || Entry.MyTraining.TrainingEnd != TrainingEnd.NotEnded;

            fillDaysCombo();
        }

        

        public bool ValidateControl()
        {
            return DetailsControl==null || DetailsControl.ValidateControl(Entry);
        }

        

        private void lvA6W_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvA6W.SelectedItems.Clear();
        }

        
    }

    public class A6WItemViewModel:ViewModelBase
    {
        private A6WEntryDTO entry;
        private A6WDay day;
        private string icon;

        public A6WItemViewModel(A6WDay day,A6WEntryDTO entry)
        {
            this.day = day;
            this.entry = entry;
        }

        public int Day
        {
            get { return day.DayNumber; }
        }

        public int Sets
        {
            get { return day.SetNumber; }
        }

        public int Repetitions {get { return day.RepetitionNumber; }}

        public string Icon
        {
            get
            {
                return icon;
            }
            set { icon=value; }
        }

        public bool IsCurrent
        {
            get { return this.entry.Day.DayNumber == day.DayNumber; }
        }

        public A6WDay A6wDay
        {
            get { return day; }
        }
    }
}
