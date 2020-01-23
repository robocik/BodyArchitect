using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Controls.UserControls;

namespace BodyArchitect.Controls.ProgressIndicator
{
    public partial class ProgressIndicatorStatus : UserControl
    {
        

        public ProgressIndicatorStatus()
        {
            InitializeComponent();
            //progressIndicator1.AutoStart = true;
        }

        
        public bool ShowProgress
        {
            get { return progressIndicator1.Visible; }
            set
            {
                progressIndicator1.Visible=value;
                if(value)
                {
                    progressIndicator1.Start();
                }
                else
                {
                    progressIndicator1.Stop();
                }
            }
        }

        public string Message
        {
            get { return labelControl1.Text; }
            set { labelControl1.Text = value; }
        }
    }
}
