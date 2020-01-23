using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
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
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BodyArchitect.WP7.Controls;
using WP7ConversationView;

namespace BodyArchitect.WP7.Pages
{
    public partial class BlogPage
    {
        private BlogViewModel viewModel;

        public BlogPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            SetControls(progressBar, null);
        }

        new public BlogEntryDTO Entry
        {
            get { return (BlogEntryDTO) base.Entry; }
        }
        //protected override void PrepareCopiedEntry(EntryObjectDTO entry)
        //{
        //    //BlogEntryDTO blog = entry as BlogEntryDTO;
        //    //if (blog != null)
        //    //{
        //    //    blog.BlogCommentsCount = 0;
        //    //    blog.LastCommentDate = null;
        //    //}
        //}
        protected override void buildApplicationBar()
        {
            
            base.buildApplicationBar();
            
            updateApplicationBarButtons();
        }

        protected override Type EntryType
        {
            get { return typeof(BlogEntryDTO); }
        }

        private void updateApplicationBarButtons()
        {
            var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn1.IsEnabled = !ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine || (viewModel.EditMode && viewModel.IsNew);
            if (!ApplicationState.Current.IsOffline)
            {
                var btn2 = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
                btn2.IsEnabled = viewModel.EditMode && !viewModel.IsNew;
            }
        }

        //protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    //viewModel.Save(State);
        //    base.OnNavigatedFrom(e);
        //}

        protected override void show( bool reload)
        {
            //if (ApplicationState.Current.TrainingDay.TrainingDay.Blog == null)
            //{
            //    var entry = new BlogEntryDTO();
            //    ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entry);
            //    entry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
            //}

            viewModel = new BlogViewModel(Entry);

            DataContext = viewModel;

            header.Text = viewModel.TrainingDate;

            buildApplicationBar();
            if(reload)
            {
                fillContent();
            }
        }


        async protected override Task SavingCompleted()
        {
            updateApplicationBarButtons();
        }

        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.TrainingDay == null)
            {
                return;
            }
            if (BAMessageBox.Ask(ApplicationStrings.BlogPage_QRemoveBlog) == MessageBoxResult.OK)
            {
                deleteEntry(Entry);
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fillContent();
        }

        private void fillContent()
        {
            if (!string.IsNullOrEmpty(viewModel.Content))
            {
                webBrowser.NavigateToString(viewModel.Content);
                webBrowser.Visibility = Visibility.Visible;
            }
            else
            {
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}

/*using System;
using System.Collections.Generic;
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
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BodyArchitect.WP7.Controls;
using WP7ConversationView;

namespace BodyArchitect.WP7.Pages
{
    public partial class BlogPage : EntryObjectPageBase
    {
        private BlogViewModel viewModel;
        private IApplicationBarIconButton btnSend;

        public BlogPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            SetControls(progressBar, pivot);

            scrollViewerScrollToEndAnim = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new SineEase()
            };
            Storyboard.SetTarget(scrollViewerScrollToEndAnim, this);
            Storyboard.SetTargetProperty(scrollViewerScrollToEndAnim, new PropertyPath(VerticalOffsetProperty));

            scrollViewerStoryboard = new Storyboard();
            scrollViewerStoryboard.Children.Add(scrollViewerScrollToEndAnim);
            this.Resources.Add("foo", scrollViewerStoryboard);
        }


        protected override void PrepareCopiedEntry(EntryObjectDTO entry)
        {
            //BlogEntryDTO blog = entry as BlogEntryDTO;
            //if (blog != null)
            //{
            //    blog.BlogCommentsCount = 0;
            //    blog.LastCommentDate = null;
            //}
        }
        protected override void buildApplicationBar()
        {
            

            if(pivot.SelectedIndex==0)
            {
                base.buildApplicationBar();
            }
            else
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
                NewCommentBox.Visibility = viewModel.IsNew ? Visibility.Collapsed : Visibility.Visible;
                
            }
            updateApplicationBarButtons();
        }

        protected override Type EntryType
        {
            get { return typeof(BlogEntryDTO); }
        }

        private void updateApplicationBarButtons()
        {
           
            if (pivot.SelectedIndex == 0)
            {
                var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn1.IsEnabled = !ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine || (viewModel.EditMode && viewModel.IsNew);
                if (!ApplicationState.Current.IsOffline)
                {
                    var btn2 = (ApplicationBarMenuItem) ApplicationBar.MenuItems[0];
                    btn2.IsEnabled = viewModel.EditMode && !viewModel.IsNew;
                }
            }
            else
            {
                //var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[0];
                //btn1.IsEnabled = viewModel.AllItemsCount > viewModel.Comments.Count;
                btnSend.IsEnabled = false;
                if(ApplicationBar.Buttons.Count==1 && !viewModel.IsNew)
                {
                    ApplicationBar.Buttons.Add(btnSend);
                }
                NewCommentBox.Visibility = viewModel.IsNew ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            //viewModel.Save(State);
            base.OnNavigatedFrom(e);
        }

        protected override void show(TrainingDayDTO day, bool reload)
        {
            if (ApplicationState.Current.TrainingDay.Blog == null)
            {
                var entry = new BlogEntryDTO();
                //entry.AllowComments = true;
                ApplicationState.Current.TrainingDay.Objects.Add(entry);
                entry.TrainingDay = ApplicationState.Current.TrainingDay;
            }

            viewModel = new BlogViewModel(day.Blog);
            //viewModel.BlogCommentsLoaded += (s, a) =>
            //{
            //    progressBar.ShowProgress(false);
            //    updateApplicationBarButtons();
            //};
            //viewModel.Restore(State);
            DataContext = viewModel;

            header.Text = viewModel.TrainingDate;

            buildApplicationBar();
            if(reload)
            {
                fillComment();
            }
        }


        protected override void SavingCompleted()
        {
            updateApplicationBarButtons();
        }

        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.TrainingDay == null)
            {
                return;
            }
            if (BAMessageBox.Ask(ApplicationStrings.BlogPage_QRemoveBlog) == MessageBoxResult.OK)
            {
                deleteEntry(ApplicationState.Current.TrainingDay.Blog);
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fillComment();
        }

        private void fillComment()
        {
            if (!string.IsNullOrEmpty(viewModel.Content))
            {
                webBrowser.NavigateToString(viewModel.Content);
                webBrowser.Visibility = Visibility.Visible;
            }
            else
            {
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
            //if (ApplicationState.Current.IsOffline)
            //{
            //    viewModel.CommentsStatus = ApplicationStrings.OfflineModeFeatureNotAvailable;
            //    return;
            //}
            
            //if (pivot.SelectedIndex == 1)
            //{//comments selected
            //    if (!viewModel.IsCommentsLoaded)
            //    {
            //        progressBar.ShowProgress(true,ApplicationStrings.BlogPage_ProgressRetrieveComments);
                    
            //        viewModel.LoadComments();
            //    }
            //}
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            //if (pivot.SelectedIndex == 1)
            //{//comments selected
            //    progressBar.ShowProgress(true,ApplicationStrings.BlogPage_ProgressRetrieveComments);
            //    viewModel.BlogCommentsLoaded += (s, a) =>
            //    {
            //        updateApplicationBarButtons();
            //        progressBar.ShowProgress(false);
            //    };
            //    if (viewModel.IsCommentsLoaded)
            //    {
            //        viewModel.LoadMore();
            //    }
            //    else
            //    {
            //        viewModel.LoadComments();
            //    }
            //}
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(txtComment.Text==string.Empty)
            {
                return;
            }
            this.Focus();
            //progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressSavingComment);
            //var m = new ServiceManager<BlogCommentOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<BlogCommentOperationCompletedEventArgs> operationCompleted)
            //{
            //    BlogCommentOperation operation = new BlogCommentOperation();
            //    operation.OperationType = BlogCommentOperationType.Add;
            //    BlogCommentDTO dto = new BlogCommentDTO();
            //    dto.CommentType = ContentType.Text;
            //    dto.Comment = txtComment.Text;
            //    dto.Profile = ApplicationState.Current.SessionData.Profile;
            //    operation.BlogComment = dto;
            //    operation.BlogEntryId = viewModel.Entry.GlobalId;
            //    client1.BlogCommentOperationAsync(ApplicationState.Current.SessionData.Token, operation);
            //    client1.BlogCommentOperationCompleted -= operationCompleted;
            //    client1.BlogCommentOperationCompleted += operationCompleted;

            //});

            //m.OperationCompleted += (s1, a1) =>
            //{
            //    if (a1.Error != null)
            //    {
            //        progressBar.ShowProgress(false);
            //        BAMessageBox.ShowError(ApplicationStrings.BlogPage_ErrSavingComment);
            //        return;
            //    }
            //    else
            //    {
            //        txtComment.Text = string.Empty;
            //        progressBar.ShowProgress(true, ApplicationStrings.BlogPage_ProgressRetrieveComments);
            //        viewModel.LoadComments();
            //    }

            //};

            //if (!m.Run())
            //{
            //    progressBar.ShowProgress(false);
            //    if (ApplicationState.Current.IsOffline)
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
            //    }
            //    else
            //    {
            //        BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
            //    }
            //    return;
            //}
        }

        #region Conversation view

        private Storyboard scrollViewerStoryboard;
        private DoubleAnimation scrollViewerScrollToEndAnim;

        private void TextInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ConversationContentContainer.ActualHeight < ConversationScrollViewer.ActualHeight)
            {
                PaddingRectangle.Show(() => ScrollConvesationToEnd());
            }
            else
            {
                ScrollConvesationToEnd();
            }

            btnSend.IsEnabled = true;
        }

        private void ScrollConvesationToEnd()
        {
            scrollViewerScrollToEndAnim.From = ConversationScrollViewer.VerticalOffset;
            scrollViewerScrollToEndAnim.To = ConversationContentContainer.ActualHeight;
            scrollViewerStoryboard.Begin();
        }

        private void TextInput_LostFocus(object sender, RoutedEventArgs e)
        {
            PaddingRectangle.Hide();
            btnSend.IsEnabled = false;
            ScrollConvesationToEnd();
        }

        #region VerticalOffset DP

        /// <summary>
        /// VerticalOffset, a private DP used to animate the scrollviewer
        /// </summary>
        private DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset",
          typeof(double), typeof(BlogPage), new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var app = d as BlogPage;
            app.OnVerticalOffsetChanged(e);
        }

        private void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            ConversationScrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion

        #endregion
    }
}*/