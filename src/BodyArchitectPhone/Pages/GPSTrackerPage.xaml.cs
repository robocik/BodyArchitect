using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BodyArchitect.Portable;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Client.WCF;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Controls.Animations;
using BodyArchitect.WP7.Utils;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.Pages
{
    public partial class GPSTrackerPage : IExerciseListInvoker
    {
        private GPSTrackerViewModel viewModel;
        private DispatcherTimer timer;
        GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
        private bool timerHasBeenStarted = false;//determines if user press start button (using this we now when we start new cycle or resume after pause)
        private DateTime timerStartDateTime;
        private bool isPause = true;

        private readonly BitmapImage GpsReady = new BitmapImage(new Uri("/Images/GpsSignal.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage GpsNotReady = new BitmapImage(new Uri("/Images/GpsNoSignal.png", UriKind.RelativeOrAbsolute));
        private ApplicationBarMenuItem mnuShowMap;

        public GPSTrackerPage()
        {
            InitializeComponent();
            SetControls(progressBar, pivot);
            AnimationContext = LayoutRoot;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimerTick;
            myWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gps_changed);
            //myWatcher.MovementThreshold = .5;
            myWatcher.StatusChanged += myWatcher_StatusChanged;
        }

        protected override AnimatorHelperBase GetAnimation(AnimationType animationType, Uri toOrFrom)
        {
            if (animationType == AnimationType.NavigateForwardIn || animationType == AnimationType.NavigateBackwardIn)
                return new SlideUpAnimator() { RootElement = LayoutRoot };
            else
                return new SlideDownAnimator() { RootElement = LayoutRoot };
        }

        public bool IsPause
        {
            get { return isPause; }
            private set { isPause = value; }
        }

        public bool IsAutoPause
        {
            get { return timer.IsEnabled && IsPause; }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is TrainingDayEntrySelectorPage || e.Content is MainPage)
            {
                cleanUp();
            }
            var page = e.Content as MapPage;
            if (page != null)
            {
                page.SelectedItem = viewModel.Entry;
            }
            base.OnNavigatedFrom(e);
        }

        void cleanUp()
        {
            ApplicationState.Current.IsTimerEnabled = false;
            myWatcher.Stop();
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        new public GPSTrackerEntryDTO Entry
        {
            get { return (GPSTrackerEntryDTO)base.Entry; }
        }

        protected override bool BeforeClose()
        {
            cleanUp();
            return true;
        }

        protected override void btnDelete_Click(object sender, EventArgs e)
        {
            if (ApplicationState.Current.TrainingDay == null)
            {
                return;
            }

            if (BAMessageBox.Ask(ApplicationStrings.GPSTrackerPage_QRemoveEntry) == MessageBoxResult.OK)
            {
                deleteEntry(Entry);
            }
        }

        private void tsShowInReports_Checked(object sender, RoutedEventArgs e)
        {
            tsShowInReports.Content = viewModel.Entry.ReportStatus == ReportStatus.ShowInReport ? ApplicationStrings.ShowInReports : ApplicationStrings.HideInReports;
        }

        protected override void show(bool reload)
        {
            ensureNewEntry();
            updateButtons();
        }

        void fillInfo()
        {
            pivot.Items.Remove(tpWorkout);
            if (!pivot.Items.Contains(tpSummary))
            {
                pivot.Items.Add(tpSummary);
            }
            pivot.SelectedItem = tpSummary;
            if (viewModel.Entry.Duration.HasValue)
            {
                tbSummaryDuration.Text = viewModel.Entry.Duration.Value.ToDisplayDuration().ToString();
            }
            tbSummaryExercise.Text = viewModel.Entry.Exercise.Name;
            if (viewModel.HasDistance)
            {
                tbSummaryDistance.Text = viewModel.Entry.Distance.Value.ToDisplayDistance().ToString("0.##");
            }
            if (viewModel.HasMaxSpeed)
            {
                tbSummaryMaxSpeed.Text = viewModel.Entry.MaxSpeed.Value.ToDisplaySpeed().ToString("0.##");
            }
            if (viewModel.HasAvgSpeed)
            {
                tbSummaryAvgSpeed.Text = viewModel.Entry.AvgSpeed.Value.ToDisplaySpeed().ToString("0.##");
                tbSummaryAvgPace.Text = WilksFormula.PaceToString((float)viewModel.Entry.AvgSpeed.Value.ToDisplayPace(), true);
            }
            if (viewModel.HasMaxAlt)
            {
                tbSummaryMaxAltitude.Text = viewModel.Entry.MaxAltitude.Value.ToDisplayAltitude().ToString("0.#");
            }
            if (viewModel.HasMinAlt)
            {
                tbSummaryMinAltitude.Text = viewModel.Entry.MinAltitude.Value.ToDisplayAltitude().ToString("0.#");
            }
            if (viewModel.HasCalories)
            {
                tbSummaryCalories.Text = Math.Round(viewModel.Entry.Calories.Value).ToString();
            }
            if (viewModel.HasWeather)
            {
                var image = new BitmapImage(new Uri(string.Format("/Images/Weather/{0}", WeatherIcon.GetIcon(viewModel.Entry.Weather.Condition)), UriKind.RelativeOrAbsolute));
                imgWeather.Source = image;
                if (viewModel.Entry.Weather.Temperature.HasValue)
                {
                    tbSummaryTemperature.Text = viewModel.Entry.Weather.Temperature.Value.ToString();
                }
            }
        }
        /*final GPSTrackerEntryDTO entry=getEntry();
        GPSPointsBag existingCoordinates = ApplicationState.getCurrent().getTrainingDay().getGpsCoordinates(entry);
        boolean isAlreadySavedInLocal=false;
        if(entry.isNew())
        {
            isAlreadySavedInLocal=ApplicationState.getCurrent().getCurrentBrowsingTrainingDays().getTrainingDays().containsKey(entry.trainingDay.trainingDate);
            if(isAlreadySavedInLocal)
            {
                TrainingDayInfo dayInfo=ApplicationState.getCurrent().getCurrentBrowsingTrainingDays().getTrainingDays().get(entry.trainingDay.trainingDate);
                EntryObjectDTO entrySavedInLocal= Helper.SingleOrDefault(filter(new Predicate<EntryObjectDTO>() {
                    public boolean apply(EntryObjectDTO item) {
                        return item.instanceId.equals(entry.instanceId);
                    }
                }, dayInfo.getTrainingDay().objects));
                isAlreadySavedInLocal=entrySavedInLocal!=null;
            }
        }
        boolean emptyEntry = entry.isNew() && !isAlreadySavedInLocal || (getEntry().status.equals(WS_Enums.EntryObjectStatus.Planned) && DateTimeHelper.isToday(getEntry().trainingDay.trainingDate) && existingCoordinates == null);
        return emptyEntry;*/
        bool isEmptyEntry()
        {
            var existingCoordinates = ApplicationState.Current.TrainingDay.GetGpsCoordinates(Entry);
            bool isAlreadySavedInLocal=ApplicationState.Current.CurrentBrowsingTrainingDays.IsSaved(Entry);
            bool emptyEntry = Entry.IsNew && !isAlreadySavedInLocal || (Entry.Status == EntryObjectStatus.Planned && Entry.TrainingDay.TrainingDate.IsToday() && existingCoordinates == null);
            return emptyEntry;
        }


        void ensureNewEntry()
        {

            var emptyEntry = isEmptyEntry();

            if (emptyEntry)
            {
                if (Settings.LocationServices)
                {
                    myWatcher.Start();
                }
                else
                {
                    BAMessageBox.ShowInfo(ApplicationStrings.GPSTrackerPage_InfoLocationServiceDisabled);
                }
                var existingCoordinates = ApplicationState.Current.TrainingDay.GetGpsCoordinates(Entry);
                if (existingCoordinates == null)
                {
                    ApplicationState.Current.TrainingDay.SetGpsCoordinates(Entry, new List<GPSPoint>(), false);
                }
            }
            else
            {
                //when user press save after the training, he can no longer continue it so we can stop timer and gps
                cleanUp();
            }

            viewModel = new GPSTrackerViewModel(Entry);
            viewModel.GpsSignal = GpsNotReady;
            DataContext = viewModel;

            if (emptyEntry)
            {
                pivot.Items.Remove(tpSummary);
                if (SelectedExercise != null)
                {//set a new exercise
                    viewModel.Entry.Exercise = SelectedExercise;
                    SelectedExercise = null;
                }
                if (viewModel.Entry.Exercise != null)
                {
                    btnExercise.Content = viewModel.Entry.Exercise.Name;
                }
                pnlWorkoutData.Visibility = viewModel.Entry.Exercise != null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                fillInfo();
            }


        }


        protected override void buildApplicationBar()
        {
            base.buildApplicationBar();
            mnuShowMap = new ApplicationBarMenuItem(ApplicationStrings.GPSTrackerPage_AppMenu_ShowMap);
            mnuShowMap.Click += mnuShowMap_Click;
            ApplicationBar.MenuItems.Add(mnuShowMap);
            updateButtons();
        }

        void updateButtons()
        {
            CanSave = viewModel.Entry.Exercise != null && IsPause && (!viewModel.Entry.IsNew || viewModel.HasDuration);
            if (mnuShowMap != null)
            {
                mnuShowMap.IsEnabled = ApplicationState.Current.CurrentBrowsingTrainingDays.IsSaved(Entry);
            }
        }

        void mnuShowMap_Click(object sender, EventArgs e)
        {
            var pointsBag = ApplicationState.Current.TrainingDay.GetGpsCoordinates(viewModel.Entry);
            if (!viewModel.Entry.HasCoordinates && (pointsBag == null || pointsBag.Points.Count==0))
            {
                BAMessageBox.ShowInfo(ApplicationStrings.GPSTrackerPage_EntryWithoutGPSData);
                return;
            }
            this.Navigate("/Pages/MapPage.xaml");
        }
        protected override Type EntryType
        {
            get { return typeof(GPSTrackerEntryDTO); }
        }

        public bool TimerHasBeenStarted
        {
            get { return timerHasBeenStarted; }
            set { timerHasBeenStarted = value; }
        }

        private void startTimer(bool start)
        {
            btnStop.Visibility = start ? Visibility.Visible : Visibility.Collapsed;
            btnStart.Visibility = start ? Visibility.Collapsed : Visibility.Visible;
            if (start)
            {
                timerStartDateTime = DateTime.Now;
                //if user started workout (timer) then set application to run under lock screen
                if (Settings.RunUnderLockScreen)
                {
                    PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
                }
                if (viewModel.Entry.StartDateTime == null)
                {
                    viewModel.Entry.StartDateTime = DateTime.Now;
                }
                if (viewModel.Entry.Duration == null)
                {
                    viewModel.Entry.Duration = 0;
                }
                if (viewModel.Entry.Distance == null)
                {
                    viewModel.Entry.Distance = 0;
                }
                viewModel.Entry.Status = EntryObjectStatus.Done;

                addCurrentGpsPoint();
                autoPauseCount = 0;
            }
            else
            {
                startPause();
            }
            IsPause = !start;
            updateButtons();
        }

        private void startPause()
        {
            viewModel.Entry.EndDateTime = DateTime.Now;
            addCurrentGpsPoint();
            addNotAvailablePoint(true);
            viewModel.Entry.Duration += (decimal?)(DateTime.Now - timerStartDateTime).TotalSeconds;
            timerStartDateTime = DateTime.Now;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!TimerHasBeenStarted && !isGPSReady() && BAMessageBox.Ask(ApplicationStrings.GPSTrackerPage_InfoGpsNotReady) == MessageBoxResult.Cancel)
            {
                return;
            }
            //if (!TimerHasBeenStarted && viewModel.Entry.HasCoordinates && BAMessageBox.Ask("This entry already contains gps coordinates. Do you want to overwrite them??") == MessageBoxResult.Cancel)
            //{
            //    return;
            //}
            if (IsPause)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
            startTimer(IsPause);
            TimerHasBeenStarted = true;
        }

        private GeoPosition<GeoCoordinate> previousPosition = null;//used to calculate distance (and speed)

        decimal getCurrentDuration()
        {
            return viewModel.Entry.Duration.Value + (decimal)(DateTime.Now - timerStartDateTime).TotalSeconds;
        }

        void addNotAvailablePoint(bool isPause)
        {
            var lastPoint = viewModel.Points.LastOrDefault();
            if (lastPoint != null)
            {
                if (!lastPoint.IsNotAvailable() && !isPause)
                {
                    viewModel.Points.Add(GPSPoint.CreateNotAvailable((float)getCurrentDuration()));
                }
                else if (!lastPoint.IsPause() && isPause)
                {
                    viewModel.Points.Add(GPSPoint.CreatePause((float)getCurrentDuration()));
                }

            }
        }

        private bool gpsSource = true;

        private void addCurrentGpsPoint()
        {
            if (IsPause)
            {
                return;
            }
            if (isGPSReady())
            {
                GeoPosition<GeoCoordinate> myPosition = myWatcher.Position;
                var location = new GPSPoint((float)myPosition.Location.Latitude,
                                            (float)myPosition.Location.Longitude,
                                            (float)myPosition.Location.Altitude, CurrentSpeed.HasValue?(float)CurrentSpeed.Value:float.NaN,
                                            (float)getCurrentDuration());
                viewModel.Points.Add(location);
                viewModel.RetrieveWeather(location);
            }
            else
            {
                //mark last point as lost connection 
                addNotAvailablePoint(false);
            }
        }

        private bool autoPauseCondition;

        private int autoPauseCount = 0;


        void checkAutoPause()
        {
            if (!Settings.AutoPause || !isGPSReady())
            {//auto pause is disabled
                //if gps is lost then we shouldn't change anything in workout - if it was started then it will be
                return;
            }
            if (CurrentSpeed == 0)
            {
                if (IsPause)
                {
                    return;
                }
                autoPauseCount++;
                if (autoPauseCount >= 5)
                {
                    //we should start auto pause after 5 seconds of standing user
                    startTimer(false);
                }
                return;
            }
            if (IsAutoPause)
            {
                startTimer(true);
            }
            autoPauseCount = 0;
        }

        void addGpsPoint()
        {
            if (!Settings.LocationServices)
            {
                return;
            }
            //todo:remove this
#if !DEBUG
gpsSource=true;//for release we always take gps source
#endif

            var lastPoint = viewModel.Points.LastOrDefault();
            var currentDuration = (float)this.getCurrentDuration();
            if (lastPoint == null || currentDuration - lastPoint.Duration >= 4)
            {
                //by default we add points every 4 seconds
                if (this.isGPSReady() || gpsSource)
                {
                    addCurrentGpsPoint();
                }
                else
                {
                    Random rand = new Random();
                    viewModel.Points.Add(new GPSPoint((float)rand.NextDouble(), (float)rand.NextDouble(), 5, 6, (float)getCurrentDuration()));
                    viewModel.RetrieveWeather(new GPSPoint(50.9313049f, 17.2975941f, 3, 4, 1));
                }
            }

        }


        async protected override Task SavingCompleted()
        {

            var entry = Entry;
            if (entry == null || viewModel.Points == null || viewModel.Points.Count == 0)
            {//todo:maybe we should delete coordinates from db?
                return;
            }
            //here GPSEntry should be saved so instead InstanceId we can use GlobalId
            ApplicationState.Current.TrainingDay.CleanUpGpsCoordinates();
            ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[entry.TrainingDay.TrainingDate].CleanUpGpsCoordinates();

            try
            {
                var result = await BAService.GPSCoordinatesOperationAsync(entry.GlobalId, GPSCoordinatesOperationType.UpdateCoordinatesWithCorrection, viewModel.Points);

                result.GPSTrackerEntry.InstanceId = entry.InstanceId;
                var tdi = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[entry.TrainingDay.TrainingDate];
                var gpsBag = tdi.GetGpsCoordinates(result.GPSTrackerEntry);
                gpsBag.IsSaved = true;
                tdi.Update(result.GPSTrackerEntry);
                ApplicationState.Current.TrainingDay = tdi.Copy();
                ApplicationState.Current.CurrentEntryId = new LocalObjectKey(result.GPSTrackerEntry);
            }
            catch (NetworkException)
            {
                progressBar.ShowProgress(false);
                if (ApplicationState.Current.IsOffline)
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrOfflineMode);
                }
                else
                {
                    BAMessageBox.ShowError(ApplicationStrings.ErrNoNetwork);
                }
            }
            catch (Exception)
            {
                //mark this entry as still modified
                var tdi = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[entry.TrainingDay.TrainingDate];
                tdi.IsModified = true;
                tdi.GetGpsCoordinates(entry).IsSaved = false;
                progressBar.ShowProgress(false);
                ApplicationBar.EnableApplicationBar(true);
                BAMessageBox.ShowError(ApplicationStrings.GPSTrackerPage_ErrUploadGpsCoordinates);
            }
        }



        void OnTimerTick(object sender, EventArgs e)
        {
            slowSpeedCorrection();
            checkAutoPause();
            if (!IsPause)
            {
                addGpsPoint();
                updateTime();
            }

            showCurrentSpeed();
        }

        public decimal? CurrentSpeed { get; private set; }

        private short slowSpeedCount = 0;
        private void slowSpeedCorrection()
        {
            CurrentSpeed = null;
            if (isGPSReady())
            {
                var myPosition = myWatcher.Position;
                if (!double.IsNaN(myPosition.Location.Speed))
                {
                    CurrentSpeed = (decimal)myPosition.Location.Speed;
                    if (previousPosition != null)
                    {//sprawdz czy gdy sie nie poruszamy to czy predkosc wynosi 0. jesli nie to oblicz ja a nie bierz z gps
                        var distance = myPosition.Location.GetDistanceTo(previousPosition.Location);
                        if (distance == 0)
                        {
                            slowSpeedCount++;
                        }
                        else
                        {
                            slowSpeedCount = 0;
                        }
                    }
                    else
                    {
                        slowSpeedCount = 0;
                    }

                    if (slowSpeedCount >= 3)
                    {
                        CurrentSpeed = 0;
                    }
                }

            }
        }

        private void showCurrentSpeed()
        {
            if (CurrentSpeed.HasValue)
            {
                tbSpeed.Text = CurrentSpeed.Value.ToDisplaySpeed().ToString("0.##");
            }
            else
            {
                tbSpeed.Text = "-"; //todo: change this to something else
            }
        }

        void updateTime()
        {
            if (viewModel.Entry.StartDateTime == null)
            {
                tbTimer.Text = TimeSpan.Zero.ToString();
            }
            else
            {
                tbTimer.Text = TimeSpan.FromSeconds((int)getCurrentDuration()).ToString();
                calculateCalories();
                if (viewModel.Entry.Calories.HasValue)
                {
                    tbCalories.Text = viewModel.Entry.Calories.Value.ToString("0.##");
                }
            }

            showCurrentGpsData();
        }

        //we show gps data every second but we add a point every 4 seconds
        private void showCurrentGpsData()
        {
            if (isGPSReady())
            {
                GeoPosition<GeoCoordinate> myPosition = myWatcher.Position;
                calculateDistance(myPosition);

                tbDistance.Text = viewModel.Entry.Distance.Value.ToDisplayDistance().ToString("0.##");
                previousPosition = myPosition;
            }

        }



        void calculateDistance(GeoPosition<GeoCoordinate> myPosition)
        {
            if (IsPause)
            {
                //timer.IsEnabled means that there is no pause 
                return;
            }

            if (previousPosition != null)
            {
                viewModel.Entry.Distance += (decimal)myPosition.Location.GetDistanceTo(previousPosition.Location);
            }

        }



        void calculateCalories()
        {
            if (ApplicationState.Current.TrainingDay.TrainingDay.CustomerId.HasValue)
            {
                var customer = ApplicationState.Current.Cache.Customers.GetItem(ApplicationState.Current.TrainingDay.TrainingDay.CustomerId.Value);
                viewModel.CalculateCaloriesBurned(viewModel.Entry, getCurrentDuration(), customer);
            }
            else
            {
                viewModel.CalculateCaloriesBurned(viewModel.Entry, getCurrentDuration(), ApplicationState.Current.ProfileInfo);
            }
        }

        void myWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            viewModel.GpsSignal = isGPSReady() ? GpsReady : GpsNotReady;
        }

        private void gps_changed(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            viewModel.GpsSignal = isGPSReady() ? GpsReady : GpsNotReady;
        }

        bool isGPSReady()
        {
            if (!Settings.LocationServices || myWatcher.Status != GeoPositionStatus.Ready)
            {
                return false;
            }
            var pos = myWatcher.Position;
            return !pos.Location.IsUnknown && !double.IsNaN(pos.Location.Altitude) && pos.Location.HorizontalAccuracy < 70;

        }

        private void btnExercise_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate("/Pages/ExerciseTypePage.xaml?Selector=true&ExerciseType=" + (int)ExerciseType.Cardio);
        }

        public ExerciseDTO SelectedExercise
        {
            get;
            set;
        }

        private void btnAutoPause_Click(object sender, RoutedEventArgs e)
        {
            autoPauseCondition = !autoPauseCondition;
        }
    }
}