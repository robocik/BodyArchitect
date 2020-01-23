using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;

namespace BodyArchitect.WP7.Pages
{
    public partial class StrengthTrainingOptionsPage 
    {
        private ICollection<MyPlaceLightDTO> myPlaces;
        private MyPlaceLightDTO selectedMyPlace;

        public StrengthTrainingOptionsPage()
        {
            InitializeComponent();
            DataContext = this;
            AnimationContext = LayoutRoot;
        }

        

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        public bool EditMode
        {
            get { return Entry.TrainingDay.IsMine && Entry.Status != EntryObjectStatus.System; }
        }

        public bool CanChangeMyPlace
        {
            get { return EditMode && ApplicationState.Current.IsPremium; }
        }

        public bool ShowMyPlacesHelp
        {
            get { return !ApplicationState.Current.IsPremium; }
        }

        public ICollection<MyPlaceLightDTO> MyPlaces
        {
            get { return myPlaces; }
            set
            {
                myPlaces = value;
                NotifyPropertyChanged("MyPlaces");
            }
        }


        public MyPlaceLightDTO SelectedMyPlace
        {
            get { return selectedMyPlace; }
            set
            {
                selectedMyPlace = value;
                Entry.MyPlace = SelectedMyPlace;
                NotifyPropertyChanged("SelectedMyPlace");
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ctrlTimer.IsStarted = false;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!EditMode)
            {//when we browser other user calendar we shouldn't retrieve my places
                MyPlaces = new List<MyPlaceLightDTO>();
                MyPlaces.Add(Entry.MyPlace);
                SelectedMyPlace = Entry.MyPlace;
                return;
            }
            ctrlTimer.IsStarted = EditMode && ApplicationState.Current.IsTimerEnabled;

            if (!ApplicationState.Current.Cache.MyPlaces.IsLoaded)
            {
                ApplicationState.Current.Cache.MyPlaces.Loaded += OnMyPlacesOnLoaded;
                progressBar.ShowProgress(true, ApplicationStrings.MsgRetrievingMyPlaces);
                ApplicationState.Current.Cache.MyPlaces.Load();
            }
            else
            {
                fillMyPlaces();
            }
        }

        private void fillMyPlaces()
        {
            MyPlaces = ApplicationState.Current.Cache.MyPlaces.Items.Values.Cast<MyPlaceLightDTO>().ToList();
            var currentPlace = Entry.MyPlace;
            if(currentPlace!=null)
            {
                SelectedMyPlace=ApplicationState.Current.Cache.MyPlaces.GetItem(currentPlace.GlobalId);
            }
            else
            {
                SelectedMyPlace = ApplicationState.Current.Cache.MyPlaces.Items.Values.Where(x=>x.IsDefault).Single();
            }
        }

        private void OnMyPlacesOnLoaded(object s, EventArgs a)
        {
            ApplicationState.Current.Cache.MyPlaces.Loaded -= OnMyPlacesOnLoaded;
            if (ApplicationState.Current.Cache.MyPlaces.IsLoaded)
            {
                fillMyPlaces();
            }
            else
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotRetrieveMyPlaces);
            }
            progressBar.ShowProgress(false);
        }

        private void btnMyPlacesHelp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_MyPlaces, this);
        }

        private void tsShowInReports_Checked(object sender, RoutedEventArgs e)
        {
            tsShowInReports.Content = Entry.ReportStatus == ReportStatus.ShowInReport ? ApplicationStrings.ShowInReports : ApplicationStrings.HideInReports;
        }
    }
}