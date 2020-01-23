using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using BodyArchitect.Service.V2.Model.Validators;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Controls.Model;
using BodyArchitect.WP7.Pages;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7
{
    public partial class StrengthWorkoutPage :  IExerciseListInvoker
    {
        private StrengthWorkoutViewModel _viewModel;
        private ApplicationBarMenuItem mnuTimer;
        private ApplicationBarMenuItem mnuReorder;

        public StrengthWorkoutPage()
        {
            InitializeComponent();
            SetControls(progressBar, pivot);
            headerTrainingDate.Text = ApplicationStrings.BlogViewModel_LoadComments_Loading;
            //headerOldTrainingDate.Text = ApplicationStrings.BlogViewModel_LoadComments_Loading;
            AnimationContext = LayoutRoot;
            
            headerOldTrainingDate.Visibility =  System.Windows.Visibility.Visible;

        }


        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (pivot.SelectedIndex == 0)
            {
                if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new TurnstileFeatherBackwardOutAnimator() {ListBox = lsItems, RootElement = LayoutRoot};
            }
            else
            {
                if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                    return new SlideUpAnimator() { RootElement = LayoutRoot };
                else
                    return new SlideDownAnimator() { RootElement = LayoutRoot };
            }
        }

        private void btnAddExercise_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/ExerciseTypePage.xaml?Selector=true");
        }

        protected override void PrepareCopiedEntry(EntryObjectDTO origEntry,EntryObjectDTO entry)
        {
            StrengthTrainingEntryDTO strength = entry as StrengthTrainingEntryDTO;
            if (strength != null)
            {
                var oldStrength = (StrengthTrainingEntryDTO) origEntry;
                copyStrengthTrainingProperties(strength, oldStrength);

                TrainingBuilder builder = new TrainingBuilder();
                builder.PrepareCopiedStrengthTraining(strength,Settings.CopyStrengthTrainingMode);
                builder.SetPreviewSets(oldStrength, strength);
            }
        }

        override protected void buildApplicationBar()
        {
            base.buildApplicationBar();
            
            if ( !ReadOnly && pivot.SelectedIndex == 0)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.add.rest.png", UriKind.Relative));
                button1.Click += new EventHandler(btnAddExercise_Click);
                button1.Text = ApplicationStrings.AppBarButton_Add;
                ApplicationBar.Buttons.Add(button1);
            }
            var menuItem = new ApplicationBarMenuItem(ApplicationStrings.AppBarButton_More);
            menuItem.Click += new EventHandler(mnuMore_Click);
            ApplicationBar.MenuItems.Add(menuItem);

            if (_viewModel.EditMode)
            {
                mnuTimer =new ApplicationBarMenuItem(ApplicationState.Current.IsTimerEnabled? ApplicationStrings.AppBarButton_StopTimer: ApplicationStrings.AppBarButton_StartTimer);
                mnuTimer.Click += new EventHandler(mnuTimer_Click);
                ApplicationBar.MenuItems.Add(mnuTimer);

                mnuReorder = new ApplicationBarMenuItem(this.lsItems.IsReorderEnabled ? ApplicationStrings.AppBarButton_StopReorder : ApplicationStrings.AppBarButton_StartReorder);
                mnuReorder.Click += new EventHandler(mnuReorder_Click);
                ApplicationBar.MenuItems.Add(mnuReorder);
            }

            if(!ApplicationState.Current.IsOffline)
            {
                var mnuShowProgress = new ApplicationBarMenuItem(ApplicationStrings.MeasurementsPage_ShowProgress);
                mnuShowProgress.Click += new EventHandler(mnuShowProgress_Click);
                ApplicationBar.MenuItems.Add(mnuShowProgress);
            }
        }

        private void mnuReorder_Click(object sender, EventArgs e)
        {
            this.lsItems.IsReorderEnabled = !this.lsItems.IsReorderEnabled;
            mnuReorder.Text = this.lsItems.IsReorderEnabled
                                  ? ApplicationStrings.AppBarButton_StopReorder
                                  : ApplicationStrings.AppBarButton_StartReorder;
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
            this.Navigate("/Pages/ExercisesWeightReportPage.xaml");
        }

        void mnuTimer_Click(object sender, EventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_StrengthTrainingTimer,this))
            {
                return;
            }
            startTimer(!ApplicationState.Current.IsTimerEnabled);
        }

        private void startTimer(bool start)
        {
            ApplicationState.Current.IsTimerEnabled = start;
            if (ApplicationState.Current.TimerStartTime == null)
            {
                ApplicationState.Current.TimerStartTime = DateTime.Now;    
            }
            
            ctrlTimer.IsStarted = ApplicationState.Current.IsTimerEnabled;
        }


        protected override void copyAllImplementation(EntryObjectDTO oldEntry)
        {
            if (oldEntry != null)
            {
                //var copyDay = oldDay.TrainingDay.StrengthWorkout.Copy(true);

                //PrepareCopiedEntry(oldDay.TrainingDay.StrengthWorkout,copyDay);


                var itemsToCopy = lstOldItems.SelectedItems.Cast<StrengthTrainingItemViewModel>().Select(x => x.Item).ToList();

                var strength = Entry;
                var oldStrength = (StrengthTrainingEntryDTO)oldEntry;

                copyStrengthTrainingProperties(strength, oldStrength);

                TrainingBuilder builder = new TrainingBuilder();
                foreach (var itemDto in itemsToCopy)
                {
                    var newItem = itemDto.Copy(true);
                    builder.PrepareCopiedStrengthTraining(newItem, Settings.CopyStrengthTrainingMode);
                    if (Settings.CopyStrengthTrainingMode != CopyStrengthTrainingMode.OnlyExercises)
                    {
                        builder.SetPreviewSets(itemDto, newItem);
                    }
                    strength.AddEntry(newItem);
                }
                builder.CleanSingleSupersets(Entry);

                StrengthWorkoutViewModel.ResetPositions(Entry);
                show( true);

                lblNoExercises.Visibility = Entry.Entries.Count == 0? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;
            }
            
        }

        private void copyStrengthTrainingProperties(StrengthTrainingEntryDTO strength, StrengthTrainingEntryDTO oldStrength)
        {
            if (strength.Intensity == Intensity.NotSet)
            {//if intensity is not set then we assume that user didn't set this property so we can copy all other values from previous day
                strength.Intensity = oldStrength.Intensity;
                strength.ReportStatus = oldStrength.ReportStatus;
                //strength.MyPlace = oldStrength.MyPlace;//uncommnt this or not:)
                strength.TrainingPlanId = oldStrength.TrainingPlanId;
                strength.TrainingPlanItemId = oldStrength.TrainingPlanItemId;
            }
        }

        private void mnuUserExercise_Click(object sender, RoutedEventArgs e)
        {
            var item = (StrengthTrainingItemViewModel)(sender as FrameworkElement).DataContext;
            
            List<StrengthTrainingItemDTO> itemsToCopy = new List<StrengthTrainingItemDTO>();
            if (Settings.TreatSuperSetsAsOne && !string.IsNullOrEmpty(item.Item.SuperSetGroup))
            {//if we should treat superset as one then when we copy one exercise which is a part of superset then we should copy all of them
                itemsToCopy.AddRange(item.Item.StrengthTrainingEntry.Entries.Where(x => x.SuperSetGroup == item.Item.SuperSetGroup).OrderBy(x => x.Position));
            }
            else
            {
                itemsToCopy.Add(item.Item);
            }

            foreach (var itemDto in itemsToCopy)
            {
                var newItem = itemDto.Copy(true);
                TrainingBuilder builder = new TrainingBuilder();
                builder.PrepareCopiedStrengthTraining(newItem, Settings.CopyStrengthTrainingMode);
                builder.SetPreviewSets(itemDto,newItem);
                Entry.AddEntry(newItem);
            }
            //_viewModel.AddExercise(item.Item.Exercise);

            lblNoExercises.Visibility = Entry.Entries.Count == 0? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;

            StrengthWorkoutViewModel.ResetPositions(Entry);
            show( true);
            pivot.SelectedIndex = 0;
        }

        void mnuMore_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/StrengthTrainingOptionsPage.xaml");
        }

        new public StrengthTrainingEntryDTO Entry
        {
            get { return (StrengthTrainingEntryDTO)base.Entry; }
        }

        protected override Type EntryType
        {
            get { return typeof (StrengthTrainingEntryDTO); }
        }

        protected override void show( bool reload)
        {
            //if (ApplicationState.Current.TrainingDay.TrainingDay.StrengthWorkout == null)
            //{
            //    StrengthTrainingEntryDTO entry = new StrengthTrainingEntryDTO();
            //    entry.StartTime = DateTime.Now;
            //    ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(entry);
            //    entry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
            //}
            _viewModel = new StrengthWorkoutViewModel(Entry);
            Connect(pivot, headerOldTrainingDate, _viewModel.ShowOldTraining,lstOldItems);
            DataContext = _viewModel;
            headerTrainingDate.Text = _viewModel.TrainingDate;

            StrengthTrainingItemViewModel itemToScroll = null;
            if (SelectedExercise != null)
            {//adding a new exercise
                itemToScroll = _viewModel.AddExercise(SelectedExercise);
                SelectedExercise = null;

            }
            if (itemToScroll != null)
            {
                lsItems.UpdateLayout();
                lsItems.ScrollIntoView(itemToScroll);
            }
            //if (ApplicationState.Current.ExercisesReposidory.ExerciseLoaded || ApplicationState.Current.TrainingDay.StrengthWorkout.Entries.Count == 0)
            //{
                
            //}
            //else
            //{
            //    progressBar.ShowProgress(true, ApplicationStrings.StrengthWorkoutPage_ProgressRetrieveExercises);
            //    ApplicationState.Current.ExercisesReposidory.LoadedExercises += (s, a) =>
            //    {
            //        if (ApplicationState.Current.TrainingDay != null)
            //        {
            //            if (ApplicationState.Current.ExercisesReposidory.ExerciseLoaded)
            //            {
            //                _viewModel =
            //                    new StrengthWorkoutViewModel(ApplicationState.Current.TrainingDay.StrengthWorkout);
            //                DataContext = _viewModel;
            //                headerTrainingDate.Text = _viewModel.TrainingDate;
            //            }
            //            else
            //            {
            //                BAMessageBox.ShowError(ApplicationStrings.ExerciseTypeViewModel_ErrRetrieveExercises);
            //            }
            //        }
            //        progressBar.ShowProgress(false);
            //    };
            //    ApplicationState.Current.ExercisesReposidory.LoadExercises();
            //}

            if (ReadOnly && pivot.Items.Count == 3)
            {//remove preview tab when we looking at other user trainings or we are in read only mode
                pivot.Items.RemoveAt(2);
            }

            tsEntryStatus.Visibility = Entry.Status != EntryObjectStatus.System
                                           ? System.Windows.Visibility.Visible
                                           : System.Windows.Visibility.Collapsed;
            lblNoExercises.Visibility = Entry.Entries.Count == 0
                                            ? System.Windows.Visibility.Visible
                                            : System.Windows.Visibility.Collapsed;
        }

        
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            StrengthWorkoutItemPage page = e.Content as StrengthWorkoutItemPage;
            if (page != null)
            {
                page.SelectedItem = _viewModel.SelectedItem.Item;
            }
            var cardioPage = e.Content as CardioStrengthTrainingItemPage;
            if (cardioPage != null)
            {
                cardioPage.SelectedItem = _viewModel.SelectedItem.Item;
            }
            //var reportPage = e.Content as ExercisesWeightReportPage;
            //if (reportPage != null)
            //{
                
            //}
            if (page != null)
            {
                page.SelectedItem = _viewModel.SelectedItem.Item;
            }
            if (e.Content is TrainingDayEntrySelectorPage || e.Content is MainPage)
            {
                ApplicationState.Current.IsTimerEnabled = false;
            }
            base.OnNavigatedFrom(e);

            ctrlTimer.IsStarted = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ctrlTimer.IsStarted = _viewModel.EditMode &&  ApplicationState.Current.IsTimerEnabled;

            if (!ctrlTimer.IsStarted && ApplicationState.Current.IsPremium && Settings.StartTimer && Entry.IsNew)
            {
                startTimer(true);
            }
        }

        public ExerciseDTO SelectedExercise
        {
            get; set;
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ReadOnly)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            var item =(StrengthTrainingItemViewModel)(sender as FrameworkElement).DataContext;
            _viewModel.Delete(item);
            lsItems.SelectedIndex = -1;
            //DataContext = null;
            DataContext = _viewModel;
            lblNoExercises.Visibility = Entry.Entries.Count == 0? System.Windows.Visibility.Visible: System.Windows.Visibility.Collapsed;
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = false;
            ContextMenu menu = (ContextMenu) sender;
            menu.Visibility = ReadOnly ? Visibility.Collapsed : System.Windows.Visibility.Visible;

            if (menu.Items.Count == 2)
            {//menu in exercises tab
                StrengthTrainingItemViewModel item = (StrengthTrainingItemViewModel) menu.Tag;
                ((MenuItem) menu.Items[1]).Visibility = string.IsNullOrEmpty(item.Item.SuperSetGroup)
                                                            ? Visibility.Collapsed
                                                            : System.Windows.Visibility.Visible;
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            LayoutRoot.IsHitTestVisible = true;
        }


        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.TrainingDay==null)
            {
                return;
            }
            if (ReadOnly)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            if (BAMessageBox.Ask(ApplicationStrings.StrengthWorkoutPage_RemoveEntry) == MessageBoxResult.OK)
            {
                deleteEntry(Entry);
            }
        }


        private void lsItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _viewModel.SelectedItem = (StrengthTrainingItemViewModel)e.AddedItems[0];
                if (_viewModel.SelectedItem.Exercise != ExerciseDTO.Deleted)
                {
                    if (_viewModel.SelectedItem.Exercise.ExerciseType == ExerciseType.Cardio)
                    {
                        this.Navigate("/Pages/CardioStrengthTrainingItemPage.xaml");
                    }
                    else
                    {
                        this.Navigate("/Pages/StrengthWorkoutItemPage.xaml");
                    }
                    return;
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.StrengthWorkoutPage_ErrCannotViewDeletedExercise);
                }

            }
            lsItems.SelectedIndex = -1;
        }

        

        private void btnEndTraining_Click(object sender, RoutedEventArgs e)
        {
            tpEndTraining.Value = DateTime.Now;
        }

        private void mnuRemoveSuperSet_Click(object sender, RoutedEventArgs e)
        {
            if (ReadOnly)
            {
                BAMessageBox.ShowError(ApplicationStrings.ErrCannotModifyEntriesOfAnotherUser);
                return;
            }
            var item = (StrengthTrainingItemViewModel)(sender as FrameworkElement).DataContext;
            _viewModel.RemoveSuperSet(item);
        }

        protected override void BeforeSaving()
        {
            if (Entry != null && Entry.EndTime==null)
            {
                Entry.EndTime = DateTime.Now;
            }
        }

        protected override bool ValidateBeforeSave()
        {
            var incorrectSets = SerieValidator.GetIncorrectSets(Entry);

            if (incorrectSets.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(ApplicationStrings.StrengthTraining_MsgIncorrectSetsValidation);
                foreach (var incorrectSet in incorrectSets.GroupBy(x => x.StrengthTrainingItem.Exercise))
                {
                    builder.Append(incorrectSet.Key.DisplayExercise + ": ");
                    foreach (var serieDto in incorrectSet)
                    {
                        builder.Append(serieDto.GetDisplayText() + ", ");
                    }
                    builder.AppendLine();
                }
                if (BAMessageBox.Ask(builder.ToString()) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void tsEntryStatus_Checked(object sender, RoutedEventArgs e)
        {
            tsEntryStatus.Content = _viewModel.Entry.Status == EntryObjectStatus.Done ? ApplicationStrings.EntryStatusDone : ApplicationStrings.EntryStatusPlanned;
        }

        private void CtrlTimer_OnIsStartedChanged(object sender, EventArgs e)
        {
            if (mnuTimer != null)
            {
                mnuTimer.Text = ctrlTimer.IsStarted
                                    ? ApplicationStrings.AppBarButton_StopTimer
                                    : ApplicationStrings.AppBarButton_StartTimer;
            }
        }

        private void lsItems_ListReordered(object sender, ListReorderedEventArgs e)
        {
            Entry.Entries.RemoveAt(e.FromIndex);
            Entry.Entries.Insert(e.ToIndex, ((StrengthTrainingItemViewModel)e.Object).Item);
            StrengthWorkoutViewModel.ResetPositions(Entry);
            _viewModel.Refresh();
        }

        private void lsItems_ListReordering(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!UpgradeAccountControl.EnsureAccountType(ApplicationStrings.Feature_Premium_MoreFeatures, this))
            {
                e.Cancel = true;
            }
        }

        

    }
}