using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;
using BodyArchitect.WP7.UserControls;
using BodyArchitect.WP7.ViewModel;
using BodyArchitectCustom;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BodyArchitect.WP7
{
    public partial class ExerciseTypePage : IExerciseListInvoker
    {
        private ExerciseTypeViewModel _viewModel;

        public ExerciseTypePage()
        {
            InitializeComponent();
            //LongList.GroupViewClosing += new EventHandler<GroupViewClosingEventArgs>(LongList_GroupViewClosing);
            //LongList.GroupViewOpened += new EventHandler<GroupViewOpenedEventArgs>(LongList_GroupViewOpened);
            buildApplicationBar();
        }

        void buildApplicationBar()
        {

            var button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnRefresh_Click);
            button1.Text = ApplicationStrings.AppBarButton_Refresh;
            button1.IsEnabled = !ApplicationState.Current.IsOffline;
            ApplicationBar.Buttons.Add(button1);

            button1 = new ApplicationBarIconButton(new Uri("/icons/appbar.feature.settings.rest.png", UriKind.Relative));
            button1.Click += new EventHandler(btnSettings_Click);
            button1.Text = ApplicationStrings.AppBarButton_Settings;
            button1.IsEnabled = !ApplicationState.Current.IsOffline;
            ApplicationBar.Buttons.Add(button1);

            
        }

        

        

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            LongList.GroupViewOpen(e);
        }

        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            LongList.GroupViewClose(e);
        }

        //void LongList_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        //{
        //    ItemContainerGenerator itemContainerGenerator = e.ItemsControl.ItemContainerGenerator;
        //    TurnstileTransition turnstileTransition = new TurnstileTransition();
        //    turnstileTransition.Mode = TurnstileTransitionMode.ForwardIn;

        //    int itemCount = e.ItemsControl.Items.Count;
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        UIElement element = itemContainerGenerator.ContainerFromIndex(i) as UIElement;
        //        ITransition animation = turnstileTransition.GetTransition(element);
        //        animation.Begin();
        //    }
        //}

        //void LongList_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        //{
        //    // set event as handled so that the group view is not closed right away
        //    // and the animations that we will start can be seen
        //    SwivelTransition transition = new SwivelTransition();
        //    ItemContainerGenerator itemContainerGenerator = e.ItemsControl.ItemContainerGenerator;

        //    int animationFinished = 0;
        //    int itemCount = e.ItemsControl.Items.Count;
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        UIElement element = itemContainerGenerator.ContainerFromIndex(i) as UIElement;

        //        ITransition animation = transition.GetTransition(element);
        //        animation.Completed += delegate
        //        {
        //            // close the group view when all animations have completed
        //            if ((++animationFinished) == itemCount)
        //            {
        //                LongList.CloseGroupView();

        //                //We have to scroll the to the selected group because we have handled the event 
        //                LongList.ScrollToGroup(e.SelectedGroup);
        //            }
        //        };
        //        animation.Begin();
        //    }
        //}

        public ExerciseTypeViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(SelectedExercise!=null )
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            
            if (ViewModel == null || !ApplicationState.Current.Cache.Exercises.IsLoaded)
            {
                LongList.ItemsSource = null;
                _viewModel=new ExerciseTypeViewModel();
                string strType;
                if (NavigationContext.QueryString.TryGetValue("Selector", out strType))
                {
                    ViewModel.SelectionMode = bool.Parse(strType);
                }
                string exerciseType = null;
                if (NavigationContext.QueryString.TryGetValue("ExerciseType", out exerciseType))
                {
                    ViewModel.ExerciseType = (ExerciseType)int.Parse(exerciseType);
                }
                DataContext = ViewModel;

                ViewModel.ExercisesLoaded += exerciseLoaded;
                progressBar.ShowProgress(true, ApplicationStrings.StrengthWorkoutPage_ProgressRetrieveExercises);
                ViewModel.LoadExercises();
            }
            else
            {
                ViewModel.LoadExercises();
                restoreListPosition();
            }
        }

        void exerciseLoaded(object sender,EventArgs e)
        {
            LongList.ItemsSource = ViewModel.GroupedExercises;
            //restoreListPosition();
            progressBar.ShowProgress(false);
            //ViewModel.ExercisesLoaded -= exerciseLoaded;
        }

        private void restoreListPosition()
        {
            try
            {
                StateHelper stateHelper = new StateHelper(this.State);
                var item = stateHelper.GetValue<Guid>("SelectedExercise", Guid.Empty);
                var gr = stateHelper.GetValue<string>("SelectedGroup", null);
                if (item != Guid.Empty)
                {
                    LongList.UpdateLayout();
                    var exerciseToScroll = ViewModel.GetExerciseView(item);
                    LongList.ScrollTo(exerciseToScroll);
                }
                else if (gr != null)
                {
                    //LongList.UpdateLayout();
                    var t = from g in (ObservableCollection<GroupingLayer<string, ExerciseViewModel>>)LongList.ItemsSource where g.Key == gr select g;
                    var tt = t.Single();
                    LongList.ScrollToGroup(tt);
                }
            }
            catch
            {
                LongList.ItemsSource = null;
                LongList.ItemsSource = ViewModel.GroupedExercises;
            }
            
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            var globalId = (Guid)btn.Tag;
            SelectedExercise = ApplicationState.Current.Cache.Exercises.GetItem(globalId);
            //NavigationService.Navigate(new Uri("/StrengthWorkoutPage.xaml?ExerciseId=" + globalId, UriKind.Relative));)
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is IExerciseListInvoker)
                (e.Content as IExerciseListInvoker).SelectedExercise = SelectedExercise;
            base.OnNavigatedFrom(e);
            var list =LongList.GetItemsWithContainers(true, false);
            if (list.Count>0)
            {//GroupingLayer<string, ExerciseViewModel>
                object first = list.First();
                var group = first as GroupingLayer<string, ExerciseViewModel>;
                var exercise = first as ExerciseViewModel;
                if (group!=null)
                {
                    //State["SelectedGroup"] = group.Key;
                    State["SelectedExercise"] = ((ExerciseViewModel)list.ElementAt(1)).Exercise.GlobalId;
                }
                else if (exercise!=null)
                {
                    State["SelectedExercise"] = exercise.Exercise.GlobalId;
                }
                //State["SelectedExercise"] = ((ExerciseViewModel) list.First()).Exercise.GlobalId;
                
            }
            //LongList
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            progressBar.ShowProgress(true, ApplicationStrings.StrengthWorkoutPage_ProgressRetrieveExercises);
            LongList.ItemsSource = null;
            _viewModel.Refresh();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            this.Navigate("/Pages/SettingsPage.xaml");
        }

        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0]!=null)
            {

                var exercise = (ExerciseViewModel)((LongListSelector)sender).SelectedItem;
                this.Navigate("/Pages/ExerciseViewPage.xaml?ExerciseId=" + exercise.Exercise.GlobalId);
            }

            //LongList.SelectedIndex = -1;
        }

        public ExerciseDTO SelectedExercise
        {
            get; set;
        }
    }


    
}