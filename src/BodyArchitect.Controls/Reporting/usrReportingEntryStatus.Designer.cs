using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Reporting
{
    partial class usrReportingEntryStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrReportingEntryStatus));
            this.grEntryReportStatus = new BaGroupControl();
            this.cmbEntryReportStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.grEntryReportStatus)).BeginInit();
            this.grEntryReportStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbEntryReportStatus.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grEntryReportStatus
            // 
            this.grEntryReportStatus.Controls.Add(this.cmbEntryReportStatus);
            resources.ApplyResources(this.grEntryReportStatus, "grEntryReportStatus");
            this.grEntryReportStatus.Name = "grEntryReportStatus";
            // 
            // cmbEntryReportStatus
            // 
            resources.ApplyResources(this.cmbEntryReportStatus, "cmbEntryReportStatus");
            this.cmbEntryReportStatus.Name = "cmbEntryReportStatus";
            this.cmbEntryReportStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbEntryReportStatus.Properties.Buttons"))))});
            this.cmbEntryReportStatus.Properties.Items.AddRange(new object[] {
            resources.GetString("cmbEntryReportStatus.Properties.Items"),
            resources.GetString("cmbEntryReportStatus.Properties.Items1")});
            this.cmbEntryReportStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // usrReportingEntryStatus
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grEntryReportStatus);
            this.Name = "usrReportingEntryStatus";
            ((System.ComponentModel.ISupportInitialize)(this.grEntryReportStatus)).EndInit();
            this.grEntryReportStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbEntryReportStatus.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grEntryReportStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cmbEntryReportStatus;
    }
}
