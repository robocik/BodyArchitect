namespace BodyArchitect.Controls.UserControls
{
    partial class usrUserSizes
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
            this.usrWymiaryEditor1 = new BodyArchitect.Controls.UserControls.usrWymiaryEditor();
            this.SuspendLayout();
            // 
            // usrWymiaryEditor1
            // 
            this.usrWymiaryEditor1.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.usrWymiaryEditor1.Appearance.Options.UseBackColor = true;
            this.usrWymiaryEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usrWymiaryEditor1.Location = new System.Drawing.Point(0, 0);
            this.usrWymiaryEditor1.Name = "usrWymiaryEditor1";
            this.usrWymiaryEditor1.ReadOnly = true;
            this.usrWymiaryEditor1.Size = new System.Drawing.Size(260, 223);
            this.usrWymiaryEditor1.TabIndex = 1;
            // 
            // usrUserSizes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.usrWymiaryEditor1);
            this.Name = "usrUserSizes";
            this.Size = new System.Drawing.Size(260, 223);
            this.ResumeLayout(false);

        }

        #endregion

        private usrWymiaryEditor usrWymiaryEditor1;
    }
}
