using System;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    partial class usrIntensity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrIntensity));
            this.groupControl1 = new BaGroupControl();
            this.lblNotSet = new DevExpress.XtraEditors.LabelControl();
            this.lblLow = new DevExpress.XtraEditors.LabelControl();
            this.lblMedium = new DevExpress.XtraEditors.LabelControl();
            this.lblHight = new DevExpress.XtraEditors.LabelControl();
            this.ztbIntensive = new DevExpress.XtraEditors.ZoomTrackBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ztbIntensive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ztbIntensive.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            resources.ApplyResources(this.groupControl1, "groupControl1");
            this.groupControl1.Controls.Add(this.lblNotSet);
            this.groupControl1.Controls.Add(this.lblLow);
            this.groupControl1.Controls.Add(this.lblMedium);
            this.groupControl1.Controls.Add(this.lblHight);
            this.groupControl1.Controls.Add(this.ztbIntensive);
            this.groupControl1.Name = "groupControl1";
            // 
            // lblNotSet
            // 
            resources.ApplyResources(this.lblNotSet, "lblNotSet");
            this.lblNotSet.Name = "lblNotSet";
            // 
            // lblLow
            // 
            resources.ApplyResources(this.lblLow, "lblLow");
            this.lblLow.Appearance.ForeColor = System.Drawing.Color.Green;
            this.lblLow.Appearance.Options.UseForeColor = true;
            this.lblLow.Name = "lblLow";
            // 
            // lblMedium
            // 
            resources.ApplyResources(this.lblMedium, "lblMedium");
            this.lblMedium.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMedium.Appearance.Options.UseForeColor = true;
            this.lblMedium.Name = "lblMedium";
            // 
            // lblHight
            // 
            resources.ApplyResources(this.lblHight, "lblHight");
            this.lblHight.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblHight.Appearance.Options.UseForeColor = true;
            this.lblHight.Name = "lblHight";
            // 
            // ztbIntensive
            // 
            resources.ApplyResources(this.ztbIntensive, "ztbIntensive");
            this.ztbIntensive.Name = "ztbIntensive";
            this.ztbIntensive.Properties.AccessibleDescription = resources.GetString("ztbIntensive.Properties.AccessibleDescription");
            this.ztbIntensive.Properties.AccessibleName = resources.GetString("ztbIntensive.Properties.AccessibleName");
            this.ztbIntensive.Properties.Maximum = 3;
            this.ztbIntensive.Properties.Orientation = ((System.Windows.Forms.Orientation)(resources.GetObject("ztbIntensive.Properties.Orientation")));
            this.ztbIntensive.Properties.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            this.ztbIntensive.EditValueChanged += new System.EventHandler(this.ztbIntensive_EditValueChanged);
            // 
            // usrIntensity
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupControl1);
            this.Name = "usrIntensity";
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ztbIntensive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ztbIntensive)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BaGroupControl groupControl1;
        private DevExpress.XtraEditors.LabelControl lblNotSet;
        private DevExpress.XtraEditors.LabelControl lblLow;
        private DevExpress.XtraEditors.LabelControl lblMedium;
        private DevExpress.XtraEditors.LabelControl lblHight;
        private DevExpress.XtraEditors.ZoomTrackBarControl ztbIntensive;
    }
}
