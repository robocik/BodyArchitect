using System;
using System.Collections.Generic;
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
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class MessageViewPage : AnimatedBasePage
    {
        private MessageViewModel viewModel;

        public MessageViewPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.delete.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnDelete_Click);
            button1.Text = ApplicationStrings.AppBarButton_Delete;
            ApplicationBar.Buttons.Add(button1);

            button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.feature.email.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnReply_Click);
            button1.Text = ApplicationStrings.AppBarButton_Reply;
            ApplicationBar.Buttons.Add(button1);
            updateApplicationBarButtons();
        }


        private void updateApplicationBarButtons()
        {
            ApplicationBar.IsVisible = !ApplicationState.Current.IsOffline;
            var btn1 = (IApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn1.IsEnabled = viewModel.CanReply;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string strType;
            if (NavigationContext.QueryString.TryGetValue("MessageId", out strType))
            {
                var msgId = Guid.Parse(strType);
                viewModel = new MessageViewModel(ApplicationState.Current.Cache.Messages.GetItem(msgId));
                DataContext = viewModel;
            }
            buildApplicationBar();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            var sendMessage = e.Content as SendMessagePage;
            if (sendMessage != null)
            {
                sendMessage.Receiver = viewModel.User;
                sendMessage.Topic = string.Format(ApplicationStrings.MessageViewPage_ReplyTopicTemplate,viewModel.Topic);
            }
            base.OnNavigatedFrom(e);
        }

        void item_OperationCompleted(object sender, EventArgs e)
        {
            var item = (MessageViewModel)sender;
            item.OperationCompleted -= new EventHandler<OperationCompletedEventArgs>(item_OperationCompleted);
            progressBar.ShowProgress(false);
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.ProgressDeleting);
            viewModel.OperationCompleted += new EventHandler<OperationCompletedEventArgs>(item_OperationCompleted);
            viewModel.Delete();
           
        }

        private void btnReply_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/SendMessagePage.xaml");
        }

    }
}