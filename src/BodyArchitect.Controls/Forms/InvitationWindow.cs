using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Controls.ProgressIndicator;
using BodyArchitect.Logger;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.Forms
{
    public partial class InvitationWindow : BaseWindow
    {
        private UserDTO user;
        private InviteFriendOperation operation;

        public InvitationWindow()
        {
            InitializeComponent();
        }

        public void Fill(UserDTO user,InviteFriendOperation operation)
        {
            this.user = user;
            this.operation = operation;
            usrProfileListEntry1.Fill(user);
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                InviteFriendOperationData data = new InviteFriendOperationData();
                data.User = user;
                data.Operation = operation;
                data.Message = memoEdit1.Text;
                ServiceManager.InviteFriendOperation( data);
                UserContext.RefreshUserData();
                ThreadSafeClose();
            }
            catch (CannotAcceptRejectInvitationDoesntExistException ex)
            {
                UserContext.RefreshUserData();
                ExceptionHandler.Default.Process(ex, ApplicationStrings.ErrorInvitationDoesntExist, ErrorWindow.MessageBox);
                ThreadSafeClose(System.Windows.Forms.DialogResult.OK);
            }
            
        }
    }
}