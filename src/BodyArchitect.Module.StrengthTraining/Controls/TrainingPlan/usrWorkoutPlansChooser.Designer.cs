namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrWorkoutPlansChooser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrWorkoutPlansChooser));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.luTrainingPlans = new DevExpress.XtraEditors.LookUpEdit();
            this.btnShowTrainingPlan = new DevExpress.XtraEditors.SimpleButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.luTrainingPlans.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.luTrainingPlans);
            this.flowLayoutPanel1.Controls.Add(this.btnShowTrainingPlan);
            this.flowLayoutPanel1.Controls.Add(this.progressIndicator1);
            this.flowLayoutPanel1.Controls.Add(this.btnRefresh);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // luTrainingPlans
            // 
            resources.ApplyResources(this.luTrainingPlans, "luTrainingPlans");
            this.luTrainingPlans.Name = "luTrainingPlans";
            this.luTrainingPlans.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
            this.luTrainingPlans.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("luTrainingPlans.Properties.Buttons"))))});
            this.luTrainingPlans.Properties.DropDownRows = 15;
            this.luTrainingPlans.Properties.NullText = resources.GetString("luTrainingPlans.Properties.NullText");
            this.luTrainingPlans.Properties.PopupWidth = 600;
            this.luTrainingPlans.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luTrainingPlans_QueryPopUp);
            this.luTrainingPlans.EditValueChanged += new System.EventHandler(this.luTrainingPlans_EditValueChanged);
            // 
            // btnShowTrainingPlan
            // 
            this.btnShowTrainingPlan.Image = ((System.Drawing.Image)(resources.GetObject("btnShowTrainingPlan.Image")));
            resources.ApplyResources(this.btnShowTrainingPlan, "btnShowTrainingPlan");
            this.btnShowTrainingPlan.Name = "btnShowTrainingPlan";
            this.btnShowTrainingPlan.Click += new System.EventHandler(this.btnShowTrainingPlan_Click);
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // usrWorkoutPlansChooser
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "usrWorkoutPlansChooser";
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.luTrainingPlans.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnShowTrainingPlan;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.LookUpEdit luTrainingPlans;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
    }
}
