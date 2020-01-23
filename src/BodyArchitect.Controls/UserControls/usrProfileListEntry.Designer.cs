namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfileListEntry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfileListEntry));
            this.lblUserName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblCreatedDate = new DevExpress.XtraEditors.LabelControl();
            this.lblCreatedDateValue = new DevExpress.XtraEditors.LabelControl();
            this.lblCountry = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUserName.ForeColor = System.Drawing.Color.Blue;
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.UseMnemonic = false;
            this.lblUserName.Click += new System.EventHandler(this.lblUserName_Click);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblGender
            // 
            resources.ApplyResources(this.lblGender, "lblGender");
            this.lblGender.Name = "lblGender";
            this.lblGender.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblCreatedDate
            // 
            resources.ApplyResources(this.lblCreatedDate, "lblCreatedDate");
            this.lblCreatedDate.Name = "lblCreatedDate";
            this.lblCreatedDate.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblCreatedDateValue
            // 
            resources.ApplyResources(this.lblCreatedDateValue, "lblCreatedDateValue");
            this.lblCreatedDateValue.Name = "lblCreatedDateValue";
            this.lblCreatedDateValue.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblCountry
            // 
            resources.ApplyResources(this.lblCountry, "lblCountry");
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // usrProfileListEntry
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCountry);
            this.Controls.Add(this.lblCreatedDateValue);
            this.Controls.Add(this.lblCreatedDate);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.pictureBox1);
            this.Name = "usrProfileListEntry";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblCreatedDate;
        private DevExpress.XtraEditors.LabelControl lblCreatedDateValue;
        private DevExpress.XtraEditors.LabelControl lblCountry;
    }
}
