using System;
using System.Windows.Data;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for TrainingPlansView.xaml
    /// </summary>
    public partial class TrainingPlansView 
    {
        public TrainingPlansView()
        {
            InitializeComponent();

        }

        public override AccountType AccountType
        {
            get { return AccountType.PremiumUser; }
        }

        public override void Fill()
        {
            Header = "TrainingPlansView_Fill_Header_TrainingPlans".TranslateStrength();
            if (PageContext == null)
            {
                PageContext = new PageContext();
            }
            usrPlans.Fill(PageContext);
            if(PageContext.SelectedItem.HasValue && WorkoutPlansReposidory.Instance.GetItem(PageContext.SelectedItem.Value)==null)
            {
                usrPlans.Fill(PageContext.SelectedItem.Value);
            }
        }

        public override void RefreshView()
        {
            usrPlans.RefreshView();
        }


        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/WorkoutPlan16.png", UriKind.Absolute); }
        }
    }
}
