using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrMessageView : DevExpress.XtraEditors.XtraUserControl
    {
        private MessageDTO message;
        public event EventHandler<ControlValidatedEventArgs> ControlValidated;
        public event EventHandler<MessageEventArgs> MessageDeleted;

        public usrMessageView()
        {
            InitializeComponent();
        }

        [DefaultValue(true)]
        public bool AllowRedirectToDetails
        {
            get { return usrProfileListEntry1.AllowRedirectToDetails; }
            set { usrProfileListEntry1.AllowRedirectToDetails = value; }
        }
        
        public MessageDTO Message
        {
            get { return message; }
        }

        private void onMessageDeleted(MessageDTO msg)
        {
            if(MessageDeleted!=null)
            {
                MessageDeleted(this,new MessageEventArgs(msg));
            }
        }

        protected void OnControlValidated(bool isValid)
        {
            if (ControlValidated != null)
            {
                ControlValidated(this, new ControlValidatedEventArgs(isValid));
            }
        }
        public MessageDTO SaveChanges()
        {
            Message.Topic = txtMessageTopic.Text;
            message.Content = txtMessageContent.Text;
            message.MessageType = MessageType.Custom;
            message.Priority=(MessagePriority) imageComboBoxEdit1.SelectedIndex;
            message.ContentType = ContentType.Text;
            return message;
        }
        public void Fill(MessageDTO message)
        {
            this.message = message;
            if (message != null)
            {
                grUser.Text = message.Sender.IsMe() ? ApplicationStrings.Message_Receiver : ApplicationStrings.Message_Sender;
                txtMessageTopic.Text = SystemMessages.GetMessageTopic(message);
                txtMessageContent.Text = SystemMessages.GetMessageContent(message);
                usrProfileListEntry1.Fill(message.Sender.IsMe()?message.Receiver:message.Sender);
                if(Message.Id == Constants.UnsavedObjectId)
                {
                    //if we send message then we should remove system priority
                    imageComboBoxEdit1.Properties.Items.RemoveAt(imageComboBoxEdit1.Properties.Items.Count-1);
                }
                else
                {
                    baGroupControl1.Text = string.Format(ApplicationStrings.usrMessageView_MessageGroup_Text, message.CreatedDate.ToLocalTime().ToRelativeDate());
                }
                imageComboBoxEdit1.SelectedIndex =(int) message.Priority;
            }
            else
            {
                ClearContent();
            }
            updateGui();
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
            flowLayoutPanel1.Visible = Message != null && Message.Id > Constants.UnsavedObjectId;
            //btnReply.Visible = Message != null && Message.Id > Constants.UnsavedObjectId;
            if (Message != null && Message.Id > Constants.UnsavedObjectId)
            {
                //we cannot reply to the deleted user
                btnReply.Enabled = !Message.Receiver.IsDeleted && !Message.Sender.IsDeleted;
            }
            
            imageComboBoxEdit1.Properties.ReadOnly = Message != null && Message.Id > Constants.UnsavedObjectId;
            txtMessageTopic.Properties.ReadOnly = Message != null && Message.Id > Constants.UnsavedObjectId;
            txtMessageContent.Properties.ReadOnly = Message != null && Message.Id > Constants.UnsavedObjectId;
        }

        private bool deleteMessage(MessageDTO msg)
        {
            
           try
           {
               PleaseWait.Run(delegate
               {
                   var arg = new MessageOperationParam();
                   arg.Operation = MessageOperationType.Delete;
                   arg.MessageId = msg.Id;
                   
                   ServiceManager.MessageOperation(arg);
                   }, false,null);
           }
           catch (ObjectNotFoundException ex)
           {
               ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrMessageDoesntExist,ErrorWindow.MessageBox);
           }
           catch (Exception ex)
           {
               ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrCannotDeleteMessage,ErrorWindow.EMailReport);
               return false;
           }
           return true;

        }
        private void btnDeleteMessage_Click(object sender, EventArgs e)
        {
            if (FMMessageBox.AskYesNo(ApplicationStrings.QDeleteMessage) == DialogResult.Yes)
            {
                if (deleteMessage(Message))
                {
                    var msg = Message;
                    ClearContent();
                    onMessageDeleted(msg);
                }
            }
        }

        private void validationProvider1_ValidationPerformed(object sender, Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs e)
        {
            dxErrorProvider1.SetError(e.ValidatedControl, null, ErrorType.None);
            foreach (ValidationResult validationResult in e.ValidationResults)
            {
                dxErrorProvider1.SetError(e.ValidatedControl, validationResult.Message, ErrorType.Default);
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
            msg.Sender = UserContext.CurrentProfile;
            msg.Topic = string.Format(ApplicationStrings.usrMessageView_ReplyTopic, message.Topic);
            dlg.Fill(msg);
            dlg.ShowDialog(this);
        }

    }
}
