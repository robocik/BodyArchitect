using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BodyArchitect.Client.Common.Plugins;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI.Controls;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    [Export(typeof(IOptionsControl))]
    public partial class usrStrengthTrainingOptions : IOptionsControl
    {
        public usrStrengthTrainingOptions()
        {
            InitializeComponent();
        }

        public void Save()
        {
            StrengthTraining.Default.FillRepetitionNumberFromPlan = chkFillRepetitionsNumberFromPlan.IsChecked.Value;
            StrengthTraining.Default.ShowExtendedInfoInSets = chkExtendedSetsInfo.IsChecked.Value;
        }

        public void Fill()
        {
            chkFillRepetitionsNumberFromPlan.IsChecked = StrengthTraining.Default.FillRepetitionNumberFromPlan;
            chkExtendedSetsInfo.IsChecked = StrengthTraining.Default.ShowExtendedInfoInSets;
        }

        public bool RestartRequired
        {
            get { return false; }
        }

        public string Title
        {
            get { return StrengthTrainingEntryStrings.OptionsStrengthTraining; }
        }

        public ImageSource Image
        {
            get
            {
                return
                    "pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/StrengthTraining.png".ToBitmap();
            }
        }
    }
}
