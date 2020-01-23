using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.StrengthTraining.Reporting
{
    partial class usrExerciseWeightReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrExerciseWeightReport));
            this.splitContainer1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.grSets = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.chkIncludeSetsWithoutBarrelWeight = new DevExpress.XtraEditors.CheckEdit();
            this.grRepetitionRange = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtRepetitionsTo = new DevExpress.XtraEditors.SpinEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.txtRepetitionFrom = new DevExpress.XtraEditors.SpinEdit();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.usrDateRange1 = new BodyArchitect.Controls.Reporting.usrDateRange();
            this.usrReportingEntryStatus1 = new BodyArchitect.Controls.Reporting.usrReportingEntryStatus();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.cmbSetTypes = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grSets)).BeginInit();
            this.grSets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeSetsWithoutBarrelWeight.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grRepetitionRange)).BeginInit();
            this.grRepetitionRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionsTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSetTypes.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.splitContainer1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("splitContainer1.AllowHtmlText"))));
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainer1.Horizontal = false;
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.SplitterPosition = 270;
            // 
            // listView1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.listView1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("listView1.AllowHtmlText"))));
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // panel1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.panel1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("panel1.AllowHtmlText"))));
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.grSets);
            this.panel1.Controls.Add(this.grRepetitionRange);
            this.panel1.Controls.Add(this.usrDateRange1);
            this.panel1.Controls.Add(this.usrReportingEntryStatus1);
            this.panel1.Name = "panel1";
            // 
            // grSets
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.grSets, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("grSets.AllowHtmlText"))));
            resources.ApplyResources(this.grSets, "grSets");
            this.grSets.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grSets.AppearanceCaption.BackColor")));
            this.grSets.AppearanceCaption.Options.UseBackColor = true;
            this.grSets.Controls.Add(this.cmbSetTypes);
            this.grSets.Controls.Add(this.chkIncludeSetsWithoutBarrelWeight);
            this.grSets.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grSets.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grSets.Name = "grSets";
            // 
            // chkIncludeSetsWithoutBarrelWeight
            // 
            resources.ApplyResources(this.chkIncludeSetsWithoutBarrelWeight, "chkIncludeSetsWithoutBarrelWeight");
            this.chkIncludeSetsWithoutBarrelWeight.Name = "chkIncludeSetsWithoutBarrelWeight";
            this.chkIncludeSetsWithoutBarrelWeight.Properties.AutoWidth = true;
            this.chkIncludeSetsWithoutBarrelWeight.Properties.Caption = resources.GetString("chkIncludeSetsWithoutBarrelWeight.Properties.Caption");
            // 
            // grRepetitionRange
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.grRepetitionRange, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("grRepetitionRange.AllowHtmlText"))));
            resources.ApplyResources(this.grRepetitionRange, "grRepetitionRange");
            this.grRepetitionRange.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grRepetitionRange.AppearanceCaption.BackColor")));
            this.grRepetitionRange.AppearanceCaption.Options.UseBackColor = true;
            this.grRepetitionRange.Controls.Add(this.txtRepetitionsTo);
            this.grRepetitionRange.Controls.Add(this.lblTo);
            this.grRepetitionRange.Controls.Add(this.txtRepetitionFrom);
            this.grRepetitionRange.Controls.Add(this.lblFrom);
            this.grRepetitionRange.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grRepetitionRange.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grRepetitionRange.Name = "grRepetitionRange";
            // 
            // txtRepetitionsTo
            // 
            resources.ApplyResources(this.txtRepetitionsTo, "txtRepetitionsTo");
            this.txtRepetitionsTo.Name = "txtRepetitionsTo";
            this.txtRepetitionsTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblTo
            // 
            resources.ApplyResources(this.lblTo, "lblTo");
            this.lblTo.Name = "lblTo";
            // 
            // txtRepetitionFrom
            // 
            resources.ApplyResources(this.txtRepetitionFrom, "txtRepetitionFrom");
            this.txtRepetitionFrom.Name = "txtRepetitionFrom";
            this.txtRepetitionFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblFrom
            // 
            resources.ApplyResources(this.lblFrom, "lblFrom");
            this.lblFrom.Name = "lblFrom";
            // 
            // usrDateRange1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrDateRange1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrDateRange1.AllowHtmlText"))));
            resources.ApplyResources(this.usrDateRange1, "usrDateRange1");
            this.usrDateRange1.Name = "usrDateRange1";
            // 
            // usrReportingEntryStatus1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrReportingEntryStatus1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrReportingEntryStatus1.AllowHtmlText"))));
            resources.ApplyResources(this.usrReportingEntryStatus1, "usrReportingEntryStatus1");
            this.usrReportingEntryStatus1.Name = "usrReportingEntryStatus1";
            // 
            // cmbSetTypes
            // 
            resources.ApplyResources(this.cmbSetTypes, "cmbSetTypes");
            this.cmbSetTypes.Name = "cmbSetTypes";
            this.cmbSetTypes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("checkedComboBoxEdit1.Properties.Buttons"))))});
            // 
            // usrExerciseWeightReport
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "usrExerciseWeightReport";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grSets)).EndInit();
            this.grSets.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeSetsWithoutBarrelWeight.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grRepetitionRange)).EndInit();
            this.grRepetitionRange.ResumeLayout(false);
            this.grRepetitionRange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionsTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSetTypes.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colName;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private BaGroupControl grSets;
        private DevExpress.XtraEditors.CheckEdit chkIncludeSetsWithoutBarrelWeight;
        private BaGroupControl grRepetitionRange;
        private DevExpress.XtraEditors.SpinEdit txtRepetitionsTo;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.SpinEdit txtRepetitionFrom;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private BodyArchitect.Controls.Reporting.usrDateRange usrDateRange1;
        private BodyArchitect.Controls.Reporting.usrReportingEntryStatus usrReportingEntryStatus1;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cmbSetTypes;
    }
}
