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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using Microsoft.Phone.Controls;

namespace BodyArchitect.WP7.Pages
{
    public class FeatureItem
    {
        public FeatureItem(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }

        public string Description { get; set; }
    }

    public partial class BasicAccountTypeDescriptionPage
    {
        
        public BasicAccountTypeDescriptionPage()
        {
            InitializeComponent();
        }

        List<FeatureItem> getBasicAccountFeatures()
        {
            List<FeatureItem> list = new List<FeatureItem>();
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_UnlimitedEntriesInCalendar, ApplicationStrings.Feature_Basic_UnlimitedEntriesInCalendar_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_ExercisesDatabase, ApplicationStrings.Feature_Basic_ExercisesDatabase_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_WorkoutPlansDatabase, ApplicationStrings.Feature_Basic_WorkoutPlansDatabase_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_SupplementsCycleDefinitions, ApplicationStrings.Feature_Basic_SupplementsCycleDefinitions_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_MeasurementsInCalendar, ApplicationStrings.Feature_Basic_MeasurementsInCalendar_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_BlogInCalendar, ApplicationStrings.Feature_Basic_BlogInCalendar_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_SupplementsInCalendar, ApplicationStrings.Feature_Basic_SupplementsInCalendar_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_A6WInCalendar, ApplicationStrings.Feature_Basic_A6WInCalendar_Description));

            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_AccessToOtherUsersCalendar, ApplicationStrings.Feature_Basic_AccessToOtherUsersCalendar_Description));

            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_SearchUsers, ApplicationStrings.Feature_Basic_SearchUsers_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Basic_ExercisesRecords, ApplicationStrings.Feature_Basic_ExercisesRecords_Description));
            

            
            return list;
        }

        List<FeatureItem> getPremiumAccountFeatures()
        {
            List<FeatureItem> list = new List<FeatureItem>();
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_Reports, ApplicationStrings.Feature_Premium_Reports_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_CreatingExercises, ApplicationStrings.Feature_Premium_CreatingExercises_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_CreatingWorkoutPlans, ApplicationStrings.Feature_Premium_CreatingWorkoutPlans_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_CreatingSupplementsDefinitions, ApplicationStrings.Feature_Premium_CreatingSupplementsDefinitions_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_SupportsMyTrainingsForSupplements, ApplicationStrings.Feature_Premium_SupportsMyTrainingsForSupplements_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_AddEntriesInFuture, ApplicationStrings.Feature_Premium_AddEntriesInFuture_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_Reminders, ApplicationStrings.Feature_Premium_Reminders_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_PrivateCalendar, ApplicationStrings.Feature_Premium_PrivateCalendar_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_Printing, ApplicationStrings.Feature_Premium_Printing_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_MyPlaces, ApplicationStrings.Feature_Premium_MyPlaces_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_Equipments, ApplicationStrings.Feature_Premium_Equipments_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_StrengthTrainingTimer, ApplicationStrings.Feature_Premium_StrengthTrainingTimer_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_AdvancedStrengthTraining, ApplicationStrings.Feature_Premium_AdvancedStrengthTraining_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_NoAds, ApplicationStrings.Feature_Premium_NoAds_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Premium_MoreFeatures, ApplicationStrings.Feature_Premium_MoreFeatures_Description));
            return list;
        }

        List<FeatureItem> getInstructorAccountFeatures()
        {
            List<FeatureItem> list = new List<FeatureItem>();
            list.Add(new FeatureItem(ApplicationStrings.Feature_Instructor_Customers, ApplicationStrings.Feature_Instructor_Customers_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Instructor_CalendarForCustomers, ApplicationStrings.Feature_Instructor_CalendarForCustomers_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Instructor_ScheduleEntries, ApplicationStrings.Feature_Instructor_ScheduleEntries_Description));
            list.Add(new FeatureItem(ApplicationStrings.Feature_Instructor_Championships, ApplicationStrings.Feature_Instructor_Championships_Description));

            return list;
        }

        private void BodyArchitectPage_Loaded(object sender, RoutedEventArgs e)
        {
            string account;
            int accountId=(int) AccountType.User;
            if (NavigationContext.QueryString.TryGetValue("Account", out account))
            {
                int.TryParse(account, out accountId);
            }
            PageTitle.Text = EnumLocalizer.Default.Translate((AccountType)accountId).ToLower();
            List<FeatureItem> features = null;
            if(accountId==1)
            {
                features = getPremiumAccountFeatures();
                tbDescription.Text = ApplicationStrings.AccountTypePage_PremiumAccount_Description;
            }
            else if (accountId == 2)
            {
                features = getInstructorAccountFeatures();
                tbDescription.Text = ApplicationStrings.AccountTypePage_InstructorAccount_Description;
            }
            else
            {
                features = getBasicAccountFeatures();
                tbDescription.Text = ApplicationStrings.AccountTypePage_BasicAccount_Description;
            }

            buildControls(features);
            startPageAnimation();

        }

        private void buildControls(List<FeatureItem> features)
        {
            foreach (var featureItem in features)
            {
                StackPanel panel = new StackPanel();
                panel.Opacity = 0;
                StackPanel panel1=new StackPanel();
                panel1.Orientation = System.Windows.Controls.Orientation.Horizontal;
                panel1.Margin=new Thickness(12,0,0,0);
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("/Images/Feature32.png", UriKind.Relative));
                panel1.Children.Add(img);
                TextBlock tbTitle = new TextBlock();
                tbTitle.FontSize = (double) Application.Current.Resources["CustomFontSizeMedium"];
                tbTitle.Foreground = (Brush) Application.Current.Resources["CustomAccentFullBrush"];
                tbTitle.FontFamily = (FontFamily) Application.Current.Resources["CustomFontFamilyNormal"];
                tbTitle.FontWeight = FontWeights.Bold;
                tbTitle.Text = featureItem.Title;
                tbTitle.Style = (Style) Application.Current.Resources["CustomTextTitle3Style"];
                panel1.Children.Add(tbTitle);
                panel.Children.Add(panel1);
                TextBlock tbDescription = new TextBlock();
                tbDescription.FontSize = (double)Application.Current.Resources["CustomFontSizeMedium"];
                tbDescription.Foreground = (Brush) Application.Current.Resources["splashScreenBrush"];
                tbDescription.Margin=new Thickness(12,12,12,36);
                tbDescription.TextWrapping = TextWrapping.Wrap;
                tbDescription.Style = (Style)Application.Current.Resources["CustomTextSmallStyle"];
                tbDescription.Text = featureItem.Description;
                panel.Children.Add(tbDescription);
                mainList.Children.Add(panel);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            destroyPageAnimation();

            base.OnBackKeyPress(e);

        }

        void destroyPageAnimation()
        {
            Storyboard anim = new Storyboard();

            for (int index = 0; index < mainList.Children.Count; index++)
            {
                var child = mainList.Children[index];
                var x = new DoubleAnimation();
                var temp=index%4;
                x.Duration = new Duration(TimeSpan.FromSeconds(0.2 + temp * 0.2));
                x.From = 1;
                x.To = 0;
                anim.Children.Add(x);
                Storyboard.SetTarget(x, child);
                Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));
            }
            //var x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g1);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.3);
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g2);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g3);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.3);
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g4);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g5);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.3);
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g6);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g7);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.3);
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g8);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g9);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.5);
            //x.From = 1;
            //x.To = 0;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, btnBuy);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));



            anim.Begin();
            anim.Completed += delegate
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                anim.Stop();
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }

            };
        }

        void startPageAnimation()
        {
            Storyboard anim = new Storyboard();

            for (int index = 0; index < mainList.Children.Count; index++)
            {
                var child = mainList.Children[index];
                var x = new DoubleAnimation();
                x.Duration = new Duration(TimeSpan.FromSeconds(0.2+index*0.3));
                x.From = 0;
                x.To = 1;
                anim.Children.Add(x);
                Storyboard.SetTarget(x, child);
                Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));
            }
            

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.6);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g2);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.7);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g3);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.8);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g4);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(0.9);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g5);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(1);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g6);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(1.1);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g7);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(1.2);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g8);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            //x = new DoubleAnimation();
            //x.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            //x.BeginTime = TimeSpan.FromSeconds(1.2);
            //x.From = 0;
            //x.To = 1;
            //anim.Children.Add(x);
            //Storyboard.SetTarget(x, g9);
            //Storyboard.SetTargetProperty(x, new PropertyPath("Opacity"));

            anim.Begin();
            //anim.Completed += delegate
            //{
            //    LayoutRoot.Visibility = Visibility.Collapsed;
            //    anim.Stop();
            //    if (NavigationService.CanGoBack)
            //    {
            //        NavigationService.GoBack();
            //    }

            //};
        }
    }
}