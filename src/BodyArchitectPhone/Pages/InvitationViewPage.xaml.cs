using System;
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
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class InvitationViewPage : AnimatedBasePage
    {
        private InvitationItemViewModel viewModel;

        public InvitationViewPage()
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
            if (viewModel.ButtonOkVisible)
            {
                ApplicationBarIconButton button1 =
                    new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAccept_Click);
                button1.Text = ApplicationStrings.AppBarButton_Accept;
                ApplicationBar.Buttons.Add(button1);
            }
            if (viewModel.ButtonCancelVisible)
            {
                var button1 =new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
                button1.Click += new EventHandler(btnCancel_Click);
                button1.Text = ApplicationStrings.AppBarButton_Reject;
                ApplicationBar.Buttons.Add(button1);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if(BAMessageBox.Ask(ApplicationStrings.InvitationViewPage_QAcceptInvitation)==MessageBoxResult.OK)
            {
                inviteOperation(InviteFriendOperation.Accept);
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(BAMessageBox.Ask(ApplicationStrings.InvitationViewPage_QRejectInvitation)==MessageBoxResult.OK)
            {
                inviteOperation(InviteFriendOperation.Reject);
            }
            
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationBar.IsVisible = !ApplicationState.Current.IsOffline;
            string strType;
            if (NavigationContext.QueryString.TryGetValue("InvitationIndex", out strType))
            {
                int msgId = int.Parse(strType);
                viewModel = new InvitationItemViewModel(ApplicationState.Current.ProfileInfo.Invitations[msgId]);
                DataContext = viewModel;

                buildApplicationBar();
            }
        }

        void inviteOperation(InviteFriendOperation operation)
        {
            progressBar.ShowProgress(true, ApplicationStrings.ProfileInfoPage_ProgressSend);
            var m = new ServiceManager<InviteFriendOperationCompletedEventArgs>(delegate(BodyArchitectAccessServiceClient client1, EventHandler<InviteFriendOperationCompletedEventArgs> operationCompleted)
            {
                InviteFriendOperationData data = new InviteFriendOperationData();
                data.Operation = operation;
                data.User = viewModel.User;
                client1.InviteFriendOperationCompleted -= operationCompleted;
                client1.InviteFriendOperationCompleted += operationCompleted;
                client1.InviteFriendOperationAsync(ApplicationState.Current.SessionData.Token, data);
                

            });

            m.OperationCompleted += (s, a) =>
            {
                progressBar.ShowProgress(false);
                FaultException<BAServiceException> faultEx = a.Error as FaultException<BAServiceException>;
                if (a.Error != null && faultEx.Detail.ErrorCode != ErrorCode.ObjectNotFound)
                {
                    BAMessageBox.ShowError(ApplicationStrings.InvitationViewPage_ErrInvitationOperation);
                }
                else
                {
                    ApplicationState.Current.ProfileInfo.Invitations.Remove(viewModel.Invitation);
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            };

            if(!m.Run())
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
            }
        }
    }
}