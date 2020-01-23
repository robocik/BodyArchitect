using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Basic
{
    public class DeleteButton:SimpleButton
    {
        public DeleteButton()
        {
            InitializeComponent();
            ControlHelper.AddSuperTip(this, Text, SuperTips.ButtonDelete);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteButton));
            this.SuspendLayout();
            // 
            // DeleteButton
            // 
            this.Appearance.Options.UseImage = true;
            this.Image = ((System.Drawing.Image)(resources.GetObject("$this.Image")));
            this.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override SuperToolTip SuperTip
        {
            get
            {
                return base.SuperTip;
            }
            set
            {
                base.SuperTip = value;
            }
        }
    }
}
