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
using BodyArchitect.WP7.Controls.Animations;
using ExtensionMethods = BodyArchitect.WP7.Controls.ExtensionMethods;

namespace BodyArchitect.WP7.Pages
{
    public partial class CommentPage
    {
        public CommentPage()
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
            var comment = stateHelper.GetValue<string>("CommentText",null);
            if (item != Guid.Empty && (CommentableObject == null || CommentableObject.InstanceId == item))
            {

                //todo:StrengthWorkout here?
                CommentableObject = (ICommentable)Entry.GetItem(item);
                if(comment!=null)
                {
                    CommentableObject.Comment = comment;
                }
            }
            
            DataContext = this;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ExtensionMethods.BindFocusedTextBox();
            base.OnNavigatedFrom(e);
            
            this.State["SelectedItemId"] = CommentableObject.InstanceId;
            this.State["CommentText"] = CommentableObject.Comment;
        }

        public ICommentable CommentableObject { get; set; }

    }
}