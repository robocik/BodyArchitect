namespace BodyArchitect.Module.Blog.Controls
{
    partial class usrBlog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrBlog));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grAllowComments = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.cmbAllowComments = new DevExpress.XtraEditors.ComboBoxEdit();
            this.usrHtmlEditor1 = new BodyArchitect.Controls.UserControls.usrHtmlEditor();
            this.usrBlogComments1 = new BodyArchitect.Module.Blog.Controls.usrBlogComments();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grAllowComments)).BeginInit();
            this.grAllowComments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbAllowComments.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrBlogComments1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 483;
            this.splitContainerControl1.SplitGroupPanelCollapsing += new DevExpress.XtraEditors.SplitGroupPanelCollapsingEventHandler(this.splitContainerControl1_SplitGroupPanelCollapsing);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.grAllowComments, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.usrHtmlEditor1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // grAllowComments
            // 
            this.grAllowComments.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grAllowComments.AppearanceCaption.BackColor")));
            this.grAllowComments.AppearanceCaption.Options.UseBackColor = true;
            this.grAllowComments.AppearanceCaption.Options.UseTextOptions = true;
            this.grAllowComments.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grAllowComments.Controls.Add(this.cmbAllowComments);
            resources.ApplyResources(this.grAllowComments, "grAllowComments");
            this.grAllowComments.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grAllowComments.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grAllowComments.Name = "grAllowComments";
            // 
            // cmbAllowComments
            // 
            resources.ApplyResources(this.cmbAllowComments, "cmbAllowComments");
            this.cmbAllowComments.Name = "cmbAllowComments";
            this.cmbAllowComments.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbAllowComments.Properties.Buttons"))))});
            this.cmbAllowComments.Properties.Items.AddRange(new object[] {
            resources.GetString("cmbAllowComments.Properties.Items"),
            resources.GetString("cmbAllowComments.Properties.Items1")});
            this.cmbAllowComments.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            superToolTip1.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipItem1);
            this.cmbAllowComments.SuperTip = superToolTip1;
            // 
            // usrHtmlEditor1
            // 
            resources.ApplyResources(this.usrHtmlEditor1, "usrHtmlEditor1");
            this.usrHtmlEditor1.Name = "usrHtmlEditor1";
            this.usrHtmlEditor1.ReadOnly = true;
            // 
            // usrBlogComments1
            // 
            resources.ApplyResources(this.usrBlogComments1, "usrBlogComments1");
            this.usrBlogComments1.Name = "usrBlogComments1";
            // 
            // usrBlog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrBlog";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grAllowComments)).EndInit();
            this.grAllowComments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbAllowComments.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private BodyArchitect.Controls.UserControls.BaGroupControl grAllowComments;
        private DevExpress.XtraEditors.ComboBoxEdit cmbAllowComments;
        private BodyArchitect.Controls.UserControls.usrHtmlEditor usrHtmlEditor1;
        private usrBlogComments usrBlogComments1;
    }
}
