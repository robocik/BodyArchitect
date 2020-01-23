namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class ExerciseMapperWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExerciseMapperWindow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.grid1 = new SourceGrid.Grid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.lblShowExerciseStyle = new DevExpress.XtraEditors.LabelControl();
            this.dtaTo = new DevExpress.XtraEditors.DateEdit();
            this.lblToDate = new DevExpress.XtraEditors.LabelControl();
            this.dtaFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.cmbExerciseViewType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExerciseViewType.Properties)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.grid1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.usrProgressIndicatorButtons1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.usrProgressIndicatorButtons1_OkClick);
            // 
            // grid1
            // 
            resources.ApplyResources(this.grid1, "grid1");
            this.grid1.EnableSort = true;
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.baGroupControl1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // baGroupControl1
            // 
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.Controls.Add(this.lblShowExerciseStyle);
            this.baGroupControl1.Controls.Add(this.dtaTo);
            this.baGroupControl1.Controls.Add(this.lblToDate);
            this.baGroupControl1.Controls.Add(this.dtaFrom);
            this.baGroupControl1.Controls.Add(this.lblFrom);
            this.baGroupControl1.Controls.Add(this.cmbExerciseViewType);
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // lblShowExerciseStyle
            // 
            resources.ApplyResources(this.lblShowExerciseStyle, "lblShowExerciseStyle");
            this.lblShowExerciseStyle.Name = "lblShowExerciseStyle";
            // 
            // dtaTo
            // 
            resources.ApplyResources(this.dtaTo, "dtaTo");
            this.dtaTo.Name = "dtaTo";
            this.dtaTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaTo.Properties.Buttons"))))});
            this.dtaTo.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblToDate
            // 
            resources.ApplyResources(this.lblToDate, "lblToDate");
            this.lblToDate.Name = "lblToDate";
            // 
            // dtaFrom
            // 
            resources.ApplyResources(this.dtaFrom, "dtaFrom");
            this.dtaFrom.Name = "dtaFrom";
            this.dtaFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("dtaFrom.Properties.Buttons"))))});
            this.dtaFrom.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            // 
            // lblFrom
            // 
            resources.ApplyResources(this.lblFrom, "lblFrom");
            this.lblFrom.Name = "lblFrom";
            // 
            // cmbExerciseViewType
            // 
            resources.ApplyResources(this.cmbExerciseViewType, "cmbExerciseViewType");
            this.cmbExerciseViewType.Name = "cmbExerciseViewType";
            this.cmbExerciseViewType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbExerciseViewType.Properties.Buttons"))))});
            this.cmbExerciseViewType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblDescription, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // lblDescription
            // 
            this.lblDescription.AllowHtmlString = true;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // ExerciseMapperWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ExerciseMapperWindow";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.baGroupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtaFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbExerciseViewType.Properties)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.Panel panel2;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private DevExpress.XtraEditors.LabelControl lblShowExerciseStyle;
        private DevExpress.XtraEditors.DateEdit dtaTo;
        private DevExpress.XtraEditors.LabelControl lblToDate;
        private DevExpress.XtraEditors.DateEdit dtaFrom;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.ComboBoxEdit cmbExerciseViewType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl lblDescription;
    }
}