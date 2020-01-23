namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfilePersonalInfo
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfilePersonalInfo));
            this.baGroupControl2 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtAbout = new DevExpress.XtraEditors.MemoEdit();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.baPictureEdit1 = new BodyArchitect.Controls.Basic.BaPictureEdit();
            this.lblImageDescription = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl2)).BeginInit();
            this.baGroupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baPictureEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // baGroupControl2
            // 
            resources.ApplyResources(this.baGroupControl2, "baGroupControl2");
            this.baGroupControl2.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl2.AppearanceCaption.BackColor")));
            this.baGroupControl2.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl2.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl2.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl2.Controls.Add(this.txtAbout);
            this.baGroupControl2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl2.Name = "baGroupControl2";
            // 
            // txtAbout
            // 
            resources.ApplyResources(this.txtAbout, "txtAbout");
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.Properties.NullValuePrompt = resources.GetString("txtAbout.Properties.NullValuePrompt");
            this.txtAbout.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("txtAbout.Properties.NullValuePromptShowForEmptyValue")));
            // 
            // baGroupControl1
            // 
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.baPictureEdit1);
            this.baGroupControl1.Controls.Add(this.lblImageDescription);
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // baPictureEdit1
            // 
            resources.ApplyResources(this.baPictureEdit1, "baPictureEdit1");
            this.baPictureEdit1.Name = "baPictureEdit1";
            this.baPictureEdit1.Properties.NullText = resources.GetString("baPictureEdit1.Properties.NullText");
            this.baPictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            // 
            // lblImageDescription
            // 
            this.lblImageDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblImageDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.lblImageDescription, "lblImageDescription");
            this.lblImageDescription.Name = "lblImageDescription";
            // 
            // usrProfilePersonalInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.baGroupControl2);
            this.Controls.Add(this.baGroupControl1);
            this.Name = "usrProfilePersonalInfo";
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl2)).EndInit();
            this.baGroupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAbout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baPictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl baGroupControl1;
        private BaGroupControl baGroupControl2;
        private DevExpress.XtraEditors.MemoEdit txtAbout;
        private DevExpress.XtraEditors.LabelControl lblImageDescription;
        private Basic.BaPictureEdit baPictureEdit1;
    }
}
