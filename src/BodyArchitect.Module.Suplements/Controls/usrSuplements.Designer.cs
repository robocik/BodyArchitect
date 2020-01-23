namespace BodyArchitect.Module.Suplements.Controls
{
    partial class usrSuplements
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrSuplements));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.usrReportStatus1 = new BodyArchitect.Controls.UserControls.usrReportStatus();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.suplementsGrid1 = new BodyArchitect.Module.Suplements.Controls.SuplementsGrid();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.usrReportStatus1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.baGroupControl1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // usrReportStatus1
            // 
            resources.ApplyResources(this.usrReportStatus1, "usrReportStatus1");
            this.usrReportStatus1.Name = "usrReportStatus1";
            this.usrReportStatus1.ReadOnly = false;
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.suplementsGrid1);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // suplementsGrid1
            // 
            this.suplementsGrid1.ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut) 
            | SourceGrid.ClipboardMode.Paste) 
            | SourceGrid.ClipboardMode.Delete)));
            this.suplementsGrid1.ClipboardUseOnlyActivePosition = true;
            resources.ApplyResources(this.suplementsGrid1, "suplementsGrid1");
            this.suplementsGrid1.EnableSort = true;
            this.suplementsGrid1.Name = "suplementsGrid1";
            this.suplementsGrid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.suplementsGrid1.ReadOnly = false;
            this.suplementsGrid1.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.suplementsGrid1.TabStop = true;
            this.suplementsGrid1.ToolTipText = "";
            // 
            // usrSuplements
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrSuplements";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private BodyArchitect.Controls.UserControls.usrReportStatus usrReportStatus1;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private SuplementsGrid suplementsGrid1;

    }
}
