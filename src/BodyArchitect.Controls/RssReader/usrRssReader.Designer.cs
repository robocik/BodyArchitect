namespace BodyArchitect.Controls.RssReader
{
    partial class usrRssReader
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrRssReader));
            this.grHeadline = new DevExpress.XtraEditors.GroupControl();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.lblRssDescription = new DevExpress.XtraEditors.LabelControl();
            this.hlRssTitle = new DevExpress.XtraEditors.HyperLinkEdit();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grHeadline)).BeginInit();
            this.grHeadline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hlRssTitle.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grHeadline
            // 
            this.grHeadline.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("grHeadline.Appearance.BackColor")));
            this.grHeadline.Appearance.BackColor2 = ((System.Drawing.Color)(resources.GetObject("grHeadline.Appearance.BackColor2")));
            this.grHeadline.Appearance.Options.UseBackColor = true;
            this.grHeadline.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grHeadline.AppearanceCaption.BackColor")));
            this.grHeadline.AppearanceCaption.Options.UseBackColor = true;
            this.grHeadline.AppearanceCaption.Options.UseTextOptions = true;
            this.grHeadline.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grHeadline.CaptionImage = ((System.Drawing.Image)(resources.GetObject("grHeadline.CaptionImage")));
            this.grHeadline.Controls.Add(this.progressIndicator1);
            this.grHeadline.Controls.Add(this.lblRssDescription);
            this.grHeadline.Controls.Add(this.hlRssTitle);
            resources.ApplyResources(this.grHeadline, "grHeadline");
            this.grHeadline.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grHeadline.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grHeadline.Name = "grHeadline";
            this.grHeadline.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.grHeadline_MouseDoubleClick);
            // 
            // progressIndicator1
            // 
            this.progressIndicator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(135)))), ((int)(((byte)(220)))));
            this.progressIndicator1.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // lblRssDescription
            // 
            this.lblRssDescription.AllowHtmlString = true;
            resources.ApplyResources(this.lblRssDescription, "lblRssDescription");
            this.lblRssDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblRssDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblRssDescription.Name = "lblRssDescription";
            this.lblRssDescription.UseMnemonic = false;
            // 
            // hlRssTitle
            // 
            resources.ApplyResources(this.hlRssTitle, "hlRssTitle");
            this.hlRssTitle.Name = "hlRssTitle";
            this.hlRssTitle.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("hlRssTitle.Properties.Appearance.Font")));
            this.hlRssTitle.Properties.Appearance.Options.UseFont = true;
            this.hlRssTitle.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.hlRssTitle.Properties.UseParentBackground = true;
            this.hlRssTitle.TabStop = false;
            this.hlRssTitle.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(this.hlRssTitle_OpenLink);
            // 
            // timer1
            // 
            this.timer1.Interval = 600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // usrRssReader
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grHeadline);
            this.Name = "usrRssReader";
            ((System.ComponentModel.ISupportInitialize)(this.grHeadline)).EndInit();
            this.grHeadline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hlRssTitle.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grHeadline;
        private DevExpress.XtraEditors.HyperLinkEdit hlRssTitle;
        private DevExpress.XtraEditors.LabelControl lblRssDescription;
        private UserControls.ProgressIndicator progressIndicator1;
        private System.Windows.Forms.Timer timer1;
    }
}
