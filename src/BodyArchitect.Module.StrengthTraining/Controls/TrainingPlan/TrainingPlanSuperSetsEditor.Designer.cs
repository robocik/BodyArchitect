namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class TrainingPlanSuperSetsEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainingPlanSuperSetsEditor));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuJoinSuperSet = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveSuperSet = new System.Windows.Forms.ToolStripMenuItem();
            this.btnJoinAsSuperset = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemoveFromSuperset = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.exercisesTrainingPlanListView1 = new BodyArchitect.Module.StrengthTraining.Controls.ExercisesTrainingPlanListView();
            this.lblHelp = new DevExpress.XtraEditors.LabelControl();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuJoinSuperSet,
            this.mnuRemoveSuperSet});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuJoinSuperSet
            // 
            resources.ApplyResources(this.mnuJoinSuperSet, "mnuJoinSuperSet");
            this.mnuJoinSuperSet.Name = "mnuJoinSuperSet";
            this.mnuJoinSuperSet.Click += new System.EventHandler(this.mnuJoinSuperSet_Click);
            // 
            // mnuRemoveSuperSet
            // 
            resources.ApplyResources(this.mnuRemoveSuperSet, "mnuRemoveSuperSet");
            this.mnuRemoveSuperSet.Name = "mnuRemoveSuperSet";
            this.mnuRemoveSuperSet.Click += new System.EventHandler(this.mnuRemoveSuperSet_Click);
            // 
            // btnJoinAsSuperset
            // 
            resources.ApplyResources(this.btnJoinAsSuperset, "btnJoinAsSuperset");
            this.btnJoinAsSuperset.Image = ((System.Drawing.Image)(resources.GetObject("btnJoinAsSuperset.Image")));
            this.btnJoinAsSuperset.Name = "btnJoinAsSuperset";
            this.btnJoinAsSuperset.Click += new System.EventHandler(this.mnuJoinSuperSet_Click);
            // 
            // btnRemoveFromSuperset
            // 
            resources.ApplyResources(this.btnRemoveFromSuperset, "btnRemoveFromSuperset");
            this.btnRemoveFromSuperset.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveFromSuperset.Image")));
            this.btnRemoveFromSuperset.Name = "btnRemoveFromSuperset";
            this.btnRemoveFromSuperset.Click += new System.EventHandler(this.mnuRemoveSuperSet_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            // 
            // exercisesTrainingPlanListView1
            // 
            resources.ApplyResources(this.exercisesTrainingPlanListView1, "exercisesTrainingPlanListView1");
            this.exercisesTrainingPlanListView1.ContextMenuStrip = this.contextMenuStrip1;
            this.exercisesTrainingPlanListView1.FullRowSelect = true;
            this.exercisesTrainingPlanListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.exercisesTrainingPlanListView1.HideSelection = false;
            this.exercisesTrainingPlanListView1.Name = "exercisesTrainingPlanListView1";
            this.exercisesTrainingPlanListView1.UseCompatibleStateImageBehavior = false;
            this.exercisesTrainingPlanListView1.View = System.Windows.Forms.View.Details;
            this.exercisesTrainingPlanListView1.SelectedIndexChanged += new System.EventHandler(this.exercisesTrainingPlanListView1_SelectedIndexChanged);
            // 
            // lblHelp
            // 
            this.lblHelp.AllowHtmlString = true;
            resources.ApplyResources(this.lblHelp, "lblHelp");
            this.lblHelp.Name = "lblHelp";
            // 
            // TrainingPlanSuperSetsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.exercisesTrainingPlanListView1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnJoinAsSuperset);
            this.Controls.Add(this.btnRemoveFromSuperset);
            this.Name = "TrainingPlanSuperSetsEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ExercisesTrainingPlanListView exercisesTrainingPlanListView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuJoinSuperSet;
        private System.Windows.Forms.ToolStripMenuItem mnuRemoveSuperSet;
        private DevExpress.XtraEditors.SimpleButton btnJoinAsSuperset;
        private DevExpress.XtraEditors.SimpleButton btnRemoveFromSuperset;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lblHelp;

    }
}