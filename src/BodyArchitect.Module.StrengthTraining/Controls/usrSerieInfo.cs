using System;
using BodyArchitect.Controls;
using BodyArchitect.Controls.Localization;
using BodyArchitect.Module.StrengthTraining.Localization;
using BodyArchitect.Module.StrengthTraining.Model;
using BodyArchitect.Service.Model;


namespace BodyArchitect.Module.StrengthTraining.Controls
{
    public partial class usrSerieInfo : DevExpress.XtraEditors.XtraUserControl
    {
        private bool readOnly;

        public usrSerieInfo()
        {
            InitializeComponent();
            fillSuperTips();

            foreach (var value in Enum.GetValues(typeof(SetType)))
            {
                ComboBoxItem item = new ComboBoxItem(value, EnumLocalizer.Default.Translate((SetType)value));
                cmbSetType.Properties.Items.Add(item);
            }
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set 
            { 
                readOnly=value;
                updateReadOnly();
            }
        }

        void updateReadOnly()
        {
            txtComment.Properties.ReadOnly = ReadOnly;
            cmbSetType.Properties.ReadOnly = ReadOnly;
            chkCiezarBezSztangi.Properties.ReadOnly = ReadOnly;
            cmbDropSet.Properties.ReadOnly = ReadOnly;
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtComment, lblComment.Text, StrengthTrainingEntryStrings.SerieInfo_CommentTE);
            //ControlHelper.AddSuperTip(this.chkWithHelp, chkWithHelp.Text, StrengthTrainingEntryStrings.SerieInfo_LastRepetitionWithHelpCHK);
            ControlHelper.AddSuperTip(this.chkCiezarBezSztangi, chkCiezarBezSztangi.Text, StrengthTrainingEntryStrings.SerieInfo_CiezarBezSztangiCHK);
            ControlHelper.AddSuperTip(this.cmbDropSet, lblDropSet.Text, StrengthTrainingEntryStrings.SerieInfo_DropSetCMB);
        }

        public void Fill(SerieDTO serie)
        {
            txtComment.Text = serie.Comment;
            cmbSetType.SelectedIndex =(int) serie.SetType;
            chkCiezarBezSztangi.Checked = serie.IsCiezarBezSztangi;
            cmbDropSet.SelectedIndex = (int)serie.DropSet;
            if(serie.IsCardio())
            {
                panel1.Visible = false;
            }
        }

        public void UpdateSerie(SerieDTO serie)
        {
            serie.Comment = txtComment.Text;
            serie.SetType = (SetType) cmbSetType.SelectedIndex;
            serie.IsCiezarBezSztangi=chkCiezarBezSztangi.Checked;
            serie.DropSet = (DropSetType)cmbDropSet.SelectedIndex;
        }
    }
}
