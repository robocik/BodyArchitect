namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrTrainingPlansView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrTrainingPlansView));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tcPlans = new DevExpress.XtraTab.XtraTabControl();
            this.tpPersnalPlans = new DevExpress.XtraTab.XtraTabPage();
            this.workoutPlansListView1 = new BodyArchitect.Module.StrengthTraining.Controls.WorkoutPlansListView();
            this.tpPlansBrowser = new DevExpress.XtraTab.XtraTabPage();
            this.usrWorkoutPlansSearch1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutPlansSearch();
            this.usrWorkoutCommentsList1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutCommentsList();
            this.lblSelectWorkoutPlanFirst = new DevExpress.XtraEditors.LabelControl();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.tbView = new System.Windows.Forms.ToolStripButton();
            this.tbClone = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbNew = new System.Windows.Forms.ToolStripButton();
            this.tbEdit = new System.Windows.Forms.ToolStripButton();
            this.tbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbPublish = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbShowComments = new System.Windows.Forms.ToolStripButton();
            this.tbAddToFavorites = new System.Windows.Forms.ToolStripButton();
            this.tbRemoveFromFavorites = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcPlans)).BeginInit();
            this.tcPlans.SuspendLayout();
            this.tpPersnalPlans.SuspendLayout();
            this.tpPlansBrowser.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.splitContainerControl1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("splitContainerControl1.AllowHtmlText"))));
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.tcPlans);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrWorkoutCommentsList1);
            this.splitContainerControl1.Panel2.Controls.Add(this.lblSelectWorkoutPlanFirst);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 362;
            // 
            // tcPlans
            // 
            resources.ApplyResources(this.tcPlans, "tcPlans");
            this.tcPlans.Name = "tcPlans";
            this.tcPlans.SelectedTabPage = this.tpPersnalPlans;
            this.tcPlans.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpPersnalPlans,
            this.tpPlansBrowser});
            this.tcPlans.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tcPlans_SelectedPageChanged);
            // 
            // tpPersnalPlans
            // 
            this.tpPersnalPlans.Controls.Add(this.workoutPlansListView1);
            this.tpPersnalPlans.Name = "tpPersnalPlans";
            resources.ApplyResources(this.tpPersnalPlans, "tpPersnalPlans");
            // 
            // workoutPlansListView1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.workoutPlansListView1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("workoutPlansListView1.AllowHtmlText"))));
            resources.ApplyResources(this.workoutPlansListView1, "workoutPlansListView1");
            this.workoutPlansListView1.FullRowSelect = true;
            this.workoutPlansListView1.GridLines = true;
            this.workoutPlansListView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups1"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups2"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups3"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups4"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups5"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups6"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups7"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups8"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups9"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups10"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups11"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups12"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups13"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups14"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups15"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups16"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("workoutPlansListView1.Groups17")))});
            this.workoutPlansListView1.HideSelection = false;
            this.workoutPlansListView1.MultiSelect = false;
            this.workoutPlansListView1.Name = "workoutPlansListView1";
            this.workoutPlansListView1.UseCompatibleStateImageBehavior = false;
            this.workoutPlansListView1.View = System.Windows.Forms.View.Details;
            this.workoutPlansListView1.SelectedIndexChanged += new System.EventHandler(this.workoutPlansListView1_SelectedIndexChanged);
            this.workoutPlansListView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.workoutPlansListView1_KeyUp);
            // 
            // tpPlansBrowser
            // 
            this.tpPlansBrowser.Controls.Add(this.usrWorkoutPlansSearch1);
            this.tpPlansBrowser.Name = "tpPlansBrowser";
            resources.ApplyResources(this.tpPlansBrowser, "tpPlansBrowser");
            // 
            // usrWorkoutPlansSearch1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrWorkoutPlansSearch1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrWorkoutPlansSearch1.AllowHtmlText"))));
            resources.ApplyResources(this.usrWorkoutPlansSearch1, "usrWorkoutPlansSearch1");
            this.usrWorkoutPlansSearch1.Name = "usrWorkoutPlansSearch1";
            this.usrWorkoutPlansSearch1.SelectedPlanChanged += new System.EventHandler(this.workoutPlansListView1_SelectedIndexChanged);
            this.usrWorkoutPlansSearch1.DeletePlanRequest += new System.EventHandler<BodyArchitect.Module.StrengthTraining.Controls.WorkoutPlanEventArgs>(this.usrWorkoutPlansSearch1_DeletePlanRequest);
            // 
            // usrWorkoutCommentsList1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.usrWorkoutCommentsList1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("usrWorkoutCommentsList1.AllowHtmlText"))));
            resources.ApplyResources(this.usrWorkoutCommentsList1, "usrWorkoutCommentsList1");
            this.usrWorkoutCommentsList1.Name = "usrWorkoutCommentsList1";
            // 
            // lblSelectWorkoutPlanFirst
            // 
            this.lblSelectWorkoutPlanFirst.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSelectWorkoutPlanFirst.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblSelectWorkoutPlanFirst.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.lblSelectWorkoutPlanFirst, "lblSelectWorkoutPlanFirst");
            this.lblSelectWorkoutPlanFirst.Name = "lblSelectWorkoutPlanFirst";
            // 
            // defaultToolTipController1
            // 
            // 
            // 
            // 
            this.defaultToolTipController1.DefaultController.Active = false;
            // 
            // toolbar
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.toolbar, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("toolbar.AllowHtmlText"))));
            this.toolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbView,
            this.tbClone,
            this.toolStripSeparator1,
            this.tbNew,
            this.tbEdit,
            this.tbDelete,
            this.toolStripSeparator2,
            this.tbPublish,
            this.toolStripSeparator3,
            this.tbShowComments,
            this.tbAddToFavorites,
            this.tbRemoveFromFavorites});
            resources.ApplyResources(this.toolbar, "toolbar");
            this.toolbar.Name = "toolbar";
            // 
            // tbView
            // 
            this.tbView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbView, "tbView");
            this.tbView.Name = "tbView";
            this.tbView.Click += new System.EventHandler(this.btnPrintPreview_Click);
            // 
            // tbClone
            // 
            this.tbClone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbClone, "tbClone");
            this.tbClone.Name = "tbClone";
            this.tbClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbNew
            // 
            this.tbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbNew, "tbNew");
            this.tbNew.Name = "tbNew";
            this.tbNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // tbEdit
            // 
            this.tbEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbEdit, "tbEdit");
            this.tbEdit.Name = "tbEdit";
            this.tbEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // tbDelete
            // 
            this.tbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbDelete, "tbDelete");
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tbPublish
            // 
            this.tbPublish.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbPublish, "tbPublish");
            this.tbPublish.Name = "tbPublish";
            this.tbPublish.Click += new System.EventHandler(this.tbPublish_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // tbShowComments
            // 
            this.tbShowComments.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbShowComments, "tbShowComments");
            this.tbShowComments.Name = "tbShowComments";
            this.tbShowComments.Click += new System.EventHandler(this.tbShowComments_Click);
            // 
            // tbAddToFavorites
            // 
            this.tbAddToFavorites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbAddToFavorites, "tbAddToFavorites");
            this.tbAddToFavorites.Name = "tbAddToFavorites";
            this.tbAddToFavorites.Click += new System.EventHandler(this.tbAddToFavorites_Click);
            // 
            // tbRemoveFromFavorites
            // 
            this.tbRemoveFromFavorites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbRemoveFromFavorites, "tbRemoveFromFavorites");
            this.tbRemoveFromFavorites.Name = "tbRemoveFromFavorites";
            this.tbRemoveFromFavorites.Click += new System.EventHandler(this.tbRemoveFromFavorites_Click);
            // 
            // usrTrainingPlansView
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.toolbar);
            this.Name = "usrTrainingPlansView";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tcPlans)).EndInit();
            this.tcPlans.ResumeLayout(false);
            this.tpPersnalPlans.ResumeLayout(false);
            this.tpPlansBrowser.ResumeLayout(false);
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripButton tbView;
        private System.Windows.Forms.ToolStripButton tbClone;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbNew;
        private System.Windows.Forms.ToolStripButton tbEdit;
        private System.Windows.Forms.ToolStripButton tbDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbPublish;
        private System.Windows.Forms.ToolStripButton tbShowComments;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private usrWorkoutCommentsList usrWorkoutCommentsList1;
        private DevExpress.XtraEditors.LabelControl lblSelectWorkoutPlanFirst;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private DevExpress.XtraTab.XtraTabControl tcPlans;
        private DevExpress.XtraTab.XtraTabPage tpPersnalPlans;
        private WorkoutPlansListView workoutPlansListView1;
        private DevExpress.XtraTab.XtraTabPage tpPlansBrowser;
        private usrWorkoutPlansSearch usrWorkoutPlansSearch1;
        private System.Windows.Forms.ToolStripButton tbAddToFavorites;
        private System.Windows.Forms.ToolStripButton tbRemoveFromFavorites;
    }
}
