using System;
using BodyArchitect.Client.Module.StrengthTraining.Localization;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Controls;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.StrengthTraining.Controls
{
    /// <summary>
    /// Interaction logic for ExercisesView.xaml
    /// </summary>
    public partial class ExercisesView
    {
        public ExercisesView()
        {
            InitializeComponent();
        }

        public override AccountType AccountType
        {
            get { return Service.V2.Model.AccountType.PremiumUser; }
        }

        public override void Fill()
        {
            Header = EnumLocalizer.Default.GetUIString("BodyArchitect.Client.Module.StrengthTraining:StrengthTrainingEntryStrings:ExercisesView_Fill_Header_Exercises");
            if (PageContext==null)
            {
                PageContext=new PageContext();
            }
            usrExercisesView.Fill(PageContext);
        }

        public override void RefreshView()
        {
            usrExercisesView.RefreshView();
        }

        public override Uri HeaderIcon
        {
            get { return new Uri("pack://application:,,,/BodyArchitect.Client.Module.StrengthTraining;component/Images/Exercises.png", UriKind.Absolute); }
        }
    }
}
