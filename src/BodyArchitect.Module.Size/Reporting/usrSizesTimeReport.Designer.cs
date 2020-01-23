namespace BodyArchitect.Module.Size.Reporting
{
    partial class usrSizesTimeReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrSizesTimeReport));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView1 = new System.Windows.Forms.ListView();
            this.usrReportingEntryStatus1 = new BodyArchitect.Controls.Reporting.usrReportingEntryStatus();
            this.usrDateRange1 = new BodyArchitect.Controls.Reporting.usrDateRange();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController();
            this.SuspendLayout();
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            // 
            // usrReportingEntryStatus1
            // 
            resources.ApplyResources(this.usrReportingEntryStatus1, "usrReportingEntryStatus1");
            this.usrReportingEntryStatus1.Name = "usrReportingEntryStatus1";
            // 
            // usrDateRange1
            // 
            resources.ApplyResources(this.usrDateRange1, "usrDateRange1");
            this.usrDateRange1.Name = "usrDateRange1";
            // 
            // usrSizesTimeReport
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrDateRange1);
            this.Controls.Add(this.usrReportingEntryStatus1);
            this.Controls.Add(this.listView1);
            this.Name = "usrSizesTimeReport";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ListView listView1;
        private BodyArchitect.Controls.Reporting.usrReportingEntryStatus usrReportingEntryStatus1;
        private BodyArchitect.Controls.Reporting.usrDateRange usrDateRange1;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
    }
}
