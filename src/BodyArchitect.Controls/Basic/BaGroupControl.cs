using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Shared;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.UserControls
{
    public class BaGroupControl : GroupControl
    {

        protected override void CreateHandle()
        {
            base.CreateHandle();

            this.Appearance.BackColor = Constants.PanelBackColor;
            this.Appearance.BackColor2 = Constants.PanelBackColor;
            this.Appearance.Options.UseBackColor = true;
            this.AppearanceCaption.BackColor = System.Drawing.Color.FromArgb(100, 135, 220);
            this.AppearanceCaption.Options.UseBackColor = true;
            this.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.AppearanceCaption.Options.UseTextOptions = true;
            this.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AppearanceObject Appearance
        {
            get
            {
                return base.Appearance;
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // BaGroupControl
            // 
            this.AppearanceCaption.Options.UseTextOptions = true;
            this.AppearanceCaption.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

    }
}
