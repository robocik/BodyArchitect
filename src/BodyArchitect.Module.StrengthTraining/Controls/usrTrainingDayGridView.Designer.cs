using BodyArchitect.Module.StrengthTraining.Controls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrTrainingDayGridView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrTrainingDayGridView));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dodajInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tslSerieCount = new System.Windows.Forms.ToolStripLabel();
            this.tcmbSeries = new System.Windows.Forms.ToolStripComboBox();
            this.tslExerciseDisplayProperty = new System.Windows.Forms.ToolStripLabel();
            this.cmbExerciseColumns = new System.Windows.Forms.ToolStripComboBox();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colExercise = new BodyArchitect.Module.StrengthTraining.Controls.DataGridViewLookUpEditColumn();
            this.colComment = new BodyArchitect.Module.StrengthTraining.Controls.DataGridViewExTextColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
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
            // toolStrip1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.toolStrip1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("toolStrip1.AllowHtmlText"))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslSerieCount,
            this.tcmbSeries,
            this.tslExerciseDisplayProperty,
            this.cmbExerciseColumns});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // tslSerieCount
            // 
            this.tslSerieCount.Name = "tslSerieCount";
            resources.ApplyResources(this.tslSerieCount, "tslSerieCount");
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
            this.tcmbSeries.SelectedIndexChanged += new System.EventHandler(this.tcmbSeries_SelectedIndexChanged);
            // 
            // tslExerciseDisplayProperty
            // 
            this.tslExerciseDisplayProperty.Name = "tslExerciseDisplayProperty";
            resources.ApplyResources(this.tslExerciseDisplayProperty, "tslExerciseDisplayProperty");
            // 
            // cmbExerciseColumns
            // 
            this.cmbExerciseColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExerciseColumns.Name = "cmbExerciseColumns";
            resources.ApplyResources(this.cmbExerciseColumns, "cmbExerciseColumns");
            this.cmbExerciseColumns.SelectedIndexChanged += new System.EventHandler(this.cmbExerciseColumns_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.dataGridView1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("dataGridView1.AllowHtmlText"))));
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colExercise,
            this.colComment});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserAddedRow);
            this.dataGridView1.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView1_UserDeletedRow);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            this.dataGridView1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dataGridView1_PreviewKeyDown);
            // 
            // colExercise
            // 
            this.colExercise.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colExercise, "colExercise");
            this.colExercise.Name = "colExercise";
            this.colExercise.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colComment
            // 
            resources.ApplyResources(this.colComment, "colComment");
            this.colComment.Name = "colComment";
            this.colComment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // usrTrainingDayGridView
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "usrTrainingDayGridView";
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox tcmbSeries;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dodajInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel tslSerieCount;
        private System.Windows.Forms.ToolStripLabel tslExerciseDisplayProperty;
        private System.Windows.Forms.ToolStripComboBox cmbExerciseColumns;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private DataGridViewLookUpEditColumn colExercise;
        private DataGridViewExTextColumn colComment;

    }
}
