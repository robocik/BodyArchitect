using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.UserControls
{
    [Export(typeof(IUserDetailControlBuilder))]
    public class usrMessagesBoardBuilder : IUserDetailControlBuilder
    {
        public IUserDetailControl Create()
        {
            return new usrMessagesBoard();
        }
    }


    public partial class usrMessagesBoard : IUserDetailControl, IWeakEventListener
    {
        private ProfileInformationDTO profileInfo;
        private ObservableCollection<MessageDTO> messages = new ObservableCollection<MessageDTO>();

        public usrMessagesBoard()
        {
            InitializeComponent();
            CollectionChangedEventManager.AddListener(MessagesReposidory.Instance, this);
            //usrMessageView1.Visible = false;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var param = (NotifyCollectionChangedEventArgs)e;
            if (param.Action != NotifyCollectionChangedAction.Reset)
            {
                UIHelper.BeginInvoke(() => Fill(profileInfo, true), Dispatcher);
            }
            
            return true;
        }

        public void Fill(ProfileInformationDTO profileInfo, bool isActive)
        {
            this.profileInfo = profileInfo;
            messages.Clear();
            if (isActive && profileInfo != null && profileInfo.User.IsMe())
            {
                MessagesReposidory.Instance.EnsureLoaded();
                foreach (var message in MessagesReposidory.Instance.Items.Values)
                {
                    messages.Add(message);
                }
                msgViewSplit.SetVisible(SelectedMessage != null);
                msgViewSplit.UpdateVisbility();
            }
            lstInvitations.ItemsSource = messages;

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(lstInvitations.ItemsSource);
            myView.SortDescriptions.Add(new SortDescription("CreatedDate", ListSortDirection.Descending));

            NotifyOfPropertyChange(()=>Caption);
        }

        public string Caption
        {
            get
            {
                if (profileInfo != null)
                {
                    return string.Format(Strings.usrMessagesBoard_Caption, MessagesReposidory.Instance.Items.Count);
                }
                return string.Format(Strings.usrMessagesBoard_Caption, 0);
            }
        }

        public ImageSource SmallImage
        {
            get
            {
                return "Message.png".ToResourceUrl().ToBitmap();
            }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.User.IsMe();
        }

        public MessageDTO SelectedMessage
        {
            get { return (MessageDTO)lstInvitations.SelectedItem; }
        }

        public ObservableCollection<MessageDTO> Messages
        {
            get { return messages; }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //splitContainerControl1.Collapsed = false;
        }

        //private void usrMessageView1_MessageDeleted(object sender, MessageEventArgs e)
        //{
        //    if (SelectedMessage == e.Message)
        //    {
        //        //listView1.SelectedItems[0].Remove();
        //        UserContext.Current.RefreshUserData();
        //    }
        //}

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && SelectedMessage != null)
            {
                deleteSelectedMessages();
            }
        }

        public bool DeleteMessages(List<MessageDTO> messagesToDelete)
        {
            bool res = false;
            if (BAMessageBox.AskYesNo(Strings.QDeleteSelectedMessages) == MessageBoxResult.Yes)
            {

                try
                {
                    PleaseWait.Run(delegate(MethodParameters par)
                    {
                        for (int index = 0; index < messagesToDelete.Count; index++)
                        {
                            var messageDto = messagesToDelete[index];
                            try
                            {
                                var arg = new MessageOperationParam();
                                arg.Operation = MessageOperationType.Delete;
                                arg.MessageId = messageDto.GlobalId;

                                if (par.Cancel)
                                {
                                    return;
                                }

                                ServiceManager.MessageOperation(arg);
                                par.SetProgress(index + 1);
                            }
                            catch (ObjectNotFoundException ex)
                            {
                                //message doesn't exists so simply skip it
                                ExceptionHandler.Default.Process(ex);
                            }
                            MessagesReposidory.Instance.Remove(messageDto.GlobalId);
                            res= true;
                        }
                    }, true, null, messagesToDelete.Count);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Default.Process(ex, Strings.ErrCannotDeleteMessage, ErrorWindow.EMailReport);
                }

            }
            return res;
        }

        void deleteSelectedMessages()
        {
            List<MessageDTO> messagesToDelete = new List<MessageDTO>();
            foreach (MessageDTO item in lstInvitations.SelectedItems)
            {
                var messageToDelete = item;
                messagesToDelete.Add(messageToDelete);
            }
            DeleteMessages(messagesToDelete);
        }

        private void btnUserInfo_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            UserDTO user =(UserDTO) btn.Tag;
            if (!user.IsDeleted && !user.IsMe())
            {
                MainWindow.Instance.ShowUserInformation(user);
            }
        }

        private void lstInvitations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            usrMessageView1.Fill(SelectedMessage);
            msgViewSplit.SetVisible(SelectedMessage != null);
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;
            MessageDTO message =(MessageDTO) btn.Tag;
            lstInvitations.SelectedItem = message;
            deleteSelectedMessages();
        }

        
    }
}
