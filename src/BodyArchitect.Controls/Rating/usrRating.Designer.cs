namespace BodyArchitect.Controls.Rating
{
    partial class usrRating
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrRating));
            this.lblCannotVoteMessage = new DevExpress.XtraEditors.LabelControl();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnRateIt = new DevExpress.XtraEditors.SimpleButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.txtShortComment = new DevExpress.XtraEditors.MemoEdit();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbUserRating = new BodyArchitect.Controls.RatingBar();
            this.lblUserRating = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbGlobalRating = new BodyArchitect.Controls.RatingBar();
            this.lblGlobalRating = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtShortComment.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCannotVoteMessage
            // 
            resources.ApplyResources(this.lblCannotVoteMessage, "lblCannotVoteMessage");
            this.lblCannotVoteMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblCannotVoteMessage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblCannotVoteMessage.Name = "lblCannotVoteMessage";
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.CaptionImage = ((System.Drawing.Image)(resources.GetObject("baGroupControl1.CaptionImage")));
            this.baGroupControl1.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCannotVoteMessage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtShortComment, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnRateIt);
            this.panel3.Controls.Add(this.progressIndicator1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // btnRateIt
            // 
            resources.ApplyResources(this.btnRateIt, "btnRateIt");
            this.btnRateIt.Name = "btnRateIt";
            this.btnRateIt.Click += new System.EventHandler(this.btnRateIt_Click);
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.AutoStart = true;
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // txtShortComment
            // 
            resources.ApplyResources(this.txtShortComment, "txtShortComment");
            this.txtShortComment.Name = "txtShortComment";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel2.Controls.Add(this.rbUserRating);
            this.panel2.Controls.Add(this.lblUserRating);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // rbUserRating
            // 
            this.rbUserRating.BackColor = System.Drawing.Color.LightSteelBlue;
            this.rbUserRating.BarBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbUserRating.Gap = ((byte)(1));
            this.rbUserRating.IconEmpty = ((System.Drawing.Image)(resources.GetObject("rbUserRating.IconEmpty")));
            this.rbUserRating.IconFull = ((System.Drawing.Image)(resources.GetObject("rbUserRating.IconFull")));
            this.rbUserRating.IconHalf = ((System.Drawing.Image)(resources.GetObject("rbUserRating.IconHalf")));
            this.rbUserRating.IconsCount = ((byte)(6));
            resources.ApplyResources(this.rbUserRating, "rbUserRating");
            this.rbUserRating.Name = "rbUserRating";
            this.rbUserRating.Rate = 0F;
            // 
            // lblUserRating
            // 
            resources.ApplyResources(this.lblUserRating, "lblUserRating");
            this.lblUserRating.Name = "lblUserRating";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel1.Controls.Add(this.rbGlobalRating);
            this.panel1.Controls.Add(this.lblGlobalRating);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // rbGlobalRating
            // 
            this.rbGlobalRating.BackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGlobalRating.BarBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGlobalRating.Gap = ((byte)(1));
            this.rbGlobalRating.IconEmpty = ((System.Drawing.Image)(resources.GetObject("rbGlobalRating.IconEmpty")));
            this.rbGlobalRating.IconFull = ((System.Drawing.Image)(resources.GetObject("rbGlobalRating.IconFull")));
            this.rbGlobalRating.IconHalf = ((System.Drawing.Image)(resources.GetObject("rbGlobalRating.IconHalf")));
            this.rbGlobalRating.IconsCount = ((byte)(6));
            resources.ApplyResources(this.rbGlobalRating, "rbGlobalRating");
            this.rbGlobalRating.Name = "rbGlobalRating";
            this.rbGlobalRating.Rate = 0F;
            this.rbGlobalRating.ReadOnly = true;
            // 
            // lblGlobalRating
            // 
            resources.ApplyResources(this.lblGlobalRating, "lblGlobalRating");
            this.lblGlobalRating.Name = "lblGlobalRating";
            // 
            // usrRating
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.baGroupControl1);
            this.Name = "usrRating";
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtShortComment.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblCannotVoteMessage;
        private UserControls.BaGroupControl baGroupControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private RatingBar rbGlobalRating;
        private DevExpress.XtraEditors.LabelControl lblGlobalRating;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.SimpleButton btnRateIt;
        private UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.MemoEdit txtShortComment;
        private System.Windows.Forms.Panel panel2;
        private RatingBar rbUserRating;
        private DevExpress.XtraEditors.LabelControl lblUserRating;
    }
}
