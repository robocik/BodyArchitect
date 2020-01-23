using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.A6W.Controls
{
    partial class usrA6W
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrA6W));
            this.lvA6W = new System.Windows.Forms.ListView();
            this.colDay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSetNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRepetitionNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblWWW = new DevExpress.XtraEditors.LabelControl();
            this.usrA6WPartialCompleted1 = new BodyArchitect.Module.A6W.Controls.usrA6WPartialCompleted();
            this.rbPartialCompleted = new DevExpress.XtraEditors.CheckEdit();
            this.rbCompleted = new DevExpress.XtraEditors.CheckEdit();
            this.grComment = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.usrReportStatus1 = new BodyArchitect.Controls.UserControls.usrReportStatus();
            this.usrMyTrainingStatus1 = new BodyArchitect.Controls.UserControls.usrMyTrainingStatus();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbPartialCompleted.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCompleted.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grComment)).BeginInit();
            this.grComment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvA6W
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.lvA6W, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lvA6W.AllowHtmlText"))));
            resources.ApplyResources(this.lvA6W, "lvA6W");
            this.lvA6W.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDay,
            this.colSetNumber,
            this.colRepetitionNumber});
            this.lvA6W.FullRowSelect = true;
            this.lvA6W.GridLines = true;
            this.lvA6W.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvA6W.MultiSelect = false;
            this.lvA6W.Name = "lvA6W";
            this.lvA6W.SmallImageList = this.imageList1;
            this.lvA6W.UseCompatibleStateImageBehavior = false;
            this.lvA6W.View = System.Windows.Forms.View.Details;
            // 
            // colDay
            // 
            resources.ApplyResources(this.colDay, "colDay");
            // 
            // colSetNumber
            // 
            resources.ApplyResources(this.colSetNumber, "colSetNumber");
            // 
            // colRepetitionNumber
            // 
            resources.ApplyResources(this.colRepetitionNumber, "colRepetitionNumber");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Completed");
            this.imageList1.Images.SetKeyName(1, "PartialCompleted");
            // 
            // groupControl1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.groupControl1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("groupControl1.AllowHtmlText"))));
            this.groupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("groupControl1.AppearanceCaption.BackColor")));
            this.groupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.groupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.groupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.groupControl1.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupControl1, "groupControl1");
            this.groupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.groupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.groupControl1.Name = "groupControl1";
            // 
            // panel1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.panel1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("panel1.AllowHtmlText"))));
            this.panel1.Controls.Add(this.lblWWW);
            this.panel1.Controls.Add(this.usrA6WPartialCompleted1);
            this.panel1.Controls.Add(this.rbPartialCompleted);
            this.panel1.Controls.Add(this.rbCompleted);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lblWWW
            // 
            resources.ApplyResources(this.lblWWW, "lblWWW");
            this.lblWWW.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblWWW.Appearance.ForeColor")));
            this.lblWWW.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWWW.Name = "lblWWW";
            this.lblWWW.Click += new System.EventHandler(this.lblWWW_Click);
            // 
            // usrA6WPartialCompleted1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrA6WPartialCompleted1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrA6WPartialCompleted1.AllowHtmlText"))));
            resources.ApplyResources(this.usrA6WPartialCompleted1, "usrA6WPartialCompleted1");
            this.usrA6WPartialCompleted1.Name = "usrA6WPartialCompleted1";
            // 
            // rbPartialCompleted
            // 
            resources.ApplyResources(this.rbPartialCompleted, "rbPartialCompleted");
            this.rbPartialCompleted.Name = "rbPartialCompleted";
            this.rbPartialCompleted.Properties.AutoWidth = true;
            this.rbPartialCompleted.Properties.Caption = resources.GetString("rbPartialCompleted.Properties.Caption");
            this.rbPartialCompleted.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbPartialCompleted.Properties.RadioGroupIndex = 1;
            this.rbPartialCompleted.TabStop = false;
            this.rbPartialCompleted.CheckedChanged += new System.EventHandler(this.rbPartialCompleted_CheckedChanged);
            // 
            // rbCompleted
            // 
            resources.ApplyResources(this.rbCompleted, "rbCompleted");
            this.rbCompleted.Name = "rbCompleted";
            this.rbCompleted.Properties.AutoWidth = true;
            this.rbCompleted.Properties.Caption = resources.GetString("rbCompleted.Properties.Caption");
            this.rbCompleted.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbCompleted.Properties.RadioGroupIndex = 1;
            this.rbCompleted.TabStop = false;
            this.rbCompleted.CheckedChanged += new System.EventHandler(this.rbPartialCompleted_CheckedChanged);
            // 
            // grComment
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.grComment, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("grComment.AllowHtmlText"))));
            this.grComment.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grComment.AppearanceCaption.BackColor")));
            this.grComment.AppearanceCaption.Options.UseBackColor = true;
            this.grComment.AppearanceCaption.Options.UseTextOptions = true;
            this.grComment.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grComment.Controls.Add(this.txtComment);
            resources.ApplyResources(this.grComment, "grComment");
            this.grComment.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grComment.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grComment.Name = "grComment";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            // 
            // defaultToolTipController1
            // 
            // 
            // 
            // 
            this.defaultToolTipController1.DefaultController.Active = false;
            // 
            // usrReportStatus1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrReportStatus1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrReportStatus1.AllowHtmlText"))));
            resources.ApplyResources(this.usrReportStatus1, "usrReportStatus1");
            this.usrReportStatus1.Name = "usrReportStatus1";
            this.usrReportStatus1.ReadOnly = false;
            // 
            // usrMyTrainingStatus1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrMyTrainingStatus1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrMyTrainingStatus1.AllowHtmlText"))));
            resources.ApplyResources(this.usrMyTrainingStatus1, "usrMyTrainingStatus1");
            this.usrMyTrainingStatus1.Name = "usrMyTrainingStatus1";
            this.usrMyTrainingStatus1.ReadOnly = false;
            // 
            // tableLayoutPanel1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.tableLayoutPanel1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("tableLayoutPanel1.AllowHtmlText"))));
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.usrMyTrainingStatus1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.usrReportStatus1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupControl1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grComment, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // usrA6W
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lvA6W);
            this.Name = "usrA6W";
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rbPartialCompleted.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbCompleted.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grComment)).EndInit();
            this.grComment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvA6W;
        private System.Windows.Forms.ColumnHeader colDay;
        private System.Windows.Forms.ColumnHeader colSetNumber;
        private System.Windows.Forms.ColumnHeader colRepetitionNumber;
        private System.Windows.Forms.ImageList imageList1;
        private BaGroupControl groupControl1;
        private BaGroupControl grComment;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private BodyArchitect.Controls.UserControls.usrReportStatus usrReportStatus1;
        private BodyArchitect.Controls.UserControls.usrMyTrainingStatus usrMyTrainingStatus1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl lblWWW;
        private usrA6WPartialCompleted usrA6WPartialCompleted1;
        private DevExpress.XtraEditors.CheckEdit rbPartialCompleted;
        private DevExpress.XtraEditors.CheckEdit rbCompleted;
    }
}
