namespace BodyArchitect.Controls.Forms
{
    partial class OptionsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsWindow));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.okButton1 = new BodyArchitect.Controls.Basic.OKButton();
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.btnResetAllOptions = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
            this.xtraTabControl1.Name = "xtraTabControl1";
            // 
            // okButton1
            // 
            resources.ApplyResources(this.okButton1, "okButton1");
            this.okButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton1.Image = ((System.Drawing.Image)(resources.GetObject("okButton1.Image")));
            this.okButton1.Name = "okButton1";
            this.okButton1.Click += new System.EventHandler(this.okButton1_Click);
            // 
            // cancelButton1
            // 
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.CausesValidation = false;
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            this.cancelButton1.Name = "cancelButton1";
            // 
            // btnResetAllOptions
            // 
            resources.ApplyResources(this.btnResetAllOptions, "btnResetAllOptions");
            this.btnResetAllOptions.Name = "btnResetAllOptions";
            this.btnResetAllOptions.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // OptionsWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.btnResetAllOptions);
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.okButton1);
            this.Controls.Add(this.xtraTabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.OptionsWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private Basic.OKButton okButton1;
        private Basic.CancelButton cancelButton1;
        private DevExpress.XtraEditors.SimpleButton btnResetAllOptions;
    }
}