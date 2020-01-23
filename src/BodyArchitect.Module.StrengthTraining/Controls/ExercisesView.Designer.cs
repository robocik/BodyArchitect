namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class ExercisesView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExercisesView));
            this.btnAddExercise = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteExercise = new DevExpress.XtraEditors.SimpleButton();
            this.defaultToolTipController1 = new DevExpress.Utils.DefaultToolTipController(this.components);
            this.exercisesTree1 = new BodyArchitect.Module.StrengthTraining.Controls.ExercisesTree();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.btnPublish = new DevExpress.XtraEditors.SimpleButton();
            this.btnMapper = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.exercisesTree1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddExercise
            // 
            resources.ApplyResources(this.btnAddExercise, "btnAddExercise");
            this.btnAddExercise.Name = "btnAddExercise";
            this.btnAddExercise.Click += new System.EventHandler(this.btnAddExercise_Click);
            // 
            // btnDeleteExercise
            // 
            resources.ApplyResources(this.btnDeleteExercise, "btnDeleteExercise");
            this.btnDeleteExercise.Name = "btnDeleteExercise";
            this.btnDeleteExercise.Click += new System.EventHandler(this.btnDeleteExercise_Click);
            // 
            // exercisesTree1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.exercisesTree1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("exercisesTree1.AllowHtmlText"))));
            this.exercisesTree1.AllowsAdd = true;
            resources.ApplyResources(this.exercisesTree1, "exercisesTree1");
            this.exercisesTree1.Images = null;
            this.exercisesTree1.MultiSelect = false;
            this.exercisesTree1.Name = "exercisesTree1";
            this.exercisesTree1.FillReqest += new System.EventHandler(this.exercisesTree1_FillReqest);
            this.exercisesTree1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.exercisesTree1_AfterSelect);
            // 
            // progressIndicator1
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this.progressIndicator1, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("progressIndicator1.AllowHtmlText"))));
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // btnPublish
            // 
            resources.ApplyResources(this.btnPublish, "btnPublish");
            this.btnPublish.Image = ((System.Drawing.Image)(resources.GetObject("btnPublish.Image")));
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // btnMapper
            // 
            resources.ApplyResources(this.btnMapper, "btnMapper");
            this.btnMapper.Name = "btnMapper";
            this.btnMapper.Click += new System.EventHandler(this.btnMapper_Click);
            // 
            // ExercisesView
            // 
            this.defaultToolTipController1.SetAllowHtmlText(this, ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("$this.AllowHtmlText"))));
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMapper);
            this.Controls.Add(this.progressIndicator1);
            this.Controls.Add(this.exercisesTree1);
            this.Controls.Add(this.btnPublish);
            this.Controls.Add(this.btnDeleteExercise);
            this.Controls.Add(this.btnAddExercise);
            this.Name = "ExercisesView";
            this.Load += new System.EventHandler(this.ExercisesView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.exercisesTree1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnAddExercise;
        private DevExpress.XtraEditors.SimpleButton btnDeleteExercise;
        private DevExpress.Utils.DefaultToolTipController defaultToolTipController1;
        private DevExpress.XtraEditors.SimpleButton btnPublish;
        private ExercisesTree exercisesTree1;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
        private DevExpress.XtraEditors.SimpleButton btnMapper;
        
    }
}