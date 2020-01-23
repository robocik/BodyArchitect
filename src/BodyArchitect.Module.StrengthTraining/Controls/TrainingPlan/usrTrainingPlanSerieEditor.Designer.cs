namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrTrainingPlanSerieEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrTrainingPlanSerieEditor));
            this.lblRepetitionsType = new DevExpress.XtraEditors.LabelControl();
            this.cmbRepetitionsType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblRepetitionsRange = new DevExpress.XtraEditors.LabelControl();
            this.txtRepetitionsRange = new DevExpress.XtraEditors.TextEdit();
            this.lblComment = new DevExpress.XtraEditors.LabelControl();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.cmbDropSet = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDropSet = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRepetitionsType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionsRange.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDropSet.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRepetitionsType
            // 
            resources.ApplyResources(this.lblRepetitionsType, "lblRepetitionsType");
            this.lblRepetitionsType.Name = "lblRepetitionsType";
            // 
            // cmbRepetitionsType
            // 
            resources.ApplyResources(this.cmbRepetitionsType, "cmbRepetitionsType");
            this.cmbRepetitionsType.Name = "cmbRepetitionsType";
            this.cmbRepetitionsType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbRepetitionsType.Properties.Buttons"))))});
            this.cmbRepetitionsType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbRepetitionsType.SelectedIndexChanged += new System.EventHandler(this.cmbRepetitionsType_SelectedIndexChanged);
            // 
            // lblRepetitionsRange
            // 
            resources.ApplyResources(this.lblRepetitionsRange, "lblRepetitionsRange");
            this.lblRepetitionsRange.Name = "lblRepetitionsRange";
            // 
            // txtRepetitionsRange
            // 
            resources.ApplyResources(this.txtRepetitionsRange, "txtRepetitionsRange");
            this.txtRepetitionsRange.Name = "txtRepetitionsRange";
            this.txtRepetitionsRange.Properties.Mask.EditMask = resources.GetString("txtRepetitionsRange.Properties.Mask.EditMask");
            this.txtRepetitionsRange.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtRepetitionsRange.Properties.Mask.MaskType")));
            this.txtRepetitionsRange.Validated += new System.EventHandler(this.txtRepetitionsRange_Validated);
            // 
            // lblComment
            // 
            resources.ApplyResources(this.lblComment, "lblComment");
            this.lblComment.Name = "lblComment";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            this.txtComment.Validated += new System.EventHandler(this.txtComment_Validated);
            // 
            // cmbDropSet
            // 
            resources.ApplyResources(this.cmbDropSet, "cmbDropSet");
            this.cmbDropSet.Name = "cmbDropSet";
            this.cmbDropSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbDropSet.Properties.Buttons"))))});
            this.cmbDropSet.Properties.Items.AddRange(new object[] {
            resources.GetString("cmbDropSet.Properties.Items"),
            resources.GetString("cmbDropSet.Properties.Items1"),
            resources.GetString("cmbDropSet.Properties.Items2"),
            resources.GetString("cmbDropSet.Properties.Items3"),
            resources.GetString("cmbDropSet.Properties.Items4")});
            this.cmbDropSet.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDropSet.SelectedIndexChanged += new System.EventHandler(this.cmbDropSet_SelectedIndexChanged);
            // 
            // lblDropSet
            // 
            resources.ApplyResources(this.lblDropSet, "lblDropSet");
            this.lblDropSet.Name = "lblDropSet";
            // 
            // usrTrainingPlanSerieEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbDropSet);
            this.Controls.Add(this.lblDropSet);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.txtRepetitionsRange);
            this.Controls.Add(this.lblRepetitionsRange);
            this.Controls.Add(this.cmbRepetitionsType);
            this.Controls.Add(this.lblRepetitionsType);
            this.Name = "usrTrainingPlanSerieEditor";
            ((System.ComponentModel.ISupportInitialize)(this.cmbRepetitionsType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRepetitionsRange.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDropSet.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblRepetitionsType;
        private DevExpress.XtraEditors.ComboBoxEdit cmbRepetitionsType;
        private DevExpress.XtraEditors.LabelControl lblRepetitionsRange;
        private DevExpress.XtraEditors.TextEdit txtRepetitionsRange;
        private DevExpress.XtraEditors.LabelControl lblComment;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDropSet;
        private DevExpress.XtraEditors.LabelControl lblDropSet;
    }
}
