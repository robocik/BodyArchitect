namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrTrainingPlanEntryEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrTrainingPlanEntryEditor));
            this.exerciseLookUp1 = new BodyArchitect.Module.StrengthTraining.Controls.ExerciseLookUp();
            this.lblExercise = new DevExpress.XtraEditors.LabelControl();
            this.lblRestTime = new DevExpress.XtraEditors.LabelControl();
            this.lblComment = new DevExpress.XtraEditors.LabelControl();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.txtRestSeconds = new DevExpress.XtraEditors.SpinEdit();
            this.lblSeconds = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestSeconds.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // exerciseLookUp1
            // 
            resources.ApplyResources(this.exerciseLookUp1, "exerciseLookUp1");
            this.exerciseLookUp1.Name = "exerciseLookUp1";
            this.exerciseLookUp1.EditValueChanged += new System.EventHandler(this.exerciseLookUp1_EditValueChanged);
            // 
            // lblExercise
            // 
            resources.ApplyResources(this.lblExercise, "lblExercise");
            this.lblExercise.Name = "lblExercise";
            // 
            // lblRestTime
            // 
            resources.ApplyResources(this.lblRestTime, "lblRestTime");
            this.lblRestTime.Name = "lblRestTime";
            // 
            // lblComment
            // 
            resources.ApplyResources(this.lblComment, "lblComment");
            this.lblComment.Name = "lblComment";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            this.txtComment.Validated += new System.EventHandler(this.txtComment_Validated);
            // 
            // txtRestSeconds
            // 
            resources.ApplyResources(this.txtRestSeconds, "txtRestSeconds");
            this.txtRestSeconds.Name = "txtRestSeconds";
            this.txtRestSeconds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.txtRestSeconds.Validated += new System.EventHandler(this.txtRestTime_Validated);
            // 
            // lblSeconds
            // 
            resources.ApplyResources(this.lblSeconds, "lblSeconds");
            this.lblSeconds.Name = "lblSeconds";
            // 
            // usrTrainingPlanEntryEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblSeconds);
            this.Controls.Add(this.txtRestSeconds);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.lblRestTime);
            this.Controls.Add(this.lblExercise);
            this.Controls.Add(this.exerciseLookUp1);
            this.Name = "usrTrainingPlanEntryEditor";
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestSeconds.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExerciseLookUp exerciseLookUp1;
        private DevExpress.XtraEditors.LabelControl lblExercise;
        private DevExpress.XtraEditors.LabelControl lblRestTime;
        private DevExpress.XtraEditors.LabelControl lblComment;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private DevExpress.XtraEditors.SpinEdit txtRestSeconds;
        private DevExpress.XtraEditors.LabelControl lblSeconds;
    }
}
