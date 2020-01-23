using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class PublishWorkoutPlanWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PublishWorkoutPlanWindow));
            this.lblPublishDescription = new DevExpress.XtraEditors.LabelControl();
            this.baGroupControl1 = new BodyArchitect.Controls.UserControls.BaGroupControl();
            this.exercisesListView1 = new BodyArchitect.Module.StrengthTraining.Controls.ExercisesListView();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.lblAddToFavoriteDescription = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).BeginInit();
            this.baGroupControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPublishDescription
            // 
            resources.ApplyResources(this.lblPublishDescription, "lblPublishDescription");
            this.lblPublishDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblPublishDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblPublishDescription.Name = "lblPublishDescription";
            // 
            // baGroupControl1
            // 
            resources.ApplyResources(this.baGroupControl1, "baGroupControl1");
            this.baGroupControl1.AppearanceCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("baGroupControl1.AppearanceCaption.BackColor")));
            this.baGroupControl1.AppearanceCaption.Options.UseBackColor = true;
            this.baGroupControl1.AppearanceCaption.Options.UseTextOptions = true;
            this.baGroupControl1.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            this.baGroupControl1.Controls.Add(this.exercisesListView1);
            this.baGroupControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.baGroupControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baGroupControl1.Name = "baGroupControl1";
            // 
            // exercisesListView1
            // 
            resources.ApplyResources(this.exercisesListView1, "exercisesListView1");
            this.exercisesListView1.FullRowSelect = true;
            this.exercisesListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.exercisesListView1.MultiSelect = false;
            this.exercisesListView1.Name = "exercisesListView1";
            this.exercisesListView1.UseCompatibleStateImageBehavior = false;
            this.exercisesListView1.View = System.Windows.Forms.View.Details;
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.usrProgressIndicatorButtons1_OkClick);
            this.usrProgressIndicatorButtons1.CancelClick += new System.EventHandler(this.usrProgressIndicatorButtons1_CancelClick);
            // 
            // lblAddToFavoriteDescription
            // 
            resources.ApplyResources(this.lblAddToFavoriteDescription, "lblAddToFavoriteDescription");
            this.lblAddToFavoriteDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblAddToFavoriteDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblAddToFavoriteDescription.Name = "lblAddToFavoriteDescription";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblPublishDescription);
            this.panel1.Controls.Add(this.lblAddToFavoriteDescription);
            this.panel1.Name = "panel1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // PublishWorkoutPlanWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.Controls.Add(this.baGroupControl1);
            this.Name = "PublishWorkoutPlanWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PublishWorkoutPlanWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.baGroupControl1)).EndInit();
            this.baGroupControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblPublishDescription;
        private BodyArchitect.Controls.UserControls.BaGroupControl baGroupControl1;
        private BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
        private ExercisesListView exercisesListView1;
        private DevExpress.XtraEditors.LabelControl lblAddToFavoriteDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}