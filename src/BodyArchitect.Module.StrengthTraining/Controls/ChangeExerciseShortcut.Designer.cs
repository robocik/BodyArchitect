using BodyArchitect.Controls.Basic;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class ChangeExerciseShortcut
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeExerciseShortcut));
            this.usrExerciseEditor1 = new BodyArchitect.Module.StrengthTraining.Controls.usrExerciseEditor();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.okButton1 = new BodyArchitect.Controls.Basic.OKButton();
            this.cancelButton1 = new BodyArchitect.Controls.Basic.CancelButton();
            this.SuspendLayout();
            // 
            // usrExerciseEditor1
            // 
            resources.ApplyResources(this.usrExerciseEditor1, "usrExerciseEditor1");
            this.usrExerciseEditor1.Name = "usrExerciseEditor1";
            this.usrExerciseEditor1.ControlValidated += new System.EventHandler<BodyArchitect.Controls.ControlValidatedEventArgs>(this.usrExerciseEditor1_ControlValidated);
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.Appearance.DisabledImage = ((System.Drawing.Image)(resources.GetObject("lblMessage.Appearance.DisabledImage")));
            this.lblMessage.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblMessage.Appearance.ForeColor")));
            this.lblMessage.Appearance.GradientMode = ((System.Drawing.Drawing2D.LinearGradientMode)(resources.GetObject("lblMessage.Appearance.GradientMode")));
            this.lblMessage.Appearance.HoverImage = ((System.Drawing.Image)(resources.GetObject("lblMessage.Appearance.HoverImage")));
            this.lblMessage.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("lblMessage.Appearance.Image")));
            this.lblMessage.Appearance.PressedImage = ((System.Drawing.Image)(resources.GetObject("lblMessage.Appearance.PressedImage")));
            this.lblMessage.Name = "lblMessage";
            // 
            // okButton1
            // 
            resources.ApplyResources(this.okButton1, "okButton1");
            this.okButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton1.Image = ((System.Drawing.Image)(resources.GetObject("okButton1.Image")));
            this.okButton1.Name = "okButton1";
            this.okButton1.Click += new System.EventHandler(this.okButton1_Click);
            // 
            // cancelButton1
            // 
            resources.ApplyResources(this.cancelButton1, "cancelButton1");
            this.cancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton1.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton1.Image")));
            this.cancelButton1.Name = "cancelButton1";
            // 
            // ChangeExerciseShortcut
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cancelButton1);
            this.Controls.Add(this.okButton1);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.usrExerciseEditor1);
            this.Name = "ChangeExerciseShortcut";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private usrExerciseEditor usrExerciseEditor1;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private OKButton okButton1;
        private CancelButton cancelButton1;
    }
}