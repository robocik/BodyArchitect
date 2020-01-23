using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Reporting
{
    public partial class usrDateRange : XtraUserControl
    {
        public usrDateRange()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.dtaFrom, lblFrom.Text, SuperTips.usrDateRange_DateFrom );
            ControlHelper.AddSuperTip(this.dtaTo, lblTo.Text, SuperTips.usrDateRange_DateTo);
        }

        public DateTime? DateFrom
        {
            get
            {
                if(dtaFrom.DateTime !=DateTime.MinValue)
                {
                    return dtaFrom.DateTime;
                }
                return null;
            }
        }

        public DateTime? DateTo
        {
            get
            {
                if (dtaTo.DateTime != DateTime.MinValue)
                {
                    return dtaTo.DateTime;
                }
                return null;
            }
        }
    }
}
