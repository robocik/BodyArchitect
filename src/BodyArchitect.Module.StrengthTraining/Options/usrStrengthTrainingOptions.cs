using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BodyArchitect.Common.Plugins;
using BodyArchitect.Controls;
using BodyArchitect.Module.StrengthTraining.Localization;

namespace BodyArchitect.Module.StrengthTraining.Options
{
    [Export(typeof(IOptionsControl))]
    public partial class usrStrengthTrainingOptions : DevExpress.XtraEditors.XtraUserControl, IOptionsControl
    {
        public usrStrengthTrainingOptions()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.chkFillRepetitionsNumberFromPlan, chkFillRepetitionsNumberFromPlan.Text, StrengthTrainingEntryStrings.Options_FillRepetitionsNumberFromPlanCHK);
        }

        public void Save()
        {
            StrengthTraining.Default.FillRepetitionNumberFromPlan = chkFillRepetitionsNumberFromPlan.Checked;
            StrengthTraining.Default.ShowExtendedInfoInSets = chkExtendedSetsInfo.Checked;
        }

        public void Fill()
        {
            chkFillRepetitionsNumberFromPlan.Checked = StrengthTraining.Default.FillRepetitionNumberFromPlan;
            chkExtendedSetsInfo.Checked = StrengthTraining.Default.ShowExtendedInfoInSets;
        }

        public bool RestartRequired
        {
            get { return false; }
        }

        public string Title
        {
            get { return StrengthTrainingEntryStrings.OptionsStrengthTraining; }
        }

        public Image Image
        {
            get { return StrengthTrainingResources.StrengthTrainingModule; }
        }
    }
}
