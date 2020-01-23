using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Forms
{
    partial class SendErrorWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendErrorWindow));
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.grMessage = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtContent = new DevExpress.XtraEditors.MemoEdit();
            this.lblContent = new DevExpress.XtraEditors.LabelControl();
            this.txtSubject = new DevExpress.XtraEditors.TextEdit();
            this.lblSubject = new DevExpress.XtraEditors.LabelControl();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            this.btnDontSend = new DevExpress.XtraEditors.SimpleButton();
            this.chkApplyAlways = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.grMessage)).BeginInit();
            this.grMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtContent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubject.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkApplyAlways.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.Name = "lblDescription";
            // 
            // grMessage
            // 
            resources.ApplyResources(this.grMessage, "grMessage");
            this.grMessage.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grMessage.AppearanceCaption.BackColor")));
            this.grMessage.AppearanceCaption.Options.UseBackColor = true;
            this.grMessage.AppearanceCaption.Options.UseTextOptions = true;
            this.grMessage.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grMessage.Controls.Add(this.txtContent);
            this.grMessage.Controls.Add(this.lblContent);
            this.grMessage.Controls.Add(this.txtSubject);
            this.grMessage.Controls.Add(this.lblSubject);
            this.grMessage.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grMessage.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grMessage.Name = "grMessage";
            // 
            // txtContent
            // 
            resources.ApplyResources(this.txtContent, "txtContent");
            this.txtContent.Name = "txtContent";
            this.txtContent.Properties.ReadOnly = true;
            // 
            // lblContent
            // 
            resources.ApplyResources(this.lblContent, "lblContent");
            this.lblContent.Name = "lblContent";
            // 
            // txtSubject
            // 
            resources.ApplyResources(this.txtSubject, "txtSubject");
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Properties.ReadOnly = true;
            // 
            // lblSubject
            // 
            resources.ApplyResources(this.lblSubject, "lblSubject");
            this.lblSubject.Name = "lblSubject";
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSend.Name = "btnSend";
            // 
            // btnDontSend
            // 
            resources.ApplyResources(this.btnDontSend, "btnDontSend");
            this.btnDontSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDontSend.Name = "btnDontSend";
            // 
            // chkApplyAlways
            // 
            resources.ApplyResources(this.chkApplyAlways, "chkApplyAlways");
            this.chkApplyAlways.Name = "chkApplyAlways";
            this.chkApplyAlways.Properties.AutoWidth = true;
            this.chkApplyAlways.Properties.Caption = resources.GetString("chkApplyAlways.Properties.Caption");
            // 
            // SendErrorWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkApplyAlways);
            this.Controls.Add(this.btnDontSend);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.grMessage);
            this.Controls.Add(this.lblDescription);
            this.Name = "SendErrorWindow";
            this.Load += new System.EventHandler(this.SendErrorWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grMessage)).EndInit();
            this.grMessage.ResumeLayout(false);
            this.grMessage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtContent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubject.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkApplyAlways.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblDescription;
        private BaGroupControl grMessage;
        private DevExpress.XtraEditors.LabelControl lblSubject;
        private DevExpress.XtraEditors.MemoEdit txtContent;
        private DevExpress.XtraEditors.LabelControl lblContent;
        private DevExpress.XtraEditors.TextEdit txtSubject;
        private DevExpress.XtraEditors.SimpleButton btnSend;
        private DevExpress.XtraEditors.SimpleButton btnDontSend;
        private DevExpress.XtraEditors.CheckEdit chkApplyAlways;
    }
}