using System;
using BodyArchitect.Service.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Module.StrengthTraining.Model;

namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrIntensity : DevExpress.XtraEditors.XtraUserControl
    {
        public usrIntensity()
        {
            InitializeComponent();
            updateLabels();
        }

        public Intensity Intensity
        {
            get { return (Intensity) ztbIntensive.Value; }
            set 
            {
                ztbIntensive.Value =(int) value;
                updateLabels();
            }
        }

        public bool ReadOnly
        {
            get { return ztbIntensive.Properties.ReadOnly; }
            set { ztbIntensive.Properties.ReadOnly=value; }
        }

        private void ztbIntensive_EditValueChanged(object sender, EventArgs e)
        {

            updateLabels();
        }

        void updateLabels()
        {
            lblNotSet.Visible = ((Intensity)ztbIntensive.Value) == Intensity.NotSet;
            lblLow.Visible = ((Intensity)ztbIntensive.Value) == Intensity.Low;
            lblMedium.Visible = ((Intensity)ztbIntensive.Value) ==Intensity.Medium;
            lblHight.Visible = ((Intensity)ztbIntensive.Value) == Intensity.Hight;
        }
    }
}
