namespace BodyArchitect.Controls.Calculators
{
    partial class BMIWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BMIWindow));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.lblCM = new DevExpress.XtraEditors.LabelControl();
            this.lblKg = new DevExpress.XtraEditors.LabelControl();
            this.txtWeight = new DevExpress.XtraEditors.TextEdit();
            this.lblWeight = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.lblBMI = new DevExpress.XtraEditors.LabelControl();
            this.txtBMI = new DevExpress.XtraEditors.TextEdit();
            this.lblBmiResult = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBMI.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // textEdit1
            // 
            resources.ApplyResources(this.textEdit1, "textEdit1");
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.Mask.EditMask = resources.GetString("textEdit1.Properties.Mask.EditMask");
            this.textEdit1.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("textEdit1.Properties.Mask.MaskType")));
            this.textEdit1.EditValueChanged += new System.EventHandler(this.textEdit1_EditValueChanged);
            // 
            // lblCM
            // 
            resources.ApplyResources(this.lblCM, "lblCM");
            this.lblCM.Name = "lblCM";
            // 
            // lblKg
            // 
            resources.ApplyResources(this.lblKg, "lblKg");
            this.lblKg.Name = "lblKg";
            // 
            // txtWeight
            // 
            resources.ApplyResources(this.txtWeight, "txtWeight");
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Properties.Mask.EditMask = resources.GetString("txtWeight.Properties.Mask.EditMask");
            this.txtWeight.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtWeight.Properties.Mask.MaskType")));
            this.txtWeight.EditValueChanged += new System.EventHandler(this.textEdit1_EditValueChanged);
            // 
            // lblWeight
            // 
            resources.ApplyResources(this.lblWeight, "lblWeight");
            this.lblWeight.Name = "lblWeight";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            // 
            // lblBMI
            // 
            resources.ApplyResources(this.lblBMI, "lblBMI");
            this.lblBMI.Name = "lblBMI";
            // 
            // txtBMI
            // 
            resources.ApplyResources(this.txtBMI, "txtBMI");
            this.txtBMI.Name = "txtBMI";
            this.txtBMI.Properties.ReadOnly = true;
            // 
            // lblBmiResult
            // 
            resources.ApplyResources(this.lblBmiResult, "lblBmiResult");
            this.lblBmiResult.Name = "lblBmiResult";
            // 
            // BMIWindow
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.lblBmiResult);
            this.Controls.Add(this.txtBMI);
            this.Controls.Add(this.lblBMI);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblKg);
            this.Controls.Add(this.txtWeight);
            this.Controls.Add(this.lblWeight);
            this.Controls.Add(this.lblCM);
            this.Controls.Add(this.textEdit1);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BMIWindow";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeight.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBMI.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.LabelControl lblCM;
        private DevExpress.XtraEditors.LabelControl lblKg;
        private DevExpress.XtraEditors.TextEdit txtWeight;
        private DevExpress.XtraEditors.LabelControl lblWeight;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lblBMI;
        private DevExpress.XtraEditors.TextEdit txtBMI;
        private DevExpress.XtraEditors.LabelControl lblBmiResult;
    }
}