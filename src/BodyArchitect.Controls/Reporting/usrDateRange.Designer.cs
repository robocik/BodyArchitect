using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.Reporting
{
    partial class usrDateRange
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrDateRange));
            this.grTime = new BaGroupControl();
            this.dtaTo = new DevExpress.XtraEditors.DateEdit();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.dtaFrom = new DevExpress.XtraEditors.DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.grTime)).BeginInit();
            this.grTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grTime
            // 
            resources.ApplyResources(this.grTime, "grTime");
            this.grTime.Controls.Add(this.dtaTo);
            this.grTime.Controls.Add(this.lblFrom);
            this.grTime.Controls.Add(this.lblTo);
            this.grTime.Controls.Add(this.dtaFrom);
            this.grTime.Name = "grTime";
            // 
            // dtaTo
            // 
            resources.ApplyResources(this.dtaTo, "dtaTo");
            this.dtaTo.Name = "dtaTo";
            this.dtaTo.Properties.AccessibleDescription = resources.GetString("dtaTo.Properties.AccessibleDescription");
            this.dtaTo.Properties.AccessibleName = resources.GetString("dtaTo.Properties.AccessibleName");
            this.dtaTo.Properties.AutoHeight = ((bool)(resources.GetObject("dtaTo.Properties.AutoHeight")));
            this.dtaTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaTo.Properties.Buttons"))))});
            this.dtaTo.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("dtaTo.Properties.Mask.AutoComplete")));
            this.dtaTo.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("dtaTo.Properties.Mask.BeepOnError")));
            this.dtaTo.Properties.Mask.EditMask = resources.GetString("dtaTo.Properties.Mask.EditMask");
            this.dtaTo.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("dtaTo.Properties.Mask.IgnoreMaskBlank")));
            this.dtaTo.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dtaTo.Properties.Mask.MaskType")));
            this.dtaTo.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("dtaTo.Properties.Mask.PlaceHolder")));
            this.dtaTo.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("dtaTo.Properties.Mask.SaveLiteral")));
            this.dtaTo.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("dtaTo.Properties.Mask.ShowPlaceHolders")));
            this.dtaTo.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("dtaTo.Properties.Mask.UseMaskAsDisplayFormat")));
            this.dtaTo.Properties.NullValuePrompt = resources.GetString("dtaTo.Properties.NullValuePrompt");
            this.dtaTo.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("dtaTo.Properties.NullValuePromptShowForEmptyValue")));
            this.dtaTo.Properties.VistaTimeProperties.AccessibleDescription = resources.GetString("dtaTo.Properties.VistaTimeProperties.AccessibleDescription");
            this.dtaTo.Properties.VistaTimeProperties.AccessibleName = resources.GetString("dtaTo.Properties.VistaTimeProperties.AccessibleName");
            this.dtaTo.Properties.VistaTimeProperties.AutoHeight = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.AutoHeight")));
            this.dtaTo.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtaTo.Properties.VistaTimeProperties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.AutoComplete")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.BeepOnError = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.BeepOnError")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.EditMask = resources.GetString("dtaTo.Properties.VistaTimeProperties.Mask.EditMask");
            this.dtaTo.Properties.VistaTimeProperties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.IgnoreMaskBlank")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.MaskType")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.PlaceHolder = ((char)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.PlaceHolder")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.SaveLiteral = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.SaveLiteral")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.ShowPlaceHolders")));
            this.dtaTo.Properties.VistaTimeProperties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.Mask.UseMaskAsDisplayFormat")));
            this.dtaTo.Properties.VistaTimeProperties.NullValuePrompt = resources.GetString("dtaTo.Properties.VistaTimeProperties.NullValuePrompt");
            this.dtaTo.Properties.VistaTimeProperties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("dtaTo.Properties.VistaTimeProperties.NullValuePromptShowForEmptyValue")));
            // 
            // lblFrom
            // 
            resources.ApplyResources(this.lblFrom, "lblFrom");
            this.lblFrom.Name = "lblFrom";
            // 
            // lblTo
            // 
            resources.ApplyResources(this.lblTo, "lblTo");
            this.lblTo.Name = "lblTo";
            // 
            // dtaFrom
            // 
            resources.ApplyResources(this.dtaFrom, "dtaFrom");
            this.dtaFrom.Name = "dtaFrom";
            this.dtaFrom.Properties.AccessibleDescription = resources.GetString("dtaFrom.Properties.AccessibleDescription");
            this.dtaFrom.Properties.AccessibleName = resources.GetString("dtaFrom.Properties.AccessibleName");
            this.dtaFrom.Properties.AutoHeight = ((bool)(resources.GetObject("dtaFrom.Properties.AutoHeight")));
            this.dtaFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaFrom.Properties.Buttons"))))});
            this.dtaFrom.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("dtaFrom.Properties.Mask.AutoComplete")));
            this.dtaFrom.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("dtaFrom.Properties.Mask.BeepOnError")));
            this.dtaFrom.Properties.Mask.EditMask = resources.GetString("dtaFrom.Properties.Mask.EditMask");
            this.dtaFrom.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("dtaFrom.Properties.Mask.IgnoreMaskBlank")));
            this.dtaFrom.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dtaFrom.Properties.Mask.MaskType")));
            this.dtaFrom.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("dtaFrom.Properties.Mask.PlaceHolder")));
            this.dtaFrom.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("dtaFrom.Properties.Mask.SaveLiteral")));
            this.dtaFrom.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("dtaFrom.Properties.Mask.ShowPlaceHolders")));
            this.dtaFrom.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("dtaFrom.Properties.Mask.UseMaskAsDisplayFormat")));
            this.dtaFrom.Properties.NullValuePrompt = resources.GetString("dtaFrom.Properties.NullValuePrompt");
            this.dtaFrom.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("dtaFrom.Properties.NullValuePromptShowForEmptyValue")));
            this.dtaFrom.Properties.VistaTimeProperties.AccessibleDescription = resources.GetString("dtaFrom.Properties.VistaTimeProperties.AccessibleDescription");
            this.dtaFrom.Properties.VistaTimeProperties.AccessibleName = resources.GetString("dtaFrom.Properties.VistaTimeProperties.AccessibleName");
            this.dtaFrom.Properties.VistaTimeProperties.AutoHeight = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.AutoHeight")));
            this.dtaFrom.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtaFrom.Properties.VistaTimeProperties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.AutoComplete")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.BeepOnError = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.BeepOnError")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.EditMask = resources.GetString("dtaFrom.Properties.VistaTimeProperties.Mask.EditMask");
            this.dtaFrom.Properties.VistaTimeProperties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.IgnoreMaskBlank")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.MaskType")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.PlaceHolder = ((char)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.PlaceHolder")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.SaveLiteral = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.SaveLiteral")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.ShowPlaceHolders")));
            this.dtaFrom.Properties.VistaTimeProperties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.Mask.UseMaskAsDisplayFormat")));
            this.dtaFrom.Properties.VistaTimeProperties.NullValuePrompt = resources.GetString("dtaFrom.Properties.VistaTimeProperties.NullValuePrompt");
            this.dtaFrom.Properties.VistaTimeProperties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("dtaFrom.Properties.VistaTimeProperties.NullValuePromptShowForEmptyValue")));
            // 
            // usrDateRange
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grTime);
            this.Name = "usrDateRange";
            ((System.ComponentModel.ISupportInitialize)(this.grTime)).EndInit();
            this.grTime.ResumeLayout(false);
            this.grTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grTime;
        private DevExpress.XtraEditors.DateEdit dtaTo;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.DateEdit dtaFrom;
    }
}
