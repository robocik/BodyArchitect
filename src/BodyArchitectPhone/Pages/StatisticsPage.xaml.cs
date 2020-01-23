using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public partial class StatisticsPage : AnimatedBasePage
    {
        private StatisticsViewModel viewModel;

        public StatisticsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn)
                return null;//new TurnstileFeatherForwardInAnimator() { ListBox = lstStat, RootElement = LayoutRoot };
            else
                return new TurnstileFeatherBackwardOutAnimator() { ListBox = lstStat, RootElement = LayoutRoot };
        }

        public UserSearchDTO User { get; set; }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            StateHelper stateHelper = new StateHelper(this.State);
            User = stateHelper.GetValue("User", User);
            viewModel = new StatisticsViewModel(User);
            DataContext = viewModel;
        }
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            State["User"] = User;
        }
    }

    
}