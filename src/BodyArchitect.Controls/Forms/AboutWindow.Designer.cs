using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Forms
{
    partial class AboutWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblBetaVersion = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new BodyArchitect.Controls.Basic.CancelButton();
            this.grInformation = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblVersionValue = new DevExpress.XtraEditors.LabelControl();
            this.lblWWW = new DevExpress.XtraEditors.LabelControl();
            this.lblVersion = new DevExpress.XtraEditors.LabelControl();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInformation)).BeginInit();
            this.grInformation.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.labelControl3);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.lblBetaVersion);
            this.panel3.Controls.Add(this.labelControl2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.labelControl1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("labelControl3.Appearance.BackColor")));
            this.labelControl3.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl3.Appearance.Font")));
            this.labelControl3.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl3.Appearance.ForeColor")));
            resources.ApplyResources(this.labelControl3, "labelControl3");
            this.labelControl3.Name = "labelControl3";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lblBetaVersion
            // 
            this.lblBetaVersion.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblBetaVersion.Appearance.Font")));
            this.lblBetaVersion.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblBetaVersion.Appearance.ForeColor")));
            resources.ApplyResources(this.lblBetaVersion, "lblBetaVersion");
            this.lblBetaVersion.Name = "lblBetaVersion";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl2.Appearance.Font")));
            this.labelControl2.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl2.Appearance.ForeColor")));
            resources.ApplyResources(this.labelControl2, "labelControl2");
            this.labelControl2.Name = "labelControl2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label2.Name = "label2";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl1.Appearance.Font")));
            this.labelControl1.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl1.Appearance.ForeColor")));
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Name = "btnCancel";
            // 
            // grInformation
            // 
            resources.ApplyResources(this.grInformation, "grInformation");
            this.grInformation.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grInformation.AppearanceCaption.BackColor")));
            this.grInformation.AppearanceCaption.Options.UseBackColor = true;
            this.grInformation.AppearanceCaption.Options.UseTextOptions = true;
            this.grInformation.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grInformation.Controls.Add(this.label3);
            this.grInformation.Controls.Add(this.label4);
            this.grInformation.Controls.Add(this.flowLayoutPanel1);
            this.grInformation.Controls.Add(this.lblWWW);
            this.grInformation.Controls.Add(this.lblVersion);
            this.grInformation.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grInformation.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grInformation.Name = "grInformation";
            this.grInformation.ShowCaption = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Name = "label4";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lblVersionValue);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblVersionValue
            // 
            this.lblVersionValue.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblVersionValue.Appearance.Font")));
            resources.ApplyResources(this.lblVersionValue, "lblVersionValue");
            this.lblVersionValue.Name = "lblVersionValue";
            // 
            // lblWWW
            // 
            resources.ApplyResources(this.lblWWW, "lblWWW");
            this.lblWWW.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblWWW.Appearance.ForeColor")));
            this.lblWWW.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWWW.Name = "lblWWW";
            this.lblWWW.Click += new System.EventHandler(this.lblWWW_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblVersion.Appearance.Font")));
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.Name = "lblVersion";
            // 
            // AboutWindow
            // 
            this.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("AboutWindow.Appearance.BackColor")));
            this.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.grInformation);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.ShowInTaskbar = false;
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInformation)).EndInit();
            this.grInformation.ResumeLayout(false);
            this.grInformation.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grInformation;
        private DevExpress.XtraEditors.LabelControl lblWWW;
        private BodyArchitect.Controls.Basic.CancelButton btnCancel;
        private DevExpress.XtraEditors.LabelControl lblBetaVersion;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl lblVersionValue;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraEditors.LabelControl lblVersion;
    }
}