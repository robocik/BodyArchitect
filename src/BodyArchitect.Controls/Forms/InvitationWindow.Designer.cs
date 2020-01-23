using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Controls.Forms
{
    partial class InvitationWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InvitationWindow));
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.usrProfileListEntry1 = new BodyArchitect.Controls.UserControls.usrProfileListEntry();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.usrProgressIndicatorButtons1_OkClick);
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.memoEdit1);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // memoEdit1
            // 
            resources.ApplyResources(this.memoEdit1, "memoEdit1");
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Properties.MaxLength = 500;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // usrProfileListEntry1
            // 
            this.usrProfileListEntry1.AllowRedirectToDetails = false;
            resources.ApplyResources(this.usrProfileListEntry1, "usrProfileListEntry1");
            this.usrProfileListEntry1.Name = "usrProfileListEntry1";
            // 
            // InvitationWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrProfileListEntry1);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.baGroupControl1);
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvitationWindow";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
        private UserControls.BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private UserControls.usrProfileListEntry usrProfileListEntry1;
    }
}