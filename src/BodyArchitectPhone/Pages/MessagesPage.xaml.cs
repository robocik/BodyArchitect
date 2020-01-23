using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Threading;
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
using BodyArchitect.WP7.Controls.Cache;
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TombstoneHelper;
using WP7ConversationView;

namespace BodyArchitect.WP7.Pages
{
    public partial class MessagesPage
    {
        private MessagesViewModel viewModel;

        public MessagesPage()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (lstMessages.IsSelectionEnabled)
            {
                lstMessages.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            buildApplicationBar();
            this.RestoreState();  // <- second line
            string pageIdString;
            int pageId=0;
            if (NavigationContext.QueryString.TryGetValue("Page", out pageIdString))
            {
                int.TryParse(pageIdString, out pageId);
            }
            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", pageId);
            pivot.SelectedIndex = pivotItem;

            fill();
        }

        private void fill()
        {
            viewModel = new MessagesViewModel(ApplicationState.Current.ProfileInfo);
            DataContext = viewModel;

            lblNoMessage.Visibility = viewModel.Messages.Count == 0
                                          ? System.Windows.Visibility.Visible
                                          : System.Windows.Visibility.Collapsed;

            lblNoInvitations.Visibility = viewModel.Invitations.Count == 0
                                              ? System.Windows.Visibility.Visible
                                              : System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.State["PivotSelectedTab"] = pivot.SelectedIndex;
            this.SaveState();
        }

        private void ShowMessage_Click(object sender, RoutedEventArgs e)
        {
            if (pivot.IsHitTestVisible)
            {
                HyperlinkButton btn = (HyperlinkButton) sender;
                var msg = (Message) btn.Tag;
                var msgViewModel = msg.MessageViewModel;
                this.Navigate("/Pages/MessageViewPage.xaml?MessageId=" + msgViewModel.Message.GlobalId);    
                
            }
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            pivot.IsHitTestVisible = false;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            pivot.IsHitTestVisible = true;
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (Message)(sender as FrameworkElement).DataContext;
            var msgViewModel = item.MessageViewModel;
            progressBar.ShowProgress(true,ApplicationStrings.ProgressDeleting,true,true);
            msgViewModel.OperationCompleted += new EventHandler<OperationCompletedEventArgs>(item_OperationCompleted);
            msgViewModel.Delete();
        }

        void item_OperationCompleted(object sender, EventArgs e)
        {
            var item = (MessageViewModel) sender;

            viewModel.Messages.Remove(viewModel.Messages.Where(x => x.MessageViewModel == item).Single());
            (item).OperationCompleted -= new EventHandler<OperationCompletedEventArgs>(item_OperationCompleted);
            progressBar.ShowProgress(false);
            lblNoMessage.Visibility = viewModel.Messages.Count == 0
                                          ? System.Windows.Visibility.Visible
                                          : System.Windows.Visibility.Collapsed;
        }

        private void ShowInvitation_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = (HyperlinkButton)sender;
            var msg = (Message)btn.Tag;
            var msgViewModel = msg.InvitationItemViewModel;
            int index = ApplicationState.Current.ProfileInfo.Invitations.IndexOf(msgViewModel.Invitation);
            this.Navigate("/Pages/InvitationViewPage.xaml?InvitationIndex=" + index);
        }

        private void EmailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultiselectList target = (MultiselectList)sender;
            if(ApplicationBar.Buttons.Count==0)
            {
                return;
            }
            ApplicationBarIconButton btnDelete = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            if (target.IsSelectionEnabled)
            {
               
                if (target.SelectedItems.Count > 0)
                {
                    btnDelete.IsEnabled = true;
                }
                else
                {
                    btnDelete.IsEnabled = false;
                }
            }
        }

        void buildApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            if(pivot.SelectedIndex==0 && !ApplicationState.Current.IsOffline)
            {
                ApplicationBar.IsVisible = true;
                if(lstMessages.IsSelectionEnabled)
                {
                    var button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.delete.rest.png", UriKind.Relative));
                    button1.Click += btnDeleteMessages_Click;
                    button1.Text = ApplicationStrings.AppBarButton_Delete;
                    button1.IsEnabled = lstMessages.SelectedItems.Count > 0;
                    ApplicationBar.Buttons.Add(button1);    
                }
                else
                {
                    var button1 = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative));
                    button1.Click += btnSelectMessages_Click;
                    button1.Text = ApplicationStrings.AppBarButton_Select;
                    ApplicationBar.Buttons.Add(button1);    
                }
                
            }
            else
            {
                ApplicationBar.IsVisible = false;
            }
        }

        void btnSelectMessages_Click(object sender, EventArgs e)
        {
            lstMessages.IsSelectionEnabled = true;
        }

        void btnDeleteMessages_Click(object sender, EventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable() || ApplicationState.Current.IsOffline)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                return;
            }

            var messagesToDelete =lstMessages.SelectedItems.Cast<Message>().Select(x => x.MessageViewModel.Message).ToList();
            foreach (var message in messagesToDelete)
            {
                ApplicationState.Current.Cache.Messages.Remove(message.GlobalId);
            }

            ThreadPool.QueueUserWorkItem((a1) =>
            {
                foreach (var message in messagesToDelete)
                {
                    ServicePool.Add(new DeleteMessageServiceCommand(message.GlobalId));
                }
            });
            fill();
            lstMessages.IsSelectionEnabled = false;

        }
        private void EmailList_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            buildApplicationBar();
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buildApplicationBar();
        }
    }
}