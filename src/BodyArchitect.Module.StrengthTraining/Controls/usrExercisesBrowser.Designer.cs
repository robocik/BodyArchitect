namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrExercisesBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrExercisesBrowser));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.cmbTypes = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.lblCountry = new DevExpress.XtraEditors.LabelControl();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.btnMoreResults = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRating = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.mnuShowUserDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.usrExerciseEditor1 = new BodyArchitect.Module.StrengthTraining.Controls.usrExerciseEditor();
            this.lblSelectExerciseMessage = new DevExpress.XtraEditors.LabelControl();
            this.lblSortOrder = new DevExpress.XtraEditors.LabelControl();
            this.cmbSortOrder = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTypes.Properties)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbSortOrder.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrExerciseEditor1);
            this.splitContainerControl1.Panel2.Controls.Add(this.lblSelectExerciseMessage);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 364;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.baGroupControl1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblDescription, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.Controls.Add(this.cmbSortOrder);
            this.baGroupControl1.Controls.Add(this.lblSortOrder);
            this.baGroupControl1.Controls.Add(this.lblStatus);
            this.baGroupControl1.Controls.Add(this.cmbTypes);
            this.baGroupControl1.Controls.Add(this.lblCountry);
            this.baGroupControl1.Controls.Add(this.progressIndicator1);
            this.baGroupControl1.Controls.Add(this.btnMoreResults);
            this.baGroupControl1.Controls.Add(this.btnSearch);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // cmbTypes
            // 
            resources.ApplyResources(this.cmbTypes, "cmbTypes");
            this.cmbTypes.Name = "cmbTypes";
            this.cmbTypes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbTypes.Properties.Buttons"))))});
            this.cmbTypes.Properties.NullValuePrompt = resources.GetString("cmbTypes.Properties.NullValuePrompt");
            this.cmbTypes.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cmbTypes.Properties.NullValuePromptShowForEmptyValue")));
            this.cmbTypes.Properties.SelectAllItemCaption = resources.GetString("cmbTypes.Properties.SelectAllItemCaption");
            // 
            // lblCountry
            // 
            resources.ApplyResources(this.lblCountry, "lblCountry");
            this.lblCountry.Name = "lblCountry";
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // btnMoreResults
            // 
            resources.ApplyResources(this.btnMoreResults, "btnMoreResults");
            this.btnMoreResults.Image = ((System.Drawing.Image)(resources.GetObject("btnMoreResults.Image")));
            this.btnMoreResults.Name = "btnMoreResults";
            this.btnMoreResults.Click += new System.EventHandler(this.btnMoreResults_Click);
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colRating,
            this.colUser});
            this.tableLayoutPanel2.SetColumnSpan(this.listView1, 2);
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // colType
            // 
            resources.ApplyResources(this.colType, "colType");
            // 
            // colRating
            // 
            resources.ApplyResources(this.colRating, "colRating");
            // 
            // colUser
            // 
            resources.ApplyResources(this.colUser, "colUser");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShowUserDetails});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // mnuShowUserDetails
            // 
            resources.ApplyResources(this.mnuShowUserDetails, "mnuShowUserDetails");
            this.mnuShowUserDetails.Name = "mnuShowUserDetails";
            this.mnuShowUserDetails.Click += new System.EventHandler(this.mnuShowUserDetails_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AllowHtmlString = true;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // usrExerciseEditor1
            // 
            resources.ApplyResources(this.usrExerciseEditor1, "usrExerciseEditor1");
            this.usrExerciseEditor1.Name = "usrExerciseEditor1";
            // 
            // lblSelectExerciseMessage
            // 
            this.lblSelectExerciseMessage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelectExerciseMessage.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            resources.ApplyResources(this.lblSelectExerciseMessage, "lblSelectExerciseMessage");
            this.lblSelectExerciseMessage.Name = "lblSelectExerciseMessage";
            // 
            // lblSortOrder
            // 
            resources.ApplyResources(this.lblSortOrder, "lblSortOrder");
            this.lblSortOrder.Name = "lblSortOrder";
            // 
            // cmbSortOrder
            // 
            resources.ApplyResources(this.cmbSortOrder, "cmbSortOrder");
            this.cmbSortOrder.Name = "cmbSortOrder";
            this.cmbSortOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("comboBoxEdit1.Properties.Buttons"))))});
            this.cmbSortOrder.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // usrExercisesBrowser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrExercisesBrowser";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.baGroupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTypes.Properties)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbSortOrder.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private usrExerciseEditor usrExerciseEditor1;
        private DevExpress.XtraEditors.LabelControl lblSelectExerciseMessage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cmbTypes;
        private DevExpress.XtraEditors.LabelControl lblCountry;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.SimpleButton btnMoreResults;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colRating;
        private System.Windows.Forms.ColumnHeader colUser;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuShowUserDetails;
        private DevExpress.XtraEditors.ComboBoxEdit cmbSortOrder;
        private DevExpress.XtraEditors.LabelControl lblSortOrder;
    }
}
