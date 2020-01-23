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
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class NewSetPage : SetPageBase
    {
        public NewSetPage()
        {
            InitializeComponent();
            buildApplicationBar();
            AnimationContext = LayoutRoot;
            Connect(btnStart, timePicker, ctrlTimer);
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        void buildApplicationBar()
        {
            if (EditMode)
            {
                ApplicationBarIconButton button1 =
                    new ApplicationBarIconButton(new Uri("/icons/appbar.delete.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnDelete_Click);
                button1.Text = ApplicationStrings.AppBarButton_Delete;
                ApplicationBar.Buttons.Add(button1);
            }
            ApplicationBar.IsVisible = EditMode;
        }

        protected override SetViewModel CreateViewModel(SerieDTO set)
        {
            return new SetViewModel(SelectedSet);
        }
 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete_Click();
        }

        

    }
}