namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class SerieInfoWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerieInfoWindow));
            this.usrSerieInfo1 = new BodyArchitect.Module.StrengthTraining.Controls.usrSerieInfo();
            this.okButton1 = new BodyArchitect.Controls.Basic.OKButton();
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.SuspendLayout();
            // 
            // usrSerieInfo1
            // 
            resources.ApplyResources(this.usrSerieInfo1, "usrSerieInfo1");
            this.usrSerieInfo1.Name = "usrSerieInfo1";
            // 
            // okButton1
            // 
            this.okButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton1.Image = ((System.Drawing.Image)(resources.GetObject("okButton1.Image")));
            resources.ApplyResources(this.okButton1, "okButton1");
            this.okButton1.Name = "okButton1";
            this.okButton1.Click += new System.EventHandler(this.okButton1_Click);
            // 
            // cancelButton1
            // 
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.Name = "cancelButton1";
            // 
            // SerieInfoWindow
            // 
            this.AcceptButton = this.okButton1;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton1;
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.okButton1);
            this.Controls.Add(this.usrSerieInfo1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SerieInfoWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private usrSerieInfo usrSerieInfo1;
        private BodyArchitect.Controls.Basic.OKButton okButton1;
        private BodyArchitect.Controls.Basic.CancelButton cancelButton1;
    }
}