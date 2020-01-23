using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class RemindersViewModel : ViewModelBase,IWeakEventListener
    {
        private IBAWindow parentView;
        private bool canEdit;
        private bool canDelete;
        private bool canNew;
        private ReminderItemDTO _selectedReminder;
        ObservableCollection<ReminderItemDTO> reminders = new ObservableCollection<ReminderItemDTO>();
        private ReminderType _selectedType;
        private bool isInProgress;

        public RemindersViewModel(IBAWindow parentView)
        {
            this.parentView = parentView;
            updateButtons();
            CollectionChangedEventManager.AddListener(ReminderItemsReposidory.Instance, this);
        }

        public bool IsInProgress
        {
            get { return isInProgress; }
            set
            {
                isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public bool CanNew
        {
            get { return canNew; }
            set
            {
                canNew = value;
                NotifyOfPropertyChange(() => CanNew);
            }
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        public bool CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                NotifyOfPropertyChange(() => CanDelete);
            }
        }


        public IBAWindow ParentWindow
        {
            get { return parentView; }
        }

        public ICollection<ReminderItemDTO> Reminders
        {
            get { return reminders; }
        }

        public bool ShowInfo
        {
            get { return SelectedType != ReminderType.Custom; }
        }

        public ReminderType SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    FillReminders();
                    updateButtons();
                    NotifyOfPropertyChange(() => SelectedType);
                    NotifyOfPropertyChange(() => ShowInfo);
                }
            }
        }

        public IList<ListItem<ReminderType>> Types
        {
            get
            {
                return new ReadOnlyCollectionBuilder<ListItem<ReminderType>>()
                           {
                               new ListItem<ReminderType>("RemindersViewModel_IList_Yours".TranslateInstructor(), ReminderType.Custom),
                               new ListItem<ReminderType>("RemindersViewModel_IList_Birthdays".TranslateInstructor(), ReminderType.Birthday),
                               new ListItem<ReminderType>("RemindersViewModel_IList_Schedules".TranslateInstructor(), ReminderType.ScheduleEntry),
                               new ListItem<ReminderType>("RemindersViewModel_IList_Calendar".TranslateInstructor(), ReminderType.EntryObject)
                           };
            }
        }

        void updateButtons()
        {
            CanNew = SelectedType == ReminderType.Custom;
            CanEdit = SelectedReminder != null && SelectedReminder.Type==ReminderType.Custom;
            CanDelete = SelectedReminder != null && SelectedReminder.Type == ReminderType.Custom;
        }

        public ReminderItemDTO SelectedReminder
        {
            get { return _selectedReminder; }
            set
            {
                _selectedReminder = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedReminder);
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            ParentWindow.SynchronizationContext.Send(state => FillReminders(), null);

            return true;
        }

        public void FillReminders()
        {
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                ReminderItemsReposidory.Instance.EnsureLoaded();
                ParentWindow.SynchronizationContext.Send(delegate
                {
                    reminders.Clear();
                    foreach (var reminder in ReminderItemsReposidory.Instance.Items.Values.Where(x => x.Type == SelectedType))
                    {
                        reminders.Add(reminder);
                    }
                    IsInProgress = false;
                }, null);

            });
        }

        public void New()
        {
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrReminderDetails();

            dlg.SetControl(ctrl);
            var reminder = new ReminderItemDTO();
            ctrl.ReminderItem = reminder;
            if (dlg.ShowDialog() == true)
            {
                ReminderItemsReposidory.Instance.Add(ctrl.ReminderItem);
            }
        }

        public void Edit()
        {
            if(SelectedReminder.Type != ReminderType.Custom)
            {
                return;
            }
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrReminderDetails();

            dlg.SetControl(ctrl);
            ctrl.ReminderItem=SelectedReminder.Clone();
            if (dlg.ShowDialog() == true)
            {
                replace(SelectedReminder, ctrl.ReminderItem);
            }
        }

        private void replace(ReminderItemDTO selected, ReminderItemDTO saved)
        {
            ReminderItemsReposidory.Instance.Replace(selected, saved);

            SelectedReminder = saved;
            NotifyOfPropertyChange(() => Reminders);
        }

        public void Delete()
        {

            if (SelectedReminder != null && SelectedReminder.Type == ReminderType.Custom && BAMessageBox.AskYesNo("RemindersViewModel_Delete_DeleteReminder".TranslateInstructor(), SelectedReminder.Name) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate
                {
                    try
                    {
                        var param = new ReminderOperationParam();
                        param.ReminderItemId = SelectedReminder.GlobalId;
                        param.Operation = ReminderOperationType.Delete;
                        ServiceManager.ReminderOperation(param);
                        ReminderItemsReposidory.Instance.Remove(SelectedReminder.GlobalId);
                        parentView.SynchronizationContext.Send((x) => NotifyOfPropertyChange(() => Reminders), null);
                    }
                    catch (Exception ex)
                    {
                        parentView.SynchronizationContext.Send((x) => ExceptionHandler.Default.Process(ex, "RemindersViewModel_Delete_CannotRemoveReminder".TranslateInstructor(), ErrorWindow.EMailReport), null);

                    }

                });
            }
        }
    }
}
