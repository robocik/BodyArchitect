namespace BodyArchitect.Controls.UserControls
{
    partial class usrMyTrainingStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrMyTrainingStatus));
            this.grTrainingInfo = new BaGroupControl();
            this.lblPercentageResult = new DevExpress.XtraEditors.LabelControl();
            this.dtaEndDate = new DevExpress.XtraEditors.DateEdit();
            this.lblEndDate = new DevExpress.XtraEditors.LabelControl();
            this.dtaStartDate = new DevExpress.XtraEditors.DateEdit();
            this.lblStartDate = new DevExpress.XtraEditors.LabelControl();
            this.lblTrainingState = new DevExpress.XtraEditors.LabelControl();
            this.btnAbortMyTraining = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grTrainingInfo)).BeginInit();
            this.grTrainingInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEndDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEndDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaStartDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaStartDate.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grTrainingInfo
            // 
            this.grTrainingInfo.Controls.Add(this.lblPercentageResult);
            this.grTrainingInfo.Controls.Add(this.dtaEndDate);
            this.grTrainingInfo.Controls.Add(this.lblEndDate);
            this.grTrainingInfo.Controls.Add(this.dtaStartDate);
            this.grTrainingInfo.Controls.Add(this.lblStartDate);
            this.grTrainingInfo.Controls.Add(this.lblTrainingState);
            this.grTrainingInfo.Controls.Add(this.btnAbortMyTraining);
            resources.ApplyResources(this.grTrainingInfo, "grTrainingInfo");
            this.grTrainingInfo.Name = "grTrainingInfo";
            // 
            // lblPercentageResult
            // 
            resources.ApplyResources(this.lblPercentageResult, "lblPercentageResult");
            this.lblPercentageResult.Name = "lblPercentageResult";
            // 
            // dtaEndDate
            // 
            resources.ApplyResources(this.dtaEndDate, "dtaEndDate");
            this.dtaEndDate.Name = "dtaEndDate";
            this.dtaEndDate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtaEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaEndDate.Properties.Buttons"))))});
            this.dtaEndDate.Properties.ReadOnly = true;
            this.dtaEndDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblEndDate
            // 
            resources.ApplyResources(this.lblEndDate, "lblEndDate");
            this.lblEndDate.Name = "lblEndDate";
            // 
            // dtaStartDate
            // 
            resources.ApplyResources(this.dtaStartDate, "dtaStartDate");
            this.dtaStartDate.Name = "dtaStartDate";
            this.dtaStartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaStartDate.Properties.Buttons"))))});
            this.dtaStartDate.Properties.ReadOnly = true;
            this.dtaStartDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblStartDate
            // 
            resources.ApplyResources(this.lblStartDate, "lblStartDate");
            this.lblStartDate.Name = "lblStartDate";
            // 
            // lblTrainingState
            // 
            resources.ApplyResources(this.lblTrainingState, "lblTrainingState");
            this.lblTrainingState.Name = "lblTrainingState";
            // 
            // btnAbortMyTraining
            // 
            resources.ApplyResources(this.btnAbortMyTraining, "btnAbortMyTraining");
            this.btnAbortMyTraining.Name = "btnAbortMyTraining";
            this.btnAbortMyTraining.Click += new System.EventHandler(this.btnAbortMyTraining_Click);
            // 
            // usrMyTrainingStatus
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grTrainingInfo);
            this.Name = "usrMyTrainingStatus";
            ((System.ComponentModel.ISupportInitialize)(this.grTrainingInfo)).EndInit();
            this.grTrainingInfo.ResumeLayout(false);
            this.grTrainingInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEndDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaEndDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaStartDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaStartDate.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grTrainingInfo;
        private DevExpress.XtraEditors.LabelControl lblTrainingState;
        private DevExpress.XtraEditors.SimpleButton btnAbortMyTraining;
        private DevExpress.XtraEditors.DateEdit dtaEndDate;
        private DevExpress.XtraEditors.LabelControl lblEndDate;
        private DevExpress.XtraEditors.DateEdit dtaStartDate;
        private DevExpress.XtraEditors.LabelControl lblStartDate;
        private DevExpress.XtraEditors.LabelControl lblPercentageResult;
    }
}
