using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Reminders.Controls
{
    class usrUserRemindersViewModel:ViewModelBase,IWeakEventListener
    {
        private Service.V2.Model.ProfileInformationDTO user;
        private bool isActive;
        private IBAWindow parentView;
        ObservableCollection<NotifyObject>  notifications = new ObservableCollection<NotifyObject>();

        public usrUserRemindersViewModel()
        {
            CollectionChangedEventManager.AddListener(NotificationsReposidory.Instance, this);
        }

        public void OnDeactivated()
        {
            CollectionChangedEventManager.RemoveListener(NotificationsReposidory.Instance, this);
        }
        public string Caption
        {
            get
            {
                return string.Format("usrUserRemindersViewModel_Caption_Notifications".TranslateInstructor(), notifications.Count);
            }
        }

        public IBAWindow ParentWindow
        {
            get { return parentView; }
        }

        public ICollection<NotifyObject> Notifications
        {
            get { return notifications; }

        }

        void fill()
        {
            notifications.Clear();
            foreach (var notify in NotificationsReposidory.Instance.Items.Values)
            {
                notifications.Add(notify);
            }
            NotifyOfPropertyChange(()=>Caption);
        }
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (ParentWindow != null)
            {
                ParentWindow.SynchronizationContext.Send(state => fill(), null);
            }
            return true;
        }

        public void Delete(ReminderItemDTO reminder)
        {
            ServicePool.Add(new ReminderOperationServiceCommand(reminder, ReminderOperationType.CloseAfterShow));
            NotificationsReposidory.Instance.Remove(reminder.GlobalId);
        }

        public void Fill(IBAWindow parentWindow, ProfileInformationDTO user, bool isActive)
        {
            this.parentView = parentWindow;
            this.user = user;
            this.isActive = isActive;
            fill();
        }
    }
}
