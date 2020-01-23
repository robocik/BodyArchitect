namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrWorkoutPlansPagerListView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrWorkoutPlansPagerListView));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.workoutPlansListView1 = new BodyArchitect.Module.StrengthTraining.Controls.WorkoutPlansListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnMoreResults = new DevExpress.XtraEditors.SimpleButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.workoutPlansListView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // workoutPlansListView1
            // 
            resources.ApplyResources(this.workoutPlansListView1, "workoutPlansListView1");
            this.workoutPlansListView1.FullRowSelect = true;
            this.workoutPlansListView1.GridLines = true;
            this.workoutPlansListView1.HideSelection = false;
            this.workoutPlansListView1.Name = "workoutPlansListView1";
            this.workoutPlansListView1.ShowGroups = false;
            this.workoutPlansListView1.UseCompatibleStateImageBehavior = false;
            this.workoutPlansListView1.View = System.Windows.Forms.View.Details;
            this.workoutPlansListView1.SelectedIndexChanged += new System.EventHandler(this.workoutPlansListView1_SelectedIndexChanged);
            this.workoutPlansListView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.workoutPlansListView1_KeyUp);
            this.workoutPlansListView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.workoutPlansListView1_MouseDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.btnMoreResults);
            this.panel1.Controls.Add(this.progressIndicator1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // btnMoreResults
            // 
            resources.ApplyResources(this.btnMoreResults, "btnMoreResults");
            this.btnMoreResults.Image = ((System.Drawing.Image)(resources.GetObject("btnMoreResults.Image")));
            this.btnMoreResults.Name = "btnMoreResults";
            this.btnMoreResults.Click += new System.EventHandler(this.btnMoreResults_Click);
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // usrWorkoutPlansPagerListView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrWorkoutPlansPagerListView";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private WorkoutPlansListView workoutPlansListView1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnMoreResults;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.LabelControl lblStatus;
    }
}
