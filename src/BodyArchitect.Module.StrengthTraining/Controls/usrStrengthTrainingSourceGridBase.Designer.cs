namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrStrengthTrainingSourceGridBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrStrengthTrainingSourceGridBase));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tcmbSeries = new System.Windows.Forms.ToolStripComboBox();
            this.cmbExerciseColumns = new System.Windows.Forms.ToolStripComboBox();
            this.tslSerieCount = new System.Windows.Forms.ToolStripLabel();
            this.tslExerciseDisplayProperty = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbShowPlan = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbMoveUp = new System.Windows.Forms.ToolStripButton();
            this.tbMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbDelete = new System.Windows.Forms.ToolStripButton();
            this.tbSupersets = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbAddSuperSet = new System.Windows.Forms.ToolStripMenuItem();
            this.tbRemoveSuperSet = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dodajInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grid1 = new SourceGrid.Grid();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcmbSeries
            // 
            this.tcmbSeries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tcmbSeries.Items.AddRange(new object[] {
            resources.GetString("tcmbSeries.Items"),
            resources.GetString("tcmbSeries.Items1"),
            resources.GetString("tcmbSeries.Items2"),
            resources.GetString("tcmbSeries.Items3"),
            resources.GetString("tcmbSeries.Items4"),
            resources.GetString("tcmbSeries.Items5"),
            resources.GetString("tcmbSeries.Items6"),
            resources.GetString("tcmbSeries.Items7"),
            resources.GetString("tcmbSeries.Items8"),
            resources.GetString("tcmbSeries.Items9"),
            resources.GetString("tcmbSeries.Items10"),
            resources.GetString("tcmbSeries.Items11"),
            resources.GetString("tcmbSeries.Items12"),
            resources.GetString("tcmbSeries.Items13"),
            resources.GetString("tcmbSeries.Items14")});
            this.tcmbSeries.Name = "tcmbSeries";
            resources.ApplyResources(this.tcmbSeries, "tcmbSeries");
            // 
            // cmbExerciseColumns
            // 
            this.cmbExerciseColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExerciseColumns.Name = "cmbExerciseColumns";
            resources.ApplyResources(this.cmbExerciseColumns, "cmbExerciseColumns");
            // 
            // tslSerieCount
            // 
            this.tslSerieCount.Name = "tslSerieCount";
            resources.ApplyResources(this.tslSerieCount, "tslSerieCount");
            // 
            // tslExerciseDisplayProperty
            // 
            this.tslExerciseDisplayProperty.Name = "tslExerciseDisplayProperty";
            resources.ApplyResources(this.tslExerciseDisplayProperty, "tslExerciseDisplayProperty");
            // 
            // toolStrip1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.toolStrip1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("toolStrip1.AllowHtmlText"))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSerieCount,
            this.tcmbSeries,
            this.tslExerciseDisplayProperty,
            this.cmbExerciseColumns,
            this.tbShowPlan,
            this.toolStripSeparator1,
            this.tbMoveUp,
            this.tbMoveDown,
            this.toolStripSeparator2,
            this.tbDelete,
            this.tbSupersets});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // tbShowPlan
            // 
            this.tbShowPlan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbShowPlan, "tbShowPlan");
            this.tbShowPlan.Name = "tbShowPlan";
            this.tbShowPlan.Click += new System.EventHandler(this.tbShowPlan_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbMoveUp
            // 
            this.tbMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbMoveUp, "tbMoveUp");
            this.tbMoveUp.Name = "tbMoveUp";
            this.tbMoveUp.Click += new System.EventHandler(this.tbMoveUp_Click);
            // 
            // tbMoveDown
            // 
            this.tbMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbMoveDown, "tbMoveDown");
            this.tbMoveDown.Name = "tbMoveDown";
            this.tbMoveDown.Click += new System.EventHandler(this.tbMoveDown_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tbDelete
            // 
            this.tbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbDelete, "tbDelete");
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Click += new System.EventHandler(this.tbDelete_Click);
            // 
            // tbSupersets
            // 
            this.tbSupersets.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSupersets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbAddSuperSet,
            this.tbRemoveSuperSet});
            resources.ApplyResources(this.tbSupersets, "tbSupersets");
            this.tbSupersets.Name = "tbSupersets";
            // 
            // tbAddSuperSet
            // 
            resources.ApplyResources(this.tbAddSuperSet, "tbAddSuperSet");
            this.tbAddSuperSet.Name = "tbAddSuperSet";
            this.tbAddSuperSet.Click += new System.EventHandler(this.tbAddSuperSet_Click);
            // 
            // tbRemoveSuperSet
            // 
            resources.ApplyResources(this.tbRemoveSuperSet, "tbRemoveSuperSet");
            this.tbRemoveSuperSet.Name = "tbRemoveSuperSet";
            this.tbRemoveSuperSet.Click += new System.EventHandler(this.tbRemoveSuperSet_Click);
            // 
            // contextMenuStrip1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.contextMenuStrip1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("contextMenuStrip1.AllowHtmlText"))));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dodajInfoToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // dodajInfoToolStripMenuItem
            // 
            this.dodajInfoToolStripMenuItem.Name = "dodajInfoToolStripMenuItem";
            resources.ApplyResources(this.dodajInfoToolStripMenuItem, "dodajInfoToolStripMenuItem");
            this.dodajInfoToolStripMenuItem.Click += new System.EventHandler(this.dodajInfoToolStripMenuItem_Click);
            // 
            // grid1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.grid1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("grid1.AllowHtmlText"))));
            resources.ApplyResources(this.grid1, "grid1");
            this.grid1.ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut) 
            | SourceGrid.ClipboardMode.Paste) 
            | SourceGrid.ClipboardMode.Delete)));
            this.grid1.ClipboardUseOnlyActivePosition = true;
            this.grid1.EnableSort = false;
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            this.grid1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.grid1_PreviewKeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // usrStrengthTrainingSourceGridBase
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.grid1);
            this.Name = "usrStrengthTrainingSourceGridBase";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripComboBox tcmbSeries;
        private System.Windows.Forms.ToolStripComboBox cmbExerciseColumns;
        private System.Windows.Forms.ToolStripLabel tslSerieCount;
        private System.Windows.Forms.ToolStripLabel tslExerciseDisplayProperty;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dodajInfoToolStripMenuItem;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.ToolStripButton tbShowPlan;
        private System.Windows.Forms.ToolStripButton tbMoveUp;
        private System.Windows.Forms.ToolStripButton tbMoveDown;
        private System.Windows.Forms.ToolStripButton tbDelete;
        private System.Windows.Forms.ToolStripDropDownButton tbSupersets;
        private System.Windows.Forms.ToolStripMenuItem tbAddSuperSet;
        private System.Windows.Forms.ToolStripMenuItem tbRemoveSuperSet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
    }
}
