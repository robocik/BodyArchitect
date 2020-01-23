namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrSerieInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrSerieInfo));
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.lblComment = new DevExpress.XtraEditors.LabelControl();
            this.chkCiezarBezSztangi = new DevExpress.XtraEditors.CheckEdit();
            this.cmbDropSet = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDropSet = new DevExpress.XtraEditors.LabelControl();
            this.cmbSetType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblSetType = new DevExpress.XtraEditors.LabelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCiezarBezSztangi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDropSet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSetType.Properties)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            // 
            // lblComment
            // 
            resources.ApplyResources(this.lblComment, "lblComment");
            this.lblComment.Name = "lblComment";
            // 
            // chkCiezarBezSztangi
            // 
            resources.ApplyResources(this.chkCiezarBezSztangi, "chkCiezarBezSztangi");
            this.chkCiezarBezSztangi.Name = "chkCiezarBezSztangi";
            this.chkCiezarBezSztangi.Properties.AutoWidth = true;
            this.chkCiezarBezSztangi.Properties.Caption = resources.GetString("chkCiezarBezSztangi.Properties.Caption");
            // 
            // cmbDropSet
            // 
            resources.ApplyResources(this.cmbDropSet, "cmbDropSet");
            this.cmbDropSet.Name = "cmbDropSet";
            this.cmbDropSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbDropSet.Properties.Buttons"))))});
            this.cmbDropSet.Properties.Items.AddRange(new object[] {
            resources.GetString("cmbDropSet.Properties.Items"),
            resources.GetString("cmbDropSet.Properties.Items1"),
            resources.GetString("cmbDropSet.Properties.Items2"),
            resources.GetString("cmbDropSet.Properties.Items3"),
            resources.GetString("cmbDropSet.Properties.Items4")});
            this.cmbDropSet.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblDropSet
            // 
            resources.ApplyResources(this.lblDropSet, "lblDropSet");
            this.lblDropSet.Name = "lblDropSet";
            // 
            // cmbSetType
            // 
            resources.ApplyResources(this.cmbSetType, "cmbSetType");
            this.cmbSetType.Name = "cmbSetType";
            this.cmbSetType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbSetType.Properties.Buttons"))))});
            this.cmbSetType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblSetType
            // 
            resources.ApplyResources(this.lblSetType, "lblSetType");
            this.lblSetType.Name = "lblSetType";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkCiezarBezSztangi);
            this.panel1.Controls.Add(this.cmbSetType);
            this.panel1.Controls.Add(this.lblDropSet);
            this.panel1.Controls.Add(this.lblSetType);
            this.panel1.Controls.Add(this.cmbDropSet);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtComment);
            this.panel2.Controls.Add(this.lblComment);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // usrSerieInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrSerieInfo";
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCiezarBezSztangi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDropSet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSetType.Properties)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.MemoEdit txtComment;
        private DevExpress.XtraEditors.LabelControl lblComment;
        private DevExpress.XtraEditors.CheckEdit chkCiezarBezSztangi;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDropSet;
        private DevExpress.XtraEditors.LabelControl lblDropSet;
        private DevExpress.XtraEditors.ComboBoxEdit cmbSetType;
        private DevExpress.XtraEditors.LabelControl lblSetType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}
