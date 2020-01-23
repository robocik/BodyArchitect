using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model.Instructor;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class ActivitiesViewModel : ViewModelBase, IWeakEventListener
    {
        private IBAWindow parentView;
     
        private bool canEdit;
        private bool canDelete;
        private bool canNew;
        private bool isInProgress;

        private ActivityDTO _selectedActivity;
        ObservableCollection<ActivityDTO>  activities = new ObservableCollection<ActivityDTO>();

        public ActivitiesViewModel(IBAWindow parentView)
        {
            this.parentView = parentView;
            updateButtons();
            CollectionChangedEventManager.AddListener(ActivitiesReposidory.Instance, this);
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
                NotifyOfPropertyChange(()=>CanEdit);
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

        public ICollection<ActivityDTO> Activities
        {
            get { return activities; }
        }

        void updateButtons()
        {
            CanNew = true;
            CanEdit = SelectedActivity != null;
            CanDelete = SelectedActivity != null;
        }
        
        public ActivityDTO SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                _selectedActivity = value;
                updateButtons();
                NotifyOfPropertyChange(() => SelectedActivity);
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            ParentWindow.SynchronizationContext.Send(state => FillActivities(), null);

            return true;
        }

        public void FillActivities()
        {
            IsInProgress = true;
            ParentWindow.RunAsynchronousOperation(delegate(OperationContext context)
            {
                ActivitiesReposidory.Instance.EnsureLoaded();
                ParentWindow.SynchronizationContext.Send(delegate
                                                             {
                                                                 activities.Clear();
                                                                 foreach (var activityDto in ActivitiesReposidory.Instance.Items.Values)
                                                                 {
                                                                     activities.Add(activityDto);
                                                                 }
                                                                 IsInProgress = false;
                                                             },null);

            });
        }

        public void NewActivity()
        {
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrActivityDetails();

            dlg.SetControl(ctrl);
            ActivityDTO activity = new ActivityDTO();
            activity.Color = Color.LightGreen.ToColorString();
            ctrl.Fill(activity);
            if (dlg.ShowDialog() == true)
            {
                ActivitiesReposidory.Instance.Add(ctrl.Activity);
                NotifyOfPropertyChange(() => Activities);
            }
        }

        public void DeleteSelectedActivity()
        {
            if (SelectedActivity != null && BAMessageBox.AskYesNo("ActivitiesViewModel_DeleteSelectedActivity".TranslateInstructor(), SelectedActivity.Name) == MessageBoxResult.Yes)
            {
                PleaseWait.Run(delegate(MethodParameters param)
                                   {
                                       try
                                       {
                                           ServiceManager.DeleteActivity(SelectedActivity);
                                           ActivitiesReposidory.Instance.Remove(SelectedActivity.GlobalId);
                                           parentView.SynchronizationContext.Send((x) => NotifyOfPropertyChange(() => Activities), null);
                                       }
                                       catch (Exception ex)
                                       {
                                           param.CloseProgressWindow();
                                           parentView.SynchronizationContext.Send((x) => ExceptionHandler.Default.Process(ex, "Exception_ActivitiesViewModel_DeleteSelectedActivity_CannotRemove".TranslateInstructor(), ErrorWindow.EMailReport), null);
                                           
                                       }
                                       
                                   });
            }
        }

        public void EditSelectedActivity()
        {
            var dlg = new EditDomainObjectWindow();

            var ctrl = new usrActivityDetails();

            dlg.SetControl(ctrl);
            ctrl.Fill(SelectedActivity.Clone());
            if (dlg.ShowDialog() == true)
            {
                replace(SelectedActivity, ctrl.Activity);
            }
        }

        private void replace(ActivityDTO selected, ActivityDTO saved)
        {
            ActivitiesReposidory.Instance.Replace(selected, saved);
            SelectedActivity = saved;
            NotifyOfPropertyChange(() => Activities);
        }
    }
}
