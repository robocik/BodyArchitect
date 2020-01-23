using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Client.Resources.Localization;

namespace BodyArchitect.Client.UI.Views
{
    class MyTrainingsViewModel : ViewModelBase, IWeakEventListener
    {
        private IBAWindow parentView;
        private MyTrainingsReposidory myTrainingsCache;
        private MyTrainingViewModel _selectedMyTraining;
        ObservableCollection<MyTrainingViewModel> myTrainings = new ObservableCollection<MyTrainingViewModel>();
        ObservableCollection<TrainingDayDTO> trainingDays = new ObservableCollection<TrainingDayDTO>();
        private DateTime activeMonthDateTime;
        private bool canBreak;
        private bool isInProgress;

        public MyTrainingsViewModel(IBAWindow parentView, UserDTO user, CustomerDTO customer)
        {
            this.parentView = parentView;
            User = user;
            ActiveMonthDateTime = DateTime.Now;
            Customer = customer;
            Guid? customerId = Customer != null ? Customer.GlobalId : (Guid?) null;
            Guid? userId = User != null ? User.GlobalId : (Guid?)null;
            myTrainingsCache = MyTrainingsReposidory.GetCache(customerId,userId);
            CollectionChangedEventManager.AddListener(myTrainingsCache, this);
            updateButtons();
        }

        #region Properties

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public UserDTO User
        {
            get; private set;
        }

        public bool CanBreak
        {
            get { return canBreak; }
            set
            {
                canBreak = value;
                NotifyOfPropertyChange(() => CanBreak);
            }
        }

        public DateTime ActiveMonthDateTime
        {
            get { return activeMonthDateTime; }
            set
            {
                activeMonthDateTime = value;
                NotifyOfPropertyChange(()=>ActiveMonthDateTime);
            }
        }
        public CustomerDTO Customer { get; private set; }

        public ICollection<MyTrainingViewModel> MyTrainings
        {
            get { return myTrainings; }
        }

        public MyTrainingViewModel SelectedMyTraining
        {
            get { return _selectedMyTraining; }
            set
            {
                _selectedMyTraining = value;
                updateButtons();
                trainingDays.Clear();
                if(value!=null)
                {
                    foreach (var trainingDayDto in value.MyTraining.EntryObjects.ToTrainingDays())
                    {
                        trainingDays.Add(trainingDayDto);
                    }
                    //set calendar view on the first training day in this cycle
                    ActiveMonthDateTime = value.MyTraining.StartDate;
                }
                NotifyOfPropertyChange(() => SelectedMyTraining);
                NotifyOfPropertyChange(() => TrainingDaysOfSelectedTraining);
            }
        }
        #endregion

        void updateButtons()
        {
            CanBreak = SelectedMyTraining != null && SelectedMyTraining.MyTraining.TrainingEnd == TrainingEnd.NotEnded;
        }


        public IEnumerable<TrainingDayDTO>  TrainingDaysOfSelectedTraining
        {
            get { return trainingDays; }
        }
        public void Fill()
        {
            IsInProgress = true;
            parentView.RunAsynchronousOperation(delegate(OperationContext context)
            {
                myTrainingsCache.EnsureLoaded();
                parentView.SynchronizationContext.Send(delegate
                {
                    myTrainings.Clear();
                    foreach (var myTraining in myTrainingsCache.Items.Values)
                    {
                        myTrainings.Add(new MyTrainingViewModel(myTraining));
                    }
                    IsInProgress = false;
                }, null);

            });
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            parentView.SynchronizationContext.Send(state => Fill(), null);
            return true;
        }

        public void Refresh()
        {
            myTrainingsCache.ClearCache();
            Fill();
        }

        public void BreakTraining()
        {
            if (BAMessageBox.AskYesNo(EnumLocalizer.Default.GetStringsString("Question_MyTrainingsViewModel_BreakTraining_AreYouSure")) == MessageBoxResult.No)
            {
                return;
            }
            PleaseWait.Run(delegate(MethodParameters param1)
            {
                try
                {
                    var param = new MyTrainingOperationParam();
                    param.Operation = MyTrainingOperationType.Stop;
                    param.MyTraining = SelectedMyTraining.MyTraining;
                    param.MyTraining.EntryObjects=new List<EntryObjectDTO>();//we clear this collection to have a smaller request (we need defacto only GlobalId here)
                    var myTraining=ServiceManager.MyTrainingOperation(param);
                    myTrainingsCache.Update(myTraining);
                }
                catch (Exception ex)
                {
                    param1.CloseProgressWindow();
                    parentView.SynchronizationContext.Send((x) => ExceptionHandler.Default.Process(ex, EnumLocalizer.Default.GetStringsString("Exception_MyTrainingsViewModel_BreakTraining"), ErrorWindow.EMailReport), null);

                }
            });
        }
    }

    class MyTrainingViewModel:ViewModelBase
    {
        private MyTrainingDTO myTraining;

        public MyTrainingViewModel(MyTrainingDTO myTraining)
        {
            this.myTraining = myTraining;
        }

        public MyTrainingDTO MyTraining
        {
            get { return myTraining; }
        }

        public string State
        {
            get { return EnumLocalizer.Default.Translate(MyTraining.TrainingEnd); }
        }

        public string Type
        {//todo:translateWhat?
            get { return MyTraining.GetType().Name; }
        }
    }
}
