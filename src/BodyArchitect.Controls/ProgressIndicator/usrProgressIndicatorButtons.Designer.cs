namespace BodyArchitect.Controls.ProgressIndicator
{
    partial class usrProgressIndicatorButtons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProgressIndicatorButtons));
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.okButton1 = new BodyArchitect.Controls.Basic.OKButton();
            this.SuspendLayout();
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // cancelButton1
            // 
            this.cancelButton1.CausesValidation = false;
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.Name = "cancelButton1";
            this.cancelButton1.Click += new System.EventHandler(this.cancelButton1_Click);
            // 
            // okButton1
            // 
            this.okButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton1.Image = ((System.Drawing.Image)(resources.GetObject("okButton1.Image")));
            resources.ApplyResources(this.okButton1, "okButton1");
            this.okButton1.Name = "okButton1";
            this.okButton1.Click += new System.EventHandler(this.okButton1_Click);
            // 
            // usrProgressIndicatorButtons
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.progressIndicator1);
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.okButton1);
            this.Name = "usrProgressIndicatorButtons";
            this.Load += new System.EventHandler(this.usrProgressIndicatorButtons_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.ProgressIndicator progressIndicator1;
        private Basic.CancelButton cancelButton1;
        private Basic.OKButton okButton1;
    }
}
