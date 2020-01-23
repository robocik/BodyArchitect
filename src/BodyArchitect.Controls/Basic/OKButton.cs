using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using BodyArchitect.Common;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Basic
{
    public class OKButton:SimpleButton
    {
        public OKButton()
        {
            InitializeComponent();
            ControlHelper.AddSuperTip(this, Text, SuperTips.ButtonOK);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            
            if (!ServicesManager.IsDesignMode)
            {
                XtraForm form = ControlHelper.GetParentControl<XtraForm>(this);
                if (form != null)
                {
                    form.AcceptButton = this;
                }
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OKButton));
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Image = global::BodyArchitect.Controls.Icons.Ok;
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
