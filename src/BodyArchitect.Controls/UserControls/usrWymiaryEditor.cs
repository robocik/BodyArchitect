using System.ComponentModel;
using BodyArchitect.Service.Model;
using BodyArchitect.Settings.Model;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrWymiaryEditor : DevExpress.XtraEditors.XtraUserControl
    {
        private bool readOnly;


        public usrWymiaryEditor()
        {
            InitializeComponent();
            fillSuperTips();
            setMeasurementsType();
        }

        void setMeasurementsType()
        {
            if(Settings.Settings1.Default.WeightType==(int)WeightType.Pounds)
            {
                lblWeightType.Text =ApplicationStrings.WeightType_Pound;
            }
            if (Settings.Settings1.Default.LengthType == (int)LengthType.Inchs)
            {
                lblAbsType.Text =lblChestType.Text=lblHeightType.Text=
                    lblLeftBicepsType.Text=lblLeftForearmType.Text=lblLeftThighType.Text= 
                    lblRightBicepsType.Text=lblRightForearmType.Text=lblRightThighType.Text=ApplicationStrings.LengthType_Inch;
            }
        }


        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                updateReadOnly();
            }
        }

        void updateReadOnly()
        {
            txtWeight.Properties.ReadOnly = ReadOnly;
            txtRightBiceps.Properties.ReadOnly = ReadOnly;
            txtLeftBiceps.Properties.ReadOnly = ReadOnly;
            txtKlata.Properties.ReadOnly = ReadOnly;
            txtPas.Properties.ReadOnly = ReadOnly;
            txtRightLeg.Properties.ReadOnly = ReadOnly;
            txtLeftLeg.Properties.ReadOnly = ReadOnly;
            txtRightForearm.Properties.ReadOnly = ReadOnly;
            txtLeftForearm.Properties.ReadOnly = ReadOnly;
            txtHeight.Properties.ReadOnly = ReadOnly;
            chkNaCzczo.Properties.ReadOnly = ReadOnly;
            txtTime.Properties.ReadOnly = ReadOnly;
        }
        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(txtWeight, lblWeight.Text, SuperTips.WymiaryEditor_Weight);
            ControlHelper.AddSuperTip(txtRightLeg, lblRightUdo.Text, SuperTips.WymiaryEditor_RightLeg);
            ControlHelper.AddSuperTip(txtLeftLeg, lblLeftLeg.Text, SuperTips.WymiaryEditor_LeftLeg);
            ControlHelper.AddSuperTip(txtTime, lblTime.Text, SuperTips.WymiaryEditor_Time);
            ControlHelper.AddSuperTip(txtRightBiceps, lblRightBiceps.Text, SuperTips.WymiaryEditor_RightBiceps);
            ControlHelper.AddSuperTip(txtLeftBiceps, lblLeftBiceps.Text, SuperTips.WymiaryEditor_LeftBiceps);
            ControlHelper.AddSuperTip(txtPas, lblPas.Text, SuperTips.WymiaryEditor_Pas);
            ControlHelper.AddSuperTip(txtKlata, lblKlata.Text, SuperTips.WymiaryEditor_Klatka);
            ControlHelper.AddSuperTip(chkNaCzczo, chkNaCzczo.Text, SuperTips.WymiaryEditor_NaCzczo);
            ControlHelper.AddSuperTip(txtRightForearm, lblRightForearm.Text, SuperTips.WymiaryEditor_RightForearm);
            ControlHelper.AddSuperTip(txtLeftForearm, lblLeftForearm.Text, SuperTips.WymiaryEditor_LeftForearm);
            ControlHelper.AddSuperTip(txtHeight, lblHeight.Text, SuperTips.WymiaryEditor_Height);
        }

        public void Fill(WymiaryDTO wymiary)
        {
            if (wymiary != null)
            {
                txtWeight.Value = (decimal) wymiary.Weight.ToDisplayWeight();
                txtRightBiceps.Value = (decimal)wymiary.RightBiceps.ToDisplayLength();
                txtLeftBiceps.Value = (decimal)wymiary.LeftBiceps.ToDisplayLength();
                txtKlata.Value = (decimal)wymiary.Klatka.ToDisplayLength(); ;
                txtPas.Value = (decimal)wymiary.Pas.ToDisplayLength(); ;
                txtRightLeg.Value = (decimal)wymiary.RightUdo.ToDisplayLength(); ;
                txtLeftLeg.Value = (decimal)wymiary.LeftUdo.ToDisplayLength(); ;
                txtRightForearm.Value = (decimal)wymiary.RightForearm.ToDisplayLength(); ;
                txtLeftForearm.Value = (decimal)wymiary.LeftForearm.ToDisplayLength(); ;
                txtHeight.Value = (decimal)((float)wymiary.Height).ToDisplayLength(); 
                chkNaCzczo.Checked = wymiary.IsNaCzczo;
                txtTime.Time = wymiary.DateTime;
            }
            updateReadOnly();
        }

        public WymiaryDTO SaveWymiary(WymiaryDTO wymiary)
        {
            if (wymiary==null)
            {
                wymiary=new WymiaryDTO();
            }
            wymiary.Weight = (float)txtWeight.Value.ToSaveWeight();
            wymiary.RightBiceps = (float)txtRightBiceps.Value.ToSaveLength();
            wymiary.LeftBiceps = (float)txtLeftBiceps.Value.ToSaveLength();
            wymiary.Klatka = (float)txtKlata.Value.ToSaveLength();
            wymiary.Pas = (float)txtPas.Value.ToSaveLength();
            wymiary.RightUdo = (float)txtRightLeg.Value.ToSaveLength();
            wymiary.LeftUdo = (float)txtLeftLeg.Value.ToSaveLength();
            wymiary.RightForearm = (float)txtRightForearm.Value.ToSaveLength();
            wymiary.LeftForearm = (float)txtLeftForearm.Value.ToSaveLength();
            wymiary.Height = (int)txtHeight.Value.ToSaveLength();
            wymiary.IsNaCzczo = chkNaCzczo.Checked;
            wymiary.DateTime = txtTime.Time;
            return wymiary;
        }

        
    }
}
