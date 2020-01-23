namespace BodyArchitect.Controls.UserControls
{
    partial class usrReportStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrReportStatus));
            this.grReportStatus = new BaGroupControl();
            this.cmbReportStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.grReportStatus)).BeginInit();
            this.grReportStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbReportStatus.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grReportStatus
            // 
            resources.ApplyResources(this.grReportStatus, "grReportStatus");
            this.grReportStatus.Controls.Add(this.cmbReportStatus);
            this.grReportStatus.Name = "grReportStatus";
            // 
            // cmbReportStatus
            // 
            resources.ApplyResources(this.cmbReportStatus, "cmbReportStatus");
            this.cmbReportStatus.Name = "cmbReportStatus";
            this.cmbReportStatus.Properties.AccessibleDescription = resources.GetString("cmbReportStatus.Properties.AccessibleDescription");
            this.cmbReportStatus.Properties.AccessibleName = resources.GetString("cmbReportStatus.Properties.AccessibleName");
            this.cmbReportStatus.Properties.AutoHeight = ((bool)(resources.GetObject("cmbReportStatus.Properties.AutoHeight")));
            this.cmbReportStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbReportStatus.Properties.Buttons"))))});
            this.cmbReportStatus.Properties.NullValuePrompt = resources.GetString("cmbReportStatus.Properties.NullValuePrompt");
            this.cmbReportStatus.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cmbReportStatus.Properties.NullValuePromptShowForEmptyValue")));
            this.cmbReportStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // usrReportStatus
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grReportStatus);
            this.Name = "usrReportStatus";
            ((System.ComponentModel.ISupportInitialize)(this.grReportStatus)).EndInit();
            this.grReportStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbReportStatus.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grReportStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cmbReportStatus;
    }
}
