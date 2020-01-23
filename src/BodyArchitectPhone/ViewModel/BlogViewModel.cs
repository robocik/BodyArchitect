using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Text;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class BlogViewModel : ViewModelBase
    {
        private BlogEntryDTO entry;

        public BlogViewModel(BlogEntryDTO blogEntryDTO)
        {
            this.entry = blogEntryDTO;
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine; }
        }

        public bool IsNew
        {
            get { return entry.GlobalId == Guid.Empty; }
        }
        public string TrainingDate
        {
            get { return entry.TrainingDay.TrainingDate.ToLongDateString(); }
        }


        private string encodeUnicode(string strText)
        {
            if (string.IsNullOrEmpty(strText))
            {
                return string.Empty;
            }
            string chararray = " `1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";
            StringBuilder txtUnicode = new StringBuilder();

            foreach (char value in strText)
            {
                if (chararray.IndexOf(value) >= 0)
                {
                    txtUnicode.Append(value);
                }
                else
                {
                    int decValue = int.Parse(string.Format("{0:x4}", (int)value), System.Globalization.NumberStyles.HexNumber);
                    txtUnicode.AppendFormat("&#{0};", decValue);
                }
            }
            return txtUnicode.ToString();
        }


        public string Content
        {
            get { return encodeUnicode(entry.Comment); }
        }

 

        public BlogEntryDTO Entry
        {
            get { return entry; }
        }

    }

    //public class BlogViewModel:ViewModelBase
    //{
    //    private BlogEntryDTO entry;
    //    private ObservableCollection<Message> _comments;
    //    public event EventHandler BlogCommentsLoaded;
    //    private string commentsStatus;
    //    private Visibility commentsStatusVisibility;
    //    private PagedResultOfBlogCommentDTO5oAtqRlh result;

    //    public BlogViewModel(BlogEntryDTO blogEntryDTO)
    //    {
    //        this.entry = blogEntryDTO;
    //        _comments = new ObservableCollection<Message>();
    //    }

    //    public bool EditMode
    //    {
    //        get { return Entry.TrainingDay.IsMine; }
    //    }

    //    public bool IsNew
    //    {
    //        get { return entry.GlobalId == Guid.Empty; }
    //    }

    //    public void Save(System.Collections.Generic.IDictionary<string, object> state)
    //    {
    //        state["BlogPageComments"] = _comments;
    //        state["BlogPageResult"] = result;
    //    }

    //    public void Restore(System.Collections.Generic.IDictionary<string, object> state)
    //    {
    //        if(state.ContainsKey("BlogPageComments"))
    //        {
    //            _comments = state["BlogPageComments"] as ObservableCollection<Message>;
    //            result = state["BlogPageResult"] as PagedResultOfBlogCommentDTO5oAtqRlh;
    //        }
    //        setCommentsStatus();
    //    }

    //    public string TrainingDate
    //    {
    //        get { return entry.TrainingDay.TrainingDate.ToLongDateString(); }
    //    }

    //    public int AllItemsCount
    //    {
    //        get { return result != null ? result.AllItemsCount : 0; }
    //    }

    //    private string encodeUnicode(string strText)
    //    {
    //        if(string.IsNullOrEmpty(strText))
    //        {
    //            return string.Empty;
    //        }
    //        string chararray = " `1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";
    //        StringBuilder txtUnicode = new StringBuilder();

    //        foreach (char value in strText)
    //        {
    //            if (chararray.IndexOf(value) >= 0)
    //            {
    //                txtUnicode.Append(value);
    //            }
    //            else
    //            {
    //                int decValue = int.Parse(string.Format("{0:x4}", (int)value), System.Globalization.NumberStyles.HexNumber);
    //                txtUnicode.AppendFormat("&#{0};", decValue);
    //            }
    //        }
    //        return txtUnicode.ToString();
    //    }


    //    public string Content
    //    {
    //        get { return encodeUnicode(entry.Comment); }
    //    }

    //    public ObservableCollection<Message> Comments
    //    {
    //        get { return _comments; }
    //    }

    //    public bool IsCommentsLoaded
    //    {
    //        get { return result != null; }
    //    }

    //    public string CommentsStatus
    //    {
    //        get { return commentsStatus; }
    //        set
    //        {
    //            if(commentsStatus!=value)
    //            {
    //                commentsStatus = value;
    //                NotifyPropertyChanged("CommentsStatus");
    //            }
    //        }
    //    }

    //    public Visibility CommentsStatusVisibility
    //    {
    //        get { return commentsStatusVisibility; }
    //        set
    //        {
    //            if (commentsStatusVisibility != value)
    //            {
    //                commentsStatusVisibility = value;
    //                NotifyPropertyChanged("CommentsStatusVisibility");
    //            }
    //        }
    //    }

    //    public BlogEntryDTO Entry
    //    {
    //        get {return entry; }
    //    }

    //    public void LoadComments()
    //    {
    //        result = null;

    //        if (entry.GlobalId == Guid.Empty)
    //        {
    //            _comments.Clear();
    //            setCommentsStatus();
    //            onBlogCommentsLoaded();
    //            return;
    //        }
    //        CommentsStatus = ApplicationStrings.BlogViewModel_LoadComments_Loading;
    //        var m = new ServiceManager<GetBlogCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetBlogCommentsCompletedEventArgs> operationCompleted)
    //        {
    //            client1.GetBlogCommentsAsync(ApplicationState.Current.SessionData.Token, entry,new PartialRetrievingInfo());
    //            client1.GetBlogCommentsCompleted -= operationCompleted;
    //            client1.GetBlogCommentsCompleted += operationCompleted;

    //        });

    //        m.OperationCompleted += (s, a) =>
    //                                    {
    //            if (a.Error != null)
    //            {
    //                onBlogCommentsLoaded();
    //                BAMessageBox.ShowError(ApplicationStrings.BlogViewModel_LoadComments_ErrorMsg);
    //                return;
    //            }
    //            else
    //            {
    //                _comments.Clear();
    //                fillComments(a);
    //                setCommentsStatus();
                    
    //            }
    //               onBlogCommentsLoaded();
    //            };

    //        if (!m.Run())
    //        {
    //            onBlogCommentsLoaded();
    //            if (ApplicationState.Current.IsOffline)
    //            {
    //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
    //            }
    //            else
    //            {
    //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
    //            }
    //        }
    //    }

    //    private void fillComments(ServiceManager<GetBlogCommentsCompletedEventArgs>.ServiceManagerOperationResult a)
    //    {
    //        result = a.Result.Result;
    //        foreach (var item in result.Items)
    //        {
    //            var msg = new Message();
    //            msg.Picture = item.Profile.Picture;
    //            msg.UserName = item.Profile.UserName;
    //            msg.Side = item.Profile.IsMe ? MessageSide.Me : MessageSide.You;
    //            msg.Text = item.Comment;
    //            msg.Timestamp = item.DateTime;
    //            msg.BlogComment = item;
    //            _comments.Add(msg);
    //        }
    //    }

    //    private void setCommentsStatus()
    //    {
    //        if(_comments.Count==0)
    //        {
    //            CommentsStatusVisibility = Visibility.Visible;
    //            CommentsStatus = ApplicationStrings.BlogViewModel_LoadComments_Empty;
    //        }
    //        else
    //        {
    //            CommentsStatusVisibility = Visibility.Collapsed;
    //        }
    //    }

    //    private void onBlogCommentsLoaded()
    //    {
    //        if(BlogCommentsLoaded!=null)
    //        {
    //            BlogCommentsLoaded(this, EventArgs.Empty);
    //        }
    //    }

    //    public void LoadMore()
    //    {
    //        var m = new ServiceManager<GetBlogCommentsCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<GetBlogCommentsCompletedEventArgs> operationCompleted)
    //        {
    //            client1.GetBlogCommentsAsync(ApplicationState.Current.SessionData.Token, entry, new PartialRetrievingInfo(){PageIndex = result.PageIndex+1});
    //            client1.GetBlogCommentsCompleted -= operationCompleted;
    //            client1.GetBlogCommentsCompleted += operationCompleted;

    //        });

    //        m.OperationCompleted += (s, a) =>
    //        {
    //            if (a.Error != null)
    //            {
    //                onBlogCommentsLoaded();
    //                BAMessageBox.ShowError(ApplicationStrings.BlogViewModel_LoadComments_ErrorMsg);
    //                return;
    //            }
    //            else
    //            {
    //                result = a.Result.Result;
    //                fillComments(a);
    //            }
    //            onBlogCommentsLoaded();
    //        };

    //        if (!m.Run())
    //        {
    //            onBlogCommentsLoaded();
    //            if (ApplicationState.Current.IsOffline)
    //            {
    //                BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
    //            }
    //            else
    //            {
    //                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
    //            }
    //        }
    //    }
    //}
}
