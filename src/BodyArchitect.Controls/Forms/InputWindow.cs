using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BodyArchitect.Controls.Forms
{
    public partial class InputWindow : DevExpress.XtraEditors.XtraForm
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        public string Value
        {
            get
            {
                return txtValue.Text;
            }
            set
            {
                txtValue.Text = value;
            }
        }

        [DefaultValue(0)]
        public int MaxLength
        {
            get { return txtValue.Properties.MaxLength; }
            set { txtValue.Properties.MaxLength=value; }
        }
    }
}