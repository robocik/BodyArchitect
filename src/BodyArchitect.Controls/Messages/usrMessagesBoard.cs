using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Common.ProgressOperation;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    [Export(typeof(IUserDetailControl))]
    public partial class usrMessagesBoard : DevExpress.XtraEditors.XtraUserControl, IUserDetailControl
    {
        private ProfileInformationDTO profileInfo;

        public usrMessagesBoard()
        {
            InitializeComponent();
            usrMessageView1.Visible = false;
        }

        public void Fill(ProfileInformationDTO profileInfo, bool isActive)
        {
            this.profileInfo = profileInfo;
            listView1.Items.Clear();
            if (profileInfo != null && isActive)
            {
                foreach (var message in profileInfo.Messages)
                {
                    ListViewItem item =
                        new ListViewItem(new string[] { message.Sender.UserName, SystemMessages.GetMessageTopic(message), message.CreatedDate.ToLocalTime().ToRelativeDate() });
                    item.Tag = message;
                    item.ImageKey = "Message";
                    item.StateImageIndex = getPriorityImageKey(message);
                    listView1.Items.Add(item);
                }
                colTopic.Width = -2;
                usrMessageView1.Visible = SelectedMessage != null;
            }
        }

        public string Caption
        {
            get
            {
                if (profileInfo!=null)
                {
                    return string.Format(ApplicationStrings.usrMessagesBoard_Caption,profileInfo.Messages.Count);    
                }
                return string.Format(ApplicationStrings.usrMessagesBoard_Caption, 0);    
            }
        }

        public Image SmallImage
        {
            get { return Icons.Message; }
        }

        public bool UpdateGui(ProfileInformationDTO user)
        {
            return user != null && user.User.IsMe();
        }

        int getPriorityImageKey(MessageDTO dto)
        {
            switch (dto.Priority)
            {
                    case MessagePriority.System:
                    return 1;
                    case MessagePriority.High:
                    return 2;
                    case MessagePriority.Low:
                    return 4;

            }
            return 3;
        }

        public void ClearContent()
        {
            listView1.Items.Clear();
        }

        public MessageDTO SelectedMessage
        {
            get
            {
                if(listView1.SelectedIndices.Count>0)
                {
                    return (MessageDTO)listView1.SelectedItems[0].Tag;
                }
                return null;
            }
        }
 
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            usrMessageView1.Fill(SelectedMessage);
            usrMessageView1.Visible = SelectedMessage != null;
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            splitContainerControl1.Collapsed = false;
        }

        private void usrMessageView1_MessageDeleted(object sender, MessageEventArgs e)
        {
            if(SelectedMessage==e.Message)
            {
                //listView1.SelectedItems[0].Remove();
                UserContext.RefreshUserData();
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && SelectedMessage!=null)
            {
                if (FMMessageBox.AskYesNo(ApplicationStrings.QDeleteSelectedMessages) == DialogResult.Yes)
                {
                    List<MessageDTO> messagesToDelete = new List<MessageDTO>();
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        var messageToDelete = (MessageDTO)item.Tag;
                        messagesToDelete.Add(messageToDelete);
                    }

                    
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
                                    arg.MessageId = messageDto.Id;

                                    if (par.Cancel)
                                    {
                                        return;
                                    }

                                    ServiceManager.MessageOperation(arg);
                                    par.SetProgress(index+1);
                                }
                                catch (ObjectNotFoundException ex)
                                {
                                    //message doesn't exists so simply skip it
                                    ExceptionHandler.Default.Process(ex);
                                }
                            }
                        }, true, messagesToDelete.Count);
                        UserContext.RefreshUserData();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrCannotDeleteMessage, ErrorWindow.EMailReport);
                    }
                    
                }
            }
        }
    }
}
