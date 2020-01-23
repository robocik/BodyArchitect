namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrWorkoutCommentsList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrWorkoutCommentsList));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGetMore = new DevExpress.XtraEditors.SimpleButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblEmptyListMessage = new DevExpress.XtraEditors.LabelControl();
            this.usrRating1 = new BodyArchitect.Controls.Rating.usrRating();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.workoutPlanCommentListControl1 = new BodyArchitect.Module.StrengthTraining.Controls.WPF.WorkoutPlanCommentListControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.baGroupControl1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrRating1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 202;
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.CaptionImage = ((System.Drawing.Image)(resources.GetObject("baGroupControl1.CaptionImage")));
            this.baGroupControl1.Controls.Add(this.tableLayoutPanel1);
            this.baGroupControl1.Controls.Add(this.lblEmptyListMessage);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.elementHost1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imageList1, "imageList1");
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGetMore);
            this.panel1.Controls.Add(this.progressIndicator1);
            this.panel1.Controls.Add(this.lblStatus);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btnGetMore
            // 
            resources.ApplyResources(this.btnGetMore, "btnGetMore");
            this.btnGetMore.Image = ((System.Drawing.Image)(resources.GetObject("btnGetMore.Image")));
            this.btnGetMore.Name = "btnGetMore";
            this.btnGetMore.Click += new System.EventHandler(this.btnGetMore_Click);
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // lblEmptyListMessage
            // 
            this.lblEmptyListMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            resources.ApplyResources(this.lblEmptyListMessage, "lblEmptyListMessage");
            this.lblEmptyListMessage.Name = "lblEmptyListMessage";
            // 
            // usrRating1
            // 
            resources.ApplyResources(this.usrRating1, "usrRating1");
            this.usrRating1.Name = "usrRating1";
            this.usrRating1.Voted += new System.EventHandler<BodyArchitect.Controls.Rating.VoteEventArgs>(this.usrRating1_Voted);
            // 
            // elementHost1
            // 
            resources.ApplyResources(this.elementHost1, "elementHost1");
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Child = this.workoutPlanCommentListControl1;
            // 
            // usrWorkoutCommentsList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrWorkoutCommentsList";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private BodyArchitect.Controls.Rating.usrRating usrRating1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.SimpleButton btnGetMore;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.LabelControl lblEmptyListMessage;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WPF.WorkoutPlanCommentListControl workoutPlanCommentListControl1;

    }
}
