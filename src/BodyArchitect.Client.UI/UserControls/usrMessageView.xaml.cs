using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using System.Collections.ObjectModel;

namespace BodyArchitect.Client.UI.UserControls
{
    /// <summary>
    /// Interaction logic for usrMessageView.xaml
    /// </summary>
    public partial class usrMessageView
    {
        private MessageDTO message;
        //public event EventHandler<MessageEventArgs> MessageDeleted;
        private ObservableCollection<ImageListItem<MessagePriority>> priorities = new ObservableCollection<ImageListItem<MessagePriority>>();

        public usrMessageView()
        {
            InitializeComponent();
            Priorities.Add(new ImageListItem<MessagePriority>(Strings.MessagePriority_Normal, "PriorityNormal.png".ToResourceString(), MessagePriority.Normal));
            Priorities.Add(new ImageListItem<MessagePriority>(Strings.MessagePriority_Low, "PriorityLow.png".ToResourceString(), MessagePriority.Low));
            Priorities.Add(new ImageListItem<MessagePriority>(Strings.MessagePriority_High, "PriorityHigh.png".ToResourceString(), MessagePriority.High));
            Priorities.Add(new ImageListItem<MessagePriority>(Strings.MessagePriority_System, "PrioritySystem.png".ToResourceString(), MessagePriority.System));
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails
        {
            get { return usrProfileListEntry1.AllowRedirectToDetails; }
            set { usrProfileListEntry1.AllowRedirectToDetails = value; }
        }

        public override bool ValidateData()
        {
            return !string.IsNullOrWhiteSpace(txtMessageTopic.Text);
        }

        public MessageDTO Message
        {
            get { return message; }
            set { message = value; }
        }

        [Required]
        public string MessageTopic
        {
            get { return message != null ? message .Topic: null; }
            set
            {
                if (message != null)
                {
                    message.Topic = value;
                }
            }
        }

        public string MessageContent
        {
            get { return message != null ? message.Content: null; }
            set
            {
                if (message != null)
                {
                    message.Content = value;
                }
            }
        }

        public MessagePriority MessagePriority
        {
            get { return message!=null?message.Priority:MessagePriority.Normal; }
            set
            {
                if (message != null)
                {
                    message.Priority = value;
                }
            }
        }


        public ObservableCollection<ImageListItem<MessagePriority>> Priorities
        {
            get { return priorities; }
        }

        //private void onMessageDeleted(MessageDTO msg)
        //{
        //    if (MessageDeleted != null)
        //    {
        //        MessageDeleted(this, new MessageEventArgs(msg));
        //    }
        //}


        public void Fill(MessageDTO message)
        {
            this.message = message;
            DataContext = this;
            if (message != null)
            {
                grUser.Header = message.Sender.IsMe() ? Strings.Message_Receiver : Strings.Message_Sender;
                //txtMessageTopic.Text = SystemMessages.GetMessageTopic(message);
                //txtMessageContent.Text = SystemMessages.GetMessageContent(message);
                
                usrProfileListEntry1.Fill(message.Sender.IsMe() ? message.Receiver : message.Sender);

                
                
                if (Message.GlobalId != Constants.UnsavedGlobalId )
                {
                    Priorities.Add(new ImageListItem<MessagePriority>(Strings.MessagePriority_System, "PrioritySystem.png".ToResourceString(), MessagePriority.System));
                    baGroupControl1.Header = string.Format(Strings.usrMessageView_MessageGroup_Text, message.CreatedDate.ToLocalTime().ToRelativeDate());
                }
                else if(Priorities.Count==4)
                {
                    Priorities.RemoveAt(3);//remove System
                }
            }
            else
            {
                ClearContent();
            }
            updateGui();

            NotifyOfPropertyChange(() => MessageContent);
            NotifyOfPropertyChange(() => MessageTopic);
            NotifyOfPropertyChange(() => MessagePriority);
            
        }

        public void ClearContent()
        {
            message = null;
            txtMessageTopic.Text = "";
            txtMessageContent.Text = "";
            usrProfileListEntry1.Fill(null);
            updateGui();
        }

        void updateGui()
        {
            buttonsPanel.SetVisible(Message != null && Message.GlobalId != Constants.UnsavedGlobalId);
            //btnReply.Visible = Message != null && Message.Id > Constants.UnsavedObjectId;
            if (Message != null && Message.GlobalId != Constants.UnsavedGlobalId)
            {
                //we cannot reply to the deleted user
                btnReply.IsEnabled = !Message.Receiver.IsDeleted && !Message.Sender.IsDeleted;
            }

            cmbPriorities.IsEnabled = !(Message != null && Message.GlobalId != Constants.UnsavedGlobalId);
            txtMessageTopic.IsReadOnly = Message != null && Message.GlobalId != Constants.UnsavedGlobalId;
            txtMessageContent.IsReadOnly = Message != null && Message.GlobalId != Constants.UnsavedGlobalId;
        }

        //private bool deleteMessage(MessageDTO msg)
        //{
        //    try
        //    {
        //        PleaseWait.Run(delegate
        //        {
        //            var arg = new MessageOperationParam();
        //            arg.Operation = MessageOperationType.Delete;
        //            arg.MessageId = msg.GlobalId;

        //            ServiceManager.MessageOperation(arg);
        //        }, false, null);
        //    }
        //    catch (ObjectNotFoundException ex)
        //    {
        //        ExceptionHandler.Default.Process(ex, Strings.ErrMessageDoesntExist, ErrorWindow.MessageBox);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandler.Default.Process(ex, Strings.ErrCannotDeleteMessage, ErrorWindow.EMailReport);
        //        return false;
        //    }
        //    return true;

        //}

        private void btnDeleteMessage_Click(object sender, EventArgs e)
        {
            //var messagesView=Parent as usrMessagesBoard;
            //if (BAMessageBox.AskYesNo(Strings.QDeleteMessage) == MessageBoxResult.Yes)
            //{
            //    if (deleteMessage(Message))
            //    {
            //        var msg = Message;
            //        ClearContent();
            //        onMessageDeleted(msg);
            //    }
            //}
            var messagesView = Parent.FindVisualParent<usrMessagesBoard>();
            if(messagesView.DeleteMessages(new List<MessageDTO>() {Message}))
            {
                ClearContent();
            }
        }

        private void txtMessageTopic_EditValueChanged(object sender, EventArgs e)
        {
            bool valid = txtMessageTopic.Text.Length > 0;
            OnControlValidated(valid);
        }

        private void btnReply_Click(object sender, EventArgs e)
        {
            SendMessageWindow dlg = new SendMessageWindow();
            MessageDTO msg = new MessageDTO();
            msg.Receiver = message.Sender;
            msg.Sender = UserContext.Current.CurrentProfile;
            msg.Topic = string.Format(Strings.usrMessageView_ReplyTopic, message.Topic);
            dlg.Fill(msg);
            dlg.ShowDialog();
        }
    }
}
