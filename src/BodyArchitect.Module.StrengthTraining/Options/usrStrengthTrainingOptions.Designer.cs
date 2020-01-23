namespace BodyArchitect.Module.StrengthTraining.Options
{
    partial class usrStrengthTrainingOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrStrengthTrainingOptions));
            this.chkFillRepetitionsNumberFromPlan = new DevExpress.XtraEditors.CheckEdit();
            this.chkExtendedSetsInfo = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFillRepetitionsNumberFromPlan.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExtendedSetsInfo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // chkFillRepetitionsNumberFromPlan
            // 
            resources.ApplyResources(this.chkFillRepetitionsNumberFromPlan, "chkFillRepetitionsNumberFromPlan");
            this.chkFillRepetitionsNumberFromPlan.Name = "chkFillRepetitionsNumberFromPlan";
            this.chkFillRepetitionsNumberFromPlan.Properties.AutoWidth = true;
            this.chkFillRepetitionsNumberFromPlan.Properties.Caption = resources.GetString("chkFillRepetitionsNumberFromPlan.Properties.Caption");
            // 
            // chkExtendedSetsInfo
            // 
            resources.ApplyResources(this.chkExtendedSetsInfo, "chkExtendedSetsInfo");
            this.chkExtendedSetsInfo.Name = "chkExtendedSetsInfo";
            this.chkExtendedSetsInfo.Properties.Appearance.Options.UseTextOptions = true;
            this.chkExtendedSetsInfo.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.chkExtendedSetsInfo.Properties.AutoHeight = ((bool)(resources.GetObject("checkEdit1.Properties.AutoHeight")));
            this.chkExtendedSetsInfo.Properties.Caption = resources.GetString("checkEdit1.Properties.Caption");
            // 
            // usrStrengthTrainingOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkExtendedSetsInfo);
            this.Controls.Add(this.chkFillRepetitionsNumberFromPlan);
            this.Name = "usrStrengthTrainingOptions";
            ((System.ComponentModel.ISupportInitialize)(this.chkFillRepetitionsNumberFromPlan.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkExtendedSetsInfo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.CheckEdit chkFillRepetitionsNumberFromPlan;
        private DevExpress.XtraEditors.CheckEdit chkExtendedSetsInfo;

    }
}
