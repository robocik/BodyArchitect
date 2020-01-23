using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7.UserControls
{
    public partial class TrainingDaySelectorControl
    {
        private ProgressStatus progressBar;
        private DateTime currentDate;
        private double durationTime = .7;

        public TrainingDaySelectorControl()
        {
            InitializeComponent();

        }

        public void PrepareTrainingDay()
        {
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine)
            {
                ApplicationState.Current.TimerStartTime = null;
                if (!ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(currentDate))
                {
                    var day = new TrainingDayDTO();
                    day.TrainingDate = currentDate;
                    day.ProfileId = ApplicationState.Current.SessionData.Profile.GlobalId;
                    if (ApplicationState.Current.CurrentViewCustomer != null)
                    {
                        day.CustomerId = ApplicationState.Current.CurrentViewCustomer.GlobalId;
                    }
                    ApplicationState.Current.TrainingDay = new TrainingDayInfo(day);
                    return;
                }
            }

            if (ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(currentDate))
            {
                ApplicationState.Current.TrainingDay = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[currentDate].Copy();
            }
            else
            {
                BAMessageBox.ShowError("Nie powinien tutaj wejsc. PrepareTrainingDay");
            }
            return;
        }

        public void SetControls(ProgressStatus progress)
        {
            progressBar = progress;
        }


        private async Task retrieveEntries(DateTime monthDate)
        {
            if (!ApplicationState.Current.CurrentBrowsingTrainingDays.IsMonthLoaded(monthDate) && !ApplicationState.Current.IsOffline)
            {
                await getCurrentTrainingDays(monthDate);

            }
        }

        private async Task getCurrentTrainingDays(DateTime monthDate)
        {
            progressBar.ShowProgress(true, ApplicationStrings.TrainingDaySelectorControl_ProgressRetrieveEntries, true, false);
            this.IsEnabled = false;
           // ApplicationState.Current.TrainingDaysRetrieved += new EventHandler<DateEventArgs>(Current_TrainingDaysRetrieved);
            await ApplicationState.Current.RetrieveMonthAsync(monthDate, ApplicationState.Current.CurrentBrowsingTrainingDays);

            this.IsEnabled = true;
            progressBar.ShowProgress(false);
        }

        //private void Current_TrainingDaysRetrieved(object sender, DateEventArgs e)
        //{
        //    fillToday();
        //    this.IsEnabled = true;
        //    progressBar.ShowProgress(false);
        //    if (ApplicationState.Current != null)
        //    {
        //        ApplicationState.Current.TrainingDaysRetrieved -= Current_TrainingDaysRetrieved;
        //    }
        //}

        public async Task Fill(DateTime date)
        {
            isOpening = false;
            currentDate = date.Date;
            await retrieveEntries(date.MonthDate());

            fillToday();
        }

        void cleanUp()
        {
            foreach (HyperlinkButton child in pnlMain.Children)
            {
                HubTile hub = (HubTile) child.Content;
                hub.Tap -= content_Tap;
            }
            pnlMain.Children.Clear();
        }
        void fillToday()
        {
            cleanUp();
            if (ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays.ContainsKey(currentDate))
            {

                var info = ApplicationState.Current.CurrentBrowsingTrainingDays.TrainingDays[currentDate];

                fillTrainingDay(info.TrainingDay);
                pnlSyncNeeded.Visibility = info.IsModified ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                pnlSyncNeeded.Visibility = System.Windows.Visibility.Collapsed;
                if (ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine)
                {

                    fillEmptyBoxes(null);
                }
            }
            
        }

        void fillTrainingDay(TrainingDayDTO day)
        {
            
            foreach (var entryObjectDto in day.Objects)
            {
                if (isSupported(entryObjectDto.GetType()))
                {
                    addButton(entryObjectDto);
                    
                }
            }
            if (day.IsMine)
            {
                fillEmptyBoxes(day);
            }
            
        }

        private HyperlinkButton addButton(EntryObjectDTO existingObject)
        {
            return addButton(existingObject.GetType(), existingObject);
        }

        private HyperlinkButton addButton(Type type)
        {
            return addButton(type, null);
        }

        HyperlinkButton addButton(Type entryObjectType, EntryObjectDTO existingObject)
        {
            HyperlinkButton link = new HyperlinkButton();
            link.Style = (Style)Application.Current.Resources["EmptyButtonStyle"];
            HubTile content = new HubTile();
            
            content.Tag=link.Tag = existingObject;
            link.Content = content;
            if (prepareButton(link, entryObjectType))
            {
                TranslateTransform transform = new TranslateTransform();
                content.RenderTransform = transform;
                pnlMain.Children.Add(link);
                content.Tap += content_Tap;
                
                return link;
            }
            return null;
        }

        static Type getEntryType(object tag)
        {
            if (tag is EntryObjectDTO)
            {
                return tag.GetType();
            }
            else
            {
                return (Type)tag;
            }
        }

        void openEntry(HubTile tile, object tag, bool forceNew)
        {
            if (tile != null)
            {
                animateControls(tile);    
            }
            
            PrepareTrainingDay();
            if (tag is EntryObjectDTO && !forceNew)
            {//open existing entry

                ApplicationState.Current.CurrentEntryId = new LocalObjectKey((EntryObjectDTO)tag);
            }
            else
            {
                addNewEntry(tag);
            }

            GoToPage(tag, this.GetParent<PhoneApplicationPage>());
        }
        private bool isOpening = false;

        void tileButtonClick(HubTile tile,object tag, bool forceNew)
        {
            try
            {
                if (isOpening)
                {
                    return;
                }
                isOpening = true;
                openEntry(tile, tag, forceNew);
            }
            catch (Exception)
            {
                isOpening = false;
                BAMessageBox.ShowError(ApplicationStrings.ErrUnhandledErrorOccured);
            }
        }
        void content_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var tile=(HubTile) sender;
            tileButtonClick(tile,tile.Tag, false);
        }

        private void mnuAddNew_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((MenuItem)sender).Tag;
            tileButtonClick(null, tag, true);
            //PrepareTrainingDay();
            //addNewEntry(tag);
            //goToPage(tag);
        }

        private void addNewEntry(object tag)
        {
            var newEntry = (EntryObjectDTO) Activator.CreateInstance(getEntryType(tag));
            ApplicationState.Current.TrainingDay.TrainingDay.Objects.Add(newEntry);
            newEntry.TrainingDay = ApplicationState.Current.TrainingDay.TrainingDay;
            fillNewEntry(newEntry);
            ApplicationState.Current.CurrentEntryId = new LocalObjectKey(newEntry.InstanceId, KeyType.InstanceId);
        }

        private void fillNewEntry(EntryObjectDTO newEntry)
        {
            var strength = newEntry as StrengthTrainingEntryDTO;
            var size = newEntry as SizeEntryDTO;
            var supple = newEntry as SuplementsEntryDTO;
            var blog = newEntry as BlogEntryDTO;
            var gps = newEntry as GPSTrackerEntryDTO;
            if (strength!=null)
            {
                strength.StartTime = DateTime.Now;
            }
            else if (size!=null)
            {
                size.Wymiary = new WymiaryDTO();
                size.Wymiary.Time.DateTime = DateTime.Now;
            }
            else if (supple!=null)
            {
                
            }
            else if (blog!=null)
            {
#if DEBUG
                throw new Exception("Blog cannot be created in WP7");
#endif
            }
            else if (gps!=null)
            {
                
            }
        }

        public static void GoToPage(object tag,PhoneApplicationPage page)
        {
            Type entryType = getEntryType(tag);
            
            if (entryType == typeof (StrengthTrainingEntryDTO))
            {
                page.Navigate("/Pages/StrengthWorkoutPage.xaml");
            }
            else if (entryType == typeof(SuplementsEntryDTO))
            {
                page.Navigate("/Pages/SupplementsPage.xaml");
            }
            else if (entryType == typeof(SizeEntryDTO))
            {
                page.Navigate("/Pages/MeasurementsPage.xaml");
            }
            else if (entryType == typeof(GPSTrackerEntryDTO))
            {
                page.Navigate("/Pages/GPSTrackerPage.xaml");
            }
            else if (entryType == typeof(BlogEntryDTO))
            {
                page.Navigate("/Pages/BlogPage.xaml");
            }
        }

        //except means tiles without animation (when we click on one tile the rest should go away but this clicked should stay)
        private void animateControls(HubTile tile, bool leaveAnimation = true)
        {
            Storyboard anim = new Storyboard();
            for (int index = 0; index < pnlMain.Children.Count; index++)
            {
                HyperlinkButton button = (HyperlinkButton)pnlMain.Children[index];
                if (tile==null || tile != button.Content)
                {//add to all button except this clicked
                    addButtonAnimation(button, anim, index, leaveAnimation);
                }
            }
            anim.Begin();
        }
        public void AnimateLeaveAll()
        {
            animateControls(null);
        }

        public void AnimateArriveAll()
        {
            animateControls(null,false);
        }

        private void addButtonAnimation(HyperlinkButton link, Storyboard anim, int currentIndex,bool leaveAnimation)
        {
            HubTile hub = (HubTile) link.Content;
            var leftSide = currentIndex%2==0;

            var trans = hub.RenderTransform;
            var x = new DoubleAnimation();
            x.Duration = new Duration(TimeSpan.FromSeconds(durationTime));
            //x.From = !leftSide?-80:80;
            if (leaveAnimation)
            {
                x.From = 0;
                x.To = leftSide ? -500 : 500;    
            }
            else
            {
                x.From = leftSide ? -500 : 500;
                x.To = 0;
            }
            


            anim.Children.Add(x);
            Storyboard.SetTarget(x, trans);
            Storyboard.SetTargetProperty(x, new PropertyPath("X"));

            //for first and last row add also vertical animation
            if (currentIndex == 0 || currentIndex == 1 || currentIndex==pnlMain.Children.Count-1 || currentIndex==pnlMain.Children.Count-2)
            {
                bool isFirstRow = currentIndex == 0 || currentIndex == 1;
                var y = new DoubleAnimation();
                y.Duration = new Duration(TimeSpan.FromSeconds(durationTime));
                if (leaveAnimation)
                {
                    y.From = 0;
                    y.To = isFirstRow ? -500 : 500;    
                }
                else
                {
                    y.From = isFirstRow ? -500 : 500;
                    y.To = 0;
                }
                
                anim.Children.Add(y);
                Storyboard.SetTarget(y, trans);
                Storyboard.SetTargetProperty(y, new PropertyPath("Y"));
            }
        }

        bool prepareButton(HyperlinkButton button, Type obj)
        {
            
            var hub = (HubTile)button.Content;
            if (button.Tag == null)
            {
                hub.Background = Application.Current.Resources["CustomDisabledBrush"] as Brush;
                hub.Tag = obj;
            }

            if (obj == typeof(StrengthTrainingEntryDTO))
            {
                hub.Source = new BitmapImage(new Uri("/Images/strengthTrainingTile.jpg", UriKind.RelativeOrAbsolute));
                hub.Title = ApplicationStrings.TrainingDaySelectorControl_StrengthTraining;
                if (button.Tag != null)
                {
                    hub.Background = EntryObjectColors.StrengthTraining;
                    hub.Message = getStrengthTrainingMessage((StrengthTrainingEntryDTO) button.Tag);
                }
            }else if (obj == typeof(SizeEntryDTO))
            {
                hub.Source = new BitmapImage(new Uri("/Images/sizesTile.jpg", UriKind.RelativeOrAbsolute));
                hub.Title = ApplicationStrings.TrainingDaySelectorControl_Measurements;
                if (button.Tag != null)
                {
                    hub.Background = EntryObjectColors.Measurements;
                    hub.Message = getMeasurementsMessage((SizeEntryDTO)button.Tag);
                }
            }else if (obj == typeof(BlogEntryDTO))
            {
                hub.Source = new BitmapImage(new Uri("/Images/blogTile.jpg", UriKind.RelativeOrAbsolute));
                hub.Title = ApplicationStrings.TrainingDaySelectorControl_Blog;
                if (button.Tag != null)
                {
                    hub.Background = EntryObjectColors.Blog;
                    hub.Message = getBlogMessage((BlogEntryDTO)button.Tag);
                }
            }else if (obj == typeof(SuplementsEntryDTO))
            {
                hub.Source = new BitmapImage(new Uri("/Images/suppleTile.jpg", UriKind.RelativeOrAbsolute));
                hub.Title = ApplicationStrings.TrainingDaySelectorControl_Supplements;
                if (button.Tag != null)
                {
                    hub.Background = EntryObjectColors.Supplements;
                    hub.Message = getSupplementsMessage((SuplementsEntryDTO)button.Tag);
                }
            }else if (obj == typeof(GPSTrackerEntryDTO))
            {
                hub.Source = new BitmapImage(new Uri("/Images/Bicycling.jpg", UriKind.RelativeOrAbsolute));
                hub.Title = ApplicationStrings.TrainingDaySelectorControl_GPSTracker;
                if (button.Tag != null)
                {
                    hub.Background = EntryObjectColors.GPSTracker;
                    hub.Message = getGpsTrackerMessage((GPSTrackerEntryDTO)button.Tag);
                }
            }
            else
            {
                return false;
            }
            EntryObjectDTO entryObj = (EntryObjectDTO) button.Tag;
            if (entryObj != null)
            {
                hub.Background.Opacity = entryObj.Status == EntryObjectStatus.Planned ? 0.4 : 1;
            }
            return true;
        }

        

        void fillEmptyBoxes(TrainingDayDTO day)
        {
            var type = typeof (StrengthTrainingEntryDTO);
            if (day==null || day.Objects.Where(x => x.GetType() == type).Count() == 0)
            {
                addButton(type);
            }
            type = typeof(SizeEntryDTO);
            if (day == null || day.Objects.Where(x => x.GetType() == type).Count() == 0)
            {
                addButton(type);
            }
            type = typeof(SuplementsEntryDTO);
            if (day == null || day.Objects.Where(x => x.GetType() == type).Count() == 0)
            {
                addButton(type);
            }
            type = typeof(GPSTrackerEntryDTO);
            if (day == null || day.Objects.Where(x => x.GetType() == type).Count() == 0)
            {
                addButton(type);
            }
        }

        bool isSupported(Type type)
        {
            return type != typeof (A6WEntryDTO);
        }


        string getBlogMessage(BlogEntryDTO blog)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("FINISH");
            return builder.ToString();
        }

        string getSupplementsMessage(SuplementsEntryDTO supple)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in supple.Items)
            {
                builder.AppendLine(string.Format("{0}: {1:0.##} {2}", item.Suplement.Name, item.Dosage, EnumLocalizer.Default.Translate(item.DosageType)));
            }
            return builder.ToString();
        }

        string getMeasurementsMessage(SizeEntryDTO size)
        {
            StringBuilder builder = new StringBuilder();
            if (size.Wymiary != null)
            {
                if (size.Wymiary.RightBiceps > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_RightBiceps + ": " + size.Wymiary.RightBiceps.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.LeftBiceps > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_LeftBiceps + ": " + size.Wymiary.LeftBiceps.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.Klatka > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_Chest + ": " + size.Wymiary.Klatka.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.RightForearm > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_RightForearm + ": " + size.Wymiary.RightForearm.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.LeftForearm > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_LeftForearm + ": " + size.Wymiary.LeftForearm.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.RightUdo > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_RightLeg + ": " + size.Wymiary.RightUdo.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.LeftUdo > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_LeftLeg + ": " + size.Wymiary.LeftUdo.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.Pas > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_Abs + ": " + size.Wymiary.Pas.ToDisplayLength().ToString("0.##"));
                }
                if (size.Wymiary.Weight > 0)
                {
                    builder.AppendLine(ApplicationStrings.MeasurementsControl_Weight + ": " + size.Wymiary.Weight.ToDisplayWeight().ToString("0.##"));
                }
            }
            return builder.ToString();
        }

        string getStrengthTrainingMessage(StrengthTrainingEntryDTO strength)
        {
            Dictionary<ExerciseType, int> exerciseCounter = new Dictionary<ExerciseType, int>();

            foreach (StrengthTrainingItemDTO list in strength.Entries)
            {
                var exerciseType = list.Exercise.ExerciseType;
                if (!exerciseCounter.ContainsKey(exerciseType))
                {
                    exerciseCounter.Add(exerciseType, 0);
                }
                exerciseCounter[exerciseType] = exerciseCounter[exerciseType] + list.Series.Count;
            }
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<ExerciseType, int> keyValuePair in exerciseCounter)
            {
                builder.AppendLine(EnumLocalizer.Default.Translate(keyValuePair.Key) + ": " + keyValuePair.Value);
            }
            return builder.ToString();
        }


        private string getGpsTrackerMessage(GPSTrackerEntryDTO gpsEntry)
        {
            StringBuilder builder = new StringBuilder();
            UIHelper ui = new UIHelper();
            builder.AppendLine(gpsEntry.Exercise.Name);
            if (gpsEntry.Distance.HasValue)
            {
                builder.AppendFormat("{2}: {0:0.#} {1}\r\n", gpsEntry.Distance.Value.ToDisplayDistance(), ui.DistanceType,ApplicationStrings.GPSTrackerPage_Distance);
            }
            if (gpsEntry.Duration.HasValue)
            {
                builder.AppendFormat("{1}: {0}\r\n", gpsEntry.Duration.Value.ToDisplayDuration(), ApplicationStrings.GPSTrackerPage_ShortDuration);
            }
            if (gpsEntry.Calories.HasValue)
            {
                builder.AppendFormat("{1}: {0:0} kcal", gpsEntry.Calories.Value, ApplicationStrings.GPSTrackerPage_Calories);
            }
            return builder.ToString();
        }

        private void TrainingDaySelectorControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            AnimateArriveAll();
        }



        #region Secondary live tile

        Uri getImageUri(Type entryType)
        {
            if (entryType == typeof (SuplementsEntryDTO))
            {
                return new Uri("/Images/suppleTile.jpg", UriKind.RelativeOrAbsolute);
            }
            else if (entryType == typeof (BlogEntryDTO))
            {
                return new Uri("/Images/blogTile.jpg", UriKind.RelativeOrAbsolute);
            }
            else if (entryType == typeof (GPSTrackerEntryDTO))
            {
                return new Uri("/Images/Bicycling.jpg", UriKind.RelativeOrAbsolute);
            }
            else if (entryType == typeof (SizeEntryDTO))
            {
                return new Uri("/Images/sizesTile.jpg", UriKind.RelativeOrAbsolute);
            }
            return new Uri("/Images/strengthTrainingTile.jpg", UriKind.RelativeOrAbsolute);
        }
        private void mnuPinToStart_Click(object sender, RoutedEventArgs e)
        {
            var tag = (HubTile)((MenuItem)sender).Tag;
            var type = getEntryType(tag.Tag);
            ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("EntryType=" + type.Name));
            if (Tile != null)
            {
                BAMessageBox.ShowInfo(ApplicationStrings.TrainingDaySelectorControl_ErrSecondaryLiveTileExists);
                return;
            }
            StandardTileData tileData = new StandardTileData
            {
                Title =tag.Title,
                BackgroundImage = getImageUri(type),
                BackBackgroundImage = new Uri("/WP7TileImage.png",UriKind.RelativeOrAbsolute),
                BackTitle = "BodyArchitect"
            };

            Uri tileUri = new Uri("/Pages/MainPage.xaml?EntryType=" + type.Name, UriKind.Relative);
            ShellTile.Create(tileUri, tileData);
        }

        public void DeepLink(string entryType)
        {
            Type type = null;
            if (entryType == "SuplementsEntryDTO")
            {
                type = typeof(SuplementsEntryDTO);
            }
            else if (entryType == "StrengthTrainingEntryDTO")
            {
                type = typeof(StrengthTrainingEntryDTO);
            }
            else if (entryType == "GPSTrackerEntryDTO")
            {
                type = typeof(GPSTrackerEntryDTO);
            }
            else if (entryType == "SizeEntryDTO")
            {
                type = typeof(SizeEntryDTO);
            }
            else if (entryType == "BlogEntryDTO")
            {
                type = typeof(BlogEntryDTO);
            }

            PrepareTrainingDay();
            var todayDay = ApplicationState.Current.TrainingDay;
            var existingEntries = todayDay.TrainingDay.GetEntries(type);
            if (existingEntries.Count > 0)
            {
                ApplicationState.Current.CurrentEntryId = new LocalObjectKey(existingEntries.First().GlobalId, KeyType.GlobalId);
            }
            else
            {
                addNewEntry(type);
            }
            GoToPage(type, this.GetParent<PhoneApplicationPage>());
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu) sender;
            if (!ApplicationState.Current.CurrentBrowsingTrainingDays.IsMine)
            {
                menu.Visibility = Visibility.Collapsed;
                return;
            }
            if (!currentDate.IsToday())
            {
                MenuItem pinToStart = (MenuItem)menu.Items[1];
                pinToStart.Visibility = Visibility.Collapsed;    
            }

        }

        #endregion
    }
}
