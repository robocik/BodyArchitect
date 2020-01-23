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
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using Microsoft.Phone.Controls;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class StrengthTrainingItemOptionsPage
    {
        public StrengthTrainingItemOptionsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StateHelper stateHelper = new StateHelper(this.State);
            var item = stateHelper.GetValue<Guid>("SelectedItemId", Guid.Empty);
            var comment = stateHelper.GetValue<string>("CommentText", null);
            if (item != Guid.Empty && (CommentableObject == null || CommentableObject.InstanceId == item))
            {
                //CommentableObject = (ICommentable)ApplicationState.Current.TrainingDay.TrainingDay.StrengthWorkout.GetItem(item);
                CommentableObject = (ICommentable)Entry.GetItem(item);
                if (comment != null)
                {
                    CommentableObject.Comment = comment;
                }
            }
            ctrlTimer.IsStarted = EditMode && ApplicationState.Current.IsTimerEnabled;
            DataContext = this;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ExtensionMethods.BindFocusedTextBox();
            ctrlTimer.IsStarted = false;
            base.OnNavigatedFrom(e);

            this.State["SelectedItemId"] = CommentableObject.InstanceId;
            this.State["CommentText"] = CommentableObject.Comment;
        }

        public StrengthTrainingItemDTO Item
        {
            get { return (StrengthTrainingItemDTO)CommentableObject; }
        }

        public int SelectedEquipment
        {
            get { return (int)Item.DoneWay; }
            set { Item.DoneWay = (ExerciseDoneWay) value; }
        }
        public ICommentable CommentableObject { get; set; }

        public bool CanEditEquipment
        {
            get { return ApplicationState.Current.IsPremium; }
        }

        public bool ShowEquipmentHelp
        {
            get { return !ApplicationState.Current.IsPremium; }
        }

        private void btnEquipmentHelp_Click(object sender, RoutedEventArgs e)
        {
            UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_Equipments, this);
        }
    }
}