using BodyArchitect.Controls.UserControls;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrStrengthTraining
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrStrengthTraining));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.usrWorkoutPlansChooser1 = new BodyArchitect.Module.StrengthTraining.Controls.usrWorkoutPlansChooser();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.usrReportStatus1 = new BodyArchitect.Controls.UserControls.usrReportStatus();
            this.usrIntensity1 = new BodyArchitect.Module.StrengthTraining.Controls.usrIntensity();
            this.grInfo = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.txtComment = new DevExpress.XtraEditors.MemoEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblTreningTime = new DevExpress.XtraEditors.LabelControl();
            this.teEndTime = new DevExpress.XtraEditors.TimeEdit();
            this.lblStart = new DevExpress.XtraEditors.LabelControl();
            this.lblEndTime = new DevExpress.XtraEditors.LabelControl();
            this.teStart = new DevExpress.XtraEditors.TimeEdit();
            this.usrTrainingDaySourceGrid1 = new BodyArchitect.Module.StrengthTraining.Controls.usrTrainingDaySourceGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grInfo)).BeginInit();
            this.grInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teStart.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.usrWorkoutPlansChooser1);
            this.splitContainerControl1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainerControl1.Panel1.Controls.Add(this.grInfo);
            this.splitContainerControl1.Panel1.Controls.Add(this.labelControl1);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblTreningTime);
            this.splitContainerControl1.Panel1.Controls.Add(this.teEndTime);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblStart);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblEndTime);
            this.splitContainerControl1.Panel1.Controls.Add(this.teStart);
            resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
            this.splitContainerControl1.Panel2.Controls.Add(this.usrTrainingDaySourceGrid1);
            resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
            this.splitContainerControl1.SplitterPosition = 201;
            // 
            // usrWorkoutPlansChooser1
            // 
            resources.ApplyResources(this.usrWorkoutPlansChooser1, "usrWorkoutPlansChooser1");
            this.usrWorkoutPlansChooser1.Name = "usrWorkoutPlansChooser1";
            this.usrWorkoutPlansChooser1.SelectedPlanDayChanged += new System.EventHandler(this.usrWorkoutPlansChooser1_SelectedPlanDayChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.usrReportStatus1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.usrIntensity1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // usrReportStatus1
            // 
            resources.ApplyResources(this.usrReportStatus1, "usrReportStatus1");
            this.usrReportStatus1.Name = "usrReportStatus1";
            this.usrReportStatus1.ReadOnly = false;
            // 
            // usrIntensity1
            // 
            resources.ApplyResources(this.usrIntensity1, "usrIntensity1");
            this.usrIntensity1.Intensity = BodyArchitect.Service.Model.Intensity.NotSet;
            this.usrIntensity1.Name = "usrIntensity1";
            this.usrIntensity1.ReadOnly = false;
            // 
            // grInfo
            // 
            resources.ApplyResources(this.grInfo, "grInfo");
            this.grInfo.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("grInfo.AppearanceCaption.BackColor")));
            this.grInfo.AppearanceCaption.Options.UseBackColor = true;
            this.grInfo.AppearanceCaption.Options.UseTextOptions = true;
            this.grInfo.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.grInfo.Controls.Add(this.txtComment);
            this.grInfo.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.grInfo.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grInfo.Name = "grInfo";
            // 
            // txtComment
            // 
            resources.ApplyResources(this.txtComment, "txtComment");
            this.txtComment.Name = "txtComment";
            // 
            // labelControl1
            // 
            resources.ApplyResources(this.labelControl1, "labelControl1");
            this.labelControl1.Name = "labelControl1";
            // 
            // lblTreningTime
            // 
            this.lblTreningTime.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblTreningTime.Appearance.Font")));
            resources.ApplyResources(this.lblTreningTime, "lblTreningTime");
            this.lblTreningTime.Name = "lblTreningTime";
            // 
            // teEndTime
            // 
            resources.ApplyResources(this.teEndTime, "teEndTime");
            this.teEndTime.Name = "teEndTime";
            this.teEndTime.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.teEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.teEndTime.Properties.Mask.EditMask = resources.GetString("teEndTime.Properties.Mask.EditMask");
            // 
            // lblStart
            // 
            resources.ApplyResources(this.lblStart, "lblStart");
            this.lblStart.Name = "lblStart";
            // 
            // lblEndTime
            // 
            resources.ApplyResources(this.lblEndTime, "lblEndTime");
            this.lblEndTime.Name = "lblEndTime";
            // 
            // teStart
            // 
            resources.ApplyResources(this.teStart, "teStart");
            this.teStart.Name = "teStart";
            this.teStart.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.teStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.teStart.Properties.Mask.EditMask = resources.GetString("teStart.Properties.Mask.EditMask");
            // 
            // usrTrainingDaySourceGrid1
            // 
            resources.ApplyResources(this.usrTrainingDaySourceGrid1, "usrTrainingDaySourceGrid1");
            this.usrTrainingDaySourceGrid1.Name = "usrTrainingDaySourceGrid1";
            // 
            // usrStrengthTraining
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "usrStrengthTraining";
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grInfo)).EndInit();
            this.grInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teStart.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTreningTime;
        private DevExpress.XtraEditors.TimeEdit teEndTime;
        private DevExpress.XtraEditors.LabelControl lblEndTime;
        private DevExpress.XtraEditors.TimeEdit teStart;
        private DevExpress.XtraEditors.LabelControl lblStart;
        private DevExpress.XtraEditors.MemoEdit txtComment;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private usrTrainingDaySourceGrid usrTrainingDaySourceGrid1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private BaGroupControl grInfo;
        private usrIntensity usrIntensity1;
        private BodyArchitect.Controls.UserControls.usrReportStatus usrReportStatus1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private usrWorkoutPlansChooser usrWorkoutPlansChooser1;
    }
}
