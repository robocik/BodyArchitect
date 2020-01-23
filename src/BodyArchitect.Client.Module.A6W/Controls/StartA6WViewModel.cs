using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Client.Module.A6W.Localization;

namespace BodyArchitect.Client.Module.A6W.Controls
{
    class StartA6WViewModel : ViewModelBase, IRemindable
    {
        private DateTime _startDate;
        private TimeSpan? _remindBefore;
        private string _name;
        private bool _canStart;
        private CustomerDTO _customer;

        public StartA6WViewModel(DateTime startDate)
        {
            Name = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.A6W:A6WEntryStrings:StartA6WViewModel_Name_A6WTraining");
            StartDate = startDate;
        }

        public bool CanStart
        {
            get { return _canStart; }
            set
            {
                _canStart = value;
                NotifyOfPropertyChange(() => CanStart);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                CanStart = _name.Length > 0;
                NotifyOfPropertyChange(() => Name);
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

        public TimeSpan? RemindBefore
        {
            get { return _remindBefore; }
            set
            {
                _remindBefore = value;
                NotifyOfPropertyChange(() => RemindBefore);
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

        public A6WTrainingDTO SimulateCycle()
        {
            return startCycle(MyTrainingOperationType.Simulate);
        }

        public void StartCycle(Dispatcher dispatcher)
        {
            var result = startCycle(MyTrainingOperationType.Start);
            //now refresh Training days cache
            var customerId = Customer != null ? Customer.GlobalId : (Guid?)null;
            var myCache = TrainingDaysReposidory.GetCache(customerId, null);
            myCache.Clear();
            MyTrainingsReposidory.GetCache(customerId, null).AddOrUpdate(result);
        }
        A6WTrainingDTO startCycle(MyTrainingOperationType operation)
        {
            var cycle = new A6WTrainingDTO();
            cycle.RemindBefore = RemindBefore;
            var param = new MyTrainingOperationParam();
            param.Operation = operation;
            param.MyTraining = cycle;
            cycle.Name = Name;
            if (Customer != null)
            {
                cycle.CustomerId = Customer.GlobalId;
            }
            cycle.StartDate = StartDate.ToUniversalTime();
            return (A6WTrainingDTO)ServiceManager.MyTrainingOperation(param);
        }
    }
}
