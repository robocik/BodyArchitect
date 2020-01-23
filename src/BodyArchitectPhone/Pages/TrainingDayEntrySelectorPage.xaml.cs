using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
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
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WP7ConversationView;

namespace BodyArchitect.WP7.Pages
{

    public partial class TrainingDayEntrySelectorPage
    {
        private TrainingDayViewModel viewModel;
        private IApplicationBarIconButton btnSend;
        private DateTime _selectedDate;

        public TrainingDayEntrySelectorPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            dayCtrl.SetControls(progressBar);
        }

        public DateTime SelectedDate
        {
            get
            {
                if (_selectedDate == DateTime.MinValue)
                {
                    return DateTime.Now.Date;
                }
                return _selectedDate;
            }
            set { _selectedDate = value; }
        }

        public TrainingDayDTO SelectedDayDTO { get; set; }

        private void buildApplicationBar()
        {

            ApplicationBar.IsVisible = false;
            if (pivot.SelectedIndex == 1 && SelectedDayDTO!=null && viewModel.Day.AllowComments)
            {
                ApplicationBar.Buttons.Clear();
                ApplicationBar.MenuItems.Clear();

                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.download.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnMore_Click);
                button1.Text = ApplicationStrings.AppBarButton_More;
                ApplicationBar.Buttons.Add(button1);

                btnSend = new ApplicationBarIconButton(new Uri("/icons/mainSent.png", UriKind.Relative));
                btnSend.Click += new EventHandler(btnAdd_Click);
                btnSend.Text = ApplicationStrings.AppBarButton_NewComment;
                btnSend.IsEnabled = false;
                if (!viewModel.IsNew)
                {
                    ApplicationBar.Buttons.Add(btnSend);
                }
                txtComment.Visibility = viewModel.IsNew ? Visibility.Collapsed : Visibility.Visible;
                ApplicationBar.IsVisible = true;
            }
            updateApplicationBarButtons();
        }

        private void updateApplicationBarButtons()
        {
            
            if (pivot.SelectedIndex == 1  )
            {
                if (ApplicationBar.Buttons.Count > 0)
                {
                    var btn1 = (IApplicationBarIconButton) ApplicationBar.Buttons[0];
                    btn1.IsEnabled = viewModel.AllItemsCount > viewModel.Comments.Count;
                    btnSend.IsEnabled = false;
                    if (ApplicationBar.Buttons.Count == 1 && !viewModel.IsNew)
                    {
                        ApplicationBar.Buttons.Add(btnSend);
                    }
                }
                txtComment.Visibility = SelectedDayDTO==null || !viewModel.Day.AllowComments || viewModel.IsNew ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            dayCtrl.AnimateLeaveAll();
            base.OnBackKeyPress(e);
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //fillToday();
            buildApplicationBar();
            StateHelper stateHelper = new StateHelper(this.State);
            SelectedDate = stateHelper.GetValue("SelectedDate", SelectedDate);
            dayCtrl.Fill(SelectedDate);
            pivot.Title = SelectedDate.ToLongDateString();

            if (ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(SelectedDate))
            {
                var info = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[SelectedDate];
                viewModel = new TrainingDayViewModel(info.TrainingDay);
                SelectedDayDTO = info.TrainingDay;
                viewModel.BlogCommentsLoaded += (s, a) =>
                {
                    progressBar.ShowProgress(false);
                    updateApplicationBarButtons();
                };
            }
            else
            {
                viewModel = new TrainingDayViewModel(null);
            }
            
            viewModel.Restore(State);
            DataContext = viewModel;
        }

    

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            //if(e.Content is EntryObjectPageBase)
            //{
            //    dayCtrl.PrepareTrainingDay();
            //}
            base.OnNavigatedFrom(e);
            viewModel.Save(State);
            //State["SelectedDate"] = SelectedDate;
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            if (pivot.SelectedIndex == 1)
            {//comments selected
                progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressRetrieveComments);
                viewModel.BlogCommentsLoaded += (s, a) =>
                {
                    updateApplicationBarButtons();
                    progressBar.ShowProgress(false);
                };
                if (viewModel.IsCommentsLoaded)
                {
                    viewModel.LoadMore();
                }
                else
                {
                    viewModel.LoadComments();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtComment.Text == string.Empty)
            {
                return;
            }
            this.Focus();
            progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressSavingComment);
            var m = new ServiceManager<TrainingDayCommentOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<TrainingDayCommentOperationCompletedEventArgs> operationCompleted)
            {
                TrainingDayCommentOperationParam operation = new TrainingDayCommentOperationParam();
                operation.OperationType = TrainingDayOperationType.Add;
                var dto = new TrainingDayCommentDTO();
                dto.Comment = txtComment.Text;
                dto.Profile = ApplicationState.Current.SessionData.Profile;
                operation.Comment = dto;
                operation.TrainingDayId = viewModel.Day.GlobalId;
                client1.TrainingDayCommentOperationAsync(ApplicationState.Current.SessionData.Token, operation);
                client1.TrainingDayCommentOperationCompleted -= operationCompleted;
                client1.TrainingDayCommentOperationCompleted += operationCompleted;

            });

            m.OperationCompleted += (s1, a1) =>
            {
                FaultException<BAServiceException> serviceEx = a1.Error as FaultException<BAServiceException>;
                if (serviceEx != null && serviceEx.Detail.ErrorCode == ErrorCode.InvalidOperationException)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrCommentsAreDisabled);
                    return;
                }else if (a1.Error != null)
                {
                    progressBar.ShowProgress(false);
                    BAMessageBox.ShowError(ApplicationStrings.BlogPage_ErrSavingComment);
                    return;
                }
                else
                {
                    txtComment.Text = string.Empty;
                    progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressRetrieveComments);
                    viewModel.LoadComments();
                }

            };

            if (!m.Run())
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
                return;
            }
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
            if(SelectedDayDTO==null)
            {
                return;
            }
            if (ApplicationState.Current.IsOffline)
            {
                viewModel.CommentsStatus = ApplicationStrings.OfflineModeFeatureNotAvailable;
                return;
            }

            if (pivot.SelectedIndex == 1)
            {//comments selected
                if (!viewModel.IsCommentsLoaded)
                {
                    progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressRetrieveComments);

                    viewModel.LoadComments();
                }
            }
        }

        private void TextInput_GotFocus(object sender, RoutedEventArgs e)
        {
            //txtComment.Height = 200;
            btnSend.IsEnabled = true;
        }

        private void TextInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //txtComment.Height = 80;
            btnSend.IsEnabled = false;
        }

    }

    public class TrainingDayViewModel:ViewModelBase
    {
            private TrainingDayDTO day;
            private ObservableCollection<Message> _comments;
            public event EventHandler BlogCommentsLoaded;
            private string commentsStatus;
            private Visibility commentsStatusVisibility;
            private PagedResultOfTrainingDayCommentDTO5oAtqRlh result;
            
            public TrainingDayViewModel(TrainingDayDTO day)
            {
                this.day = day;
                _comments = new ObservableCollection<Message>();
            }

            public bool EditMode
            {
                get { return day==null || day.IsMine; }
            }

            public bool IsNew
            {
                get { return day==null || day.GlobalId == Guid.Empty; }
            }


            public void Save(System.Collections.Generic.IDictionary<string, object> state)
            {
                state["BlogPageComments"] = _comments;
                state["BlogPageResult"] = result;
            }

            public void Restore(System.Collections.Generic.IDictionary<string, object> state)
            {
                if(state.ContainsKey("BlogPageComments"))
                {
                    _comments = state["BlogPageComments"] as ObservableCollection<Message>;
                    result = state["BlogPageResult"] as PagedResultOfTrainingDayCommentDTO5oAtqRlh;
                }
                setCommentsStatus();
            }

            public string TrainingDate
            {
                get { return day!=null ?day.TrainingDate.ToLongDateString():null; }
            }

            public int AllItemsCount
            {
                get { return result != null ? result.AllItemsCount : 0; }
            }
        
            public ObservableCollection<Message> Comments
            {
                get { return _comments; }
            }

            public bool IsCommentsLoaded
            {
                get { return result != null; }
            }

            public string CommentsStatus
            {
                get { return commentsStatus; }
                set
                {
                    if(commentsStatus!=value)
                    {
                        commentsStatus = value;
                        NotifyPropertyChanged("CommentsStatus");
                    }
                }
            }

            public Visibility CommentsStatusVisibility
            {
                get { return commentsStatusVisibility; }
                set
                {
                    if (commentsStatusVisibility != value)
                    {
                        commentsStatusVisibility = value;
                        NotifyPropertyChanged("CommentsStatusVisibility");
                    }
                }
            }

            public TrainingDayDTO Day
            {
                get {return day; }
            }

            public void LoadComments()
            {
                result = null;

                if (day==null || day.GlobalId == Guid.Empty)
                {
                    _comments.Clear();
                    setCommentsStatus();
                    onBlogCommentsLoaded();
                    return;
                }
                CommentsStatus = ApplicationStrings.BlogViewModel_LoadComments_Loading;
                var m = new ServiceManager<GetTrainingDayCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDayCommentsCompletedEventArgs> operationCompleted)
                {
                    client1.GetTrainingDayCommentsAsync(ApplicationState.Current.SessionData.Token, day,new PartialRetrievingInfo());
                    client1.GetTrainingDayCommentsCompleted -= operationCompleted;
                    client1.GetTrainingDayCommentsCompleted += operationCompleted;

                });

                m.OperationCompleted += (s, a) =>
                                            {
                    if (a.Error != null)
                    {
                        onBlogCommentsLoaded();
                        BAMessageBox.ShowError(ApplicationStrings.BlogViewModel_LoadComments_ErrorMsg);
                        return;
                    }
                    else
                    {
                        _comments.Clear();
                        fillComments(a);
                        setCommentsStatus();

                    }
                       onBlogCommentsLoaded();
                    };

                if (!m.Run())
                {
                    onBlogCommentsLoaded();
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

            private void fillComments(ServiceManager<GetTrainingDayCommentsCompletedEventArgs>.ServiceManagerOperationResult a)
            {
                result = a.Result.Result;
                foreach (var item in result.Items)
                {
                    var msg = new Message();
                    msg.User = item.Profile;
                    msg.Picture = item.Profile.Picture;
                    msg.UserName = item.Profile.UserName;
                    msg.Side = item.Profile.IsMe ? MessageSide.Me : MessageSide.You;
                    msg.Text = item.Comment;
                    msg.Timestamp = item.DateTime;
                    msg.TrainingDayComment = item;
                    _comments.Add(msg);
                }
            }

            private void setCommentsStatus()
            {
                if (Day==null || !Day.AllowComments)
                {
                    CommentsStatusVisibility = Visibility.Visible;
                    CommentsStatus = ApplicationStrings.BlogViewModel_CommentsDisabled;
                }else if(_comments.Count==0)
                {
                    CommentsStatusVisibility = Visibility.Visible;
                    CommentsStatus = ApplicationStrings.BlogViewModel_LoadComments_Empty;
                }
                else
                {
                    CommentsStatusVisibility = Visibility.Collapsed;
                }
            }

            private void onBlogCommentsLoaded()
            {
                if(BlogCommentsLoaded!=null)
                {
                    BlogCommentsLoaded(this, EventArgs.Empty);
                }
            }

            public void LoadMore()
            {
                var m = new ServiceManager<GetTrainingDayCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetTrainingDayCommentsCompletedEventArgs> operationCompleted)
                {
                    client1.GetTrainingDayCommentsAsync(ApplicationState.Current.SessionData.Token, day, new PartialRetrievingInfo(){PageIndex = result.PageIndex+1});
                    client1.GetTrainingDayCommentsCompleted -= operationCompleted;
                    client1.GetTrainingDayCommentsCompleted += operationCompleted;

                });

                m.OperationCompleted += (s, a) =>
                {
                    if (a.Error != null)
                    {
                        onBlogCommentsLoaded();
                        BAMessageBox.ShowError(ApplicationStrings.BlogViewModel_LoadComments_ErrorMsg);
                        return;
                    }
                    else
                    {
                        result = a.Result.Result;
                        fillComments(a);
                    }
                    onBlogCommentsLoaded();
                };

                if (!m.Run())
                {
                    onBlogCommentsLoaded();
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


    }
}