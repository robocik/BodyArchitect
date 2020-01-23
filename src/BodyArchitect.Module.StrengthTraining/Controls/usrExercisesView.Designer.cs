namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrExercisesView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrExercisesView));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.exercisesTree1 = new BodyArchitect.Module.StrengthTraining.Controls.ExercisesTree();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMore = new DevExpress.XtraEditors.SimpleButton();
            this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exercisesTree1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.exercisesTree1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // exercisesTree1
            // 
            this.exercisesTree1.AllowsAdd = false;
            resources.ApplyResources(this.exercisesTree1, "exercisesTree1");
            this.exercisesTree1.Images = null;
            this.exercisesTree1.MultiSelect = false;
            this.exercisesTree1.Name = "exercisesTree1";
            this.exercisesTree1.FillReqest += new System.EventHandler(this.exercisesTree1_FillReqest);
            this.exercisesTree1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.exercisesTree1_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMore);
            this.panel1.Controls.Add(this.progressIndicator1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btnMore
            // 
            resources.ApplyResources(this.btnMore, "btnMore");
            this.btnMore.Name = "btnMore";
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            // 
            // progressIndicator1
            // 
            resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
            this.progressIndicator1.Name = "progressIndicator1";
            this.progressIndicator1.Percentage = 0F;
            // 
            // usrExercisesView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrExercisesView";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.exercisesTree1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ExercisesTree exercisesTree1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnMore;
        private BodyArchitect.Controls.UserControls.ProgressIndicator progressIndicator1;
    }
}
