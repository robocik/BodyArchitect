namespace BodyArchitect.Controls.UserControls
{
    partial class usrProfileInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrProfileInfoView));
            this.usrUserInformation1 = new BodyArchitect.Controls.UserControls.usrUserInformation();
            this.SuspendLayout();
            // 
            // usrUserInformation1
            // 
            resources.ApplyResources(this.usrUserInformation1, "usrUserInformation1");
            this.usrUserInformation1.Name = "usrUserInformation1";
            // 
            // usrProfileInfoView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.usrUserInformation1);
            this.Name = "usrProfileInfoView";
            this.ResumeLayout(false);

        }

        #endregion

        private usrUserInformation usrUserInformation1;




    }
}
