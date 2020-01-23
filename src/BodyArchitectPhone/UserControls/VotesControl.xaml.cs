using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.UserControls
{
    public partial class VotesControl : UserControl,INotifyPropertyChanged
    {
        private IRatingable entry;
        private ObservableCollection<VoteViewModel> _comments;
        public event EventHandler CommentsLoaded;
        private bool loaded;
        private PagedResultOfCommentEntryDTO5oAtqRlh result;
        private bool refreshRequired;
        public event PropertyChangedEventHandler PropertyChanged;

        public VotesControl()
        {
            InitializeComponent();
            _comments = new ObservableCollection<VoteViewModel>();
            DataContext = this;
        }

        public ObservableCollection<VoteViewModel> Comments
        {
            get { return _comments; }
            private set
            {
                _comments = value;
                onPropertyChanged("Comments");
            }

        }


        public void Voting()
        {
            this.GetParent<PhoneApplicationPage>().Navigate("/Pages/VotingPage.xaml");
        }

        void onPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(name));
            }
        }

        public bool HasMore
        {
            get { return result != null && _comments.Count<result.AllItemsCount; }
        }

        private void onCommentsLoaded()
        {
            if (CommentsLoaded != null)
            {
                CommentsLoaded(this, EventArgs.Empty);
            }
        }


        public new bool Loaded
        {
            get { return loaded; }
        }

        public bool RefreshRequired
        {
            get { return refreshRequired; }
        }

        public void UpdateMyComment()
        {
            refreshRequired = true;
        }

        public void Load(IRatingable exercise)
        {
            entry = exercise;

            var m = new ServiceManager<GetCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetCommentsCompletedEventArgs> operationCompleted)
            {
                client1.GetCommentsCompleted -= operationCompleted;
                client1.GetCommentsCompleted += operationCompleted;
                client1.GetCommentsAsync(ApplicationState.Current.SessionData.Token, entry.GlobalId, new PartialRetrievingInfo());


            });

            m.OperationCompleted += (s, a) =>
            {
                refreshRequired = false;
                if (a.Error != null)
                {
                    onCommentsLoaded();
                    BAMessageBox.ShowError(ApplicationStrings.VotesControl_ErrRetrieveRatings);
                    return;
                }
                else
                {
                    Comments.Clear();
                    result = a.Result.Result;
                    foreach (var item in a.Result.Result.Items)
                    {
                        Comments.Add(new VoteViewModel(item));
                    }
                    updateNoRatingsLabel();
                    loaded = true;
                    //setCommentsStatus();

                }
                onCommentsLoaded();
            };

            if (!m.Run())
            {
                onCommentsLoaded();
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
        }

        private void updateNoRatingsLabel()
        {
            if (Comments.Count == 0)
            {
                lblNoRatings.Visibility = Visibility.Visible;
            }
            else
            {
                lblNoRatings.Visibility = Visibility.Collapsed;
            }
        }

        public void Save(System.Collections.Generic.IDictionary<string, object> state)
        {
            state["PageComments"] = _comments;
            state["PageResult"] = result;
            state["Loaded"] = loaded;
            state["Entry"] = entry;
        }

        public void Restore(System.Collections.Generic.IDictionary<string, object> state)
        {
            if (state.ContainsKey("PageComments"))
            {
                Comments = state["PageComments"] as ObservableCollection<VoteViewModel>;
                result = state["PageResult"] as PagedResultOfCommentEntryDTO5oAtqRlh;
                loaded =(bool) state["Loaded"];
                entry = (IRatingable)state["Entry"];
                updateNoRatingsLabel();
            }
            //setCommentsStatus();
        }

        public void LoadMore()
        {
            var m = new ServiceManager<GetCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetCommentsCompletedEventArgs> operationCompleted)
            {
                client1.GetCommentsAsync(ApplicationState.Current.SessionData.Token, entry.GlobalId, new PartialRetrievingInfo() { PageIndex = result.PageIndex + 1 });
                client1.GetCommentsCompleted -= operationCompleted;
                client1.GetCommentsCompleted += operationCompleted;

            });

            m.OperationCompleted += (s, a) =>
            {
                if (a.Error != null)
                {
                    onCommentsLoaded();
                    BAMessageBox.ShowError(ApplicationStrings.VotesControl_ErrRetrieveRatings);
                    return;
                }
                else
                {
                    result = a.Result.Result;
                    foreach (var item in a.Result.Result.Items)
                    {
                        Comments.Add(new VoteViewModel(item));
                    }
                }
                onCommentsLoaded();
            };

            if (!m.Run())
            {
                onCommentsLoaded();
                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
            }
        }

        
    }
}
