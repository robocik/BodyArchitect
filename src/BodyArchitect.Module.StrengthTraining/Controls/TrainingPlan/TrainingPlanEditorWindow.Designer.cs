using System.Windows.Forms;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class TrainingPlanEditorWindow
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainingPlanEditorWindow));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            this.grDescription = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.grDetails = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tvDetails = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbMoveUp = new System.Windows.Forms.ToolStripButton();
            this.tbMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbNewDay = new System.Windows.Forms.ToolStripButton();
            this.tbEditDay = new System.Windows.Forms.ToolStripButton();
            this.tbDeleteDay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbNewEntry = new System.Windows.Forms.ToolStripButton();
            this.tbDeleteEntry = new System.Windows.Forms.ToolStripButton();
            this.tbNewSet = new System.Windows.Forms.ToolStripButton();
            this.tbDeleteSet = new System.Windows.Forms.ToolStripButton();
            this.expandGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lvOutput = new System.Windows.Forms.ListView();
            this.colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colObject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colObjectType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgOutput = new System.Windows.Forms.ImageList(this.components);
            this.btnSuperSets = new DevExpress.XtraEditors.SimpleButton();
            this.btnValidate = new DevExpress.XtraEditors.SimpleButton();
            this.validationProvider1 = new Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider();
            this.luCountries = new DevExpress.XtraEditors.LookUpEdit();
            this.txtUrl = new DevExpress.XtraEditors.TextEdit();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.okButton1 = new BodyArchitect.Controls.Basic.OKButton();
            this.grInformation = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lblSeconds = new DevExpress.XtraEditors.LabelControl();
            this.txtRestSeconds = new DevExpress.XtraEditors.SpinEdit();
            this.cmbPurpose = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblLanguage = new DevExpress.XtraEditors.LabelControl();
            this.lblUrl = new DevExpress.XtraEditors.LabelControl();
            this.lblRestSeconds = new DevExpress.XtraEditors.LabelControl();
            this.cmbDifficult = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDifficult = new DevExpress.XtraEditors.LabelControl();
            this.cmbTrainingType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblTrainingType = new DevExpress.XtraEditors.LabelControl();
            this.txtAuthor = new DevExpress.XtraEditors.TextEdit();
            this.lblAuthor = new DevExpress.XtraEditors.LabelControl();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.usrTrainingPlanSerieEditor1 = new BodyArchitect.Module.StrengthTraining.Controls.usrTrainingPlanSerieEditor();
            this.UsrTrainingPlanEntryEditor1 = new BodyArchitect.Module.StrengthTraining.Controls.usrTrainingPlanEntryEditor();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
            this.splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grDescription)).BeginInit();
            this.grDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grDetails)).BeginInit();
            this.grDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl3)).BeginInit();
            this.splitContainerControl3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expandGroupControl1)).BeginInit();
            this.expandGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.luCountries.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInformation)).BeginInit();
            this.grInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestSeconds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPurpose.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDifficult.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTrainingType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAuthor.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.splitContainerControl2);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.expandGroupControl1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 81;
            // 
            // splitContainerControl2
            // 
            resources.ApplyResources(this.splitContainerControl2, "splitContainerControl2");
            this.splitContainerControl2.Horizontal = false;
            this.splitContainerControl2.Name = "splitContainerControl2";
            this.splitContainerControl2.Panel1.Controls.Add(this.grDescription);
            resources.ApplyResources(this.splitContainerControl2.Panel1, "splitContainerControl2.Panel1");
            this.splitContainerControl2.Panel2.Controls.Add(this.grDetails);
            resources.ApplyResources(this.splitContainerControl2.Panel2, "splitContainerControl2.Panel2");
            this.splitContainerControl2.SplitterPosition = 123;
            // 
            // grDescription
            // 
            this.grDescription.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grDescription.AppearanceCaption.BackColor")));
            this.grDescription.AppearanceCaption.Options.UseBackColor = true;
            this.grDescription.AppearanceCaption.Options.UseTextOptions = true;
            this.grDescription.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grDescription.Controls.Add(this.txtComment);
            resources.ApplyResources(this.grDescription, "grDescription");
            this.grDescription.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grDescription.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grDescription.Name = "grDescription";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            // 
            // grDetails
            // 
            this.grDetails.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grDetails.AppearanceCaption.BackColor")));
            this.grDetails.AppearanceCaption.Options.UseBackColor = true;
            this.grDetails.AppearanceCaption.Options.UseTextOptions = true;
            this.grDetails.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grDetails.Controls.Add(this.splitContainerControl3);
            resources.ApplyResources(this.grDetails, "grDetails");
            this.grDetails.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grDetails.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grDetails.Name = "grDetails";
            // 
            // splitContainerControl3
            // 
            resources.ApplyResources(this.splitContainerControl3, "splitContainerControl3");
            this.splitContainerControl3.Name = "splitContainerControl3";
            this.splitContainerControl3.Panel1.Controls.Add(this.tvDetails);
            this.splitContainerControl3.Panel1.Controls.Add(this.toolStrip1);
            resources.ApplyResources(this.splitContainerControl3.Panel1, "splitContainerControl3.Panel1");
            this.splitContainerControl3.Panel2.Controls.Add(this.usrTrainingPlanSerieEditor1);
            this.splitContainerControl3.Panel2.Controls.Add(this.UsrTrainingPlanEntryEditor1);
            resources.ApplyResources(this.splitContainerControl3.Panel2, "splitContainerControl3.Panel2");
            this.splitContainerControl3.SplitterPosition = 304;
            // 
            // tvDetails
            // 
            resources.ApplyResources(this.tvDetails, "tvDetails");
            this.tvDetails.HideSelection = false;
            this.tvDetails.ImageList = this.imageList1;
            this.tvDetails.LabelEdit = true;
            this.tvDetails.Name = "tvDetails";
            this.tvDetails.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvDetails_BeforeLabelEdit);
            this.tvDetails.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvDetails_AfterLabelEdit);
            this.tvDetails.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvDetails_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "TrainingPlanDay");
            this.imageList1.Images.SetKeyName(1, "TrainingPlanEntry");
            this.imageList1.Images.SetKeyName(2, "TrainingPlanEntryError");
            this.imageList1.Images.SetKeyName(3, "Set");
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbMoveUp,
            this.tbMoveDown,
            this.toolStripSeparator1,
            this.tbNewDay,
            this.tbEditDay,
            this.tbDeleteDay,
            this.toolStripSeparator2,
            this.tbNewEntry,
            this.tbDeleteEntry,
            this.tbNewSet,
            this.tbDeleteSet});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // tbMoveUp
            // 
            this.tbMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbMoveUp, "tbMoveUp");
            this.tbMoveUp.Name = "tbMoveUp";
            this.tbMoveUp.Click += new System.EventHandler(this.tbMoveUp_Click);
            // 
            // tbMoveDown
            // 
            this.tbMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbMoveDown, "tbMoveDown");
            this.tbMoveDown.Name = "tbMoveDown";
            this.tbMoveDown.Click += new System.EventHandler(this.tbMoveDown_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tbNewDay
            // 
            this.tbNewDay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbNewDay, "tbNewDay");
            this.tbNewDay.Name = "tbNewDay";
            this.tbNewDay.Click += new System.EventHandler(this.tbNewDay_Click);
            // 
            // tbEditDay
            // 
            this.tbEditDay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbEditDay, "tbEditDay");
            this.tbEditDay.Name = "tbEditDay";
            this.tbEditDay.Click += new System.EventHandler(this.tbEditDay_Click);
            // 
            // tbDeleteDay
            // 
            this.tbDeleteDay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbDeleteDay, "tbDeleteDay");
            this.tbDeleteDay.Name = "tbDeleteDay";
            this.tbDeleteDay.Click += new System.EventHandler(this.tbDeleteDay_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tbNewEntry
            // 
            this.tbNewEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbNewEntry, "tbNewEntry");
            this.tbNewEntry.Name = "tbNewEntry";
            this.tbNewEntry.Click += new System.EventHandler(this.tbNewEntry_Click);
            // 
            // tbDeleteEntry
            // 
            this.tbDeleteEntry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbDeleteEntry, "tbDeleteEntry");
            this.tbDeleteEntry.Name = "tbDeleteEntry";
            this.tbDeleteEntry.Click += new System.EventHandler(this.tbDeleteEntry_Click);
            // 
            // tbNewSet
            // 
            this.tbNewSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbNewSet, "tbNewSet");
            this.tbNewSet.Name = "tbNewSet";
            this.tbNewSet.Click += new System.EventHandler(this.tbNewSet_Click);
            // 
            // tbDeleteSet
            // 
            this.tbDeleteSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tbDeleteSet, "tbDeleteSet");
            this.tbDeleteSet.Name = "tbDeleteSet";
            this.tbDeleteSet.Click += new System.EventHandler(this.tbDeleteSet_Click);
            // 
            // expandGroupControl1
            // 
            this.expandGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("expandGroupControl1.AppearanceCaption.BackColor")));
            this.expandGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.expandGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.expandGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            resources.ApplyResources(this.expandGroupControl1, "expandGroupControl1");
            this.expandGroupControl1.Controls.Add(this.lvOutput);
            this.expandGroupControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.expandGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.expandGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.expandGroupControl1.Name = "expandGroupControl1";
            // 
            // lvOutput
            // 
            this.lvOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMessage,
            this.colObject,
            this.colObjectType});
            resources.ApplyResources(this.lvOutput, "lvOutput");
            this.lvOutput.FullRowSelect = true;
            this.lvOutput.GridLines = true;
            this.lvOutput.HideSelection = false;
            this.lvOutput.MultiSelect = false;
            this.lvOutput.Name = "lvOutput";
            this.lvOutput.SmallImageList = this.imgOutput;
            this.lvOutput.UseCompatibleStateImageBehavior = false;
            this.lvOutput.View = System.Windows.Forms.View.Details;
            this.lvOutput.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvOutput_MouseDoubleClick);
            // 
            // colMessage
            // 
            resources.ApplyResources(this.colMessage, "colMessage");
            // 
            // colObject
            // 
            resources.ApplyResources(this.colObject, "colObject");
            // 
            // colObjectType
            // 
            resources.ApplyResources(this.colObjectType, "colObjectType");
            // 
            // imgOutput
            // 
            this.imgOutput.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgOutput.ImageStream")));
            this.imgOutput.TransparentColor = System.Drawing.Color.Magenta;
            this.imgOutput.Images.SetKeyName(0, "Information");
            this.imgOutput.Images.SetKeyName(1, "Warning");
            this.imgOutput.Images.SetKeyName(2, "Error");
            // 
            // btnSuperSets
            // 
            resources.ApplyResources(this.btnSuperSets, "btnSuperSets");
            this.btnSuperSets.Name = "btnSuperSets";
            this.btnSuperSets.Click += new System.EventHandler(this.btnSuperSets_Click);
            // 
            // btnValidate
            // 
            resources.ApplyResources(this.btnValidate, "btnValidate");
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // validationProvider1
            // 
            this.validationProvider1.ErrorProvider = null;
            this.validationProvider1.RulesetName = "";
            this.validationProvider1.SourceTypeName = "BodyArchitect.Service.Model.TrainingPlans.TrainingPlan, BodyArchitect.Service.Mod" +
    "el";
            this.validationProvider1.ValidationPerformed += new System.EventHandler<Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationPerformedEventArgs>(this.validationProvider1_ValidationPerformed);
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
            this.validationProvider1.SetSourcePropertyName(this.luCountries, "Language");
            this.validationProvider1.SetValidatedProperty(this.luCountries, "EditValue");
            // 
            // txtUrl
            // 
            resources.ApplyResources(this.txtUrl, "txtUrl");
            this.txtUrl.Name = "txtUrl";
            this.validationProvider1.SetPerformValidation(this.txtUrl, true);
            this.validationProvider1.SetSourcePropertyName(this.txtUrl, "Url");
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.validationProvider1.SetPerformValidation(this.txtName, true);
            this.validationProvider1.SetSourcePropertyName(this.txtName, "Name");
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // cancelButton1
            // 
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.CausesValidation = false;
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            this.cancelButton1.Name = "cancelButton1";
            // 
            // okButton1
            // 
            resources.ApplyResources(this.okButton1, "okButton1");
            this.okButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton1.Image = ((System.Drawing.Image)(resources.GetObject("okButton1.Image")));
            this.okButton1.Name = "okButton1";
            this.okButton1.Click += new System.EventHandler(this.okButton1_Click);
            // 
            // grInformation
            // 
            resources.ApplyResources(this.grInformation, "grInformation");
            this.grInformation.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grInformation.AppearanceCaption.BackColor")));
            this.grInformation.AppearanceCaption.Options.UseBackColor = true;
            this.grInformation.AppearanceCaption.Options.UseTextOptions = true;
            this.grInformation.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grInformation.Controls.Add(this.lblSeconds);
            this.grInformation.Controls.Add(this.txtRestSeconds);
            this.grInformation.Controls.Add(this.cmbPurpose);
            this.grInformation.Controls.Add(this.labelControl1);
            this.grInformation.Controls.Add(this.luCountries);
            this.grInformation.Controls.Add(this.lblLanguage);
            this.grInformation.Controls.Add(this.txtUrl);
            this.grInformation.Controls.Add(this.lblUrl);
            this.grInformation.Controls.Add(this.lblRestSeconds);
            this.grInformation.Controls.Add(this.cmbDifficult);
            this.grInformation.Controls.Add(this.lblDifficult);
            this.grInformation.Controls.Add(this.cmbTrainingType);
            this.grInformation.Controls.Add(this.lblTrainingType);
            this.grInformation.Controls.Add(this.txtAuthor);
            this.grInformation.Controls.Add(this.lblAuthor);
            this.grInformation.Controls.Add(this.txtName);
            this.grInformation.Controls.Add(this.lblName);
            this.grInformation.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grInformation.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grInformation.Name = "grInformation";
            // 
            // lblSeconds
            // 
            resources.ApplyResources(this.lblSeconds, "lblSeconds");
            this.lblSeconds.Name = "lblSeconds";
            // 
            // txtRestSeconds
            // 
            resources.ApplyResources(this.txtRestSeconds, "txtRestSeconds");
            this.txtRestSeconds.Name = "txtRestSeconds";
            this.txtRestSeconds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // cmbPurpose
            // 
            resources.ApplyResources(this.cmbPurpose, "cmbPurpose");
            this.cmbPurpose.Name = "cmbPurpose";
            this.cmbPurpose.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbPurpose.Properties.Buttons"))))});
            this.cmbPurpose.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // labelControl1
            // 
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // lblLanguage
            // 
            resources.ApplyResources(this.lblLanguage, "lblLanguage");
            this.lblLanguage.Name = "lblLanguage";
            // 
            // lblUrl
            // 
            resources.ApplyResources(this.lblUrl, "lblUrl");
            this.lblUrl.Name = "lblUrl";
            // 
            // lblRestSeconds
            // 
            resources.ApplyResources(this.lblRestSeconds, "lblRestSeconds");
            this.lblRestSeconds.Name = "lblRestSeconds";
            // 
            // cmbDifficult
            // 
            resources.ApplyResources(this.cmbDifficult, "cmbDifficult");
            this.cmbDifficult.Name = "cmbDifficult";
            this.cmbDifficult.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbDifficult.Properties.Buttons"))))});
            this.cmbDifficult.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblDifficult
            // 
            resources.ApplyResources(this.lblDifficult, "lblDifficult");
            this.lblDifficult.Name = "lblDifficult";
            // 
            // cmbTrainingType
            // 
            resources.ApplyResources(this.cmbTrainingType, "cmbTrainingType");
            this.cmbTrainingType.Name = "cmbTrainingType";
            this.cmbTrainingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbTrainingType.Properties.Buttons"))))});
            this.cmbTrainingType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // lblTrainingType
            // 
            resources.ApplyResources(this.lblTrainingType, "lblTrainingType");
            this.lblTrainingType.Name = "lblTrainingType";
            // 
            // txtAuthor
            // 
            resources.ApplyResources(this.txtAuthor, "txtAuthor");
            this.txtAuthor.Name = "txtAuthor";
            // 
            // lblAuthor
            // 
            resources.ApplyResources(this.lblAuthor, "lblAuthor");
            this.lblAuthor.Name = "lblAuthor";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // usrTrainingPlanSerieEditor1
            // 
            resources.ApplyResources(this.usrTrainingPlanSerieEditor1, "usrTrainingPlanSerieEditor1");
            this.usrTrainingPlanSerieEditor1.Name = "usrTrainingPlanSerieEditor1";
            // 
            // UsrTrainingPlanEntryEditor1
            // 
            resources.ApplyResources(this.UsrTrainingPlanEntryEditor1, "UsrTrainingPlanEntryEditor1");
            this.UsrTrainingPlanEntryEditor1.Name = "UsrTrainingPlanEntryEditor1";
            // 
            // TrainingPlanEditorWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.btnSuperSets);
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.okButton1);
            this.Controls.Add(this.grInformation);
            this.Name = "TrainingPlanEditorWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrainingPlanEditorWindow_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
            this.splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grDescription)).EndInit();
            this.grDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grDetails)).EndInit();
            this.grDetails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl3)).EndInit();
            this.splitContainerControl3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.expandGroupControl1)).EndInit();
            this.expandGroupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.luCountries.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grInformation)).EndInit();
            this.grInformation.ResumeLayout(false);
            this.grInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestSeconds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPurpose.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDifficult.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbTrainingType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAuthor.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl grInformation;
        private DevExpress.XtraEditors.TextEdit txtAuthor;
        private DevExpress.XtraEditors.LabelControl lblAuthor;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private BodyArchitect.Controls.Basic.OKButton okButton1;
        private BodyArchitect.Controls.Basic.CancelButton cancelButton1;
        private DevExpress.XtraEditors.SimpleButton btnSuperSets;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTrainingType;
        private DevExpress.XtraEditors.LabelControl lblTrainingType;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDifficult;
        private DevExpress.XtraEditors.LabelControl lblDifficult;
        private DevExpress.XtraEditors.LabelControl lblRestSeconds;
        private ImageList imageList1;
        private BaGroupControl expandGroupControl1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private BaGroupControl grDescription;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private BaGroupControl grDetails;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl3;
        private TreeView tvDetails;
        private ToolStrip toolStrip1;
        private ToolStripButton tbMoveUp;
        private ToolStripButton tbMoveDown;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tbNewDay;
        private ToolStripButton tbEditDay;
        private ToolStripButton tbDeleteDay;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tbNewEntry;
        private ToolStripButton tbDeleteEntry;
        private ToolStripButton tbNewSet;
        private ToolStripButton tbDeleteSet;
        private usrTrainingPlanSerieEditor usrTrainingPlanSerieEditor1;
        private usrTrainingPlanEntryEditor UsrTrainingPlanEntryEditor1;
        private DevExpress.XtraEditors.SimpleButton btnValidate;
        private ListView lvOutput;
        private ColumnHeader colMessage;
        private ColumnHeader colObject;
        private ColumnHeader colObjectType;
        private ImageList imgOutput;
        private DevExpress.XtraEditors.TextEdit txtUrl;
        private DevExpress.XtraEditors.LabelControl lblUrl;
        private DevExpress.XtraEditors.LookUpEdit luCountries;
        private DevExpress.XtraEditors.LabelControl lblLanguage;
        private DevExpress.XtraEditors.ComboBoxEdit cmbPurpose;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WinForms.ValidationProvider validationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.LabelControl lblSeconds;
        private DevExpress.XtraEditors.SpinEdit txtRestSeconds;

    }
}