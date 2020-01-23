using System;
using System.ComponentModel;
using System.Windows.Forms;
using BodyArchitect.Controls.Forms;


namespace BodyArchitect.Controls
{
    public partial class usrBaseControl : DevExpress.XtraEditors.XtraUserControl
    {
        public usrBaseControl()
        {
            InitializeComponent();
            
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UserContext.LoginStatusChanged -= new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            base.OnHandleDestroyed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            UserContext.LoginStatusChanged += new EventHandler<LoginStatusEventArgs>(UserContext_LoginStatusChanged);
            base.OnLoad(e);
            
        }
        void UserContext_LoginStatusChanged(object sender, LoginStatusEventArgs e)
        {
            if (ParentWindow==null)
            {
                return;
            }
            if(InvokeRequired)
            {
                Invoke(new Action<LoginStatus>(LoginStatusChanged), e.Status);
            }
            else
            {
                LoginStatusChanged(e.Status);
            }
        }

        protected virtual void LoginStatusChanged(LoginStatus newStatus)
        {
            
        }

        private const int WM_KEYDOWN = 0x100;

        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                switch ((ConsoleKey)m.WParam.ToInt32())
                {
                    case ConsoleKey.Enter:
                        if (AcceptButton != null)
                        {
                            AcceptButton.PerformClick();
                        }
                        break;
                }
            }
            return base.ProcessKeyPreview(ref m);
        }

        [DefaultValue(null)]
        public IButtonControl AcceptButton { get; set; }

        [Browsable(false)]
        public BaseWindow ParentWindow
        {
            get
            {
                return this.GetParentControl<BaseWindow>();
            }
        }
    }
}
