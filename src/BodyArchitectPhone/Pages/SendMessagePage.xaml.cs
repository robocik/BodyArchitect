using System;
using System.Collections.Generic;
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
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Shell;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class SendMessagePage 
    {
        MessageViewModel viewModel;

        public SendMessagePage()
        {
            InitializeComponent();
            buildApplicationBar();
        }

        void buildApplicationBar()
        {
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/mainSent.png", UriKind.Relative));
            button1.Click += new EventHandler(btnSend_Click);
            button1.Text = ApplicationStrings.AppBarButton_Send;
            ApplicationBar.Buttons.Add(button1);
        }


        public UserDTO Receiver { get; set; }

        public string Topic
        {
            get; set;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var message = new MessageDTO();

            StateHelper stateHelper = new StateHelper(this.State);
            Receiver = stateHelper.GetValue("Receiver", Receiver);
            Topic = stateHelper.GetValue("Topic", Topic);
            message.Content = stateHelper.GetValue("Content", string.Empty);
            message.Priority = (MessagePriority)stateHelper.GetValue("Priority", 0);
            message.Topic = Topic;
            message.Receiver = Receiver;
            viewModel=new MessageViewModel(message);
            viewModel.OperationCompleted += new EventHandler<OperationCompletedEventArgs>(viewModel_OperationCompleted);
            DataContext = viewModel;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            State["Receiver"] = Receiver;
            State["Topic"] = txtTopic.Text;
            State["Content"] = txtContent.Text;
            State["Priority"] = lpPriority.SelectedIndex;

        }

        void viewModel_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            
            //
            if (!e.Error)
            {
                viewModel.OperationCompleted -= new EventHandler<OperationCompletedEventArgs>(viewModel_OperationCompleted);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                progressBar.ShowProgress(false); 
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.SendMessagePage_ProgressSending);
            ExtensionMethods.BindFocusedTextBox();
            viewModel.Send();
        }
    }
}