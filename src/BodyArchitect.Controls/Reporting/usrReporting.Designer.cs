namespace BodyArchitect.Controls.Reporting
{
    partial class usrReporting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrReporting));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.splitContainer1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tcmbReportList = new System.Windows.Forms.ToolStripComboBox();
            this.tbReportSettings = new System.Windows.Forms.ToolStripButton();
            this.tbSaveReport = new System.Windows.Forms.ToolStripButton();
            this.tbZoomReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbGenerateReport = new System.Windows.Forms.ToolStripButton();
            this.tbCancelGenerateReport = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.chart1);
            this.splitContainer1.SplitterPosition = 399;
            this.splitContainer1.SplitGroupPanelCollapsed += new DevExpress.XtraEditors.SplitGroupPanelCollapsedEventHandler(this.splitContainer1_SplitGroupPanelCollapsed);
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Empty;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "Default";
            this.chart1.ChartAreas.Add(chartArea1);
            resources.ApplyResources(this.chart1, "chart1");
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Name = "chart1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tcmbReportList,
            this.tbReportSettings,
            this.tbSaveReport,
            this.tbZoomReset,
            this.toolStripSeparator1,
            this.tbGenerateReport,
            this.tbCancelGenerateReport});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // tcmbReportList
            // 
            this.tcmbReportList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tcmbReportList.Name = "tcmbReportList";
            resources.ApplyResources(this.tcmbReportList, "tcmbReportList");
            this.tcmbReportList.SelectedIndexChanged += new System.EventHandler(this.tcmbReportList_SelectedIndexChanged);
            // 
            // tbReportSettings
            // 
            this.tbReportSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbReportSettings, "tbReportSettings");
            this.tbReportSettings.Name = "tbReportSettings";
            this.tbReportSettings.Click += new System.EventHandler(this.tbReportSettings_Click);
            // 
            // tbSaveReport
            // 
            this.tbSaveReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbSaveReport, "tbSaveReport");
            this.tbSaveReport.Name = "tbSaveReport";
            this.tbSaveReport.Click += new System.EventHandler(this.tbSaveReport_Click);
            // 
            // tbZoomReset
            // 
            this.tbZoomReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbZoomReset, "tbZoomReset");
            this.tbZoomReset.Name = "tbZoomReset";
            this.tbZoomReset.Click += new System.EventHandler(this.tbZoomReset_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbGenerateReport
            // 
            this.tbGenerateReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbGenerateReport, "tbGenerateReport");
            this.tbGenerateReport.Name = "tbGenerateReport";
            this.tbGenerateReport.Click += new System.EventHandler(this.tbGenerateReport_Click);
            // 
            // tbCancelGenerateReport
            // 
            this.tbCancelGenerateReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbCancelGenerateReport, "tbCancelGenerateReport");
            this.tbCancelGenerateReport.Name = "tbCancelGenerateReport";
            this.tbCancelGenerateReport.Click += new System.EventHandler(this.tbCancelGenerateReport_Click);
            // 
            // usrReporting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "usrReporting";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox tcmbReportList;
        private System.Windows.Forms.ToolStripButton tbReportSettings;
        private System.Windows.Forms.ToolStripButton tbSaveReport;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ToolStripButton tbGenerateReport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbZoomReset;
        private System.Windows.Forms.ToolStripButton tbCancelGenerateReport;

    }
}
