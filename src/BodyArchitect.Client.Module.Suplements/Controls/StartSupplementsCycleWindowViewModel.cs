using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Settings.Model;

namespace BodyArchitect.Client.Module.Suplements.Controls
{
    public class StartSupplementsCycleWindowViewModel : ViewModelBase, IRemindable
    {
        private SupplementCycleDefinitionDTO _cycleDefinition;
        private string _weightUnit;
        private decimal _weight;
        ObservableCollection<IntListItem<DayOfWeek>> daysOfWeek = new ObservableCollection<IntListItem<DayOfWeek>>();
        private DateTime _startDate;
        private TimeSpan? _remindBefore;
        private int _totalWeeks;
        private bool _cannotBeLonger;
        private bool allowChangePlan;
        private CustomerDTO _customer;


        public StartSupplementsCycleWindowViewModel()
        {
            if (UserContext.Current.ProfileInformation.Wymiary != null)
            {
                Weight = UserContext.Current.ProfileInformation.Wymiary.Weight.ToDisplayWeight();
            }
            if (UserContext.Current.ProfileInformation.Settings.WeightType == WeightType.Pounds)
            {
                WeightUnit = Strings.WeightType_Pound;
            }
            else
            {
                WeightUnit = Strings.WeightType_Kg;
            }
            StartDate = DateTime.Now;

            foreach (DayOfWeek value in Enum.GetValues(typeof(DayOfWeek)))
            {
                daysOfWeek.Add(new IntListItem<DayOfWeek>(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(value), value));
            }
        }


        public SupplementCycleDefinitionDTO CycleDefinition
        {
            get { return _cycleDefinition; }
            set
            {
                _cycleDefinition = value;
                if (_cycleDefinition != null)
                {
                    TotalWeeks = value.GetTotalWeeks();
                    CannotBeLonger = _cycleDefinition.Weeks.Where(x => x.IsRepetitable).Count() == 0;
                }
                NotifyOfPropertyChange(()=>CycleDefinition);
            }
        }

        public bool CannotBeLonger
        {
            get { return _cannotBeLonger; }
            set
            {
                _cannotBeLonger = value;
                NotifyOfPropertyChange(() => CannotBeLonger);
            }
        }

        public int TotalWeeks
        {
            get { return _totalWeeks; }
            set
            {
                _totalWeeks = value;
                NotifyOfPropertyChange(() => TotalWeeks);
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public string WeightUnit
        {
            get { return _weightUnit; }
            set
            {
                _weightUnit = value;
                NotifyOfPropertyChange(()=>WeightUnit);
            }
        }

        public decimal Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                NotifyOfPropertyChange(() => Weight);
            }
        }

        public IEnumerable<IntListItem<DayOfWeek>> DaysOfWeek
        {
            get { return daysOfWeek; }
        }

        public void StartCycle(Dispatcher dispatcher)
        {
            var result=startCycle(MyTrainingOperationType.Start);
            //now refresh Training days cache
            var customerId = Customer != null ? Customer.GlobalId : (Guid?)null;
            var myCache = TrainingDaysReposidory.GetCache(customerId, null);
            myCache.Clear();
            MyTrainingsReposidory.GetCache(customerId, null).AddOrUpdate(result);
        }
        SupplementsCycleDTO startCycle(MyTrainingOperationType operation)
        {
            var cycle = new SupplementsCycleDTO();
            cycle.RemindBefore = RemindBefore;
            var param = new MyTrainingOperationParam();
            param.Operation = operation;
            param.MyTraining = cycle;
            if (Customer != null)
            {
                cycle.CustomerId = Customer.GlobalId;
            }
            cycle.TrainingDays = getTrainingDays();
            cycle.TotalWeeks = TotalWeeks;
            cycle.SupplementsCycleDefinitionId = CycleDefinition.GlobalId;
            cycle.Name = CycleDefinition.Name;//TODO:user should allow change this name
            cycle.StartDate = StartDate.ToUniversalTime();
            cycle.Weight =((double?) Weight).ToSaveWeight();
            return (SupplementsCycleDTO) ServiceManager.MyTrainingOperation(param);
        }

        string getTrainingDays()
        {
            var trainingDays = DaysOfWeek.Where(x => x.IntValue > 0).Select(x => (int)x.Value + intToTrainingType(x.IntValue)).ToList();
            return string.Join(",", trainingDays);
        }

        string intToTrainingType(int value)
        {
            switch (value)
            {
                case 1:
                    return "S";
                case 2:
                    return "C";
                case 3:
                    return "B";
            }
            return null;
        }

        public SupplementsCycleDTO SimulateCycle()
        {
            return startCycle(MyTrainingOperationType.Simulate);
        }

        public TimeSpan? RemindBefore
        {
            get { return _remindBefore; }
            set
            {
                _remindBefore = value;
                NotifyOfPropertyChange(() => RemindBefore);
            }
        }

        public bool AllowChangePlan
        {
            get { return allowChangePlan; }
            set
            {
                allowChangePlan = value;
                NotifyOfPropertyChange(() => AllowChangePlan);
            }
        }

        public CustomerDTO Customer
        {
            get {
                return _customer;
            }
            set {
                _customer = value;
                NotifyOfPropertyChange(() => Customer);
            }
        }
    }
}
