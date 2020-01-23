using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.ComponentModel;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Basic
{
    public class CancelButton:SimpleButton
    {
        public CancelButton()
        {
            InitializeComponent();
            ControlHelper.AddSuperTip(this, Text, SuperTips.ButtonCancel);
        }


        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (!ServicesManager.IsDesignMode)
            {
                XtraForm form = ControlHelper.GetParentControl<XtraForm>(this);
                if (form != null)
                {
                    form.CancelButton = this;
                }
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CancelButton));
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            this.CausesValidation = false;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Image = global::BodyArchitect.Controls.Icons.Cancel;
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
                ControlHelper.AddSuperTip(this, Text, SuperTips.ButtonCancel);
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
