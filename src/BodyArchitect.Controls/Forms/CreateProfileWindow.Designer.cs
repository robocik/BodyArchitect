using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Controls.Forms
{
    partial class CreateProfileWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateProfileWindow));
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.usrCreateProfile1 = new BodyArchitect.Controls.UserControls.usrCreateProfile();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblShowTermsOfService = new DevExpress.XtraEditors.LabelControl();
            this.chkAcceptLicence = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblShowPrivacyPolicy = new DevExpress.XtraEditors.LabelControl();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkAcceptLicence.Properties)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // baGroupControl1
            // 
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.usrCreateProfile1);
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // usrCreateProfile1
            // 
            resources.ApplyResources(this.usrCreateProfile1, "usrCreateProfile1");
            this.usrCreateProfile1.Name = "usrCreateProfile1";
            this.usrCreateProfile1.ControlValidated += new System.EventHandler<BodyArchitect.Controls.ControlValidatedEventArgs>(this.usrCreateProfile1_ControlValidated);
            // 
            // usrProgressIndicatorButtons1
            // 
            this.usrProgressIndicatorButtons1.AllowCancel = false;
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.okButton1_Click);
            this.usrProgressIndicatorButtons1.TaskProgressChanged += new System.EventHandler<BodyArchitect.Controls.UserControls.TaskStateChangedEventArgs>(this.usrProgressIndicatorButtons1_TaskProgressChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Name = "panel1";
            // 
            // lblShowTermsOfService
            // 
            resources.ApplyResources(this.lblShowTermsOfService, "lblShowTermsOfService");
            this.lblShowTermsOfService.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblShowTermsOfService.Appearance.ForeColor")));
            this.lblShowTermsOfService.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblShowTermsOfService.Name = "lblShowTermsOfService";
            this.lblShowTermsOfService.Click += new System.EventHandler(this.lblShowTermsOfService_Click);
            // 
            // chkAcceptLicence
            // 
            resources.ApplyResources(this.chkAcceptLicence, "chkAcceptLicence");
            this.chkAcceptLicence.Name = "chkAcceptLicence";
            this.chkAcceptLicence.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("chkAcceptLicence.Properties.Appearance.Font")));
            this.chkAcceptLicence.Properties.Appearance.Options.UseFont = true;
            this.chkAcceptLicence.Properties.Caption = resources.GetString("chkAcceptLicence.Properties.Caption");
            this.chkAcceptLicence.CheckedChanged += new System.EventHandler(this.chkAcceptLicence_CheckedChanged);
            // 
            // labelControl1
            // 
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // labelControl2
            // 
            resources.ApplyResources(this.labelControl2, "labelControl2");
            this.labelControl2.Name = "labelControl2";
            // 
            // lblShowPrivacyPolicy
            // 
            resources.ApplyResources(this.lblShowPrivacyPolicy, "lblShowPrivacyPolicy");
            this.lblShowPrivacyPolicy.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblShowPrivacyPolicy.Appearance.ForeColor")));
            this.lblShowPrivacyPolicy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblShowPrivacyPolicy.Name = "lblShowPrivacyPolicy";
            this.lblShowPrivacyPolicy.Click += new System.EventHandler(this.lblShowPrivacyPolicy_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.labelControl1);
            this.flowLayoutPanel1.Controls.Add(this.lblShowTermsOfService);
            this.flowLayoutPanel1.Controls.Add(this.labelControl2);
            this.flowLayoutPanel1.Controls.Add(this.lblShowPrivacyPolicy);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // CreateProfileWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.chkAcceptLicence);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.Controls.Add(this.baGroupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateProfileWindow";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.CreateProfileWindow_HelpButtonClicked);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkAcceptLicence.Properties)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.BaGroupControl baGroupControl1;
        private UserControls.usrCreateProfile usrCreateProfile1;
        private ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl lblShowTermsOfService;
        private DevExpress.XtraEditors.CheckEdit chkAcceptLicence;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblShowPrivacyPolicy;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}