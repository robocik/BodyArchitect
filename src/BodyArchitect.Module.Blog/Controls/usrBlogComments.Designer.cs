namespace BodyArchitect.Module.Blog.Controls
{
    partial class usrBlogComments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrBlogComments));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.blogCommentsList1 = new BodyArchitect.Module.Blog.Controls.WPF.BlogCommentsList();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.baGroupControl1);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 130;
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.CaptionImage = ((System.Drawing.Image)(resources.GetObject("baGroupControl1.CaptionImage")));
            this.baGroupControl1.Controls.Add(this.elementHost1);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // elementHost1
            // 
            resources.ApplyResources(this.elementHost1, "elementHost1");
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Child = this.blogCommentsList1;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtComment, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressIndicator1);
            this.panel1.Controls.Add(this.btnSend);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.Image = ((System.Drawing.Image)(resources.GetObject("btnSend.Image")));
            this.btnSend.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSend.Name = "btnSend";
            resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1");
            toolTipItem1.LeftIndent = 6;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.btnSend.SuperTip = superToolTip1;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            superToolTip2.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            resources.ApplyResources(toolTipTitleItem2, "toolTipTitleItem2");
            toolTipItem2.LeftIndent = 6;
            resources.ApplyResources(toolTipItem2, "toolTipItem2");
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this.txtComment.SuperTip = superToolTip2;
            this.txtComment.EditValueChanged += new System.EventHandler(this.txtComment_EditValueChanged);
            // 
            // usrBlogComments
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrBlogComments";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.SimpleButton btnSend;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WPF.BlogCommentsList blogCommentsList1;
    }
}
