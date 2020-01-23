namespace BodyArchitect.Controls.UserControls
{
    partial class usrCalendarOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrCalendarOptions));
            this.cmbTextFrom = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.chkShowRelativeDates = new DevExpress.XtraEditors.CheckEdit();
            this.lblShowIconsFor = new DevExpress.XtraEditors.LabelControl();
            this.lblTextFrom = new DevExpress.XtraEditors.LabelControl();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.pluginsGrid1 = new BodyArchitect.Controls.UserControls.PluginsGrid();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTextFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowRelativeDates.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbTextFrom
            // 
            resources.ApplyResources(this.cmbTextFrom, "cmbTextFrom");
            this.cmbTextFrom.Name = "cmbTextFrom";
            this.cmbTextFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbTextFrom.Properties.Buttons"))))});
            // 
            // chkShowRelativeDates
            // 
            resources.ApplyResources(this.chkShowRelativeDates, "chkShowRelativeDates");
            this.chkShowRelativeDates.Name = "chkShowRelativeDates";
            this.chkShowRelativeDates.Properties.AutoWidth = true;
            this.chkShowRelativeDates.Properties.Caption = resources.GetString("chkShowMissingPlugins.Properties.Caption");
            // 
            // lblShowIconsFor
            // 
            resources.ApplyResources(this.lblShowIconsFor, "lblShowIconsFor");
            this.lblShowIconsFor.Name = "lblShowIconsFor";
            // 
            // lblTextFrom
            // 
            resources.ApplyResources(this.lblTextFrom, "lblTextFrom");
            this.lblTextFrom.Name = "lblTextFrom";
            // 
            // pluginsGrid1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.pluginsGrid1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("pluginsGrid1.AllowHtmlText"))));
            resources.ApplyResources(this.pluginsGrid1, "pluginsGrid1");
            this.pluginsGrid1.ColumnsCount = 3;
            this.pluginsGrid1.EnableSort = true;
            this.pluginsGrid1.Name = "pluginsGrid1";
            this.pluginsGrid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.pluginsGrid1.RowsCount = 1;
            this.pluginsGrid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.pluginsGrid1.TabStop = true;
            this.pluginsGrid1.ToolTipText = "";
            // 
            // usrCalendarOptions
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pluginsGrid1);
            this.Controls.Add(this.cmbTextFrom);
            this.Controls.Add(this.chkShowRelativeDates);
            this.Controls.Add(this.lblShowIconsFor);
            this.Controls.Add(this.lblTextFrom);
            this.Name = "usrCalendarOptions";
            ((System.ComponentModel.ISupportInitialize)(this.cmbTextFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowRelativeDates.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ImageComboBoxEdit cmbTextFrom;
        private DevExpress.XtraEditors.CheckEdit chkShowRelativeDates;
        private DevExpress.XtraEditors.LabelControl lblShowIconsFor;
        private DevExpress.XtraEditors.LabelControl lblTextFrom;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private PluginsGrid pluginsGrid1;
    }
}
