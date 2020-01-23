using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls.Forms;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfileInfoView : usrBaseControl, IMainTabControl
    {
        public usrProfileInfoView()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UserContext.ProfileInformationChanged += new EventHandler(UserContext_ProfileInformationChanged);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UserContext.ProfileInformationChanged -= new EventHandler(UserContext_ProfileInformationChanged);
            base.OnHandleDestroyed(e);
        }

        void UserContext_ProfileInformationChanged(object sender, EventArgs e)
        {
            Fill();
        }

        protected override void LoginStatusChanged(LoginStatus newStatus)
        {
            if (newStatus != LoginStatus.Logged)
            {
                usrUserInformation1.ClearContent();
            }
        }
        public void Fill()
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(Fill));
            }
            else
            {
                if (UserContext.IsConnected && UserContext.LoginStatus == LoginStatus.Logged)
                {
                    usrUserInformation1.Fill(UserContext.CurrentProfile);
                }
                else
                {
                    usrUserInformation1.ClearContent();

                }
            }
            
        }

        public void RefreshView()
        {
            ParentWindow.RunAsynchronousOperation(delegate
            {
                UserContext.RefreshUserData();
                //we don't need to invoke fill method because there is an event and fill will be called automatically
            });
        }
    }
}
