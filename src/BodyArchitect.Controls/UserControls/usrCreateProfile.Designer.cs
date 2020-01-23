namespace BodyArchitect.Controls.UserControls
{
    partial class usrCreateProfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrCreateProfile));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
            DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule compareAgainstControlValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule();
            this.txtPassword1 = new DevExpress.XtraEditors.TextEdit();
            this.validationProvider1 = new Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.dtaBornDate = new DevExpress.XtraEditors.DateEdit();
            this.luCountries = new DevExpress.XtraEditors.LookUpEdit();
            this.lblCountry = new DevExpress.XtraEditors.LabelControl();
            this.lblEmail = new DevExpress.XtraEditors.LabelControl();
            this.picUserNameAvailability = new System.Windows.Forms.PictureBox();
            this.lblVerifyPassword = new DevExpress.XtraEditors.LabelControl();
            this.lblPassword1 = new DevExpress.XtraEditors.LabelControl();
            this.txtVerifyPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblBornDate = new System.Windows.Forms.Label();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.rbMale = new DevExpress.XtraEditors.CheckEdit();
            this.rbFemale = new DevExpress.XtraEditors.CheckEdit();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaBornDate.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaBornDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.luCountries.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserNameAvailability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVerifyPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbMale.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbFemale.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPassword1
            // 
            resources.ApplyResources(this.txtPassword1, "txtPassword1");
            this.txtPassword1.Name = "txtPassword1";
            this.txtPassword1.Properties.UseSystemPasswordChar = true;
            this.txtPassword1.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // validationProvider1
            // 
            this.validationProvider1.ErrorProvider = null;
            this.validationProvider1.RulesetName = "";
            this.validationProvider1.SourceTypeName = "BodyArchitect.Service.Model.ProfileDTO, BodyArchitect.Service.Model";
            this.validationProvider1.ValidationPerformed += new System.EventHandler<Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs>(this.validationProvider1_ValidationPerformed);
            // 
            // txtUserName
            // 
            resources.ApplyResources(this.txtUserName, "txtUserName");
            this.txtUserName.Name = "txtUserName";
            this.validationProvider1.SetPerformValidation(this.txtUserName, true);
            this.txtUserName.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("txtUserName.Properties.Mask.AutoComplete")));
            this.txtUserName.Properties.Mask.EditMask = resources.GetString("txtUserName.Properties.Mask.EditMask");
            this.txtUserName.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtUserName.Properties.Mask.MaskType")));
            this.validationProvider1.SetSourcePropertyName(this.txtUserName, "UserName");
            resources.ApplyResources(toolTipTitleItem1, "toolTipTitleItem1");
            toolTipItem1.LeftIndent = 6;
            resources.ApplyResources(toolTipItem1, "toolTipItem1");
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.txtUserName.SuperTip = superToolTip1;
            this.txtUserName.EditValueChanged += new System.EventHandler(this.txtUserName_EditValueChanged);
            // 
            // txtEmail
            // 
            resources.ApplyResources(this.txtEmail, "txtEmail");
            this.txtEmail.Name = "txtEmail";
            this.validationProvider1.SetPerformValidation(this.txtEmail, true);
            this.validationProvider1.SetSourcePropertyName(this.txtEmail, "Email");
            resources.ApplyResources(toolTipTitleItem2, "toolTipTitleItem2");
            toolTipItem2.LeftIndent = 6;
            resources.ApplyResources(toolTipItem2, "toolTipItem2");
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this.txtEmail.SuperTip = superToolTip2;
            this.txtEmail.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // dtaBornDate
            // 
            resources.ApplyResources(this.dtaBornDate, "dtaBornDate");
            this.dtaBornDate.Name = "dtaBornDate";
            this.validationProvider1.SetPerformValidation(this.dtaBornDate, true);
            this.dtaBornDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaBornDate.Properties.Buttons"))))});
            this.dtaBornDate.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.validationProvider1.SetSourcePropertyName(this.dtaBornDate, "Birthday");
            resources.ApplyResources(toolTipTitleItem3, "toolTipTitleItem3");
            toolTipItem3.LeftIndent = 6;
            resources.ApplyResources(toolTipItem3, "toolTipItem3");
            superToolTip3.Items.Add(toolTipTitleItem3);
            superToolTip3.Items.Add(toolTipItem3);
            this.dtaBornDate.SuperTip = superToolTip3;
            this.validationProvider1.SetValidatedProperty(this.dtaBornDate, "DateTime");
            this.dtaBornDate.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // luCountries
            // 
            resources.ApplyResources(this.luCountries, "luCountries");
            this.luCountries.Name = "luCountries";
            this.validationProvider1.SetPerformValidation(this.luCountries, true);
            this.luCountries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("luCountries.Properties.Buttons"))))});
            this.luCountries.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luCountries.Properties.Columns"), ((int)(resources.GetObject("luCountries.Properties.Columns1"))), resources.GetString("luCountries.Properties.Columns2")),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("luCountries.Properties.Columns3"), resources.GetString("luCountries.Properties.Columns4"))});
            this.luCountries.Properties.NullValuePrompt = resources.GetString("luCountries.Properties.NullValuePrompt");
            this.luCountries.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("luCountries.Properties.NullValuePromptShowForEmptyValue")));
            this.validationProvider1.SetSourcePropertyName(this.luCountries, "CountryId");
            resources.ApplyResources(toolTipTitleItem4, "toolTipTitleItem4");
            toolTipItem4.LeftIndent = 6;
            resources.ApplyResources(toolTipItem4, "toolTipItem4");
            superToolTip4.Items.Add(toolTipTitleItem4);
            superToolTip4.Items.Add(toolTipItem4);
            this.luCountries.SuperTip = superToolTip4;
            this.validationProvider1.SetValidatedProperty(this.luCountries, "EditValue");
            this.luCountries.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // lblCountry
            // 
            resources.ApplyResources(this.lblCountry, "lblCountry");
            this.lblCountry.Name = "lblCountry";
            // 
            // lblEmail
            // 
            resources.ApplyResources(this.lblEmail, "lblEmail");
            this.lblEmail.Name = "lblEmail";
            // 
            // picUserNameAvailability
            // 
            resources.ApplyResources(this.picUserNameAvailability, "picUserNameAvailability");
            this.picUserNameAvailability.Name = "picUserNameAvailability";
            this.picUserNameAvailability.TabStop = false;
            // 
            // lblVerifyPassword
            // 
            resources.ApplyResources(this.lblVerifyPassword, "lblVerifyPassword");
            this.lblVerifyPassword.Name = "lblVerifyPassword";
            // 
            // lblPassword1
            // 
            resources.ApplyResources(this.lblPassword1, "lblPassword1");
            this.lblPassword1.Name = "lblPassword1";
            // 
            // txtVerifyPassword
            // 
            resources.ApplyResources(this.txtVerifyPassword, "txtVerifyPassword");
            this.txtVerifyPassword.Name = "txtVerifyPassword";
            this.txtVerifyPassword.Properties.UseSystemPasswordChar = true;
            compareAgainstControlValidationRule1.CaseSensitive = true;
            compareAgainstControlValidationRule1.CompareControlOperator = DevExpress.XtraEditors.DXErrorProvider.CompareControlOperator.Equals;
            compareAgainstControlValidationRule1.Control = this.txtPassword1;
            resources.ApplyResources(compareAgainstControlValidationRule1, "compareAgainstControlValidationRule1");
            this.dxValidationProvider1.SetValidationRule(this.txtVerifyPassword, compareAgainstControlValidationRule1);
            this.txtVerifyPassword.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            this.txtVerifyPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtVerifyPassword_Validating);
            // 
            // lblBornDate
            // 
            resources.ApplyResources(this.lblBornDate, "lblBornDate");
            this.lblBornDate.Name = "lblBornDate";
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.Name = "lblUserName";
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Manual;
            // 
            // rbMale
            // 
            resources.ApplyResources(this.rbMale, "rbMale");
            this.rbMale.Name = "rbMale";
            this.rbMale.Properties.AutoWidth = true;
            this.rbMale.Properties.Caption = resources.GetString("rbMale.Properties.Caption");
            this.rbMale.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbMale.Properties.RadioGroupIndex = 0;
            this.rbMale.TabStop = false;
            this.rbMale.CheckedChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // rbFemale
            // 
            resources.ApplyResources(this.rbFemale, "rbFemale");
            this.rbFemale.Name = "rbFemale";
            this.rbFemale.Properties.AutoWidth = true;
            this.rbFemale.Properties.Caption = resources.GetString("rbFemale.Properties.Caption");
            this.rbFemale.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rbFemale.Properties.RadioGroupIndex = 0;
            this.rbFemale.TabStop = false;
            this.rbFemale.CheckedChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // lblGender
            // 
            resources.ApplyResources(this.lblGender, "lblGender");
            this.lblGender.Name = "lblGender";
            // 
            // usrCreateProfile
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.rbFemale);
            this.Controls.Add(this.rbMale);
            this.Controls.Add(this.luCountries);
            this.Controls.Add(this.lblCountry);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.picUserNameAvailability);
            this.Controls.Add(this.lblVerifyPassword);
            this.Controls.Add(this.lblPassword1);
            this.Controls.Add(this.txtVerifyPassword);
            this.Controls.Add(this.txtPassword1);
            this.Controls.Add(this.dtaBornDate);
            this.Controls.Add(this.lblBornDate);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblUserName);
            this.Name = "usrCreateProfile";
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaBornDate.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaBornDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.luCountries.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUserNameAvailability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVerifyPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbMale.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbFemale.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider validationProvider1;
        private DevExpress.XtraEditors.TextEdit txtEmail;
        private DevExpress.XtraEditors.LookUpEdit luCountries;
        private DevExpress.XtraEditors.LabelControl lblCountry;
        private DevExpress.XtraEditors.LabelControl lblEmail;
        private System.Windows.Forms.PictureBox picUserNameAvailability;
        private DevExpress.XtraEditors.LabelControl lblVerifyPassword;
        private DevExpress.XtraEditors.LabelControl lblPassword1;
        private DevExpress.XtraEditors.TextEdit txtVerifyPassword;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.TextEdit txtPassword1;
        private DevExpress.XtraEditors.DateEdit dtaBornDate;
        private System.Windows.Forms.Label lblBornDate;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraEditors.LabelControl lblUserName;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.CheckEdit rbFemale;
        private DevExpress.XtraEditors.CheckEdit rbMale;
        private DevExpress.XtraEditors.LabelControl lblGender;


    }
}
