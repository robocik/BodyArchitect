using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class AddExercise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddExercise));
            this.usrExerciseEditor1 = new BodyArchitect.Module.StrengthTraining.Controls.usrExerciseEditor();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.SuspendLayout();
            // 
            // usrExerciseEditor1
            // 
            this.usrExerciseEditor1.AllowRedirectToDetails = false;
            resources.ApplyResources(this.usrExerciseEditor1, "usrExerciseEditor1");
            this.usrExerciseEditor1.Name = "usrExerciseEditor1";
            this.usrExerciseEditor1.ControlValidated += new System.EventHandler<BodyArchitect.Controls.ControlValidatedEventArgs>(this.usrExerciseEditor1_ControlValidated);
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.okButton1_Click);
            // 
            // AddExercise
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.Controls.Add(this.usrExerciseEditor1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddExercise";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private usrExerciseEditor usrExerciseEditor1;
        private BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;

    }
}