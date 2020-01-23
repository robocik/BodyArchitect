using BodyArchitect.Controls.ProgressIndicator;

namespace BodyArchitect.Controls.Forms
{
    partial class SendMessageWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendMessageWindow));
            this.usrMessageView1 = new BodyArchitect.Controls.UserControls.usrMessageView();
            this.usrProgressIndicatorButtons1 = new BodyArchitect.Controls.ProgressIndicator.usrProgressIndicatorButtons();
            this.SuspendLayout();
            // 
            // usrMessageView1
            // 
            this.usrMessageView1.AllowRedirectToDetails = false;
            resources.ApplyResources(this.usrMessageView1, "usrMessageView1");
            this.usrMessageView1.Name = "usrMessageView1";
            this.usrMessageView1.ControlValidated += new System.EventHandler<BodyArchitect.Controls.ControlValidatedEventArgs>(this.usrMessageView1_ControlValidated);
            // 
            // usrProgressIndicatorButtons1
            // 
            resources.ApplyResources(this.usrProgressIndicatorButtons1, "usrProgressIndicatorButtons1");
            this.usrProgressIndicatorButtons1.CausesValidation = false;
            this.usrProgressIndicatorButtons1.Name = "usrProgressIndicatorButtons1";
            this.usrProgressIndicatorButtons1.OkClick += new System.EventHandler<BodyArchitect.Controls.ProgressIndicator.CancellationSourceEventArgs>(this.usrProgressIndicatorButtons1_OkClick);
            // 
            // SendMessageWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrProgressIndicatorButtons1);
            this.Controls.Add(this.usrMessageView1);
            this.Name = "SendMessageWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.usrMessageView usrMessageView1;
        private ProgressIndicator.usrProgressIndicatorButtons usrProgressIndicatorButtons1;
    }
}