namespace BodyArchitect.Controls.UserControls
{
    partial class usrHtmlEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrHtmlEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbBold = new System.Windows.Forms.ToolStripButton();
            this.tbItalic = new System.Windows.Forms.ToolStripButton();
            this.tbUnderline = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbNumbered = new System.Windows.Forms.ToolStripButton();
            this.tbList = new System.Windows.Forms.ToolStripButton();
            this.htmlEditor1 = new BodyArchitect.Controls.Basic.HtmlEditor();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbCut = new System.Windows.Forms.ToolStripButton();
            this.tbCopy = new System.Windows.Forms.ToolStripButton();
            this.tbPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbBold,
            this.tbItalic,
            this.tbUnderline,
            this.toolStripSeparator1,
            this.tbNumbered,
            this.tbList,
            this.toolStripSeparator2,
            this.tbCut,
            this.tbCopy,
            this.tbPaste});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // tbBold
            // 
            this.tbBold.CheckOnClick = true;
            this.tbBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbBold, "tbBold");
            this.tbBold.Name = "tbBold";
            this.tbBold.Click += new System.EventHandler(this.tbBold_Click);
            // 
            // tbItalic
            // 
            this.tbItalic.CheckOnClick = true;
            this.tbItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbItalic, "tbItalic");
            this.tbItalic.Name = "tbItalic";
            this.tbItalic.Click += new System.EventHandler(this.tbItalic_Click);
            // 
            // tbUnderline
            // 
            this.tbUnderline.CheckOnClick = true;
            this.tbUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbUnderline, "tbUnderline");
            this.tbUnderline.Name = "tbUnderline";
            this.tbUnderline.Click += new System.EventHandler(this.tbUnderline_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbNumbered
            // 
            this.tbNumbered.CheckOnClick = true;
            this.tbNumbered.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbNumbered, "tbNumbered");
            this.tbNumbered.Name = "tbNumbered";
            this.tbNumbered.Click += new System.EventHandler(this.tbNumbered_Click);
            // 
            // tbList
            // 
            this.tbList.CheckOnClick = true;
            this.tbList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbList, "tbList");
            this.tbList.Name = "tbList";
            this.tbList.Click += new System.EventHandler(this.tbList_Click);
            // 
            // htmlEditor1
            // 
            this.htmlEditor1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.htmlEditor1, "htmlEditor1");
            this.htmlEditor1.IsContextMenuEnabled = false;
            this.htmlEditor1.Name = "htmlEditor1";
            this.htmlEditor1.UpdateUI += new System.EventHandler(this.updateButtons);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tbCut
            // 
            this.tbCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbCut, "tbCut");
            this.tbCut.Name = "tbCut";
            this.tbCut.Click += new System.EventHandler(this.tbCut_Click);
            // 
            // tbCopy
            // 
            this.tbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbCopy, "tbCopy");
            this.tbCopy.Name = "tbCopy";
            this.tbCopy.Click += new System.EventHandler(this.tbCopy_Click);
            // 
            // tbPaste
            // 
            this.tbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbPaste, "tbPaste");
            this.tbPaste.Name = "tbPaste";
            this.tbPaste.Click += new System.EventHandler(this.tbPaste_Click);
            // 
            // usrHtmlEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.htmlEditor1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "usrHtmlEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Basic.HtmlEditor htmlEditor1;
        private System.Windows.Forms.ToolStripButton tbBold;
        private System.Windows.Forms.ToolStripButton tbItalic;
        private System.Windows.Forms.ToolStripButton tbUnderline;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbNumbered;
        private System.Windows.Forms.ToolStripButton tbList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tbCut;
        private System.Windows.Forms.ToolStripButton tbCopy;
        private System.Windows.Forms.ToolStripButton tbPaste;
    }
}
