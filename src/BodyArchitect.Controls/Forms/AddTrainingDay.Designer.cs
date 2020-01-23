namespace BodyArchitect.Controls.Forms
{
    partial class AddTrainingDay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTrainingDay));
            this.btnApply = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new BodyArchitect.Controls.Basic.CancelButton();
            this.btnOK = new BodyArchitect.Controls.Basic.OKButton();
            this.usrAddTrainingDay1 = new BodyArchitect.Controls.UserControls.usrAddTrainingDay();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblShowTutorial = new DevExpress.XtraEditors.LabelControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Name = "btnCancel";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // usrAddTrainingDay1
            // 
            resources.ApplyResources(this.usrAddTrainingDay1, "usrAddTrainingDay1");
            this.usrAddTrainingDay1.Name = "usrAddTrainingDay1";
            // 
            // progressIndicator1
            // 
            this.progressIndicator1.AutoStart = true;
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.btnApply);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.progressIndicator1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblShowTutorial
            // 
            resources.ApplyResources(this.lblShowTutorial, "lblShowTutorial");
            this.lblShowTutorial.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblShowTutorial.Appearance.ForeColor")));
            this.lblShowTutorial.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblShowTutorial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblShowTutorial.Name = "lblShowTutorial";
            this.lblShowTutorial.Click += new System.EventHandler(this.lblShowTutorial_Click);
            // 
            // AddTrainingDay
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblShowTutorial);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.usrAddTrainingDay1);
            this.Name = "AddTrainingDay";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddTrainingDay_FormClosing);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BodyArchitect.Controls.UserControls.usrAddTrainingDay usrAddTrainingDay1;
        private BodyArchitect.Controls.Basic.OKButton btnOK;
        private BodyArchitect.Controls.Basic.CancelButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnApply;
        private UserControls.ProgressIndicator progressIndicator1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.LabelControl lblShowTutorial;
    }
}