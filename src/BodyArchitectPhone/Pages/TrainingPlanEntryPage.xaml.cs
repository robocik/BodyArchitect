using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class TrainingPlanEntryPage : AnimatedBasePage
    {
        private TrainingPlanEntryViewModel viewModel;

        public TrainingPlanEntryPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (pivot.SelectedIndex == 0)
            {
                if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new TurnstileFeatherBackwardOutAnimator() { ListBox = lstSets, RootElement = LayoutRoot };
            }
            else
            {
                if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.State["PivotSelectedTab"] = pivot.SelectedIndex;
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string strGlobalId=NavigationContext.QueryString["GlobalId"];
            viewModel=new TrainingPlanEntryViewModel(ApplicationState.Current.CurrentTrainingPlan.GetEntry(new Guid(strGlobalId)));
            lblTitle.Text = viewModel.Exercise.Name;
            DataContext = viewModel;

            StateHelper stateHelper = new StateHelper(this.State);
            var pivotItem = stateHelper.GetValue<int>("PivotSelectedTab", 0);
            pivot.SelectedIndex = pivotItem;
        }
    }
}