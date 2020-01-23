using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class MyProfileViewModel : ViewModelBase
    {
        private ProfileInformationDTO info;

        public MyProfileViewModel(ProfileInformationDTO info)
        {
            this.info = info;
        }

        public ProfileInformationDTO ProfileInfo
        {
            get { return info; }
        }

        public bool HasStatus
        {
            get
            {
                return !string.IsNullOrEmpty(info.User.Statistics.Status.Status);
            }
            set { }
        }

        public string Status
        {
            get { return string.Format("„{0}”", info.User.Statistics.Status.Status); }
            set
            {
                info.User.Statistics.Status.Status = value;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("HasStatus");
            }
        }

        public string UserName
        {
            get { return info.User.UserName.ToUpper(); }
        }

        public int Points
        {
            get { return info.Licence.BAPoints; }
        }

        public UserSearchDTO User
        {
            get { return info.User; }
        }

        public string MessagesLinkText
        {
            get { return string.Format(ApplicationStrings.MyProfileViewModel_MessagesLinkText, ApplicationState.Current.Cache.Messages.IsLoaded?ApplicationState.Current.Cache.Messages.Items.Count:0); }
        }

        public Visibility MessagesLinkVisible
        {
            get
            {
                return ApplicationState.Current.Cache.Messages.IsLoaded && ApplicationState.Current.Cache.Messages.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                //return Visibility.Visible; //return info.Messages.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
            }
        }

        public string LocalModifiedEntriesText
        {
            get
            {
                var count = ApplicationState.Current.MyDays.SelectMany(x => x.Value.GetLocalModifiedEntries()).Count();
                if(count>0)
                {
                    return string.Format(ApplicationStrings.MyProfileViewModel_NumberOfEntriesToSync, count);
                }
                return string.Empty;
            }
        }

        public string InvitationsLinkText
        {
            get { return string.Format(ApplicationStrings.MyProfileViewModel_InvitationsLinkText, info.Invitations.Count); }
        }

        public Visibility InvitationsLinkVisible
        {//visibility of invitations link depends on messages because we cannot allow user to go to messages page in case if messages are not loaded yet
            get { return ApplicationState.Current.Cache.Messages.IsLoaded && info.Invitations.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void LoadMessages()
        {
            if (!ApplicationState.Current.Cache.Messages.IsLoaded)
            {
                ApplicationState.Current.Cache.Messages.Loaded += new EventHandler(ObjectsReposidory_MessagesLoaded);
                ApplicationState.Current.Cache.Messages.Load();
            }
            else
            {
                NotifyPropertyChanged("MessagesLinkText");
                NotifyPropertyChanged("MessagesLinkVisible");
                NotifyPropertyChanged("InvitationsLinkText");
                NotifyPropertyChanged("InvitationsLinkVisible");
            }
            
        }

        public void RefreshMessages()
        {
            ApplicationState.Current.Cache.Messages.Loaded += new EventHandler(ObjectsReposidory_MessagesLoaded);
            ApplicationState.Current.Cache.Messages.Refresh();
            NotifyPropertyChanged("MessagesLinkText");
            NotifyPropertyChanged("MessagesLinkVisible");
            NotifyPropertyChanged("InvitationsLinkText");
            NotifyPropertyChanged("InvitationsLinkVisible");
        }

        void ObjectsReposidory_MessagesLoaded(object sender, EventArgs e)
        {
            ApplicationState.Current.Cache.Messages.Loaded -= new EventHandler(ObjectsReposidory_MessagesLoaded);
            NotifyPropertyChanged("MessagesLinkText");
            NotifyPropertyChanged("MessagesLinkVisible");
            NotifyPropertyChanged("InvitationsLinkText");
            NotifyPropertyChanged("InvitationsLinkVisible");
        }
    }
}
