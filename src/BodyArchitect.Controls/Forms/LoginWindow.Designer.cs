namespace BodyArchitect.Controls.Forms
{
    partial class LoginWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginWindow));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lblConnectionSettings = new DevExpress.XtraEditors.LabelControl();
            this.lblForgotPassword = new DevExpress.XtraEditors.LabelControl();
            this.lblRegister = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.chkAutologin = new DevExpress.XtraEditors.CheckEdit();
            this.lblProfileName = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.baGroupControl2 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.chkEnable100Continue = new DevExpress.XtraEditors.CheckEdit();
            this.lblSendActivationEmail = new DevExpress.XtraEditors.LabelControl();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblForceUpdate = new DevExpress.XtraEditors.LabelControl();
            this.lblConnectionSettingsInfo = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutologin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl2)).BeginInit();
            this.baGroupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkEnable100Continue.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.baGroupControl1);
            this.splitContainerControl1.Panel1.MinSize = 170;
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.baGroupControl2);
            this.splitContainerControl1.Panel2.MinSize = 100;
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
            this.splitContainerControl1.SplitterPosition = 171;
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.lblConnectionSettings);
            this.baGroupControl1.Controls.Add(this.lblForgotPassword);
            this.baGroupControl1.Controls.Add(this.lblRegister);
            this.baGroupControl1.Controls.Add(this.txtName);
            this.baGroupControl1.Controls.Add(this.chkAutologin);
            this.baGroupControl1.Controls.Add(this.lblProfileName);
            this.baGroupControl1.Controls.Add(this.txtPassword);
            this.baGroupControl1.Controls.Add(this.lblPassword);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // lblConnectionSettings
            // 
            resources.ApplyResources(this.lblConnectionSettings, "lblConnectionSettings");
            this.lblConnectionSettings.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblConnectionSettings.Appearance.ForeColor")));
            this.lblConnectionSettings.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblConnectionSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblConnectionSettings.Name = "lblConnectionSettings";
            this.lblConnectionSettings.Click += new System.EventHandler(this.lblConnectionSettings_Click_1);
            // 
            // lblForgotPassword
            // 
            resources.ApplyResources(this.lblForgotPassword, "lblForgotPassword");
            this.lblForgotPassword.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblForgotPassword.Appearance.ForeColor")));
            this.lblForgotPassword.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblForgotPassword.Name = "lblForgotPassword";
            this.lblForgotPassword.Click += new System.EventHandler(this.lblForgotPassword_Click);
            // 
            // lblRegister
            // 
            resources.ApplyResources(this.lblRegister, "lblRegister");
            this.lblRegister.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblRegister.Appearance.ForeColor")));
            this.lblRegister.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRegister.Name = "lblRegister";
            this.lblRegister.Click += new System.EventHandler(this.lblRegister_Click);
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // chkAutologin
            // 
            resources.ApplyResources(this.chkAutologin, "chkAutologin");
            this.chkAutologin.Name = "chkAutologin";
            this.chkAutologin.Properties.AutoWidth = true;
            this.chkAutologin.Properties.Caption = resources.GetString("chkAutologin.Properties.Caption");
            // 
            // lblProfileName
            // 
            resources.ApplyResources(this.lblProfileName, "lblProfileName");
            this.lblProfileName.Name = "lblProfileName";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            // 
            // baGroupControl2
            // 
            this.baGroupControl2.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl2.AppearanceCaption.BackColor")));
            this.baGroupControl2.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl2.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl2.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl2.Controls.Add(this.lblConnectionSettingsInfo);
            this.baGroupControl2.Controls.Add(this.chkEnable100Continue);
            resources.ApplyResources(this.baGroupControl2, "baGroupControl2");
            this.baGroupControl2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl2.Name = "baGroupControl2";
            // 
            // chkEnable100Continue
            // 
            resources.ApplyResources(this.chkEnable100Continue, "chkEnable100Continue");
            this.chkEnable100Continue.Name = "chkEnable100Continue";
            this.chkEnable100Continue.Properties.Caption = resources.GetString("chkEnable100Continue.Properties.Caption");
            superToolTip1.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1");
            toolTipItem1.LeftIndent = 6;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.chkEnable100Continue.SuperTip = superToolTip1;
            this.chkEnable100Continue.CheckedChanged += new System.EventHandler(this.chkEnable100Continue_CheckedChanged);
            // 
            // lblSendActivationEmail
            // 
            resources.ApplyResources(this.lblSendActivationEmail, "lblSendActivationEmail");
            this.lblSendActivationEmail.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblSendActivationEmail.Appearance.ForeColor")));
            this.lblSendActivationEmail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSendActivationEmail.Name = "lblSendActivationEmail";
            this.lblSendActivationEmail.Click += new System.EventHandler(this.lblSendActivationEmail_Click);
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Image = ((System.Drawing.Image)(resources.GetObject("btnLogin.Image")));
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // cancelButton1
            // 
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.CausesValidation = false;
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            this.cancelButton1.Name = "cancelButton1";
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.CircleColor = System.Drawing.Color.DarkOliveGreen;
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lblForceUpdate
            // 
            resources.ApplyResources(this.lblForceUpdate, "lblForceUpdate");
            this.lblForceUpdate.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblForceUpdate.Appearance.ForeColor")));
            this.lblForceUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblForceUpdate.Name = "lblForceUpdate";
            this.lblForceUpdate.Click += new System.EventHandler(this.lblForceUpdate_Click);
            // 
            // lblConnectionSettingsInfo
            // 
            this.lblConnectionSettingsInfo.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl1.Appearance.ForeColor")));
            this.lblConnectionSettingsInfo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblConnectionSettingsInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.lblConnectionSettingsInfo, "lblConnectionSettingsInfo");
            this.lblConnectionSettingsInfo.Name = "lblConnectionSettingsInfo";
            // 
            // LoginWindow
            // 
            this.AcceptButton = this.btnLogin;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton1;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.lblSendActivationEmail);
            this.Controls.Add(this.lblForceUpdate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressIndicator1);
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginWindow";
            this.Load += new System.EventHandler(this.LoginWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.baGroupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutologin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl2)).EndInit();
            this.baGroupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkEnable100Continue.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private BodyArchitect.Controls.Basic.CancelButton cancelButton1;
        private UserControls.ProgressIndicator progressIndicator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl lblForceUpdate;
        private DevExpress.XtraEditors.LabelControl lblSendActivationEmail;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private UserControls.BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.LabelControl lblConnectionSettings;
        private DevExpress.XtraEditors.LabelControl lblForgotPassword;
        private DevExpress.XtraEditors.LabelControl lblRegister;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.CheckEdit chkAutologin;
        private DevExpress.XtraEditors.LabelControl lblProfileName;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.LabelControl lblPassword;
        private UserControls.BaGroupControl baGroupControl2;
        private DevExpress.XtraEditors.CheckEdit chkEnable100Continue;
        private DevExpress.XtraEditors.LabelControl lblConnectionSettingsInfo;
    }
}