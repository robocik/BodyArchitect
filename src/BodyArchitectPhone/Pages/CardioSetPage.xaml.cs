using System;
using System.Windows;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.ViewModel;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class CardioSetPage : SetPageBase
    {
        

        public CardioSetPage()
        {
            InitializeComponent();
            buildApplicationBar();
            Connect(btnStart, timePicker,ctrlTimer);
            AnimationContext = LayoutRoot;
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
            if (ApplicationState.Current.TrainingDay.TrainingDay.IsMine)
            {
                ApplicationBarIconButton button1 =
                    new ApplicationBarIconButton(new Uri("/icons/appbar.delete.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnDelete_Click);
                button1.Text = ApplicationStrings.AppBarButton_Delete;
                ApplicationBar.Buttons.Add(button1);
            }
        }

        protected override SetViewModel CreateViewModel(SerieDTO set)
        {
            return new CardioSessionViewModel(SelectedSet);
        }

        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete_Click();
        }

    }
}