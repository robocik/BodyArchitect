namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrUserExercises
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrUserExercises));
            this.usrExercisesView1 = new BodyArchitect.Module.StrengthTraining.Controls.usrExercisesView();
            this.SuspendLayout();
            // 
            // usrExercisesView1
            // 
            resources.ApplyResources(this.usrExercisesView1, "usrExercisesView1");
            this.usrExercisesView1.Name = "usrExercisesView1";
            // 
            // usrUserExercises
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrExercisesView1);
            this.Name = "usrUserExercises";
            this.ResumeLayout(false);

        }

        #endregion

        private usrExercisesView usrExercisesView1;



    }
}
