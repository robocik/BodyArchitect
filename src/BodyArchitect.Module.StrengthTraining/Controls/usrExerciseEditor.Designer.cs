namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrExerciseEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrExerciseEditor));
            this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.tpExerciseInfo = new DevExpress.XtraTab.XtraTabPage();
            this.cmbForce = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblForce = new DevExpress.XtraEditors.LabelControl();
            this.cmbMechanicsType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblMechanicType = new DevExpress.XtraEditors.LabelControl();
            this.cmbExerciseType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtShortcut = new DevExpress.XtraEditors.TextEdit();
            this.lblShortcut = new DevExpress.XtraEditors.LabelControl();
            this.zoomDifficuilt = new DevExpress.XtraEditors.ZoomTrackBarControl();
            this.lblDifficuilt = new DevExpress.XtraEditors.LabelControl();
            this.lblExerciseType = new DevExpress.XtraEditors.LabelControl();
            this.txtUrl = new DevExpress.XtraEditors.TextEdit();
            this.lblUrl = new DevExpress.XtraEditors.LabelControl();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.tpComments = new DevExpress.XtraTab.XtraTabPage();
            this.usrWorkoutCommentsList1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutCommentsList();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.validationProvider1 = new Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
            this.xtraTabControl.SuspendLayout();
            this.tpExerciseInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbForce.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMechanicsType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExerciseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShortcut.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomDifficuilt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomDifficuilt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.tpComments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl
            // 
            resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
            this.xtraTabControl.Name = "xtraTabControl";
            this.xtraTabControl.SelectedTabPage = this.tpExerciseInfo;
            this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpExerciseInfo,
            this.tpComments});
            this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
            // 
            // tpExerciseInfo
            // 
            this.tpExerciseInfo.Controls.Add(this.cmbForce);
            this.tpExerciseInfo.Controls.Add(this.lblForce);
            this.tpExerciseInfo.Controls.Add(this.cmbMechanicsType);
            this.tpExerciseInfo.Controls.Add(this.lblMechanicType);
            this.tpExerciseInfo.Controls.Add(this.cmbExerciseType);
            this.tpExerciseInfo.Controls.Add(this.txtShortcut);
            this.tpExerciseInfo.Controls.Add(this.lblShortcut);
            this.tpExerciseInfo.Controls.Add(this.zoomDifficuilt);
            this.tpExerciseInfo.Controls.Add(this.lblDifficuilt);
            this.tpExerciseInfo.Controls.Add(this.lblExerciseType);
            this.tpExerciseInfo.Controls.Add(this.txtUrl);
            this.tpExerciseInfo.Controls.Add(this.lblUrl);
            this.tpExerciseInfo.Controls.Add(this.txtDescription);
            this.tpExerciseInfo.Controls.Add(this.lblDescription);
            this.tpExerciseInfo.Controls.Add(this.txtName);
            this.tpExerciseInfo.Controls.Add(this.lblName);
            this.tpExerciseInfo.Name = "tpExerciseInfo";
            resources.ApplyResources(this.tpExerciseInfo, "tpExerciseInfo");
            // 
            // cmbForce
            // 
            resources.ApplyResources(this.cmbForce, "cmbForce");
            this.cmbForce.Name = "cmbForce";
            this.cmbForce.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbForce.Properties.Buttons"))))});
            this.cmbForce.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblForce
            // 
            resources.ApplyResources(this.lblForce, "lblForce");
            this.lblForce.Name = "lblForce";
            // 
            // cmbMechanicsType
            // 
            resources.ApplyResources(this.cmbMechanicsType, "cmbMechanicsType");
            this.cmbMechanicsType.Name = "cmbMechanicsType";
            this.cmbMechanicsType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbMechanicsType.Properties.Buttons"))))});
            this.cmbMechanicsType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblMechanicType
            // 
            resources.ApplyResources(this.lblMechanicType, "lblMechanicType");
            this.lblMechanicType.Name = "lblMechanicType";
            // 
            // cmbExerciseType
            // 
            resources.ApplyResources(this.cmbExerciseType, "cmbExerciseType");
            this.cmbExerciseType.Name = "cmbExerciseType";
            this.cmbExerciseType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbExerciseType.Properties.Buttons"))))});
            this.cmbExerciseType.Properties.ReadOnly = true;
            // 
            // txtShortcut
            // 
            resources.ApplyResources(this.txtShortcut, "txtShortcut");
            this.txtShortcut.Name = "txtShortcut";
            this.validationProvider1.SetPerformValidation(this.txtShortcut, true);
            this.txtShortcut.Properties.MaxLength = 10;
            this.validationProvider1.SetSourcePropertyName(this.txtShortcut, "Shortcut");
            this.txtShortcut.EditValueChanged += new System.EventHandler(this.txtShortcut_EditValueChanged);
            // 
            // lblShortcut
            // 
            resources.ApplyResources(this.lblShortcut, "lblShortcut");
            this.lblShortcut.Name = "lblShortcut";
            // 
            // zoomDifficuilt
            // 
            resources.ApplyResources(this.zoomDifficuilt, "zoomDifficuilt");
            this.zoomDifficuilt.Name = "zoomDifficuilt";
            this.zoomDifficuilt.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            // 
            // lblDifficuilt
            // 
            resources.ApplyResources(this.lblDifficuilt, "lblDifficuilt");
            this.lblDifficuilt.Name = "lblDifficuilt";
            // 
            // lblExerciseType
            // 
            resources.ApplyResources(this.lblExerciseType, "lblExerciseType");
            this.lblExerciseType.Name = "lblExerciseType";
            // 
            // txtUrl
            // 
            resources.ApplyResources(this.txtUrl, "txtUrl");
            this.txtUrl.Name = "txtUrl";
            this.validationProvider1.SetPerformValidation(this.txtUrl, true);
            this.txtUrl.Properties.MaxLength = 1000;
            this.validationProvider1.SetSourcePropertyName(this.txtUrl, "Url");
            // 
            // lblUrl
            // 
            resources.ApplyResources(this.lblUrl, "lblUrl");
            this.lblUrl.Name = "lblUrl";
            // 
            // txtDescription
            // 
            resources.ApplyResources(this.txtDescription, "txtDescription");
            this.txtDescription.Name = "txtDescription";
            this.validationProvider1.SetPerformValidation(this.txtDescription, true);
            this.validationProvider1.SetSourcePropertyName(this.txtDescription, "Description");
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.validationProvider1.SetPerformValidation(this.txtName, true);
            this.txtName.Properties.MaxLength = 100;
            this.validationProvider1.SetSourcePropertyName(this.txtName, "Name");
            this.txtName.EditValueChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // tpComments
            // 
            this.tpComments.Controls.Add(this.usrWorkoutCommentsList1);
            this.tpComments.Name = "tpComments";
            resources.ApplyResources(this.tpComments, "tpComments");
            // 
            // usrWorkoutCommentsList1
            // 
            resources.ApplyResources(this.usrWorkoutCommentsList1, "usrWorkoutCommentsList1");
            this.usrWorkoutCommentsList1.Name = "usrWorkoutCommentsList1";
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // validationProvider1
            // 
            this.validationProvider1.ErrorProvider = null;
            this.validationProvider1.RulesetName = "";
            this.validationProvider1.SourceTypeName = "BodyArchitect.Service.Model.ExerciseDTO, BodyArchitect.Service.Model";
            this.validationProvider1.ValidationPerformed += new System.EventHandler<Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs>(this.validationProvider1_ValidationPerformed);
            // 
            // usrExerciseEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xtraTabControl);
            this.Name = "usrExerciseEditor";
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
            this.xtraTabControl.ResumeLayout(false);
            this.tpExerciseInfo.ResumeLayout(false);
            this.tpExerciseInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbForce.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMechanicsType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExerciseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShortcut.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomDifficuilt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoomDifficuilt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.tpComments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl;
        private DevExpress.XtraTab.XtraTabPage tpExerciseInfo;
        private DevExpress.XtraEditors.ComboBoxEdit cmbForce;
        private DevExpress.XtraEditors.LabelControl lblForce;
        private DevExpress.XtraEditors.ComboBoxEdit cmbMechanicsType;
        private DevExpress.XtraEditors.LabelControl lblMechanicType;
        private DevExpress.XtraEditors.LookUpEdit cmbExerciseType;
        private DevExpress.XtraEditors.TextEdit txtShortcut;
        private DevExpress.XtraEditors.LabelControl lblShortcut;
        private DevExpress.XtraEditors.ZoomTrackBarControl zoomDifficuilt;
        private DevExpress.XtraEditors.LabelControl lblDifficuilt;
        private DevExpress.XtraEditors.LabelControl lblExerciseType;
        private DevExpress.XtraEditors.TextEdit txtUrl;
        private DevExpress.XtraEditors.LabelControl lblUrl;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private DevExpress.XtraTab.XtraTabPage tpComments;
        private usrWorkoutCommentsList usrWorkoutCommentsList1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider validationProvider1;
    }
}
