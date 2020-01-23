namespace BodyArchitect.Controls.UserControls
{
    partial class usrMessageView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrMessageView));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessageTopic = new DevExpress.XtraEditors.LabelControl();
            this.txtMessageContent = new DevExpress.XtraEditors.MemoEdit();
            this.lblMessageContent = new DevExpress.XtraEditors.LabelControl();
            this.txtMessageTopic = new DevExpress.XtraEditors.TextEdit();
            this.lblPriority = new DevExpress.XtraEditors.LabelControl();
            this.imageComboBoxEdit1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.grUser = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.usrProfileListEntry1 = new BodyArchitect.Controls.UserControls.usrProfileListEntry();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDeleteMessage = new DevExpress.XtraEditors.SimpleButton();
            this.btnReply = new DevExpress.XtraEditors.SimpleButton();
            this.validationProvider1 = new Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessageContent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessageTopic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grUser)).BeginInit();
            this.grUser.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.baGroupControl1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grUser, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.lblMessageTopic, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtMessageContent, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblMessageContent, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtMessageTopic, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblPriority, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.imageComboBoxEdit1, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // lblMessageTopic
            // 
            resources.ApplyResources(this.lblMessageTopic, "lblMessageTopic");
            this.lblMessageTopic.Name = "lblMessageTopic";
            // 
            // txtMessageContent
            // 
            resources.ApplyResources(this.txtMessageContent, "txtMessageContent");
            this.txtMessageContent.Name = "txtMessageContent";
            this.validationProvider1.SetPerformValidation(this.txtMessageContent, true);
            this.txtMessageContent.Properties.AppearanceReadOnly.BackColor = ((System.Drawing.Color)(resources.GetObject("txtMessageContent.Properties.AppearanceReadOnly.BackColor")));
            this.txtMessageContent.Properties.AppearanceReadOnly.BackColor2 = ((System.Drawing.Color)(resources.GetObject("txtMessageContent.Properties.AppearanceReadOnly.BackColor2")));
            this.txtMessageContent.Properties.AppearanceReadOnly.Options.UseBackColor = true;
            this.validationProvider1.SetSourcePropertyName(this.txtMessageContent, "Content");
            this.txtMessageContent.EditValueChanged += new System.EventHandler(this.txtMessageTopic_EditValueChanged);
            // 
            // lblMessageContent
            // 
            resources.ApplyResources(this.lblMessageContent, "lblMessageContent");
            this.lblMessageContent.Name = "lblMessageContent";
            // 
            // txtMessageTopic
            // 
            resources.ApplyResources(this.txtMessageTopic, "txtMessageTopic");
            this.txtMessageTopic.Name = "txtMessageTopic";
            this.validationProvider1.SetPerformValidation(this.txtMessageTopic, true);
            this.txtMessageTopic.Properties.AppearanceReadOnly.BackColor = ((System.Drawing.Color)(resources.GetObject("txtMessageTopic.Properties.AppearanceReadOnly.BackColor")));
            this.txtMessageTopic.Properties.AppearanceReadOnly.BackColor2 = ((System.Drawing.Color)(resources.GetObject("txtMessageTopic.Properties.AppearanceReadOnly.BackColor2")));
            this.txtMessageTopic.Properties.AppearanceReadOnly.Options.UseBackColor = true;
            this.validationProvider1.SetSourcePropertyName(this.txtMessageTopic, "Topic");
            this.txtMessageTopic.EditValueChanged += new System.EventHandler(this.txtMessageTopic_EditValueChanged);
            // 
            // lblPriority
            // 
            resources.ApplyResources(this.lblPriority, "lblPriority");
            this.lblPriority.Name = "lblPriority";
            // 
            // imageComboBoxEdit1
            // 
            resources.ApplyResources(this.imageComboBoxEdit1, "imageComboBoxEdit1");
            this.imageComboBoxEdit1.Name = "imageComboBoxEdit1";
            this.imageComboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("imageComboBoxEdit1.Properties.Buttons"))))});
            this.imageComboBoxEdit1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("imageComboBoxEdit1.Properties.Items"), resources.GetString("imageComboBoxEdit1.Properties.Items1"), ((int)(resources.GetObject("imageComboBoxEdit1.Properties.Items2")))),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("imageComboBoxEdit1.Properties.Items3"), resources.GetString("imageComboBoxEdit1.Properties.Items4"), ((int)(resources.GetObject("imageComboBoxEdit1.Properties.Items5")))),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("imageComboBoxEdit1.Properties.Items6"), resources.GetString("imageComboBoxEdit1.Properties.Items7"), ((int)(resources.GetObject("imageComboBoxEdit1.Properties.Items8")))),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem(resources.GetString("imageComboBoxEdit1.Properties.Items9"), resources.GetString("imageComboBoxEdit1.Properties.Items10"), ((int)(resources.GetObject("imageComboBoxEdit1.Properties.Items11"))))});
            this.imageComboBoxEdit1.Properties.SmallImages = this.imageList1;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Normal");
            this.imageList1.Images.SetKeyName(1, "Low");
            this.imageList1.Images.SetKeyName(2, "High");
            this.imageList1.Images.SetKeyName(3, "System");
            // 
            // grUser
            // 
            this.grUser.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grUser.AppearanceCaption.BackColor")));
            this.grUser.AppearanceCaption.Options.UseBackColor = true;
            this.grUser.AppearanceCaption.Options.UseTextOptions = true;
            this.grUser.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grUser.Controls.Add(this.usrProfileListEntry1);
            resources.ApplyResources(this.grUser, "grUser");
            this.grUser.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grUser.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grUser.Name = "grUser";
            // 
            // usrProfileListEntry1
            // 
            resources.ApplyResources(this.usrProfileListEntry1, "usrProfileListEntry1");
            this.usrProfileListEntry1.Name = "usrProfileListEntry1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnDeleteMessage);
            this.flowLayoutPanel1.Controls.Add(this.btnReply);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // btnDeleteMessage
            // 
            resources.ApplyResources(this.btnDeleteMessage, "btnDeleteMessage");
            this.btnDeleteMessage.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteMessage.Image")));
            this.btnDeleteMessage.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDeleteMessage.Name = "btnDeleteMessage";
            resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1");
            toolTipItem1.LeftIndent = 6;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.btnDeleteMessage.SuperTip = superToolTip1;
            this.btnDeleteMessage.Click += new System.EventHandler(this.btnDeleteMessage_Click);
            // 
            // btnReply
            // 
            resources.ApplyResources(this.btnReply, "btnReply");
            this.btnReply.Image = ((System.Drawing.Image)(resources.GetObject("btnReply.Image")));
            this.btnReply.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnReply.Name = "btnReply";
            resources.ApplyResources(toolTipTitleItem2, "toolTipTitleItem2");
            toolTipItem2.LeftIndent = 6;
            resources.ApplyResources(toolTipItem2, "toolTipItem2");
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this.btnReply.SuperTip = superToolTip2;
            this.btnReply.Click += new System.EventHandler(this.btnReply_Click);
            // 
            // validationProvider1
            // 
            this.validationProvider1.ErrorProvider = null;
            this.validationProvider1.RulesetName = global::BodyArchitect.Controls.Localization.DomainModelStrings.AchievementRank_Rank1;
            this.validationProvider1.SourceTypeName = "BodyArchitect.Service.Model.MessageDTO, BodyArchitect.Service.Model";
            this.validationProvider1.ValidationPerformed += new System.EventHandler<Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs>(this.validationProvider1_ValidationPerformed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // usrMessageView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrMessageView";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessageContent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessageTopic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grUser)).EndInit();
            this.grUser.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.MemoEdit txtMessageContent;
        private DevExpress.XtraEditors.LabelControl lblMessageContent;
        private DevExpress.XtraEditors.TextEdit txtMessageTopic;
        private DevExpress.XtraEditors.LabelControl lblMessageTopic;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private BaGroupControl grUser;
        private DevExpress.XtraEditors.SimpleButton btnDeleteMessage;
        private usrProfileListEntry usrProfileListEntry1;
        private Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider validationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.LabelControl lblPriority;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnReply;

    }
}
