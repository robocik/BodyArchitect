namespace BodyArchitect.Controls.Forms
{
    partial class InputWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputWindow));
            this.lblValue = new DevExpress.XtraEditors.LabelControl();
            this.txtValue = new DevExpress.XtraEditors.TextEdit();
            this.btnOK = new BodyArchitect.Controls.Basic.OKButton();
            this.btnCancel = new BodyArchitect.Controls.Basic.CancelButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblValue
            // 
            resources.ApplyResources(this.lblValue, "lblValue");
            this.lblValue.Name = "lblValue";
            // 
            // txtValue
            // 
            resources.ApplyResources(this.txtValue, "txtValue");
            this.txtValue.Name = "txtValue";
            this.txtValue.Properties.AccessibleDescription = resources.GetString("txtValue.Properties.AccessibleDescription");
            this.txtValue.Properties.AccessibleName = resources.GetString("txtValue.Properties.AccessibleName");
            this.txtValue.Properties.AutoHeight = ((bool)(resources.GetObject("txtValue.Properties.AutoHeight")));
            this.txtValue.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("txtValue.Properties.Mask.AutoComplete")));
            this.txtValue.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("txtValue.Properties.Mask.BeepOnError")));
            this.txtValue.Properties.Mask.EditMask = resources.GetString("txtValue.Properties.Mask.EditMask");
            this.txtValue.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("txtValue.Properties.Mask.IgnoreMaskBlank")));
            this.txtValue.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtValue.Properties.Mask.MaskType")));
            this.txtValue.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("txtValue.Properties.Mask.PlaceHolder")));
            this.txtValue.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("txtValue.Properties.Mask.SaveLiteral")));
            this.txtValue.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtValue.Properties.Mask.ShowPlaceHolders")));
            this.txtValue.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("txtValue.Properties.Mask.UseMaskAsDisplayFormat")));
            this.txtValue.Properties.MaxLength = 100;
            this.txtValue.Properties.NullValuePrompt = resources.GetString("txtValue.Properties.NullValuePrompt");
            this.txtValue.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("txtValue.Properties.NullValuePromptShowForEmptyValue")));
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.Name = "btnOK";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Name = "btnCancel";
            // 
            // InputWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.lblValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.txtValue.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblValue;
        private DevExpress.XtraEditors.TextEdit txtValue;
        private Basic.OKButton btnOK;
        private Basic.CancelButton btnCancel;
    }
}