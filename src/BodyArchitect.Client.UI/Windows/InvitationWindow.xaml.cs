using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Resources.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Client.UI.Windows
{
    /// <summary>
    /// Interaction logic for InvitationWindow.xaml
    /// </summary>
    public partial class InvitationWindow
    {
        private UserDTO user;
        private InviteFriendOperation operation;
        private string message;

        public InvitationWindow()
        {
            InitializeComponent();
        }

        public void Fill(UserDTO user, InviteFriendOperation operation)
        {
            this.user = user;
            this.operation = operation;
            usrProfileListEntry1.Fill(user);
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyOfPropertyChange(()=>Message);
            }
        }

        private void usrProgressIndicatorButtons1_OkClick(object sender, CancellationSourceEventArgs e)
        {
            try
            {
                InviteFriendOperationData data = new InviteFriendOperationData();
               
                data.User = user;
                data.Operation = operation;
                data.Message = Message;
                ServiceManager.InviteFriendOperation(data);
                UserContext.Current.RefreshUserData();
                ThreadSafeClose(true);
            }
            catch (CannotAcceptRejectInvitationDoesntExistException ex)
            {
                UserContext.Current.RefreshUserData();
                ExceptionHandler.Default.Process(ex, Strings.ErrorInvitationDoesntExist, ErrorWindow.MessageBox);
                ThreadSafeClose(true);
            }

        }
    }
}
