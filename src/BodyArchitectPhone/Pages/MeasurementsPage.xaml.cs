using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class MeasurementsPage
    {
        private MeasurementsViewModel viewModel;

        public MeasurementsPage()
        {
            InitializeComponent();
            AnimationContext = LayoutRoot;
            SetControls(progressBar,pivot);
        }

        new public SizeEntryDTO Entry
        {
            get { return (SizeEntryDTO) base.Entry; }
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        protected override void show( bool reload)
        {
            //if (ApplicationState.Current.TrainingDay.TrainingDay.Size == null)
            //{
            //    var entry = new SizeEntryDTO();
            //    entry.Wymiary = new WymiaryDTO();
            //    entry.Wymiary.Time.DateTime = DateTime.Now;
            //    ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entry);
            //    entry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
            //}

            viewModel = new MeasurementsViewModel(Entry);
            sizeCtrl.ReadOnly = !viewModel.EditMode;
            timeCtrl.ReadOnly = !viewModel.EditMode;
            DataContext = viewModel;
            sizeCtrl.Fill(Entry.Wymiary, ApplicationState.Current.CurrentBrowsingTrainingDays);
            //
            header.Text = viewModel.TrainingDate;
        }

        protected override Type EntryType
        {
            get { return typeof(SizeEntryDTO); }
        }

        protected override void buildApplicationBar()
        {
            base.buildApplicationBar();
            if (!ApplicationState.Current.IsOffline)
            {
                var mnuShowProgress = new ApplicationBarMenuItem(ApplicationStrings.MeasurementsPage_ShowProgress);
                mnuShowProgress.Click += new EventHandler(mnuShowProgress_Click);
                ApplicationBar.MenuItems.Add(mnuShowProgress);
            }
        }

        void mnuShowProgress_Click(object sender, EventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_Reports, this))
            {
                return;
            }
            if (isModified() && BAMessageBox.Ask(ApplicationStrings.StrengthWorkoutPage_QShowProgressForModifiedEntry) == MessageBoxResult.Cancel)
            {
                return;
            }
            this.Navigate("/Pages/MeasurementsReportPage.xaml");
        }

        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.TrainingDay == null)
            {
                return;
            }
            if (BAMessageBox.Ask(ApplicationStrings.MeasurementsPage_QRemoveMeasurements) == MessageBoxResult.OK)
            {
                deleteEntry(Entry);
            }
        }

        private void tsShowInReports_Checked(object sender, RoutedEventArgs e)
        {
            tsShowInReports.Content = viewModel.Entry.ReportStatus == ReportStatus.ShowInReport ? ApplicationStrings.ShowInReports : ApplicationStrings.HideInReports;
        }

        private void tsEntryStatus_Checked(object sender, RoutedEventArgs e)
        {
            tsEntryStatus.Content = viewModel.Entry.Status == EntryObjectStatus.Done ? ApplicationStrings.EntryStatusDone : ApplicationStrings.EntryStatusPlanned;
        }

        
    }
}