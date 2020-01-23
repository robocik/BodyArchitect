namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrUserWorkoutPlans
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrUserWorkoutPlans));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.usrWorkoutPlansPagerListView1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutPlansPagerListView();
            this.usrWorkoutCommentsList1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutCommentsList();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.usrWorkoutPlansPagerListView1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrWorkoutCommentsList1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 174;
            // 
            // usrWorkoutPlansPagerListView1
            // 
            resources.ApplyResources(this.usrWorkoutPlansPagerListView1, "usrWorkoutPlansPagerListView1");
            this.usrWorkoutPlansPagerListView1.Name = "usrWorkoutPlansPagerListView1";
            this.usrWorkoutPlansPagerListView1.SelectedPlanChanged += new System.EventHandler(this.usrWorkoutPlansPagerListView1_SelectedPlanChanged);
            // 
            // usrWorkoutCommentsList1
            // 
            resources.ApplyResources(this.usrWorkoutCommentsList1, "usrWorkoutCommentsList1");
            this.usrWorkoutCommentsList1.Name = "usrWorkoutCommentsList1";
            // 
            // usrUserWorkoutPlans
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrUserWorkoutPlans";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private usrWorkoutPlansPagerListView usrWorkoutPlansPagerListView1;
        private usrWorkoutCommentsList usrWorkoutCommentsList1;


    }
}
