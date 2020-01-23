using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Navigation;
using BodyArchitect.Client.Common;

namespace BodyArchitect.Client.UI.Windows
{
    //we've introduced viewmodel (insead of setting MainWindow as DataContext) because of problem adding to Quick Access Toolbar buttons in the ribbon).
    //Basically application hanged then (stackoverflow)
    class MainWindowViewModel:ViewModelBase
    {
        private string _accountType;
        private bool _canGoBack;
        private bool _canGoForward;
        private ObservableCollection<ListItem<JournalEntry>> currentTabBackHistory = new ObservableCollection<ListItem<JournalEntry>>();
        private ObservableCollection<ListItem<JournalEntry>> currentTabForwardHistory = new ObservableCollection<ListItem<JournalEntry>>();

        public bool ShowPerformanceOutput
        {
            get { return false; }
        }

        public ObservableCollection<ListItem<JournalEntry>> CurrentTabBackHistory
        {
            get { return currentTabBackHistory; }
        }

        public ObservableCollection<ListItem<JournalEntry>> CurrentTabForwardHistory
        {
            get { return currentTabForwardHistory; }
        }

        public string AccountType
        {
            get { return _accountType; }
            set
            {
                _accountType = value;
                NotifyOfPropertyChange(() => AccountType);
            }
        }

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set
            {
                _canGoBack = value;
                NotifyOfPropertyChange(() => CanGoBack);
            }
        }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set
            {
                _canGoForward = value;
                NotifyOfPropertyChange(() => CanGoForward);
            }
        }
    }
}
