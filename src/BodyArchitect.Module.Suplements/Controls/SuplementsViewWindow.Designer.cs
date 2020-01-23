namespace BodyArchitect.Module.Suplements.Controls
{
    partial class SuplementsViewWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuplementsViewWindow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lvSuplements = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.tableLayoutPanel1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("tableLayoutPanel1.AllowHtmlText"))));
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lvSuplements, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelControl1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lvSuplements
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.lvSuplements, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lvSuplements.AllowHtmlText"))));
            this.lvSuplements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            resources.ApplyResources(this.lvSuplements, "lvSuplements");
            this.lvSuplements.FullRowSelect = true;
            this.lvSuplements.GridLines = true;
            this.lvSuplements.Name = "lvSuplements";
            this.lvSuplements.UseCompatibleStateImageBehavior = false;
            this.lvSuplements.View = System.Windows.Forms.View.Details;
            this.lvSuplements.SelectedIndexChanged += new System.EventHandler(this.lvSuplements_SelectedIndexChanged);
            this.lvSuplements.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvSuplements_KeyUp);
            this.lvSuplements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvSuplements_MouseDoubleClick);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // panelControl1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.panelControl1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("panelControl1.AllowHtmlText"))));
            this.panelControl1.Controls.Add(this.btnDelete);
            this.panelControl1.Controls.Add(this.btnEdit);
            this.panelControl1.Controls.Add(this.btnAdd);
            resources.ApplyResources(this.panelControl1, "panelControl1");
            this.panelControl1.Name = "panelControl1";
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // SuplementsViewWindow
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SuplementsViewWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListView lvSuplements;
        private System.Windows.Forms.ColumnHeader colName;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
    }
}